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

namespace AODL.Document.Styles.Properties
{
    /// <summary>
    /// Summary description for AxesProperties.
    /// </summary>
    public class AxesProperties : ChartProperties
    {
        /// <summary>
        /// The Constructor, create new instance of ChartProperties
        /// </summary>
        /// <param name="style">The ColumnStyle</param>
        public AxesProperties(IStyle style) : base(style)
        {
            //this.Style			= style;
            //this.NewXmlNode();
        }

        /// <summary>
        /// gets and sets the link data format
        /// </summary>
        public string LinkDataFormat
        {
            get { return (string) Node.Attribute(Ns.Chart + "link-data-style-to-source"); }
            set { Node.SetAttributeValue(Ns.Chart + "link-data-style-to-source", value); }
        }

        /// <summary>
        /// gets and sets the visibility
        /// </summary>
        public string Visibility
        {
            get { return (string) Node.Attribute(Ns.Chart + "axis-visible"); }
            set { Node.SetAttributeValue(Ns.Chart + "axis-visible", value); }
        }

        /// <summary>
        /// gets and sets the logarithmic
        /// </summary>
        public string Logarithmic
        {
            get { return (string) Node.Attribute(Ns.Chart + "axis-logarithmic"); }
            set { Node.SetAttributeValue(Ns.Chart + "axis-logarithmic", value); }
        }

        /// <summary>
        /// gets and sets tick mark major inner
        /// </summary>
        public string TickMarkMajorInner
        {
            get { return (string) Node.Attribute(Ns.Chart + "tick-marks-major-inner"); }
            set { Node.SetAttributeValue(Ns.Chart + "tick-marks-major-inner", value); }
        }

        /// <summary>
        /// gets and sets tick mark major outer
        /// </summary>
        public string TickMarkMajorOuter
        {
            get { return (string) Node.Attribute(Ns.Chart + "tick-marks-minor-outer"); }
            set { Node.SetAttributeValue(Ns.Chart + "tick-marks-minor-outer", value); }
        }

        /// <summary>
        /// gets and set the maximum
        /// </summary>
        public string Maximum
        {
            get { return (string) Node.Attribute(Ns.Chart + "maximum"); }
            set { Node.SetAttributeValue(Ns.Chart + "maximum", value); }
        }

        /// <summary>
        /// gets and sets minimum
        /// </summary>
        public string Minimum
        {
            get { return (string) Node.Attribute(Ns.Chart + "minimum"); }
            set { Node.SetAttributeValue(Ns.Chart + "minimum", value); }
        }

        /// <summary>
        /// gets and set origin
        /// </summary>
        public string Origin
        {
            get { return (string) Node.Attribute(Ns.Chart + "origin"); }
            set { Node.SetAttributeValue(Ns.Chart + "origin", value); }
        }

        /// <summary>
        /// gets and sets the interval major
        /// </summary>
        public string IntervalMajor
        {
            get { return (string) Node.Attribute(Ns.Chart + "interval-major"); }
            set { Node.SetAttributeValue(Ns.Chart + "interval-major", value); }
        }

        /// <summary>
        /// gets and sets interval minor
        /// </summary>
        public string IntervalMinor
        {
            get { return (string) Node.Attribute(Ns.Chart + "interval-minor"); }
            set { Node.SetAttributeValue(Ns.Chart + "interval-minor", value); }
        }

        /// <summary>
        /// gets and sets tick mark minor inner
        /// </summary>
        public string TickMarkMinorInner
        {
            get { return (string) Node.Attribute(Ns.Chart + "tick-marks-minor-inner"); }
            set { Node.SetAttributeValue(Ns.Chart + "tick-marks-minor-inner", value); }
        }

        /// <summary>
        /// gets and sets tick mark minor outer
        /// </summary>
        public string TickMarkMinorOuter
        {
            get { return (string) Node.Attribute(Ns.Chart + "tick-marks-minor-outer"); }
            set { Node.SetAttributeValue(Ns.Chart + "tick-marks-minor-outer", value); }
        }

        /// <summary>
        /// gets and sets the display label
        /// </summary>
        public string DisplayLabel
        {
            get { return (string) Node.Attribute(Ns.Chart + "display-label"); }
            set { Node.SetAttributeValue(Ns.Chart + "display-label", value); }
        }

        /// <summary>
        /// gets and sets text overlap
        /// </summary>
        public string TextOverLap
        {
            get { return (string) Node.Attribute(Ns.Chart + "text-overlap"); }
            set { Node.SetAttributeValue(Ns.Chart + "text-overlap", value); }
        }

        /// <summary>
        /// gets and sets line break
        /// </summary>
        public string LineBreak
        {
            get { return (string) Node.Attribute(Ns.Chart + "line-break"); }
            set { Node.SetAttributeValue(Ns.Chart + "line-break", value); }
        }

        /// <summary>
        /// gets and sets the label arragement
        /// </summary>
        public string LabelArrangement
        {
            get { return (string) Node.Attribute(Ns.Chart + "label-arrangement"); }
            set { Node.SetAttributeValue(Ns.Chart + "label-arrangement", value); }
        }
    }
}