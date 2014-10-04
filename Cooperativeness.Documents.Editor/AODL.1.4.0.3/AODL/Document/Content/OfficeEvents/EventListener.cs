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
using AODL.Document.Import.OpenDocument.NodeProcessors;
using AODL.Document.Styles;

//using System.Collections.Generic;

namespace AODL.Document.Content.OfficeEvents
{
    /// <summary>
    /// Summary for EventListener.
    /// </summary>
    public class EventListener : IContent, ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventListener"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="eventname">The eventname.</param>
        /// <param name="language">The language.</param>
        /// <param name="href">The href.</param>
        public EventListener(IDocument document,
                             string eventname,
                             string language,
                             string href)
        {
            Document = document;
            Node = new XElement(Ns.Script + "event-listener");

            EventName = eventname;
            Language = language;
            Href = href;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventListener"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public EventListener(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

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
        /// A draw:area-rectangle doesn't have a style-name.
        /// </summary>
        /// <value>The name</value>
        public string StyleName
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// A draw:area-rectangle doesn't have a style.
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

        #region IContent Member old

//		private TextDocument _document;
//		/// <summary>
//		/// The TextDocument to which this draw-area is bound.
//		/// </summary>
//		public TextDocument Document
//		{
//			get
//			{
//				return this._document;
//			}
//			set
//			{
//				this._document = value;
//			}
//		}
//
//		private XElement _node;
//		/// <summary>
//		/// The XElement.
//		/// </summary>
//		public XElement Node
//		{
//			get
//			{
//				return this._node;
//			}
//			set
//			{
//				this._node = value;
//			}
//		}
//
//		/// <summary>
//		/// A draw:area-rectangle doesn't have a style-name.
//		/// </summary>
//		/// <value>The name</value>
//		public string Stylename
//		{
//			get
//			{
//				return null;
//			}
//			set
//			{
//			}
//		}
//
//		/// <summary>
//		/// A draw:area-rectangle doesn't have a style.
//		/// </summary>
//		public IStyle Style
//		{
//			get
//			{
//				return null;
//			}
//			set
//			{
//			}
//		}
//
//		/// <summary>
//		/// A draw:area-rectangle doesn't contain text.
//		/// </summary>
//		public ITextCollection TextContent
//		{
//			get
//			{
//				return null;
//			}
//			set
//			{
//			}
//		}

        #endregion

        #region ICloneable Member

        /// <summary>
        /// Create a deep clone of this EventListener object.
        /// </summary>
        /// <remarks>A possible Attached Style wouldn't be cloned!</remarks>
        /// <returns>
        /// A clone of this object.
        /// </returns>
        public object Clone()
        {
            EventListener eventListenerClone = null;

            if (Document != null && Node != null)
            {
                MainContentProcessor mcp = new MainContentProcessor(Document);
                eventListenerClone = mcp.CreateEventListener(new XElement(Node));
            }

            return eventListenerClone;
        }

        #endregion

        /// <summary>
        /// Gets or sets the href. e.g http://www.sourceforge.net
        /// </summary>
        /// <value>The href.</value>
        public string Href
        {
            get { return (string) Node.Attribute(Ns.XLink + "href"); }
            set { Node.SetAttributeValue(Ns.XLink + "href", value); }
        }

        /// <summary>
        /// Gets or sets the type of the X link.
        /// </summary>
        /// <value>The type of the X link.</value>
        public string XLinkType
        {
            get { return (string) Node.Attribute(Ns.XLink + "type"); }
            set { Node.SetAttributeValue(Ns.XLink + "type", value); }
        }

        /// <summary>
        /// Gets or sets the event name ("dom:onclick").
        /// </summary>
        /// <value>The event name of the event listener.</value>
        public string EventName
        {
            get { return (string) Node.Attribute(Ns.Script + "event-name"); }
            set { Node.SetAttributeValue(Ns.Script + "event-name", value); }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The language of the event listener.</value>
        public string Language
        {
            get { return (string) Node.Attribute(Ns.Script + "language"); }
            set { Node.SetAttributeValue(Ns.Script + "language", value); }
        }

//		/// <summary>
//		/// Contents the collection_ inserted.
//		/// </summary>
//		/// <param name="index">The index.</param>
//		/// <param name="value">The value.</param>
//		protected void ContentCollection_Inserted(int index, object value)
//		{
//			this.Node.AppendChild(((IContent)value).Node);
//		}
    }
}