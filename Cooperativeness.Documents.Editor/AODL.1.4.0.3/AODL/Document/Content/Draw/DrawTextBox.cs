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
using AODL.Document.Styles;

namespace AODL.Document.Content.Draw
{
    /// <summary>
    /// DrawTextBox represent a draw text box which could be e.g. used
    /// to host graphic frame
    /// </summary>
    public class DrawTextBox : IContent, IContentContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawTextBox"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public DrawTextBox(IDocument document, XElement node)
        {
            Document = document;
            Content = new ContentCollection();
            Content.Inserted += Content_Inserted;
            Content.Removed += Content_Removed;
            Node = node;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawTextBox"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public DrawTextBox(IDocument document) : this(document, new XElement(Ns.Draw + "text-box"))
        {
        }

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
            get { return (string) Node.Attribute(Ns.Draw + "style-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "style-name", value); }
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
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content { get; set; }

        #endregion

        /// <summary>
        /// If the context of the text box exceedes it's capacity,
        /// the content flows into the next text box in the chain
        /// and get or set the name of the next text box within this property.
        /// [optional]
        /// </summary>
        /// <value>The chain.</value>
        public string Chain
        {
            get { return (string) Node.Attribute(Ns.Draw + "chain-next-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "chain-next-name", value); }
        }

        /// <summary>
        /// Gets or sets the corner radius.
        /// [optional]
        /// </summary>
        /// <value>The corner radius.</value>
        public string CornerRadius
        {
            get { return (string) Node.Attribute(Ns.Draw + "corner-radius"); }
            set { Node.SetAttributeValue(Ns.Draw + "corner-radius", value); }
        }

        /// <summary>
        /// Gets or sets the minimum height of the text box.
        /// [optional]
        /// </summary>
        /// <value>The height of the min.</value>
        public string MinHeight
        {
            get { return (string) Node.Attribute(Ns.Fo + "min-height"); }
            set { Node.SetAttributeValue(Ns.Fo + "min-height", value); }
        }

        /// <summary>
        /// Gets or sets the minimum height of the text box.
        /// [optional]
        /// </summary>
        /// <value>The height of the min.</value>
        public string MinWidth
        {
            get { return (string) Node.Attribute(Ns.Fo + "min-width"); }
            set { Node.SetAttributeValue(Ns.Fo + "min-width", value); }
        }

        /// <summary>
        /// Gets or sets the maximum width of the text box.
        /// [optional]
        /// </summary>
        /// <value>The height of the min.</value>
        public string MaxHeight
        {
            get { return (string) Node.Attribute(Ns.Fo + "max-height"); }
            set { Node.SetAttributeValue(Ns.Fo + "max-height", value); }
        }

        /// <summary>
        /// Gets or sets the maximum width of the text box.
        /// [optional]
        /// </summary>
        /// <value>The height of the min.</value>
        public string MaxWidth
        {
            get { return (string) Node.Attribute(Ns.Fo + "max-width"); }
            set { Node.SetAttributeValue(Ns.Fo + "max-width", value); }
        }

        /// <summary>
        /// Content_s the inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Inserted(int index, object value)
        {
            Node.Add(((IContent) value).Node);
        }

        /// <summary>
        /// Content_s the removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void Content_Removed(int index, object value)
        {
            ((IContent) value).Node.Remove();
        }
    }
}

/*
 * $Log: DrawTextBox.cs,v $
 * Revision 1.2  2008/04/29 15:39:44  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:32  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.1  2006/01/29 11:29:46  larsbm
 * *** empty log message ***
 *
 */