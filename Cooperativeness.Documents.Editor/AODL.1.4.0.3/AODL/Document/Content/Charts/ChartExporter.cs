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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using AODL.Document.Content.EmbedObjects;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Styles;
using AODL.IO;

namespace AODL.Document.Content.Charts
{
    /// <summary>
    /// Summary description for OpenDocumentTextExporter.
    /// </summary>
    public class ChartExporter
    {
        private readonly string[] _directories = {"ObjectReplacements"};
        private IDocument _document;

        #region IExporter Member

        /// <summary>
        /// Exports the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="packageWriter">The packageWriter.</param>
        public void Export(IDocument document, IPackageWriter packageWriter)
        {
            _document = document;
            //PrepareDirectory(dir);

            foreach (EmbedObject eo in document.EmbedObjects)
            {
                if (eo.ObjectType.Equals("chart"))
                {
                    WriteSingleFiles(((Chart) eo).ChartStyles.Styles, packageWriter, 
                                     Path.Combine(eo.ObjectName, ChartStyles.FileName));
                    WriteSingleFiles(((Chart) eo).ChartDoc, packageWriter, Path.Combine(eo.ObjectName, "content.xml"));
                    WriteFileEntry(eo.ObjectName);
                }
            }
        }

        /// <summary>
        /// Writes the single files.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="packageWriter"></param>
        /// <param name="filename">The filename.</param>
        private static void WriteSingleFiles(XNode document, IPackageWriter packageWriter, string filename)
        {
            //document.Save(filename);
            using (TextWriter textWriter = new StreamWriter(packageWriter.Open(filename), Encoding.UTF8))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings {Indent = false}))
                {
                    document.WriteTo(xmlWriter);
                }
            }
        }

        #endregion

        /// <summary>
        /// Create an output directory with all necessary subfolders.
        /// </summary>
        /// <param name="directory">The directory.</param>
        private void PrepareDirectory(string directory)
        {
            foreach (EmbedObject eo in _document.EmbedObjects)
                if (eo.ObjectType.Equals("chart"))
                    Directory.CreateDirectory(directory + @"\" + eo.ObjectName);

            foreach (string d in _directories)
                Directory.CreateDirectory(directory + @"\" + d);
        }

        private void WriteFileEntry(string objectName)
        {
            XElement manifest =
                ((SpreadsheetDocument) _document).DocumentManifest.Manifest.Element(Ns.Manifest + "manifest");

            XElement node = new XElement(Ns.Manifest + "file-entry");
            node.SetAttributeValue(Ns.Manifest + "media-type", "text/xml");
            node.SetAttributeValue(Ns.Manifest + "full-path", string.Format(@"{0}/content.xml", objectName));
            manifest.Add(node);

            node = new XElement(Ns.Manifest + "file-entry");
            node.SetAttributeValue(Ns.Manifest + "media-type", "text/xml");
            node.SetAttributeValue(Ns.Manifest + "full-path", string.Format(@"{0}/styles.xml", objectName));
            manifest.Add(node);

            node = new XElement(Ns.Manifest + "file-entry");
            node.SetAttributeValue(Ns.Manifest + "media-type", "application/vnd.oasis.opendocument.chart");
            node.SetAttributeValue(Ns.Manifest + "full-path", string.Format(@"{0}/", objectName));
            manifest.Add(node);
        }
    }
}