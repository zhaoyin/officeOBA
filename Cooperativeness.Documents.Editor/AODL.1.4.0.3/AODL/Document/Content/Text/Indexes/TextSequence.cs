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

using System.Xml.Linq;
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.Styles;

namespace AODL.Document.Content.Text.Indexes
{
    /// <summary>
    /// Zusammenfassung für TextSequence.
    /// </summary>
    public class TextSequence : IText, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextSequence"/> class.
        /// </summary>
        public TextSequence()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextSequence"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public TextSequence(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextSequence"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public TextSequence(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Text + "sequence");
            InitStandards();
        }

        /// <summary>
        /// Gets or sets the ref name.
        /// e.g. for a Illustration refIllustration0
        /// </summary>
        /// <value>The name of the ref.</value>
        public string RefName
        {
            get { return (string) Node.Attribute(Ns.Text + "ref-name"); }
            set { Node.SetAttributeValue(Ns.Text + "ref-name", value); }
        }

        /// <summary>
        /// Gets or sets the name of the TextSequence.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return (string) Node.Attribute(Ns.Text + "name"); }
            set { Node.SetAttributeValue(Ns.Text + "name", value); }
        }

        /// <summary>
        /// Gets or sets the num format.
        /// e.g. 1, I, A ..
        /// </summary>
        /// <value>The num format.</value>
        public string NumFormat
        {
            get { return (string) Node.Attribute(Ns.Text + "num-format"); }
            set { Node.SetAttributeValue(Ns.Text + "num-format", value); }
        }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        /// <value>The formula.</value>
        public string Formula
        {
            get { return (string) Node.Attribute(Ns.Text + "formula"); }
            set { Node.SetAttributeValue(Ns.Text + "formula", value); }
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

        #region ICloneable Member

        /// <summary>
        /// Create a deep clone of this FormatedText object.
        /// </summary>
        /// <remarks>A possible Attached Style wouldn't be cloned!</remarks>
        /// <returns>
        /// A clone of this object.
        /// </returns>
        public object Clone()
        {
            FormatedText formatedTextClone = null;

            if (Document != null && Node != null)
            {
                TextContentProcessor tcp = new TextContentProcessor();
                formatedTextClone = tcp.CreateFormatedText(
                    Document, new XElement(Node));
            }

            return formatedTextClone;
        }

        #endregion

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
        /// Use this if use text without control character,
        /// otherwise use the the TextColllection TextContent. 
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
        /// Not supported. A TextSequence doesn't has a style
        /// </summary>
        /// <value></value>
        public IStyle Style
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Not supported. A TextSequence doesn't has a style
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
    }
}