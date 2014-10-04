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
using AODL.Document.Styles.Properties;

namespace AODL.Document.Styles
{
    /// <summary>
    /// Represent the style for a Paragraph object.
    /// </summary>
    public class ParagraphStyle : AbstractStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphStyle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="styleName">Name of the style.</param>
        public ParagraphStyle(IDocument document, string styleName)
        {
            Document = document;
            Node = new XElement(Ns.Style + "style");
            Node.SetAttributeValue(Ns.Style + "name", styleName);
            Node.SetAttributeValue(Ns.Style + "family", FamiliyStyles.Paragraph);
            Node.SetAttributeValue(Ns.Style + "parent-style-name", ParentStyles.Standard.ToString());
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParagraphStyle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public ParagraphStyle(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        /// <summary>
        /// Gets or sets the paragraph properties.
        /// </summary>
        /// <value>The paragraph properties.</value>
        public ParagraphProperties ParagraphProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is ParagraphProperties)
                        return (ParagraphProperties) property;
                ParagraphProperties parProperties = new ParagraphProperties(this);
                PropertyCollection.Add(parProperties);
                return parProperties;
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
        /// The parent style of this object.
        /// </summary>
        public string ParentStyle
        {
            get { return (string) Node.Attribute(Ns.Style + "parent-style-name"); }
            set { Node.SetAttributeValue(Ns.Style + "parent-style-name", value); }
        }

        /// <summary>
        /// The parent style of this object.
        /// </summary>
        public string ListStyleName
        {
            get { return (string) Node.Attribute(Ns.Style + "list-style-name"); }
            set { Node.SetAttributeValue(Ns.Style + "list-style-name", value); }
        }

        /// <summary>
        /// The family style.
        /// </summary>
        public string Family
        {
            get { return (string) Node.Attribute(Ns.Style + "family"); }
            set { Node.SetAttributeValue(Ns.Style + "family", value); }
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
 * $Log: ParagraphStyle.cs,v $
 * Revision 1.2  2008/04/29 15:39:54  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:49  larsbehr
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
 * Revision 1.6  2006/01/05 10:31:10  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 * Revision 1.5  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 * Revision 1.4  2005/10/22 15:52:10  larsbm
 * - Changed some styles from Enum to Class with statics
 * - Add full support for available OpenOffice fonts
 *
 * Revision 1.3  2005/10/09 15:52:47  larsbm
 * - Changed some design at the paragraph usage
 * - add list support
 *
 * Revision 1.2  2005/10/08 07:55:35  larsbm
 * - added cvs tags
 *
 */