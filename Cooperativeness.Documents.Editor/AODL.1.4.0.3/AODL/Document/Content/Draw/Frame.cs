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
using AODL.Document.Content.Charts;
using AODL.Document.Content.EmbedObjects;
using AODL.Document.Exceptions;
using AODL.Document.Styles;
using AODL.IO;

namespace AODL.Document.Content.Draw
{
    public class AODLGraphicException : AODLException
    {
        public AODLGraphicException(string message, Exception e)
            : base(message, e)
        {
        }
    }


    /// <summary>
    /// Frame represent graphic resp. a draw container.
    /// </summary>
    public class Frame : IContent, IContentContainer
    {
        private int _dPI_X;
        private int _dPI_Y;
        private double _height;
        private int _heightInPixel;
        private string _measurementFormat;
        private string _realgraphicname;
        private double _width;
        private int _widthInPixel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
        /// <param name="document">The textdocument.</param>
        /// <param name="stylename">The stylename.</param>
        public Frame(IDocument document, string stylename)
        {
            Document = document;

            Node = new XElement(Ns.Draw + "frame");
            InitStandards();

            if (stylename != null)
            {
                Style = new FrameStyle(Document, stylename);
                StyleName = stylename;
                Document.Styles.Add(Style);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
        /// <param name="document">The textdocument.</param>
        /// <param name="stylename">The stylename.</param>
        /// <param name="drawName">The  draw name.</param>
        /// <param name="graphicfile">The graphicfile.</param>
        public Frame(IDocument document, string stylename, string drawName, IFile graphicfile)
        {
            Document = document;
            Node = new XElement(Ns.Draw + "frame");
            InitStandards();

            StyleName = stylename;
            //			this.AnchorType			= "paragraph";

            DrawName = drawName;
            GraphicSource = graphicfile;

            GraphicIdentifier = Guid.NewGuid();
            _realgraphicname = GraphicIdentifier
                               + LoadImageFromFile(graphicfile);
            Graphic graphic = new Graphic(Document, this, _realgraphicname);
            graphic.GraphicFile = GraphicSource;
            Content.Add(graphic);
            Style = new FrameStyle(Document, stylename);
            Document.Styles.Add(Style);
        }

        /// <summary>
        /// Gets or sets the frame style.
        /// </summary>
        /// <value>The frame style.</value>
        public FrameStyle FrameStyle
        {
            get { return (FrameStyle) Style; }
            set { Style = value; }
        }

        /// <summary>
        /// Gets the name of the real graphic.
        /// </summary>
        /// <value>The name of the real graphic.</value>
        public string RealGraphicName
        {
            get { return _realgraphicname; }
            set { _realgraphicname = value; }
        }

        /// <summary>
        /// Gets or sets the graphic source path.
        /// </summary>
        /// <value>The graphic source path.</value>
        public IFile GraphicSource { get; set; }

        /// <summary>
        /// Gets the image height in pixel.
        /// </summary>
        /// <value>The height in pixel.</value>
        public int HeightInPixel
        {
            get { return _heightInPixel; }
        }

        /// <summary>
        /// Gets the image width in pixel.
        /// </summary>
        /// <value>The width in pixel.</value>
        public int WidthInPixel
        {
            get { return _widthInPixel; }
        }

        /// <summary>
        /// Gets the frame height in cm or inch depending on which format is used in the current document.
        /// </summary>
        /// <value>The height.</value>
        public double Height
        {
            get { return _height; }
        }

        /// <summary>
        /// Gets the frame width in cm or inch depending on which format is used in the current document.
        /// </summary>
        /// <value>The height.</value>
        public double Width
        {
            get { return _width; }
        }

        /// <summary>
        /// Gets the measurement format. This will depent on the current document.
        /// Possible values are cm or in (inch).
        /// </summary>
        /// <value>The measurement format.</value>
        public string MeasurementFormat
        {
            get { return _measurementFormat; }
        }

        /// <summary>
        /// Gets the image vertical resulotion.
        /// </summary>
        /// <value>The vertical resulotion.</value>
        public int DPI_Y
        {
            get { return _dPI_Y; }
        }

        /// <summary>
        /// Gets the image horizontal resulotion.
        /// </summary>
        /// <value>The horízontal resulotion.</value>
        public int DPI_X
        {
            get { return _dPI_X; }
        }

        public string AlternateText
        {
            get { return _node.Value; }
            set { _node.Value = value; }
        }

        /// <summary>
        /// Gets or sets the draw name.
        /// </summary>
        /// <value>The name of the graphic.</value>
        public string DrawName
        {
            get { return (string) Node.Attribute(Ns.Draw + "name"); }
            set { Node.SetAttributeValue(Ns.Draw + "name", value); }
        }

        /// <summary>
        /// Gets or sets the type of the anchor. e.g paragraph
        /// </summary>
        /// <value>The type of the anchor.</value>
        public string AnchorType
        {
            get { return (string) Node.Attribute(Ns.Text + "anchor-type"); }
            set { Node.SetAttributeValue(Ns.Text + "anchor-type", value); }
        }

        /// <summary>
        /// Gets or sets the z index.
        /// </summary>
        /// <value>The index of the Z.</value>
        public string ZIndex
        {
            get { return (string) Node.Attribute(Ns.Draw + "z-index"); }
            set { Node.SetAttributeValue(Ns.Draw + "z-index", value); }
        }

        /// <summary>
        /// Gets or sets the width of the frame. e.g 2.98cm
        /// </summary>
        /// <value>The width of the graphic.</value>
        public string SvgWidth
        {
            get { return (string) Node.Attribute(Ns.Svg + "width"); }
            set { Node.SetAttributeValue(Ns.Svg + "width", value); }
        }

        /// <summary>
        /// Gets or sets the height of the frame. e.g 3.00cm
        /// </summary>
        /// <value>The height of the graphic.</value>
        public string SvgHeight
        {
            get { return (string) Node.Attribute(Ns.Svg + "height"); }
            set { Node.SetAttributeValue(Ns.Svg + "height", value); }
        }

        /// <summary>
        /// Gets or sets the horizontal position where
        /// the hosted drawing e.g Graphic should be
        /// anchored.
        /// </summary>
        /// <example>myFrame.SvgX = "1.5cm"</example>
        /// <value>The SVG X.</value>
        public string SvgX
        {
            get { return (string) Node.Attribute(Ns.Svg + "x"); }
            set { Node.SetAttributeValue(Ns.Svg + "x", value); }
        }

        /// <summary>
        /// Gets or sets the vertical position where
        /// the hosted drawing e.g Graphic should be
        /// anchored.
        /// </summary>
        /// <example>myFrame.SvgY = "1.5cm"</example>
        /// <value>The SVG Y.</value>
        public string SvgY
        {
            get { return (string) Node.Attribute(Ns.Svg + "y"); }
            set { Node.SetAttributeValue(Ns.Svg + "y", value); }
        }

        /// <summary>
        /// Gets or sets the graphic identifier.
        /// </summary>
        /// <value>The graphic identifier.</value>
        public Guid GraphicIdentifier { get; set; }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            //Todo: FrameBuilder
            AnchorType = "paragraph";

            Content = new ContentCollection();
            Content.Inserted += Content_Inserted;
            Content.Removed += Content_Removed;
        }

        /// <summary>
        /// Loads the image from file.
        /// </summary>
        /// <param name="graphicfile">The graphicfile.</param>
        public string LoadImageFromFile(IFile graphicfile)
        {
            ImageInfo imageInfo = new ImageInfo(graphicfile, SvgWidth, SvgHeight);
            _widthInPixel = imageInfo.WidthInPixel;
            _heightInPixel = imageInfo.HeightInPixel;
            _dPI_X = imageInfo.DPI_X;
            _dPI_Y = imageInfo.DPI_Y;
            _measurementFormat = imageInfo.MeasurementFormat;
            _width = imageInfo.Width;
            _height = imageInfo.Height;
            SvgWidth = imageInfo.SvgWidth;
            SvgHeight = imageInfo.SvgHeight;

            return imageInfo.Name;
        }

        /// <summary>
        /// Content_s the inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Inserted(int index, object value)
        {
            if (value is Graphic)
            {
                if (((Graphic) value).Frame == null)
                {
                    ((Graphic) value).Frame = this;
                    if (((Graphic) value).GraphicFile != null
                        && GraphicSource == null)
                        GraphicSource = ((Graphic) value).GraphicFile;
                    Node.Add(((IContent) value).Node);
                }
                if (((IContent) value).Node != null)
                {
                    if (((IContent) value).Node.Parent == null
                        || !((IContent) value).Node.Parent.Equals(Node))
                    {
                        Node.Add(((IContent) value).Node);
                    }
                }
            }
            else if (value is EmbedObject)
            {
                if (((EmbedObject) value).ObjectType == "chart")
                {
                    if (Document.IsLoadedFile && !((Chart) value).IsNewed)
                    {
                        Node.Add(((Chart) value).ParentNode);
                    }
                    else
                    {
                        string objectLink = "." + @"/" + ((EmbedObject) value).ObjectName;

                        Node.Add(((Chart) value).CreateParentNode(objectLink));
                    }
                }
            }
            else
            {
                if (((IContent) value).Node != null)
                {
                    if (((IContent) value).Node.Parent == null
                        || !((IContent) value).Node.Parent.Equals(Node))
                    {
                        Node.Add(((IContent) value).Node);
                    }
                }
            }
        }

        /// <summary>
        /// Content_s the removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Removed(int index, object value)
        {
            ((IContent) value).Node.Remove();
            //if graphic remove it
            if (value is Graphic)
                if (Document.Graphics.Contains(value as Graphic))
                    Document.Graphics.Remove(value as Graphic);
        }

        public void CreatePubAttr(string name, string text, XNamespace prefix)
        {
            Node.SetAttributeValue(prefix + name, text);
        }

        #region IContent Member

        private XElement _node;
        private IStyle _style;

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public XElement Node
        {
            get { return _node; }
            set { _node = value; }
        }

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

        #region IHtml Member

        //
        //		/// <summary>
        //		/// Return the content as Html string
        //		/// </summary>
        //		/// <returns>The html string</returns>
        //		public string GetHtml()
        //		{
        //			string html			= "";
        //			if (this.Content.Count > 0)
        //				if (this.Content[0] is Graphic)
        //					html			= ((Graphic)this.Content[0]).GetHtml();
        //
        //			foreach(IContent content in this.Content)
        //				if (content is IHtml)
        //					html		+= ((IHtml)content).GetHtml();
        //
        //			return html;
        //		}

        #endregion

        #region IContentContainer Member

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content { get; set; }

        #endregion
    }
}

/*
 * $Log: Frame.cs,v $
 * Revision 1.4  2008/04/29 15:39:44  mt
 * new copyright header
 *
 * Revision 1.3  2008/02/08 07:12:19  larsbehr
 * - added initial chart support
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:32  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.6  2007/02/13 17:58:47  larsbm
 * - add first part of implementation of master style pages
 * - pdf exporter conversations for tables and images and added measurement helper
 *
 * Revision 1.5  2006/05/02 17:37:16  larsbm
 * - Flag added graphics with guid
 * - Set guid based read and write directories
 *
 * Revision 1.4  2006/02/16 18:35:41  larsbm
 * - Add FrameBuilder class
 * - TextSequence implementation (Todo loading!)
 * - Free draing postioning via x and y coordinates
 * - Graphic will give access to it's full qualified path
 *   via the GraphicRealPath property
 * - Fixed Bug with CellSpan in Spreadsheetdocuments
 * - Fixed bug graphic of loaded files won't be deleted if they
 *   are removed from the content.
 * - Break-Before property for Paragraph properties for Page Break
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
 * Revision 1.1  2006/01/29 11:29:46  larsbm
 * *** empty log message ***
 *
 * Revision 1.5  2005/12/18 18:29:46  larsbm
 * - AODC Gui redesign
 * - AODC HTML exporter refecatored
 * - Full Meta Data Support
 * - Increase textprocessing performance
 *
 * Revision 1.4  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.3  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 * Revision 1.2  2005/10/22 10:47:41  larsbm
 * - add graphic support
 *
 * Revision 1.1  2005/10/17 19:32:47  larsbm
 * - start vers. 1.0.3.0
 * - add frame, framestyle, graphic, graphicproperties
 *
 */