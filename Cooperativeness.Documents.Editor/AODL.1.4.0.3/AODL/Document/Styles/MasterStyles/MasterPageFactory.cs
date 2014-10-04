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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AODL.Document.Content;
using AODL.Document.Content.Tables;
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.TextDocuments;

namespace AODL.Document.Styles.MasterStyles
{
    /// <summary>
    /// Summary for MasterPageFactory.
    /// </summary>
    public class MasterPageFactory
    {
        /// <summary>
        /// Rename master styles.
        /// </summary>
        /// <param name="styleDocument">The style document.</param>
        /// <param name="contentDocument">The content document.</param>
        /// <param name="namespaceMng">The namespace MNG.</param>
        public static void RenameMasterStyles(XDocument styleDocument, XDocument contentDocument)
        {
            // Rename style names used in inside the master page
            // contents. This is necessary since OpenOffice create
            // duplicate style names.
            XElement automaticStyles = styleDocument.Descendants(Ns.Office + "automatic-styles").FirstOrDefault();
            XElement masterStyles = styleDocument.Descendants(Ns.Office + "master-styles").FirstOrDefault();
            if (automaticStyles != null && masterStyles != null && automaticStyles.HasElements)
            {
                foreach (XElement styleNode in automaticStyles.Elements())
                {
                    string styleName = (string) styleNode.Attribute(Ns.Style + "name");
                    if (styleName != null)
                    {
                        // Look for associated content inside header and footer
                        XNamespace family = Ns.Text; // default text
                        XElement contentNode;
                        if ((string) styleNode.Attribute(Ns.Style + "family") != null &&
                            ((string) styleNode.Attribute(Ns.Style + "family")).StartsWith("table"))
                        {
                            contentNode =
                                masterStyles.Descendants().Where(
                                    e => string.Equals((string) e.Attribute(Ns.Table + "style-name"), styleName)).
                                    FirstOrDefault();
                            family = Ns.Table;
                        }
                        else
                        {
                            contentNode =
                                masterStyles.Descendants().Where(
                                    e => string.Equals((string) e.Attribute(Ns.Text + "style-name"), styleName)).
                                    FirstOrDefault();
                        }
                        if (contentNode != null)
                        {
                            // This style name has to be changed
                            const string master = "master";
                            styleNode.SetAttributeValue(Ns.Style + "name", master + styleName);
                            contentNode.SetAttributeValue(family + "style-name", master + styleName);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fill/read the existing master page styles.
        /// </summary>
        /// <param name="textDocument">The owner text document.</param>
        public void FillFromXMLDocument(TextDocument textDocument)
        {
            TextMasterPageCollection txtMPCollection = new TextMasterPageCollection();
            IEnumerable<XElement> masterPageNodes = textDocument.DocumentStyles.Styles.Descendants(Ns.Style + "master-page");
            if (masterPageNodes != null)
            {
                foreach (XElement mpNode in masterPageNodes)
                {
                    // Build the master page
                    TextMasterPage txtMasterPage = new TextMasterPage(textDocument, mpNode);
                    // Even if there is no usage of header within the master page style,
                    // but of course there exists the header:style node, so we create
                    // the TextPageHeader.
                    txtMasterPage.TextPageHeader = new TextPageHeader();
                    txtMasterPage.TextPageHeader.TextDocument = textDocument;
                    txtMasterPage.TextPageHeader.TextMasterPage = txtMasterPage;
                    // see comment above its the same procedure
                    txtMasterPage.TextPageFooter = new TextPageFooter();
                    txtMasterPage.TextPageFooter.TextDocument = textDocument;
                    txtMasterPage.TextPageFooter.TextMasterPage = txtMasterPage;

                    // Build header content
                    XElement headerNode = mpNode.Descendants(Ns.Style + "header").FirstOrDefault();
                    if (headerNode != null)
                    {
                        txtMasterPage.TextPageHeader.ContentNode = headerNode;
                        ContentCollection contents = GetContentHeaderFooter(headerNode, textDocument);
                        if (contents != null)
                        {
                            headerNode.RemoveAll();
                            foreach (IContent iContent in contents)
                            {
                                txtMasterPage.TextPageHeader.ContentCollection.Add(iContent);
                            }
                        }
                    }

                    // Build footer content
                    XElement footerNode = mpNode.Descendants(Ns.Style + "footer").FirstOrDefault();
                    if (footerNode != null)
                    {
                        txtMasterPage.TextPageFooter.ContentNode = footerNode;
                        ContentCollection contents = GetContentHeaderFooter(footerNode, textDocument);
                        if (contents != null)
                        {
                            footerNode.RemoveAll();
                            foreach (IContent iContent in contents)
                            {
                                txtMasterPage.TextPageFooter.ContentCollection.Add(iContent);
                            }
                        }
                    }

                    // Build master page layout
                    XElement txtPageLayoutNode = textDocument.DocumentStyles.Styles.Descendants(Ns.Style + "page-layout")
                        .Where(
                        e => string.Equals((string) e.Attribute(Ns.Style + "name"), txtMasterPage.PageLayoutName)).
                        FirstOrDefault();
                    if (txtPageLayoutNode != null)
                    {
                        // Build master page layout properties
                        XElement txtPageLayoutPropNode =
                            txtPageLayoutNode.Descendants(Ns.Style + "page-layout-properties").FirstOrDefault();
                        if (txtPageLayoutPropNode != null)
                        {
                            TextPageLayout txtPageLayout = new TextPageLayout(
                                textDocument, txtPageLayoutNode, txtPageLayoutPropNode);
                            txtMasterPage.TextPageLayout = txtPageLayout;
                        }
                        // Build master page header layout
                        XElement txtHeaderStyleNode =
                            txtPageLayoutNode.Descendants(Ns.Style + "header-style").FirstOrDefault();
                        if (txtHeaderStyleNode != null)
                        {
                            txtMasterPage.TextPageHeader.StyleNode = txtHeaderStyleNode;
                            if (txtHeaderStyleNode.FirstNode != null
                                &&
                                ((XElement) txtHeaderStyleNode.FirstNode).Name ==
                                Ns.Style + "header-footer-properties")
                                txtMasterPage.TextPageHeader.PropertyNode = (XElement) txtHeaderStyleNode.FirstNode;
                        }
                        // Build master page footer layout
                        XElement txtFooterStyleNode =
                            txtPageLayoutNode.Descendants(Ns.Style + "footer-style").FirstOrDefault();
                        if (txtFooterStyleNode != null)
                        {
                            txtMasterPage.TextPageFooter.StyleNode = txtFooterStyleNode;
                            if (txtFooterStyleNode.FirstNode != null
                                &&
                                ((XElement) txtFooterStyleNode.FirstNode).Name ==
                                Ns.Style + "header-footer-properties")
                                txtMasterPage.TextPageFooter.PropertyNode = (XElement) txtFooterStyleNode.FirstNode;
                        }
                    }

                    txtMPCollection.Add(txtMasterPage);
                }
            }
            textDocument.TextMasterPageCollection = txtMPCollection;
        }

        /// <summary>
        /// Gets the content for headers and footers.
        /// </summary>
        /// <param name="contentNode">The content node.</param>
        /// <param name="textDocument">The text document.</param>
        /// <returns>The contents as IContentCollection.</returns>
        public ContentCollection GetContentHeaderFooter(XElement contentNode, TextDocument textDocument)
        {
            ContentCollection contents = new ContentCollection();
            if (contentNode != null && contentNode.HasElements)
            {
                XElement node = textDocument.XmlDoc != contentNode.Document ? new XElement(contentNode) : contentNode;
                MainContentProcessor mcp = new MainContentProcessor(textDocument);
                foreach (XElement nodeChild in node.Elements())
                {
                    IContent iContent = mcp.CreateContent(nodeChild);
                    if (iContent != null)
                    {
                        if (iContent is Table)
                            ((Table) iContent).BuildNode();
                        contents.Add(iContent);
                    }
                }
            }
            return contents;
        }
    }
}