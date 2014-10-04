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
using AODL.Document.Styles;

namespace AODL.Document.Content.Fields
{
    public class DateField : Field
    {
        public DateField(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Text + "date");
        }

        public DateField(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        /// <summary>
        /// Specifies whether or not the value of a field element is fixed
        /// </summary>
        public bool? Fixed
        {
            get { return (bool?)Node.Attribute(Ns.Text + "fixed"); }
            set { Node.SetAttributeValue(Ns.Text + "fixed", value); }
        }

        /// <summary>
        /// Specifies a particular date value.
        /// </summary>
        public DateTime DateValue
        {
            get
            {
                DateTime? value = (DateTime?)Node.Attribute(Ns.Text + "date-value");
                return value ?? new DateTime();
            }
            set { Node.SetAttributeValue(Ns.Text + "date-value", value); }
        }


        /// <summary>
        /// The adjustment of the date
        /// </summary>
        public string DateAdjust
        {
            get { return (string) Node.Attribute(Ns.Text + "date-adjust"); }
            set { Node.SetAttributeValue(Ns.Text + "date-adjust", value); }
        }

        public string DataStyleName
        {
            get { return (string) Node.Attribute(Ns.Style + "data-style-name"); }
            set { Node.SetAttributeValue(Ns.Style + "data-style-name", value); }
        }

        public IStyle DataStyle
        {
            get { return Document.Styles.GetStyleByName(DataStyleName); }
            set { DataStyleName = value.StyleName; }
        }
    }
}