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
using System.Collections.Generic;
using System.Xml.Linq;
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.Styles;

namespace AODL.Document.Content.OfficeEvents
{
    /// <summary>
    /// Summary for EventListeners
    /// </summary>
    /// <example>
    ///		<code>
    ///		<pre>
    ///        &lt;office:event-listeners&gt;
    ///            &lt;script:event-listener
    ///                script:language="JavaScript"
    ///               script:event-name="dom:onclick"
    ///                xlink:href="changeImage(‘dim’)"/&gt;
    ///            &lt;script:event-listener
    ///                script:language="JavaScript"
    ///                script:event-name="dom:onmouseover"
    ///                xlink:href="setCursor('hand')"/&gt;
    ///            &lt;script:event-listener
    ///                script:language="JavaScript"
    ///                script:event-name="dom:onmouseout"
    ///                xlink:href="setCursor('arrow')"/&gt;
    ///        &lt;/office:event-listeners&gt;
    ///      </pre>
    ///      </code>
    /// </example>
    public class EventListeners : IContent, IContentContainer, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventListeners"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public EventListeners(IDocument document, XElement node)
        {
            Document = document;
            InitStandards();
            Node = node;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListeners"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="eventListeners">Array of EventListeners.</param>
        public EventListeners(IDocument document, IEnumerable<EventListener> eventListeners)
        {
            Document = document;
            InitStandards();
            Node = new XElement(Ns.Office + "event-listeners");

            if (eventListeners != null)
            {
                foreach (EventListener d in eventListeners)
                {
                    _content.Add(d);
                }
            }
        }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            Content = new ContentCollection();
            Content.Inserted += Content_Inserted;
            Content.Removed += Content_Removed;
        }

        /// <summary>
        /// Content_s the inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void Content_Inserted(int index, object value)
        {
            if (Node != null)
                Node.Add(((IContent) value).Node);
        }

        /// <summary>
        /// Content_s the removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void Content_Removed(int index, object value)
        {
            ((IContent) value).Node.Remove();
        }

        #region IContentCollection Member

        private ContentCollection _content;

        /// <summary>
        /// Gets or sets the content collection.
        /// </summary>
        /// <value>The content collection.</value>
        public ContentCollection Content
        {
            get { return _content; }
            set { _content = value; }
        }

        #endregion

        #region IContent Member		

        /// <summary>
        /// Gets or sets the node.
        /// </summary>
        /// <value>The node.</value>
        public XElement Node { get; set; }

        /// <summary>
        /// Every object (typeof(IContent)) have to know his document.
        /// </summary>
        /// <value></value>
        public IDocument Document { get; set; }

        /// <summary>
        /// A office:event-listeners doesn't have a style-name.
        /// </summary>
        /// <value>The name</value>
        public string StyleName
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// A office:event-listeners doesn't have a style.
        /// </summary>
        public IStyle Style
        {
            get { return null; }
            set { }
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

        #region ICloneable Member

        /// <summary>
        /// Create a deep clone of this EventListeners object.
        /// </summary>
        /// <remarks>A possible Attached Style wouldn't be cloned!</remarks>
        /// <returns>
        /// A clone of this object.
        /// </returns>
        public object Clone()
        {
            EventListeners eventListenersClone = null;

            if (Document != null && Node != null)
            {
                MainContentProcessor mcp = new MainContentProcessor(Document);
                eventListenersClone = mcp.CreateEventListeners(new XElement(Node));
            }

            return eventListenersClone;
        }

        #endregion
    }
}