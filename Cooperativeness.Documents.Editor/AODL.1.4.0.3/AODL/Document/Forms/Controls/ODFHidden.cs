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

    public class ODFHidden : ODFFormControl
    {
        /// <summary>
        /// Creates an ODFHidden
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFHidden(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
        }

        /// <summary>
        /// Creates an ODFHidden
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFHidden(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                         string width, string height) : base(parentForm, contentCollection, id, x, y, width, height)
        {
        }

        public ODFHidden(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
        }

        public override string Type
        {
            get { return "hidden"; }
        }

        /// <summary>
        /// Specifies the default value of the control
        /// </summary>
        public string Value
        {
            get { return (string) Node.Attribute(Ns.Form + "value"); }
            set { Node.SetAttributeValue(Ns.Form + "value", value); }
        }

        protected override void CreateBasicNode()
        {
            XElement xe = new XElement(Ns.Form + "hidden");
            Node.Add(xe);
            Node = xe;
            ControlImplementation = "ooo:com.sun.star.form.component.Hidden";
        }
    }
}