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
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AODL.Document.Content.Charts;
using AODL.Document.Content.Draw;
using AODL.Document.Content.EmbedObjects;
using AODL.Document.Custom;
using AODL.Document.Exceptions;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.TextDocuments;
using AODL.IO;
using DocumentManifest=AODL.Document.TextDocuments.DocumentManifest;
using DocumentSetting=AODL.Document.TextDocuments.DocumentSetting;
using DocumentStyles=AODL.Document.TextDocuments.DocumentStyles;

namespace AODL.Document.Export.OpenDocument
{
    /// <summary>
    /// OpenDocumentTextExporter is the standard exporter of AODL for the export
    /// of documents in the OpenDocument format.
    /// </summary>
    public class OpenDocumentTextExporter : IExporter, IPublisherInfo
    {
        private readonly string[] _directories = {"Configurations2", "META-INF", "Pictures", "Thumbnails"};
        private IDocument _document;
        private readonly IPackageWriter _packageWriter;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenDocumentTextExporter"/> class.
        /// </summary>
        public OpenDocumentTextExporter(IPackageWriter packageWriter)
        {
            _packageWriter = packageWriter;
            _exportError = new List<AODLWarning>();

            _supportedExtensions = new List<DocumentSupportInfo>
                                       {
                                           new DocumentSupportInfo(".odt", DocumentTypes.TextDocument),
                                           new DocumentSupportInfo(".ods", DocumentTypes.SpreadsheetDocument)
                                       };

            _author = "Lars Behrmann, lb@OpenDocument4all.com";
            _infoUrl = "http://AODL.OpenDocument4all.com";
            _description = "This the standard OpenDocument format exporter of the OpenDocument library AODL.";
        }

        #region IExporter Member

        private readonly IList<AODLWarning> _exportError;
        private readonly IList<DocumentSupportInfo> _supportedExtensions;

        /// <summary>
        /// List of DocumentSupportInfo objects
        /// </summary>
        /// <value>List of DocumentSupportInfo objects.</value>
        public IList<DocumentSupportInfo> DocumentSupportInfos
        {
            get { return _supportedExtensions; }
        }

        /// <summary>
        /// Gets the export error.
        /// </summary>
        /// <value>The export error.</value>
        public IList<AODLWarning> ExportError
        {
            get { return _exportError; }
        }

        /// <summary>
        /// Exports the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="filename">The filename.</param>
        public void Export(IDocument document, string filename)
        {
            _document = document;
            PrepareDirectory(filename);
            //Write content
            if (document is TextDocument)
            {
                WriteSingleFiles(((TextDocument) document).DocumentManifest.Manifest,
                                 Path.Combine(DocumentManifest.FolderName, DocumentManifest.FileName));
                WriteSingleFiles(((TextDocument) document).DocumentMetadata.Meta, DocumentMetadata.FileName);
                WriteSingleFiles(((TextDocument) document).DocumentSetting.Settings, DocumentSetting.FileName);
                WriteSingleFiles(((TextDocument) document).DocumentStyles.Styles, DocumentStyles.FileName);
                WriteSingleFiles(((TextDocument) document).XmlDoc, "content.xml");
                //Save graphics, which were build during creating a new document
                WriteCustomFiles(document.CustomFiles);
                CopyGraphics(document);
            }
            else if (document is SpreadsheetDocument)
            {
                WriteSingleFiles(((SpreadsheetDocument) document).DocumentManifest.Manifest,
                                 Path.Combine(DocumentManifest.FolderName, DocumentManifest.FileName));
                WriteSingleFiles(((SpreadsheetDocument) document).DocumentMetadata.Meta, DocumentMetadata.FileName);
                WriteSingleFiles(((SpreadsheetDocument) document).DocumentSetting.Settings, DocumentSetting.FileName);
                WriteSingleFiles(((SpreadsheetDocument) document).DocumentStyles.Styles, DocumentStyles.FileName);
                WriteSingleFiles(((SpreadsheetDocument) document).XmlDoc, "content.xml");

                if (document.EmbedObjects.Count != 0)
                {
                    foreach (EmbedObject eo in document.EmbedObjects)
                    {
                        if (eo.ObjectType.Equals("chart"))
                        {
                            ((Chart) eo).SaveTo(_packageWriter);
                        }
                    }
                }

                WriteCustomFiles(document.CustomFiles);

                WriteSingleFiles(((SpreadsheetDocument) document).DocumentManifest.Manifest,
                                 Path.Combine(DocumentManifest.FolderName, DocumentManifest.FileName));
            }
            else
                throw new Exception("Unsupported document type!");
            //Write Pictures and Thumbnails
//				this.SaveExistingGraphics(document.DocumentPictures, dir+"Pictures\\");
//				this.SaveExistingGraphics(document.DocumentThumbnails, dir+"Thumbnails\\");
            //Don't know why VS couldn't read a textfile resource without file prefix
            WriteMimetypeFile("mimetyp");
            //Now create the document
            CreateOpenDocument();

            //DeleteExportDirectory(exportDir);
        }

        public void DeleteExportDirectory(string directory)
        {
            // Clean up export
            DirectoryInfo di = new DirectoryInfo(directory);
            di.Delete(true);
        }

        private void WriteCustomFiles(IEnumerable<ICustomFile> customFiles)
        {
            foreach (ICustomFile file in customFiles)
            {
                WriteCustomFile(file);
            }
        }

        private void WriteCustomFile(ICustomFile file)
        {
            using (Stream targetStream = _packageWriter.Open(file.FullPath))
            {
                using (Stream sourceStream = file.OpenRead())
                {
                    byte[] buffer = new byte[1024];
                    while (true)
                    {
                        int read = sourceStream.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                            return;
                        targetStream.Write(buffer, 0, read);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the single files.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="filename">The filename.</param>
        private void WriteSingleFiles(XNode document, string filename)
        {
            //document.Save(filename);
            using (TextWriter textWriter = new StreamWriter(_packageWriter.Open(filename), Encoding.UTF8))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings { Indent = false }))
                {
                    document.WriteTo(xmlWriter);
                }
            }
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

        /// <summary>
        /// Create a zip archive with .odt.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="directory">The directory to zip.</param>
        private void CreateOpenDocument()
        {
            _packageWriter.EndWrite();
        }

        /// <summary>
        /// Create an output directory with all necessary subfolders.
        /// </summary>
        /// <param name="filename">The target filename.</param>
        private void PrepareDirectory(string filename)
        {
            _packageWriter.Initialize(filename);

            //foreach (string d in _directories)
            //    _packageWriter.CreateDirectory(d);
        }

        /// <summary>
        /// Helper Method: Don't know why, but it seems to be impossible
        /// to embbed a textfile as resource
        /// </summary>
        /// <param name="file">The filename.</param>
        private void WriteMimetypeFile(string file)
        {
            //Don't know why, but it seems to be impossible
            //to embbed a textfile as resource
            using (Stream stream = _packageWriter.Open(file))
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    if (_document is TextDocument)
                    {
                        sw.WriteLine("application/vnd.oasis.opendocument.text");
                    }
                    else if (_document is SpreadsheetDocument)
                    {
                        sw.WriteLine("application/vnd.oasis.opendocument.spreadsheet");
                    }
                }
            }
        }

        /// <summary>
        /// Copies the graphics.
        /// </summary>
        /// <param name="document">The document.</param>
        private void CopyGraphics(IDocument document)
        {
            const string picturedir = "Pictures";

            foreach (Graphic graphic in document.Graphics)
            {
                if (graphic.GraphicFile != null)
                {
                    //Loaded or added
                    if (graphic.GraphicFileName == null)
                    {
                        if (!_packageWriter.FileExists(Path.Combine(picturedir, graphic.GraphicFile.Name)))
                            graphic.GraphicFile.CopyTo(_packageWriter.Open(Path.Combine(picturedir, graphic.GraphicFile.Name)));
                    }
                    else
                        graphic.GraphicFile.CopyTo(_packageWriter.Open(Path.Combine(picturedir, graphic.GraphicFileName)));
                }
            }
            //MovePicturesIfLoaded(document, picturedir);
        }

        /// <summary>
        /// Moves the pictures if loaded.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="targetDir">The target dir.</param>
//		private static void MovePicturesIfLoaded(IDocument document,string targetDir)
//		{
        ////			if (document.DocumentPictures.Count > 0)
        ////			{
        ////				foreach(DocumentPicture docPic in document.DocumentPictures)
        ////				{
        ////					if (File.Exists(docPic.ImagePath))
        ////					{
        ////						FileInfo fInfo			= new FileInfo(docPic.ImagePath);
        ////						File.Copy(docPic.ImagePath, targetDir+fInfo.Name, true);
        ////					}
        ////
        ////				}
        ////			}
//		}
    }
}

/*
 * $Log: OpenDocumentTextExporter.cs,v $
 * Revision 1.6  2008/05/07 17:19:45  larsbehr
 * - Optimized Exporter Save procedure
 * - Optimized Tests behaviour
 * - Added ODF Package Layer
 * - SharpZipLib updated to current version
 *
 * Revision 1.5  2008/04/29 15:39:48  mt
 * new copyright header
 *
 * Revision 1.4  2008/02/08 07:12:20  larsbehr
 * - added initial chart support
 * - several bug fixes
 *
 * Revision 1.2  2007/04/08 16:51:31  larsbehr
 * - finished master pages and styles for text documents
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:43  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.6  2006/05/02 17:37:16  larsbm
 * - Flag added graphics with guid
 * - Set guid based read and write directories
 *
 * Revision 1.5  2006/02/21 19:34:55  larsbm
 * - Fixed Bug text that contains a xml tag will be imported  as UnknowText and not correct displayed if document is exported  as HTML.
 * - Fixed Bug [ 1436080 ] Common styles
 *
 * Revision 1.4  2006/02/16 18:35:41  larsbm
 * - Add FrameBuilder class
 * - TextSequence implementation (Todo loading!)
 * - Free draing postioning via x and y coordinates
 * - Graphic will give access to it's full qualified path
 *   via the GraphicRealPath property
 * - Fixed Bug with CellSpan in Spreadsheetdocuments
 * - Fixed bug graphic of loaded files won't be deleted if they
 *   are removed from the content.
 * - Break-Before property for Paragraph properties for Page Break
 *
 * Revision 1.3  2006/02/05 20:03:32  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.2  2006/01/29 18:52:14  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.5  2006/01/05 10:28:06  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
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