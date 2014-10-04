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

    #region ODFOption class

    public class ODFOption
    {
        /// <summary>
        /// Creates an ODFOption
        /// </summary>
        /// <param name="document">Main document</param>
        /// <param name="label">Option label</param>
        public ODFOption(IDocument document, string label) : this(document)
        {
            Label = label;
        }

        /// <summary>
        /// Creates an ODFOption
        /// </summary>
        /// <param name="document">Main document</param>
        /// <param name="label">Option label</param>
        /// <param name="val">Option value</param>
        public ODFOption(IDocument document, string label, string val) : this(document, label)
        {
            Value = val;
        }

        /// <summary>
        /// Creates an ODFOption
        /// </summary>
        /// <param name="document">Main document</param>
        /// <param name="label">Option label</param>
        /// <param name="val">Option value</param>
        /// <param name="currentSelected">Is it currently selected?</param>
        public ODFOption(IDocument document, string label, string val, bool? currentSelected)
            : this(document, label, val)
        {
            CurrentSelected = currentSelected;
        }

        /// <summary>
        /// Creates an ODFOption
        /// </summary>
        /// <param name="document">Main document</param>
        public ODFOption(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Form + "option");
        }

        public ODFOption(IDocument document, XElement node)
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
        /// Specifies if the option is currently selected
        /// </summary>
        public bool? CurrentSelected
        {
            get { return (bool?) Node.Attribute(Ns.Form + "current-selected"); }
            set { Node.SetAttributeValue(Ns.Form + "current-selected", value); }
        }

        /// <summary>
        /// Specifies the default state of a radio button or option
        /// </summary>
        public bool? Selected
        {
            get { return (bool?) Node.Attribute(Ns.Form + "selected"); }
            set { Node.SetAttributeValue(Ns.Form + "selected", value); }
        }

        /// <summary>
        /// Specifies the default value of the control
        /// </summary>
        public string Value
        {
            get { return (string) Node.Attribute(Ns.Form + "value"); }
            set { Node.SetAttributeValue(Ns.Form + "value", value); }
        }

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