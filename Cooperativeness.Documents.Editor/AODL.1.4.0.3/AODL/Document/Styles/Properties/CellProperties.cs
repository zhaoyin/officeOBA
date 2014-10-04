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

namespace AODL.Document.Styles.Properties
{
    /// <summary>
    /// Represent the Cell Properties within a Cell which is used
    /// within a Tabe resp. a Row.
    /// </summary>
    public class CellProperties : IProperty, IHtmlStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CellProperties"/> class.
        /// </summary>
        /// <param name="cellstyle">The cellstyle.</param>
        public CellProperties(CellStyle cellstyle)
        {
            CellStyle = cellstyle;
            Node = new XElement(Ns.Style + "table-cell-properties");
            //TODO: Check localisations cm?? inch??
            //defaults 
            Padding = "0.097cm";
        }

        #region IProperty Member

        /// <summary>
        /// The XElement which represent the property element.
        /// </summary>
        /// <value>The node</value>
        public XElement Node { get; set; }

        /// <summary>
        /// The style object to which this property object belongs
        /// </summary>
        /// <value></value>
        public IStyle Style { get; set; }

        #endregion

        #region IHtmlStyle Member

        /// <summary>
        /// Get the css style fragement
        /// </summary>
        /// <returns>The css style attribute</returns>
        public string GetHtmlStyle()
        {
            string style = "style=\"";

            if (BackgroundColor != null)
                if (BackgroundColor.ToLower() != "transparent")
                    style += "background-color: " + BackgroundColor + "; ";
                else
                    style += "background-color: #FFFFFF; ";
            else
                style += "background-color: #FFFFFF; ";

            if (!style.EndsWith("; "))
                style = "";
            else
                style += "\"";

            return style;
        }

        #endregion

        /// <summary>
        /// Gets or sets the cell style.
        /// </summary>
        /// <value>The cell style.</value>
        public CellStyle CellStyle
        {
            get { return (CellStyle) Style; }
            set { Style = value; }
        }

        /// <summary>
        /// Gets or sets the padding. 
        /// Default 0.097cm
        /// </summary>
        /// <value>The padding.</value>
        public string Padding
        {
            get { return (string) Node.Attribute(Ns.Fo + "padding"); }
            set { Node.SetAttributeValue(Ns.Fo + "padding", value); }
        }

        /// <summary>
        /// Gets or sets the border.
        /// This could be e.g. 0.002cm solid #000000 (width, linestyle, color)
        /// or none
        /// </summary>
        /// <value>The border.</value>
        public string Border
        {
            get { return (string) Node.Attribute(Ns.Fo + "border"); }
            set { Node.SetAttributeValue(Ns.Fo + "border", value); }
        }

        /// <summary>
        /// Gets or sets the border left.
        /// This could be e.g. 0.002cm solid #000000 (width, linestyle, color)
        /// or none
        /// </summary>
        /// <value>The border left.</value>
        public string BorderLeft
        {
            get { return (string) Node.Attribute(Ns.Fo + "border-left"); }
            set { Node.SetAttributeValue(Ns.Fo + "border-left", value); }
        }

        /// <summary>
        /// Gets or sets the border right.
        /// This could be e.g. 0.002cm solid #000000 (width, linestyle, color)
        /// or none
        /// </summary>
        /// <value>The border right.</value>
        public string BorderRight
        {
            get { return (string) Node.Attribute(Ns.Fo + "border-right"); }
            set { Node.SetAttributeValue(Ns.Fo + "border-right", value); }
        }

        /// <summary>
        /// Gets or sets the border top.
        /// This could be e.g. 0.002cm solid #000000 (width, linestyle, color)
        /// or none
        /// </summary>
        /// <value>The border top.</value>
        public string BorderTop
        {
            get { return (string) Node.Attribute(Ns.Fo + "border-top"); }
            set { Node.SetAttributeValue(Ns.Fo + "border-top", value); }
        }

        /// <summary>
        /// Gets or sets the border bottom.
        /// This could be e.g. 0.002cm solid #000000 (width, linestyle, color)
        /// or none
        /// </summary>
        /// <value>The border bottom.</value>
        public string BorderBottom
        {
            get { return (string) Node.Attribute(Ns.Fo + "border-bottom"); }
            set { Node.SetAttributeValue(Ns.Fo + "border-bottom", value); }
        }

        /// <summary>
        /// Gets or sets the color of the background. e.g #000000 for black
        /// </summary>
        /// <value>The color of the background.</value>
        public string BackgroundColor
        {
            get { return (string) Node.Attribute(Ns.Fo + "background-color"); }
            set { Node.SetAttributeValue(Ns.Fo + "background-color", value); }
        }
    }
}