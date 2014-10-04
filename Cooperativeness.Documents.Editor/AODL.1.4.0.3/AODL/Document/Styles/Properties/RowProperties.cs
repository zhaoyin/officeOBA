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
    /// RowProperties represent table row properties.
    /// </summary>
    public class RowProperties : IProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RowProperties"/> class.
        /// </summary>
        /// <param name="style">The rowstyle.</param>
        public RowProperties(IStyle style)
        {
            Style = style;
            Node = new XElement(Ns.Style + "table-row-properties");
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
        /// Gets the HTML style.
        /// </summary>
        /// <returns></returns>
        public string GetHtmlStyle()
        {
            string style = "style=\"";

            if (BackgroundColor != null)
                style += "background-color: " + BackgroundColor + "; ";

            if (!style.EndsWith("; "))
                style = "";
            else
                style += "\"";

            return style;
        }

        #endregion

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public string BackgroundColor
        {
            get { return (string) Node.Attribute(Ns.Fo + "background-color"); }
            set { Node.SetAttributeValue(Ns.Fo + "background-color", value); }
        }
    }
}

/*
 * $Log: RowProperties.cs,v $
 * Revision 1.2  2008/04/29 15:39:56  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:55  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.2  2006/02/05 20:03:32  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.2  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.1  2005/10/15 11:40:31  larsbm
 * - finished first step for table support
 *
 */