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

using System.Linq;
using System.Xml.Linq;
using AODL.Document.Styles;
using AODL.Document.Styles.Properties;

namespace AODL.Document.Content.Charts
{
    /// <summary>
    /// Summary description for ChartStyleProcessor.
    /// </summary>
    public class ChartStyleProcessor
    {
        /// <summary>
        /// the constructor of the chartStyleProcesser
        /// </summary>
        /// <param name="chart"></param>
        public ChartStyleProcessor(Chart chart)
        {
            Chart = chart;
        }

        public Chart Chart { get; set; }

        /// <summary>
        /// read the styles of the chart
        /// </summary>
        /// <param name="node"></param>
        /// <param name="styleType"></param>
        /// <returns></returns>
        public IStyle ReadStyle(XElement node, string styleType)
        {
            switch (styleType)
            {
                case "chart":
                    return CreateChartStyle(new XElement(node));
                case "title":
                    return CreateChartTitleStyle(new XElement(node));
                case "legend":
                    return CreateChartLegendStyle(new XElement(node));
                case "plotarea":
                    return CreateChartPlotAreaStyle(new XElement(node));
                case "axes":
                    return CreateChartAxesStyle(new XElement(node));
                case "series":
                    return CreateChartSeriesStyle(new XElement(node));
                case "wall":
                    return CreateChartWallStyle(new XElement(node));
                case "floor":
                    return CreateChartFloorStyle(new XElement(node));
                case "dr3d":
                    //return CreateChartDr3dLightStyle(new XElement(node));

                default:
                    return null;
            }
        }

        /// <summary>
        /// read the style node 
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public XElement ReadStyleNode(string styleName)
        {
            XElement automaticStyleNode = Chart.ChartDoc.Elements(Ns.Office + "document-content")
                .Elements(Ns.Office + "automatic-styles").FirstOrDefault();

            if (automaticStyleNode != null)
            {
                foreach (XElement nodeChild in automaticStyleNode.Elements())
                {
                    string family = (string) nodeChild.Attribute(Ns.Style + "family");

                    if (family != null && family == "chart")
                    {
                        string childStyleName = (string) nodeChild.Attribute(Ns.Style + "name");

                        if (childStyleName == styleName)

                            return nodeChild;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// create chart title style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <returns></returns>
        public TitleStyle CreateChartTitleStyle(XElement nodeStyle)
        {
            TitleStyle titleStyle = new TitleStyle(Chart.Document) {Node = nodeStyle};

            IPropertyCollection pCollection = new IPropertyCollection();

            if (nodeStyle.HasElements)
            {
                foreach (XElement nodeChild in nodeStyle.Elements())
                {
                    IProperty property = GetProperty(titleStyle, nodeChild);
                    if (property != null)

                        pCollection.Add(property);
                }
            }

            titleStyle.Node.Value = "";

            foreach (IProperty property in pCollection)

                titleStyle.PropertyCollection.Add(property);

            return titleStyle;
        }

        /// <summary>
        /// create chart style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <returns></returns>
        public ChartStyle CreateChartStyle(XElement nodeStyle)
        {
            ChartStyle chartStyle = new ChartStyle(Chart.Document) {Node = nodeStyle};

            IPropertyCollection pCollection = new IPropertyCollection();

            if (nodeStyle.HasElements)
            {
                foreach (XElement nodeChild in nodeStyle.Elements())
                {
                    IProperty property = GetProperty(chartStyle, nodeChild);
                    if (property != null)

                        pCollection.Add(property);
                }
            }

            chartStyle.Node.Value = "";

            foreach (IProperty property in pCollection)

                chartStyle.PropertyCollection.Add(property);

            //this.Chart .Styles .Add (chartStyle);

            return chartStyle;
        }

        /// <summary>
        /// create chart legend style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <returns></returns>
        public LegendStyle CreateChartLegendStyle(XElement nodeStyle)
        {
            LegendStyle legendStyle = new LegendStyle(Chart.Document) {Node = nodeStyle};

            IPropertyCollection pCollection = new IPropertyCollection();

            if (nodeStyle.HasElements)
            {
                foreach (XElement nodeChild in nodeStyle.Elements())
                {
                    IProperty property = GetProperty(legendStyle, nodeChild);
                    if (property != null)

                        pCollection.Add(property);
                }
            }

            legendStyle.Node.Value = "";

            foreach (IProperty property in pCollection)

                legendStyle.PropertyCollection.Add(property);

            //this.Chart .Styles .Add (legendStyle);

            return legendStyle;
        }

        /// <summary>
        /// create chart plotarea style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <returns></returns>
        public PlotAreaStyle CreateChartPlotAreaStyle(XElement nodeStyle)
        {
            PlotAreaStyle plotareaStyle = new PlotAreaStyle(Chart.Document) {Node = nodeStyle};

            IPropertyCollection pCollection = new IPropertyCollection();

            if (nodeStyle.HasElements)
            {
                foreach (XElement nodeChild in nodeStyle.Elements())
                {
                    IProperty property = GetProperty(plotareaStyle, nodeChild);
                    if (property != null)

                        pCollection.Add(property);
                }
            }

            plotareaStyle.Node.Value = "";

            foreach (IProperty property in pCollection)

                plotareaStyle.PropertyCollection.Add(property);

            //this.Chart .Styles .Add (plotareaStyle);

            return plotareaStyle;
        }

        /// <summary>
        /// create chart axes style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <returns></returns>
        private AxesStyle CreateChartAxesStyle(XElement nodeStyle)
        {
            AxesStyle axesStyle = new AxesStyle(Chart.Document) {Node = nodeStyle};
            IPropertyCollection pCollection = new IPropertyCollection();

            if (nodeStyle.HasElements)
            {
                foreach (XElement nodeChild in nodeStyle.Elements())
                {
                    IProperty property = GetProperty(axesStyle, nodeChild);
                    if (property != null)

                        pCollection.Add(property);
                }
            }

            axesStyle.Node.Value = "";

            foreach (IProperty property in pCollection)

                axesStyle.PropertyCollection.Add(property);

            //this.Chart .Styles .Add (axesStyle);

            return axesStyle;
        }

        /// <summary>
        /// create chart series style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <returns></returns>
        private SeriesStyle CreateChartSeriesStyle(XElement nodeStyle)
        {
            SeriesStyle seriesStyle = new SeriesStyle(Chart.Document, nodeStyle) {Node = nodeStyle};

            IPropertyCollection pCollection = new IPropertyCollection();

            if (nodeStyle.HasElements)
            {
                foreach (XElement nodeChild in nodeStyle.Elements())
                {
                    IProperty property = GetProperty(seriesStyle, nodeChild);
                    if (property != null)

                        pCollection.Add(property);
                }
            }

            seriesStyle.Node.Value = "";

            foreach (IProperty property in pCollection)

                seriesStyle.PropertyCollection.Add(property);

            //this.Chart .Styles .Add (seriesStyle);

            return seriesStyle;
        }

        /// <summary>
        /// create chart wall style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <returns></returns>
        private WallStyle CreateChartWallStyle(XElement nodeStyle)
        {
            WallStyle wallStyle = new WallStyle(Chart.Document) {Node = nodeStyle};

            IPropertyCollection pCollection = new IPropertyCollection();

            if (nodeStyle.HasElements)
            {
                foreach (XElement nodeChild in nodeStyle.Elements())
                {
                    IProperty property = GetProperty(wallStyle, nodeChild);
                    if (property != null)

                        pCollection.Add(property);
                }
            }

            wallStyle.Node.Value = "";

            foreach (IProperty property in pCollection)

                wallStyle.PropertyCollection.Add(property);

            //this.Chart .Styles .Add (wallStyle);

            return wallStyle;
        }

        /// <summary>
        /// create chart floor style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <returns></returns>
        private FloorStyle CreateChartFloorStyle(XElement nodeStyle)
        {
            FloorStyle floorStyle = new FloorStyle(Chart.Document) {Node = nodeStyle};

            IPropertyCollection pCollection = new IPropertyCollection();

            if (nodeStyle.HasElements)
            {
                foreach (XElement nodeChild in nodeStyle.Elements())
                {
                    IProperty property = GetProperty(floorStyle, nodeChild);
                    if (property != null)

                        pCollection.Add(property);
                }
            }

            floorStyle.Node.Value = "";

            foreach (IProperty property in pCollection)

                floorStyle.PropertyCollection.Add(property);

            //this.Chart .Styles .Add (floorStyle);

            return floorStyle;
        }

        /// <summary>
        /// get the property of the style
        /// </summary>
        /// <param name="nodeStyle"></param>
        /// <param name="propertyNode"></param>
        /// <returns></returns>
        private IProperty GetProperty(IStyle nodeStyle, XElement propertyNode)
        {
            if (propertyNode != null && nodeStyle != null)
            {
                if (propertyNode.Name == Ns.Style + "graphic-properties")
                    return CreateGraphicProperties(nodeStyle, propertyNode);
                if (propertyNode.Name == Ns.Style + "text-properties")
                    return CreateTextProperties(nodeStyle, propertyNode);
                if (propertyNode.Name == Ns.Style + "chart-properties")
                    return CreateChartProperty(nodeStyle, propertyNode);
                return CreateUnknownProperties(nodeStyle, propertyNode);
            }
            return null;
        }

        /// <summary>
        /// create the graphic property
        /// </summary>
        /// <param name="style"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public ChartGraphicProperties CreateGraphicProperties(IStyle style, XElement node)
        {
            ChartGraphicProperties graphicProperty = new ChartGraphicProperties(style) {Node = node};

            return graphicProperty;
        }

        /// <summary>
        /// create the text property
        /// </summary>
        /// <param name="style"></param>
        /// <param name="propertyNode"></param>
        /// <returns></returns>
        private static TextProperties CreateTextProperties(IStyle style, XElement propertyNode)
        {
            TextProperties textProperties = new TextProperties(style) {Node = propertyNode};

            return textProperties;
        }

        /// <summary>
        /// create the chart property
        /// </summary>
        /// <param name="style"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static IProperty CreateChartProperty(IStyle style, XElement node)
        {
            if (style is PlotAreaProperties)
                return CreatePlotAreaProperties(style, node);

            if (style is AxesProperties)
                return CreateAxesProperties(style, node);
            return CreateChartProperties(style, node);
        }

        /// <summary>
        /// create the plotarea property
        /// </summary>
        /// <param name="style"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static PlotAreaProperties CreatePlotAreaProperties(IStyle style, XElement node)
        {
            PlotAreaProperties plotareaProperty = new PlotAreaProperties(style) {Node = node};
            return plotareaProperty;
        }

        /// <summary>
        /// create the axes property
        /// </summary>
        /// <param name="style"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static AxesProperties CreateAxesProperties(IStyle style, XElement node)
        {
            AxesProperties axisProperty = new AxesProperties(style) {Node = node};
            return axisProperty;
        }

        /// <summary>
        /// create the chart property
        /// </summary>
        /// <param name="style"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static ChartProperties CreateChartProperties(IStyle style, XElement node)
        {
            ChartProperties chartProperty = new ChartProperties(style) {Node = node};
            return chartProperty;
        }

        /// <summary>
        /// create unknown property
        /// </summary>
        /// <param name="style"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static UnknownProperty CreateUnknownProperties(IStyle style, XElement node)
        {
            return new UnknownProperty(style, node);
        }
    }
}