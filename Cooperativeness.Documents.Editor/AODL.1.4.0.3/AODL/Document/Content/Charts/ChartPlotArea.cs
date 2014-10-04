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
    /// Summary description for ChartPlotArea.
    /// </summary>
    public class ChartPlotArea : IContent, IContentContainer
    {
        /// <summary>
        /// Initializes a new instance of the ChartPlotArea class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public ChartPlotArea(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        public ChartPlotArea(Chart chart) : this(chart, null)
        {
        }


        /// <summary>
        /// Initializes a new instance of the ChartPlotArea class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="styleName">The style name.</param>
        public ChartPlotArea(Chart chart, string styleName)
        {
            Chart = chart;
            Document = chart.Document;
            Node = new XElement(Ns.Chart + "plot-area");

            InitStandards();

            StyleName = styleName;
            PlotAreaStyle = new PlotAreaStyle(chart.Document, styleName);
            chart.Styles.Add(PlotAreaStyle);
            InitPlotArea();

            chart.Content.Add(this);
        }

        public Chart Chart { get; set; }

        /// <summary>
        /// Gets or sets the plotarea style.
        /// </summary>
        /// <value>The plotarea style.</value>
        public PlotAreaStyle PlotAreaStyle
        {
            get { return (PlotAreaStyle) Style; }
            set
            {
                StyleName = value.StyleName;
                Style = value;
            }
        }

        public AxisCollection AxisCollection { get; set; }

        public Dr3dLightCollection Dr3dLightCollection { get; set; }

        public SeriesCollection SeriesCollection { get; set; }

        #region plotarea properties

        /// <summary>
        /// Gets or sets the horizontal position where
        /// the plotarea should be
        /// anchored. 
        /// </summary>
        public string SvgX
        {
            get { return (string) Node.Attribute(Ns.Svg + "x"); }
            set { Node.SetAttributeValue(Ns.Svg + "x", value); }
        }

        /// <summary>
        /// Gets or sets the vertical position where
        /// the plotarea should be
        /// anchored. 
        /// </summary>
        public string SvgY
        {
            get { return (string) Node.Attribute(Ns.Svg + "y"); }
            set { Node.SetAttributeValue(Ns.Svg + "y", value); }
        }

        /// <summary>
        /// the width of the plotarea
        /// </summary>
        public string Width
        {
            get { return (string) Node.Attribute(Ns.Svg + "width"); }
            set { Node.SetAttributeValue(Ns.Svg + "width", value); }
        }

        /// <summary>
        /// the height of the plotarea
        /// </summary>
        public string Height
        {
            get { return (string) Node.Attribute(Ns.Svg + "height"); }
            set { Node.SetAttributeValue(Ns.Svg + "height", value); }
        }

        /// <summary>
        /// sets and gets the data source has labels
        /// </summary>
        public string DataSourceHasLabels
        {
            get { return (string) Node.Attribute(Ns.Chart + "data-source-has-labels"); }
            set { Node.SetAttributeValue(Ns.Chart + "data-source-has-labels", value); }
        }

        /// <summary>
        /// gets and sets the table number list
        /// </summary>
        public string TableNumberList
        {
            get { return (string) Node.Attribute(Ns.Chart + "table-number-list"); }
            set { Node.SetAttributeValue(Ns.Chart + "table-number-list", value); }
        }

        /// <summary>
        /// gets and sets the dr3dvrp
        /// </summary>
        public string Dr3dVrp
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "vrp"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "vrp", value); }
        }

        /// <summary>
        /// gets and sets the dr3dvpn
        /// </summary>
        public string Dr3dVpn
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "vpn"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "vpn", value); }
        }

        /// <summary>
        /// gets and sets the dr3dvup
        /// </summary>
        public string Dr3dVup
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "vup"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "vup", value); }
        }

        /// <summary>
        /// gets and sets the projection
        /// </summary>
        public string Projection
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "projection"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "projection", value); }
        }

        /// <summary>
        /// gets and sets the distance
        /// </summary>
        public string Distance
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "distance"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "distance", value); }
        }

        /// <summary>
        /// gets and sets the focallength
        /// </summary>
        public string FocalLength
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "focal-length"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "focal-length", value); }
        }

        /// <summary>
        /// gets and sets the shadowslant
        /// </summary>
        public string ShadowSlant
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "shadow-slant"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "shadow-slant", value); }
        }

        /// <summary>
        /// gets and sets the shademode
        /// </summary>
        public string ShadeMode
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "shade-mode"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "shade-mode", value); }
        }

        /// <summary>
        /// gets and sets the ambientcolor
        /// </summary>
        public string AmbientColor
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "ambient-color"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "ambient-color", value); }
        }

        /// <summary>
        /// gets and sets the lightingmode
        /// </summary>
        public string LightingMode
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "lighting-mode"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "lighting-mode", value); }
        }

        /// <summary>
        /// gets and sets table cell range
        /// </summary>
        public string TableCellRange
        {
            get { return (string) Node.Attribute(Ns.Table + "cell-range-address"); }
            set { Node.SetAttributeValue(Ns.Table + "cell-range-address", value); }
        }

        #endregion

        private void InitStandards()
        {
            Content = new ContentCollection();
            AxisCollection = new AxisCollection();
            Dr3dLightCollection = new Dr3dLightCollection();
            SeriesCollection = new SeriesCollection();

            //	AxisCollection.Inserted      += new ZipStream.CollectionWithEvents.CollectionChange (AxisCollection_Inserted);
            //    AxisCollection.Removed       += new ZipStream.CollectionWithEvents.CollectionChange (AxisCollection_Removed);
            //	Dr3dLightCollection.Inserted += new ZipStream.CollectionWithEvents.CollectionChange (Dr3dLightCollection_Inserted);
            //    Dr3dLightCollection.Removed  += new ZipStream.CollectionWithEvents.CollectionChange (Dr3dLightCollection_Removed);
            //	SeriesCollection.Inserted    += new ZipStream.CollectionWithEvents.CollectionChange (SeriesCollection_Inserted);
            //	SeriesCollection.Removed     += new ZipStream.CollectionWithEvents.CollectionChange (SeriesCollection_Removed);	
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
        /// Builds the node.
        /// </summary>
        /// <returns>XElement</returns>
        public XElement BuildNode()
        {
            foreach (ChartAxis axes in AxisCollection)
            {
                Node.Add(new XElement(axes.Node));
            }

            foreach (Dr3dLight light in Dr3dLightCollection)
            {
                Node.Add(light.Node);
            }

            foreach (ChartSeries series in SeriesCollection)
            {
                //node=this.Chart .ChartDoc.ImportNode (series.BuildNode (),true );
                Node.Add(series.BuildNode());
            }

            return Node;
        }

        /// <summary>
        /// if  fistLine has labels or not
        /// </summary>
        /// <returns></returns>
        public bool FirstLineHasLabels()
        {
            if (DataSourceHasLabels == "row" || DataSourceHasLabels == "both")
                return true;
            return false;
        }


        /// <summary>
        /// if  fistColumnl  has labels          
        /// </summary>
        /// <returns></returns>
        public bool FirstColumnHasLabels()
        {
            if (DataSourceHasLabels == "column" || DataSourceHasLabels == "both")
                return true;
            return false;
        }

        /// <summary>
        /// Init the plotarea
        /// </summary>
        public void InitPlotArea()
        {
            string seriesSource = PlotAreaStyle.PlotAreaProperties.SeriesSource;

            if (seriesSource == null)
                PlotAreaStyle.PlotAreaProperties.SeriesSource = "columns";
            //SeriesSource= this.PlotAreaStyle .PlotAreaProperties .SeriesSource;
        }

        #region IContentContainer Member

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content { get; set; }

        #endregion

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
    }
}