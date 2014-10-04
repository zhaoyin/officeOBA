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
    public class TextInput : Field
    {
        public TextInput(IDocument document, string value)
        {
            Document = document;
            Node = new XElement(Ns.Text + "text-input");
            Value = value;
        }

        public TextInput(IDocument document, string value, string description) : this(document, value)
        {
            Description = description;
        }

        /// <summary>
        /// Provides additional description for the text input field
        /// </summary>
        public string Description
        {
            get { return (string) Node.Attribute(Ns.Text + "description"); }
            set { Node.SetAttributeValue(Ns.Text + "description", value); }
        }
    }
}