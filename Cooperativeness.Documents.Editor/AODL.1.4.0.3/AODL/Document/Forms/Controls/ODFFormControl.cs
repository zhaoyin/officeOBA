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

using System;
using System.Xml.Linq;
using AODL.Document.Content;
using AODL.Document.Styles;

namespace AODL.Document.Forms.Controls
{
    /// <summary>
    /// Summary description for IODFFormControl.
    /// </summary>
    public abstract class ODFFormControl
    {
        private ContentCollection _contentCollection;
        private ODFControlRef _controlRef;
        protected IDocument _document;
        private int _refCounter;

        protected ODFFormControl(ODFForm parentForm, XElement node)
        {
            _document = parentForm.Document;
            ParentForm = parentForm;
            Node = node;

            _contentCollection = null;
            _controlRef = null;

            Properties = new FormPropertyCollection();
            Properties.Inserted += PropertyCollection_Inserted;
            Properties.Removed += PropertyCollection_Removed;
        }

        protected ODFFormControl(ODFForm parentForm, ContentCollection contentCollection, string id)
        {
            _document = parentForm.Document;
            ParentForm = parentForm;
            Node = parentForm.Node;
            _contentCollection = contentCollection;
            CreateBasicNode();
            Id = id;
            ControlImplementation = "ooo:com.sun.star.form.component.TextField";
            _controlRef = new ODFControlRef(_document, id);

            Properties = new FormPropertyCollection();
            Properties.Inserted += PropertyCollection_Inserted;
            Properties.Removed += PropertyCollection_Removed;
        }

        protected ODFFormControl(ODFForm parentForm, ContentCollection contentCollection, string id, string x, string y,
                                 string width, string height)
            //: base (document, ContentCollection, id, x, y, width, height)
        {
            _document = parentForm.Document;
            ParentForm = parentForm;
            Node = parentForm.Node;
            _contentCollection = contentCollection;
            CreateBasicNode();
            Id = id;
            _controlRef = new ODFControlRef(_document, id, x, y, width, height);

            Properties = new FormPropertyCollection();
            Properties.Inserted += PropertyCollection_Inserted;
            Properties.Removed += PropertyCollection_Removed;
        }

        public ContentCollection ContentCollection
        {
            get { return _contentCollection; }
            set { _contentCollection = value; }
        }

        public ODFControlRef ControlRef
        {
            get { return _controlRef; }
            set { _controlRef = value; }
        }


        /// <summary>
        /// Collection of generic form properties (form:property in ODF)
        /// </summary>
        public FormPropertyCollection Properties { get; set; }

        /// <summary>
        /// Parent form
        /// </summary>
        public ODFForm ParentForm { get; set; }

        /// <summary>
        /// XML node that represents the control
        /// </summary>
        public XElement Node { get; set; }

        /// <summary>
        /// Control implementation. Don't change it unless required
        /// </summary>
        public string ControlImplementation
        {
            get { return (string) Node.Attribute(Ns.Form + "control-implementation"); }
            set { Node.SetAttributeValue(Ns.Form + "control-implementation", value); }
        }

        /// <summary>
        /// The name of the control
        /// </summary>
        public string Name
        {
            get { return (string) Node.Attribute(Ns.Form + "name"); }
            set { Node.SetAttributeValue(Ns.Form + "name", value); }
        }

        /// <summary>
        /// Control ID
        /// </summary>
        public string Id
        {
            get { return (string) Node.Attribute(Ns.Form + "id"); }
            set
            {
                Node.SetAttributeValue(Ns.Form + "id", value);
                if (_controlRef != null) _controlRef.DrawControl = value;
            }
        }

        /// <summary>
        /// Returns the type of the control
        /// </summary>
        public abstract string Type { get; }

        protected virtual void CreateBasicNode()
        {
        }

        public virtual void AddToContentCollection()
        {
            if (_contentCollection != null)
            {
                if (_refCounter == 0)
                {
                    _contentCollection.Add(_controlRef);
                    _refCounter++;
                }
                else
                {
                    throw new Exception("Cannot add control to form: it already belongs to another form!");
                }
            }
        }

        public virtual void RemoveFromContentCollection()
        {
            if (_contentCollection != null && _refCounter > 0)
            {
                _contentCollection.Remove(_controlRef);
                _refCounter--;
            }
        }

        public void SuppressPropertyEvents()
        {
            Properties.Inserted -= PropertyCollection_Inserted;
            Properties.Removed -= PropertyCollection_Removed;
        }

        public void RestorePropertyEvents()
        {
            Properties.Inserted += PropertyCollection_Inserted;
            Properties.Removed += PropertyCollection_Removed;
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

            if (formProp == null)
                return;
            FormProperty prop = (FormProperty) value;
            prop.Node.Remove();
            if (index == 0)
            {
                formProp.Remove();
            }
        }

        /// <summary>
        /// Look for a control generic property by its name
        /// </summary>
        /// <param name="name">Property name</param>
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
                    SingleFormProperty sp = new SingleFormProperty(_document, nodeChild);
                    Properties.Add(sp);
                }
                if (nodeChild.Name == Ns.Form + "list-property" && nodeChild.Parent == formProp)
                {
                    ListFormProperty lp = new ListFormProperty(_document, nodeChild);
                    Properties.Add(lp);
                }
            }
            RestorePropertyEvents();
        }

        #region ODFControlRef properties

        /// <summary>
        /// X coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)
        /// </summary>
        public string X
        {
            get { return _controlRef.X; }
            set { _controlRef.X = value; }
        }

        /// <summary>
        /// Y coordinate of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)
        /// </summary>
        public string Y
        {
            get { return _controlRef.Y; }
            set { _controlRef.Y = value; }
        }

        /// <summary>
        /// Width of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)
        /// </summary>
        public string Width
        {
            get { return _controlRef.Width; }
            set { _controlRef.Width = value; }
        }

        /// <summary>
        /// Height of the control in ODF format (eg. "1cm", "15mm", 3.2cm" etc)
        /// </summary>
        public string Height
        {
            get { return _controlRef.Height; }
            set { _controlRef.Height = value; }
        }

        /// <summary>
        /// Control's Z index
        /// </summary>
        public int ZIndex
        {
            get { return _controlRef.ZIndex; }
            set { _controlRef.ZIndex = value; }
        }


        /// <summary>
        /// A Style class which is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        public IStyle Style
        {
            get { return _controlRef.Style; }
            set { _controlRef.Style = value; }
        }

        /// <summary>
        /// Style name
        /// </summary>
        public string StyleName
        {
            get { return _controlRef.StyleName; }
            set { _controlRef.StyleName = value; }
        }

        /// <summary>
        /// Text style name
        /// </summary>
        public string TextStyleName
        {
            get { return _controlRef.TextStyleName; }
            set { _controlRef.TextStyleName = value; }
        }

        /// <summary>
        /// A text style.
        /// If no style is available this is null.
        /// </summary>
        public IStyle TextStyle
        {
            get { return _controlRef.TextStyle; }
            set { _controlRef.TextStyle = value; }
        }

        /// <summary>
        /// Number of page to use as an anchor
        /// </summary>
        public int AnchorPageNumber
        {
            get { return _controlRef.AnchorPageNumber; }
            set { _controlRef.AnchorPageNumber = value; }
        }

        /// <summary>
        /// What to use as an anchor
        /// </summary>
        public AnchorType? AnchorType
        {
            get { return _controlRef.AnchorType; }
            set { _controlRef.AnchorType = value; }
        }

        /// <summary>
        /// Table background
        /// </summary>
        public bool? TableBackground
        {
            get { return _controlRef.TableBackground; }
            set { _controlRef.TableBackground = value; }
        }

        /// <summary>
        /// Transform. For the details, see ODF v1.0 specification
        /// </summary>
        public string Transform
        {
            get { return _controlRef.Transform; }
            set { _controlRef.Transform = value; }
        }

        ~ODFFormControl()
        {
            RemoveFromContentCollection();
        }

        #endregion
    }
}