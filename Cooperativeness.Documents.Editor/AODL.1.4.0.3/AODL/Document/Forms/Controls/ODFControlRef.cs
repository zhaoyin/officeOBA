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
using AODL.Document.Content;
using AODL.Document.Styles;

namespace AODL.Document.Forms.Controls
{
    /// <summary>
    /// Summary description for ODFControlRef.
    /// </summary>
    public enum AnchorType
    {
        Page,
        Frame,
        Paragraph,
        Char,
        AsChar
    } ;

    public class ODFControlRef : IContent
    {
        public ODFControlRef(IDocument doc, XElement node)
        {
            Document = doc;
            Node = node;
        }

        public ODFControlRef(IDocument doc, string drawControl)
        {
            Document = doc;
            Node = new XElement(Ns.Draw + "control");
            DrawControl = drawControl;
        }

        public ODFControlRef(IDocument doc, string drawControl, string x, string y, string width, string height)
            : this(doc, drawControl)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public string X
        {
            get { return (string) Node.Attribute(Ns.Svg + "x"); }
            set { Node.SetAttributeValue(Ns.Svg + "x", value); }
        }

        public string Y
        {
            get { return (string) Node.Attribute(Ns.Svg + "y"); }
            set { Node.SetAttributeValue(Ns.Svg + "y", value); }
        }

        public string Width
        {
            get { return (string) Node.Attribute(Ns.Svg + "width"); }
            set { Node.SetAttributeValue(Ns.Svg + "width", value); }
        }

        public string Height
        {
            get { return (string) Node.Attribute(Ns.Svg + "height"); }
            set { Node.SetAttributeValue(Ns.Svg + "height", value); }
        }

        public string Layer
        {
            get { return (string) Node.Attribute(Ns.Draw + "layer"); }
            set { Node.SetAttributeValue(Ns.Draw + "layer", value); }
        }

        public string Id
        {
            get { return (string) Node.Attribute(Ns.Draw + "id"); }
            set { Node.SetAttributeValue(Ns.Draw + "id", value); }
        }

        public string DrawControl
        {
            get { return (string) Node.Attribute(Ns.Draw + "control"); }
            set { Node.SetAttributeValue(Ns.Draw + "control", value); }
        }

        public string Transform
        {
            get { return (string) Node.Attribute(Ns.Draw + "transform"); }
            set { Node.SetAttributeValue(Ns.Draw + "transform", value); }
        }

        public int ZIndex
        {
            get { return (int) Node.Attribute(Ns.Svg + "z-index"); }
            set
            {
                if (value <= 0)
                    return;
                Node.SetAttributeValue(Ns.Svg + "z-index", value);
            }
        }

        public int AnchorPageNumber
        {
            get { return (int) Node.Attribute(Ns.Text + "anchor-page-number"); }
            set
            {
                if (value <= 0)
                    return;
                Node.SetAttributeValue(Ns.Text + "anchor-page-number", value);
            }
        }

        public AnchorType? AnchorType
        {
            get
            {
                string s = (string) Node.Attribute(Ns.Text + "anchor-type");
                if (s == null) return null;

                switch (s)
                {
                    case "page":
                        return Controls.AnchorType.Page;
                    case "frame":
                        return Controls.AnchorType.Frame;
                    case "paragraph":
                        return Controls.AnchorType.Paragraph;
                    case "char":
                        return Controls.AnchorType.Char;
                    case "as-char":
                        return Controls.AnchorType.AsChar;
                    default:
                        return null;
                }
            }
            set
            {
                string s;
                switch (value)
                {
                    case Controls.AnchorType.Page:
                        s = "page";
                        break;
                    case Controls.AnchorType.Frame:
                        s = "frame";
                        break;
                    case Controls.AnchorType.Paragraph:
                        s = "paragraph";
                        break;
                    case Controls.AnchorType.Char:
                        s = "char";
                        break;
                    case Controls.AnchorType.AsChar:
                        s = "as-char";
                        break;
                    default:
                        s = null;
                        break;
                }
                Node.SetAttributeValue(Ns.Text + "anchor-type", s);
            }
        }

        public bool? TableBackground
        {
            get { return (bool?) Node.Attribute(Ns.Table + "table-background"); }
            set { Node.SetAttributeValue(Ns.Table + "table-background", value); }
        }

        /// <summary>
        /// The text style name.
        /// If no style is available this is null.
        /// </summary>
        public string TextStyleName
        {
            get { return (string) Node.Attribute(Ns.Draw + "text-style-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "text-style-name", value); }
        }


        /// <summary>
        /// A Style class wich is referenced with the text content object.
        /// If no style is available this is null.
        /// </summary>
        public IStyle TextStyle
        {
            get { return Document.Styles.GetStyleByName(TextStyleName); }
            set { TextStyleName = value.StyleName; }
        }


        /// <summary>
        /// Represents the XElement within the content.xml from the odt file.
        /// </summary>
        /// 
        public XElement Node { get; set; }

        #region IContent Members

        public IDocument Document { get; set; }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        XNode IContent.Node
        {
            get { return Node; }
            set { Node = (XElement) value; }
        }

        /// <summary>
        /// The stylename which is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        public string StyleName
        {
            get { return (string) Node.Attribute(Ns.Draw + "style-name"); }
            set { Node.SetAttributeValue(Ns.Draw + "style-name", value); }
        }

        /// <summary>
        /// A Style class wich is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        public IStyle Style
        {
            get { return Document.Styles.GetStyleByName(StyleName); }
            set { StyleName = value.StyleName; }
        }

        #endregion
    }
}