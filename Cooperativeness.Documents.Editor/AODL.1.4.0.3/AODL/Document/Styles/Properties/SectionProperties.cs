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
    /// SectionProperties represent the section properties which is e.g used
    /// within a table of contents.
    /// </summary>
    public class SectionProperties : IProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionProperties"/> class.
        /// </summary>
        /// <param name="style">The style.</param>
        public SectionProperties(IStyle style)
        {
            Style = style;
            Node = new XElement(Ns.Style + "section-properties");

            XAttribute xa = new XAttribute(Ns.Style + "editable", "false");
            Node.Add(xa);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SectionProperties"/> is editable.
        /// </summary>
        /// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
        public bool Editable
        {
            get { return (bool?)Node.Attribute(Ns.Style + "editable") ?? false; }
            set { Node.SetAttributeValue(Ns.Style + "editable", value); }
        }

        /// <summary>
        /// Adds the standard column style.
        /// While creating new TableOfContent objects
        /// AODL will only support a TableOfContent
        /// which use the Header styles with outlining
        /// without table columns
        /// </summary>
        public void AddStandardColumnStyle()
        {
            XElement standardColStyle = new XElement(Ns.Style + "columns");

            XAttribute xa = new XAttribute(Ns.Fo + "column-count", "0");
            standardColStyle.Add(xa);

            xa = new XAttribute(Ns.Fo + "column-gap", "0cm");
            standardColStyle.Add(xa);

            Node.Add(standardColStyle);
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
    }
}

/*
 * $Log: SectionProperties.cs,v $
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
 * Revision 1.1  2006/01/05 10:31:10  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 */