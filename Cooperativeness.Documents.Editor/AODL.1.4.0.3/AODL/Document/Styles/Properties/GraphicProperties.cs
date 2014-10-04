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

namespace AODL.Document.Styles.Properties
{
    /// <summary>
    /// GraphicProperties represent the hraphic properties.
    /// </summary>
    public class GraphicProperties : IProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicProperties"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        public GraphicProperties(IStyle style)
        {
            Style = style;
            Node = new XElement(Ns.Style + "graphic-properties");
            InitStandardImplemenation();
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
        /// Gets or sets the margin to the left.
        /// (distance bewteen image and surrounding text)
        /// </summary>
        /// <value>The distance e.g 0.3cm.</value>
        public string MarginLeft
        {
            get { return (string) Node.Attribute(Ns.Fo + "margin-left"); }
            set { Node.SetAttributeValue(Ns.Fo + "margin-left", value); }
        }

        /// <summary>
        /// Gets or sets the margin to the right.
        /// (distance bewteen image and surrounding text)
        /// </summary>
        /// <value>The distance e.g 0.3cm.</value>
        public string MarginRight
        {
            get { return (string) Node.Attribute(Ns.Fo + "margin-right"); }
            set { Node.SetAttributeValue(Ns.Fo + "margin-right", value); }
        }

        /// <summary>
        /// Gets or sets the margin to the top.
        /// (distance bewteen image and surrounding text)
        /// </summary>
        /// <value>The distance e.g 0.3cm.</value>
        public string MarginTop
        {
            get { return (string) Node.Attribute(Ns.Fo + "margin-top"); }
            set { Node.SetAttributeValue(Ns.Fo + "margin-top", value); }
        }

        /// <summary>
        /// Gets or sets the margin to the bottom.
        /// (distance bewteen image and surrounding text)
        /// </summary>
        /// <value>The distance e.g 0.3cm.</value>
        public string MarginBottom
        {
            get { return (string) Node.Attribute(Ns.Fo + "margin-bottom"); }
            set { Node.SetAttributeValue(Ns.Fo + "margin-bottom", value); }
        }

        /// <summary>
        /// Gets or sets the horizontal position. e.g center, from-left, right
        /// </summary>
        /// <value>The horizontal position.</value>
        public string HorizontalPosition
        {
            get { return (string) Node.Attribute(Ns.Style + "horizontal-pos"); }
            set { Node.SetAttributeValue(Ns.Style + "horizontal-pos", value); }
        }

        /// <summary>
        /// Gets or sets the vertical position. e.g. from-top
        /// </summary>
        /// <value>The vertical position.</value>
        public string VerticalPosition
        {
            get { return (string) Node.Attribute(Ns.Style + "vertical-pos"); }
            set { Node.SetAttributeValue(Ns.Style + "vertical-pos", value); }
        }

        /// <summary>
        /// Gets or sets the vertical relative. e.g. paragraph
        /// </summary>
        /// <value>The vertical relative.</value>
        public string VerticalRelative
        {
            get { return (string) Node.Attribute(Ns.Style + "vertical-rel"); }
            set { Node.SetAttributeValue(Ns.Style + "vertical-rel", value); }
        }

        /// <summary>
        /// Gets or sets the horizontal relative. e.g. paragraph
        /// </summary>
        /// <value>The horizontal relative.</value>
        public string HorizontalRelative
        {
            get { return (string) Node.Attribute(Ns.Style + "horizontal-rel"); }
            set { Node.SetAttributeValue(Ns.Style + "horizontal-rel", value); }
        }

        /// <summary>
        /// Gets or sets the mirror. e.g. none
        /// </summary>
        /// <value>The mirror.</value>
        public string Mirror
        {
            get { return (string) Node.Attribute(Ns.Style + "mirror"); }
            set { Node.SetAttributeValue(Ns.Style + "mirror", value); }
        }

        /// <summary>
        /// Gets or sets the clip. e.g rect(0cm 0cm 0cm 0cm)
        /// </summary>
        /// <value>The clip value.</value>
        public string Clip
        {
            get { return (string) Node.Attribute(Ns.Fo + "clip"); }
            set { Node.SetAttributeValue(Ns.Fo + "clip", value); }
        }


        /// <summary>
        /// Gets or sets the luminance in procent. e.g 10%
        /// </summary>
        /// <value>The luminance in procent.</value>
        public string LuminanceInProcent
        {
            get { return (string) Node.Attribute(Ns.Draw + "luminance"); }
            set { Node.SetAttributeValue(Ns.Draw + "luminance", value); }
        }

        /// <summary>
        /// Gets or sets the contrast in procent. e.g 10%
        /// </summary>
        /// <value>The contrast in procent.</value>
        public string ContrastInProcent
        {
            get { return (string) Node.Attribute(Ns.Draw + "contrast"); }
            set { Node.SetAttributeValue(Ns.Draw + "contrast", value); }
        }

        /// <summary>
        /// Gets or sets the draw red in procent. e.g. 10%
        /// </summary>
        /// <value>The draw red in procent.</value>
        public string DrawRedInProcent
        {
            get { return (string) Node.Attribute(Ns.Draw + "red"); }
            set { Node.SetAttributeValue(Ns.Draw + "red", value); }
        }

        /// <summary>
        /// Gets or sets the draw green in procent. e.g 10%
        /// </summary>
        /// <value>The draw green in procent.</value>
        public string DrawGreenInProcent
        {
            get { return (string) Node.Attribute(Ns.Draw + "green"); }
            set { Node.SetAttributeValue(Ns.Draw + "green", value); }
        }

        /// <summary>
        /// Gets or sets the draw blue in procent. e.g 0%
        /// </summary>
        /// <value>The draw blue in procent.</value>
        public string DrawBlueInProcent
        {
            get { return (string) Node.Attribute(Ns.Draw + "blue"); }
            set { Node.SetAttributeValue(Ns.Draw + "blue", value); }
        }

        /// <summary>
        /// Gets or sets the draw gamma in procent. e.g. 100%
        /// </summary>
        /// <value>The draw gamma in procent.</value>
        public string DrawGammaInProcent
        {
            get { return (string) Node.Attribute(Ns.Draw + "gamma"); }
            set { Node.SetAttributeValue(Ns.Draw + "gamma", value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [color inversion].
        /// </summary>
        /// <value><c>true</c> if [color inversion]; otherwise, <c>false</c>.</value>
        public bool ColorInversion
        {
            get { return (bool?) Node.Attribute(Ns.Draw + "color-inversion") ?? false; }
            set { Node.SetAttributeValue(Ns.Draw + "color-inversion", value); }
        }

        /// <summary>
        /// Gets or sets the image opacity in procent. e.g. 100%
        /// </summary>
        /// <value>The image opacity in procent.</value>
        public string ImageOpacityInProcent
        {
            get { return (string) Node.Attribute(Ns.Draw + "image-opacity"); }
            set { Node.SetAttributeValue(Ns.Draw + "image-opacity", value); }
        }

        /// <summary>
        /// Gets or sets the color mode. e.g. standard
        /// </summary>
        /// <value>The color mode.</value>
        public string ColorMode
        {
            get { return (string) Node.Attribute(Ns.Draw + "color-mode"); }
            set { Node.SetAttributeValue(Ns.Draw + "color-mode", value); }
        }

        /// <summary>
        /// Inits the standard implemenation.
        /// </summary>
        private void InitStandardImplemenation()
        {
            Clip = "rect(0cm 0cm 0cm 0cm)";
            ColorInversion = false;
            ColorMode = "standard";
            ContrastInProcent = "0%";
            DrawBlueInProcent = "0%";
            DrawGammaInProcent = "100%";
            DrawGreenInProcent = "0%";
            DrawRedInProcent = "0%";
            HorizontalPosition = "center";
            HorizontalRelative = "paragraph";
            ImageOpacityInProcent = "100%";
            LuminanceInProcent = "0%";
            Mirror = "none";
        }

        #region IProperty Member

        /// <summary>
        /// The XElement which represent the property element.
        /// </summary>
        /// <value>The node</value>
        public XElement Node { get; set; }

        /// <summary>
        /// The style object to which this property object belongs
        /// </summary>
        /// <value></value>
        public IStyle Style { get; set; }

        #endregion
    }
}

/*
 * $Log: GraphicProperties.cs,v $
 * Revision 1.4  2008/04/29 15:39:56  mt
 * new copyright header
 *
 * Revision 1.3  2008/02/08 07:12:21  larsbehr
 * - added initial chart support
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:54  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.4  2007/02/13 17:58:49  larsbm
 * - add first part of implementation of master style pages
 * - pdf exporter conversations for tables and images and added measurement helper
 *
 * Revision 1.3  2006/02/16 18:35:41  larsbm
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
 * Revision 1.2  2006/02/05 20:03:32  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.3  2005/12/18 18:29:46  larsbm
 * - AODC Gui redesign
 * - AODC HTML exporter refecatored
 * - Full Meta Data Support
 * - Increase textprocessing performance
 *
 * Revision 1.2  2005/10/22 10:47:41  larsbm
 * - add graphic support
 *
 * Revision 1.1  2005/10/17 19:32:47  larsbm
 * - start vers. 1.0.3.0
 * - add frame, framestyle, graphic, graphicproperties
 *
 */