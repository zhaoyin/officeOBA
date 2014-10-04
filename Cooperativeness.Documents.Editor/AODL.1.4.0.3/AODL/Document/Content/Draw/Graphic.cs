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
using AODL.IO;

namespace AODL.Document.Content.Draw
{
    /// <summary>
    /// Graphic represent a graphic resp. image.
    /// </summary>
    public class Graphic : IContent, IContentContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Graphic"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="graphiclink">The graphiclink.</param>
        public Graphic(IDocument document, Frame frame, string graphiclink)
        {
            Frame = frame;
            Document = document;
            GraphicFileName = graphiclink;
            Node = new XElement(Ns.Draw + "image");
            Node.SetAttributeValue(Ns.XLink + "href", "Pictures/" + graphiclink);
            Node.SetAttributeValue(Ns.XLink + "type", "standard");
            Node.SetAttributeValue(Ns.XLink + "show", "embed");
            Node.SetAttributeValue(Ns.XLink + "actuate", "onLoad");
            InitStandards();
            Document.Graphics.Add(this);
            Document.DocumentMetadata.ImageCount += 1;
        }

        /// <summary>
        /// Gets or sets the H ref.
        /// </summary>
        /// <value>The H ref.</value>
        public string HRef
        {
            get { return (string) Node.Attribute(Ns.XLink + "href"); }
            set { Node.SetAttributeValue(Ns.XLink + "href", value); }
        }

        /// <summary>
        /// Gets or sets the actuate.
        /// e.g. onLoad
        /// </summary>
        /// <value>The actuate.</value>
        public string Actuate
        {
            get { return (string) Node.Attribute(Ns.XLink + "actuate"); }
            set { Node.SetAttributeValue(Ns.XLink + "actuate", value); }
        }

        /// <summary>
        /// Gets or sets the type of the Xlink.
        /// e.g. simple, standard, ..
        /// </summary>
        /// <value>The type of the X link.</value>
        public string XLinkType
        {
            get { return (string) Node.Attribute(Ns.XLink + "type"); }
            set { Node.SetAttributeValue(Ns.XLink + "type", value); }
        }

        /// <summary>
        /// Gets or sets the show.
        /// e.g. embed
        /// </summary>
        /// <value>The show.</value>
        public string Show
        {
            get { return (string) Node.Attribute(Ns.XLink + "show"); }
            set { Node.SetAttributeValue(Ns.XLink + "show", value); }
        }

        /// <summary>
        /// Gets or sets the graphic real path.
        /// </summary>
        /// <value>The graphic real path.</value>
        public IFile GraphicFile { get; set; }

        /// <summary>
        /// Gets or sets the name of the graphic file.
        /// </summary>
        /// <value>The name of the graphic file.</value>
        public string GraphicFileName { get; set; }

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public Frame Frame { get; set; }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            Content = new ContentCollection();
            Content.Inserted += Content_Inserted;
            Content.Removed += Content_Removed;
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
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content { get; set; }

        #endregion

//		#region IHtml Member
//
//		/// <summary>
//		/// Return the content as Html string
//		/// </summary>
//		/// <returns>The html string</returns>
//		public string GetHtml()
//		{
//			string align	= "<span align=\"#align#\">\n";
//			string html		= "<img src=\""+this.GetHtmlImgFolder()+"\" hspace=\"14\" vspace=\"14\""; //>\n";
//
//			Size size		= this.GetSizeInPix();
//			html			+= " width=\""+size.Width+"\" height=\""+size.Height+"\">\n";
//
//			if (this.Frame.FrameStyle.GraphicProperties != null)
//				if (this.Frame.FrameStyle.GraphicProperties.HorizontalPosition != null)
//				{
//					align	= align.Replace("#align#", 
//						this.Frame.FrameStyle.GraphicProperties.HorizontalPosition);
//					html	= align + html + "</span>\n";
//				}
//
//			return html;
//		}
//
//		/// <summary>
//		/// Gets the HTML img folder.
//		/// </summary>
//		/// <returns></returns>
//		private string GetHtmlImgFolder()
//		{
//			try
//			{
//				if (this.Frame.RealGraphicName != null)
//				{
//					return "temphtmlimg/Pictures/"+this.Frame.RealGraphicName;
//				}
//			}
//			catch(Exception ex)
//			{
//			}
//			return "";
//		}
//
//		/// <summary>
//		/// Gets the size in pix. As it is set in the frame.
//		/// This is needed, because the size of the graphic
//		/// could be another.
//		/// </summary>
//		/// <returns>The size in pixel</returns>
//		private Size GetSizeInPix()
//		{
//			try
//			{
//				double pxtocm		= 37.7928;
//				double intocm		= 2.41;
//				double height		= 0.0;
//				double width		= 0.0;
//
//				if (this.Frame.GraphicHeight.IndexOf("cm") > 0)
//				{
//					height		= Convert.ToDouble(this.Frame.GraphicHeight.Replace("cm",""), 
//						System.Globalization.NumberFormatInfo.InvariantInfo);
//					width		= Convert.ToDouble(this.Frame.GraphicWidth.Replace("cm",""),
//						System.Globalization.NumberFormatInfo.InvariantInfo);
//
//					height				*= pxtocm;
//					width				*= pxtocm;
//				}
//				else if (this.Frame.GraphicHeight.IndexOf("in") > 0)
//				{
//					height		= Convert.ToDouble(this.Frame.GraphicHeight.Replace("in",""), 
//						System.Globalization.NumberFormatInfo.InvariantInfo);
//					width		= Convert.ToDouble(this.Frame.GraphicWidth.Replace("in",""),
//						System.Globalization.NumberFormatInfo.InvariantInfo);
//
//					height				*= intocm*pxtocm;
//					width				*= intocm*pxtocm;
//				}
//				else if (this.Frame.GraphicHeight.IndexOf("px") > 0)
//				{
//					height		= Convert.ToDouble(this.Frame.GraphicHeight.Replace("px",""), 
//						System.Globalization.NumberFormatInfo.InvariantInfo);
//					width		= Convert.ToDouble(this.Frame.GraphicWidth.Replace("px",""),
//						System.Globalization.NumberFormatInfo.InvariantInfo);
//				}
//
//
//				Size size			= new Size((int)width, (int)height);
//
//				return size;
//			}
//			catch(Exception ex)
//			{
//				throw;
//			}
//		}
//
//		#endregion
    }
}