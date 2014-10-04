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
using AODL.Document.Content.Text;
using AODL.Document.Styles;

namespace AODL.Document.Content.Charts
{
    /// <summary>
    /// Summary description for ChartTitle.
    /// </summary>
    public class ChartTitle : IContent, IContentContainer
    {
        /// <summary>
        /// Initializes a new instance of the charttitle class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public ChartTitle(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the charttitle class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public ChartTitle(Chart chart) : this(chart, null)
        {
            //chart.Content .Add (this);
        }

        /// <summary>
        /// Initializes a new instance of the charttitle class.
        /// </summary>
        /// <param name="chart">The Chart.</param>
        /// <param name="styleName">The style name.</param>
        public ChartTitle(Chart chart, string styleName)
        {
            Chart = chart;
            Document = chart.Document;
            Node = new XElement(Ns.Chart + "title");
            InitStandards();

            StyleName = styleName;
            TitleStyle = new TitleStyle(Document, styleName);
            Chart.Styles.Add(TitleStyle);

            //chart.Content .Add (this);
        }

        public Chart Chart { get; set; }

        /// <summary>
        /// Gets or sets the title style.
        /// </summary>
        /// <value>The title style.</value>
        public TitleStyle TitleStyle
        {
            get { return (TitleStyle) Style; }
            set
            {
                StyleName = (value).StyleName;
                Style = value;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal position where
        /// the title should be
        /// anchored. 
        /// </summary>
        public string SvgX
        {
            get { return (string) Node.Attribute(Ns.Svg + "x"); }
            set { Node.SetAttributeValue(Ns.Svg + "x", value); }
        }

        /// <summary>
        /// Gets or sets the vertical position where
        /// the title should be
        /// anchored. 
        /// </summary>
        public string SvgY
        {
            get { return (string) Node.Attribute(Ns.Svg + "y"); }
            set { Node.SetAttributeValue(Ns.Svg + "y", value); }
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
        /// Init the title's default text
        /// </summary>
        public void InitTitle()
        {
            Paragraph para = new Paragraph(Chart.Document);
            para.TextContent.Add(new SimpleText(Document, "Ö÷±êÌâ"));
            Content.Add(para);
        }

        /// <summary>
        /// set the title
        /// </summary>
        /// <param name="title"></param>
        public void SetTitle(string title)
        {
            if (Content.Count != 0)
            {
                for (int i = 0; i < Content.Count; i++)
                    Content.RemoveAt(i);
            }
            Paragraph para = new Paragraph(Chart.Document);
            para.TextContent.Add(new SimpleText(Document, title));

            if (Chart.Document.IsLoadedFile && !Chart.IsNewed)
            {
                Node.Add(new XElement(para.Node));
            }

            else
            {
                Content.Add(para);
            }
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