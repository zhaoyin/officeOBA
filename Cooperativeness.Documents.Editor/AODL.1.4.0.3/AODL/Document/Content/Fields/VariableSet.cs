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
using AODL.Document.Styles;
using AODL.Document.TextDocuments;

namespace AODL.Document.Content.Fields
{
    public enum Display
    {
        None,
        Formula,
        Value
    } ;

    public class VariableSet : Field
    {
        public VariableSet(IDocument document, VariableDecl variableDeclaration)
        {
            Document = document;
            Node = new XElement(Ns.Text + "variable-set");
            VariableDeclaration = variableDeclaration;
        }

        public VariableSet(IDocument document, VariableDecl variableDeclaration, string value)
            : this(document, variableDeclaration)
        {
            Value = value;
        }

        /// <summary>
        /// The formula to compute the value of the variable field
        /// </summary>
        public string Formula
        {
            get { return (string) Node.Attribute(Ns.Text + "formula"); }
            set { Node.SetAttributeValue(Ns.Text + "formula", value); }
        }

        /// <summary>
        /// The formula to compute the value of the variable field
        /// </summary>
        public VariableDecl VariableDeclaration
        {
            get
            {
                string name = (string) Node.Attribute(Ns.Text + "name");
                if (name == null)
                    return null;
                return ((TextDocument) Document).VariableDeclarations.GetVariableDeclByName(name);
            }
            set
            {
                if (value == null)
                    return;
                Node.SetAttributeValue(Ns.Text + "name", value.Name);
            }
        }

        /// <summary>
        /// Defines the way the variable is displayed
        /// </summary>
        public Display? Display
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Text + "display");
                if (s == null) return null;

                switch (s)
                {
                    case "value":
                        return Fields.Display.Value;
                    case "formula":
                        return Fields.Display.Formula;
                    case "none":
                        return Fields.Display.None;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Fields.Display.Value:
                        s = "value";
                        break;
                    case Fields.Display.Formula:
                        s = "formula";
                        break;
                    case Fields.Display.None:
                        s = "none";
                        break;
                    default:
                        s = null;
                        break;
                }

                Node.SetAttributeValue(Ns.Text + "display", s);
            }
        }

        /// <summary>
        /// Defines the type of the variable
        /// </summary>
        public VariableValueType? VariableValueType
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Office + "value-type");
                if (s == null) return null;

                switch (s)
                {
                    case "float":
                        return Fields.VariableValueType.Float;
                    case "percentage":
                        return Fields.VariableValueType.Percentage;
                    case "currency":
                        return Fields.VariableValueType.Currency;
                    case "date":
                        return Fields.VariableValueType.Date;
                    case "time":
                        return Fields.VariableValueType.Time;
                    case "boolean":
                        return Fields.VariableValueType.Boolean;
                    case "string":
                        return Fields.VariableValueType.String;
                    default:
                        return Fields.VariableValueType.String;
                }
            }
            set
            {
                string s = "";
                switch (value)
                {
                    case Fields.VariableValueType.Float:
                        s = "float";
                        break;
                    case Fields.VariableValueType.Percentage:
                        s = "percentage";
                        break;
                    case Fields.VariableValueType.Currency:
                        s = "currency";
                        break;
                    case Fields.VariableValueType.Date:
                        s = "date";
                        break;
                    case Fields.VariableValueType.Time:
                        s = "time";
                        break;
                    case Fields.VariableValueType.Boolean:
                        s = "boolean";
                        break;
                    case Fields.VariableValueType.String:
                        s = "string";
                        break;
                }

                Node.SetAttributeValue(Ns.Office + "value-type", s);
            }
        }

        /// <summary>
        /// Refers to the data style used to format the numeric value.
        /// If no style is available this is null.
        /// </summary>
        public string DataStyleName
        {
            get { return (string) Node.Attribute(Ns.Style + "data-style-name"); }
            set { Node.SetAttributeValue(Ns.Style + "data-style-name", value); }
        }

        /// <summary>
        /// Refers to the data style used to format the numeric value.
        /// If no style is available this is null.
        /// </summary>
        public IStyle DataStyle
        {
            get { return Document.Styles.GetStyleByName(DataStyleName); }
            set { DataStyleName = value.StyleName; }
        }
    }
}