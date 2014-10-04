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
    public enum PlaceholderType
    {
        Text,
        Table,
        TextBox,
        Image,
        Object
    } ;

    public class Placeholder : Field
    {
        public Placeholder(IDocument document, PlaceholderType placeholderType)
        {
            Document = document;
            Node = new XElement(Ns.Text + "placeholder");
            PlaceholderType = placeholderType;
        }

        public Placeholder(IDocument document, PlaceholderType placeholderType, string description)
            : this(document, placeholderType)
        {
            Description = description;
        }

        /// <summary>
        /// This attribute is mandatory and it indicates which type of text content the placeholder represents
        /// </summary>
        public PlaceholderType? PlaceholderType
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Text + "placeholder-type");
                if (s == null) return null;
                switch (s)
                {
                    case "text":
                        return Fields.PlaceholderType.Text;
                    case "table":
                        return Fields.PlaceholderType.Table;
                    case "text-box":
                        return Fields.PlaceholderType.TextBox;
                    case "image":
                        return Fields.PlaceholderType.Image;
                    case "object":
                        return Fields.PlaceholderType.Object;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Fields.PlaceholderType.Text:
                        s = "text";
                        break;
                    case Fields.PlaceholderType.Table:
                        s = "table";
                        break;
                    case Fields.PlaceholderType.TextBox:
                        s = "text-box";
                        break;
                    case Fields.PlaceholderType.Image:
                        s = "image";
                        break;
                    case Fields.PlaceholderType.Object:
                        s = "object";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Text + "placeholder-type", s);
            }
        }

        /// <summary>
        /// Provides additional description for the placeholder
        /// </summary>
        public string Description
        {
            get { return (string) Node.Attribute(Ns.Text + "description"); }
            set { Node.SetAttributeValue(Ns.Text + "description", value); }
        }
    }
}