/*************************************************************************
 *
 * DO NOT ALTER OR REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER
 * 
 * Copyright 2008 Sun Microsystems, Inc. All rights reserved.
 * 
 * Use is subject to license terms.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy
 * of the License at http://www.apache.org/licenses/LICENSE-2.0. You can also
 * obtain a copy of the License at http://odftoolkit.org/docs/license.txt
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * 
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 ************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using AODL.Document.Content;
using AODL.Document.Content.Draw;
using AODL.Document.Content.EmbedObjects;
using AODL.Document.Content.Tables;
using AODL.Document.Custom;
using AODL.Document.Exceptions;
using AODL.Document.Export;
using AODL.Document.Import;
using AODL.Document.Import.OpenDocument;
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.Styles;
using AODL.Document.TextDocuments;

namespace AODL.Document.SpreadsheetDocuments
{
    /// <summary>
    /// The SpreadsheetDocument class represent an OpenDocument
    /// spreadsheet document.
    /// </summary>
    public class SpreadsheetDocument : IDocument, IDisposable
    {
        private readonly IList<Graphic> _graphics;
        private bool _isLoadedFile;
        private const string _mimeTyp = "application/vnd.oasis.opendocument.spreadsheet";
        private int _tableCount;
        private readonly CustomFileCollection _customFiles;
        private XDocument _xmldoc;

        private IImporter _importer;

        private readonly StyleFactory m_styleFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadsheetDocument"/> class.
        /// </summary>
        public SpreadsheetDocument()
        {
            TableCollection = new TableCollection();
            Styles = new StyleCollection();
            m_styleFactory = new StyleFactory(this);
            CommonStyles = new StyleCollection();
            Content = new ContentCollection();
            _graphics = new List<Graphic>();
            FontList = new List<string>();
            EmbedObjects = new List<EmbedObject>();
            TableCollection.Inserted += TableCollection_Inserted;
            TableCollection.Removed += TableCollection_Removed;
            Content.Inserted += Content_Inserted;
            Content.Removed += Content_Removed;

            _customFiles = new CustomFileCollection();
            _customFiles.Clearing += CustomFiles_Clearing;
            _customFiles.Inserting += CustomFiles_Inserting;
            _customFiles.Removing += CustomFiles_Removing;
        }

        /// <summary>
        /// Gets the table count.
        /// </summary>
        /// <value>The table count.</value>
        public int TableCount
        {
            get { return _tableCount; }
        }

        /// <summary>
        /// Gets or sets the table collection.
        /// </summary>
        /// <value>The table collection.</value>
        public TableCollection TableCollection { get; set; }

        /// <summary>
        /// Gets or sets the document styes.
        /// </summary>
        /// <value>The document styes.</value>
        public DocumentStyles DocumentStyles { get; set; }

        /// <summary>
        /// Gets or sets the document setting.
        /// </summary>
        /// <value>The document setting.</value>
        public DocumentSetting DocumentSetting { get; set; }

        /// <summary>
        /// Gets or sets the document manifest.
        /// </summary>
        /// <value>The document manifest.</value>
        public DocumentManifest DocumentManifest { get; set; }

        /// <summary>
        /// Gets the MIME typ.
        /// </summary>
        /// <value>The MIME typ.</value>
        public string MimeTyp
        {
            get { return _mimeTyp; }
        }

        #region IDocument Members

        public IImporter Importer
        {
            get { return _importer; }
        }

        public StyleFactory StyleFactory
        {
            get { return m_styleFactory; }
        }

        /// <summary>
        /// The xml document the spreadsheet document based on.
        /// </summary>
        public XDocument XmlDoc
        {
            get { return _xmldoc; }
            set { _xmldoc = value; }
        }

        /// <summary>
        /// If this file was loaded
        /// </summary>
        /// <value></value>
        public bool IsLoadedFile
        {
            get { return _isLoadedFile; }
        }

        /// <summary>
        /// The font list
        /// </summary>
        /// <value></value>
        public IList<string> FontList { get; set; }

        /// <summary>
        /// Collection of local styles used with this document.
        /// </summary>
        /// <value></value>
        public StyleCollection Styles { get; set; }

        /// <summary>
        /// Collection of common styles used with this document.
        /// </summary>
        /// <value></value>
        public StyleCollection CommonStyles { get; set; }

        /// <summary>
        /// Collection of contents used by this document.
        /// </summary>
        /// <value></value>
        public ContentCollection Content { get; set; }

        /// <summary>
        /// Collection of custom files to include in package
        /// </summary>
        public CustomFileCollection CustomFiles
        {
            get { return _customFiles; }
        }

        /// <summary>
        /// Gets or sets the document metadata.
        /// </summary>
        /// <value>The document metadata.</value>
        public DocumentMetadata DocumentMetadata { get; set; }

        /// <summary>
        /// Gets or sets the document configuration2.
        /// </summary>
        /// <value>The document configuration2.</value>
        public DocumentConfiguration2 DocumentConfigurations2 { get; set; }

        /// <summary>
        /// Gets or sets the document pictures.
        /// </summary>
        /// <value>The document pictures.</value>
        public DocumentPictureCollection DocumentPictures { get; set; }

        /// <summary>
        /// Gets or sets the document thumbnails.
        /// </summary>
        /// <value>The document thumbnails.</value>
        public DocumentPictureCollection DocumentThumbnails { get; set; }

        /// <summary>
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public IList<Graphic> Graphics
        {
            get { return _graphics; }
        }

        /// <summary>
        /// Gets the embedobject.
        /// </summary>
        /// <value>The embedobject.</value>
        public IList<EmbedObject> EmbedObjects { get; set; }

        /// <summary>
        /// Save the document by using the passed IExporter
        /// with the passed file name.
        /// </summary>
        /// <param name="filename">The name of the new file.</param>
        /// <param name="exporter"></param>
        public void Save(string filename, IExporter exporter)
        {
            CreateContentBody();
            exporter.Export(this, filename);
        }

        /// <summary>
        /// Load the given file.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="importer"></param>
        public void Load(string file, IImporter importer)
        {
            _importer = importer;
            _isLoadedFile = true;
            LoadBlankContent();

            if (_importer != null)
            {
                if (_importer.NeedNewOpenDocument)
                    New();
                _importer.Import(this, file);

                if (_importer.ImportError != null)
                    if (_importer.ImportError.Count > 0)
                        foreach (AODLWarning ob in _importer.ImportError)
                        {
                            if (ob.Message != null)
                                Console.WriteLine("Err: {0}", ob.Message);
                            if (ob.Node != null)
                            {
                                using (XmlWriter writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
                                {
                                    ob.Node.WriteTo(writer);
                                }
                            }
                        }
            }
        }

        #endregion

        /// <summary>
        /// Create a new blank spreadsheet document.
        /// </summary>
        public void New()
        {
            LoadBlankContent();

            DocumentConfigurations2 = new DocumentConfiguration2();

            DocumentManifest = new DocumentManifest();
            DocumentManifest.New();

            DocumentMetadata = new DocumentMetadata(this);
            DocumentMetadata.New();

            DocumentPictures = new DocumentPictureCollection();

            DocumentSetting = new DocumentSetting();
            DocumentSetting.New();

            DocumentStyles = new DocumentStyles();
            DocumentStyles.New();
            ReadCommonStyles();

            DocumentThumbnails = new DocumentPictureCollection();
        }

        /// <summary>
        /// Reads the common styles.
        /// </summary>
        private void ReadCommonStyles()
        {
            OpenDocumentImporter odImporter = new OpenDocumentImporter(null) {Document = this};
            odImporter.ImportCommonStyles();

            LocalStyleProcessor lsp = new LocalStyleProcessor(this, true);
            lsp.ReadStyles();
        }

        /// <summary>
        /// Load a blank the spreadsheet content document.
        /// </summary>
        private void LoadBlankContent()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            Stream str = ass.GetManifestResourceStream("AODL.Resources.OD.spreadsheetcontent.xml");
            using (XmlReader reader = XmlReader.Create(str))
            {
                _xmldoc = XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Tables the collection_ inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void TableCollection_Inserted(int index, object value)
        {
            _tableCount++;
            if (!Content.Contains(value as IContent))
                Content.Add(value as IContent);
        }

        /// <summary>
        /// Tables the collection_ removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void TableCollection_Removed(int index, object value)
        {
            _tableCount--;
            if (Content.Contains(value as IContent))
                Content.Remove(value as IContent);
        }

        /// <summary>
        /// Content_s the inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Inserted(int index, object value)
        {
            if (value is Table)
                if (!TableCollection.Contains((Table) value))
                    TableCollection.Add(value as Table);
        }

        /// <summary>
        /// Content_s the removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Removed(int index, object value)
        {
            if (value is Table)
                if (TableCollection.Contains((Table) value))
                    TableCollection.Remove(value as Table);
        }

        /// <summary>
        /// Creates the content body.
        /// </summary>
        private void CreateContentBody()
        {
            XElement nodeSpreadSheets = XmlDoc.Elements(Ns.Office + "document-content")
                .Elements(Ns.Office + "body")
                .Elements(Ns.Office + "spreadsheet").First();

            foreach (IContent iContent in Content)
            {
                //only table content
                if (iContent is Table)
                    nodeSpreadSheets.Add(((Table) iContent).BuildNode());
            }

            CreateLocalStyleContent();
            CreateCommonStyleContent();
        }

        /// <summary>
        /// Creates the content of the local style.
        /// </summary>
        private void CreateLocalStyleContent()
        {
            XElement nodeAutomaticStyles = XmlDoc.Elements(Ns.Office + "document-content")
                .Elements(Ns.Office + "automatic-styles").First();

            foreach (IStyle style in Styles.ToValueList())
                nodeAutomaticStyles.Add(style.Node);
        }

        /// <summary>
        /// Creates the content of the common style.
        /// </summary>
        private void CreateCommonStyleContent()
        {
            XElement nodeCommonStyles = DocumentStyles.Styles.Elements(Ns.Office + "document-styles")
                .Elements(Ns.Office + "styles").First();
            nodeCommonStyles.Value = "";

            foreach (IStyle style in CommonStyles.ToValueList())
            {
                nodeCommonStyles.Add(new XElement(style.Node));
            }

            //Remove styles node
            nodeCommonStyles = XmlDoc.Elements(Ns.Office + "document-content")
                .Elements(Ns.Office + "styles").FirstOrDefault();
            if (nodeCommonStyles != null)
                nodeCommonStyles.Remove();
        }

        public EmbedObject GetObjectByName(string objectName)
        {
            for (int i = 0; i < EmbedObjects.Count; i++)
            {
                if ((EmbedObjects[i]).ObjectName == objectName)

                    return (EmbedObject) EmbedObjects[i];
            }

            return null;
        }

        private void CustomFiles_Clearing()
        {
            XElement root = DocumentManifest.Manifest.Element(Ns.Manifest + "manifest");
            foreach (ICustomFile customFile in _customFiles)
            {
                IEnumerable<XElement> fileEntries =
                    root.Elements(Ns.Manifest + "file-entry").Where(
                        e =>
                        ((string)e.Attribute(Ns.Manifest + "media-type")).Equals(customFile.MediaType ?? string.Empty,
                                                                                  StringComparison.InvariantCulture) &&
                        ((string)e.Attribute(Ns.Manifest + "full-path")).Equals(customFile.FullPath,
                                                                                 StringComparison.InvariantCulture));
                foreach (XElement fileEntry in fileEntries)
                {
                    fileEntry.Remove();
                }
            }
        }

        private void CustomFiles_Inserting(int index, ICustomFile value)
        {
            XElement root = DocumentManifest.Manifest.Element(Ns.Manifest + "manifest");
            IEnumerable<XElement> fileEntries =
                root.Elements(Ns.Manifest + "file-entry").Where(
                    e =>
                    ((string)e.Attribute(Ns.Manifest + "media-type")).Equals(value.MediaType ?? string.Empty,
                                                                              StringComparison.InvariantCulture) &&
                    ((string)e.Attribute(Ns.Manifest + "full-path")).Equals(value.FullPath,
                                                                             StringComparison.InvariantCulture));
            if (fileEntries.Count() > 0)
                return;
            XElement fileEntry = new XElement(Ns.Manifest + "file-entry");
            fileEntry.SetAttributeValue(Ns.Manifest + "media-type", value.MediaType ?? string.Empty);
            fileEntry.SetAttributeValue(Ns.Manifest + "full-path", value.FullPath);
            root.Add(fileEntry);
        }

        private void CustomFiles_Removing(int index, ICustomFile value)
        {
            XElement root = DocumentManifest.Manifest.Element(Ns.Manifest + "manifest");
            IEnumerable<XElement> fileEntries =
                root.Elements(Ns.Manifest + "file-entry").Where(
                    e =>
                    ((string)e.Attribute(Ns.Manifest + "media-type")).Equals(value.MediaType ?? string.Empty,
                                                                              StringComparison.InvariantCulture) &&
                    ((string)e.Attribute(Ns.Manifest + "full-path")).Equals(value.FullPath,
                                                                             StringComparison.InvariantCulture));
            foreach (XElement fileEntry in fileEntries)
            {
                fileEntry.Remove();
            }
        }

        #region IDisposable Member

        private bool _disposed;

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// is reclaimed by garbage collection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the specified disposing.
        /// </summary>
        /// <param name="disposing">if set to <c>true</c> [disposing].</param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //DeleteUnpackedFiles();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="AODL.Document.TextDocuments.TextDocument"/> is reclaimed by garbage collection.
        /// </summary>
        ~SpreadsheetDocument()
        {
            Dispose();
        }

        #endregion
    }
}

/*
 * $Log: SpreadsheetDocument.cs,v $
 * Revision 1.3  2008/04/29 15:39:53  mt
 * new copyright header
 *
 * Revision 1.2  2008/02/08 07:12:20  larsbehr
 * - added initial chart support
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:47  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.7  2007/02/04 22:52:57  larsbm
 * - fixed bug in resize algorithm for rows and cells
 * - extending IDocument, overload SaveTo to accept external exporter impl.
 * - initial version of AODL PDF exporter add on
 *
 * Revision 1.6  2006/02/21 19:34:55  larsbm
 * - Fixed Bug text that contains a xml tag will be imported  as UnknowText and not correct displayed if document is exported  as HTML.
 * - Fixed Bug [ 1436080 ] Common styles
 *
 * Revision 1.5  2006/02/06 19:27:23  larsbm
 * - fixed bug in spreadsheet document
 * - added smal OpenOfficeLib for document printing
 *
 * Revision 1.4  2006/02/05 20:03:32  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.3  2006/02/02 21:55:59  larsbm
 * - Added Clone object support for many AODL object types
 * - New Importer implementation PlainTextImporter and CsvImporter
 * - New tests
 *
 * Revision 1.2  2006/01/29 18:52:14  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 */