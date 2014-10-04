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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AODL.Document.Collections;
using AODL.Document.Content.Text;
using AODL.Document.Exceptions;
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.Styles;

namespace AODL.Document.Content.Charts
{
    /// <summary>
    /// Summary description for ChartImporter.
    /// </summary>
    public class ChartImporter
    {
        #region Delegates

        public delegate void Warning(AODLWarning warning);

        #endregion

        /// <summary>
        /// the constructor of the ChartImporter
        /// </summary>
        /// <param name="chart"></param>
        public ChartImporter(Chart chart)
        {
            Chart = chart;
        }

        public Chart Chart { get; set; }

        public event Warning OnWarning;

        /// <summary>
        /// import the content and style of the chart
        /// </summary>
        public void Import()
        {
            ReadContent();
            ImportChartStyles();
        }

        /// <summary>
        /// import the style of the chart
        /// </summary>
        public void ImportChartStyles()
        {
            using (TextReader reader = new StreamReader(Chart.Document.Importer.Open(Path.Combine(Chart.ObjectRealPath, "styles.xml"))))
            {
                Chart.ChartStyles.Styles = XDocument.Load(reader);
            }
        }

        /// <summary>
        /// read the content of the chart
        /// </summary>
        public void ReadContent()
        {
            using (TextReader reader = new StreamReader(Chart.Document.Importer.Open(Path.Combine(Chart.ObjectRealPath, "content.xml"))))
            {
                Chart.ChartDoc = XDocument.Load(reader);
            }

            ReadContentNodes();
        }

        public void ReadContentNodes()
        {
            try
            {
                //				this._document.XmlDoc	= new XDocument();
                //				this._document.XmlDoc.Load(contentFile);

                XElement node = Chart.ChartDoc.Elements(Ns.Office + "document-content")
                    .Elements(Ns.Office + "body")
                    .Elements(Ns.Office + "chart").FirstOrDefault();

                if (node != null)
                {
                    CreateMainContent(node);
                }
                else
                {
                    throw new AODLException("Unknow content type.");
                }
                //Remove all existing content will be created new
                node.RemoveAll();
            }
            catch (Exception ex)
            {
                throw new AODLException("Error while trying to load the main content", ex);
            }
        }

        /// <summary>
        /// create the main content of the chart
        /// </summary>
        /// <param name="node"></param>
        public void CreateMainContent(XElement node)
        {
            try
            {
                foreach (XElement nodeChild in node.Elements())
                {
                    IContent iContent = CreateContent(new XElement(nodeChild));

                    if (iContent != null)
                        AddToCollection(iContent, Chart.Content);
                        //this._document.Content.Add(iContent);
                    else
                    {
                        if (OnWarning != null)
                        {
                            AODLWarning warning =
                                new AODLWarning("A couldn't create any content from an an first level node!.")
                                    {Node = nodeChild};
                            //warning.InMethod			= AODLException.GetExceptionSourceInfo(new StackFrame(1, true));
                            OnWarning(warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while processing a content node.", ex);
            }
        }

        /// <summary>
        /// create the content of the chart
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateContent(XElement node)
        {
            try
            {
                if (node.Name == Ns.Chart + "chart")
                    return CreateChart(new XElement(node));
                if (node.Name == Ns.Chart + "title")
                    return CreateChartTitle(new XElement(node));
                if (node.Name == Ns.Chart + "legend")
                    return CreateChartLegend(new XElement(node));
                if (node.Name == Ns.Chart + "plot-area")
                    return CreateChartPlotArea(new XElement(node));
                if (node.Name == Ns.Chart + "axis")
                    return CreateChartAxes(new XElement(node));
                if (node.Name == Ns.Chart + "categories")
                    return CreateChartCategories(new XElement(node));
                if (node.Name == Ns.Chart + "grid")
                    return CreateChartGrid(new XElement(node));
                if (node.Name == Ns.Chart + "series")
                    return CreateChartSeries(new XElement(node));
                if (node.Name == Ns.Chart + "data-point")
                    return CreateChartDataPoint(new XElement(node));
                if (node.Name == Ns.Chart + "wall")
                    return CreateChartWall(new XElement(node));
                if (node.Name == Ns.Chart + "floor")
                    return CreateChartFloor(new XElement(node));
                if (node.Name == Ns.Dr3d + "light")
                    return CreateDr3dLight(new XElement(node));
                if (node.Name == Ns.Text + "p")
                    return CreateParagraph(new XElement(node));
                if (node.Name == Ns.Table + "table")
                    return null;
                //return CreateDataTable();
                return null;
            }


            catch (Exception ex)
            {
                throw new AODLException("Exception while processing a content node.", ex);
            }
        }


        /// <summary>
        /// create the chart
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChart(XElement node)
        {
            try
            {
                Chart.Node = node;
                ContentCollection iColl = new ContentCollection();
                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(Chart.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "chart");

                if (style != null)
                {
                    Chart.ChartStyle = (ChartStyle) style;
                    Chart.Styles.Add(style);
                }

                foreach (XElement nodeChild in Chart.Node.Elements())
                {
                    IContent icontent = CreateContent(nodeChild);
                    if (icontent != null)

                        AddToCollection(icontent, iColl);
                }

                Chart.Node.Value = "";
                foreach (IContent icontent in iColl)
                {
                    AddToCollection(icontent, Chart.Content);
                }

                return Chart;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart!", ex);
            }
        }

        /// <summary>
        /// create the chart title
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartTitle(XElement node)
        {
            try
            {
                ChartTitle title = new ChartTitle(Chart.Document, node) {Chart = Chart};
                Chart.ChartTitle = title;
                //title.Node                   = node;
                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(title.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "title");
                ContentCollection iColl = new ContentCollection();

                if (style != null)
                {
                    title.Style = style;
                    Chart.Styles.Add(style);
                }

                foreach (XElement nodeChild in title.Node.Elements())
                {
                    IContent icontent = CreateContent(nodeChild);

                    if (icontent != null)
                        AddToCollection(icontent, iColl);
                }

                title.Node.Value = "";

                foreach (IContent icontent in iColl)
                {
                    AddToCollection(icontent, title.Content);
                }

                return title;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart title!", ex);
            }
        }

        /// <summary>
        /// create the chart legend
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartLegend(XElement node)
        {
            try
            {
                ChartLegend legend = new ChartLegend(Chart.Document, node) {Chart = Chart};
                //legend.Node                  = node;
                Chart.ChartLegend = legend;
                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(legend.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "legend");

                if (style != null)
                {
                    legend.Style = style;
                    Chart.Styles.Add(style);
                }

                return legend;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart legend!", ex);
            }
        }

        /// <summary>
        /// create the chart plotarea
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartPlotArea(XElement node)
        {
            try
            {
                ChartPlotArea plotarea = new ChartPlotArea(Chart.Document, node) {Chart = Chart};
                //plotarea.Node               = node;
                Chart.ChartPlotArea = plotarea;

                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(plotarea.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "plotarea");

                ContentCollection iColl = new ContentCollection();

                if (style != null)
                {
                    plotarea.Style = style;
                    Chart.Styles.Add(style);
                }

                foreach (XElement nodeChild in plotarea.Node.Elements())
                {
                    IContent icontent = CreateContent(nodeChild);

                    if (icontent != null)
                        AddToCollection(icontent, iColl);
                }

                plotarea.Node.Value = "";

                foreach (IContent icontent in iColl)
                {
                    if (icontent is Dr3dLight)
                    {
                        ((Dr3dLight) icontent).PlotArea = plotarea;
                        plotarea.Dr3dLightCollection.Add(icontent as Dr3dLight);
                    }

                    else if (icontent is ChartAxis)
                    {
                        ((ChartAxis) icontent).PlotArea = plotarea;
                        plotarea.AxisCollection.Add(icontent as ChartAxis);
                    }

                    else if (icontent is ChartSeries)
                    {
                        ((ChartSeries) icontent).PlotArea = plotarea;
                        plotarea.SeriesCollection.Add(icontent as ChartSeries);
                    }

                    else
                    {
                        AddToCollection(icontent, plotarea.Content);
                    }
                }

                return plotarea;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart plotarea!", ex);
            }
        }

        /// <summary>
        /// create the chart axes
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartAxes(XElement node)
        {
            try
            {
                ChartAxis axes = new ChartAxis(Chart.Document, node) {Chart = Chart};
                //axes.Node                   = node;
                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(axes.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "axes");
                ContentCollection iColl = new ContentCollection();

                if (style != null)
                {
                    axes.Style = style;
                    Chart.Styles.Add(style);
                }


                foreach (XElement nodeChild in axes.Node.Elements())
                {
                    IContent icontent = CreateContent(nodeChild);

                    if (icontent != null)
                        AddToCollection(icontent, iColl);
                }


                axes.Node.Value = "";

                foreach (IContent icontent in iColl)
                {
                    AddToCollection(icontent, axes.Content);
                }

                return axes;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart axes!", ex);
            }
        }

        /// <summary>
        /// create the chart category
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartCategories(XElement node)
        {
            try
            {
                ChartCategories categories = new ChartCategories(Chart.Document, node) {Chart = Chart};
                //categories.Node             = node;

                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(categories.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "categories");

                if (style != null)
                {
                    categories.Style = style;
                    Chart.Styles.Add(style);
                }

                return categories;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart categories!", ex);
            }
        }

        /// <summary>
        /// create the chart grid
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartGrid(XElement node)
        {
            try
            {
                ChartGrid grid = new ChartGrid(Chart.Document, node) {Chart = Chart};
                //grid.Node                   = node;

                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(grid.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "grid");

                if (style != null)
                {
                    grid.Style = style;
                    Chart.Styles.Add(style);
                }

                return grid;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart grid!", ex);
            }
        }

        /// <summary>
        /// create the chart series
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartSeries(XElement node)
        {
            try
            {
                ChartSeries series = new ChartSeries(Chart.Document, node) {Chart = Chart};

                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(series.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "series");
                ContentCollection iColl = new ContentCollection();

                if (style != null)
                {
                    series.Style = style;
                    Chart.Styles.Add(style);
                }

                foreach (XElement nodeChild in series.Node.Elements())
                {
                    IContent icontent = CreateContent(nodeChild);

                    if (icontent != null)
                        AddToCollection(icontent, iColl);
                }

                series.Node.Value = "";

                foreach (IContent iContent in iColl)
                {
                    if (iContent is ChartDataPoint)
                        series.DataPointCollection.Add(iContent as ChartDataPoint);
                }

                return series;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart series!", ex);
            }
        }

        /// <summary>
        /// create the chart data point
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartDataPoint(XElement node)
        {
            try
            {
                ChartDataPoint datapoint = new ChartDataPoint(Chart.Document, node) {Chart = Chart};
                //grid.Node                   = node;

                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(datapoint.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "datapoint");

                if (style != null)
                {
                    datapoint.Style = style;
                    Chart.Styles.Add(style);
                }

                return datapoint;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart datapoint!", ex);
            }
        }

        /// <summary>
        /// create the chart wall
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartWall(XElement node)
        {
            try
            {
                ChartWall wall = new ChartWall(Chart.Document, node) {Chart = Chart};
                //grid.Node                   = node;

                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(wall.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "wall");

                if (style != null)
                {
                    wall.Style = style;
                    Chart.Styles.Add(style);
                }

                return wall;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart wall!", ex);
            }
        }

        /// <summary>
        /// create the chart floor
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateChartFloor(XElement node)
        {
            try
            {
                ChartFloor floor = new ChartFloor(Chart.Document, node) {Chart = Chart};
                //grid.Node                   = node;

                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(floor.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "floor");

                if (style != null)
                {
                    floor.Style = style;
                    Chart.Styles.Add(style);
                }

                return floor;
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart floor!", ex);
            }
        }

        /// <summary>
        /// create the dr3d light
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private IContent CreateDr3dLight(XElement node)
        {
            try
            {
                Dr3dLight light = new Dr3dLight(Chart.Document, node) {Chart = Chart};
                //grid.Node                    = node;

                ChartStyleProcessor csp = new ChartStyleProcessor(Chart);
                XElement nodeStyle = csp.ReadStyleNode(light.StyleName);
                IStyle style = csp.ReadStyle(nodeStyle, "dr3d");

                if (style != null)
                {
                    light.Style = style;
                    Chart.Styles.Add(style);
                }

                return light;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while creating the chart dr3dlight!", ex);
            }
        }

        public Paragraph CreateParagraph(XElement paragraphNode)
        {
            try
            {
                //Create a new Paragraph
                Paragraph paragraph = new Paragraph(paragraphNode, Chart.Document);
                //Recieve the ParagraphStyle
                return ReadParagraphTextContent(paragraph);
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Paragraph.", ex);
            }
        }

        private Paragraph ReadParagraphTextContent(Paragraph paragraph)
        {
            try
            {
                IList<IContent> mixedContent = new List<IContent>();
                foreach (XElement nodeChild in paragraph.Node.Elements())
                {
                    //Check for IText content first
                    TextContentProcessor tcp = new TextContentProcessor();
                    IText iText = tcp.CreateTextObject(Chart.Document, new XElement(nodeChild));

                    if (iText != null)
                        mixedContent.Add(iText);
                    else
                    {
                        //Check against IContent
                        IContent iContent = CreateContent(nodeChild);

                        if (iContent != null)
                            mixedContent.Add(iContent);
                    }
                }

                //Remove all
                paragraph.Node.Value = "";

                foreach (IContent ob in mixedContent)
                {
                    if (ob is IText)
                    {
                        XNode textNode = ob.Node;
                        paragraph.Node.Add(textNode is XText
                                               ? (XNode) new XText((XText) textNode)
                                               : new XElement((XElement) textNode));
                    }
                    else
                    {
                        //paragraph.Content.Add(ob as IContent);
                        AddToCollection(ob, paragraph.Content);
                    }
                }
                return paragraph;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create the Paragraph content.", ex);
            }
        }


        private static void AddToCollection(IContent content, CollectionWithEvents<IContent> coll)
        {
            coll.Add(content);
        }
    }
}