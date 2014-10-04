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
    public class ODFGrid : ODFFormControl
    {
        /// <summary>
        /// Creates an ODFGrid
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFGrid(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
            Columns = new ODFGridColumnCollection();
            RestoreColumnEvents();
        }

        /// <summary>
        /// Creates an ODFGrid
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFGrid(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                       string width, string height) : base(parentForm, contentCollection, id, x, y, width, height)
        {
            Columns = new ODFGridColumnCollection();
            RestoreColumnEvents();
        }

        public ODFGrid(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
            Columns = new ODFGridColumnCollection();
            RestoreColumnEvents();
        }

        /// <summary>
        /// The collection of the ODFGridColumns (each column stand for a list box element)
        /// </summary>
        public ODFGridColumnCollection Columns { get; set; }

        public override string Type
        {
            get { return "grid"; }
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

        public void SuppressColumnEvents()
        {
            Columns.Inserted -= ColumnCollection_Inserted;
            Columns.Removed -= ColumnCollection_Removed;
        }

        public void RestoreColumnEvents()
        {
            Columns.Inserted += ColumnCollection_Inserted;
            Columns.Removed += ColumnCollection_Removed;
        }

        private void ColumnCollection_Inserted(int index, object value)
        {
            ODFGridColumn col = (ODFGridColumn) value;
            Node.Add(col.Node);
        }

        private static void ColumnCollection_Removed(int index, object value)
        {
            ODFGridColumn col = value as ODFGridColumn;
            if (col != null)
                col.Node.Remove();
        }

        /// <summary>
        /// Looks for a specified column by its value
        /// </summary>
        /// <param name="name">column value</param>
        /// <returns></returns>
        public ODFGridColumn GetColumnByName(string name)
        {
            foreach (ODFGridColumn col in Columns)
            {
                if (col.Name == name)
                {
                    return col;
                }
            }
            return null;
        }

        public void FixColumnCollection()
        {
            Columns.Clear();
            SuppressColumnEvents();
            foreach (XElement nodeChild in Node.Elements())
            {
                if (nodeChild.Name == Ns.Form + "column" && nodeChild.Parent == Node)
                {
                    ODFGridColumn sp = new ODFGridColumn(_document, nodeChild);
                    Columns.Add(sp);
                }
            }
            RestoreColumnEvents();
        }


        protected override void CreateBasicNode()
        {
            XElement xe = new XElement(Ns.Form + "grid");
            Node.Add(xe);
            Node = xe;
            ControlImplementation = "ooo:com.sun.star.form.component.GridControl";
        }
    }
}