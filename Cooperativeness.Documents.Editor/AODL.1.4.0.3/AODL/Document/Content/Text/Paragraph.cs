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
using System.Linq;
using System.Xml.Linq;
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.Styles;
using AODL.Document.TextDocuments;

namespace AODL.Document.Content.Text
{
    /// <summary>
    /// Represent a paragraph within a opendocument document.
    /// </summary>
    public class Paragraph : IContent, IContentContainer, IHtml, ITextContainer, ICloneable
    {
        /// <summary>
        /// Mixed content - needed for alternative
        /// exporter implementations. In OpenDocument
        /// the order will be right automatically.
        /// </summary>
        private readonly ParentStyles _parentStyle;

        private IList<IContent> _mixedContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// This is a blank paragraph.
        /// </summary>
        /// <param name="document">The document.</param>
        public Paragraph(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Text + "p");
            InitStandards();
        }

        /// <summary>
        /// Create a new Paragraph object.
        /// </summary>
        /// <param name="document">The Texdocumentocument.</param>
        /// <param name="styleName">The styleName which should be referenced with this paragraph.</param>
        public Paragraph(IDocument document, string styleName)
        {
            Document = document;
            Node = new XElement(Ns.Text + "p");
            Init(styleName);
        }

        /// <summary>
        /// Overloaded constructor.
        /// Use this to create a standard paragraph with the given text from
        /// string simpletext. Notice, the text will be styled as standard.
        /// You won't be able to style it bold, underline, etc. this will only
        /// occur if standard style attributes of the texdocumentocument are set to
        /// this.
        /// </summary>
        /// <param name="document">The IDocument.</param>
        /// <param name="style">The only accepted ParentStyle is Standard! All other styles will be ignored!</param>
        /// <param name="simpletext">The text which should be append within this paragraph.</param>
        public Paragraph(IDocument document, ParentStyles style, string simpletext)
        {
            Document = document;
            Node = new XElement(Ns.Text + "p");
            if (style == ParentStyles.Standard)
                Init(ParentStyles.Standard.ToString());
            else if (style == ParentStyles.Table)
                Init(ParentStyles.Table.ToString());
            else if (style == ParentStyles.Text_20_body)
                Init(ParentStyles.Text_20_body.ToString());

            //Attach simple text withhin the paragraph
            if (simpletext != null)
                TextContent.Add(new SimpleText(Document, simpletext));
            _parentStyle = style;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paragraph"/> class.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="document">The document.</param>
        public Paragraph(XElement node, IDocument document)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        /// <summary>
        /// Gets the content of the mixed.
        /// </summary>
        /// <value>The content of the mixed.</value>
        public IList<IContent> MixedContent
        {
            get { return _mixedContent; }
        }

        /// <summary>
        /// Gets the parent style.
        /// </summary>
        /// <value>The parent style.</value>
        public ParentStyles ParentStyle
        {
            get { return _parentStyle; }
        }

        /// <summary>
        /// Gets or sets the paragraph style.
        /// </summary>
        /// <value>The paragraph style.</value>
        public ParagraphStyle ParagraphStyle
        {
            get { return (ParagraphStyle) Style; }
            set { Style = value; }
        }

        /// <summary>
        /// Create the Paragraph.
        /// </summary>
        /// <param name="styleName">The style name.</param>
        private void Init(string styleName)
        {
            if (styleName != "Standard"
                && styleName != "Table_20_Contents"
                && styleName != "Text_20_body")
            {
                Style = new ParagraphStyle(Document, styleName);
                Document.Styles.Add(Style);
            }
            InitStandards();
            StyleName = styleName;
        }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            TextContent = new ITextCollection();
            Content = new ContentCollection();
            _mixedContent = new List<IContent>();

            if (Document is TextDocument)
                Document.DocumentMetadata.ParagraphCount += 1;

            TextContent.Inserted += TextContent_Inserted;
            Content.Inserted += Content_Inserted;
            TextContent.Removed += TextContent_Removed;
            Content.Removed += Content_Removed;
        }

        /// <summary>
        /// Append the xml from added IText object.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        private void TextContent_Inserted(int index, object value)
        {
            Node.Add(((IText) value).Node);
            _mixedContent.Add((IText)value);

            if (((IText) value).Text != null)
            {
                try
                {
                    if (Document is TextDocument)
                    {
                        string text = ((IText) value).Text;
                        Document.DocumentMetadata.CharacterCount += text.Length;
                        string[] words = text.Split(' ');
                        Document.DocumentMetadata.WordCount += words.Length;
                    }
                }
                catch (Exception)
                {
                    //unhandled, only word and character count wouldn' be correct
                }
            }
        }

        /// <summary>
        /// Content_s the inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Inserted(int index, object value)
        {
            Node.Add(((IContent) value).Node);
            _mixedContent.Add((IContent)value);
        }

        /// <summary>
        /// Texts the content_ removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void TextContent_Removed(int index, object value)
        {
            ((IText) value).Node.Remove();
            RemoveMixedContent(value);
        }

        /// <summary>
        /// Content_s the removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Removed(int index, object value)
        {
            ((IContent) value).Node.Remove();
            RemoveMixedContent(value);
        }

        /// <summary>
        /// Removes the mixed content
        /// </summary>
        /// <param name="value">The value.</param>
        private void RemoveMixedContent(object value)
        {
            if (_mixedContent.Contains((IContent)value))
                _mixedContent.Remove((IContent)value);
        }

        #region IHtml Member

        /// <summary>
        /// Return the content as Html string
        /// </summary>
        /// <returns>The html string</returns>
        public string GetHtml()
        {
            string html = "<p ";
            string textStyle;
            bool useSpan = false;
            bool useGlobal = false;

            if (Style != null)
            {
                if (((ParagraphStyle) Style).ParentStyle == "Heading"
                    && ParagraphStyle.ParagraphProperties == null
                    && ParagraphStyle.TextProperties == null)
                    useGlobal = true;
                else
                {
                    if (ParagraphStyle.ParagraphProperties != null)
                        html += ParagraphStyle.ParagraphProperties.GetHtmlStyle();

                    if (ParagraphStyle.TextProperties != null)
                    {
                        textStyle = ParagraphStyle.TextProperties.GetHtmlStyle();
                        if (textStyle.Length > 0)
                        {
                            html += "<span " + textStyle;
                            useSpan = true;
                        }
                    }
                }
            }
            else
                useGlobal = true;

            if (useGlobal)
            {
                string global = GetHtmlStyleFromGlobalStyles();
                if (global.Length > 0)
                    html += global;
            }

            html += ">\n";

            //There check all content if they
            //support HTML
            foreach (IContent content in _mixedContent)
            {
                if (content is IHtml)
                {
                    string text = ((IHtml) content).GetHtml();
                    html += text;
                }
            }

            if (useSpan)
                return html + "</span>&nbsp;</p>\n";
            return html + "&nbsp;</p>\n";

//			if (this.TextContent.Count > 0)
//			{
//				if (useSpan)
//					return html + this.GetTextHtmlContent()+"</span></p>\n";
//				else
//					return html + this.GetTextHtmlContent()+"</p>\n";
//			}
//			else
//			{
//				string text		= this.GetContentHtmlContent();
//				text			= (text!=String.Empty) ? text : "&nbsp;";
//
//				if (useSpan)
//					return html + text +"</span></p>\n";
//				else
//					return html + text +"</p>\n";
//			}
        }

        /// <summary>
        /// Gets the content of the text HTML.
        /// </summary>
        /// <returns>Textcontent as Html string</returns>
        private string GetTextHtmlContent()
        {
            string html = "";

            foreach (IText itext in TextContent)
            {
                if (itext is IHtml)
                    html += ((IHtml) itext).GetHtml() + "\n";
            }

            return html;
        }

        /// <summary>
        /// Gets the content of the text HTML.
        /// </summary>
        /// <returns>Textcontent as Html string</returns>
        private string GetContentHtmlContent()
        {
            string html = "";

            foreach (IContent icontent in Content)
            {
                if (icontent is IHtml)
                    html += ((IHtml) icontent).GetHtml();
            }

            return html;
        }

        /// <summary>
        /// Gets the HTML style from global styles.
        /// This isn't supported by AODL yet. But if
        /// OpenDocument text documents are loaded,
        /// this could be.
        /// </summary>
        /// <returns>The style from Global Styles</returns>
        private string GetHtmlStyleFromGlobalStyles()
        {
            try
            {
                string style = "style=\"";

                if (Document is TextDocument)
                {
                    XElement styleNode =
                        ((TextDocument) Document).DocumentStyles.Styles.Descendants(Ns.Office + "styles")
                            .Elements(Ns.Style + "style")
                            .Where(e => string.Equals((string) e.Attribute(Ns.Style + "name"), StyleName)).
                            FirstOrDefault();

                    if (styleNode == null)
                        styleNode = ((TextDocument) Document).DocumentStyles.Styles.Descendants(Ns.Office + "styles")
                            .Elements(Ns.Style + "style")
                            .Where(
                            e =>
                            string.Equals((string) e.Attribute(Ns.Style + "name"), ((ParagraphStyle) Style).ParentStyle))
                            .FirstOrDefault();

                    if (styleNode != null)
                    {
                        XElement paraPropNode = styleNode.Element(Ns.Style + "paragraph-properties");

                        //Last change via parent style
                        XAttribute parentNode = styleNode.Attribute(Ns.Style + "parent-style-name");

                        XElement paraPropNodeP = null;
                        XElement parentStyleNode = null;
                        if (parentNode != null)
                            if (parentNode.Value != null)
                            {
                                //Console.WriteLine("Parent-Style-Name: {0}", parentNode.InnerText);
                                parentStyleNode =
                                    ((TextDocument) Document).DocumentStyles.Styles.Descendants(Ns.Office + "styles")
                                        .Elements(Ns.Style + "style")
                                        .Where(
                                        e => string.Equals((string) e.Attribute(Ns.Style + "name"), parentNode.Value)).
                                        FirstOrDefault();

                                if (parentStyleNode != null)
                                    paraPropNodeP = parentStyleNode.Element(Ns.Style + "paragraph-properties");
                            }


                        //Check first parent style paragraph properties
                        if (paraPropNodeP != null)
                        {
                            //Console.WriteLine("ParentStyleNode: {0}", parentStyleNode.OuterXml);
                            string alignMent = (string) paraPropNodeP.Attribute(Ns.Fo + "text-align");
                            if (alignMent != null)
                            {
                                alignMent = alignMent.ToLower().Replace("end", "right");
                                if (alignMent.ToLower() == "center" || alignMent.ToLower() == "right")
                                    style += "text-align: " + alignMent + "; ";
                            }

                            string lineSpace = (string) paraPropNodeP.Attribute(Ns.Fo + "line-height");
                            if (lineSpace != null)
                                style += "line-height: " + lineSpace + "; ";

                            string marginTop = (string) paraPropNodeP.Attribute(Ns.Fo + "margin-top");
                            if (marginTop != null)
                                style += "margin-top: " + marginTop + "; ";

                            string marginBottom = (string) paraPropNodeP.Attribute(Ns.Fo + "margin-bottom");
                            if (marginBottom != null)
                                style += "margin-bottom: " + marginBottom + "; ";

                            string marginLeft = (string) paraPropNodeP.Attribute(Ns.Fo + "margin-left");
                            if (marginLeft != null)
                                style += "margin-left: " + marginLeft + "; ";

                            string marginRight = (string) paraPropNodeP.Attribute(Ns.Fo + "margin-right");
                            if (marginRight != null)
                                style += "margin-right: " + marginRight + "; ";
                        }
                        //Check paragraph properties, maybe parents style is overwritten or extended
                        if (paraPropNode != null)
                        {
                            string alignMent = (string) paraPropNode.Attribute(Ns.Fo + "text-align");
                            if (alignMent != null)
                            {
                                alignMent = alignMent.ToLower().Replace("end", "right");
                                if (alignMent.ToLower() == "center" || alignMent.ToLower() == "right")
                                    style += "text-align: " + alignMent + "; ";
                            }

                            string lineSpace = (string) paraPropNode.Attribute(Ns.Fo + "line-height");
                            if (lineSpace != null)
                                style += "line-height: " + lineSpace + "; ";

                            string marginTop = (string) paraPropNode.Attribute(Ns.Fo + "margin-top");
                            if (marginTop != null)
                                style += "margin-top: " + marginTop + "; ";

                            string marginBottom = (string) paraPropNode.Attribute(Ns.Fo + "margin-bottom");
                            if (marginBottom != null)
                                style += "margin-bottom: " + marginBottom + "; ";

                            string marginLeft = (string) paraPropNode.Attribute(Ns.Fo + "margin-left");
                            if (marginLeft != null)
                                style += "margin-left: " + marginLeft + "; ";

                            string marginRight = (string) paraPropNode.Attribute(Ns.Fo + "margin-right");
                            if (marginRight != null)
                                style += "margin-right: " + marginRight + "; ";
                        }

                        XElement textPropNode = styleNode.Element(Ns.Style + "text-properties");

                        XElement textPropNodeP = null;
                        if (parentStyleNode != null)
                            textPropNodeP = parentStyleNode.Element(Ns.Style + "text-properties");

                        //Check first text properties of parent style
                        if (textPropNodeP != null)
                        {
                            string fontSize = (string) textPropNodeP.Attribute(Ns.Fo + "font-size");
                            if (fontSize != null)
                                style += "font-size: " + FontFamilies.PtToPx(fontSize) + "; ";

                            string italic = (string) textPropNodeP.Attribute(Ns.Fo + "font-style");
                            if (italic != null)
                                style += "font-size: italic; ";

                            string bold = (string) textPropNodeP.Attribute(Ns.Fo + "font-weight");
                            if (bold != null)
                                style += "font-weight: bold; ";

                            string underline = (string) textPropNodeP.Attribute(Ns.Style + "text-underline-style");
                            if (underline != null)
                                style += "text-decoration: underline; ";

                            string fontName = (string) textPropNodeP.Attribute(Ns.Style + "font-name");
                            if (fontName != null)
                                style += "font-family: " + FontFamilies.HtmlFont(fontName) + "; ";

                            string color = (string) textPropNodeP.Attribute(Ns.Fo + "color");
                            if (color != null)
                                style += "color: " + color + "; ";
                        }
                        //Check now text properties of style, maybe some setting are overwritten or extended
                        if (textPropNode != null)
                        {
                            string fontSize = (string) textPropNode.Attribute(Ns.Fo + "font-size");
                            if (fontSize != null)
                                style += "font-size: " + FontFamilies.PtToPx(fontSize) + "; ";

                            string italic = (string) textPropNode.Attribute(Ns.Fo + "font-style");
                            if (italic != null)
                                style += "font-size: italic; ";

                            string bold = (string) textPropNode.Attribute(Ns.Fo + "font-weight");
                            if (bold != null)
                                style += "font-weight: bold; ";

                            string underline = (string) textPropNode.Attribute(Ns.Style + "text-underline-style");
                            if (underline != null)
                                style += "text-decoration: underline; ";

                            string fontName = (string) textPropNode.Attribute(Ns.Style + "font-name");
                            if (fontName != null)
                                style += "font-family: " + FontFamilies.HtmlFont(fontName) + "; ";

                            string color = (string) textPropNode.Attribute(Ns.Fo + "color");
                            if (color != null)
                                style += "color: " + color + "; ";
                        }
                    }
                }

                if (!style.EndsWith("; "))
                    style = "";
                else
                    style += "\"";

                return style;
            }
            catch (Exception)
            {
                //unhandled, only a paragraph style wouldn't be displayed correct
                //Console.WriteLine("GetHtmlStyleFromGlobalStyles(): {0}", ex.Message);
            }

            return "";
        }

        #endregion

        #region ITextContainer Member

        private ITextCollection _textContent;

        /// <summary>
        /// All Content objects have a Text container. Which represents
        /// his Text this could be SimpleText, FormatedText or mixed.
        /// </summary>
        /// <value></value>
        public ITextCollection TextContent
        {
            get { return _textContent; }
            set
            {
                if (_textContent != null)
                    foreach (IText text in _textContent)
                        text.Node.Remove();

                _textContent = value;

                if (_textContent != null)
                    foreach (IText text in _textContent)
                        Node.Add(text.Node);
            }
        }

        #endregion

        #region ICloneable Member

        /// <summary>
        /// Create a deep clone of this paragraph object.
        /// </summary>
        /// <remarks>A possible Attached Style wouldn't be cloned!</remarks>
        /// <returns>
        /// A clone of this object.
        /// </returns>
        public object Clone()
        {
            Paragraph pargaphClone = null;

            if (Document != null && Node != null)
            {
                MainContentProcessor mcp = new MainContentProcessor(Document);
                pargaphClone = mcp.CreateParagraph(new XElement(Node));
            }

            return pargaphClone;
        }

        #endregion

        #region IContentContainer Member

        private ContentCollection _content;

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content
        {
            get { return _content; }
            set
            {
                if (_content != null)
                    foreach (IContent content in _content)
                        content.Node.Remove();

                _content = value;

                if (_content != null)
                    foreach (IContent content in _content)
                        Node.Add(content.Node);
            }
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
    }
}

/*
 * $Log: Paragraph.cs,v $
 * Revision 1.3  2008/04/29 15:39:46  mt
 * new copyright header
 *
 * Revision 1.2  2007/04/08 16:51:23  larsbehr
 * - finished master pages and styles for text documents
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:39  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
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
 * Revision 1.1  2006/01/29 11:28:22  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.11  2006/01/05 10:31:10  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 * Revision 1.10  2005/12/18 18:29:46  larsbm
 * - AODC Gui redesign
 * - AODC HTML exporter refecatored
 * - Full Meta Data Support
 * - Increase textprocessing performance
 *
 * Revision 1.9  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.8  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 * Revision 1.7  2005/10/23 16:47:48  larsbm
 * - Bugfix ListItem throws IStyleInterface not implemented exeption
 * - now. build the document after call saveto instead prepare the do at runtime
 * - add remove support for IText objects in the paragraph class
 *
 * Revision 1.6  2005/10/22 10:47:41  larsbm
 * - add graphic support
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
 * Revision 1.2  2005/10/08 08:19:25  larsbm
 * - added cvs tags
 *
 */