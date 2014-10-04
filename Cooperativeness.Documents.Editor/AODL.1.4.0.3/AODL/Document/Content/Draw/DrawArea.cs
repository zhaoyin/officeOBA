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
    // @@@@ Add EventListeners to DrawArea
    // @@@@ draw:area-circle attributes: svg:cx, svg:cy, svg:r
    // @@@@ draw:area-polygon:  omit
    // 

    /// <summary>
    /// DrawAreaRectangle represent draw area rectangle which
    /// could be used within a ImageMap.
    /// </summary>
    public class DrawAreaRectangle : DrawArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawAreaRectangle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public DrawAreaRectangle(IDocument document, XElement node) : base(document)
        {
            Node = node;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawAreaRectangle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public DrawAreaRectangle(IDocument document,
                                 string x, string y, string width, string height)
            : this(document, new XElement(Ns.Draw + "area-rectangle"))
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawAreaRectangle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="listeners">The listeners.</param>
        public DrawAreaRectangle(IDocument document,
                                 string x, string y, string width, string height,
                                 IContent listeners)
            : this(document, x, y, width, height)
        {
            if (listeners != null)
            {
                Content.Add(listeners);
            }
        }

        /// <summary>
        /// Gets or sets the x-position.
        /// </summary>
        /// <value>The description of the area-position.</value>
        public string X
        {
            get { return (string) Node.Attribute(Ns.Svg + "x"); }
            set { Node.SetAttributeValue(Ns.Svg + "x", value); }
        }

        /// <summary>
        /// Gets or sets the y-position.
        /// </summary>
        /// <value>The y-position of the area-rectangle.</value>
        public string Y
        {
            get { return (string) Node.Attribute(Ns.Svg + "y"); }
            set { Node.SetAttributeValue(Ns.Svg + "y", value); }
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width of the area-rectangle.</value>
        public string Width
        {
            get { return (string) Node.Attribute(Ns.Svg + "width"); }
            set { Node.SetAttributeValue(Ns.Svg + "width", value); }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height of the area-rectangle.</value>
        public string Height
        {
            get { return (string) Node.Attribute(Ns.Svg + "height"); }
            set { Node.SetAttributeValue(Ns.Svg + "height", value); }
        }
    }

    /// <summary>
    /// Summary for DrawAreaRectangle.
    /// </summary>
    public class DrawAreaCircle : DrawArea
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawAreaCircle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public DrawAreaCircle(IDocument document, XElement node) : base(document)
        {
            Node = node;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawAreaCircle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="cx">The cx.</param>
        /// <param name="cy">The cy.</param>
        /// <param name="radius">The radius.</param>
        public DrawAreaCircle(IDocument document,
                              string cx, string cy, string radius)
            : this(document, new XElement(Ns.Draw + "area-circle"))
        {
            CX = cx;
            CY = cy;
            Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawAreaCircle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="cx">The cx.</param>
        /// <param name="cy">The cy.</param>
        /// <param name="radius">The radius.</param>
        /// <param name="listeners">The listeners.</param>
        public DrawAreaCircle(IDocument document,
                              string cx, string cy, string radius, IContent listeners)
            : this(document, cx, cy, radius)
        {
            if (listeners != null)
            {
                Content.Add(listeners);
            }
        }

        /// <summary>
        /// Gets or sets the cx-position.
        /// </summary>
        /// <value>The center of the area-circle.</value>
        public string CX
        {
            get { return (string) Node.Attribute(Ns.Svg + "cx"); }
            set { Node.SetAttributeValue(Ns.Svg + "cx", value); }
        }

        /// <summary>
        /// Gets or sets the cy-position.
        /// </summary>
        /// <value>The center position of the area-cicle.</value>
        public string CY
        {
            get { return (string) Node.Attribute(Ns.Svg + "cy"); }
            set { Node.SetAttributeValue(Ns.Svg + "cy", value); }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius of the area-circle.</value>
        public string Radius
        {
            get { return (string) Node.Attribute(Ns.Svg + "r"); }
            set { Node.SetAttributeValue(Ns.Svg + "r", value); }
        }
    }

    /// <summary>
    /// Summary for DrawArea.
    /// </summary>
    public abstract class DrawArea : IContent, IContentContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawArea"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        protected DrawArea(IDocument document)
        {
            Document = document;
            Content = new ContentCollection();
            Content.Inserted += Content_Inserted;
            Content.Removed += Content_Removed;
        }

        /// <summary>
        /// Gets or sets the href. e.g http://www.sourceforge.net
        /// </summary>
        /// <value>The href.</value>
        public string Href
        {
            get { return (string) Node.Attribute(Ns.XLink + "href"); }
            set { Node.SetAttributeValue(Ns.XLink + "href", value); }
        }

        /// <summary>
        /// Gets or sets the type of the X link.
        /// </summary>
        /// <value>The type of the X link.</value>
        public string XLinkType
        {
            get { return (string) Node.Attribute(Ns.XLink + "type"); }
            set { Node.SetAttributeValue(Ns.XLink + "type", value); }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description of the draw area.</value>
        public string Description
        {
            get { return (string) Node.Attribute(Ns.Draw + "desc"); }
            set { Node.SetAttributeValue(Ns.Draw + "desc", value); }
        }

        /// <summary>
        /// Content_s the inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        protected void Content_Inserted(int index, object value)
        {
            if (Node != null)
                Node.Add(((IContent) value).Node);
        }

        /// <summary>
        /// Content_s the removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        protected void Content_Removed(int index, object value)
        {
            ((IContent) value).Node.Remove();
        }

        #region IContent Member		

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public XElement Node { get; set; }

        /// <summary>
        /// Every object (typeof(IContent)) have to know his document.
        /// </summary>
        /// <value></value>
        public IDocument Document { get; set; }

        /// <summary>
        /// A draw:area-rectangle doesn't have a style-name.
        /// </summary>
        /// <value>The name</value>
        public string StyleName
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// A draw:area-rectangle doesn't have a style.
        /// </summary>
        public IStyle Style
        {
            get { return null; }
            set { }
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

        #region IContentCollection Member

        /// <summary>
        /// Gets or sets the content collection.
        /// </summary>
        /// <value>The content collection.</value>
        public ContentCollection Content { get; set; }

        #endregion

        #region IContent Member old

//		private IDocument _document;
//		/// <summary>
//		/// The IDocument to which this draw-area is bound.
//		/// </summary>
//		public IDocument Document
//		{
//			get
//			{
//				return this._document;
//			}
//			set
//			{
//				this._document = value;
//			}
//		}
//
//		private XElement _node;
//		/// <summary>
//		/// The XElement.
//		/// </summary>
//		public XElement Node
//		{
//			get
//			{
//				return this._node;
//			}
//			set
//			{
//				this._node = value;
//			}
//		}
//
//		/// <summary>
//		/// A draw:area-rectangle doesn't have a style-name.
//		/// </summary>
//		/// <value>The name</value>
//		public string Stylename
//		{
//			get
//			{
//				return null;
//			}
//			set
//			{
//			}
//		}
//
//		/// <summary>
//		/// A draw:area-rectangle doesn't have a style.
//		/// </summary>
//		public IStyle Style
//		{
//			get
//			{
//				return null;
//			}
//			set
//			{
//			}
//		}
//
//		/// <summary>
//		/// A draw:area-rectangle doesn't contain text.
//		/// </summary>
//		public ITextCollection TextContent
//		{
//			get
//			{
//				return null;
//			}
//			set
//			{
//			}
//		}

        #endregion
    }
}