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
using AODL.Document.Forms.Controls;

namespace AODL.Document.Forms
{
    /// <summary>
    /// Summary description for ODFForm.
    /// </summary>
    public class ODFForm
    {
        private ODFFormCollection _formCollection;

        /// <summary>
        /// Creates an ODFForm
        /// </summary>
        /// <param name="document">Parent document</param>
        /// <param name="name">Form name</param>
        public ODFForm(IDocument document, string name)
        {
            Document = document;
            CreateBasicNode();
            ControlImplementation = "ooo:com.sun.star.form.component.Form";
            ApplyFilter = true;
            CommandType = Forms.CommandType.Table;
            Name = name;

            Controls = new ODFControlsCollection();
            Controls.Inserted += ControlsCollection_Inserted;
            Controls.Removed += ControlsCollection_Removed;
            Controls.Clearing += ControlsCollection_Clearing;

            Properties = new FormPropertyCollection();
            Properties.Inserted += PropertyCollection_Inserted;
            Properties.Removed += PropertyCollection_Removed;

            _formCollection = new ODFFormCollection();
            _formCollection.Inserted += FormCollection_Inserted;
            _formCollection.Removed += FormCollection_Removed;
        }

        public ODFForm(XElement node, IDocument document)
        {
            Document = document;
            Node = node;

            Controls = new ODFControlsCollection();
            Controls.Inserted += ControlsCollection_Inserted;
            Controls.Removed += ControlsCollection_Removed;
            Controls.Clearing += ControlsCollection_Clearing;

            Properties = new FormPropertyCollection();
            Properties.Inserted += PropertyCollection_Inserted;
            Properties.Removed += PropertyCollection_Removed;

            _formCollection = new ODFFormCollection();
            _formCollection.Inserted += FormCollection_Inserted;
            _formCollection.Removed += FormCollection_Removed;
        }

        /// <summary>
        /// Represents the IRI of the processing agent for the form
        /// </summary>
        public string Href
        {
            get { return (string) Node.Attribute(Ns.XLink + "href"); }
            set { Node.SetAttributeValue(Ns.XLink + "href", value); }
        }

        /// <summary>
        /// Do not change it unless it is necessary
        /// </summary>
        public string ControlImplementation
        {
            get { return (string) Node.Attribute(Ns.Form + "control-implementation"); }
            set { Node.SetAttributeValue(Ns.Form + "control-implementation", value); }
        }

        /// <summary>
        /// Form name
        /// </summary>
        public string Name
        {
            get { return (string) Node.Attribute(Ns.Form + "name"); }
            set { Node.SetAttributeValue(Ns.Form + "name", value); }
        }

        /// <summary>
        /// Specifies the target frame of the form
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
        /// Specifies the HTTP method to use to submit the data in the form to
        /// the server
        /// </summary>
        public Method? Method
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "method");
                if (s == null) return null;

                switch (s)
                {
                    case "get":
                        return Forms.Method.Get;
                    case "post":
                        return Forms.Method.Post;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.Method.Get:
                        s = "get";
                        break;
                    case Forms.Method.Post:
                        s = "post";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "method", s);
            }
        }

        /// <summary>
        ///  Specifies the content type used to submit the form to the server
        /// </summary>
        public string Enctype
        {
            get { return (string) Node.Attribute(Ns.Form + "enctype"); }
            set { Node.SetAttributeValue(Ns.Form + "enctype", value); }
        }

        /// <summary>
        /// Specifies whether or not data records can be deleted
        /// </summary>
        public bool? AllowDeletes
        {
            get { return (bool?) Node.Attribute(Ns.Form + "allow-deletes"); }
            set { Node.SetAttributeValue(Ns.Form + "allow-deletes", value); }
        }

        /// <summary>
        /// Specifies whether or not new data records can be inserted
        /// </summary>
        public bool? AllowInserts
        {
            get { return (bool?) Node.Attribute(Ns.Form + "allow-inserts"); }
            set { Node.SetAttributeValue(Ns.Form + "allow-inserts", value); }
        }

        /// <summary>
        /// Specifies whether or not data records can be updated
        /// </summary>
        public bool? AllowUpdates
        {
            get { return (bool?) Node.Attribute(Ns.Form + "allow-updates"); }
            set { Node.SetAttributeValue(Ns.Form + "allow-updates", value); }
        }

        /// <summary>
        /// Specifies whether or not filters should be applied to the form
        /// </summary>
        public bool? ApplyFilter
        {
            get { return (bool?) Node.Attribute(Ns.Form + "apply-filter"); }
            set { Node.SetAttributeValue(Ns.Form + "apply-filter", value); }
        }


        /// <summary>
        /// Specifies the type of command to execute on the data source
        /// </summary>
        public CommandType? CommandType
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "command-type");
                if (s == null) return null;

                switch (s)
                {
                    case "table":
                        return Forms.CommandType.Table;
                    case "query":
                        return Forms.CommandType.Query;
                    case "command":
                        return Forms.CommandType.Command;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.CommandType.Table:
                        s = "table";
                        break;
                    case Forms.CommandType.Query:
                        s = "query";
                        break;
                    case Forms.CommandType.Command:
                        s = "command";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "command-type", s);
            }
        }

        /// <summary>
        /// Specifies the command to execute on the data source
        /// </summary>
        public string Command
        {
            get { return (string) Node.Attribute(Ns.Form + "command"); }
            set { Node.SetAttributeValue(Ns.Form + "command", value); }
        }

        /// <summary>
        /// Specifies the name of a data source to use for the form
        /// </summary>
        public string DataSource
        {
            get { return (string) Node.Attribute(Ns.Form + "datasource"); }
            set { Node.SetAttributeValue(Ns.Form + "datasource", value); }
        }

        /// <summary>
        /// Specifies the names of the columns in the result set represented by the parent form
        /// </summary>
        public string MasterFields
        {
            get { return (string) Node.Attribute(Ns.Form + "master-fields"); }
            set { Node.SetAttributeValue(Ns.Form + "master-fields", value); }
        }

        /// <summary>
        /// Specifies the names of the columns in detail forms that are related to columns in the parent form
        /// </summary>
        public string DetailFields
        {
            get { return (string) Node.Attribute(Ns.Form + "detail-fields"); }
            set { Node.SetAttributeValue(Ns.Form + "detail-fields", value); }
        }

        /// <summary>
        /// Specifies whether or not the application processes the command before passing it to the
        /// database driver
        /// </summary>
        public bool? EscapeProcessing
        {
            get { return (bool?) Node.Attribute(Ns.Form + "escape-processing"); }
            set { Node.SetAttributeValue(Ns.Form + "escape-processing", value); }
        }

        /// <summary>
        /// Specifies a filter for the command to base the form on
        /// </summary>
        public string Filter
        {
            get { return (string) Node.Attribute(Ns.Form + "filter"); }
            set { Node.SetAttributeValue(Ns.Form + "filter", value); }
        }

        /// <summary>
        /// Specifies how the records in a database form are navigated
        /// </summary>
        public NavigationMode? NavigationMode
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "navigation-mode");
                if (s == null) return null;

                switch (s)
                {
                    case "current":
                        return Forms.NavigationMode.Current;
                    case "parent":
                        return Forms.NavigationMode.Parent;
                    case "none":
                        return Forms.NavigationMode.None;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.NavigationMode.Current:
                        s = "current";
                        break;
                    case Forms.NavigationMode.Parent:
                        s = "parent";
                        break;
                    case Forms.NavigationMode.None:
                        s = "none";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "navigation-mode", s);
            }
        }


        /// <summary>
        /// Specifies whether or not to discard all results that are
        /// retrieved from the underlying data source
        /// </summary>
        public bool? IgnoreResult
        {
            get { return (bool?) Node.Attribute(Ns.Form + "ignore-result"); }
            set { Node.SetAttributeValue(Ns.Form + "ignore-result", value); }
        }


        /// <summary>
        /// Specifies a sort criteria for the command.
        /// </summary>
        public string Order
        {
            get { return (string) Node.Attribute(Ns.Form + "order"); }
            set { Node.SetAttributeValue(Ns.Form + "order", value); }
        }

        /// <summary>
        /// Specifies how the application responds when the user presses
        /// the TAB key in the controls in a for
        /// </summary>
        public TabCycle? TabCycle
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Form + "tab-cycle");
                if (s == null) return null;
                switch (s)
                {
                    case "current":
                        return Forms.TabCycle.Current;
                    case "records":
                        return Forms.TabCycle.Records;
                    case "page":
                        return Forms.TabCycle.Page;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Forms.TabCycle.Current:
                        s = "current";
                        break;
                    case Forms.TabCycle.Records:
                        s = "records";
                        break;
                    case Forms.TabCycle.Page:
                        s = "page";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Form + "tab-cycle", s);
            }
        }

        /// <summary>
        /// Specifies the source database by an [XLink].
        /// </summary>
        public string ConnectionResource
        {
            get { return (string) Node.Attribute(Ns.Form + "connection-resource"); }
            set { Node.SetAttributeValue(Ns.Form + "connection-resource", value); }
        }


        /// <summary>
        /// The XML node that represents the form and its content
        /// </summary>
        public XElement Node { get; set; }

        /// <summary>
        /// Parent document
        /// </summary>
        public IDocument Document { get; set; }

        /// <summary>
        /// List of the child controls
        /// </summary>
        public ODFControlsCollection Controls { get; set; }

        /// <summary>
        /// Generic form:property collection
        /// </summary>
        public FormPropertyCollection Properties { get; set; }

        /// <summary>
        /// Child forms collection
        /// </summary>
        public ODFFormCollection ChildForms
        {
            get { return _formCollection; }
            set { _formCollection = value; }
        }

        private void CreateBasicNode()
        {
            Node = new XElement(Ns.Form + "form");
        }


        public void SuppressControlEvents()
        {
            Controls.Inserted -= ControlsCollection_Inserted;
            Controls.Removed -= ControlsCollection_Removed;
            Controls.Clearing -= ControlsCollection_Clearing;
        }

        public void RestoreControlEvents()
        {
            Controls.Inserted += ControlsCollection_Inserted;
            Controls.Removed += ControlsCollection_Removed;
            Controls.Clearing += ControlsCollection_Clearing;
        }

        public void SuppressPropertyEvents()
        {
            Properties.Inserted -= PropertyCollection_Inserted;
            Properties.Removed -= PropertyCollection_Removed;
        }

        private void RestorePropertyEvents()
        {
            Properties.Inserted += PropertyCollection_Inserted;
            Properties.Removed += PropertyCollection_Removed;
        }

        private void ControlsCollection_Inserted(int index, object value)
        {
            ODFFormControl ctrl = (ODFFormControl) value;
            Node.Add(ctrl.Node);

            ctrl.AddToContentCollection();
        }

        private static void ControlsCollection_Removed(int index, object value)
        {
            ODFFormControl ctrl = (ODFFormControl) value;
            ctrl.Node.Remove();

            ctrl.RemoveFromContentCollection();
        }

        private void ControlsCollection_Clearing()
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                ODFFormControl ctrl = Controls[i];
                if (ctrl != null)
                {
                    ctrl.Node.Remove();
                    ctrl.RemoveFromContentCollection();
                }
            }
        }

        private void PropertyCollection_Inserted(int index, object value)
        {
            XElement formProp = Node.Element(Ns.Form + "properties");

            if (formProp == null)
            {
                formProp = new XElement(Ns.Form + "properties");
                Node.Add(formProp);
            }

            FormProperty prop = (FormProperty) value;
            formProp.Add(prop.Node);
        }

        private void PropertyCollection_Removed(int index, object value)
        {
            XElement formProp = Node.Element(Ns.Form + "properties");

            if (formProp != null)
            {
                FormProperty prop = (FormProperty) value;
                prop.Node.Remove();
                if (index == 0)
                {
                    formProp.Remove();
                }
            }
        }

        private void FormCollection_Inserted(int index, object value)
        {
            ODFForm child = (value as ODFForm);
            if (child != null)
                Node.Add(child.Node);
        }

        private static void FormCollection_Removed(int index, object value)
        {
            ODFForm child = (value as ODFForm);
            if (child != null)
            {
                child.Controls.Clear();
                if (child.Node != null)
                    child.Node.Remove();
            }
        }

        /// <summary>
        /// Looks up a control by its ID
        /// </summary>
        /// <param name="id">Control ID</param>
        /// <param name="searchInSubforms">Specifies whether to look in the subforms</param>
        /// <returns></returns>
        public ODFFormControl FindControlById(string id, bool searchInSubforms)
        {
            if (searchInSubforms)
            {
                foreach (ODFForm f in ChildForms)
                {
                    ODFFormControl ctrl = f.FindControlById(id, true);
                    if (ctrl != null) return ctrl;
                }
            }
            foreach (ODFFormControl c in Controls)
            {
                if (c != null)
                {
                    if (c.Id == id)
                        return c;
                }
            }
            return null;
        }

        /// <summary>
        /// Looks up a control by its name
        /// </summary>
        /// <param name="name">Control name</param>
        /// <param name="searchInSubforms">Specifies whether to look in the subforms</param>
        /// <returns></returns>
        public ODFFormControl FindControlByName(string name, bool searchInSubforms)
        {
            if (searchInSubforms)
            {
                foreach (ODFForm f in ChildForms)
                {
                    ODFFormControl ctrl = f.FindControlByName(name, true);
                    if (ctrl != null) return ctrl;
                }
            }
            foreach (ODFFormControl c in Controls)
            {
                if (c != null)
                {
                    if (c.Name == name)
                        return c;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the generic form property by its name
        /// </summary>
        /// <param name="name">Generic form property</param>
        /// <returns></returns>
        public FormProperty GetFormProperty(string name)
        {
            foreach (FormProperty fp in Properties)
            {
                if (fp.Name == name)
                {
                    return fp;
                }
            }
            return null;
        }

        public void FixPropertyCollection()
        {
            Properties.Clear();
            SuppressPropertyEvents();
            XElement formProp = Node.Element(Ns.Form + "properties");
            if (formProp == null) return;

            foreach (XElement nodeChild in formProp.Elements())
            {
                if (nodeChild.Name == Ns.Form + "property" && nodeChild.Parent == formProp)
                {
                    SingleFormProperty sp = new SingleFormProperty(Document, nodeChild);
                    Properties.Add(sp);
                }
                if (nodeChild.Name == Ns.Form + "list-property" && nodeChild.Parent == formProp)
                {
                    ListFormProperty lp = new ListFormProperty(Document, nodeChild);
                    Properties.Add(lp);
                }
            }
            RestorePropertyEvents();
        }
    }
}