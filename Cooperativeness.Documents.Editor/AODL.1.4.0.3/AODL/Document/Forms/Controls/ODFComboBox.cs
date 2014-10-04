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
    public class ODFComboBox : ODFFormControl
    {
        private ODFItemCollection _items;

        /// <summary>
        /// Creates an ODFComboBox
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        public ODFComboBox(ODFForm parentForm, ContentCollection contentCollection, string id)
            : base(parentForm, contentCollection, id)
        {
            _items = new ODFItemCollection();
            RestoreItemEvents();
        }

        /// <summary>
        /// Creates an ODFComboBox
        /// </summary>
        /// <param name="parentForm">Parent form that the control belongs to</param>
        /// <param name="contentCollection">Collection of content where the control will be referenced</param>
        /// <param name="id">Control ID. Please specify a unique ID!</param>
        /// <param name="x">X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="y">Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="width">Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        /// <param name="height">Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)</param>
        public ODFComboBox(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                           string width, string height) : base(parentForm, contentCollection, id, x, y, width, height)
        {
            _items = new ODFItemCollection();
            RestoreItemEvents();
        }

        public ODFComboBox(ODFForm parentForm, XElement node) : base(parentForm, node)
        {
            _items = new ODFItemCollection();
            RestoreItemEvents();
        }

        /// <summary>
        /// Collection of combo box items
        /// </summary>
        public ODFItemCollection Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public override string Type
        {
            get { return "combobox"; }
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
        /// Specifies whether, when the user enters text in the 
        ///	combobox that matches one of the list items in the combobox, the application automatically 
        /// completes the text for the user.
        /// </summary>
        public bool? AutoComplete
        {
            get { return (bool?) Node.Attribute(Ns.Form + "auto-complete"); }
            set { Node.SetAttributeValue(Ns.Form + "auto-complete", value); }
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
        /// Specifies the source used to populate the list in a list box or 
        /// combo box. The first column of the list source result set populates the list.
        /// </summary>
        public string ListSource
        {
            get { return (string) Node.Attribute(Ns.Form + "list-source"); }
            set { Node.SetAttributeValue(Ns.Form + "list-source", value); }
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
        /// Specifies the type of data source that is used to 
        /// populate the list data in a list box or combo box
        /// </summary>
        public ListSourceType? ListSourceType
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "list-source-type");
                if (s == null) return null;

                switch (s)
                {
                    case "table":
                        return Forms.ListSourceType.Table;
                    case "query":
                        return Forms.ListSourceType.Query;
                    case "sql":
                        return Forms.ListSourceType.Sql;
                    case "sql-pass-through":
                        return Forms.ListSourceType.SqlPassThrough;
                    case "value-list":
                        return Forms.ListSourceType.ValueList;
                    case "table-fields":
                        return Forms.ListSourceType.TableFields;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.ListSourceType.Table:
                        s = "table";
                        break;
                    case Forms.ListSourceType.Query:
                        s = "query";
                        break;
                    case Forms.ListSourceType.Sql:
                        s = "sql";
                        break;
                    case Forms.ListSourceType.SqlPassThrough:
                        s = "sql-pass-through";
                        break;
                    case Forms.ListSourceType.ValueList:
                        s = "value-list";
                        break;
                    case Forms.ListSourceType.TableFields:
                        s = "table-fields";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "list-source-type", s);
            }
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
        /// Specifies whether or not a user can modify the value of a control
        /// </summary>
        public bool? ReadOnly
        {
            get { return (bool?) Node.Attribute(Ns.Form + "readonly"); }
            set { Node.SetAttributeValue(Ns.Form + "readonly", value); }
        }

        /// <summary>
        /// Specifies whether the list in a combo box or list box is always 
        /// visible or is only visible when the user clicks the drop-down button
        /// </summary>
        public bool? DropDown
        {
            get { return (bool?) Node.Attribute(Ns.Form + "dropdown"); }
            set { Node.SetAttributeValue(Ns.Form + "dropdown", value); }
        }

        /// <summary>
        /// specifies the number of rows that are visible at a time in a combo box 
        /// list or a list box list
        /// </summary>
        public int Size
        {
            get { return (int?) Node.Attribute(Ns.Form + "size") ?? -1; }
            set
            {
                if (value <= 0)
                    return;
                Node.SetAttributeValue(Ns.Form + "size", value);
            }
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
        /// Specifies the current status of an input control
        /// </summary>
        public string CurrentValue
        {
            get { return (string) Node.Attribute(Ns.Form + "current-value"); }
            set { Node.SetAttributeValue(Ns.Form + "current-value", value); }
        }

        /// <summary>
        /// Specifies the maximum number of characters that a user can 
        /// enter in an input control
        /// </summary>
        public int MaxLength
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
        /// Specifies the default value of the control
        /// </summary>
        public string Value
        {
            get { return (string) Node.Attribute(Ns.Form + "value"); }
            set { Node.SetAttributeValue(Ns.Form + "value", value); }
        }

        public void SuppressItemEvents()
        {
            _items.Inserted -= ItemCollection_Inserted;
            _items.Removed -= ItemCollection_Removed;
        }

        public void RestoreItemEvents()
        {
            _items.Inserted += ItemCollection_Inserted;
            _items.Removed += ItemCollection_Removed;
        }

        private void ItemCollection_Inserted(int index, object value)
        {
            ODFItem opt = (ODFItem) value;
            Node.Add(opt.Node);
        }

        private static void ItemCollection_Removed(int index, object value)
        {
            ODFItem opt = value as ODFItem;
            if (opt != null)
                opt.Node.Remove();
        }

        public void FixItemCollection()
        {
            _items.Clear();
            SuppressItemEvents();
            foreach (XElement nodeChild in Node.Elements())
            {
                if (nodeChild.Name == Ns.Form + "item" && nodeChild.Parent == Node)
                {
                    ODFItem sp = new ODFItem(_document, nodeChild);
                    _items.Add(sp);
                }
            }
            RestoreItemEvents();
        }

        /// <summary>
        /// Looks for a specified item by its label
        /// </summary>
        /// <param name="label">Option value</param>
        /// <returns></returns>
        public ODFItem GetItemByLabel(string label)
        {
            foreach (ODFItem it in _items)
            {
                if (it.Label == label)
                {
                    return it;
                }
            }
            return null;
        }

        protected override void CreateBasicNode()
        {
            XElement xe = new XElement(Ns.Form + "combobox");
            Node.Add(xe);
            Node = xe;
            ControlImplementation = "ooo:com.sun.star.form.component.ComboBox";
        }
    }
}