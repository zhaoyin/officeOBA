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
    public class ODFRadioButton : ODFFormControl
    {
        /// <summary>
        /// Creates an ODFRadioButton
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFRadioButton(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
        }

        /// <summary>
        /// Creates an ODFRadioButton
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFRadioButton(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                              string width, string height)
            : base(parentForm, contentCollection, id, x, y, width, height)
        {
        }

        public ODFRadioButton(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
        }

        public override string Type
        {
            get { return "radio"; }
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
        /// Specifies the name of a result set column. The result set is 
        /// determined by the form which the control belongs to
        /// </summary>
        public string DataField
        {
            get { return (string) Node.Attribute(Ns.Form + "data-field"); }
            set { Node.SetAttributeValue(Ns.Form + "data-field", value); }
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
        /// Specifies whether or not a control is printed when a user prints 
        /// the document in which the control is contained
        /// </summary>
        public bool? Printable
        {
            get { return (bool?) Node.Attribute(Ns.Form + "printable"); }
            set { Node.SetAttributeValue(Ns.Form + "printable", value); }
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
        /// Specifies if the radio button is currently selected
        /// </summary>
        public bool? CurrentSelected
        {
            get { return (bool?) Node.Attribute(Ns.Form + "current-selected"); }
            set { Node.SetAttributeValue(Ns.Form + "current-selected", value); }
        }

        /// <summary>
        /// Specifies a visual affect to apply to a control
        /// </summary>
        public VisualEffect? VisualEffect
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "visual-effect");
                if (s == null) return null;

                switch (s)
                {
                    case "flat":
                        return Forms.VisualEffect.Flat;
                    case "3d":
                        return Forms.VisualEffect.ThreeD;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.VisualEffect.Flat:
                        s = "flat";
                        break;
                    case Forms.VisualEffect.ThreeD:
                        s = "3d";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "visual-effect", s);
            }
        }

        /// <summary>
        /// Specifies the position of an image to be displayed in a form control, relative to the label text
        /// </summary>
        public ImagePosition? ImagePosition
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "image-position");
                if (s == null) return null;

                switch (s)
                {
                    case "start":
                        return Forms.ImagePosition.Start;
                    case "end":
                        return Forms.ImagePosition.End;
                    case "top":
                        return Forms.ImagePosition.Top;
                    case "bottom":
                        return Forms.ImagePosition.Bottom;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.ImagePosition.Start:
                        s = "start";
                        break;
                    case Forms.ImagePosition.End:
                        s = "end";
                        break;
                    case Forms.ImagePosition.Top:
                        s = "top";
                        break;
                    case Forms.ImagePosition.Bottom:
                        s = "bottom";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "image-position", s);
            }
        }


        /// <summary>
        /// Specifies the position of an image to be displayed in a form control, relative to the label text
        /// </summary>
        public ImageAlign? ImageAlign
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "image-align");
                if (s == null) return null;

                switch (s)
                {
                    case "start":
                        return Forms.ImageAlign.Start;
                    case "end":
                        return Forms.ImageAlign.End;
                    case "center":
                        return Forms.ImageAlign.Center;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.ImageAlign.Start:
                        s = "start";
                        break;
                    case Forms.ImageAlign.End:
                        s = "end";
                        break;
                    case Forms.ImageAlign.Center:
                        s = "center";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "image-align", s);
            }
        }


        protected override void CreateBasicNode()
        {
            XElement xe = new XElement(Ns.Form + "radio");
            Node.Add(xe);
            Node = xe;
            ControlImplementation = "ooo:com.sun.star.form.component.RadioButton";
        }
    }
}