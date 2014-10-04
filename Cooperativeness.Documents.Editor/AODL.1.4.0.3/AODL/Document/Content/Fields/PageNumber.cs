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
    public enum SelectPage
    {
        Previous,
        Next,
        Current
    } ;

    public class PageNumber : Field
    {
        public PageNumber(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Text + "page-number");
        }

        public PageNumber(IDocument document, int pageAdjustment) : this(document)
        {
            PageAdjustment = pageAdjustment;
        }

        /// <summary>
        /// The value of a page number field can be adjusted by a specified number, allowing the display of
        /// page numbers of following or preceding pages.
        /// </summary>
        public int PageAdjustment
        {
            get { return (int) Node.Attribute(Ns.Text + "page-adjust"); }
            set { Node.SetAttributeValue(Ns.Text + "page-adjust", value); }
        }

        /// <summary>
        ///  Is used to display the number of the previous or the following page rather than the number of the current page.
        /// </summary>
        public SelectPage? SelectPage
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Text + "select-page");

                if (s == null) return null;
                switch (s)
                {
                    case "current":
                        return Fields.SelectPage.Current;
                    case "next":
                        return Fields.SelectPage.Next;
                    case "previous":
                        return Fields.SelectPage.Previous;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Fields.SelectPage.Current:
                        s = "current";
                        break;
                    case Fields.SelectPage.Next:
                        s = "next";
                        break;
                    case Fields.SelectPage.Previous:
                        s = "previous";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Text + "select-page", s);
            }
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
        /// Specifies the format of the number in the same way as the [XSLT] format attribute
        /// </summary>
        public string NumFormat
        {
            get { return (string) Node.Attribute(Ns.Style + "num-format"); }
            set { Node.SetAttributeValue(Ns.Style + "num-format", value); }
        }
    }
}