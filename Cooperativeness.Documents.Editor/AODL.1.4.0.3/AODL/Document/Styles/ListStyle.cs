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
    /// Represent the ListStyle for a List.
    /// </summary>
    public class ListStyle : AbstractStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ListStyle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public ListStyle(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            ListlevelStyles = new ListLevelStyleCollection();
        }

        /// <summary>
        /// Create a new ListStyle object.
        /// </summary>
        /// <param name="document">The docuemnt</param>
        /// <param name="styleName">The style name</param>
        public ListStyle(IDocument document, string styleName)
        {
            Document = document;
            Node = new XElement(Ns.Text + "tab");
            PropertyCollection = new IPropertyCollection();
            PropertyCollection.Inserted += PropertyCollection_Inserted;
            PropertyCollection.Removed += PropertyCollection_Removed;
            //			this.Document.Styles.Add(this);
            ListlevelStyles = new ListLevelStyleCollection();
            StyleName = styleName;
        }

        /// <summary>
        /// The ListLevelStyles which belongs to this object.
        /// </summary>
        public ListLevelStyleCollection ListlevelStyles { get; set; }

        /// <summary>
        /// Add all possible ListLevelStyle objects automatically.
        /// Throws exception, if there are already ListLevelStyles
        /// which could'nt removed.
        /// </summary>
        /// <param name="typ">The Liststyle bullet, numbered, ..s</param>
        public void AutomaticAddListLevelStyles(ListStyles typ)
        {
            Node.RemoveNodes();

            ListlevelStyles.Clear();

            for (int i = 1; i <= 10; i++)
            {
                ListLevelStyle style = new ListLevelStyle(Document, this, typ, i);
                ListlevelStyles.Add(style);
                //this.Document.Styles.Add(style);
            }

            foreach (ListLevelStyle lls in ListlevelStyles)
                Node.Add(lls.Node);
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

    /// <summary>
    /// Represent the different kinds of lis styles.
    /// </summary>
    public enum ListStyles
    {
        /// <summary>
        /// Numbered list
        /// </summary>
        Number,
        /// <summary>
        /// Bullet list
        /// </summary>
        Bullet
    }
}

/*
 * $Log: ListStyle.cs,v $
 * Revision 1.2  2008/04/29 15:39:54  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:49  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.3  2006/02/21 19:34:56  larsbm
 * - Fixed Bug text that contains a xml tag will be imported  as UnknowText and not correct displayed if document is exported  as HTML.
 * - Fixed Bug [ 1436080 ] Common styles
 *
 * Revision 1.2  2006/01/29 18:52:51  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
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
 * Revision 1.1  2005/10/09 15:52:47  larsbm
 * - Changed some design at the paragraph usage
 * - add list support
 *
 */