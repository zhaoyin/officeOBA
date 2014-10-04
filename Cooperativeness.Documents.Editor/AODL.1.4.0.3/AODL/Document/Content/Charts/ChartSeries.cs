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
    /// Summary description for ChartSeries.
    /// </summary>
    public class ChartSeries : IContent
    {
        /// <summary>
        /// the constructor of the chartseries
        /// </summary>
        /// <param name="document"></param>
        /// <param name="node"></param>
        public ChartSeries(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            DataPointCollection = new DataPointCollection();
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the ChartPlotArea class.
        /// This will create an empty cell that use the default cell style
        /// </summary>
        /// <param name="chart">The chart.</param>
        public ChartSeries(Chart chart)
        {
            Chart = chart;
            Document = chart.Document;
            DataPointCollection = new DataPointCollection();
            Node = new XElement(Ns.Chart + "series");
            SeriesStyle = new SeriesStyle(Document);
            Chart.Styles.Add(SeriesStyle);

            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the ChartPlotArea class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="styleName">The style name.</param>
        public ChartSeries(Chart chart, string styleName)
        {
            Chart = chart;
            Document = chart.Document;
            Node = new XElement(Ns.Chart + "series");
            Node.SetAttributeValue(Ns.Chart + "style-name", styleName);
            InitStandards();
            DataPointCollection = new DataPointCollection();

            if (styleName != null)
            {
                StyleName = styleName;
                SeriesStyle = new SeriesStyle(Document, styleName);
                Chart.Styles.Add(SeriesStyle);
            }
        }

        public Chart Chart { get; set; }

        public ChartPlotArea PlotArea { get; set; }

        /// <summary>
        /// the style of the series
        /// </summary>
        public SeriesStyle SeriesStyle
        {
            get { return (SeriesStyle) Style; }
            set
            {
                StyleName = value.StyleName;
                Style = value;
            }
        }

        public DataPointCollection DataPointCollection { get; set; }

        /// <summary>
        /// gets and sets the value cell range address
        /// </summary>
        public string ValuesCellRangeAddress
        {
            get { return (string) Node.Attribute(Ns.Chart + "values-cell-range-address"); }
            set { Node.SetAttributeValue(Ns.Chart + "values-cell-range-address", value); }
        }

        /// <summary>
        /// gets and sets the label cell address
        /// </summary>
        public string LabelCellAddress
        {
            get { return (string) Node.Attribute(Ns.Chart + "label-cell-address"); }
            set { Node.SetAttributeValue(Ns.Chart + "label-cell-address", value); }
        }

        /// <summary>
        /// gets and sets the class 
        /// </summary>
        public string Class
        {
            get { return (string) Node.Attribute(Ns.Chart + "class"); }
            set { Node.SetAttributeValue(Ns.Chart + "class", value); }
        }

        /// <summary>
        /// gets and sets the attached axis
        /// </summary>
        public string AttachAxis
        {
            get { return (string) Node.Attribute(Ns.Chart + "attached-axis"); }
            set { Node.SetAttributeValue(Ns.Chart + "attached-axis", value); }
        }

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

        /// <summary>
        /// builds the content
        /// </summary>
        /// <returns></returns>
        public XElement BuildNode()
        {
            foreach (ChartDataPoint dataPoint in DataPointCollection)
            {
                //XElement node = this.Chart .ChartDoc .ImportNode (DataPoint.Node ,true);
                Node.Add(dataPoint.Node);
            }
            return Node;
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

        #region IContentContainer Member

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content { get; set; }

        #endregion
    }
}