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
    /// TableProperties represent the table properties.
    /// </summary>
    public class TableProperties : IProperty, IHtmlStyle
    {
        /// <summary>
        /// Constructor create a new TableProperties instance.
        /// </summary>
        /// <param name="style"></param>
        public TableProperties(IStyle style)
        {
            Style = style;
            Node = new XElement(Ns.Style + "table-properties");
            //Set default properties
            Width = "16.99cm";
            Align = "margin";
            Shadow = "none";
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

            if (Width != null)
                style += "width: " + Width.Replace(",", ".") + "; ";

            if (!style.EndsWith("; "))
                style = "";
            else
                style += "\"";

            //style		+= " border=\"1\"";

            return style;
        }

        #endregion

        /// <summary>
        /// Set the table width -> 16.99cm
        /// </summary>
        public string Width
        {
            get { return (string) Node.Attribute(Ns.Style + "width"); }
            set { Node.SetAttributeValue(Ns.Style + "width", value); }
        }

        /// <summary>
        /// Set the table align -> margin
        /// </summary>
        public string Align
        {
            get { return (string) Node.Attribute(Ns.Table + "align"); }
            set { Node.SetAttributeValue(Ns.Table + "align", value); }
        }

        /// <summary>
        /// Set the table shadow
        /// </summary>
        public string Shadow
        {
            get { return (string) Node.Attribute(Ns.Style + "shadow"); }
            set { Node.SetAttributeValue(Ns.Style + "shadow", value); }
        }
    }
}

/*
 * $Log: TableProperties.cs,v $
 * Revision 1.2  2008/04/29 15:39:56  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:56  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.2  2006/02/05 20:03:32  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.3  2005/12/18 18:29:46  larsbm
 * - AODC Gui redesign
 * - AODC HTML exporter refecatored
 * - Full Meta Data Support
 * - Increase textprocessing performance
 *
 * Revision 1.2  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.1  2005/10/12 19:52:56  larsbm
 * - start table implementation
 * - added uml diagramm
 *
 */