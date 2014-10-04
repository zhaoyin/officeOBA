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

namespace AODL.Document.Content.Fields
{
    public enum VariableValueType
    {
        Float,
        Percentage,
        Currency,
        Date,
        Time,
        Boolean,
        String
    }

    public class VariableDecl
    {
        /// <summary>
        /// Creates an VariableDecl instance
        /// </summary>
        /// <param name="document">The document it belogs to</param>
        /// <param name="valueType">Variable value type</param>
        public VariableDecl(IDocument document, VariableValueType valueType)
        {
            Document = document;
            Node = new XElement(Ns.Text + "variable-decl");
            VariableValueType = valueType;
        }

        /// <summary>
        /// Creates an VariableDecl instance
        /// </summary>
        /// <param name="document">The document it belogs to</param>
        /// <param name="valueType">Variable value type</param>
        /// <param name="name">Variable name</param>
        public VariableDecl(IDocument document, VariableValueType valueType, string name) : this(document, valueType)
        {
            Name = name;
        }

        public VariableDecl(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        public XElement Node { get; set; }

        /// <summary>
        /// The document
        /// </summary>
        public IDocument Document { get; set; }

        /// <summary>
        /// The name of the variable
        /// </summary>
        public string Name
        {
            get { return (string) Node.Attribute(Ns.Text + "name"); }
            set { Node.SetAttributeValue(Ns.Text + "name", value); }
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
                string s;
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
                    default:
                        s = string.Empty;
                        break;
                }

                Node.SetAttributeValue(Ns.Office + "value-type", s);
            }
        }
    }
}