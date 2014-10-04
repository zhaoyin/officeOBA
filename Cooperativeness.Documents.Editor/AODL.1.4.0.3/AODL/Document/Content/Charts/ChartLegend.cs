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
    /// Summary description for ChartLegend.
    /// </summary>
    public class ChartLegend : IContent
    {
        /// <summary>
        /// the consturctor of the chart legend
        /// </summary>
        /// <param name="document"></param>
        /// <param name="node"></param>
        public ChartLegend(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        public ChartLegend(Chart chart)
        {
            Chart = chart;
            Document = chart.Document;
            Node = new XElement(Ns.Chart + "legend");
            LegendStyle = new LegendStyle(chart.Document);

            chart.Content.Add(this);
        }

        public ChartLegend(Chart chart, string styleName)
        {
            Chart = chart;
            Document = chart.Document;
            Node = new XElement(Ns.Chart + "legend");
            XAttribute xa = new XAttribute(Ns.Chart + "style-name", styleName);
            Node.Add(xa);
            LegendStyle = new LegendStyle(Document, styleName);
            Chart.Styles.Add(LegendStyle);

            chart.Content.Add(this);
        }

        #region IContent Member

        private IStyle _style;
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

        /// <summary>
        /// Gets or sets the horizontal position 
        /// where the legend should be anchored
        /// </summary>
        public string SvgX
        {
            get { return (string) Node.Attribute(Ns.Svg + "x"); }
            set { Node.SetAttributeValue(Ns.Svg + "x", value); }
        }

        /// <summary>
        /// Gets or sets the vertical position where
        /// the legend should be
        /// anchored. 
        /// </summary>
        public string SvgY
        {
            get { return (string) Node.Attribute(Ns.Svg + "y"); }
            set { Node.SetAttributeValue(Ns.Svg + "y", value); }
        }

        public Chart Chart { get; set; }

        /// <summary>
        /// gets and sets the style of the legend
        /// </summary>
        public LegendStyle LegendStyle
        {
            get { return (LegendStyle) Style; }
            set
            {
                StyleName = value.StyleName;
                Style = value;
            }
        }

        /// <summary>
        /// gets and sets the  position of the legend
        /// </summary>
        public string LegendPosition
        {
            get { return (string) Node.Attribute(Ns.Chart + "legend-position"); }
            set { Node.SetAttributeValue(Ns.Chart + "legend-position", value); }
        }

        /// <summary>
        /// gets and sets the align of the legend
        /// </summary>
        public string LegendAlign
        {
            get { return (string) Node.Attribute(Ns.Chart + "legend-align"); }
            set { Node.SetAttributeValue(Ns.Chart + "legend-align", value); }
        }
    }
}