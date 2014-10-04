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
    public class ODFImageFrame : ODFFormControl
    {
        /// <summary>
        /// Creates an ODFImageFrame
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFImageFrame(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
        }

        /// <summary>
        /// Creates an ODFImageFrame
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFImageFrame(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                             string width, string height) : base(parentForm, contentCollection, id, x, y, width, height)
        {
        }

        public ODFImageFrame(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
        }

        public override string Type
        {
            get { return "image-frame"; }
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
        /// Links the control to an external file containing image data
        /// </summary>
        public string ImageData
        {
            get
            {
                string path = (string) Node.Attribute(Ns.Form + "image-data");
                if (path == null) return null;
                if (path[0] == '/')
                    path = path.Substring(1);
                path.Replace(@"/", @"\");
                return path;
            }
            set
            {
                string path = value;
                if (value[0] != '/')
                {
                    path = "/" + value;
                }
                path = path.Replace(@"\", @"/");
                Node.SetAttributeValue(Ns.Form + "image-data", path);
            }
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
        /// Specifies whether or not a user can modify the value of a control
        /// </summary>
        public bool? ReadOnly
        {
            get { return (bool?) Node.Attribute(Ns.Form + "readonly"); }
            set { Node.SetAttributeValue(Ns.Form + "readonly", value); }
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
            XElement xe = new XElement(Ns.Form + "image-frame");
            Node.Add(xe);
            Node = xe;
            ControlImplementation = "ooo:com.sun.star.form.component.DatabaseImageControl";
        }
    }
}