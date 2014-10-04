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
using System.Xml.Linq;
using AODL.Document.Content.Fields;
using AODL.Document.Custom;
using AODL.Document.Exceptions;
using AODL.Document.Export;
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Styles.MasterStyles;
using AODL.Document.TextDocuments;
using AODL.IO;
using DocumentManifest=AODL.Document.TextDocuments.DocumentManifest;
using DocumentSetting=AODL.Document.TextDocuments.DocumentSetting;
using DocumentStyles=AODL.Document.TextDocuments.DocumentStyles;

namespace AODL.Document.Import.OpenDocument
{
    /// <summary>
    /// OpenDocumentImporter - Importer for OpenDocuments in different formats.
    /// </summary>
    public class OpenDocumentImporter : IImporter, IPublisherInfo
    {
        private static readonly string[] KnownFiles = new [] {"content.xml", "styles.xml", "meta.xml", "settings.xml"};
        private readonly IPackageReader _packageReader;

        /// <summary>
        /// The document to fill with content.
        /// </summary>
        private IDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenDocumentImporter"/> class.
        /// </summary>
        public OpenDocumentImporter(IPackageReader packageReader)
        {
            _packageReader = packageReader;
            _importError = new List<AODLWarning>();

            _supportedExtensions = new List<DocumentSupportInfo>
                                       {
                                           new DocumentSupportInfo(".odt", DocumentTypes.TextDocument),
                                           new DocumentSupportInfo(".ods", DocumentTypes.SpreadsheetDocument)
                                       };

            _author = "Lars Behrmann, lb@OpenDocument4all.com";
            _infoUrl = "http://AODL.OpenDocument4all.com";
            _description = "This the standard importer of the OpenDocument library AODL.";
        }

        #region IExporter Member

        private readonly IList<AODLWarning> _importError;
        private readonly IList<DocumentSupportInfo> _supportedExtensions;

        /// <summary>
        /// Gets the document support infos.
        /// </summary>
        /// <value>The document support infos.</value>
        public IList<DocumentSupportInfo> DocumentSupportInfos
        {
            get { return _supportedExtensions; }
        }

        /// <summary>
        /// Imports the specified filename.
        /// </summary>
        /// <param name="document">The TextDocument to fill.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>The created TextDocument</returns>
        public void Import(IDocument document, string filename)
        {
            try
            {
                _document = document;
                UnpackFiles(filename);
                ReadContent();
            }
            catch (Exception ex)
            {
                throw new ImporterException(string.Format(
                                                "Failed to import document '{0}'", filename), ex);
            }
        }

        public Stream Open(string path)
        {
            return _packageReader.Open(path);
        }

        public IFile GetFile(string path)
        {
            return _packageReader.GetFile(path);
        }

        /// <summary>
        /// Gets the import errors as List of strings.
        /// </summary>
        /// <value>The import errors.</value>
        public IList<AODLWarning> ImportError
        {
            get { return _importError; }
        }

        /// <summary>
        /// If the import file format isn't any OpenDocument
        /// format you have to return true and AODL will
        /// create a new one.
        /// </summary>
        /// <value></value>
        public bool NeedNewOpenDocument
        {
            get { return false; }
        }

        #endregion

        #region IPublisherInfo Member

        private readonly string _author;
        private readonly string _description;

        private readonly string _infoUrl;

        /// <summary>
        /// The name the Author
        /// </summary>
        /// <value></value>
        public string Author
        {
            get { return _author; }
        }

        /// <summary>
        /// Url to a info site
        /// </summary>
        /// <value></value>
        public string InfoUrl
        {
            get { return _infoUrl; }
        }

        /// <summary>
        /// Description about the exporter resp. importer
        /// </summary>
        /// <value></value>
        public string Description
        {
            get { return _description; }
        }

        #endregion

        #region unpacking files and images

        /// <summary>
        /// Unpacks the files.
        /// </summary>
        /// <param name="file">The file.</param>
        private void UnpackFiles(string file)
        {
            _packageReader.Initialize(file);

            MovePictures();
            ReadResources();
        }

        /// <summary>
        /// Moves the pictures folder
        /// To avoid gdi errors.
        /// </summary>
        private static void MovePictures()
        {
//			if (Directory.Exists(dir+"Pictures"))
//			{
//				if (Directory.Exists(dirpics))
//					Directory.Delete(dirpics, true);
//				Directory.Move(dir+"Pictures", dirpics);
//			}
        }

        /// <summary>
        /// Reads the resources.
        /// </summary>
        private void ReadResources()
        {
            _document.DocumentConfigurations2 = new DocumentConfiguration2();
            ReadDocumentConfigurations2();

            _document.DocumentMetadata = new DocumentMetadata(_document);
            _document.DocumentMetadata.LoadFromFile(_packageReader.Open(DocumentMetadata.FileName));

            if (_document is TextDocument)
            {
                ((TextDocument) _document).DocumentSetting = new DocumentSetting();
                string file = DocumentSetting.FileName;
                ((TextDocument) _document).DocumentSetting.LoadFromFile(_packageReader.Open(file));

                ((TextDocument) _document).DocumentManifest = new DocumentManifest();
                string folder = DocumentManifest.FolderName;
                file = DocumentManifest.FileName;
                ((TextDocument) _document).DocumentManifest.LoadFromFile(_packageReader.Open(Path.Combine(folder, file)));

                ((TextDocument) _document).DocumentStyles = new DocumentStyles();
                file = DocumentStyles.FileName;
                ((TextDocument) _document).DocumentStyles.LoadFromFile(_packageReader.Open(file));

                ReadCustomFiles(((TextDocument) _document).DocumentManifest);
            }
            else if (_document is SpreadsheetDocument)
            {
                ((SpreadsheetDocument) _document).DocumentSetting = new SpreadsheetDocuments.DocumentSetting();
                string file = DocumentSetting.FileName;
                ((SpreadsheetDocument) _document).DocumentSetting.LoadFromFile(_packageReader.Open(file));

                ((SpreadsheetDocument) _document).DocumentManifest = new SpreadsheetDocuments.DocumentManifest();
                string folder = DocumentManifest.FolderName;
                file = DocumentManifest.FileName;
                ((SpreadsheetDocument) _document).DocumentManifest.LoadFromFile(_packageReader.Open(Path.Combine(folder, file)));

                ((SpreadsheetDocument) _document).DocumentStyles = new SpreadsheetDocuments.DocumentStyles();
                file = DocumentStyles.FileName;
                ((SpreadsheetDocument) _document).DocumentStyles.LoadFromFile(_packageReader.Open(file));

                ReadCustomFiles(((SpreadsheetDocument)_document).DocumentManifest);
            }

            _document.DocumentPictures = ReadImageResources("Pictures");

            _document.DocumentThumbnails = ReadImageResources("Thumbnails");

            //There's no really need to read the fonts.

            InitMetaData();
        }

        /// <summary>
        /// Reads the document configurations2.
        /// </summary>
        private void ReadDocumentConfigurations2()
        {
            if (!_packageReader.DirectoryExists(DocumentConfiguration2.FolderName))
                return;
            IFile file = _packageReader.GetFiles(DocumentConfiguration2.FolderName).FirstOrDefault();
            if (file != null)
            {
                _document.DocumentConfigurations2.FileName = file.Name;
                using (TextReader reader = new StreamReader(file.OpenRead()))
                {
                    _document.DocumentConfigurations2.Configurations2Content += reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Reads the image resources.
        /// </summary>
        /// <param name="folder">The folder.</param>
        private DocumentPictureCollection ReadImageResources(string folder)
        {
            DocumentPictureCollection dpc = new DocumentPictureCollection();
            //If folder not exists, return (folder will only unpacked if not empty)
            if (!_packageReader.DirectoryExists(folder))
                return dpc;
            //Only image files should be in this folder, if not -> Exception
            foreach (IFile file in _packageReader.GetFiles(folder))
            {
                DocumentPicture dp = new DocumentPicture(file);
                dpc.Add(dp);
            }

            return dpc;
        }

        private void ReadCustomFiles(DocumentManifest manifest)
        {
            foreach (XElement fileEntry in manifest.Manifest.Element(Ns.Manifest + "manifest").Elements(Ns.Manifest + "file-entry"))
            {
                string fullPath = (string)fileEntry.Attribute(Ns.Manifest + "full-path");
                if (string.IsNullOrEmpty(fullPath))
                    continue;
                if (fullPath.EndsWith("/", StringComparison.InvariantCulture))
                    continue;
                if (fullPath.StartsWith("Configurations2/", StringComparison.InvariantCulture))
                    continue;
                if (fullPath.StartsWith("Pictures/", StringComparison.InvariantCulture))
                    continue;
                if (fullPath.StartsWith("Thumbnails/", StringComparison.InvariantCulture))
                    continue;
                if (KnownFiles.Contains(fullPath))
                    continue;

                string mediaType = (string)fileEntry.Attribute(Ns.Manifest + "media-type");
                _document.CustomFiles.Add(new PackageCustomFile(mediaType, _packageReader.GetFile(fullPath)));
            }
        }

        #endregion

        public IDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }

        /// <summary>
        /// Reads the content.
        /// </summary>
        private void ReadContent()
        {
            /*
			 * NOTICE:
			 * Do not change this order!
			 */

            // 1. load content file
            using (TextReader reader = new StreamReader(_packageReader.Open("content.xml")))
            {
                _document.XmlDoc = XDocument.Load(reader);
            }

            // 2. Read local styles
            LocalStyleProcessor lsp = new LocalStyleProcessor(_document, false);
            lsp.ReadStyles();

            // 3. Import common styles and read common styles
            ImportCommonStyles();
            lsp = new LocalStyleProcessor(_document, true);
            lsp.ReadStyles();

            if (_document is TextDocument)
            {
                FormsProcessor fp = new FormsProcessor(_document);
                fp.ReadFormNodes();

                TextDocument td = _document as TextDocument;
                td.VariableDeclarations.Clear();

                XElement nodeText =
                    td.XmlDoc.Elements(Ns.Office + "document-content")
                        .Elements(Ns.Office + "body")
                        .Elements(Ns.Office + "text").FirstOrDefault();
                if (nodeText != null)
                {
                    XElement nodeVarDecls = nodeText.Element(Ns.Text + "variable-decls");
                    if (nodeVarDecls != null)
                    {
                        foreach (XElement vd in new XElement(nodeVarDecls).Elements(Ns.Text + "variable-decl"))
                        {
                            td.VariableDeclarations.Add(new VariableDecl(td, vd));
                        }
                        nodeVarDecls.Value = "";
                    }
                }
            }

            // 4. Register warnig events
            MainContentProcessor mcp = new MainContentProcessor(_document);
            mcp.Warning += mcp_OnWarning;

            // 5. Read the content
            mcp.ReadContentNodes();

            // 6.1 load master pages and styles for TextDocument
            if (_document is TextDocument)
            {
                MasterPageFactory.RenameMasterStyles(
                    ((TextDocument) _document).DocumentStyles.Styles,
                    _document.XmlDoc);
                // Read the moved and renamed styles
                lsp = new LocalStyleProcessor(_document, false);
                lsp.ReReadKnownAutomaticStyles();
                new MasterPageFactory().FillFromXMLDocument(_document as TextDocument);
            }
        }

        /// <summary>
        /// If the common styles are placed in the DocumentStyles,
        /// they will be imported into the content file.
        /// </summary>
        public void ImportCommonStyles()
        {
            XElement nodeStyles = null;

            if (_document is TextDocument)
                nodeStyles =
                    ((TextDocument) _document).DocumentStyles.Styles.Elements(Ns.Office + "document-styles").Elements(
                        Ns.Office + "styles").FirstOrDefault();
            else if (_document is SpreadsheetDocument)
                nodeStyles =
                    ((SpreadsheetDocument) _document).DocumentStyles.Styles.Elements(Ns.Office + "document-styles").
                        Elements(Ns.Office + "styles").FirstOrDefault();

            XElement nodeOfficeDocument = _document.XmlDoc.Element(Ns.Office + "document-content");

            if (nodeOfficeDocument != null && nodeStyles != null)
            {
                nodeOfficeDocument.Add(new XElement(nodeStyles));
            }
        }

        /// <summary>
        /// Inits the meta data.
        /// </summary>
        private void InitMetaData()
        {
            _document.DocumentMetadata.ImageCount = 0;
            _document.DocumentMetadata.ObjectCount = 0;
            _document.DocumentMetadata.ParagraphCount = 0;
            _document.DocumentMetadata.TableCount = 0;
            _document.DocumentMetadata.WordCount = 0;
            _document.DocumentMetadata.CharacterCount = 0;
            _document.DocumentMetadata.LastModified = DateTime.Now.ToString("s");
        }

        /// <summary>
        /// MCP_s the on warning.
        /// </summary>
        /// <param name="warning">The warning.</param>
        private void mcp_OnWarning(AODLWarning warning)
        {
            _importError.Add(warning);
        }

        private void TextContentProcessor_OnWarning(AODLWarning warning)
        {
            _importError.Add(warning);
        }
    }
}

/*
 * $Log: OpenDocumentImporter.cs,v $
 * Revision 1.10  2008/04/29 15:39:52  mt
 * new copyright header
 *
 * Revision 1.9  2007/08/15 11:53:40  larsbehr
 * - Optimized Mono related stuff
 *
 * Revision 1.8  2007/07/15 09:30:46  yegorov
 * Issue number:
 * Submitted by:
 * Reviewed by:
 *
 * Revision 1.5  2007/06/20 17:37:19  yegorov
 * Issue number:
 * Submitted by:
 * Reviewed by:
 *
 * Revision 1.2  2007/04/08 16:51:37  larsbehr
 * - finished master pages and styles for text documents
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:45  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.5  2006/05/02 17:37:16  larsbm
 * - Flag added graphics with guid
 * - Set guid based read and write directories
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
 * Revision 1.4  2005/12/18 18:29:46  larsbm
 * - AODC Gui redesign
 * - AODC HTML exporter refecatored
 * - Full Meta Data Support
 * - Increase textprocessing performance
 *
 * Revision 1.3  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.2  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 * Revision 1.1  2005/11/06 14:55:25  larsbm
 * - Interfaces for Import and Export
 * - First implementation of IExport OpenDocumentTextExporter
 *
 */