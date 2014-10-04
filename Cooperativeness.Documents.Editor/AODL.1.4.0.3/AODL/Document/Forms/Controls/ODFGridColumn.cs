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

namespace AODL.Document.Forms.Controls
{

    #region ODFGridColumn class

    public class ODFGridColumn
    {
        /// <summary>
        /// Creates an ODFGridColumn
        /// </summary>
        /// <param name="document">Main document</param>
        /// <param name="name">Column name</param>
        /// <param name="label">Column label</param>
        /// <param name="style">Column style</param>
        public ODFGridColumn(IDocument document, string name, string label, string style) : this(document, name, label)
        {
            ColumnStyle = style;
        }

        /// <summary>
        /// Creates an ODFGridColumn
        /// </summary>
        /// <param name="document">Main document</param>
        /// <param name="name">Column name</param>
        /// <param name="label">Column label</param>
        public ODFGridColumn(IDocument document, string name, string label) : this(document)
        {
            Name = name;
            Label = label;
        }

        /// <summary>
        /// Creates an ODFGridColumn
        /// </summary>
        /// <param name="document">Main document</param>
        public ODFGridColumn(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Form + "column");
        }

        public ODFGridColumn(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        public XElement Node { get; set; }

        /// <summary>
        /// The document
        /// </summary>
        public IDocument Document { get; set; }


        /// <summary>
        /// Specifies the name of the column
        /// </summary>
        public string Name
        {
            get { return (string) Node.Attribute(Ns.Form + "name"); }
            set { Node.SetAttributeValue(Ns.Form + "name", value); }
        }

        /// <summary>
        /// Specifies the label of the column
        /// </summary>
        public string Label
        {
            get { return (string) Node.Attribute(Ns.Form + "label"); }
            set { Node.SetAttributeValue(Ns.Form + "label", value); }
        }

        /// <summary>
        /// Specifies the style of the column
        /// </summary>
        public string ColumnStyle
        {
            get { return (string) Node.Attribute(Ns.Form + "text-style-name"); }
            set { Node.SetAttributeValue(Ns.Form + "text-style-name", value); }
        }
    }

    #endregion
}