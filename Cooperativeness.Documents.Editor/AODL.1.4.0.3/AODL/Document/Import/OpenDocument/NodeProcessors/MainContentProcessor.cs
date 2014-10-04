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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AODL.Document.Content;
using AODL.Document.Content.Draw;
using AODL.Document.Content.EmbedObjects;
using AODL.Document.Content.OfficeEvents;
using AODL.Document.Content.Tables;
using AODL.Document.Content.Text;
using AODL.Document.Content.Text.Indexes;
using AODL.Document.Exceptions;
using AODL.Document.Forms.Controls;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.Styles;
using AODL.Document.TextDocuments;

namespace AODL.Document.Import.OpenDocument.NodeProcessors
{
    public class MainContentProcessor
    {
        #region Delegates

        /// <summary>
        /// Warning delegate
        /// </summary>
        public delegate void WarningHandler(AODLWarning warning);

        #endregion

        /// <summary>
        /// The textdocument
        /// </summary>
        private readonly IDocument _document;

        /// <summary>
        /// If set to true all node content would be directed
        /// to Console.Out
        /// </summary>
        private bool _debugMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainContentProcessor"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public MainContentProcessor(IDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// Warning event fired if something unexpected
        /// occour.
        /// </summary>
        public event WarningHandler Warning;

        private void OnWarning(AODLWarning warning)
        {
            if (Warning != null)
            {
                Warning(warning);
            }
        }

        private void AddToCollection(IContent content, ContentCollection coll)
        {
            coll.Add(content);

            if (content is ODFControlRef)
            {
                ODFControlRef ctrlRef = content as ODFControlRef;
                if (_document is TextDocument)
                {
                    TextDocument td = _document as TextDocument;
                    ODFFormControl fc = td.FindControlById(ctrlRef.DrawControl);

                    if (fc != null)
                    {
                        fc.ContentCollection = coll;
                        fc.ControlRef = ctrlRef;
                    }
                }
            }
        }

        /// <summary>
        /// Reads the content nodes.
        /// </summary>
        public void ReadContentNodes()
        {
            try
            {
                //				this._document.XmlDoc	= new XDocument();
                //				this._document.XmlDoc.Load(contentFile);

                XElement node = null;

                if (_document is TextDocument)
                    node = _document.XmlDoc.Elements(Ns.Office + "document-content")
                        .Elements(Ns.Office + "body")
                        .Elements(Ns.Office + "text").FirstOrDefault();
                else if (_document is SpreadsheetDocument)
                    node = _document.XmlDoc.Elements(Ns.Office + "document-content")
                        .Elements(Ns.Office + "body")
                        .Elements(Ns.Office + "spreadsheet").FirstOrDefault();

                if (node != null)
                {
                    CreateMainContent(node);
                }
                else
                {
                    throw new AODLException("Unknow content type.");
                }
                //Remove all existing content will be created new
                node.RemoveAll();
            }
            catch (Exception ex)
            {
                throw new AODLException("Error while trying to load the content file!", ex);
            }
        }

        /// <summary>
        /// Creates the content.
        /// </summary>
        /// <param name="node">The node.</param>
        public void CreateMainContent(XElement node)
        {
            try
            {
                foreach (XElement nodeChild in node.Elements())
                {
                    IContent iContent = CreateContent(new XElement(nodeChild));

                    if (iContent != null)
                        AddToCollection(iContent, _document.Content);
                        //this._document.Content.Add(iContent);
                    else
                    {
                        OnWarning(new AODLWarning("A couldn't create any content from an an first level node!.",
                                                  nodeChild));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while processing a content node.", ex);
            }
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="node">The node to clone and create content from.</param>
        /// <returns></returns>
        public IContent CreateContent(XNode node)
        {
            try
            {
                if (node is XElement)
                {
                    XElement element = (XElement) node;
                    if (element.Name == Ns.Text + "p")
                        return CreateParagraph(new XElement(element));
                    if (element.Name == Ns.Text + "list")
                        return CreateList(new XElement(element));
                    if (element.Name == Ns.Text + "list-item")
                        return CreateListItem(new XElement(element));
                    if (element.Name == Ns.Table + "table")
                        return CreateTable(new XElement(element));
                    if (element.Name == Ns.Table + "table-column")
                        return CreateTableColumn(new XElement(element));
                    if (element.Name == Ns.Table + "table-row")
                        return CreateTableRow(new XElement(element));
                    if (element.Name == Ns.Table + "table-header-rows")
                        return CreateTableHeaderRow(new XElement(element));
                    if (element.Name == Ns.Table + "table-cell")
                        return CreateTableCell(new XElement(element));
                    if (element.Name == Ns.Table + "covered-table-cell")
                        return CreateTableCellSpan(new XElement(element));
                    if (element.Name == Ns.Text + "h")
                        return CreateHeader(new XElement(element));
                    if (element.Name == Ns.Text + "table-of-content")
                        //Possible?
                        return CreateTableOfContents(new XElement(element));
                    if (element.Name == Ns.Draw + "frame")
                        return CreateFrame(new XElement(element));
                    if (element.Name == Ns.Draw + "object")
                        return CreateEmbedObject(new XElement(element));
                    if (element.Name == Ns.Draw + "text-box")
                        return CreateDrawTextBox(new XElement(element));
                    if (element.Name == Ns.Draw + "image")
                        return CreateGraphic(new XElement(element));
                    //@Liu Yuhua: What's that??? This is of course a image and not unknown!! Lars
                    //return new UnknownContent(this._document, new XElement(element));
                    if (element.Name == Ns.Draw + "area-rectangle")
                        return CreateDrawAreaRectangle(new XElement(element));
                    if (element.Name == Ns.Draw + "area-circle")
                        return CreateDrawAreaCircle(new XElement(element));
                    if (element.Name == Ns.Draw + "image-map")
                        return CreateImageMap(new XElement(element));
                    if (element.Name == Ns.Office + "event-listeners")
                        return CreateEventListeners(new XElement(element));
                    if (element.Name == Ns.Script + "event-listener")
                        return CreateEventListeners(new XElement(element));

                    if (element.Name == Ns.Draw + "control")
                        return CreateControlRef(new XElement(element));
                    return new UnknownContent(_document, new XElement(element));
                }
                return new SimpleText(_document, ((XText) node).Value);
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while processing a content node.", ex);
            }
        }

        /// <summary>
        /// Creates the table of contents.
        /// </summary>
        /// <param name="tocNode">The toc node.</param>
        /// <returns></returns>
        private TableOfContents CreateTableOfContents(XElement tocNode)
        {
            try
            {
                if (_document is TextDocument)
                {
                    //Create the TableOfContents object
                    TableOfContents tableOfContents = new TableOfContents(
                        (_document), tocNode);
                    //Recieve the Section style
                    IStyle sectionStyle = _document.Styles.GetStyleByName(tableOfContents.StyleName);

                    if (sectionStyle != null)
                        tableOfContents.Style = sectionStyle;
                    else
                    {
                        OnWarning(new AODLWarning("A SectionStyle for the TableOfContents object wasn't found.", tocNode));
                    }

                    //Create the text entries
                    IEnumerable<XElement> paragraphNodeList =
                        tocNode.Elements(Ns.Text + "index-body").Elements(Ns.Text + "p");
                    XElement indexBodyNode = tocNode.Element(Ns.Text + "index-body");
                    tableOfContents.IndexBodyNode = indexBodyNode;
                    ContentCollection pCollection = new ContentCollection();

                    foreach (XElement paragraphnode in paragraphNodeList)
                    {
                        Paragraph paragraph = CreateParagraph(paragraphnode);
                        if (indexBodyNode != null)
                            paragraphnode.Remove();
                        //pCollection.Add(paragraph);
                        AddToCollection(paragraph, pCollection);
                    }

                    foreach (IContent content in pCollection)
                        AddToCollection(content, tableOfContents.Content);
                    //tableOfContents.Content.Add(content);

                    return tableOfContents;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a TableOfContents.", ex);
            }
        }

        /// <summary>
        /// Creates the paragraph.
        /// </summary>
        /// <param name="paragraphNode">The paragraph node.</param>
        public Paragraph CreateParagraph(XElement paragraphNode)
        {
            try
            {
                //Create a new Paragraph
                Paragraph paragraph = new Paragraph(paragraphNode, _document);
                //Recieve the ParagraphStyle
                IStyle paragraphStyle = _document.Styles.GetStyleByName(paragraph.StyleName);

                if (paragraphStyle != null)
                {
                    paragraph.Style = paragraphStyle;
                }
                else if (paragraph.StyleName != "Standard"
                         && paragraph.StyleName != "Table_20_Contents"
                         && paragraph.StyleName != "Text_20_body"
                         && _document is TextDocument)
                {
                    //Check if it's a user defined style
                    IStyle commonStyle = _document.CommonStyles.GetStyleByName(paragraph.StyleName);
                    if (commonStyle == null)
                    {
                        OnWarning(new AODLWarning(string.Format(
                                                      "A ParagraphStyle '{0}' wasn't found.", paragraph.StyleName),
                                                  paragraph.Node));
                    }
                }

                return ReadParagraphTextContent(paragraph);
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Paragraph.", ex);
            }
        }

        /// <summary>
        /// Reads the content of the paragraph text.
        /// </summary>
        /// <param name="paragraph">The paragraph.</param>
        /// <returns></returns>
        private Paragraph ReadParagraphTextContent(Paragraph paragraph)
        {
            try
            {
                if (_debugMode)
                    LogNode(paragraph.Node, "Log Paragraph node before");

                IList<IContent> mixedContent = new List<IContent>();
                foreach (XNode nodeChild in paragraph.Node.Nodes())
                {
                    //Check for IText content first
                    TextContentProcessor tcp = new TextContentProcessor();
                    IText iText = tcp.CreateTextObject(_document, nodeChild);

                    if (iText != null)
                        mixedContent.Add(iText);
                    else
                    {
                        //Check against IContent
                        IContent iContent = CreateContent(nodeChild);

                        if (iContent != null)
                            mixedContent.Add(iContent);
                    }
                }

                //Remove all
                paragraph.Node.Value = "";

                foreach (IContent ob in mixedContent)
                {
                    if (ob is IText)
                    {
                        if (_debugMode)
                            LogNode(ob.Node, "Log IText node read");
                        paragraph.TextContent.Add(ob as IText);
                    }
                    else
                    {
                        if (_debugMode)
                            LogNode(ob.Node, "Log IContent node read");
                        //paragraph.Content.Add(ob as IContent);
                        AddToCollection(ob, paragraph.Content);
                    }
                }

                if (_debugMode)
                    LogNode(paragraph.Node, "Log Paragraph node after");

                return paragraph;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create the Paragraph content.", ex);
            }
        }

        /// <summary>
        /// Creates the header.
        /// </summary>
        /// <param name="headernode">The headernode.</param>
        /// <returns></returns>
        public Header CreateHeader(XElement headernode)
        {
            try
            {
                if (_debugMode)
                    LogNode(headernode, "Log header node before");

                //Create a new Header
                Header header = new Header(headernode, _document);
                //Create a ITextCollection
                ITextCollection textColl = new ITextCollection();
                //Recieve the HeaderStyle
                IStyle headerStyle = _document.Styles.GetStyleByName(header.StyleName);

                if (headerStyle != null)
                    header.Style = headerStyle;

                //Create the IText content
                foreach (XNode nodeChild in header.Node.Nodes())
                {
                    TextContentProcessor tcp = new TextContentProcessor();
                    IText iText = tcp.CreateTextObject(_document, nodeChild);

                    if (iText != null)
                        textColl.Add(iText);
                    else
                    {
                        OnWarning(new AODLWarning("Couldn't create IText object from header child node!.", nodeChild));
                    }
                }

                //Remove all
                header.Node.Value = "";

                foreach (IText iText in textColl)
                {
                    if (_debugMode)
                        LogNode(iText.Node, "Log IText node read from header");
                    header.TextContent.Add(iText);
                }

                return header;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Header.", ex);
            }
        }

        /// <summary>
        /// Creates the graphic.
        /// </summary>
        /// <param name="graphicnode">The graphicnode.</param>
        /// <returns>The Graphic object</returns>
        private Graphic CreateGraphic(XElement graphicnode)
        {
            try
            {
                Graphic graphic = new Graphic(_document, null, null) {Node = graphicnode};
                graphic.GraphicFile = _document.Importer.GetFile(graphic.HRef);

                return graphic;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Graphic.", ex);
            }
        }

        /// <summary>
        /// Creates the draw text box.
        /// </summary>
        /// <param name="drawTextBoxNode">The draw text box node.</param>
        /// <returns></returns>
        private DrawTextBox CreateDrawTextBox(XElement drawTextBoxNode)
        {
            try
            {
                DrawTextBox drawTextBox = new DrawTextBox(_document, drawTextBoxNode);
                ContentCollection iColl = new ContentCollection();

                foreach (XNode nodeChild in drawTextBox.Node.Nodes())
                {
                    IContent iContent = CreateContent(nodeChild);
                    if (iContent != null)
                        //iColl.Add(iContent);
                        AddToCollection(iContent, iColl);
                    else
                    {
                        OnWarning(new AODLWarning("Couldn't create a IContent object for a DrawTextBox.", nodeChild));
                    }
                }

                drawTextBox.Node.Value = "";

                foreach (IContent iContent in iColl)
                    AddToCollection(iContent, drawTextBox.Content);
                //drawTextBox.Content.Add(iContent);

                return drawTextBox;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Graphic.", ex);
            }
        }

        /// <summary>
        /// Creates the draw area rectangle.
        /// </summary>
        /// <param name="drawAreaRectangleNode">The draw area rectangle node.</param>
        /// <returns></returns>
        private DrawAreaRectangle CreateDrawAreaRectangle(XElement drawAreaRectangleNode)
        {
            try
            {
                DrawAreaRectangle dAreaRec = new DrawAreaRectangle(_document, drawAreaRectangleNode);
                ContentCollection iCol = new ContentCollection();

                if (dAreaRec.Node != null)
                    foreach (XNode nodeChild in dAreaRec.Node.Nodes())
                    {
                        IContent iContent = CreateContent(nodeChild);
                        if (iContent != null)
                            AddToCollection(iContent, iCol);
                        //iCol.Add(iContent);
                    }

                dAreaRec.Node.Value = "";

                foreach (IContent iContent in iCol)
                    AddToCollection(iContent, dAreaRec.Content);

                //dAreaRec.Content.Add(iContent);

                return dAreaRec;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a DrawAreaRectangle.", ex);
            }
        }

        /// <summary>
        /// Creates the draw area circle.
        /// </summary>
        /// <param name="drawAreaCircleNode">The draw area circle node.</param>
        /// <returns></returns>
        private DrawAreaCircle CreateDrawAreaCircle(XElement drawAreaCircleNode)
        {
            try
            {
                DrawAreaCircle dAreaCirc = new DrawAreaCircle(_document, drawAreaCircleNode);
                ContentCollection iCol = new ContentCollection();

                if (dAreaCirc.Node != null)
                    foreach (XNode nodeChild in dAreaCirc.Node.Nodes())
                    {
                        IContent iContent = CreateContent(nodeChild);
                        if (iContent != null)
                            AddToCollection(iContent, iCol);
                        //iCol.Add(iContent);
                    }

                dAreaCirc.Node.Value = "";

                foreach (IContent iContent in iCol)
                    AddToCollection(iContent, dAreaCirc.Content);
                //dAreaCirc.Content.Add(iContent);

                return dAreaCirc;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a DrawAreaCircle.", ex);
            }
        }

        /// <summary>
        /// Creates the image map.
        /// </summary>
        /// <param name="imageMapNode">The image map node.</param>
        /// <returns></returns>
        private ImageMap CreateImageMap(XElement imageMapNode)
        {
            try
            {
                ImageMap imageMap = new ImageMap(_document, imageMapNode);
                ContentCollection iCol = new ContentCollection();

                if (imageMap.Node != null)
                    foreach (XNode nodeChild in imageMap.Node.Nodes())
                    {
                        IContent iContent = CreateContent(nodeChild);
                        if (iContent != null)
                            AddToCollection(iContent, iCol);
                        //iCol.Add(iContent);
                    }

                imageMap.Node.Value = "";

                foreach (IContent iContent in iCol)
                    AddToCollection(iContent, imageMap.Content);
                //imageMap.Content.Add(iContent);

                return imageMap;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a ImageMap.", ex);
            }
        }

        /// <summary>
        /// Creates the event listener.
        /// </summary>
        /// <param name="eventListenerNode">The event listener node.</param>
        /// <returns></returns>
        public EventListener CreateEventListener(XElement eventListenerNode)
        {
            try
            {
                EventListener eventListener = new EventListener(_document, eventListenerNode);

                return eventListener;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a EventListener.", ex);
            }
        }

        /// <summary>
        /// Creates the event listeners.
        /// </summary>
        /// <param name="eventListenersNode">The event listeners node.</param>
        /// <returns></returns>
        public EventListeners CreateEventListeners(XElement eventListenersNode)
        {
            try
            {
                EventListeners eventList = new EventListeners(_document, eventListenersNode);
                ContentCollection iCol = new ContentCollection();

                if (eventList.Node != null)
                    foreach (XNode nodeChild in eventList.Node.Nodes())
                    {
                        IContent iContent = CreateContent(nodeChild);
                        if (iContent != null)
                            AddToCollection(iContent, iCol);
                        //iCol.Add(iContent);
                    }

                eventList.Node.Value = "";

                foreach (IContent iContent in iCol)
                    AddToCollection(iContent, eventList.Content);
                //eventList.Content.Add(iContent);

                return eventList;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a ImageMap.", ex);
            }
        }


        /// <summary>
        /// Creates the frame.
        /// </summary>
        /// <param name="refNode">The framenode.</param>
        /// <returns>The Frame object.</returns>
        public ODFControlRef CreateControlRef(XElement refNode)
        {
            try
            {
                ODFControlRef controlRef = new ODFControlRef(_document, refNode);
                return controlRef;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Control Reference.", ex);
            }
        }


        /// <summary>
        /// Creates the frame.
        /// </summary>
        /// <param name="frameNode">The framenode.</param>
        /// <returns>The Frame object.</returns>
        public Frame CreateFrame(XElement frameNode)
        {
            try
            {
                #region Old code Todo: delete

                //				Frame frame					= null;
                //				XElement graphicnode			= null;
                //				XElement graphicproperties	= null;
                //				string realgraphicname		= "";
                //				string stylename			= "";
                //				stylename					= this.GetStyleName(framenode.OuterXml);
                //				XElement stylenode			= this.GetAStyleNode("style:style", stylename);
                //				realgraphicname				= this.GetAValueFromAnAttribute(framenode, "@draw:name");
                //
                //				//Console.WriteLine("frame: {0}", framenode.OuterXml);
                //
                //				//Up to now, the only sopported, inner content of a frame is a graphic
                //				if (framenode.Elements().Count > 0)
                //					if (framenode.Elements().Item(0).OuterXml.StartsWith("<draw:image"))
                //						graphicnode			= framenode.Elements().Item(0).CloneNode(true);
                //
                //				//If not graphic, it could be text-box, ole or something else
                //				//try to find graphic frame inside
                //				if (graphicnode == null)
                //				{
                //					XElement child		= framenode.SelectSingleNode("//draw:frame", this._document.NamespaceManager);
                //					if (child != null)
                //						frame		= this.CreateFrame(child);
                //					return frame;
                //				}
                //
                //				string graphicpath			= this.GetAValueFromAnAttribute(graphicnode, "@xlink:href");
                //
                //				if (stylenode != null)
                //					if (stylenode.Elements().Count > 0)
                //						if (stylenode.Elements().Item(0).OuterXml.StartsWith("<style:graphic-properties"))
                //							graphicproperties	= stylenode.Elements().Item(0).CloneNode(true);
                //
                //				if (stylename.Length > 0 && stylenode != null && realgraphicname.Length > 0
                //					&& graphicnode != null && graphicpath.Length > 0 && graphicproperties != null)
                //				{
                //					graphicpath				= graphicpath.Replace("Pictures", "");
                //					graphicpath				= OpenDocumentTextImporter.dirpics+graphicpath.Replace("/", @"\");
                //
                //					frame					= new Frame(this._document, stylename,
                //												realgraphicname, graphicpath);
                //
                //					frame.Style.Node		= stylenode;
                //					frame.Graphic.Node		= graphicnode;
                //					((FrameStyle)frame.Style).GraphicProperties.Node = graphicproperties;
                //
                //					XElement nodeSize		= framenode.SelectSingleNode("@svg:height",
                //						this._document.NamespaceManager);
                //
                //					if (nodeSize != null)
                //						if (nodeSize.InnerText != null)
                //							frame.GraphicHeight	= nodeSize.InnerText;
                //
                //					nodeSize		= framenode.SelectSingleNode("@svg:width",
                //						this._document.NamespaceManager);
                //
                //					if (nodeSize != null)
                //						if (nodeSize.InnerText != null)
                //							frame.GraphicWidth	= nodeSize.InnerText;
                //				}

                #endregion

                //Create a new Frame
                Frame frame = new Frame(_document, null) {Node = frameNode};
                ContentCollection iColl = new ContentCollection();
                //Revieve the FrameStyle
                IStyle frameStyle = _document.Styles.GetStyleByName(frame.StyleName);

                if (frameStyle != null)
                    frame.Style = frameStyle;
                else
                {
                    OnWarning(new AODLWarning("Couldn't recieve a FrameStyle.", frameNode));
                }

                //Create the frame content
                foreach (XNode nodeChild in frame.Node.Nodes())
                {
                    IContent iContent = CreateContent(nodeChild);
                    if (iContent != null)
                        AddToCollection(iContent, iColl);
                        //iColl.Add(iContent);
                    else
                    {
                        OnWarning(new AODLWarning("Couldn't create a IContent object for a frame.", nodeChild));
                    }
                }

                frame.Node.Value = "";

                foreach (IContent iContent in iColl)
                {
                    AddToCollection(iContent, frame.Content);
                    //frame.Content.Add(iContent);
                    if (iContent is Graphic)
                    {
                        LoadFrameGraphic(frame, iContent as Graphic);
                    }

                    if (iContent is EmbedObject)
                    {
                        //(EmbedObject(iContent)).Frame  =frame;
                        (iContent as EmbedObject).Frame = frame;
                    }
                }
                return frame;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Frame.", ex);
            }
        }

        private void LoadFrameGraphic(Frame frame, Graphic content)
        {
            try
            {
                frame.LoadImageFromFile(content.GraphicFile);
            }
            catch (AODLGraphicException e)
            {
                OnWarning(
                    new AODLWarning("A couldn't create any content from an an first level node!.", content.Node, e));
            }
        }

        private EmbedObject CreateEmbedObject(XElement objNode)
        {
            try
            {
                XElement objectNode = new XElement(objNode);

                string href = (string) objectNode.Attribute(Ns.XLink + "href");

                string objectFullPath = href.Substring(2) + "/";

                string objectRealPath = href.Substring(2);

                string objectName = href.Substring(2);

                //ObjectRealPath                      = ObjectRealPath.Replace ("/","\\");


                string mediaType = GetMediaType(objectFullPath);

                EmbedObjectHandler embedobjhandler = new EmbedObjectHandler(_document);

                return embedobjhandler.CreateEmbedObject(objectNode, mediaType, objectRealPath, objectName);
            }

            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Graphic.", ex);
            }
        }

        public string GetMediaType(string objectFullPath)
        {
            XDocument doc = ((SpreadsheetDocument) _document).DocumentManifest.Manifest;
            XElement node = doc.Element(Ns.Manifest + "manifest");

            foreach (XElement nodeChild in node.Elements())
            {
                //XElement Entry  = nodeChild.SelectSingleNode ("@manifest:file-entry",this._document.NamespaceManager);
                string fullPath = (string) nodeChild.Attribute(Ns.Manifest + "full-path");

                if (fullPath == objectFullPath)
                {
                    string mediaType = (string) nodeChild.Attribute(Ns.Manifest + "media-type");

                    if (!string.IsNullOrEmpty(mediaType))

                        return mediaType;
                }
            }

            return null;
        }

        /// <summary>
        /// Creates the list.
        /// </summary>
        /// <param name="listNode">The list node.</param>
        /// <returns>The List object</returns>
        private List CreateList(XElement listNode)
        {
            try
            {
                #region Old code Todo: delete

                //				string stylename				= null;
                //				XElement	stylenode				= null;
                //				ListStyles liststyles			= ListStyles.Bullet; //as default
                //				string paragraphstylename		= null;
                //
                //				if (outerlist == null)
                //				{
                //					stylename			= this.GetStyleName(listNode.OuterXml);
                //					stylenode			= this.GetAStyleNode("text:list-style", stylename);
                //					liststyles			= this.GetListStyle(listNode);
                //				}
                //				List list					= null;
                //
                //				if (listNode.Elements().Count > 0)
                //				{
                //					try
                //					{
                //						paragraphstylename	= this.GetAValueFromAnAttribute(listNode.Elements().Item(0).Elements().Item(0), "@style:style-name");
                //					}
                //					catch(Exception ex)
                //					{
                //						paragraphstylename	= "P1";
                //					}
                //				}

                #endregion

                //Create a new List
                List list = new List(_document, listNode);
                ContentCollection iColl = new ContentCollection();
                //Revieve the ListStyle
                IStyle listStyle = _document.Styles.GetStyleByName(list.StyleName);

                if (listStyle != null)
                    list.Style = listStyle;

                foreach (XNode nodeChild in list.Node.Nodes())
                {
                    IContent iContent = CreateContent(nodeChild);

                    if (iContent != null)
                        AddToCollection(iContent, iColl);
                    //iColl.Add(iContent);
                }

                list.Node.Value = "";

                foreach (IContent iContent in iColl)
                    AddToCollection(iContent, list.Content);
                //list.Content.Add(iContent);

                return list;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a List.", ex);
            }
        }

        /// <summary>
        /// Creates the list item.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private ListItem CreateListItem(XElement node)
        {
            try
            {
                ListItem listItem = new ListItem(_document);
                ContentCollection iColl = new ContentCollection();
                listItem.Node = node;

                foreach (XNode nodeChild in listItem.Node.Nodes())
                {
                    IContent iContent = CreateContent(nodeChild);
                    if (iContent != null)
                        AddToCollection(iContent, iColl);
                        //iColl.Add(iContent);
                    else
                    {
                        OnWarning(new AODLWarning("Couldn't create a IContent object for a ListItem.", nodeChild));
                    }
                }

                listItem.Node.Value = "";

                foreach (IContent iContent in iColl)
                    //listItem.Content.Add(iContent);
                    AddToCollection(iContent, listItem.Content);

                return listItem;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a ListItem.", ex);
            }
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="tableNode">The tablenode.</param>
        /// <returns></returns>
        private Table CreateTable(XElement tableNode)
        {
            try
            {
                //Create a new table
                Table table = new Table(_document, tableNode);
                ContentCollection iColl = new ContentCollection();
                //Recieve the table style
                IStyle tableStyle = _document.Styles.GetStyleByName(table.StyleName);

                if (tableStyle != null)
                    table.Style = tableStyle;
                else
                {
                    OnWarning(new AODLWarning("Couldn't recieve a TableStyle.", tableNode));
                }


                //Create the table content
                foreach (XNode nodeChild in table.Node.Nodes())
                {
                    IContent iContent = CreateContent(nodeChild);

                    if (iContent != null)
                    {
                        //iColl.Add(iContent);
                        AddToCollection(iContent, iColl);
                    }
                    else
                    {
                        OnWarning(
                            new AODLWarning(
                                "Couldn't create IContent from a table node. Content is unknown table content!",
                                iContent.Node));
                    }
                }

                table.Node.Value = "";

                foreach (IContent iContent in iColl)
                {
                    if (iContent is Column)
                    {
                        ((Column) iContent).Table = table;
                        table.ColumnCollection.Add(iContent as Column);
                    }
                    else if (iContent is Row)
                    {
                        ((Row) iContent).Table = table;
                        table.Rows.Add(iContent as Row);
                    }
                    else if (iContent is RowHeader)
                    {
                        ((RowHeader) iContent).Table = table;
                        table.RowHeader = iContent as RowHeader;
                    }
                    else
                    {
                        table.Node.Add(iContent.Node);
                        OnWarning(new AODLWarning("Couldn't create IContent from a table node.", tableNode));
                    }
                }

                return table;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Table.", ex);
            }
        }

        /// <summary>
        /// Creates the table row.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private Row CreateTableRow(XElement node)
        {
            try
            {
                //Create a new Row
                Row row = new Row(_document, node);
                ContentCollection iColl = new ContentCollection();
                //Recieve RowStyle
                IStyle rowStyle = _document.Styles.GetStyleByName(row.StyleName);

                if (rowStyle != null)
                    row.Style = rowStyle;
                //No need for a warning

                //Create the cells
                foreach (XElement nodeChild in row.Node.Elements())
                {
                    // Phil Jollans 24-March-2008
                    // Handle the attribute table:number-columns-repeated on cell nodes,
                    // by inserting multiple nodes. CreateContent clones the nodes so this
                    // seems fairly safe.
                    int iRepeatCount = 1;
                    XAttribute xn = nodeChild.Attribute(Ns.Table + "number-columns-repeated");
                    if (xn != null)
                    {
                        iRepeatCount = int.Parse(xn.Value);

                        // Inetrnally, the node is no longer repeated, so it seems correct
                        // to remove the the attribute table:number-columns-repeated.
                        xn.Remove();
                    }

                    for (int i = 0; i < iRepeatCount; i++)
                    {
                        IContent iContent = CreateContent(nodeChild);

                        if (iContent != null)
                        {
                            //iColl.Add(iContent);
                            AddToCollection(iContent, iColl);
                        }
                        else
                        {
                            OnWarning(new AODLWarning("Couldn't create IContent from a table row.", nodeChild));
                        }
                    }
                }

                row.Node.Value = "";

                foreach (IContent iContent in iColl)
                {
                    if (iContent is Cell)
                    {
                        ((Cell) iContent).Row = row;
                        row.Cells.Add(iContent as Cell);
                    }
                    else if (iContent is CellSpan)
                    {
                        ((CellSpan) iContent).Row = row;
                        row.CellSpanCollection.Add(iContent as CellSpan);
                    }
                    else
                    {
                        OnWarning(
                            new AODLWarning(
                                "Couldn't create IContent from a row node. Content is unknown table row content!",
                                iContent.Node));
                    }
                }

                return row;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Table Row.", ex);
            }
        }

        /// <summary>
        /// Creates the table header row.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private RowHeader CreateTableHeaderRow(XElement node)
        {
            try
            {
                //Create a new Row
                RowHeader rowHeader = new RowHeader(_document, node);
                ContentCollection iColl = new ContentCollection();
                //Recieve RowStyle
                IStyle rowStyle = _document.Styles.GetStyleByName(rowHeader.StyleName);

                if (rowStyle != null)
                    rowHeader.Style = rowStyle;
                //No need for a warning

                //Create the cells
                foreach (XNode nodeChild in rowHeader.Node.Nodes())
                {
                    IContent iContent = CreateContent(nodeChild);

                    if (iContent != null)
                    {
                        //iColl.Add(iContent);
                        AddToCollection(iContent, iColl);
                    }
                    else
                    {
                        OnWarning(new AODLWarning("Couldn't create IContent from a table row.", nodeChild));
                    }
                }

                rowHeader.Node.Value = "";

                foreach (IContent iContent in iColl)
                {
                    if (iContent is Row)
                    {
                        rowHeader.RowCollection.Add(iContent as Row);
                    }
                    else
                    {
                        OnWarning(
                            new AODLWarning(
                                "Couldn't create IContent from a row header node. Content is unknown table row header content!",
                                iContent.Node));
                    }
                }
                return rowHeader;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Table Row.", ex);
            }
        }

        /// <summary>
        /// Creates the table column.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private Column CreateTableColumn(XElement node)
        {
            try
            {
                //Create a new Row
                Column column = new Column(_document, node);
                //Recieve RowStyle
                IStyle columnStyle = _document.Styles.GetStyleByName(column.StyleName);

                if (columnStyle != null)
                    column.Style = columnStyle;
                //No need for a warning

                return column;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Table Column.", ex);
            }
        }

        /// <summary>
        /// Creates the table cell span.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private CellSpan CreateTableCellSpan(XElement node)
        {
            try
            {
                //Create a new CellSpan
                CellSpan cellSpan = new CellSpan(_document, node);

                //No need for a warnings or styles

                return cellSpan;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Table CellSpan.", ex);
            }
        }

        /// <summary>
        /// Creates the table cell.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns></returns>
        private Cell CreateTableCell(XElement node)
        {
            try
            {
                //Create a new Cel
                Cell cell = new Cell(_document, node);
                ContentCollection iColl = new ContentCollection();
                //Recieve CellStyle
                IStyle cellStyle = _document.Styles.GetStyleByName(cell.StyleName);

                if (cellStyle != null)
                {
                    cell.Style = cellStyle;
                }
                //No need for a warning

                //Create the cells content
                foreach (XNode nodeChild in cell.Node.Nodes())
                {
                    IContent iContent = CreateContent(nodeChild);

                    if (iContent != null)
                    {
                        //iColl.Add(iContent);
                        AddToCollection(iContent, iColl);
                    }
                    else
                    {
                        OnWarning(new AODLWarning("Couldn't create IContent from a table cell.", nodeChild));
                    }
                }

                cell.Node.Value = "";

                foreach (IContent iContent in iColl)
                    AddToCollection(iContent, cell.Content);
                //cell.Content.Add(iContent);
                return cell;
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while trying to create a Table Row.", ex);
            }
        }

        /// <summary>
        /// Logs the node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="msg">The MSG.</param>
        private static void LogNode(XNode node, string msg)
        {
            Console.WriteLine("\n#############################\n{0}", msg);
            using (XmlWriter writer = XmlWriter.Create(Console.Out, new XmlWriterSettings { Indent = true }))
            {
                node.WriteTo(writer);
            }
        }
    }
}

//AODLTest.DocumentImportTest.SimpleLoadTest : System.IO.DirectoryNotFoundException : Could not find a part of the path "D:\OpenDocument\AODL\AODLTest\bin\Debug\GeneratedFiles\OpenOffice.net.odt.rel.odt".
/*
 * $Log: MainContentProcessor.cs,v $
 * Revision 1.10  2008/04/29 15:39:52  mt
 * new copyright header
 *
 * Revision 1.9  2008/04/10 17:33:23  larsbehr
 * - Added several bug fixes mainly for the table handling which are submitted by Phil  Jollans
 *
 * Revision 1.8  2008/02/08 07:12:20  larsbehr
 * - added initial chart support
 * - several bug fixes
 *
 * Revision 1.3  2007/05/29 13:43:25  yegorov
 * Issue number:  1.2
 * Submitted by:  Oleg Yegorov
 * Reviewed by:   Oleg Yegorov
 *
 * Revision 1.2  2007/04/08 16:51:37  larsbehr
 * - finished master pages and styles for text documents
 * - several bug fixes
 *
 * Revision 1.1  2007/02/25 08:58:45  larsbehr
 * initial checkin, import from Sourceforge.net to OpenOffice.org
 *
 * Revision 1.6  2007/02/13 17:58:48  larsbm
 * - add first part of implementation of master style pages
 * - pdf exporter conversations for tables and images and added measurement helper
 *
 * Revision 1.5  2006/02/16 18:35:41  larsbm
 * - Add FrameBuilder class
 * - TextSequence implementation (Todo loading!)
 * - Free draing postioning via x and y coordinates
 * - Graphic will give access to it's full qualified path
 *   via the GraphicRealPath property
 * - Fixed Bug with CellSpan in Spreadsheetdocuments
 * - Fixed bug graphic of loaded files won't be deleted if they
 *   are removed from the content.
 * - Break-Before property for Paragraph properties for Page Break
 *
 * Revision 1.4  2006/02/05 20:03:32  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.3  2006/02/02 21:55:59  larsbm
 * - Added Clone object support for many AODL object types
 * - New Importer implementation PlainTextImporter and CsvImporter
 * - New tests
 *
 * Revision 1.2  2006/01/29 18:52:14  larsbm
 * - Added support for common styles (style templates in OpenOffice)
 * - Draw TextBox import and export
 * - DrawTextBox html export
 *
 * Revision 1.1  2006/01/29 11:28:23  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.5  2006/01/05 10:28:06  larsbm
 * - AODL merged cells
 * - AODL toc
 * - AODC batch mode, splash screen
 *
 * Revision 1.4  2005/12/21 17:17:12  larsbm
 * - AODL new feature save gui settings
 * - Bugfixes, in MainContentProcessor
 *
 * Revision 1.3  2005/12/18 18:29:46  larsbm
 * - AODC Gui redesign
 * - AODC HTML exporter refecatored
 * - Full Meta Data Support
 * - Increase textprocessing performance
 *
 * Revision 1.2  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 * Revision 1.1  2005/11/20 17:31:20  larsbm
 * - added suport for XLinks, TabStopStyles
 * - First experimental of loading dcuments
 * - load and save via importer and exporter interfaces
 *
 */