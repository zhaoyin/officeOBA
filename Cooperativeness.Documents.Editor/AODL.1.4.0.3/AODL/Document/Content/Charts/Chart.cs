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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using AODL.Document.Content.Draw;
using AODL.Document.Content.EmbedObjects;
using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Styles;
using AODL.IO;

namespace AODL.Document.Content.Charts
{
    /// <summary>
    /// chart respresents a chart object which embed into the spresdsheet document.
    /// It is inherited from the EmbedObject
    /// </summary>
    public class Chart : EmbedObject
    {
        #region  chart content

        private ChartTitle _charttitle;

        /// <summary>
        /// Gets or Sets the chart Title
        /// 
        /// <value>The chart Title.</value>
        /// </summary>
        public ChartTitle ChartTitle
        {
            get { return _charttitle; }

            set
            {
                for (int i = 0; i < _content.Count; i++)
                {
                    if (_content[i] is ChartTitle)
                    {
                        _content[i] = value;
                    }
                }
                _charttitle = value;
            }
        }

        /// <summary>
        /// Gets or Sets the chart Legend
        /// 
        /// <value>The chart Legend.</value>
        /// </summary>
        public ChartLegend ChartLegend { get; set; }

        /// <summary>
        /// Gets or Sets the chart PlotArea
        /// 
        /// <value>The chart PlotArea.</value>
        /// </summary>
        public ChartPlotArea ChartPlotArea { get; set; }

        /// <summary>
        /// Gets or sets the chart style.
        /// </summary>
        /// <value>The chart style.</value>
        public ChartStyle ChartStyle
        {
            get { return (ChartStyle) Style; }
            set { Style = value; }
        }

        /// <summary>
        /// Collection of local styles used with the chart.
        /// </summary>
        /// <value></value>
        public StyleCollection Styles { get; set; }

        public ChartStyles ChartStyles { get; set; }

        /// <summary>
        /// If this file was loaded
        /// </summary>
        /// <value></value>
        public bool IsNewed { get; set; }

        #endregion

        #region chart propertity

        /// <summary>
        /// Gets or sets the width of the chart
        /// </summary>
        /// <value>The name of the width of the chart</value>
        public string SvgWidth
        {
            get { return (string) Node.Attribute(Ns.Svg + "width"); }
            set { Node.SetAttributeValue(Ns.Svg + "width", value); }
        }

        /// <summary>
        /// Gets or sets the height of the chart
        /// </summary>
        /// <value>The name of the height of the chart</value>
        public string SvgHeight
        {
            get { return (string) Node.Attribute(Ns.Svg + "height"); }
            set { Node.SetAttributeValue(Ns.Svg + "height", value); }
        }

        /// <summary>
        /// Gets or sets the type of the chart
        /// </summary>
        /// <value>The name of the type of the chart</value>
        public string ChartType
        {
            get { return (string) Node.Attribute(Ns.Chart + "class"); }
            set { Node.SetAttributeValue(Ns.Chart + "class", value); }
        }

        /// <summary>
        /// Gets or sets the cellrange of the data table
        /// It sets or gets from the parentNode
        /// </summary>
        /// <value>the cellrange of the data table</value>
        public string CellRange
        {
            get { return (string) ParentNode.Attribute(Ns.Draw + "notify-on-update-of-ranges"); }
            set { ParentNode.SetAttributeValue(Ns.Draw + "notify-on-update-of-ranges", value); }
        }

        /// <summary>
        /// sets the title of the xaxis
        /// </summary>
        /// <value>the title of the xaxis</value>
        public string XAxisName
        {
            get

            {
                foreach (ChartAxis axis in ChartPlotArea.AxisCollection)
                {
                    if (axis.Dimension == "x")

                    {
                        foreach (IContent iContent in axis.Content)
                        {
                            if (iContent is ChartTitle)
                            {
                                Paragraph para = (Paragraph) ((ChartTitle) iContent).Content[0];
                                return (para.TextContent[0].Node is XElement)
                                           ? ((XElement) para.TextContent[0].Node).Value
                                           : ((XText) para.TextContent[0].Node).Value;
                            }
                        }
                    }

                    else
                        return null;
                }

                return null;
            }
            set
            {
                ChartAxis xaxis = null;
                bool flag = false;
                foreach (ChartAxis axis in ChartPlotArea.AxisCollection)
                {
                    if (axis.Dimension == "x")
                        xaxis = axis;
                }
                if (xaxis != null)
                {
                    Styles.Remove(xaxis.Style);
                    ChartPlotArea.AxisCollection.Remove(xaxis);
                    flag = true;
                }


                xaxis = new ChartAxis(this, "ch5") {Dimension = "x", AxisName = "primary-x"};
                xaxis.AxesStyle.AxesProperties.DisplayLabel = "true";
                xaxis.IsModified = flag;
                ChartTitle axisTitle = new ChartTitle(this);
                Paragraph para = new Paragraph(Document);
                para.TextContent.Add(new SimpleText(Document, value));
                axisTitle.Content.Add(para);
                xaxis.Content.Add(axisTitle);
                ChartPlotArea.AxisCollection.Add(xaxis);
            }
        }

        /// <summary>
        /// sets the title of the yaxis
        /// </summary>
        /// <value>the title of the yaxis</value>
        public string YAxisName
        {
            get

            {
                foreach (ChartAxis axis in ChartPlotArea.AxisCollection)
                {
                    if (axis.Dimension == "y")

                    {
                        foreach (IContent iContent in axis.Content)
                        {
                            if (iContent is ChartTitle)
                            {
                                Paragraph para = (Paragraph) ((ChartTitle) iContent).Content[0];
                                return (para.TextContent[0].Node is XElement)
                                           ? ((XElement) para.TextContent[0].Node).Value
                                           : ((XText) para.TextContent[0].Node).Value;
                            }
                        }
                    }

                    else
                        return null;
                }

                return null;
            }
            set

            {
                ChartAxis yaxis = null;

                bool flag = false;

                foreach (ChartAxis axis in ChartPlotArea.AxisCollection)
                {
                    if (axis.Dimension == "y")
                        yaxis = axis;
                }

                if (yaxis != null)
                {
                    Styles.Remove(yaxis.Style);
                    ChartPlotArea.AxisCollection.Remove(yaxis);
                    flag = true;
                }


                yaxis = new ChartAxis(this, "ch6") {Dimension = "y", AxisName = "primary-y", IsModified = flag};

                yaxis.AxesStyle.AxesProperties.DisplayLabel = "true";
                ChartTitle axisTitle = new ChartTitle(this);
                Paragraph para = new Paragraph(Document);
                para.TextContent.Add(new SimpleText(Document, value));
                axisTitle.Content.Add(para);
                yaxis.Content.Add(axisTitle);
                ChartPlotArea.AxisCollection.Add(yaxis);
            }
        }


        /// <summary>
        /// gets and sets the end cell address of the chart
        /// It sets and gets from the frame node which contains the embed object
        /// <value> the end cell range of the frame</value>
        /// </summary>
        public string EndCellAddress
        {
            get { return (string) Frame.Node.Attribute(Ns.Table + "end-cell-address"); }
            set { Frame.Node.SetAttributeValue(Ns.Table + "end-cell-address", value); }
        }

        /// <summary>
        /// gets and sets the end x of  the frame which chart in
        /// It sets and gets from the frame node which contains the embed object
        /// <value> the end x of the frame</value>
        /// </summary>
        public string EndX
        {
            get { return (string) Frame.Node.Attribute(Ns.Table + "end-x"); }
            set { Frame.Node.SetAttributeValue(Ns.Table + "end-x", value); }
        }

        /// <summary>
        /// gets and sets the end y of  the frame which chart in
        /// It sets and gets from the frame node which contains the embed object
        /// <value> the end y of the frame</value>
        /// </summary>
        public string EndY
        {
            get { return (string) Frame.Node.Attribute(Ns.Table + "end-y"); }
            set { Frame.Node.SetAttributeValue(Ns.Table + "end-y", value); }
        }

        /// <summary>
        /// Gets or sets the horizontal position where
        /// the chart should be
        /// anchored.
        /// </summary>
        /// <example>myFrame.SvgX = "1.5cm"</example>
        /// <value>The SVG X.</value>
        public string SvgX
        {
            get { return (string) Frame.Node.Attribute(Ns.Svg + "x"); }
            set { Frame.Node.SetAttributeValue(Ns.Svg + "x", value); }
        }

        /// <summary>
        /// Gets or sets the vertical position where
        /// the chart should be
        /// anchored.
        /// </summary>
        /// <example>myFrame.SvgY = "1.5cm"</example>
        /// <value>The SVG Y.</value>
        public string SvgY
        {
            get { return (string) Frame.Node.Attribute(Ns.Svg + "y"); }
            set { Frame.Node.SetAttributeValue(Ns.Svg + "y", value); }
        }

        #endregion

        public CellRanges TableData;

        //public  CellRanges     DisplayArea;

        /// <summary>
        /// the xml document which save the content of the chart
        /// </summary>
        public XDocument ChartDoc { get; set; }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <value>The name of the style.</value>
        public override string StyleName
        {
            get { return (string) Node.Attribute(Ns.Chart + "style-name"); }
            set { Node.SetAttributeValue(Ns.Chart + "style-name", value); }
        }

        #region chart constructor

        public Chart(IDocument document, XElement node, XElement parentNode) : base(parentNode, document)
        {
            Node = node;
            Styles = new StyleCollection();
            ChartStyles = new ChartStyles(this);
            Document.EmbedObjects.Add(this);
            Document.DocumentMetadata.ObjectCount += 1;
            InitStandards();
        }

        public Chart(Table table, string styleName) : base(table.Document)
        {
            TableData.table = table;
            Document = table.Document;
            Styles = new StyleCollection();
            ChartStyles = new ChartStyles(this);
            ChartDoc = new XDocument();
            NewXmlNode(styleName);
            ObjectType = "chart";
            Document.DocumentMetadata.ObjectCount += 1;
            ObjectName = "Object " + Document.DocumentMetadata.ObjectCount;
            New();
            Document.EmbedObjects.Add(this);
        }

        #endregion

        #region create the new chart

        public void New()
        {
            IsNewed = true;
            LoadBlankContent();
            LoadBlankStyles();
            InitStandards();
            ChartLegend = new ChartLegend(this, "ch2");
            ChartTitle = new ChartTitle(this, "ch3");
            ChartTitle.InitTitle();
            Content.Add(ChartTitle);
            ChartPlotArea = new ChartPlotArea(this, "ch4");
            if (Frame == null)
            {
                Frame = new Frame(Document, "gr1");
                //this.CreateParentNode (null);
                Frame.Content.Add(this);
            }
            InitChart();
        }

        /// <summary>
        /// Load the blank content
        /// </summary>
        private void LoadBlankContent()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            Stream str = ass.GetManifestResourceStream("AODL.Resources.OD.chartcontent.xml");
            using (XmlReader reader = XmlReader.Create(str))
            {
                ChartDoc = XDocument.Load(reader);
            }
        }

        /// <summary>
        /// Load the blank style
        /// </summary>
        private void LoadBlankStyles()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            Stream str = ass.GetManifestResourceStream("AODL.Resources.OD.chartstyle.xml");
            using (XmlReader reader = XmlReader.Create(str))
            {
                ChartStyles.Styles = XDocument.Load(reader);
            }
        }

        #endregion

        private void NewXmlNode(string styleName)
        {
            Node = new XElement(Ns.Chart + "chart");

            if (styleName != null)
            {
                Node.SetAttributeValue(Ns.Chart + "style-name", styleName);
                ChartStyle = new ChartStyle(Document, styleName);
                Styles.Add(ChartStyle);
            }
        }


        private void InitStandards()
        {
            Content = new ContentCollection();
            //this.Content.Inserted	+=new CollectionWithEvents.CollectionChange(Content_Inserted);
            //this.Content.Removed	+=new CollectionWithEvents.CollectionChange(Content_Removed);
        }

        /// <summary>
        /// create the parentnode of the chart
        /// </summary>
        /// <param name="objectlink"></param>
        public XElement CreateParentNode(string objectlink)
        {
            ParentNode.SetAttributeValue(Ns.XLink + "href", objectlink);
            return ParentNode;
        }

        [Obsolete("Use chart builder instead")]
        public void CreateTableFromCellRange()
        {
        }

        /// <summary>
        /// create the chart from the cell range of the data table
        /// </summary>
        /// <param name="cellRange"></param>
        public void CreateFromCellRange(string cellRange)
        {
            int index1 = cellRange.IndexOf(":", 0);
            int length = cellRange.Length;
            string startCell = cellRange.Substring(0, index1);
            int index2 = index1 + 1;
            string endCell = cellRange.Substring(index2, length - index2);
            string tableName = TableData.table.TableName;
            string range = tableName + "." + startCell + ":" + tableName + "." + endCell;

            CellRange = range;
            CellPos StartCell = GetCellPos(startCell, TableData.table);


            TableData.startcell = StartCell;


            CellPos EndCell = GetCellPos(endCell, TableData.table);


            TableData.endcell = EndCell;


            CellRange = range;

            if (IsNewed)
                CreateSeries();
        }

        /// <summary>
        /// analyse the xml string which
        /// contains the cell range of the data table
        /// </summary>
        /// <returns>CellRange</returns>
        public CellRanges GetCellRangeFromXMLString()
        {
            CellRanges chartData = new CellRanges();

            string cellrange = CellRange;

            #region

            if (!string.IsNullOrEmpty(cellrange))
            {
                int index1 = cellrange.IndexOf(".", 0);

                if (index1 != -1)
                {
                    string tableName = cellrange.Substring(0, index1);
                    Table dataTable = GetTable(tableName);

                    if (dataTable != null)
                        chartData.table = dataTable;

                    int index2 = cellrange.IndexOf(":", 0);
                    string startCell = cellrange.Substring((index1 + 1), index2 - index1 - 1);
                    CellPos StartCell = GetCellPos(startCell, dataTable);


                    chartData.startcell = StartCell;

                    int index3 = cellrange.IndexOf(".", (index1 + 1));
                    int length = cellrange.Length;
                    string endCell = cellrange.Substring((index3 + 1), length - index3 - 1);
                    CellPos EndCell = GetCellPos(endCell, dataTable);


                    chartData.endcell = EndCell;
                    TableData = chartData;
                }
            }

            #endregion

            return chartData;
        }

        /*
		public CellRanges  GetDisplayCellRangeFromXMLString()
		{
			try
			{
				CellRanges chartDisplay = new CellRanges ();
				string endcellAddress      = this.EndCellAddress ;
				

				if (endcellAddress!=null&&endcellAddress!="")
				{
			    
					int index1             = endcellAddress.IndexOf (".",0);
					int length             = endcellAddress.Length ;
					string tableName       = endcellAddress.Substring (0,index1);
					Table tableDisplay     = GetTable(tableName);
					if (tableDisplay!=null)
						chartDisplay.table = tableDisplay;
					string endCell         = endcellAddress.Substring (index1,length-index1);
					CellPos   EndCell         = GetCellPos(tableName,tableDisplay);
					
					chartDisplay.endcell =EndCell;
				}

				return chartDisplay;
			}
			catch(Exception ex)
			{
				throw;

			}
		}
		 
		 */


        /// <summary>
        /// gets the table according to the name of the table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public Table GetTable(string tableName)
        {
            IDocument doc = Document;

            if (doc is SpreadsheetDocument)
            {
                foreach (Table table in ((SpreadsheetDocument) doc).TableCollection)
                {
                    if (table.TableName == tableName)
                        return table;
                }
            }

            return null;
        }

        /// <summary>
        /// gets the rowIndex and colIndex of the chart according to the name of the cell
        /// </summary>
        /// <param name="cellName"></param>
        /// <param name="tableData"></param>
        /// <returns></returns>
        public CellPos GetCellPos(string cellName, Table tableData)
        {
            int i = 0;
            int columnIndex = 0;
            char[] columnStr = new char[2];

            foreach (char c in cellName)
            {
                if (c >= 'A' && c <= 'Z')
                {
                    columnStr[i] = c;
                    i++;
                }
            }

            if (i == 1)
                columnIndex = columnStr[0] - 'A';
            if (i == 2)
                columnIndex = (columnStr[0] - 'A')*26 + (columnStr[1] - 'A');
            columnIndex = columnIndex + 1;

            string rowStr = cellName.Substring(i);
            int rowIndex = Int32.Parse(rowStr);

            Cell cell = null;

            if (rowIndex <= tableData.Rows.Count)
            {
                Row row = tableData.Rows[rowIndex - 1];
                if (columnIndex <= row.Cells.Count)
                {
                    cell = tableData.Rows[rowIndex - 1].Cells[columnIndex - 1];
                }
            }

            if (cell == null)
            {
                cell = new Cell(Document);
                tableData.InsertCellAt(rowIndex, columnIndex, cell);
            }

            CellPos pos = new CellPos {cell = cell, columnIndex = columnIndex, rowIndex = rowIndex};

            return pos;
        }

        /// <summary>
        /// Init the chart
        /// </summary>
        private void InitChart()
        {
            EndX = "0.096cm";
            EndY = "0.45cm";
            SvgWidth = "8cm";
            SvgHeight = "7cm";
            SvgX = "1.128cm";
            SvgY = "0.225cm";
        }

        /// <summary>
        /// Creates the content body
        /// </summary>
        private void CreateContentBody()
        {
            XElement nodeRoot = ChartDoc.Elements(Ns.Office + "document-content")
                .Elements(Ns.Office + "body")
                .Elements(Ns.Office + "chart").First();


            nodeRoot.RemoveAll();
            XElement nodeChart = new XElement(Node);
            nodeRoot.Add(nodeChart);


            foreach (IContent iContent in Content)
            {
                //if (iContent is Chart)
                //nodeChart.AppendChild(((Chart)iContent).Node );
                if (iContent is ChartLegend)
                {
                    nodeChart.Add(new XElement(((ChartLegend) iContent).Node));
                }

                if (iContent is ChartTitle)
                {
                    nodeChart.Add(new XElement(((ChartTitle) iContent).Node));
                }

                if (iContent is ChartPlotArea)
                {
                    nodeChart.Add(new XElement(((ChartPlotArea) iContent).BuildNode()));
                }

                if (iContent is Table)
                {
                    nodeChart.Add(new XElement(((Table) iContent).BuildNode()));
                }
            }

            CreateLocalStyleContent();
            //this.CreateCommonStyleContent();
        }


        /// <summary>
        /// add  data table to the content of the chart
        /// </summary>
        private void AdddDataTableToChart()
        {
            GetCellRangeFromXMLString();
            Table table = new ChartBuilderHelper(Document, ChartPlotArea, TableData)
                .CreateTableFromCellRange();
            //this.CreateTableFromCellRange ();
            Content.Add(table);
        }

        /// <summary>
        /// save the content of the chart
        /// </summary>
        /// <param name="packageWriter"></param>
        public void SaveTo(IPackageWriter packageWriter)
        {
            AdddDataTableToChart();
            CreateContentBody();
            ChartExporter export = new ChartExporter();
            export.Export(Document, packageWriter);
        }

        /// <summary>
        /// Creates the content of the local style.
        /// </summary>
        private void CreateLocalStyleContent()
        {
            XElement nodeAutomaticStyles = ChartDoc.Elements(Ns.Office + "document-content")
                .Elements(Ns.Office + "automatic-styles").First();

            nodeAutomaticStyles.RemoveAll();
            foreach (IStyle style in Styles.ToValueList())
            {
                nodeAutomaticStyles.Add(new XElement(style.Node));
            }
        }

        /// <summary>
        /// create the series of the chart
        /// </summary>
        private void CreateSeries()
        {
            int rowStart = TableData.startcell.rowIndex;
            int rowEnd = TableData.endcell.rowIndex;
            int colStart = TableData.startcell.columnIndex;
            int colEnd = TableData.endcell.columnIndex;

            if (ChartPlotArea.PlotAreaStyle.PlotAreaProperties.SeriesSource == "rows")
            {
                CreateSingleSeries(rowStart, rowEnd, colStart, colEnd);
            }

            if (ChartPlotArea.PlotAreaStyle.PlotAreaProperties.SeriesSource == "columns")
            {
                CreateSingleSeries(colStart, colEnd, rowStart, rowEnd);
            }
        }

        /// <summary>
        /// create a single series
        /// </summary>
        /// <param name="rowStartIndex">the start row index</param>
        /// <param name="rowEndINdex">the end row index</param>
        /// <param name="colStartIndex">the start col index</param>
        /// <param name="colEndIndex">the end col index</param>
        private void CreateSingleSeries(int rowStartIndex, int rowEndINdex, int colStartIndex, int colEndIndex)
        {
            for (int i = rowStartIndex; i <= rowEndINdex; i++)
            {
                string styleRowname = "ch1" + i;
                ChartSeries series = new ChartSeries(this, styleRowname);

                if (ChartType == "Circle")
                {
                    for (int j = colStartIndex; j <= colEndIndex; j++)
                    {
                        string styleColname = "ch2" + j;
                        ChartDataPoint dataPoint = new ChartDataPoint(this, styleColname);
                        series.DataPointCollection.Add(dataPoint);
                    }
                }
                else
                {
                    string pointStylename = "ch3" + i;
                    ChartDataPoint dataPoint = new ChartDataPoint(this, pointStylename);
                    int repeated = colEndIndex - colStartIndex + 1;
                    dataPoint.Repeated = repeated.ToString();
                    series.DataPointCollection.Add(dataPoint);
                }

                ChartPlotArea.SeriesCollection.Add(series);
            }
        }

        #region IContentContainer Member

        private ContentCollection _content;

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public ContentCollection Content
        {
            get { return _content; }
            set { _content = value; }
        }

        #endregion
    }
}