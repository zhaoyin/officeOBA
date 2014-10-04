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

namespace AODL.Document.Forms
{

    #region FormProperty abstract class

    public abstract class FormProperty
    {
        protected IDocument _document;
        protected XElement _node;

        /// <summary>
        /// The XML node representing the property
        /// </summary>
        public XElement Node
        {
            get { return _node; }
            set { _node = value; }
        }

        /// <summary>
        /// The document that contains the form
        /// </summary>
        public IDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }

        /// <summary>
        /// Property name
        /// </summary>
        public abstract string Name { get; set; }

        public abstract PropertyValueType? PropertyValueType { get; set; }
    }

    #endregion

    #region SingleFormProperty class

    public class SingleFormProperty : FormProperty
    {
        protected object _value;

        /// <summary>
        /// Creates the SingleFormProperty
        /// </summary>
        /// <param name="document">Document containing the form</param>
        /// <param name="propValueType">Type of the property value</param>
        /// <param name="propName">Property name</param>
        /// <param name="propValue">Property value</param>
        public SingleFormProperty(IDocument document, PropertyValueType propValueType, string propName, string propValue)
        {
            Document = document;
            Node = new XElement(Ns.Form + "property");
            PropertyValueType = propValueType;
            Name = propName;
            Value = propValue;
        }

        /// <summary>
        /// Creates the SingleFormProperty
        /// </summary>
        /// <param name="document">Document containing the form</param>
        /// <param name="propValueType">Type of the property value</param>
        public SingleFormProperty(IDocument document, PropertyValueType propValueType)
        {
            Document = document;
            Node = new XElement(Ns.Form + "property");
            PropertyValueType = propValueType;
        }

        public SingleFormProperty(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        public override PropertyValueType? PropertyValueType
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Office + "value-type");

                if (s == null) return null;

                switch (s)
                {
                    case "float":
                        return Forms.PropertyValueType.Float;
                    case "percentage":
                        return Forms.PropertyValueType.Percentage;
                    case "currency":
                        return Forms.PropertyValueType.Currency;
                    case "date":
                        return Forms.PropertyValueType.Date;
                    case "time":
                        return Forms.PropertyValueType.Time;
                    case "boolean":
                        return Forms.PropertyValueType.Boolean;
                    case "string":
                        return Forms.PropertyValueType.String;
                    default:
                        return Forms.PropertyValueType.String;
                }
            }
            set
            {
                string s = "";
                switch (value)
                {
                    case Forms.PropertyValueType.Float:
                        s = "float";
                        break;
                    case Forms.PropertyValueType.Percentage:
                        s = "percentage";
                        break;
                    case Forms.PropertyValueType.Currency:
                        s = "currency";
                        break;
                    case Forms.PropertyValueType.Date:
                        s = "date";
                        break;
                    case Forms.PropertyValueType.Time:
                        s = "time";
                        break;
                    case Forms.PropertyValueType.Boolean:
                        s = "boolean";
                        break;
                    case Forms.PropertyValueType.String:
                        s = "string";
                        break;
                }

                Node.SetAttributeValue(Ns.Office + "value-type", s);
            }
        }

        /// <summary>
        /// Property value
        /// </summary>
        public string Value
        {
            get
            {
                string s = "";
                switch (PropertyValueType)
                {
                    case Forms.PropertyValueType.Float:
                        s = "value";
                        break;
                    case Forms.PropertyValueType.Percentage:
                        s = "value";
                        break;
                    case Forms.PropertyValueType.Currency:
                        s = "value";
                        break;
                    case Forms.PropertyValueType.Date:
                        s = "date-value";
                        break;
                    case Forms.PropertyValueType.Time:
                        s = "time-value";
                        break;
                    case Forms.PropertyValueType.Boolean:
                        s = "boolean-value";
                        break;
                    case Forms.PropertyValueType.String:
                        s = "string-value";
                        break;
                }
                return (string) Node.Attribute(Ns.Office + s);
            }
            set
            {
                string s = "";
                switch (PropertyValueType)
                {
                    case Forms.PropertyValueType.Float:
                        s = "value";
                        break;
                    case Forms.PropertyValueType.Percentage:
                        s = "value";
                        break;
                    case Forms.PropertyValueType.Currency:
                        s = "value";
                        break;
                    case Forms.PropertyValueType.Date:
                        s = "date-value";
                        break;
                    case Forms.PropertyValueType.Time:
                        s = "time-value";
                        break;
                    case Forms.PropertyValueType.Boolean:
                        s = "boolean-value";
                        break;
                    case Forms.PropertyValueType.String:
                        s = "string-value";
                        break;
                }

                Node.SetAttributeValue(Ns.Office + s, value);
            }
        }

        /// <summary>
        /// Property name
        /// </summary>
        public override string Name
        {
            get { return (string) Node.Attribute(Ns.Form + "property-name"); }
            set { Node.SetAttributeValue(Ns.Form + "property-name", value); }
        }
    }

    #endregion

    #region ListFormPropertyElem class

    public class ListFormPropertyElement
    {
        protected IDocument _document;
        protected XElement _node;
        protected PropertyValueType? _propertyValueType;
        protected object _value;

        /// <summary>
        /// Creates the ListFormPropertyElement
        /// </summary>
        /// <param name="property">Property containing this element</param>
        /// <param name="propValue">Element value</param>
        public ListFormPropertyElement(FormProperty property, string propValue)
        {
            Document = property.Document;
            Node = new XElement(Ns.Form + "list-value");
            _propertyValueType = property.PropertyValueType;
            Value = propValue;
        }

        public ListFormPropertyElement(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        /// <summary>
        /// Creates the ListFormPropertyElement
        /// </summary>
        /// <param name="property">Property containing this element</param>
        public ListFormPropertyElement(FormProperty property)
        {
            Document = property.Document;
            Node = new XElement(Ns.Form + "list-value");
            _propertyValueType = property.PropertyValueType;
        }

        /// <summary>
        /// XML node representing the ListFormProperty element
        /// </summary>
        public XElement Node
        {
            get { return _node; }
            set { _node = value; }
        }

        /// <summary>
        /// Main document
        /// </summary>
        public IDocument Document
        {
            get { return _document; }
            set { _document = value; }
        }

        /// <summary>
        /// Element value
        /// </summary>
        public string Value
        {
            get
            {
                string s = "";
                switch (_propertyValueType)
                {
                    case PropertyValueType.Float:
                        s = "value";
                        break;
                    case PropertyValueType.Percentage:
                        s = "value";
                        break;
                    case PropertyValueType.Currency:
                        s = "value";
                        break;
                    case PropertyValueType.Date:
                        s = "date-value";
                        break;
                    case PropertyValueType.Time:
                        s = "time-value";
                        break;
                    case PropertyValueType.Boolean:
                        s = "boolean-value";
                        break;
                    case PropertyValueType.String:
                        s = "string-value";
                        break;
                }
                return (string) Node.Attribute(Ns.Office + s);
            }
            set
            {
                string s = "";
                switch (_propertyValueType)
                {
                    case PropertyValueType.Float:
                        s = "value";
                        break;
                    case PropertyValueType.Percentage:
                        s = "value";
                        break;
                    case PropertyValueType.Currency:
                        s = "value";
                        break;
                    case PropertyValueType.Date:
                        s = "date-value";
                        break;
                    case PropertyValueType.Time:
                        s = "time-value";
                        break;
                    case PropertyValueType.Boolean:
                        s = "boolean-value";
                        break;
                    case PropertyValueType.String:
                        s = "string-value";
                        break;
                }
                Node.SetAttributeValue(Ns.Office + s, value);
            }
        }
    }

    #endregion

    #region ListFormProperty class

    public class ListFormProperty : FormProperty
    {
        protected ListFormPropertyElemCollection _propertyValues;

        /// <summary>
        /// Creates the ListFormProperty
        /// </summary>
        /// <param name="document">Main document</param>
        /// <param name="propValueType">Property value type</param>
        public ListFormProperty(IDocument document, PropertyValueType propValueType)
        {
            Document = document;
            Node = new XElement(Ns.Form + "list-property");
            PropertyValueType = propValueType;

            _propertyValues = new ListFormPropertyElemCollection();
            _propertyValues.Inserted += PropertyValuesCollection_Inserted;
            _propertyValues.Removed += PropertyValuesCollection_Removed;
        }


        public ListFormProperty(IDocument document, XElement node)
        {
            Document = document;
            Node = node;

            _propertyValues = new ListFormPropertyElemCollection();

            foreach (XElement nodeChild in node.Elements())
            {
                if (nodeChild.Name == Ns.Form + "list-value")
                {
                    _propertyValues.Add(new ListFormPropertyElement(document, nodeChild));
                }
            }

            _propertyValues.Inserted += PropertyValuesCollection_Inserted;
            _propertyValues.Removed += PropertyValuesCollection_Removed;
        }

        /// <summary>
        /// Get the list of property elements
        /// </summary>
        public ListFormPropertyElemCollection PropertyValues
        {
            get { return _propertyValues; }
            set { _propertyValues = value; }
        }

        public override PropertyValueType? PropertyValueType
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Office + "value-type");
                if (s == null) return null;

                //PropertyValueType vt;
                switch (s)
                {
                    case "float":
                        return Forms.PropertyValueType.Float;
                    case "percentage":
                        return Forms.PropertyValueType.Percentage;
                    case "currency":
                        return Forms.PropertyValueType.Currency;
                    case "date":
                        return Forms.PropertyValueType.Date;
                    case "time":
                        return Forms.PropertyValueType.Time;
                    case "boolean":
                        return Forms.PropertyValueType.Boolean;
                    case "string":
                        return Forms.PropertyValueType.String;
                    default:
                        return Forms.PropertyValueType.String;
                }
            }
            set
            {
                string s = "";
                switch (value)
                {
                    case Forms.PropertyValueType.Float:
                        s = "float";
                        break;
                    case Forms.PropertyValueType.Percentage:
                        s = "percentage";
                        break;
                    case Forms.PropertyValueType.Currency:
                        s = "currency";
                        break;
                    case Forms.PropertyValueType.Date:
                        s = "date";
                        break;
                    case Forms.PropertyValueType.Time:
                        s = "time";
                        break;
                    case Forms.PropertyValueType.Boolean:
                        s = "boolean";
                        break;
                    case Forms.PropertyValueType.String:
                        s = "string";
                        break;
                }

                Node.SetAttributeValue(Ns.Office + "value-type", s);
            }
        }

        /// <summary>
        /// Propert name
        /// </summary>
        public override string Name
        {
            get { return (string) Node.Attribute(Ns.Form + "property-name"); }
            set { Node.SetAttributeValue(Ns.Form + "property-name", value); }
        }


        private void PropertyValuesCollection_Inserted(int index, object value)
        {
            Node.Add(((ListFormPropertyElement) value).Node);
        }

        private static void PropertyValuesCollection_Removed(int index, object value)
        {
            ((ListFormPropertyElement) value).Node.Remove();
        }
    }

    #endregion
}