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
using AODL.Document.Content.Fields;
using AODL.Document.Content.Tables;
using AODL.Document.Custom;
using AODL.Document.Exceptions;
using AODL.Document.Export;
using AODL.Document.Forms;
using AODL.Document.Forms.Controls;
using AODL.Document.Import;
using AODL.Document.Import.OpenDocument;
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.Styles;
using AODL.Document.Styles.MasterStyles;

namespace AODL.Document.TextDocuments
{
    /// <summary>
    /// Represent a opendocument text document.
    /// </summary>
    /// <example>
    /// <code>
    /// TextDocument td = new TextDocument();
    /// td.New();
    /// Paragraph p = new Paragraph(td, "P1");
    /// //add text
    /// p.TextContent.Add(new SimpleText(p, "Hello"));
    /// //Add the Paragraph
    /// td.Content.Add((IContent)p);
    /// //Blank para
    /// td.Content.Add(new Paragraph(td, ParentStyles.Standard.ToString()));
    /// // new para
    /// p = new Paragraph(td, "P2");
    /// p.TextContent.Add(new SimpleText(p, "Hello again"));
    /// td.Content.Add(p);
    /// td.Save("parablank.odt");
    /// </code>
    /// </example>
    public class TextDocument : IDisposable, IDocument //AODL.Document.TextDocuments.Content.IContentContainer,
    {
        private const string _mimeTyp = "application/vnd.oasis.opendocument.text";
        private const int _tableCount = 0;
        private const int _tableOfContentsCount = 0;
        private readonly IList<Graphic> _graphics;
        private readonly StyleFactory m_styleFactory;
        private FieldsCollection _fields;
        private ODFFormCollection _formCollection;
        private bool _isLoadedFile;
        private VariableDeclCollection _varDecls;
        private readonly CustomFileCollection _customFiles;
        private XDocument _xmldoc;

        private IImporter _importer;

        /// <summary>
        /// Create a new TextDocument object.
        /// </summary>
        public TextDocument()
        {
            _fields = new FieldsCollection();
            Content = new ContentCollection();
            Styles = new StyleCollection();
            m_styleFactory = new StyleFactory(this);
            CommonStyles = new StyleCollection();
            FontList = new List<string>();
            _graphics = new List<Graphic>();

            _formCollection = new ODFFormCollection();
            _formCollection.Clearing += FormsCollection_Clear;
            _formCollection.Removed += FormsCollection_Removed;

            VariableDeclarations = new VariableDeclCollection();

            _customFiles = new CustomFileCollection();
            _customFiles.Clearing += CustomFiles_Clearing;
            _customFiles.Inserting += CustomFiles_Inserting;
            _customFiles.Removing += CustomFiles_Removing;
        }

        /// <summary>
        /// Gets the tableof contents count.
        /// </summary>
        /// <value>The tableof contents count.</value>
        public int TableofContentsCount
        {
            get { return _tableOfContentsCount; }
        }

        /// <summary>
        /// Gets the tableof contents count.
        /// </summary>
        /// <value>The tableof contents count.</value>
        public int TableCount
        {
            get { return _tableCount; }
        }

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

        public ODFFormCollection Forms
        {
            get { return _formCollection; }
            set { _formCollection = value; }
        }

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

        /// <summary>
        /// Gets or sets the text master page collection.
        /// </summary>
        /// <value>The text master page collection.</value>
        public TextMasterPageCollection TextMasterPageCollection { get; set; }

        public FieldsCollection Fields
        {
            get { return _fields; }
            set
            {
                if (value != _fields)
                {
                    throw new Exception("Cannot assign a new value to Fields property!");
                }
            }
        }

        public VariableDeclCollection VariableDeclarations
        {
            get { return _varDecls; }
            set { _varDecls = value; }
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
        /// The xmldocument the textdocument based on.
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
        /// Gets the graphics.
        /// </summary>
        /// <value>The graphics.</value>
        public IList<Graphic> Graphics
        {
            get { return _graphics; }
        }

        /// <summary>
        /// Gets or sets the document metadata.
        /// </summary>
        /// <value>The document metadata.</value>
        public DocumentMetadata DocumentMetadata { get; set; }

        /// <summary>
        /// Gets or sets the document configurations2.
        /// </summary>
        /// <value>The document configurations2.</value>
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
        public IList<EmbedObject> EmbedObjects { get; set; }

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
        /// Gets or sets the font list.
        /// </summary>
        /// <value>The font list.</value>
        public IList<String> FontList { get; set; }

        /// <summary>
        /// Loads the document by using the specified importer.
        /// </summary>
        /// <param name="file">The the file.</param>
        /// <param name="importer"></param>
        public void Load(string file, IImporter importer)
        {
            _importer = importer;
            _isLoadedFile = true;

            Styles = new StyleCollection();
            _fields = new FieldsCollection();
            Content = new ContentCollection();

            _xmldoc = XDocument.Parse(TextDocumentHelper.GetBlankDocument());

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
            _formCollection.Clearing += FormsCollection_Clear;
            _formCollection.Removed += FormsCollection_Removed;
        }

        /// <summary>
        /// Save the document by using the passed IExporter
        /// with the passed file name.
        /// </summary>
        /// <param name="filename">The name of the new file.</param>
        /// <param name="exporter"></param>
        public void Save(string filename, IExporter exporter)
        {
            //Build document first
            foreach (string font in FontList)
                AddFont(font);
            CreateContentBody();

            exporter.Export(this, filename);
        }

        #endregion

        /// <summary>
        /// Create a blank new document.
        /// </summary>
        public TextDocument New()
        {
            _xmldoc = XDocument.Parse(TextDocumentHelper.GetBlankDocument());
            Styles = new StyleCollection();

            DocumentConfigurations2 = new DocumentConfiguration2();

            DocumentManifest = new DocumentManifest();
            DocumentManifest.New();

            DocumentMetadata = new DocumentMetadata(this);
            DocumentMetadata.New();

            DocumentPictures = new DocumentPictureCollection();

            DocumentSetting = new DocumentSetting();
            DocumentSetting.New();

            DocumentStyles = new DocumentStyles();
            DocumentStyles.New(this);
            ReadCommonStyles();

            Forms = new ODFFormCollection();
            _formCollection.Clearing += FormsCollection_Clear;
            _formCollection.Removed += FormsCollection_Removed;

            Fields.Clear();
            Content.Clear();


            VariableDeclarations = new VariableDeclCollection();

            DocumentThumbnails = new DocumentPictureCollection();

            MasterPageFactory.RenameMasterStyles(
                DocumentStyles.Styles,
                XmlDoc);
            // Read the moved and renamed styles
            LocalStyleProcessor lsp = new LocalStyleProcessor(this, false);
            lsp.ReReadKnownAutomaticStyles();
            new MasterPageFactory().FillFromXMLDocument(this);
            return this;
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
        /// Adds a font to the document. All fonts that you use
        /// within your text must be added to the document.
        /// The class FontFamilies represent all available fonts.
        /// </summary>
        /// <param name="fontname">The fontname take it from class FontFamilies.</param>
        private void AddFont(string fontname)
        {
            try
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                Stream stream = ass.GetManifestResourceStream("AODL.Resources.OD.fonts.xml");

                XDocument fontdoc;
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    fontdoc = XDocument.Load(reader);
                }

                XElement exfontnode = XmlDoc.Elements(Ns.Office + "document-content")
                    .Elements(Ns.Office + "font-face-decls")
                    .Elements(Ns.Style + "font-face")
                    .Where(e => string.Equals((string) e.Attribute(Ns.Style + "name"), fontname)).FirstOrDefault();

                if (exfontnode != null)
                    return; //Font exist;

                XElement newfontnode = fontdoc.Elements(Ns.Office + "document-content")
                    .Elements(Ns.Office + "font-face-decls")
                    .Elements(Ns.Style + "font-face")
                    .Where(e => string.Equals((string) e.Attribute(Ns.Style + "name"), fontname)).FirstOrDefault();

                if (newfontnode != null)
                {
                    XElement fontsnode = XmlDoc.Element(Ns.Office + "document-content");
                    if (fontsnode != null)
                    {
                        foreach (XElement xn in fontsnode.Elements())
                            if (xn.Name == Ns.Office + "font-face-decls")
                            {
                                XElement node = new XElement(Ns.Style + "font-face");
                                foreach (XAttribute xa in newfontnode.Attributes())
                                {
                                    node.Add(new XAttribute(xa));
                                }
                                xn.Add(node);
                                break;
                            }
                    }
                }
            }
            catch (Exception)
            {
                //Should never happen
                throw;
            }
        }

        /// <summary>
        /// Creates the content body.
        /// </summary>
        private void CreateContentBody()
        {
            XElement nodeText = XmlDoc.Elements(Ns.Office + "document-content")
                .Elements(Ns.Office + "body")
                .Elements(Ns.Office + "text").First();

            if (Forms.Count != 0)
            {
                XElement nodeForms = nodeText.Element(Ns.Office + "forms");
                if (nodeForms == null)
                {
                    nodeForms = new XElement(Ns.Office + "forms");
                }

                foreach (ODFForm f in Forms)
                {
                    nodeForms.Add(f.Node);
                }
                nodeText.Add(nodeForms);
            }

            if (_varDecls.Count != 0)
            {
                XElement nodeVarDecls = nodeText.Element(Ns.Text + "variable-decls");
                if (nodeVarDecls == null)
                {
                    nodeVarDecls = new XElement(Ns.Text + "variable-decls");
                }

                foreach (VariableDecl vd in _varDecls)
                {
                    nodeVarDecls.Add(new XElement(vd.Node));
                }

                nodeText.Add(nodeVarDecls);
            }

            foreach (IContent content in Content)
            {
                if (content is Table)
                    ((Table) content).BuildNode();
                nodeText.Add(content.Node);
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
            {
                bool exist = false;
                if (style.StyleName != null)
                {
                    string styleName = style.StyleName;
                    XElement node = nodeAutomaticStyles.Elements(Ns.Style + "style")
                        .Where(e => string.Equals((string) e.Attribute(Ns.Style + "name"), styleName)).FirstOrDefault();
                    if (node != null)
                        exist = true;
                }
                if (!exist)
                    nodeAutomaticStyles.Add(style.Node);
            }
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
            nodeCommonStyles =
                XmlDoc.Elements(Ns.Office + "document-content").Elements(Ns.Office + "styles").FirstOrDefault();
            if (nodeCommonStyles != null)
                nodeCommonStyles.Remove();
        }

        /// <summary>
        /// Looks for a specific control through all the forms by its ID
        /// </summary>
        /// <param name="id">Control ID</param>
        /// <returns>The control</returns>
        public ODFFormControl FindControlById(string id)
        {
            foreach (ODFForm f in Forms)
            {
                ODFFormControl fc = f.FindControlById(id, true);
                if (fc != null)
                    return fc;
            }
            return null;
        }

        /// <summary>
        /// Looks for a specific control through all the forms by its name
        /// </summary>
        /// <param name="name">Control name</param>
        /// <returns>The control</returns>
        public ODFFormControl FindControlByName(string name)
        {
            foreach (ODFForm f in Forms)
            {
                ODFFormControl fc = f.FindControlByName(name, true);
                if (fc != null)
                    return fc;
            }
            return null;
        }

        /// <summary>
        /// Adds new form to the forms collection
        /// </summary>
        /// <param name="name">Form name</param>
        /// <returns></returns>
        public ODFForm AddNewForm(string name)
        {
            ODFForm f = new ODFForm(this, name);
            Forms.Add(f);
            return f;
        }

        private void FormsCollection_Clear()
        {
            for (int i = 0; i < _formCollection.Count; i++)
            {
                ODFForm f = _formCollection[i];
                f.Controls.Clear();
            }
        }

        private static void FormsCollection_Removed(int index, object value)
        {
            ((ODFForm)value).Controls.Clear();
        }

        public EmbedObject GetObjectByName(string objectName)
        {
            for (int i = 0; i < EmbedObjects.Count; i++)
            {
                if (EmbedObjects[i].ObjectName == objectName)

                    return EmbedObjects[i];
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
                        ((string) e.Attribute(Ns.Manifest + "media-type")).Equals(customFile.MediaType ?? string.Empty,
                                                                                  StringComparison.InvariantCulture) &&
                        ((string) e.Attribute(Ns.Manifest + "full-path")).Equals(customFile.FullPath,
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
                    ((string) e.Attribute(Ns.Manifest + "media-type")).Equals(value.MediaType ?? string.Empty,
                                                                              StringComparison.InvariantCulture) &&
                    ((string) e.Attribute(Ns.Manifest + "full-path")).Equals(value.FullPath,
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
        ~TextDocument()
        {
            Dispose();
        }

        #endregion
    }
}

/*
 * $Log: TextDocument.cs,v $
 * Revision 1.10  2008/04/29 15:39:57  mt
 * new copyright header
 *
 * Revision 1.9  2008/02/08 07:12:21  larsbehr
 * - added initial chart support
 * - several bug fixes
 *
 * Revision 1.8  2007/07/15 09:30:28  yegorov
 * Issue number:
 * Submitted by:
 * Reviewed by:
 *
 * Revision 1.5  2007/06/20 17:37:19  yegorov
 * Issue number:
 * Submitted by:
 * Reviewed by:
 *
 * Revision 1.2  2007/04/08 16:51:24  larsbehr
 * - finished master pages and styles for text documents
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:59  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.7  2007/02/13 17:58:49  larsbm
 * - add first part of implementation of master style pages
 * - pdf exporter conversations for tables and images and added measurement helper
 *
 * Revision 1.6  2007/02/04 22:52:58  larsbm
 * - fixed bug in resize algorithm for rows and cells
 * - extending IDocument, overload SaveTo to accept external exporter impl.
 * - initial version of AODL PDF exporter add on
 *
 * Revision 1.5  2006/02/21 19:34:56  larsbm
 * - Fixed Bug text that contains a xml tag will be imported  as UnknowText and not correct displayed if document is exported  as HTML.
 * - Fixed Bug [ 1436080 ] Common styles
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
 * Revision 1.2  2006/01/29 18:52:51  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
 *
 * Revision 1.1  2006/01/29 11:28:30  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.18  2006/01/05 10:31:10  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 * Revision 1.17  2005/12/18 18:29:46  larsbm
 * - AODC Gui redesign
 * - AODC HTML exporter refecatored
 * - Full Meta Data Support
 * - Increase textprocessing performance
 *
 * Revision 1.16  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.15  2005/11/23 19:18:17  larsbm
 * - New Textproperties
 * - New Paragraphproperties
 * - New Border Helper
 * - Textproprtie helper
 *
 * Revision 1.14  2005/11/22 21:09:19  larsbm
 * - Add simple header and footer support
 *
 * Revision 1.13  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 * Revision 1.12  2005/11/06 14:55:25  larsbm
 * - Interfaces for Import and Export
 * - First implementation of IExport OpenDocumentTextExporter
 *
 * Revision 1.11  2005/10/23 16:47:48  larsbm
 * - Bugfix ListItem throws IStyleInterface not implemented exeption
 * - now. build the document after call saveto instead prepare the do at runtime
 * - add remove support for IText objects in the paragraph class
 *
 * Revision 1.10  2005/10/23 09:17:20  larsbm
 * - Release 1.0.3.0
 *
 * Revision 1.9  2005/10/22 15:52:10  larsbm
 * - Changed some styles from Enum to Class with statics
 * - Add full support for available OpenOffice fonts
 *
 * Revision 1.8  2005/10/22 10:47:41  larsbm
 * - add graphic support
 *
 * Revision 1.7  2005/10/16 08:36:29  larsbm
 * - Fixed bug [ 1327809 ] Invalid Cast Exception while insert table with cells that contains lists
 * - Fixed bug [ 1327820 ] Cell styles run into loop
 *
 * Revision 1.6  2005/10/15 12:13:20  larsbm
 * - fixed bug in add pargraph to cell
 *
 * Revision 1.5  2005/10/15 11:40:31  larsbm
 * - finished first step for table support
 *
 * Revision 1.4  2005/10/09 15:52:47  larsbm
 * - Changed some design at the paragraph usage
 * - add list support
 *
 * Revision 1.3  2005/10/08 12:31:33  larsbm
 * - better usabilty of paragraph handling
 * - create paragraphs with text and blank paragraphs with one line of code
 *
 * Revision 1.2  2005/10/08 07:50:15  larsbm
 * - added cvs tags
 *
 */