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
using AODL.Document.Content.Draw;
using AODL.Document.Styles;

namespace AODL.Document.Content.EmbedObjects
{
    /// <summary>
    /// Summary description for EmbedObject.
    /// </summary>
    public class EmbedObject : IContent
    {
        public EmbedObject(XElement parentNode, IDocument document)
        {
            ParentNode = parentNode;
            Document = document;
        }

        public EmbedObject(IDocument document)
        {
            Document = document;
            ParentNode = new XElement(Ns.Draw + "object");
            ParentNode.SetAttributeValue(Ns.XLink + "type", "simple");
            ParentNode.SetAttributeValue(Ns.XLink + "show", "embed");
            ParentNode.SetAttributeValue(Ns.XLink + "actuate", "onLoad");
        }

        /// <summary>
        /// Gets or sets the H ref.
        /// </summary>
        /// <value>The H ref.</value>
        public string HRef
        {
            get { return (string) ParentNode.Attribute(Ns.XLink + "href"); }
            set { ParentNode.SetAttributeValue(Ns.XLink + "href", value); }
        }

        /// <summary>
        /// Gets or sets the actuate.
        /// e.g. onLoad
        /// </summary>
        /// <value>The actuate.</value>
        public string Actuate
        {
            get { return (string)ParentNode.Attribute(Ns.XLink + "actuate"); }
            set { ParentNode.SetAttributeValue(Ns.XLink + "actuate", value); }
        }

        /// <summary>
        /// Gets or sets the type of the Xlink.
        /// e.g. simple, standard, ..
        /// </summary>
        /// <value>The type of the X link.</value>
        public string XLinkType
        {
            get { return (string)ParentNode.Attribute(Ns.XLink + "type"); }
            set { ParentNode.SetAttributeValue(Ns.XLink + "type", value); }
        }

        /// <summary>
        /// Gets or sets the show.
        /// e.g. embed
        /// </summary>
        /// <value>The show.</value>
        public string Show
        {
            get { return (string)ParentNode.Attribute(Ns.XLink + "show"); }
            set { ParentNode.SetAttributeValue(Ns.XLink + "show", value); }
        }

        /// <summary>
        /// Gets or sets the embed object real path.
        /// </summary>
        /// <value>The object real path.</value>
        public string ObjectRealPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the object file.
        /// </summary>
        /// <value>The name of the object file.</value>
        public string ObjectFileName { get; set; }

        /// <summary>
        /// Gets or sets the frame.
        /// </summary>
        /// <value>The frame.</value>
        public Frame Frame { get; set; }

        public string ObjectName { get; set; }

        public string ObjectType { get; set; }


        public XElement ParentNode { get; set; }

        #region IContent Member

        private IStyle _style;

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public XElement Node { get; set; }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <value>The name of the style.</value>
        public virtual string StyleName
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Every object (typeof(IContent)) have to know his document.
        /// </summary>
        /// <value></value>
        public IDocument Document { get; set; }

        /// <summary>
        /// A Style class wich is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        /// <value></value>
        public IStyle Style
        {
            get { return _style; }
            set
            {
                StyleName = value.StyleName;
                _style = value;
            }
        }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        XNode IContent.Node
        {
            get { return Node; }
            set { Node = (XElement) value; }
        }

        #endregion
    }
}