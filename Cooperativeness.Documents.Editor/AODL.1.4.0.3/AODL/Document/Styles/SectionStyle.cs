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
using AODL.Document.Content;
using AODL.Document.Styles.Properties;

namespace AODL.Document.Styles
{
    /// <summary>
    /// SectionStyle represent a section style.
    /// </summary>
    public class SectionStyle : AbstractStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SectionStyle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public SectionStyle(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
            SectionProperties = new SectionProperties(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionStyle"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="styleName">Name of the style.</param>
        public SectionStyle(IContent content, string styleName)
        {
            Content = content;
            Document = content.Document;
            Node = new XElement(Ns.Style + "style");
            Node.SetAttributeValue(Ns.Style + "name", styleName);
            Node.SetAttributeValue(Ns.Style + "family", "section");
            InitStandards();
            SectionProperties = new SectionProperties(this);
//			this.Document.Styles.Add(this);
        }

        /// <summary>
        /// Gets or sets the content to this
        /// section style belongs
        /// </summary>
        /// <value>The content.</value>
        public IContent Content { get; set; }

        /// <summary>
        /// Gets or sets the section properties.
        /// </summary>
        /// <value>The section properties.</value>
        public SectionProperties SectionProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is SectionProperties)
                        return (SectionProperties) property;
                SectionProperties sectionProperties = new SectionProperties(this);
                PropertyCollection.Add(sectionProperties);
                return sectionProperties;
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
            //this.Document.Styles.Add(this);
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
 * $Log: SectionStyle.cs,v $
 * Revision 1.2  2008/04/29 15:39:54  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:49  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.3  2006/02/05 20:03:32  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.2  2006/01/29 18:52:51  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
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