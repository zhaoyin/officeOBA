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

namespace AODL.Document.Content.Charts
{
    /// <summary>
    /// Summary description for ChartDataPoint.
    /// </summary>
    public class ChartDataPoint : IContent
    {
        public Chart Chart { get; set; }

        /// <summary>
        /// gets and sets the style of the datapoint
        /// </summary>
        public DataPointStyle DataPointStyle
        {
            get { return (DataPointStyle) Style; }
            set
            {
                StyleName = value.StyleName;
                Style = value;
            }
        }

        /// <summary>
        /// gets and sets the cell repeat count
        /// </summary>
        public string Repeated
        {
            get { return (string) Node.Attribute(Ns.Chart + "repeated"); }
            set { Node.SetAttributeValue(Ns.Chart + "repeated", value); }
        }

        #region IContent Member

        private IStyle _style;

        public ChartDataPoint(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            //this.InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the ChartPlotArea class.
        /// This will create an empty cell that use the default cell style
        /// </summary>
        /// <param name="chart">The chart.</param>
        public ChartDataPoint(Chart chart) : this(chart, null)
        {
        }

        public ChartDataPoint(Chart chart, string styleName)
        {
            Chart = chart;
            Document = chart.Document;
            Node = new XElement(Ns.Chart + "data-point");

            //this.InitStandards();

            StyleName = styleName;
            DataPointStyle = new DataPointStyle(Document, styleName);
            Chart.Styles.Add(DataPointStyle);
        }

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
            get { return (string) Node.Attribute(Ns.Chart + "style-name"); }
            set { Node.SetAttributeValue(Ns.Chart + "style-name", value); }
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
    }
}