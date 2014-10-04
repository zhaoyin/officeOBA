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
using AODL.Document.TextDocuments;

namespace AODL.Document.Styles.MasterStyles
{
    /// <summary>
    /// Summary for TextMasterPage.
    /// </summary>
    public class TextMasterPage
    {
        private TextPageFooter _textPageFooter;
        private TextPageHeader _textPageHeader;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMasterPage"/> class.
        /// </summary>
        public TextMasterPage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMasterPage"/> class.
        /// </summary>
        /// <param name="ownerTextDocument">The owner text document.</param>
        /// <param name="masterPageNode">The master page node.</param>
        public TextMasterPage(TextDocument ownerTextDocument, XElement masterPageNode)
        {
            TextDocument = ownerTextDocument;
            Node = masterPageNode;
        }

        /// <summary>
        /// Gets or sets the text page layout.
        /// </summary>
        /// <value>The text page layout.</value>
        public TextPageLayout TextPageLayout { get; set; }

        /// <summary>
        /// Gets or sets the text page header.
        /// </summary>
        /// <value>The text page header.</value>
        public TextPageHeader TextPageHeader
        {
            get { return _textPageHeader; }
            set { _textPageHeader = value; }
        }

        /// <summary>
        /// Gets or sets the text page footer.
        /// </summary>
        /// <value>The text page footer.</value>
        public TextPageFooter TextPageFooter
        {
            get { return _textPageFooter; }
            set { _textPageFooter = value; }
        }

        /// <summary>
        /// Gets or sets the name of the style.
        /// </summary>
        /// <value>The name of the style.</value>
        public string StyleName
        {
            get { return (string) Node.Attribute(Ns.Style + "name"); }
            set { Node.SetAttributeValue(Ns.Style + "name", value); }
        }

        /// <summary>
        /// Gets or sets the name of the page layout.
        /// </summary>
        /// <value>The name of the page layout.</value>
        public string PageLayoutName
        {
            get { return (string) Node.Attribute(Ns.Style + "page-layout-name"); }
            set { Node.SetAttributeValue(Ns.Style + "page-layout-name", value); }
        }

        /// <summary>
        /// Gets or sets the text document.
        /// </summary>
        /// <value>The text document.</value>
        public TextDocument TextDocument { get; set; }

        /// <summary>
        /// The XElement which represent the master layout element.
        /// </summary>
        /// <value>The node</value>
        public XElement Node { get; set; }

        /// <summary>
        /// Activate usage of the page header.
        /// </summary>
        public void ActivatePageHeader()
        {
            if (_textPageHeader != null)
            {
                TextPageHeader.Activate();
            }
        }

        /// <summary>
        /// Activate usage of the page footer.
        /// </summary>
        public void ActivatePageFooter()
        {
            if (_textPageFooter != null)
            {
                TextPageFooter.Activate();
            }
        }

        /// <summary>
        /// Activate usage of the page header and page footer.
        /// </summary>
        public void ActivatePageHeaderAndFooter()
        {
            ActivatePageHeader();
            ActivatePageFooter();
        }
    }
}