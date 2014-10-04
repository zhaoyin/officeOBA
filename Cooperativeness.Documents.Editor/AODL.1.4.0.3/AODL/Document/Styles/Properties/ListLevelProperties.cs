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
    /// Represent the properties of the list levels.
    /// </summary>
    public class ListLevelProperties : IProperty
    {
        /// <summary>
        /// Constructor create a new ListLevelProperties object.
        /// </summary>
        public ListLevelProperties(IStyle style)
        {
            Style = style;
            Node = new XElement(Ns.Style + "list-level-properties");
        }

        #region IProperty Member

        /// <summary>
        /// The XElement.
        /// </summary>
        public XElement Node { get; set; }

        /// <summary>
        /// The style to this ListLevelProperties object belongs.
        /// </summary>
        public IStyle Style { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the space before.
        /// </summary>
        /// <value>The space before.</value>
        public string SpaceBefore
        {
            get { return (string) Node.Attribute(Ns.Text + "space-before"); }
            set { Node.SetAttributeValue(Ns.Text + "space-before", value); }
        }

        /// <summary>
        /// Gets or sets the width of the min label.
        /// </summary>
        /// <value>The width of the min label.</value>
        public string MinLabelWidth
        {
            get { return (string) Node.Attribute(Ns.Text + "min-label-width"); }
            set { Node.SetAttributeValue(Ns.Text + "min-label-width", value); }
        }
    }
}