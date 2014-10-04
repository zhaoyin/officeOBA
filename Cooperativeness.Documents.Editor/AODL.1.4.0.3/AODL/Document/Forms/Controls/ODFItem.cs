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

    #region ODFItem class

    public class ODFItem
    {
        /// <summary>
        /// Creates an ODFItem instance
        /// </summary>
        /// <param name="document">The document it belogs to</param>
        /// <param name="label">Item label</param>
        public ODFItem(IDocument document, string label) : this(document)
        {
            Label = label;
        }

        /// <summary>
        /// Creates an ODFItem instance
        /// </summary>
        /// <param name="document">The document it belogs to</param>
        public ODFItem(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Form + "item");
        }

        public ODFItem(IDocument document, XElement node)
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
        /// Contains a label for the control
        /// </summary>
        public string Label
        {
            get { return (string) Node.Attribute(Ns.Form + "label"); }
            set { Node.SetAttributeValue(Ns.Form + "label", value); }
        }
    }

    #endregion
}