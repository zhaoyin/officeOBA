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
using AODL.Document.Helper;
using AODL.Document.TextDocuments;

namespace AODL.Document.Styles.MasterStyles
{
    /// <summary>
    /// Summary for TextPageLayout.
    /// </summary>
    public class TextPageLayout
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TextPageLayout"/> class.
        /// </summary>
        public TextPageLayout()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextPageLayout"/> class.
        /// </summary>
        /// <param name="ownerDocument">The owner document.</param>
        /// <param name="styleNode">The style node.</param>
        /// <param name="propertyNode">The property node.</param>
        public TextPageLayout(TextDocument ownerDocument, XElement styleNode, XElement propertyNode)
        {
            TextDocument = ownerDocument;
            StyleNode = styleNode;
            PropertyNode = propertyNode;
        }

        /// <summary>
        /// Gets or sets the width of the page.
        /// e.g. 20.99 cm (A4)
        /// </summary>
        /// <value>The width of the page.</value>
        public string PageWidth
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "page-width"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "page-width", value); }
        }

        /// <summary>
        /// Gets or sets the height of the page.
        /// e.g 29,699cm (A4)
        /// </summary>
        /// <value>The height of the page.</value>
        public string PageHeight
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "page-height"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "page-height", value); }
        }

        /// <summary>
        /// Gets or sets the print orientation.
        /// e.g portrait, landscape
        /// </summary>
        /// <value>The print orientation.</value>
        public string PrintOrientation
        {
            get { return (string) PropertyNode.Attribute(Ns.Style + "print-orientation"); }
            set { PropertyNode.SetAttributeValue(Ns.Style + "print-orientation", value); }
        }

        /// <summary>
        /// Gets or sets the height of the footnote max. e.g. 0cm == without limit
        /// </summary>
        /// <value>The height of the footnote max.</value>
        public string FootnoteMaxHeight
        {
            get { return (string) PropertyNode.Attribute(Ns.Style + "footnote-max-height"); }
            set { PropertyNode.SetAttributeValue(Ns.Style + "footnote-max-height", value); }
        }

        /// <summary>
        /// Gets or sets the writing mode. e.g. lr-tab
        /// </summary>
        /// <value>The writing mode.</value>
        public string WritingMode
        {
            get { return (string) PropertyNode.Attribute(Ns.Style + "writing-mode"); }
            set { PropertyNode.SetAttributeValue(Ns.Style + "writing-mode", value); }
        }

        /// <summary>
        /// Gets or sets the num format. e.g. 1
        /// </summary>
        /// <value>The num format.</value>
        public string NumFormat
        {
            get { return (string) PropertyNode.Attribute(Ns.Style + "num-format"); }
            set { PropertyNode.SetAttributeValue(Ns.Style + "num-format", value); }
        }

        /// <summary>
        /// Gets or sets the margin top.
        /// e.g. 2cm
        /// </summary>
        /// <value>The margin top.</value>
        public string MarginTop
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "margin-top"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "margin-top", value); }
        }

        /// <summary>
        /// Gets or sets the margin bottom. e.g. 2cm
        /// </summary>
        /// <value>The margin bottom.</value>
        public string MarginBottom
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "margin-bottom"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "margin-bottom", value); }
        }

        /// <summary>
        /// Gets or sets the margin left. e.g. 2cm
        /// </summary>
        /// <value>The margin left.</value>
        public string MarginLeft
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "margin-left"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "margin-left", value); }
        }

        /// <summary>
        /// Gets or sets the margin right. e.g. 2cm
        /// </summary>
        /// <value>The margin right.</value>
        public string MarginRight
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "margin-right"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "margin-right", value); }
        }

        /// <summary>
        /// Gets or sets the text document.
        /// </summary>
        /// <value>The text document.</value>
        public TextDocument TextDocument { get; set; }

        /// <summary>
        /// The XElement which represent the page layout property element.
        /// </summary>
        /// <value>The node</value>
        public XElement PropertyNode { get; set; }

        /// <summary>
        /// The XElement which represent the page layout style element.
        /// </summary>
        /// <value>The node</value>
        public XElement StyleNode { get; set; }

        /// <summary>
        /// Gets the width of the content.
        /// </summary>
        /// <value>The width of the content area as double value without the left and right margins. 
        /// Notice, that you have to call GetLayoutMeasurement() to find out if 
        /// the the document use cm or inch.</value>
        /// <remarks>Will return 0 if the width couldn't be calculated.</remarks>
        public double GetContentWidth()
        {
            if (PageWidth != null
                && SizeConverter.GetDoubleFromAnOfficeSizeValue(PageWidth) > 0)
            {
                return SizeConverter.GetDoubleFromAnOfficeSizeValue(PageWidth)
                       - SizeConverter.GetDoubleFromAnOfficeSizeValue(MarginLeft)
                       - SizeConverter.GetDoubleFromAnOfficeSizeValue(MarginRight);
            }
            return 0;
        }

        /// <summary>
        /// Gets the layout measurement.
        /// </summary>
        /// <returns>True if it's in cm and false if it's in in.</returns>
        public bool GetLayoutMeasurement()
        {
            if (PageWidth != null)
            {
                return PageWidth.EndsWith("cm");
            }
            return true;
        }
    }
}