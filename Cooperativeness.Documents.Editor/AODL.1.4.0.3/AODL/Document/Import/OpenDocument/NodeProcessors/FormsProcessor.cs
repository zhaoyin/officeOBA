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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using AODL.Document.Content.Tables;
using AODL.Document.Exceptions;
using AODL.Document.Forms;
using AODL.Document.Forms.Controls;
using AODL.Document.SpreadsheetDocuments;
using AODL.Document.TextDocuments;

namespace AODL.Document.Import.OpenDocument.NodeProcessors
{
    public class FormsProcessor
    {
        #region Delegates

        /// <summary>
        /// Warning delegate
        /// </summary>
        public delegate void Warning(AODLWarning warning);

        #endregion

        /// <summary>
        /// If set to true all node content would be directed
        /// to Console.Out
        /// </summary>
        private const bool DEBUG_MODE = false;

        /// <summary>
        /// The textdocument
        /// </summary>
        private readonly IDocument _document;


        /// <summary>
        /// Initializes a new instance of the <see cref="FormsProcessor"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        public FormsProcessor(IDocument document)
        {
            _document = document;
        }

        /// <summary>
        /// Reads the content nodes.
        /// </summary>
        public void ReadFormNodes()
        {
            try
            {
                XElement node = null;
                if (_document is TextDocument)
                {
                    node = _document.XmlDoc
                        .Elements(Ns.Office + "document-content")
                        .Elements(Ns.Office + "body")
                        .Elements(Ns.Office + "text")
                        .FirstOrDefault();
                    (_document as TextDocument).Forms = new ODFFormCollection();
                }
                else if (_document is SpreadsheetDocument)
                    node = _document.XmlDoc
                        .Elements(Ns.Office + "document-content")
                        .Elements(Ns.Office + "body")
                        .Elements(Ns.Office + "spreadsheet")
                        .FirstOrDefault();

                if (node != null)
                {
                    CreateDocumentForms(node);
                }
                else
                {
                    throw new AODLException("Unknown content type.");
                }
            }
            catch (Exception ex)
            {
                throw new AODLException("Error while trying to load the content file!", ex);
            }
        }

        /// <summary>
        /// Creates the document forms.
        /// </summary>
        /// <param name="table">The table.</param>
        public void CreateCellForms(Table table)
        {
            /// TODO: ADD IMPLEMENTATION!

            try
            {
                XElement nodeOfficeForms = table.Node.Element(Ns.Office + "forms");

                if (nodeOfficeForms != null)
                {
                    if (_document is SpreadsheetDocument)
                    {
                        foreach (XElement nodeChild in nodeOfficeForms.Elements())
                        {
                            ODFForm f = CreateForm(nodeChild);
                            table.Forms.Add(f);
                        }
                    }
                    nodeOfficeForms.RemoveAll();
                }
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while processing forms.", ex);
            }
        }

        /// <summary>
        /// Creates the document forms.
        /// </summary>
        /// <param name="node">The node.</param>
        private void CreateDocumentForms(XContainer node)
        {
            /// TODO: ADD IMPLEMENTATION!

            try
            {
                XElement nodeOfficeForms = node.Element(Ns.Office + "forms");

                if (nodeOfficeForms != null)
                {
                    if (_document is TextDocument)
                    {
                        foreach (XElement nodeChild in nodeOfficeForms.Elements())
                        {
                            ODFForm f = CreateForm(nodeChild);
                            (_document as TextDocument).Forms.Add(f);
                        }
                    }
                    nodeOfficeForms.RemoveAll();
                }
            }
            catch (Exception ex)
            {
                throw new AODLException("Exception while processing forms.", ex);
            }
        }

        /// <summary>
        /// Gets the form.
        /// </summary>
        /// <param name="formnode">The node of the form.</param>
        /// <returns></returns>
        private ODFForm CreateForm(XElement formnode)
        {
            ODFForm form = null;
            try
            {
                if (formnode.Name == Ns.Form + "form")
                {
                    if (DEBUG_MODE)
                        LogNode(formnode, "Log form node before");

                    //Create a new ODFForm

                    ///////////TODO. Fix for child forms!
                    form = new ODFForm(new XElement(formnode), _document);

                    form.SuppressControlEvents();
                    foreach (XElement nodeChild in form.Node.Elements())
                    {
                        if (nodeChild.Name.Namespace != Ns.Form)
                        {
                            if (nodeChild.Parent == form.Node)
                            {
                                ODFGenericControl gc = new ODFGenericControl(form, nodeChild);
                                gc.FixPropertyCollection();
                                form.Controls.Add(gc);
                            }
                        }
                        else
                        {
                            switch (nodeChild.Name.LocalName)
                            {
                                case "form":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFForm frmchild = CreateForm(nodeChild);
                                        if (frmchild != null)
                                        {
                                            form.ChildForms.Add(frmchild);
                                        }
                                        nodeChild.Remove();
                                    }
                                    break;
                                case "properties":
                                    break;

                                case "button":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFButton button = new ODFButton(form, nodeChild);
                                        button.FixPropertyCollection();
                                        form.Controls.Add(button);
                                    }
                                    break;

                                case "listbox":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFListBox listbox = new ODFListBox(form, nodeChild);
                                        listbox.FixPropertyCollection();
                                        listbox.FixOptionCollection();
                                        form.Controls.Add(listbox);
                                    }
                                    break;

                                case "combobox":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFComboBox combobox = new ODFComboBox(form, nodeChild);
                                        combobox.FixPropertyCollection();
                                        combobox.FixItemCollection();
                                        form.Controls.Add(combobox);
                                    }
                                    break;

                                case "textarea":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFTextArea text = new ODFTextArea(form, nodeChild);
                                        text.FixPropertyCollection();
                                        form.Controls.Add(text);
                                    }
                                    break;

                                case "frame":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFFrame frm = new ODFFrame(form, nodeChild);
                                        frm.FixPropertyCollection();
                                        form.Controls.Add(frm);
                                    }
                                    break;

                                case "file":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFFile file = new ODFFile(form, nodeChild);
                                        file.FixPropertyCollection();
                                        form.Controls.Add(file);
                                    }
                                    break;

                                case "hidden":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFHidden hidden = new ODFHidden(form, nodeChild);
                                        hidden.FixPropertyCollection();
                                        form.Controls.Add(hidden);
                                    }
                                    break;

                                case "checkbox":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFCheckBox cb = new ODFCheckBox(form, nodeChild);
                                        cb.FixPropertyCollection();
                                        form.Controls.Add(cb);
                                    }
                                    break;

                                case "radio":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFRadioButton rb = new ODFRadioButton(form, nodeChild);
                                        rb.FixPropertyCollection();
                                        form.Controls.Add(rb);
                                    }
                                    break;

                                case "formatted-text":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFFormattedText text = new ODFFormattedText(form, nodeChild);
                                        text.FixPropertyCollection();
                                        form.Controls.Add(text);
                                    }
                                    break;
                                case "value-range":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFValueRange vr = new ODFValueRange(form, nodeChild);
                                        vr.FixPropertyCollection();
                                        form.Controls.Add(vr);
                                    }
                                    break;
                                case "image":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFImage img = new ODFImage(form, nodeChild);
                                        img.FixPropertyCollection();
                                        form.Controls.Add(img);
                                    }
                                    break;
                                case "image-frame":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFImageFrame imgf = new ODFImageFrame(form, nodeChild);
                                        imgf.FixPropertyCollection();
                                        form.Controls.Add(imgf);
                                    }
                                    break;
                                case "grid":
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFGrid gr = new ODFGrid(form, nodeChild);
                                        gr.FixPropertyCollection();
                                        gr.FixColumnCollection();
                                        form.Controls.Add(gr);
                                    }
                                    break;
                                default:
                                    if (nodeChild.Parent == form.Node)
                                    {
                                        ODFGenericControl gc = new ODFGenericControl(form, nodeChild);
                                        gc.FixPropertyCollection();
                                        form.Controls.Add(gc);
                                    }
                                    break;
                            }
                        }
                    }

                    form.RestoreControlEvents();
                    form.FixPropertyCollection();
                    //	formnode.RemoveAll();
                    //	formnode.InnerText = "";
                }
            }


            catch (Exception ex)
            {
                throw new AODLException("Exception while processing a form:form node.", ex);
            }
            return form;
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