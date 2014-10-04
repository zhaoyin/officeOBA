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
    /// ColumnProperties represent the table column properties.
    /// </summary>
    public class ColumnProperties : IProperty
    {
        /// <summary>
        /// The Constructor, create new instance of ColumnProperties
        /// </summary>
        /// <param name="style">The ColumnStyle</param>
        public ColumnProperties(IStyle style)
        {
            Style = style;
            Node = new XElement(Ns.Style + "table-column-properties");
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

        /// <summary>
        /// Set the column width -> table = 16.99cm -> column = 8.49cm
        /// </summary>
        public string Width
        {
            get { return (string) Node.Attribute(Ns.Style + "column-width"); }
            set { Node.SetAttributeValue(Ns.Style + "column-width", value); }
        }

        /// <summary>
        /// Set the column relative width
        /// </summary>
        public string RelativeWidth
        {
            get { return (string) Node.Attribute(Ns.Style + "rel-column-width"); }
            set { Node.SetAttributeValue(Ns.Style + "rel-column-width", value); }
        }
    }
}

/*
 * $Log: ColumnProperties.cs,v $
 * Revision 1.3  2008/05/07 17:19:51  larsbehr
 * - Optimized Exporter Save procedure
 * - Optimized Tests behaviour
 * - Added ODF Package Layer
 * - SharpZipLib updated to current version
 *
 * Revision 1.2  2008/04/29 15:39:56  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:53  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.2  2006/02/05 20:03:32  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.3  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 * Revision 1.2  2005/10/15 11:40:31  larsbm
 * - finished first step for table support
 *
 * Revision 1.1  2005/10/12 19:52:56  larsbm
 * - start table implementation
 * - added uml diagramm
 *
 */