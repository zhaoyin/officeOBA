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
    /// Summary description for ChartGraphicProperties.
    /// </summary>
    public class ChartGraphicProperties : IProperty
    {
        /// <summary>
        /// the constructor of chart graphic property
        /// </summary>
        /// <param name="style"></param>
        public ChartGraphicProperties(IStyle style)
        {
            Style = style;
            Node = new XElement(Ns.Style + "graphic-properties");
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

        /// <summary>
        /// gets and sets the draw stroke
        /// </summary>
        public string DrawStroke
        {
            get { return (string) Node.Attribute(Ns.Draw + "stroke"); }
            set { Node.SetAttributeValue(Ns.Draw + "stroke", value); }
        }

        /// <summary>
        /// gets and sets stroke dash
        /// </summary>
        public string StrokeDash
        {
            get { return (string) Node.Attribute(Ns.Draw + "stroke-dash"); }
            set { Node.SetAttributeValue(Ns.Draw + "stroke-dash", value); }
        }

        /// <summary>
        /// gets and sets stroke dash name
        /// </summary>
        public string StrokeDashNames
        {
            get { return (string) Node.Attribute(Ns.Draw + "stroke-dash-names"); }
            set { Node.SetAttributeValue(Ns.Draw + "stroke-dash-names", value); }
        }

        /// <summary>
        /// gets and sets the stroke width
        /// </summary>
        public string StrokeWidth
        {
            get { return (string) Node.Attribute(Ns.Svg + "stroke-width"); }
            set { Node.SetAttributeValue(Ns.Svg + "stroke-width", value); }
        }

        /// <summary>
        /// gets and sets stroke color
        /// </summary>
        public string StrokeColor
        {
            get { return (string) Node.Attribute(Ns.Svg + "stroke-color"); }
            set { Node.SetAttributeValue(Ns.Svg + "stroke-color", value); }
        }

        /// <summary>
        /// gets and sets marker start
        /// </summary>
        public string MarkerStart
        {
            get { return (string) Node.Attribute(Ns.Draw + "marker-start"); }
            set { Node.SetAttributeValue(Ns.Draw + "marker-start", value); }
        }

        /// <summary>
        /// gets and sets marker end
        /// </summary>
        public string MarkerEnd
        {
            get { return (string) Node.Attribute(Ns.Draw + "marker-end"); }
            set { Node.SetAttributeValue(Ns.Draw + "marker-end", value); }
        }

        /// <summary>
        /// gets and sets marker start width
        /// </summary>
        public string MarkerStartWidth
        {
            get { return (string) Node.Attribute(Ns.Draw + "marker-start-width"); }
            set { Node.SetAttributeValue(Ns.Draw + "marker-start-width", value); }
        }

        /// <summary>
        /// gets and set marker end width
        /// </summary>
        public string MarkerEndWidth
        {
            get { return (string) Node.Attribute(Ns.Draw + "marker-end-width"); }
            set { Node.SetAttributeValue(Ns.Draw + "marker-end-width", value); }
        }

        /// <summary>
        /// gets and sets marker end center
        /// </summary>
        public string MarkerEndCenter
        {
            get { return (string) Node.Attribute(Ns.Draw + "marker-end-center"); }
            set { Node.SetAttributeValue(Ns.Draw + "marker-end-center", value); }
        }

        /// <summary>
        /// gets and sets marker start center
        /// </summary>
        public string MarkerStartCenter
        {
            get { return (string) Node.Attribute(Ns.Draw + "marker-start-center"); }
            set { Node.SetAttributeValue(Ns.Draw + "marker-start-center", value); }
        }

        /// <summary>
        /// gets and sets stroke opacity
        /// </summary>
        public string StrokeOpacity
        {
            get { return (string) Node.Attribute(Ns.Svg + "stroke-opacity"); }
            set { Node.SetAttributeValue(Ns.Svg + "stroke-opacity", value); }
        }


        /// <summary>
        /// gets and sets stroke line join
        /// </summary>
        public string StrokeLineJoin
        {
            get { return (string) Node.Attribute(Ns.Draw + "stroke-linejoin"); }
            set { Node.SetAttributeValue(Ns.Draw + "stroke-linejoin", value); }
        }

        /// <summary>
        /// gets and sets fill color
        /// </summary>
        public string FillColor
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-color"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-color", value); }
        }

        /// <summary>
        /// gets and sets secondary fill color
        /// </summary>
        public string SecondaryFillColor
        {
            get { return (string) Node.Attribute(Ns.Draw + "secondary-fill-color"); }
            set { Node.SetAttributeValue(Ns.Draw + "secondary-fill-color", value); }
        }


        /// <summary>
        /// gets and sets fill gradient name
        /// </summary>
        public string FillGradientName
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-gradient-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-gradient-name", value); }
        }

        /// <summary>
        /// gets and sets gradient step count
        /// </summary>
        public string GradientStepCount
        {
            get { return (string) Node.Attribute(Ns.Draw + "gradient-step-count"); }
            set { Node.SetAttributeValue(Ns.Draw + "gradient-step-count", value); }
        }

        /// <summary>
        /// gets and sets fill hatch name
        /// </summary>
        public string FillHatchName
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-hatch-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-hatch-name", value); }
        }

        /// <summary>
        /// gets and sets fill hatch solid
        /// </summary>
        public string FillHatchSolid
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-hatch-solid"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-hatch-solid", value); }
        }

        /// <summary>
        /// gets and sets fill image name
        /// </summary>
        public string FillImageName
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-image-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-image-name", value); }
        }

        /// <summary>
        /// gets and sets style repeat
        /// </summary>
        public string StyleRepeat
        {
            get { return (string) Node.Attribute(Ns.Style + "repeat"); }
            set { Node.SetAttributeValue(Ns.Style + "repeat", value); }
        }

        /// <summary>
        /// gets and sets fill image width
        /// </summary>
        public string FillImageWidth
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-image-width"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-image-width", value); }
        }

        /// <summary>
        /// gets and sets fill image height
        /// </summary>
        public string FillImageHeight
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-image-height"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-image-height", value); }
        }

        /// <summary>
        /// gets and sets fill image ref point
        /// </summary>
        public string FillImageRefPoint
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-image-ref-point"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-image-ref-point", value); }
        }

        /// <summary>
        /// gets and sets fill image ref pointx
        /// </summary>
        public string FillImageRefPointX
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-image-ref-point-x"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-image-ref-point-x", value); }
        }

        /// <summary>
        /// gets and sets fill image ref pointy
        /// </summary>
        public string FillImageRefPointY
        {
            get { return (string) Node.Attribute(Ns.Draw + "fill-image-ref-point-y"); }
            set { Node.SetAttributeValue(Ns.Draw + "fill-image-ref-point-y", value); }
        }

        /// <summary>
        /// gets and sets tile repeat offset
        /// </summary>
        public string TileRepeatOffset
        {
            get { return (string) Node.Attribute(Ns.Draw + "tile-repeat-offset"); }
            set { Node.SetAttributeValue(Ns.Draw + "tile-repeat-offset", value); }
        }


        /// <summary>
        /// gets and sets opacity
        /// </summary>
        public string Opacity
        {
            get { return (string) Node.Attribute(Ns.Draw + "opacity"); }
            set { Node.SetAttributeValue(Ns.Draw + "opacity", value); }
        }

        /// <summary>
        /// gets and sets opacity name
        /// </summary>
        public string OpacityName
        {
            get { return (string) Node.Attribute(Ns.Draw + "opacity-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "opacity-name", value); }
        }

        /// <summary>
        /// gets and sets fill rule
        /// </summary>
        public string FillRule
        {
            get { return (string) Node.Attribute(Ns.Svg + "fill-rule"); }
            set { Node.SetAttributeValue(Ns.Svg + "fill-rule", value); }
        }

        /// <summary>
        /// gets and sets symbol color
        /// </summary>
        public string SymbolColor
        {
            get { return (string) Node.Attribute(Ns.Draw + "symbol-color"); }
            set { Node.SetAttributeValue(Ns.Draw + "symbol-color", value); }
        }

        /// <summary>
        /// gets and sets horizontal segment
        /// </summary>
        public string HorizontalSegment
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "horizontal-segments"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "horizontal-segments", value); }
        }

        /// <summary>
        /// gets and sets vertical segment
        /// </summary>
        public string VerticalSegment
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "vertical-segments"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "vertical-segments", value); }
        }

        /// <summary>
        /// gets and sets edge rounding
        /// </summary>
        public string EdgeRounding
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "edge-rounding"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "edge-rounding", value); }
        }

        /// <summary>
        /// gets and sets edge rounding mode
        /// </summary>
        public string EdgeRoundingMode
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "edge-rounding-mode"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "edge-rounding-mode", value); }
        }

        /// <summary>
        /// gets and sets back scale
        /// </summary>
        public string BackScale
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "back-scale"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "back-scale", value); }
        }

        /// <summary>
        /// gets and sets depth
        /// </summary>
        public string Depth
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "depth"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "depth", value); }
        }

        /// <summary>
        /// gets and sets back face culling
        /// </summary>
        public string BackFaceCulling
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "backface-culling"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "backface-culling", value); }
        }

        /// <summary>
        /// gets and sets end angle
        /// </summary>
        public string EndAngle
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "end-angle"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "end-angle", value); }
        }

        /// <summary>
        /// gets and sets close front
        /// </summary>
        public string CloseFront
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "close-front"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "close-front", value); }
        }

        /// <summary>
        /// gets and sets close back
        /// </summary>
        public string CloseBack
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "close-back"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "close-back", value); }
        }
    }
}