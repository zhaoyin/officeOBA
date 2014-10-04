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
    /********************************* WARNING!*********************************
	 * This control is not supported by the current version of OpenOffice (2.2)
	 ***************************************************************************/

    public class ODFTime : ODFFormControl
    {
        /// <summary>
        /// Creates an ODFTime
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFTime(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
        }

        /// <summary>
        /// Creates an ODFTime
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFTime(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                       string width, string height) : base(parentForm, contentCollection, id, x, y, width, height)
        {
        }

        public ODFTime(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
        }

        public override string Type
        {
            get { return "time"; }
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
        /// Contains additional information about a control.
        /// </summary>
        public string Title
        {
            get { return (string) Node.Attribute(Ns.Form + "title"); }
            set { Node.SetAttributeValue(Ns.Form + "title", value); }
        }

        /// <summary>
        /// Specifies the maximum number of characters that a user can 
        /// enter in an input control
        /// </summary>
        public int? MaxLength
        {
            get { return (int?) Node.Attribute(Ns.Form + "max-length"); }
            set
            {
                if (value.HasValue && value.Value <= 0)
                    value = null;
                Node.SetAttributeValue(Ns.Form + "max-length", value.HasValue);
            }
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
        /// Specifies whether or not a user can modify the value of a control
        /// </summary>
        public bool? ReadOnly
        {
            get { return (bool?) Node.Attribute(Ns.Form + "readonly"); }
            set { Node.SetAttributeValue(Ns.Form + "readonly", value); }
        }

        /// <summary>
        /// Specifies the tabbing navigation order of a control within a form
        /// </summary>
        public int TabIndex
        {
            get { return (int) Node.Attribute(Ns.Form + "tab-index"); }
            set { Node.SetAttributeValue(Ns.Form + "tab-index", value); }
        }

        /// <summary>
        /// Specifies whether or not a control is included in the tabbing 
        /// navigation order
        /// </summary>
        public bool? TabStop
        {
            get { return (bool?) Node.Attribute(Ns.Form + "tab-stop"); }
            set { Node.SetAttributeValue(Ns.Form + "tab-stop", value); }
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
        /// Specifies the minimum value that a user can enter.
        /// </summary>
        public string MinValue
        {
            get { return (string) Node.Attribute(Ns.Form + "min-value"); }
            set { Node.SetAttributeValue(Ns.Form + "min-value", value); }
        }

        /// <summary>
        /// Specifies the maximum value that a user can enter.
        /// </summary>
        public string MaxValue
        {
            get { return (string) Node.Attribute(Ns.Form + "max-value"); }
            set { Node.SetAttributeValue(Ns.Form + "max-value", value); }
        }

        /// <summary>
        /// specifies whether or not empty current values are regarded as NULL
        /// </summary>
        public bool? ConvertEmptyToNull
        {
            get { return (bool?) Node.Attribute(Ns.Form + "convert-empty-to-null"); }
            set { Node.SetAttributeValue(Ns.Form + "convert-empty-to-null", value); }
        }

        /// <summary>
        /// Specifies the name of a result set column. The result set is 
        /// determined by the form which the control belongs to
        /// </summary>
        public string DataField
        {
            get { return (string) Node.Attribute(Ns.Form + "data-field"); }
            set { Node.SetAttributeValue(Ns.Form + "data-field", value); }
        }

        protected override void CreateBasicNode()
        {
            XElement xe = new XElement(Ns.Form + "time");
            Node.Add(xe);
            Node = xe;
        }
    }
}