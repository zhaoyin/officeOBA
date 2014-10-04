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
    /// Summary description for ChartProperties.
    /// </summary>
    public class ChartProperties : IProperty
    {
        /// <summary>
        /// The Constructor, create new instance of ChartProperties
        /// </summary>
        /// <param name="style">The ColumnStyle</param>
        public ChartProperties(IStyle style)
        {
            Style = style;
            Node = new XElement(Ns.Style + "chart-properties");
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
        /// gets and sets the direction
        /// </summary>
        public string Direction
        {
            get { return (string) Node.Attribute(Ns.Style + "direction"); }
            set { Node.SetAttributeValue(Ns.Style + "direction", value); }
        }

        /// <summary>
        /// gets and sets the three dimensional
        /// </summary>
        public string ThreeDimensional
        {
            get { return (string) Node.Attribute(Ns.Chart + "three-dimensional"); }
            set { Node.SetAttributeValue(Ns.Chart + "three-dimensional", value); }
        }

        /// <summary>
        /// gets and sets candle stick
        /// </summary>
        public string CandleStick
        {
            get { return (string) Node.Attribute(Ns.Chart + "japanese-candle-stick"); }
            set { Node.SetAttributeValue(Ns.Chart + "japanese-candle-stick", value); }
        }

        /// <summary>
        /// gets and sets the data label number
        /// </summary>
        public string DataLabelNumber
        {
            get { return (string) Node.Attribute(Ns.Chart + "data-label-number"); }
            set { Node.SetAttributeValue(Ns.Chart + "data-label-number", value); }
        }

        /// <summary>
        /// gets and sets data label text
        /// </summary>
        public string DataLabelText
        {
            get { return (string) Node.Attribute(Ns.Chart + "data-label-text"); }
            set { Node.SetAttributeValue(Ns.Chart + "data-label-text", value); }
        }

        /// <summary>
        /// gets and sets data label symbol
        /// </summary>
        public string DataLabelSymbol
        {
            get { return (string) Node.Attribute(Ns.Chart + "data-label-symbol"); }
            set { Node.SetAttributeValue(Ns.Chart + "data-label-symbol", value); }
        }

        /// <summary>
        /// gets and sets mean value
        /// </summary>
        public string MeanValue
        {
            get { return (string) Node.Attribute(Ns.Chart + "mean-value"); }
            set { Node.SetAttributeValue(Ns.Chart + "mean-value", value); }
        }

        /// <summary>
        /// gets and sets error category
        /// </summary>
        public string ErrorCategory
        {
            get { return (string) Node.Attribute(Ns.Chart + "error-category"); }
            set { Node.SetAttributeValue(Ns.Chart + "error-category", value); }
        }


        /// <summary>
        /// gets and sets error percentage
        /// </summary>
        public string ErrorPercentage
        {
            get { return (string) Node.Attribute(Ns.Chart + "error-percentage"); }
            set { Node.SetAttributeValue(Ns.Chart + "error-percentage", value); }
        }

        /// <summary>
        /// gets and sets error margin
        /// </summary>
        public string ErrorMargin
        {
            get { return (string) Node.Attribute(Ns.Chart + "error-margin"); }
            set { Node.SetAttributeValue(Ns.Chart + "error-margin", value); }
        }

        /// <summary>
        /// gets and sets error lower limit
        /// </summary>
        public string ErrorLowerLimit
        {
            get { return (string) Node.Attribute(Ns.Chart + "error-lower-limit"); }
            set { Node.SetAttributeValue(Ns.Chart + "error-lower-limit", value); }
        }

        /// <summary>
        /// gets and sets error upper limit
        /// </summary>
        public string ErrorUpperLimit
        {
            get { return (string) Node.Attribute(Ns.Chart + "error-upper-limit"); }
            set { Node.SetAttributeValue(Ns.Chart + "error-upper-limit", value); }
        }

        /// <summary>
        /// gets and sets error upper indicator
        /// </summary>
        public string ErrorUpperIndicator
        {
            get { return (string) Node.Attribute(Ns.Chart + "error-upper-indicator"); }
            set { Node.SetAttributeValue(Ns.Chart + "error-upper-indicator", value); }
        }


        /// <summary>
        /// gets and sets error lower indicator
        /// </summary>
        public string ErrorLowerIndicator
        {
            get { return (string) Node.Attribute(Ns.Chart + "error-lower-indicator"); }
            set { Node.SetAttributeValue(Ns.Chart + "error-lower-indicator", value); }
        }

        /// <summary>
        /// gets and sets vertical
        /// </summary>
        public string Vertical
        {
            get { return (string) Node.Attribute(Ns.Chart + "vertical"); }
            set { Node.SetAttributeValue(Ns.Chart + "vertical", value); }
        }

        /// <summary>
        /// gets and sets connect bars
        /// </summary>
        public string ConnectBars
        {
            get { return (string) Node.Attribute(Ns.Chart + "connect-bars"); }
            set { Node.SetAttributeValue(Ns.Chart + "connect-bars", value); }
        }

        /// <summary>
        /// gets and sets gap width
        /// </summary>
        public string GapWidth
        {
            get { return (string) Node.Attribute(Ns.Chart + "gap-width"); }
            set { Node.SetAttributeValue(Ns.Chart + "gap-width", value); }
        }

        /// <summary>
        /// gets and sets over lap
        /// </summary>
        public string OverLap
        {
            get { return (string) Node.Attribute(Ns.Chart + "overlap"); }
            set { Node.SetAttributeValue(Ns.Chart + "overlap", value); }
        }


        /// <summary>
        /// gets and sets deep
        /// </summary>
        public string Deep
        {
            get { return (string) Node.Attribute(Ns.Chart + "deep"); }
            set { Node.SetAttributeValue(Ns.Chart + "deep", value); }
        }

        /// <summary>
        /// gets and sets symbol type
        /// </summary>
        public string SymbolType
        {
            get { return (string) Node.Attribute(Ns.Chart + "symbol-type"); }
            set { Node.SetAttributeValue(Ns.Chart + "symbol-type", value); }
        }


        /*	public string SymbolType
            {
                get 
                { 
                    return (string)Node.Attribute(Ns.Chart + "symbol-type");
                }
                set
                {
                    Node.SetAttributeValue(Ns.Chart + "symbol-type", value);
                }
            }*/

        /// <summary>
        /// gets and sets lines
        /// </summary>
        public string Lines
        {
            get { return (string) Node.Attribute(Ns.Chart + "lines"); }
            set { Node.SetAttributeValue(Ns.Chart + "lines", value); }
        }


        /// <summary>
        /// gets and sets solid type
        /// </summary>
        public string SolidType
        {
            get { return (string) Node.Attribute(Ns.Chart + "solid-type"); }
            set { Node.SetAttributeValue(Ns.Chart + "solid-type", value); }
        }

        /// <summary>
        /// gets and sets stacked
        /// </summary>
        public string Stacked
        {
            get { return (string) Node.Attribute(Ns.Chart + "stacked"); }
            set { Node.SetAttributeValue(Ns.Chart + "stacked", value); }
        }


        /// <summary>
        /// gets and sets percentage
        /// </summary>
        public string Percentage
        {
            get { return (string) Node.Attribute(Ns.Chart + "percentage"); }
            set { Node.SetAttributeValue(Ns.Chart + "percentage", value); }
        }

        /// <summary>
        /// gets and sets inter polation
        /// </summary>
        public string InterPolation
        {
            get { return (string) Node.Attribute(Ns.Chart + "interpolation"); }
            set { Node.SetAttributeValue(Ns.Chart + "interpolation", value); }
        }
    }
}