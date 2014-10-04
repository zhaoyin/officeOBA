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

using System.Linq;
using System.Xml.Linq;
using AODL.Document.Styles;

namespace AODL.Document.Content.Text
{
    /// <summary>
    /// Represent a Footnote which could be 
    /// a Foot- or a Endnote
    /// </summary>
    public class Footnote : IText, IHtml, ITextContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Footnote"/> class.
        /// </summary>
        /// <param name="document">The text document.</param>
        /// <param name="notetext">The notetext.</param>
        /// <param name="id">The id.</param>
        /// <param name="type">The type.</param>
        public Footnote(IDocument document, string notetext, string id, FootnoteType type)
        {
            Document = document;
            Node = new XElement(Ns.Text + "note");

            XAttribute xa = new XAttribute(Ns.Text + "id", "ftn" + id);
            Node.Add(xa);

            xa = new XAttribute(Ns.Text + "note-class", type.ToString());
            Node.Add(xa);

            //Node citation
            XElement node = new XElement(Ns.Text + "note-citation", id);

            Node.Add(node);

            //Node Footnode body
            XElement nodebody = new XElement(Ns.Text + "note-body");

            //Node Footnode text
            node = new XElement(Ns.Text + "p", notetext);

            xa = new XAttribute(Ns.Text + "style-name", (type == FootnoteType.Footnode) ? "Footnote" : "Endnote");
            node.Add(xa);

            nodebody.Add(node);

            Node.Add(nodebody);
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Footnote"/> class.
        /// </summary>
        /// <param name="document">The text document.</param>
        public Footnote(IDocument document)
        {
            Document = document;
            InitStandards();
        }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public string Id
        {
            get
            {
                XAttribute attribute = Node.DescendantsAndSelf().Attributes(Ns.Text + "note-citation").FirstOrDefault();
                if (attribute == null)
                    return null;
                return attribute.Value;
            }
            set
            {
                XAttribute attribute = Node.DescendantsAndSelf().Attributes(Ns.Text + "note-citation").FirstOrDefault();
                if (attribute == null) return;
                attribute.Value = value;
                attribute = Node.DescendantsAndSelf().Attributes(Ns.Text + "id").FirstOrDefault();
                attribute.Value = "ftn" + value;
            }
        }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            TextContent = new ITextCollection();
            TextContent.Inserted += TextContent_Inserted;
            TextContent.Removed += TextContent_Removed;
        }

        /// <summary>
        /// Texts the content_ inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void TextContent_Inserted(int index, object value)
        {
            Node.Add(((IText) value).Node);
        }

        /// <summary>
        /// Texts the content_ removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void TextContent_Removed(int index, object value)
        {
            ((IText) value).Node.Remove();
        }

        #region IText Member

        /// <summary>
        /// The node that represent the text content.
        /// </summary>
        /// <value></value>
        public XElement Node { get; set; }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        XNode IContent.Node
        {
            get { return Node; }
            set { Node = (XElement) value; }
        }

        /// <summary>
        /// The text.
        /// </summary>
        /// <value></value>
        public string Text
        {
            get { return Node.Value; }
            set { Node.Value = value; }
        }

        /// <summary>
        /// The document to which this text content belongs to.
        /// </summary>
        /// <value></value>
        public IDocument Document { get; set; }

        /// <summary>
        /// Is null no style is available.
        /// </summary>
        /// <value></value>
        public IStyle Style
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// No style name available
        /// </summary>
        /// <value></value>
        public string StyleName
        {
            get { return null; }
            set { }
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

        #region IHtml Member

        /// <summary>
        /// Return the content as Html string
        /// </summary>
        /// <returns>The html string</returns>
        public string GetHtml()
        {
            string html = "<sup>(";
            html += Id;
            html += ". " + Text;
            html += ")</sup>";

            return html;
        }

        #endregion
    }

    /// <summary>
    /// Represent the possible footnodes
    /// </summary>
    public enum FootnoteType
    {
        /// <summary>
        /// A footnode
        /// </summary>
        Footnode,
        /// <summary>
        /// A endnote
        /// </summary>
        Endnote
    }
}

/*
 * $LoG$
 */