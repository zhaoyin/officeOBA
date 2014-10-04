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

namespace AODL.Document.Styles
{
    /// <summary>
    /// Class represent a TabStopStyle.
    /// </summary>
    public class TabStopStyle : AbstractStyle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabStopStyle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="position">The position.</param>
        public TabStopStyle(IDocument document, double position)
        {
            Document = document;
            Node = new XElement(Ns.Style + "tab-stop");

            XAttribute attribute = new XAttribute(Ns.Style + "position", position.ToString().Replace(",", ".") + "cm");
            Node.Add(attribute);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabStopStyle"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public TabStopStyle(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        /// <summary>
        /// Position e.g = "4.98cm";
        /// </summary>
        public string Position
        {
            get { return (string) Node.Attribute(Ns.Style + "position"); }
            set { Node.SetAttributeValue(Ns.Style + "position", value); }
        }

        /// <summary>
        /// A Tabstoptype e.g center
        /// </summary>
        public string Type
        {
            get { return (string) Node.Attribute(Ns.Style + "type"); }
            set { Node.SetAttributeValue(Ns.Style + "type", value); }
        }

        /// <summary>
        /// The Tabstop LeaderStyle e.g dotted
        /// </summary>
        public string LeaderStyle
        {
            get { return (string) Node.Attribute(Ns.Style + "leader-style"); }
            set { Node.SetAttributeValue(Ns.Style + "leader-style", value); }
        }

        /// <summary>
        /// The Tabstop Leader text e.g. "."
        /// Use this if you use the LeaderStyle property
        /// </summary>
        public string LeaderText
        {
            get { return (string) Node.Attribute(Ns.Style + "leader-text"); }
            set { Node.SetAttributeValue(Ns.Style + "leader-text", value); }
        }
    }
}

/*
 * $Log: TabStopStyle.cs,v $
 * Revision 1.2  2008/04/29 15:39:54  mt
 * new copyright header
 *
 * Revision 1.1  2007/02/25 08:58:50  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.1  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 */