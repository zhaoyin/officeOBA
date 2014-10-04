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
    public class ODFButton : ODFFormControl
    {
        /// <summary>
        /// Creates an ODFButton
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFButton(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
        }

        /// <summary>
        /// Creates an ODFButton
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFButton(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                         string width, string height) : base(parentForm, contentCollection, id, x, y, width, height)
        {
        }

        public ODFButton(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
        }

        public override string Type
        {
            get { return "button"; }
        }

        /// <summary>
        /// Contains a label for the control
        /// </summary>
        public string Label
        {
            get { return (string) Node.Attribute(Ns.Form + "label"); }
            set { Node.SetAttributeValue(Ns.Form + "label", value); }
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
        /// Specifies the default value of the control
        /// </summary>
        public string Value
        {
            get { return (string) Node.Attribute(Ns.Form + "value"); }
            set { Node.SetAttributeValue(Ns.Form + "value", value); }
        }

        /// <summary>
        /// Links the control to an external file containing image data
        /// </summary>
        public string ImageData
        {
            get { return (string) Node.Attribute(Ns.Form + "image-data"); }
            set { Node.SetAttributeValue(Ns.Form + "image-data", value); }
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
        /// Specifies whether or not a control is printed when a user prints 
        /// the document in which the control is contained
        /// </summary>
        public bool? Printable
        {
            get { return (bool?) Node.Attribute(Ns.Form + "printable"); }
            set { Node.SetAttributeValue(Ns.Form + "printable", value); }
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
        /// specifies whether a form button control, when it is operated (via 
        /// mouse or keyboard), should be toggled between a "pressed" and a "not pressed" state
        /// </summary>
        public bool? Toggle
        {
            get { return (bool?) Node.Attribute(Ns.Form + "toggle"); }
            set { Node.SetAttributeValue(Ns.Form + "toggle", value); }
        }

        /// <summary>
        /// Determines whether  or not the button is the default button on the form
        /// </summary>
        public bool? DefaultButton
        {
            get { return (bool?) Node.Attribute(Ns.Form + "default-button"); }
            set { Node.SetAttributeValue(Ns.Form + "default-button", value); }
        }


        /// <summary>
        /// Specifies whether a form button control should grab the focus when it is clicked with the mouse
        /// </summary>
        public bool? FocusOnClick
        {
            get { return (bool?) Node.Attribute(Ns.Form + "focus-on-click"); }
            set { Node.SetAttributeValue(Ns.Form + "focus-on-click", value); }
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
        /// Specifies the type of the button
        /// </summary>
        public ButtonType? ButtonType
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "button-type");
                if (s == null) return null;
                switch (s)
                {
                    case "push":
                        return Forms.ButtonType.Push;
                    case "submit":
                        return Forms.ButtonType.Submit;
                    case "reset":
                        return Forms.ButtonType.Reset;
                    case "url":
                        return Forms.ButtonType.Url;

                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.ButtonType.Push:
                        s = "push";
                        break;
                    case Forms.ButtonType.Submit:
                        s = "submit";
                        break;
                    case Forms.ButtonType.Reset:
                        s = "reset";
                        break;
                    case Forms.ButtonType.Url:
                        s = "url";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "button-type", s);
            }
        }

        /// <summary>
        /// Specifies the target frame
        /// </summary>
        public TargetFrame? TargetFrame
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Office + "target-frame");
                if (s == null) return null;

                switch (s)
                {
                    case "_self":
                        return Forms.TargetFrame.Self;
                    case "_blank":
                        return Forms.TargetFrame.Blank;
                    case "_parent":
                        return Forms.TargetFrame.Parent;
                    case "_top":
                        return Forms.TargetFrame.Top;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.TargetFrame.Self:
                        s = "_self";
                        break;
                    case Forms.TargetFrame.Blank:
                        s = "_blank";
                        break;
                    case Forms.TargetFrame.Parent:
                        s = "_parent";
                        break;
                    case Forms.TargetFrame.Top:
                        s = "_top";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Office + "target-frame", s);
            }
        }

        /// <summary>
        /// Specifies the URL that is loaded if a button is clicked
        /// </summary>
        public string TargetLocation
        {
            get { return (string) Node.Attribute(Ns.XLink + "href"); }
            set { Node.SetAttributeValue(Ns.XLink + "href", value); }
        }

        protected override void CreateBasicNode()
        {
            XElement xe = new XElement(Ns.Form + "button");
            Node.Add(xe);
            Node = xe;
            ControlImplementation = "ooo:com.sun.star.form.component.CommandButton";
        }
    }
}