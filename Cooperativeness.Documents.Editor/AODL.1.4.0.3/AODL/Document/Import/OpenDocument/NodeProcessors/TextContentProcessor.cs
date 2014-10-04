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
using System.Xml.Linq;
using AODL.Document.Content.Text;
using AODL.Document.Content.Text.Indexes;
using AODL.Document.Content.Text.TextControl;
using AODL.Document.Exceptions;
using AODL.Document.Styles;

namespace AODL.Document.Import.OpenDocument.NodeProcessors
{
    /// <summary>
    /// Represent a Text Content Processor.
    /// </summary>
    public class TextContentProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextContentProcessor"/> class.
        /// </summary>
        /// <summary>
        /// Creates the text object.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="aTextNode">A text node.</param>
        /// <returns></returns>
        public IText CreateTextObject(IDocument document, XNode aTextNode)
        {
            //aTextNode.InnerText				= this.ReplaceSpecialCharacter(aTextNode.InnerText);
            if (aTextNode is XText)
                return new SimpleText(document, ((XText) aTextNode).Value);

            if (!(aTextNode is XElement))
                return null;

            XElement textElement = (XElement) aTextNode;
            if (textElement.Name.Namespace != Ns.Text)
                return null;
            switch (textElement.Name.LocalName)
            {
                case "span":
                    return CreateFormatedText(document, textElement);
                case "bookmark":
                    return CreateBookmark(document, textElement, BookmarkType.Standard);
                case "bookmark-start":
                    return CreateBookmark(document, textElement, BookmarkType.Start);
                case "bookmark-end":
                    return CreateBookmark(document, textElement, BookmarkType.End);
                case "a":
                    return CreateXLink(document, textElement);
                case "note":
                    return CreateFootnote(document, textElement);
                case "line-break":
                    return new LineBreak(document);
                case "s":
                    return new WhiteSpace(document, new XElement(textElement));
                case "tab":
                    return new TabStop(document);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates the formated text.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public FormatedText CreateFormatedText(IDocument document, XElement node)
        {
            //Create a new FormatedText object
            FormatedText formatedText = new FormatedText(document, node);
            ITextCollection iTextColl = new ITextCollection();
            formatedText.Document = document;
            formatedText.Node = node;
            //Recieve a TextStyle

            IStyle textStyle = document.Styles.GetStyleByName(formatedText.StyleName);

            if (textStyle != null)
                formatedText.Style = textStyle;
            //else
            //{
            //	IStyle iStyle				= document.CommonStyles.GetStyleByName(formatedText.StyleName);
            //}

            //Ceck for more IText object
            foreach (XNode iTextNode in node.Nodes())
            {
                XNode clone = iTextNode is XElement
                                  ? (XNode) new XElement((XElement) iTextNode)
                                  : new XText((XText) iTextNode);
                IText iText = CreateTextObject(document, clone);
                if (iText != null)
                {
                    iTextColl.Add(iText);
                }
                else
                    iTextColl.Add(new UnknownTextContent(document, (XElement) iTextNode));
            }

            formatedText.Node.Value = "";

            foreach (IText iText in iTextColl)
                formatedText.TextContent.Add(iText);

            return formatedText;
        }

        /// <summary>
        /// Creates the bookmark.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public Bookmark CreateBookmark(IDocument document, XElement node, BookmarkType type)
        {
            try
            {
                Bookmark bookmark;
                if (type == BookmarkType.Standard)
                    bookmark = new Bookmark(document, BookmarkType.Standard, "noname");
                else if (type == BookmarkType.Start)
                    bookmark = new Bookmark(document, BookmarkType.Start, "noname");
                else
                    bookmark = new Bookmark(document, BookmarkType.End, "noname");

                bookmark.Node = new XElement(node);

                return bookmark;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Bookmark.", ex);
            }
        }

        /// <summary>
        /// Creates the X link.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public XLink CreateXLink(IDocument document, XElement node)
        {
            try
            {
                XLink xlink = new XLink(document) {Node = new XElement(node)};
                ITextCollection iTxtCol = new ITextCollection();

                foreach (XNode nodeText in xlink.Node.Nodes())
                {
                    IText iText = CreateTextObject(xlink.Document, nodeText);
                    if (iText != null)
                        iTxtCol.Add(iText);
                }

                xlink.Node.Value = "";

                foreach (IText iText in iTxtCol)
                    xlink.TextContent.Add(iText);

                return xlink;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a XLink.", ex);
            }
        }

        /// <summary>
        /// Creates the footnote.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public Footnote CreateFootnote(IDocument document, XElement node)
        {
            try
            {
                Footnote fnote = new Footnote(document) {Node = new XElement(node)};

                return fnote;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Footnote.", ex);
            }
        }

        /// <summary>
        /// Creates the text sequence.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        public TextSequence CreateTextSequence(IDocument document, XElement node)
        {
            try
            {
                TextSequence textSequence = new TextSequence(document, node);

                return textSequence;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a TextSequence.", ex);
            }
        }

        /// <summary>
        /// Replaces the special character.
        /// </summary>
        /// <param name="nodeInnerText">The node inner text.</param>
        /// <returns></returns>
        private string ReplaceSpecialCharacter(string nodeInnerText)
        {
            nodeInnerText = nodeInnerText.Replace("<", "&lt;");
            nodeInnerText = nodeInnerText.Replace(">", "&gt;");

            return nodeInnerText;
        }
    }
}