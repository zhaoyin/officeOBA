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

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using AODL.Document.Styles;
using AODL.Document.Styles.Properties;

namespace AODL.Document.Import.OpenDocument.NodeProcessors
{
    /// <summary>
    /// LocalStyleProcessor.
    /// </summary>
    public class LocalStyleProcessor
    {
        /// <summary>
        /// Is true if the common styles are read.
        /// </summary>
        private readonly bool _common;

        /// <summary>
        /// The document
        /// </summary>
        private readonly IDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalStyleProcessor"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="readCommonStyles">if set to <c>true</c> [read common styles].</param>
        public LocalStyleProcessor(IDocument document, bool readCommonStyles)
        {
            _document = document;
            CurrentXmlDocument = _document.XmlDoc;
            _common = readCommonStyles;
        }

        /// <summary>
        /// Gets or sets the current XML document.
        /// </summary>
        /// <value>The current XML document.</value>
        public XDocument CurrentXmlDocument { get; set; }

        /// <summary>
        /// Reads the styles.
        /// </summary>
        public void ReadStyles()
        {
            XElement automaticStyleNode;

            if (!_common)
                automaticStyleNode = _document.XmlDoc.Elements(Ns.Office + "document-content")
                    .Elements(Ns.Office + "automatic-styles").FirstOrDefault();
            else
                automaticStyleNode = _document.XmlDoc.Elements(Ns.Office + "document-content")
                    .Elements(Ns.Office + "styles").FirstOrDefault();

            if (automaticStyleNode != null)
            {
                foreach (XElement styleNode in automaticStyleNode.Elements())
                {
                    string family = (string) styleNode.Attribute(Ns.Style + "family");

                    if (family != null)
                    {
                        if (family == "table")
                        {
                            CreateTableStyle(styleNode);
                        }
                        else if (family == "table-column")
                        {
                            CreateColumnStyle(styleNode);
                        }
                        else if (family == "table-row")
                        {
                            CreateRowStyle(styleNode);
                        }
                        else if (family == "table-cell")
                        {
                            CreateCellStyle(styleNode);
                        }
                        else if (family == "paragraph")
                        {
                            CreateParagraphStyle(styleNode);
                        }
                        else if (family == "graphic")
                        {
                            CreateFrameStyle(styleNode);
                        }
                        else if (family == "section")
                        {
                            CreateSectionStyle(styleNode);
                        }
                        else if (family == "text")
                        {
                            CreateTextStyle(styleNode);
                        }
                    }
                    else if (styleNode.Name == Ns.Text + "list-style")
                    {
                        CreateListStyle(styleNode);
                    }
                    else
                    {
                        CreateUnknownStyle(styleNode);
                    }
                }
                automaticStyleNode.RemoveAll();
            }
        }


        /// <summary>
        /// Re read known automatic styles.
        /// </summary>
        /// <remarks>
        /// NOTICE: The re read nodes will be deleted.
        /// </remarks>
        public void ReReadKnownAutomaticStyles()
        {
            XElement contentAutomaticStyles =
                _document.XmlDoc.Descendants(Ns.Office + "automatic-styles").FirstOrDefault();
            if (contentAutomaticStyles != null && contentAutomaticStyles.HasElements)
            {
                IList<XNode> deleteNodes = new List<XNode>();
                foreach (XElement styleNode in contentAutomaticStyles.Elements())
                {
                    string family = (string) styleNode.Attribute(Ns.Style + "family");

                    if (family != null)
                    {
                        if (family == "table")
                        {
                            CreateTableStyle(styleNode);
                            deleteNodes.Add(styleNode);
                        }
                        else if (family == "table-column")
                        {
                            CreateColumnStyle(styleNode);
                            deleteNodes.Add(styleNode);
                        }
                        else if (family == "table-row")
                        {
                            CreateRowStyle(styleNode);
                            deleteNodes.Add(styleNode);
                        }
                        else if (family == "table-cell")
                        {
                            CreateCellStyle(styleNode);
                            deleteNodes.Add(styleNode);
                        }
                        else if (family == "paragraph")
                        {
                            CreateParagraphStyle(styleNode);
                            deleteNodes.Add(styleNode);
                        }
                        else if (family == "graphic")
                        {
                            CreateFrameStyle(styleNode);
                            deleteNodes.Add(styleNode);
                        }
                        else if (family == "section")
                        {
                            CreateSectionStyle(styleNode);
                            deleteNodes.Add(styleNode);
                        }
                        else if (family == "text")
                        {
                            CreateTextStyle(styleNode);
                            deleteNodes.Add(styleNode);
                        }
                    }
                    else if (styleNode.Name == Ns.Text + "list-style")
                    {
                        CreateListStyle(styleNode);
                        deleteNodes.Add(styleNode);
                    }
                }

                foreach (XNode deleteNode in deleteNodes)
                {
                    deleteNode.Remove();
                }
            }
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private IProperty GetProperty(IStyle style, XElement propertyNode)
        {
            if (propertyNode != null && style != null)
            {
                if (propertyNode.Name == Ns.Style + "table-cell-properties")
                    return CreateCellProperties(style, propertyNode);
                if (propertyNode.Name == Ns.Style + "table-column-properties")
                    return CreateColumnProperties(style, propertyNode);
                if (propertyNode.Name == Ns.Style + "graphic-properties")
                    return CreateGraphicProperties(style, propertyNode);
                if (propertyNode.Name == Ns.Style + "paragraph-properties")
                    return CreateParagraphProperties(style, propertyNode);
                if (propertyNode.Name == Ns.Style + "table-row-properties")
                    return CreateRowProperties(style, propertyNode);
                if (propertyNode.Name == Ns.Style + "section-properties")
                    return CreateSectionProperties(style, propertyNode);
                if (propertyNode.Name == Ns.Style + "table-properties")
                    return CreateTableProperties(style, propertyNode);
                if (propertyNode.Name == Ns.Style + "text-properties")
                    return CreateTextProperties(style, propertyNode);
                return CreateUnknownProperties(style, propertyNode);
            }
            return null;
        }

        /// <summary>
        /// Creates the unknown style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateUnknownStyle(XElement styleNode)
        {
            if (!_common)
                _document.Styles.Add(new UnknownStyle(_document, new XElement(styleNode)));
            else
                _document.CommonStyles.Add(new UnknownStyle(_document, new XElement(styleNode)));
        }

        /// <summary>
        /// Creates the table style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateTableStyle(XElement styleNode)
        {
            TableStyle tableStyle = new TableStyle(_document) {Node = new XElement(styleNode)};
            IPropertyCollection pCollection = new IPropertyCollection();

            if (styleNode.HasElements)
            {
                foreach (XElement node in styleNode.Elements())
                {
                    IProperty property = GetProperty(tableStyle, new XElement(node));
                    if (property != null)
                        pCollection.Add(property);
                }
            }

            tableStyle.Node.Value = "";

            foreach (IProperty property in pCollection)
                tableStyle.PropertyCollection.Add(property);

            if (!_common)
                _document.Styles.Add(tableStyle);
            else
                _document.CommonStyles.Add(tableStyle);
        }

        /// <summary>
        /// Creates the column style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateColumnStyle(XElement styleNode)
        {
            ColumnStyle columnStyle = new ColumnStyle(_document) {Node = new XElement(styleNode)};
            IPropertyCollection pCollection = new IPropertyCollection();

            if (styleNode.HasElements)
            {
                foreach (XElement node in styleNode.Elements())
                {
                    IProperty property = GetProperty(columnStyle, new XElement(node));
                    if (property != null)
                        pCollection.Add(property);
                }
            }

            columnStyle.Node.Value = "";

            foreach (IProperty property in pCollection)
                columnStyle.PropertyCollection.Add(property);

            if (!_common)
                _document.Styles.Add(columnStyle);
            else
                _document.CommonStyles.Add(columnStyle);
        }

        /// <summary>
        /// Creates the row style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateRowStyle(XElement styleNode)
        {
            RowStyle rowStyle = new RowStyle(_document) {Node = new XElement(styleNode)};
            IPropertyCollection pCollection = new IPropertyCollection();

            if (styleNode.HasElements)
            {
                foreach (XElement node in styleNode.Elements())
                {
                    IProperty property = GetProperty(rowStyle, new XElement(node));
                    if (property != null)
                        pCollection.Add(property);
                }
            }

            rowStyle.Node.Value = "";

            foreach (IProperty property in pCollection)
                rowStyle.PropertyCollection.Add(property);

            if (!_common)
                _document.Styles.Add(rowStyle);
            else
                _document.CommonStyles.Add(rowStyle);
        }

        /// <summary>
        /// Creates the cell style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateCellStyle(XElement styleNode)
        {
            CellStyle cellStyle = new CellStyle(_document) {Node = new XElement(styleNode)};
            IPropertyCollection pCollection = new IPropertyCollection();

            if (styleNode.HasElements)
            {
                foreach (XElement node in styleNode.Elements())
                {
                    IProperty property = GetProperty(cellStyle, new XElement(node));
                    if (property != null)
                        pCollection.Add(property);
                }
            }

            cellStyle.Node.Value = "";

            foreach (IProperty property in pCollection)
                cellStyle.PropertyCollection.Add(property);

            if (!_common)
                _document.Styles.Add(cellStyle);
            else
                _document.CommonStyles.Add(cellStyle);
        }

        /// <summary>
        /// Creates the list style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateListStyle(XElement styleNode)
        {
            ListStyle listStyle = new ListStyle(_document, styleNode);

            if (!_common)
                _document.Styles.Add(listStyle);
            else
                _document.CommonStyles.Add(listStyle);
        }

        /// <summary>
        /// Creates the paragraph style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateParagraphStyle(XElement styleNode)
        {
            ParagraphStyle paragraphStyle = new ParagraphStyle(_document, styleNode);
            IPropertyCollection pCollection = new IPropertyCollection();

            if (styleNode.HasElements)
            {
                foreach (XElement node in styleNode.Elements())
                {
                    IProperty property = GetProperty(paragraphStyle, new XElement(node));
                    if (property != null)
                        pCollection.Add(property);
                }
            }

            paragraphStyle.Node.Value = "";

            foreach (IProperty property in pCollection)
                paragraphStyle.PropertyCollection.Add(property);

            if (!_common)
                _document.Styles.Add(paragraphStyle);
            else
                _document.CommonStyles.Add(paragraphStyle);
        }

        /// <summary>
        /// Creates the text style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateTextStyle(XElement styleNode)
        {
            TextStyle textStyle = new TextStyle(_document, new XElement(styleNode));
            IPropertyCollection pCollection = new IPropertyCollection();

            if (styleNode.HasElements)
            {
                foreach (XElement node in styleNode.Elements())
                {
                    IProperty property = GetProperty(textStyle, new XElement(node));
                    if (property != null)
                        pCollection.Add(property);
                }
            }

            textStyle.Node.Value = "";

            foreach (IProperty property in pCollection)
                textStyle.PropertyCollection.Add(property);

            if (!_common)
                _document.Styles.Add(textStyle);
            else
                _document.CommonStyles.Add(textStyle);
        }

        /// <summary>
        /// Creates the frame style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateFrameStyle(XElement styleNode)
        {
            FrameStyle frameStyle = new FrameStyle(_document, new XElement(styleNode));
            IPropertyCollection pCollection = new IPropertyCollection();

            if (styleNode.HasElements)
            {
                foreach (XElement node in styleNode.Elements())
                {
                    IProperty property = GetProperty(frameStyle, new XElement(node));
                    if (property != null)
                        pCollection.Add(property);
                }
            }

            frameStyle.Node.Value = "";

            foreach (IProperty property in pCollection)
                frameStyle.PropertyCollection.Add(property);

            if (!_common)
                _document.Styles.Add(frameStyle);
            else
                _document.CommonStyles.Add(frameStyle);
        }

        /// <summary>
        /// Creates the section style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        private void CreateSectionStyle(XElement styleNode)
        {
            SectionStyle sectionStyle = new SectionStyle(_document, new XElement(styleNode));
            IPropertyCollection pCollection = new IPropertyCollection();

            if (styleNode.HasElements)
            {
                foreach (XElement node in styleNode.Elements())
                {
                    IProperty property = GetProperty(sectionStyle, new XElement(node));
                    if (property != null)
                        pCollection.Add(property);
                }
            }

            sectionStyle.Node.Value = "";

            foreach (IProperty property in pCollection)
                sectionStyle.PropertyCollection.Add(property);

            if (!_common)
                _document.Styles.Add(sectionStyle);
            else
                _document.CommonStyles.Add(sectionStyle);
        }

        /// <summary>
        /// Creates the tab stop style.
        /// </summary>
        /// <param name="styleNode">The style node.</param>
        /// <returns></returns>
        private TabStopStyle CreateTabStopStyle(XElement styleNode)
        {
            TabStopStyle tabStopStyle = new TabStopStyle(_document, new XElement(styleNode));

            return tabStopStyle;
        }

        /// <summary>
        /// Creates the table properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private static TableProperties CreateTableProperties(IStyle style, XElement propertyNode)
        {
            TableProperties tableProperties = new TableProperties(style) {Node = propertyNode};

            return tableProperties;
        }

        /// <summary>
        /// Creates the column properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private static ColumnProperties CreateColumnProperties(IStyle style, XElement propertyNode)
        {
            ColumnProperties columnProperties = new ColumnProperties(style) {Node = propertyNode};

            return columnProperties;
        }

        /// <summary>
        /// Creates the row properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private static RowProperties CreateRowProperties(IStyle style, XElement propertyNode)
        {
            RowProperties rowProperties = new RowProperties(style) {Node = propertyNode};

            return rowProperties;
        }

        /// <summary>
        /// Creates the cell properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private static CellProperties CreateCellProperties(IStyle style, XElement propertyNode)
        {
            CellProperties cellProperties = new CellProperties(style as CellStyle) {Node = propertyNode};

            return cellProperties;
        }

        /// <summary>
        /// Creates the graphic properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private static GraphicProperties CreateGraphicProperties(IStyle style, XElement propertyNode)
        {
            GraphicProperties graphicProperties = new GraphicProperties(style) {Node = propertyNode};

            return graphicProperties;
        }

        /// <summary>
        /// Creates the paragraph properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private ParagraphProperties CreateParagraphProperties(IStyle style, XElement propertyNode)
        {
            ParagraphProperties paragraphProperties = new ParagraphProperties(style) {Node = propertyNode};
            TabStopStyleCollection tabCollection = new TabStopStyleCollection(_document);

            if (propertyNode.HasElements)
                foreach (XElement node in propertyNode.Elements())
                    if (node.Name == Ns.Style + "tab-stops")
                        foreach (XElement nodeTab in node.Elements())
                            if (nodeTab.Name == Ns.Style + "tab-stop")
                                tabCollection.Add(CreateTabStopStyle(nodeTab));

            if (tabCollection.Count > 0)
                paragraphProperties.TabStopStyleCollection = tabCollection;

            return paragraphProperties;
        }

        /// <summary>
        /// Creates the text properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private static TextProperties CreateTextProperties(IStyle style, XElement propertyNode)
        {
            TextProperties textProperties = new TextProperties(style) {Node = propertyNode};

            return textProperties;
        }

        /// <summary>
        /// Creates the section properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private static SectionProperties CreateSectionProperties(IStyle style, XElement propertyNode)
        {
            SectionProperties sectionProperties = new SectionProperties(style) {Node = propertyNode};

            return sectionProperties;
        }

        /// <summary>
        /// Creates the unknown properties.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <param name="propertyNode">The property node.</param>
        /// <returns></returns>
        private static UnknownProperty CreateUnknownProperties(IStyle style, XElement propertyNode)
        {
            return new UnknownProperty(style, propertyNode);
        }
    }
}