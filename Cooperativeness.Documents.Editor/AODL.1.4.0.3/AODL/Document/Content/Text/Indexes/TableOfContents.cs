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
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AODL.Document.Content.Text.TextControl;
using AODL.Document.Styles;
using AODL.Document.TextDocuments;

namespace AODL.Document.Content.Text.Indexes
{
    /// <summary>
    /// TableOfContent represent a table of contents.
    /// </summary>
    public class TableOfContents : IContent, IContentContainer, IHtml
    {
        /// <summary>
        /// The display name for content entries
        /// </summary>
        private const string _contentStyleDisplayName = "Contents ";

        /// <summary>
        /// The style name for content entries
        /// </summary>
        private const string _contentStyleName = "Contents_20_";

        /// <summary>
        /// Initializes a new instance of the <see cref="TableOfContents"/> class.
        /// </summary>
        /// <param name="textDocument">The text document.</param>
        /// <param name="styleName">Name of the style.</param>
        /// <param name="useHyperlinks">if set to <c>true</c> [use hyperlinks].</param>
        /// <param name="protectChanges">if set to <c>true</c> [protect changes].</param>
        /// <param name="textName">Title for the Table of content e.g. Table of Content</param>
        public TableOfContents(IDocument textDocument, string styleName, bool useHyperlinks, bool protectChanges,
                               string textName)
        {
            Document = textDocument;
            UseHyperlinks = useHyperlinks;
            Node = new XElement(Ns.Text + "table-of-content");
            Node.SetAttributeValue(Ns.Text + "style-name", styleName);
            Node.SetAttributeValue(Ns.Text + "protected", Convert.ToString(protectChanges).ToLower());
            Node.SetAttributeValue(Ns.Text + "use-outline-level", "true");
            Node.SetAttributeValue(Ns.Text + "name", textName ?? "Table of Contents1");
            Style = new SectionStyle(this, styleName);
            Document.Styles.Add(Style);

            TableOfContentsSource = new TableOfContentsSource(this);
            TableOfContentsSource.InitStandardTableOfContentStyle();
            Node.Add(TableOfContentsSource.Node);

            CreateIndexBody();
            CreateTitlePargraph();
            InsertContentStyle();
            SetOutlineStyle();
            RegisterEvents();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableOfContents"/> class.
        /// </summary>
        /// <param name="textDocument">The text document.</param>
        /// <param name="tocNode">The toc node.</param>
        public TableOfContents(IDocument textDocument, XElement tocNode)
        {
            Document = textDocument;
            Node = tocNode;
            RegisterEvents();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use hyperlinks].
        /// If it's set to true, the text entries automaticaly will
        /// extended with Hyperlinks.
        /// </summary>
        /// <value><c>true</c> if [use hyperlinks]; otherwise, <c>false</c>.</value>
        public bool UseHyperlinks { get; set; }

        public XElement IndexBodyNode { get; set; }

        /// <summary>
        /// Gets the title which is displayed
        /// for this table of content in english
        /// this will always be Table of Content,
        /// but this is free of choice.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Text + "name");
                if (s != null)
                    //Todo: Algo for more entries then 9
                    return s.Substring(0, s.Length - 1);
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the title paragraph.
        /// </summary>
        /// <value>The title paragraph.</value>
        public Paragraph TitleParagraph { get; set; }

        /// <summary>
        /// Gets or sets the table of content source.
        /// </summary>
        /// <value>The table of content source.</value>
        public TableOfContentsSource TableOfContentsSource { get; set; }

        /// <summary>
        /// Registers the events.
        /// </summary>
        private void RegisterEvents()
        {
            Content = new ContentCollection();
            Content.Inserted += Content_Inserted;
            Content.Removed += Content_Removed;
        }

        /// <summary>
        /// Creates the index body node.
        /// </summary>
        private void CreateIndexBody()
        {
            IndexBodyNode = new XElement(Ns.Text + "index-body");

            //First not is always the index title
            XElement indexTitleNode = new XElement(Ns.Text + "index-title");
            indexTitleNode.SetAttributeValue(Ns.Text + "style-name", StyleName);
            indexTitleNode.SetAttributeValue(Ns.Text + "name", Title + "_Head");

            IndexBodyNode.Add(indexTitleNode);
            Node.Add(IndexBodyNode);
        }

        /// <summary>
        /// Creates the title pargraph.
        /// </summary>
        private void CreateTitlePargraph()
        {
            TitleParagraph = new Paragraph(Document, "Table_Of_Contents_Title");
            TitleParagraph.TextContent.Add(new SimpleText(Document, Title));
            //Set default styles
            TitleParagraph.ParagraphStyle.TextProperties.Bold = "bold";
            TitleParagraph.ParagraphStyle.TextProperties.FontName = FontFamilies.Arial;
            TitleParagraph.ParagraphStyle.TextProperties.FontSize = "20pt";
            //Add to the index title
            IndexBodyNode.Elements().ElementAt(0).Add(TitleParagraph.Node);
        }

        /// <summary>
        /// Insert the content style nodes. These are 10 styles for
        /// each outline number one style.
        /// TODO: Section Style move to document common styles
        /// </summary>
        private void InsertContentStyle()
        {
            for (int i = 1; i <= 10; i++)
            {
                XElement styleNode = new XElement(Ns.Style + "style");
                styleNode.SetAttributeValue(Ns.Style + "name", _contentStyleName + i);
                styleNode.SetAttributeValue(Ns.Style + "display-name", _contentStyleDisplayName + i);
                styleNode.SetAttributeValue(Ns.Style + "parent-style-name", "Index");
                styleNode.SetAttributeValue(Ns.Style + "family", "paragraph");
                styleNode.SetAttributeValue(Ns.Style + "class", "index");

                XElement ppNode = new XElement(Ns.Style + "paragraph-properties");
                ppNode.SetAttributeValue(Ns.Fo + "margin-left",
                                         string.Format("{0}cm", (0.499*(i - 1)).ToString("F3").Replace(",", ".")));
                ppNode.SetAttributeValue(Ns.Fo + "margin-right", "0cm");
                ppNode.SetAttributeValue(Ns.Fo + "text-indent", "0cm");
                ppNode.SetAttributeValue(Ns.Fo + "auto-text-indent", "0cm");

                XElement tabsNode = new XElement(Ns.Style + "tab-stops");

                XElement tabNode = new XElement(Ns.Style + "tab-stop");
                tabNode.SetAttributeValue(Ns.Style + "position",
                                          (16.999 - (i*0.499)).ToString("F3").Replace(",", ".") + "cm");
                tabNode.SetAttributeValue(Ns.Style + "type", "right");
                tabNode.SetAttributeValue(Ns.Style + "leader-style", "dotted");
                tabNode.SetAttributeValue(Ns.Style + "leader-text", ".");

                tabsNode.Add(tabNode);
                ppNode.Add(tabsNode);
                styleNode.Add(ppNode);

                IStyle iStyle = new UnknownStyle(Document, styleNode);
                Document.CommonStyles.Add(iStyle);

//				XElement styleNode	= ((TextDocument)this.Document).DocumentStyles.Styles.CreateElement(
//					"style", "style", ((TextDocument)this.Document).GetNamespaceUri("style"));
//
//				XmlAttribute xa		= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "name", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= this._contentStyleName+i.ToString();
//				styleNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "display-name", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= this._contentStyleDisplayName+i.ToString();
//				styleNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "parent-style-name", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= "Index";
//				styleNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "family", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= "paragraph";
//				styleNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "class", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= "index";
//				styleNode.Attributes.Append(xa);
//
//				XElement ppNode		= ((TextDocument)this.Document).DocumentStyles.Styles.CreateElement(
//					"style", "paragraph-properties", ((TextDocument)this.Document).GetNamespaceUri("style"));
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"fo", "margin-left", ((TextDocument)this.Document).GetNamespaceUri("fo"));
//				xa.InnerText		= (0.499*(i-1)).ToString("F3").Replace(",",".")+"cm";
//				ppNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"fo", "margin-right", ((TextDocument)this.Document).GetNamespaceUri("fo"));
//				xa.InnerText		= "0cm";
//				ppNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"fo", "text-indent", ((TextDocument)this.Document).GetNamespaceUri("fo"));
//				xa.InnerText		= "0cm";
//				ppNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"fo", "auto-text-indent", ((TextDocument)this.Document).GetNamespaceUri("fo"));
//				xa.InnerText		= "0cm";
//				ppNode.Attributes.Append(xa);
//
//				XElement tabsNode		= ((TextDocument)this.Document).DocumentStyles.Styles.CreateElement(
//					"style", "tab-stops", ((TextDocument)this.Document).GetNamespaceUri("style"));
//
//				XElement tabNode			= ((TextDocument)this.Document).DocumentStyles.Styles.CreateElement(
//					"style", "tab-stop", ((TextDocument)this.Document).GetNamespaceUri("style"));
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "position", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= (16.999-(i*0.499)).ToString("F3").Replace(",",".")+"cm";
//				tabNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "type", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= "right";
//				tabNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "leader-style", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= "dotted";
//				tabNode.Attributes.Append(xa);
//
//				xa					= ((TextDocument)this.Document).DocumentStyles.Styles.CreateAttribute(
//					"style", "leader-text", ((TextDocument)this.Document).GetNamespaceUri("style"));
//				xa.InnerText		= ".";
//				tabNode.Attributes.Append(xa);
//
//				tabsNode.AppendChild(tabNode);
//				ppNode.AppendChild(tabsNode);
//				styleNode.AppendChild(ppNode);
//
//				((TextDocument)this.Document).DocumentStyles.InsertOfficeStylesNode(
//					styleNode, ((TextDocument)this.Document));
            }
        }

        /// <summary>
        /// Set the outline style.
        /// </summary>
        private void SetOutlineStyle()
        {
            for (int i = 1; i <= 10; i++)
            {
                ((TextDocument) Document).DocumentStyles.SetOutlineStyle(
                    i, "1", ((TextDocument) Document));
            }
        }

        /// <summary>
        /// Content was inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Inserted(int index, object value)
        {
            IndexBodyNode.Add(((IContent) value).Node);
        }

        /// <summary>
        /// Content was removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void Content_Removed(int index, object value)
        {
            ((IContent) value).Node.Remove();
        }

        /// <summary>
        /// Gets the tab stop style.
        /// </summary>
        /// <param name="leaderStyle">The leader style.</param>
        /// <param name="leadingChar">The leading char.</param>
        /// <param name="position">The position.</param>
        /// <returns>A for a table of contents optimized TabStopStyleCollection</returns>
        public TabStopStyleCollection GetTabStopStyle(string leaderStyle, string leadingChar, double position)
        {
            TabStopStyleCollection tabStopStyleCol = new TabStopStyleCollection(Document);
            //Create TabStopStyles
            TabStopStyle tabStopStyle = new TabStopStyle(Document, position)
                                            {
                                                LeaderStyle = leaderStyle,
                                                LeaderText = leadingChar,
                                                Type = TabStopTypes.Center
                                            };
            //Add the tabstop
            tabStopStyleCol.Add(tabStopStyle);

            return tabStopStyleCol;
        }

        /// <summary>
        /// Insert the given text as an Table of contents entry.
        /// e.g. You just insert a Headline 1. My headline to
        /// the document and want this text also as an Table of
        /// contents entry, so you can simply add the text using
        /// this method.
        /// </summary>
        /// <param name="textEntry">The text entry.</param>
        /// <param name="outLineLevel">The outline level possible 1-10.</param>
        public void InsertEntry(string textEntry, int outLineLevel)
        {
            Paragraph paragraph = new Paragraph(
                Document, "P1_Toc_Entry" + outLineLevel);
            ((ParagraphStyle) paragraph.Style).ParentStyle =
                _contentStyleName + outLineLevel;
            if (UseHyperlinks)
            {
                int firstWhiteSpace = textEntry.IndexOf(" ");
                StringBuilder sb = new StringBuilder(textEntry);
                sb = sb.Remove(firstWhiteSpace, 1);
                string link = "#" + sb + "|outline";
                XLink xlink = new XLink(Document, link, textEntry) {XLinkType = "simple"};
                paragraph.TextContent.Add(xlink);
                paragraph.TextContent.Add(new TabStop(Document));
                paragraph.TextContent.Add(new SimpleText(Document, "1"));
            }
            else
            {
                //add the tabstop and the page number, the number is
                //always set to 1, but will be updated by the most
                //word processors immediately to the correct page number.
                paragraph.TextContent.Add(new SimpleText(Document, textEntry));
                paragraph.TextContent.Add(new TabStop(Document));
                paragraph.TextContent.Add(new SimpleText(Document, "1"));
            }
            //There is a bug which deny to add new simple ta
//			this.TitleParagraph.ParagraphStyle.ParagraphProperties.TabStopStyleCollection =
//				this.GetTabStopStyle(TabStopLeaderStyles.Dotted, ".", 16.999);
            paragraph.ParagraphStyle.ParagraphProperties.TabStopStyleCollection =
                GetTabStopStyle(TabStopLeaderStyles.Dotted, ".", 16.999);

            Content.Add(paragraph);
        }

        #region IHtml Member

        /// <summary>
        /// Return the content as Html string
        /// </summary>
        /// <returns>The html string</returns>
        public string GetHtml()
        {
            string html = "<br>&nbsp;\n";
            try
            {
                foreach (IContent content in Content)
                    if (content is IHtml)
                        html += ((IHtml) content).GetHtml() + "\n";
            }
            catch (Exception)
            {
            }
            return html;
        }

        #endregion

        #region IContent Member

        private IStyle _style;

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public XElement Node { get; set; }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <value>The name of the style.</value>
        public string StyleName
        {
            get { return (string) Node.Attribute(Ns.Text + "style-name"); }
            set { Node.SetAttributeValue(Ns.Text + "style-name", value); }
        }

        /// <summary>
        /// Every object (typeof(IContent)) have to know his document.
        /// </summary>
        /// <value></value>
        public IDocument Document { get; set; }

        /// <summary>
        /// A Style class wich is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        /// <value></value>
        public IStyle Style
        {
            get { return _style; }
            set
            {
                StyleName = value.StyleName;
                _style = value;
            }
        }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        XNode IContent.Node
        {
            get { return Node; }
            set { Node = (XElement) value; }
        }

        #endregion

        #region IContentContainer Member

        /// <summary>
        /// Represent all visible entries of this Table of contents.
        /// Normally you should only insert paragraph objects as
        /// entry which match the following structure
        /// e.g. 3.1 My header text \t
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content { get; set; }

        #endregion
    }
}

/*
 * $Log: TableOfContents.cs,v $
 * Revision 1.3  2008/04/29 15:39:47  mt
 * new copyright header
 *
 * Revision 1.2  2007/04/08 16:51:24  larsbehr
 * - finished master pages and styles for text documents
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:40  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.4  2006/02/21 19:34:55  larsbm
 * - Fixed Bug text that contains a xml tag will be imported  as UnknowText and not correct displayed if document is exported  as HTML.
 * - Fixed Bug [ 1436080 ] Common styles
 *
 * Revision 1.3  2006/02/05 20:02:25  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.2  2006/01/29 18:52:14  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
 *
 * Revision 1.1  2006/01/29 11:28:22  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.1  2006/01/05 10:31:10  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 */