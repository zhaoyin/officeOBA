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

namespace AODL.Document.Forms.Controls
{
    public class ODFFixedText : ODFFormControl
    {
        /// <summary>
        /// Creates an ODFFixedText
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFFixedText(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
        }

        /// <summary>
        /// Creates an ODFFixedText
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFFixedText(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                            string width, string height) : base(parentForm, contentCollection, id, x, y, width, height)
        {
        }

        public ODFFixedText(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
        }

        public override string Type
        {
            get { return "fixed-text"; }
        }


        /// <summary>
        /// Specifies whether or not a control can accept user input
        /// </summary>
        public bool? Disabled
        {
            get { return (bool?) Node.Attribute(Ns.Form + "disabled"); }
            set { Node.SetAttributeValue(Ns.Form + "disabled", value); }
        }

        /// <summary>
        /// Specifies the IDs of the controls with which control element is labeling
        /// </summary>
        public string For
        {
            get { return (string) Node.Attribute(Ns.Form + "for"); }
            set { Node.SetAttributeValue(Ns.Form + "for", value); }
        }

        /// <summary>
        /// Contains additional information about a control.
        /// </summary>
        public string Title
        {
            get { return (string) Node.Attribute(Ns.Form + "title"); }
            set { Node.SetAttributeValue(Ns.Form + "title", value); }
        }

        /// <summary>
        /// Specifies whether or not a control is printed when a user prints 
        /// the document in which the control is contained
        /// </summary>
        public bool? Printable
        {
            get { return (bool?) Node.Attribute(Ns.Form + "printable"); }
            set { Node.SetAttributeValue(Ns.Form + "printable", value); }
        }

        /// <summary>
        /// Specifies whether or not the label is displayed on multiple lines
        /// </summary>
        public bool? MultiLine
        {
            get { return (bool?) Node.Attribute(Ns.Form + "multi-line"); }
            set { Node.SetAttributeValue(Ns.Form + "multi-line", value); }
        }

        /// <summary>
        /// Contains a label for the control
        /// </summary>
        public string Label
        {
            get { return (string) Node.Attribute(Ns.Form + "label"); }
            set { Node.SetAttributeValue(Ns.Form + "label", value); }
        }

        protected override void CreateBasicNode()
        {
            XElement xe = new XElement(Ns.Form + "fixed-text");
            Node.Add(xe);
            Node = xe;
            ControlImplementation = "ooo:com.sun.star.form.component.FixedText";
        }
    }
}