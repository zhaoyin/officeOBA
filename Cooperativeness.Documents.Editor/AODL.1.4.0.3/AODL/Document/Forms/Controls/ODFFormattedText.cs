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
    /// <summary>
    /// Summary description for ODFFormattedText.
    /// </summary>
    public class ODFFormattedText : ODFTextArea
    {
        /// <summary>
        /// Creates an ODFFormattedText
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFFormattedText(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
        }

        /// <summary>
        /// Creates an ODFFormattedText
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFFormattedText(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                                string width, string height)
            : base(parentForm, contentCollection, id, x, y, width, height)
        {
        }

        public ODFFormattedText(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
        }

        public override string Type
        {
            get { return "formatted-text"; }
        }

        /// <summary>
        /// Specifies the minimum value that a user can enter.
        /// </summary>
        public int MinValue
        {
            get { return (int?) Node.Attribute(Ns.Form + "min-value") ?? 0; }
            set { Node.SetAttributeValue(Ns.Form + "min-value", value); }
        }

        /// <summary>
        /// Specifies the maximum value that a user can enter.
        /// </summary>
        public int MaxValue
        {
            get { return (int?) Node.Attribute(Ns.Form + "max-value") ?? 0; }
            set { Node.SetAttributeValue(Ns.Form + "max-value", value); }
        }

        /// <summary>
        /// Specifies whether or not the text that the user enters is 
        /// validated during input
        /// </summary>
        public bool? Validation
        {
            get { return (bool?) Node.Attribute(Ns.Form + "validation"); }
            set { Node.SetAttributeValue(Ns.Form + "validation", value); }
        }

        /// <summary>
        /// Specifies the tabbing navigation order of a control within a form
        /// </summary>
        public new int TabIndex
        {
            get { return (int) Node.Attribute(Ns.Form + "tab-index"); }
            set { Node.SetAttributeValue(Ns.Form + "tab-index", value); }
        }

        /// <summary>
        /// Specifies whether or not a control is included in the tabbing 
        /// navigation order
        /// </summary>
        public new bool? TabStop
        {
            get { return (bool?) Node.Attribute(Ns.Form + "tab-stop"); }
            set { Node.SetAttributeValue(Ns.Form + "tab-stop", value); }
        }

        /// <summary>
        /// Specifies the current status of an input control
        /// </summary>
        public new string CurrentValue
        {
            get { return (string) Node.Attribute(Ns.Form + "current-value"); }
            set { Node.SetAttributeValue(Ns.Form + "current-value", value); }
        }

        /// <summary>
        /// Specifies whether or not a control can accept user input
        /// </summary>
        public new bool? Disabled
        {
            get { return (bool?) Node.Attribute(Ns.Form + "disabled"); }
            set { Node.SetAttributeValue(Ns.Form + "disabled", value); }
        }

        /// <summary>
        /// Specifies the maximum number of characters that a user can 
        /// enter in an input control
        /// </summary>
        public new int MaxLength
        {
            get { return (int?) Node.Attribute(Ns.Form + "max-length") ?? -1; }
            set
            {
                if (value <= 0)
                    return;
                Node.SetAttributeValue(Ns.Form + "max-length", value);
            }
        }

        /// <summary>
        /// Specifies whether or not a control is printed when a user prints 
        /// the document in which the control is contained
        /// </summary>
        public new bool? Printable
        {
            get { return (bool?) Node.Attribute(Ns.Form + "printable"); }
            set { Node.SetAttributeValue(Ns.Form + "printable", value); }
        }

        /// <summary>
        /// Specifies whether or not a user can modify the value of a control
        /// </summary>
        public new bool? ReadOnly
        {
            get { return (bool?) Node.Attribute(Ns.Form + "readonly"); }
            set { Node.SetAttributeValue(Ns.Form + "readonly", value); }
        }

        /// <summary>
        /// Contains additional information about a control.
        /// </summary>
        public new string Title
        {
            get { return (string) Node.Attribute(Ns.Form + "title"); }
            set { Node.SetAttributeValue(Ns.Form + "title", value); }
        }

        /// <summary>
        /// Specifies the default value of the control
        /// </summary>
        public new string Value
        {
            get { return (string) Node.Attribute(Ns.Form + "value"); }
            set { Node.SetAttributeValue(Ns.Form + "value", value); }
        }

        /// <summary>
        /// specifies whether or not empty current values are regarded as NULL
        /// </summary>
        public new bool? ConvertEmptyToNull
        {
            get { return (bool?) Node.Attribute(Ns.Form + "convert-empty-to-null"); }
            set { Node.SetAttributeValue(Ns.Form + "convert-empty-to-null", value); }
        }

        /// <summary>
        /// Specifies the name of a result set column. The result set is 
        /// determined by the form which the control belongs to
        /// </summary>
        public new string DataField
        {
            get { return (string) Node.Attribute(Ns.Form + "data-field"); }
            set { Node.SetAttributeValue(Ns.Form + "data-field", value); }
        }

        protected override void CreateBasicNode()
        {
            XElement xe = new XElement(Ns.Form + "formatted-text");
            Node.Add(xe);
            Node = xe;
            ControlImplementation = "ooo:com.sun.star.form.component.FormattedField";
        }
    }
}