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
    /// Summary description for ChartAxis.
    /// </summary>
    public class ChartAxis : IContent, IContentContainer
    {
        /// <summary>
        /// the constructor of the chart axes
        /// </summary>
        /// <param name="document"></param>
        /// <param name="node"></param>
        public ChartAxis(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the chartaxis class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public ChartAxis(Chart chart)
        {
            Chart = chart;
            Document = chart.Document;
            Node = new XElement(Ns.Chart + "axis");
            AxesStyle = new AxesStyle(chart.Document);
            Chart.Styles.Add(AxesStyle);
            InitStandards();

            if (AxesStyle.AxesProperties.DisplayLabel == null)
                AxesStyle.AxesProperties.DisplayLabel = "true";
        }

        /// <summary>
        /// Initializes a new instance of the ChartAxis class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="styleName">The style name.</param>
        public ChartAxis(Chart chart, string styleName)
        {
            Chart = chart;
            Document = chart.Document;
            Node = new XElement(Ns.Chart + "axis");


            if (styleName != null)
            {
                StyleName = styleName;
                AxesStyle = new AxesStyle(Document, styleName);
                Chart.Styles.Add(AxesStyle);
            }

            InitStandards();
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

        public Chart Chart { get; set; }

        public AxesStyle AxesStyle
        {
            get { return (AxesStyle) Style; }
            set { Style = value; }
        }

        public ChartPlotArea PlotArea { get; set; }

        public bool IsModified { get; set; }

        /// <summary>
        /// Gets or sets the axis dimension
        /// </summary>
        public string Dimension
        {
            get { return (string) Node.Attribute(Ns.Chart + "dimension"); }
            set { Node.SetAttributeValue(Ns.Chart + "dimension", value); }
        }

        /// <summary>
        /// Gets or sets the axis name
        /// </summary>
        public string AxisName
        {
            get { return (string) Node.Attribute(Ns.Chart + "name"); }
            set { Node.SetAttributeValue(Ns.Chart + "name", value); }
        }

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

        public void Setgrid()
        {
            ChartGrid grid = new ChartGrid(Chart) {GridClass = "major"};
            Content.Add(grid);
        }
    }
}