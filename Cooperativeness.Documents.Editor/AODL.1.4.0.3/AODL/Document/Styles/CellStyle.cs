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
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Styles.Properties;

namespace AODL.Document.Styles
{
    /// <summary>
    /// CellStyle represent the style that is
    /// used within a table cell.
    /// </summary>
    public class CellStyle : AbstractStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CellStyle"/> class.
        /// </summary>
        /// <param name="document">The spreadsheet document.</param>
        public CellStyle(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Style + "style");
            FamilyStyle = FamiliyStyles.TableCell;

            if (Document is SpreadsheetDocument)
                ParentStyleName = "Default";
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CellStyle"/> class.
        /// </summary>
        /// <param name="document">The spreadsheet document.</param>
        /// <param name="styleName">Name of the style.</param>
        public CellStyle(IDocument document, string styleName) : this(document)
        {
            StyleName = styleName;
        }

        /// <summary>
        /// Gets or sets the cell properties.
        /// </summary>
        /// <value>The cell properties.</value>
        public CellProperties CellProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is CellProperties)
                        return (CellProperties) property;
                CellProperties cellProperties = new CellProperties(this);
                PropertyCollection.Add(cellProperties);
                return cellProperties;
            }
            set
            {
                if (PropertyCollection.Contains(value))
                    PropertyCollection.Remove(value);
                PropertyCollection.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the family style.
        /// </summary>
        /// <value>The family style.</value>
        public string FamilyStyle
        {
            get { return (string) Node.Attribute(Ns.Style + "family"); }
            set { Node.SetAttributeValue(Ns.Style + "family", value); }
        }

        /// <summary>
        /// Gets or sets the name of the parent style.
        /// </summary>
        /// <value>The name of the parent style.</value>
        public string ParentStyleName
        {
            get { return (string) Node.Attribute(Ns.Style + "parent-style-name"); }
            set { Node.SetAttributeValue(Ns.Style + "parent-style-name", value); }
        }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            PropertyCollection = new IPropertyCollection();
            PropertyCollection.Inserted += PropertyCollection_Inserted;
            PropertyCollection.Removed += PropertyCollection_Removed;
//			this.Document.Styles.Add(this);
        }

        /// <summary>
        /// Properties the collection_ inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void PropertyCollection_Inserted(int index, object value)
        {
            Node.Add(((IProperty) value).Node);
        }

        /// <summary>
        /// Properties the collection_ removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void PropertyCollection_Removed(int index, object value)
        {
            ((IProperty) value).Node.Remove();
        }
    }
}

/*
 * $Log: CellStyle.cs,v $
 * Revision 1.2  2008/04/29 15:39:53  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:47  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.2  2006/01/29 18:52:51  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 */