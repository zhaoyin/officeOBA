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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using AODL.Document.Content.Text;
using AODL.Document.Styles;

namespace AODL.Document.TextDocuments
{
    /// <summary>
    /// DocumentStyles global Document Style
    /// </summary>
    public class DocumentStyles
    {
        /// <summary>
        /// The file name.
        /// </summary>
        public static readonly string FileName = "styles.xml";

        private TextDocument _textDocument;

        /// <summary>
        /// Gets or sets the styles.
        /// </summary>
        /// <value>The styles.</value>
        public XDocument Styles { get; set; }

        /// <summary>
        /// Gets or sets the text document.
        /// </summary>
        /// <value>The text document.</value>
        public TextDocument TextDocument
        {
            get { return _textDocument; }
            set { _textDocument = value; }
        }

        /// <summary>
        /// Load the style from assmebly resource.
        /// </summary>
        public virtual void New()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            Stream str = ass.GetManifestResourceStream("AODL.Resources.OD.styles.xml");
            using (XmlReader reader = XmlReader.Create(str))
            {
                Styles = XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Create new document styles document and set the owner text document.
        /// </summary>
        /// <param name="ownerTextDocument">The owner text document.</param>
        public virtual void New(TextDocument ownerTextDocument)
        {
            _textDocument = ownerTextDocument;
            New();
        }

        /// <summary>
        /// Loads from file.
        /// </summary>
        /// <param name="file">The file.</param>
        public void LoadFromFile(Stream file)
        {
            using (TextReader reader = new StreamReader(file))
            {
                Styles = XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Inserts the office styles node into this XML style document.
        /// </summary>
        /// <param name="aStyleNode">A style node.</param>
        public virtual void InsertOfficeStylesNode(XElement aStyleNode)
        {
            Styles.Descendants(Ns.Office + "styles").First().Add(aStyleNode);
        }

        /// <summary>
        /// Inserts the office automatic styles node into this XML style document.
        /// </summary>
        /// <param name="aOfficeAutomaticStyleNode">A office automatic style node.</param>
        public virtual void InsertOfficeAutomaticStylesNode(XElement aOfficeAutomaticStyleNode)
        {
            Styles.Descendants(Ns.Office + "automatic-styles").First().Add(aOfficeAutomaticStyleNode);
        }

        /// <summary>
        /// Inserts the office master styles node into this XML style document.
        /// </summary>
        /// <param name="aOfficeMasterStyleNode">A office master style node.</param>
        public virtual void InsertOfficeMasterStylesNode(XElement aOfficeMasterStyleNode)
        {
            Styles.Descendants(Ns.Office + "master-styles").First().Add(aOfficeMasterStyleNode);
        }

        /// <summary>
        /// Sets the outline style.
        /// </summary>
        /// <param name="outlineLevel">The outline level.</param>
        /// <param name="numFormat">The num format.</param>
        /// <param name="document">The text document.</param>
        public void SetOutlineStyle(int outlineLevel, string numFormat, TextDocument document)
        {
            XElement outlineStyleNode = null;
            foreach (IStyle iStyle in document.CommonStyles.ToValueList())
                if (iStyle.Node.Name == Ns.Text + "outline-style")
                    outlineStyleNode = iStyle.Node;
//				XElement outlineStyleNode		= this.Styles.SelectSingleNode(
//					"//text:outline-style",
//					document.NamespaceManager);

            XElement outlineLevelNode = null;
            if (outlineStyleNode != null)
                outlineLevelNode = outlineStyleNode.Elements(Ns.Text + "outline-level-style")
                    .Where(
                    e =>
                    Convert.ToString(outlineLevel).Equals((string) e.Attribute(Ns.Text + "level"),
                                                          StringComparison.InvariantCulture)).FirstOrDefault();

            if (outlineLevelNode != null)
            {
                XAttribute numberFormatNode = outlineLevelNode.Attribute(Ns.Style + "num-format");
                if (numberFormatNode != null)
                    numberFormatNode.Value = numFormat;

                outlineLevelNode.SetAttributeValue(Ns.Style + "num-suffix", ".");

                if (outlineLevel > 1)
                {
                    outlineLevelNode.SetAttributeValue(Ns.Text + "display-levels", outlineLevel);
                }
            }
        }

        /// <summary>
        /// Inserts the footer.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="document">The document.</param>
        public void InsertFooter(Paragraph content, TextDocument document)
        {
            bool exist = true;
            XElement node = Styles.Descendants(Ns.Office + "master-styles")
                .Elements(Ns.Style + "master-page")
                .Elements(Ns.Style + "footer").FirstOrDefault();
            if (node != null)
                node.Value = "";
            else
            {
                node = new XElement(Ns.Style + "footer");
                exist = false;
            }

            node.Add(new XElement(content.Node));

            if (!exist)
                Styles.Descendants(Ns.Office + "master-styles")
                    .Elements(Ns.Style + "master-page").First().Add(node);

            InsertParagraphStyle(content);
        }

        /// <summary>
        /// Inserts the header.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="document">The document.</param>
        public void InsertHeader(Paragraph content, TextDocument document)
        {
            bool exist = true;
            XElement node = Styles.Descendants(Ns.Office + "master-styles")
                .Elements(Ns.Style + "master-page")
                .Elements(Ns.Style + "header").FirstOrDefault();
            if (node != null)
                node.Value = "";
            else
            {
                node = new XElement(Ns.Style + "header");
                exist = false;
            }

            node.Add(new XElement(content.Node));

            if (!exist)
                Styles.Descendants(Ns.Office + "master-styles")
                    .Elements(Ns.Style + "master-page").First().Add(node);

            InsertParagraphStyle(content);
        }

        /// <summary>
        /// Inserts the paragraph style.
        /// </summary>
        /// <param name="content">The content.</param>
        private void InsertParagraphStyle(Paragraph content)
        {
            if (content.Style != null)
            {
                Styles.Descendants(Ns.Office + "styles").First().Add(new XElement(content.Style.Node));
            }

            if (content.TextContent != null)
                foreach (IText it in content.TextContent)
                    if (it is FormatedText)
                    {
                        Styles.Descendants(Ns.Office + "styles").First().Add(new XElement(it.Style.Node));
                    }
        }

        /// <summary>
        /// Gets the HTML header.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The html string which represent the document header.</returns>
        public string GetHtmlHeader(TextDocument document)
        {
            const string html = "";
            XElement node = Styles.Descendants(Ns.Office + "master-styles")
                .Elements(Ns.Style + "master-page")
                .Elements(Ns.Style + "header").FirstOrDefault();

            if (node != null)
            {
            }
            return html;
        }

        /// <summary>
        /// Gets the HTML footer.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>The html string which represent the document footer.</returns>
        public string GetHtmlFooter(TextDocument document)
        {
            const string html = "";
            XElement node = Styles.Descendants(Ns.Office + "master-styles")
                .Elements(Ns.Style + "master-page")
                .Elements(Ns.Style + "footer").FirstOrDefault();

            if (node != null)
            {
            }
            return html;
        }
    }
}

/*
 * $Log: DocumentStyles.cs,v $
 * Revision 1.3  2008/04/29 15:39:57  mt
 * new copyright header
 *
 * Revision 1.2  2007/04/08 16:51:24  larsbehr
 * - finished master pages and styles for text documents
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:59  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.3  2007/02/13 17:58:49  larsbm
 * - add first part of implementation of master style pages
 * - pdf exporter conversations for tables and images and added measurement helper
 *
 * Revision 1.2  2006/02/21 19:34:56  larsbm
 * - Fixed Bug text that contains a xml tag will be imported  as UnknowText and not correct displayed if document is exported  as HTML.
 * - Fixed Bug [ 1436080 ] Common styles
 *
 * Revision 1.1  2006/01/29 11:28:30  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.5  2006/01/05 10:31:10  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 * Revision 1.4  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.3  2005/11/22 21:09:19  larsbm
 * - Add simple header and footer support
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