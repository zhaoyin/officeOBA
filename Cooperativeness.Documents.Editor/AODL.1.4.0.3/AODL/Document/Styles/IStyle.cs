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
    public abstract class AbstractStyle : IStyle
    {
        protected XElement _node;
        private string _styleNameCache;

        protected AbstractStyle()
        {
            PropertyCollection = new IPropertyCollection();
            PropertyCollection.Inserted += PropertyCollection_Inserted;
            PropertyCollection.Removed += PropertyCollection_Removed;
        }

        #region IStyle Members

        /// <summary>
        /// The Xml node which represent the
        /// style
        /// </summary>
        /// <value></value>
        public XElement Node
        {
            get { return _node; }
            set { _node = value; }
        }

        /// <summary>
        /// The document to which this style
        /// belongs.
        /// </summary>
        /// <value></value>
        public IDocument Document { get; set; }

        /// <summary>
        /// Collection of properties.
        /// </summary>
        /// <value></value>
        public IPropertyCollection PropertyCollection { get; set; }

        /// <summary>
        /// The style name
        /// </summary>
        /// <value></value>
        public string StyleName
        {
            get
            {
                if (_styleNameCache != null)
                    return _styleNameCache;

                _styleNameCache = (string) Node.Attribute(Ns.Style + "name");
                return _styleNameCache;
            }
            set
            {
                Node.SetAttributeValue(Ns.Style + "name", value);
                _styleNameCache = value;
            }
        }

        #endregion

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
    /// All styles that can be used within an document
    /// in the OpenDocument format have to implement
    /// this interface.
    /// </summary>
    public interface IStyle
    {
        /// <summary>
        /// The Xml node which represent the
        /// style
        /// </summary>
        XElement Node { get; set; }

        /// <summary>
        /// The style name
        /// </summary>
        string StyleName { get; }

        /// <summary>
        /// The OpenDocument document to which this style
        /// belongs.
        /// </summary>
        IDocument Document { get; set; }

        /// <summary>
        /// Collection of properties.
        /// </summary>
        IPropertyCollection PropertyCollection { get; set; }
    }
}

/*
 * $Log: IStyle.cs,v $
 * Revision 1.2  2008/04/29 15:39:54  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:48  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 */