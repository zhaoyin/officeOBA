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
using AODL.Document.Styles;

namespace AODL.Document.Content.Fields
{
    /// <summary>
    /// A base abstract class for all the fields
    /// </summary>
    public abstract class Field : IContent
    {
        public ContentCollection ContentCollection { get; set; }

        /// <summary>
        /// The inner content of the field
        /// </summary>
        public string Value
        {
            get { return Node.Value; }
            set { Node.Value = value; }
        }

        public XElement Node { get; set; }

        #region IContent Members

        public IDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        XNode IContent.Node
        {
            get { return Node; }
            set { Node = (XElement) value; }
        }

        /// <summary>
        /// The stylename wihich is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        public string StyleName
        {
            get { return (string) Node.Attribute(Ns.Draw + "style-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "style-name", value); }
        }

        /// <summary>
        /// A Style class wich is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        public IStyle Style
        {
            get { return Document.Styles.GetStyleByName(StyleName); }
            set { StyleName = value.StyleName; }
        }

        #endregion
    }
}