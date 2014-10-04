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
using AODL.Document.TextDocuments;

namespace AODL.Document.Styles.MasterStyles
{
    /// <summary>
    /// Summary for TextPageHeaderFooterBase.
    /// </summary>
    public class TextPageHeaderFooterBase
    {
        private TextDocument _textDocument;
        private TextMasterPage _textMasterPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextPageFooter"/> class.
        /// </summary>
        protected TextPageHeaderFooterBase()
        {
            ContentCollection = new ContentCollection();
            ContentCollection.Inserted += _contentCollection_Inserted;
            ContentCollection.Removed += _contentCollection_Removed;
        }

        /// <summary>
        /// Gets or sets the owner text master page.
        /// </summary>
        /// <value>The text master page.</value>
        public TextMasterPage TextMasterPage
        {
            get { return _textMasterPage; }
            set { _textMasterPage = value; }
        }

        /// <summary>
        /// Gets or sets the text document.
        /// </summary>
        /// <value>The text document.</value>
        public TextDocument TextDocument
        {
            get { return _textDocument; }
            set { _textDocument = value; }
        }

        /// <summary>
        /// Gets or sets the property node.
        /// </summary>
        /// <value>The property node.</value>
        public XElement PropertyNode { get; set; }

        /// <summary>
        /// Gets or sets the content node.
        /// </summary>
        /// <value>The content node.</value>
        public XElement ContentNode { get; set; }

        /// <summary>
        /// The XElement which represent the page layout style element.
        /// </summary>
        /// <value>The node</value>
        public XElement StyleNode { get; set; }

        /// <summary>
        /// Gets or sets the min height. e.g. 0cm 
        /// </summary>
        /// <value>The height of the min.</value>
        public string MinHeight
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "min-height"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "min-height", value); }
        }

        /// <summary>
        /// Gets or sets the margin right. e.g. 0cm
        /// </summary>
        /// <value>The margin right.</value>
        public string MarginRight
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "margin-right"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "margin-right", value); }
        }

        /// <summary>
        /// Gets or sets the margin bottom. e.g. 0.499cm
        /// </summary>
        /// <value>The margin bottom.</value>
        public string MarginBottom
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "margin-bottom"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "margin-bottom", value); }
        }

        /// <summary>
        /// Gets or sets the margin top. e.g. 0.499cm
        /// </summary>
        /// <value>The margin bottom.</value>
        public string MarginTop
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "margin-top"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "margin-top", value); }
        }

        /// <summary>
        /// Gets or sets the margin left. e.g. 0cm
        /// </summary>
        /// <value>The margin left.</value>
        public string MarginLeft
        {
            get { return (string) PropertyNode.Attribute(Ns.Fo + "margin-left"); }
            set { PropertyNode.SetAttributeValue(Ns.Fo + "margin-left", value); }
        }

        /// <summary>
        /// Gets or sets the content collection.
        /// </summary>
        /// <value>The content collection.</value>
        /// <remarks>This is the content which will be displayed within the footer or the header.</remarks>
        public ContentCollection ContentCollection { get; set; }

        /// <summary>
        /// Create a new TextPageFooter object.
        /// !!NOTICE: The TextPageLayout of the TextMasterPage object must exist!
        /// </summary>
        /// <param name="textMasterPage">The text master page.</param>
        /// <param name="typeName">Name of the type to create header or footer.</param>
        /// <remarks>The TextPageLayout of the TextMasterPage object must exist!</remarks>
        protected void New(TextMasterPage textMasterPage, string typeName)
        {
            _textMasterPage = textMasterPage;
            _textDocument = textMasterPage.TextDocument;
            StyleNode = new XElement(Ns.Style + "footer-style");
            _textMasterPage.TextPageLayout.StyleNode.Add(StyleNode);
        }

        /// <summary>
        /// This method call will activate resp. create the header or footer 
        /// for a master page. There is no need of call it directly.
        /// Activation for footer and header will be done through
        /// the TextMasterPage object. 
        /// </summary>
        public void Activate()
        {
            string typeName = (this is TextPageHeader) ? "header" : "footer";

            // only if the content node doesn't exist
            if (ContentNode == null)
            {
                ContentNode = new XElement(Ns.Style + typeName);
                _textMasterPage.Node.Add(ContentNode);
            }

            // only if the property node doesn't exist
            if (PropertyNode == null)
            {
                PropertyNode = new XElement(Ns.Style + "header-footer-properties");
                // Set defaults
                MarginLeft = "0cm";
                MarginRight = "0cm";
                MinHeight = "0cm";
                MarginBottom = (typeName.Equals("header")) ? "0.499cm" : "0cm";
                MarginTop = (typeName.Equals("footer")) ? "0.499cm" : "0cm";
                StyleNode.Add(PropertyNode);
            }
        }

        /// <summary>
        /// Called after content was added.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void _contentCollection_Inserted(int index, object value)
        {
            ContentNode.Add(((IContent) value).Node);
        }

        /// <summary>
        /// Called after content was removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void _contentCollection_Removed(int index, object value)
        {
            ((IContent) value).Node.Remove();
        }
    }
}