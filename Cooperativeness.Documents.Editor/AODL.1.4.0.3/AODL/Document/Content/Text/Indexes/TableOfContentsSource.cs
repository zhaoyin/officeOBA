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

namespace AODL.Document.Content.Text.Indexes
{
    /// <summary>
    /// TableOfContentSource.
    /// </summary>
    public class TableOfContentsSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableOfContentsSource"/> class.
        /// </summary>
        /// <param name="tableOfContents">Content of the table of.</param>
        public TableOfContentsSource(TableOfContents tableOfContents)
        {
            TableOfContents = tableOfContents;
            TableOfContentsIndexTemplateCollection = new TableOfContentsIndexTemplateCollection();
            Node = new XElement(Ns.Text + "table-of-content-source");

            //the only supported table of content style, will be
            //based on the header and their outline levels,
            //but loading other styles should be possible, but won't
            //be modifiable.
            Node.SetAttributeValue(Ns.Text + "outline-level", "10");

            //Create the index-title-template node
            //this is always the title of the TableOfContent
            //of the referenced TableOfContent oject
            XElement indexTitleTemplateNode = new XElement(Ns.Text + "index-title-template", TableOfContents.Title);

            //Fixed style for the title template
            indexTitleTemplateNode.SetAttributeValue(Ns.Text + "style-name", "Contents_20_Heading");
            Node.Add(indexTitleTemplateNode);
        }

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public XElement Node { get; set; }

        /// <summary>
        /// Gets or sets the content of the table of.
        /// </summary>
        /// <value>The content of the table of.</value>
        public TableOfContents TableOfContents { get; set; }

        /// <summary>
        /// Gets or sets the table of content index template collection.
        /// </summary>
        /// <value>The table of content index template collection.</value>
        public TableOfContentsIndexTemplateCollection TableOfContentsIndexTemplateCollection { get; set; }

        /// <summary>
        /// Init the standard style source template styles.
        /// </summary>
        public void InitStandardTableOfContentStyle()
        {
            for (int i = 1; i <= 10; i++)
            {
                TableOfContentsIndexTemplate tableOfContentsIndexTemplate =
                    new TableOfContentsIndexTemplate(
                        TableOfContents,
                        i,
                        "Contents_20_" + i);

                tableOfContentsIndexTemplate.InitStandardTemplate();
                Node.Add(tableOfContentsIndexTemplate.Node);
                TableOfContentsIndexTemplateCollection.Add(
                    tableOfContentsIndexTemplate);
            }
        }
    }
}

/*
 * $Log: TableOfContentsSource.cs,v $
 * Revision 1.3  2008/04/29 15:39:47  mt
 * new copyright header
 *
 * Revision 1.2  2007/04/08 16:51:24  larsbehr
 * - finished master pages and styles for text documents
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:41  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.1  2006/01/29 11:28:22  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.1  2006/01/05 10:31:10  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 */