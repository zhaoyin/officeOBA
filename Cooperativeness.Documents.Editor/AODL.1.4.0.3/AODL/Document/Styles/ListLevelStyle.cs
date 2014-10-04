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
using System.Xml.Linq;
using AODL.Document.Styles.Properties;

namespace AODL.Document.Styles
{
    /// <summary>
    /// Represent the list level style.
    /// </summary>
    public class ListLevelStyle : AbstractStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListLevelStyle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="style">The style.</param>
        /// <param name="typ">The typ.</param>
        /// <param name="level">The level.</param>
        public ListLevelStyle(IDocument document, ListStyle style, ListStyles typ, int level)
        {
            Document = document;
            ListStyle = style;
            InitStandards();

            NewXmlNode(typ, level);
            AddListLevel(level);
        }

        /// <summary>
        /// The ListStyle to this object belongs.
        /// </summary>
        public ListStyle ListStyle { get; set; }

        /// <summary>
        /// Gets or sets the text properties.
        /// </summary>
        /// <value>The text properties.</value>
        public ListLevelProperties ListLevelProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is ListLevelProperties)
                        return (ListLevelProperties) property;
                ListLevelProperties listLevelProperties = new ListLevelProperties(this);
                PropertyCollection.Add(listLevelProperties);
                return listLevelProperties;
            }
            set
            {
                if (PropertyCollection.Contains(value))
                    PropertyCollection.Remove(value);
                PropertyCollection.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the text properties.
        /// </summary>
        /// <value>The text properties.</value>
        public TextProperties TextProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is TextProperties)
                        return (TextProperties) property;
                TextProperties textProperties = new TextProperties(this);
                PropertyCollection.Add(textProperties);
                return textProperties;
            }
            set
            {
                if (PropertyCollection.Contains(value))
                    PropertyCollection.Remove(value);
                PropertyCollection.Add(value);
            }
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
        /// Adds the list level.
        /// </summary>
        private void AddListLevel(int level)
        {
            ListLevelProperties = new ListLevelProperties(this);
            double spacebefore = 0.635;
            spacebefore *= level;
            string space = spacebefore.ToString().Replace(",", ".") + "cm";
            const string minlabelwidth = "0.635cm";
            ListLevelProperties.MinLabelWidth = minlabelwidth;
            ListLevelProperties.SpaceBefore = space;
        }

        /// <summary>
        /// Create the XElement that represent the Style element.
        /// </summary>
        /// <param name="typ">The style name.</param>
        /// <param name="level">The level number which represent this style.</param>
        private void NewXmlNode(ListStyles typ, int level)
        {
            if (typ == ListStyles.Bullet)
            {
                Node = new XElement(Ns.Text + "list-level-style-bullet");
                Node.SetAttributeValue(Ns.Text + "style-name", "Bullet_20_Symbols");
                Node.SetAttributeValue(Ns.Text + "bullet-char", "\u2022");

                AddTextPropertie();
            }
            else if (typ == ListStyles.Number)
            {
                Node = new XElement(Ns.Text + "list-level-style-number");
                Node.SetAttributeValue(Ns.Text + "style-name", "Numbering_20_Symbols");
                Node.SetAttributeValue(Ns.Style + "num-format", "1");
            }
            else
                throw new Exception("Unknown ListStyles typ");

            Node.SetAttributeValue(Ns.Text + "level", level.ToString());
            Node.SetAttributeValue(Ns.Style + "num-suffix", ".");
        }

        /// <summary>
        /// Add the TextPropertie, necessary if the list is a bullet list.
        /// </summary>
        private void AddTextPropertie()
        {
            TextProperties = new TextProperties(this) {FontName = "StarSymbol"};
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
 * $Log: ListLevelStyle.cs,v $
 * Revision 1.2  2008/04/29 15:39:54  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:48  larsbehr
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
 * Revision 1.2  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 * Revision 1.1  2005/10/09 15:52:47  larsbm
 * - Changed some design at the paragraph usage
 * - add list support
 *
 */