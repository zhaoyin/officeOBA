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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AODL.Document.Styles;

namespace AODL.Document.Content.Text.TextControl
{
    /// <summary>
    /// WhiteSpace represent a white space element.
    /// </summary>
    public class WhiteSpace : IText
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WhiteSpace"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="node">The node.</param>
        public WhiteSpace(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WhiteSpace"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="whiteSpacesCount">The document.</param>
        public WhiteSpace(IDocument document, int whiteSpacesCount)
        {
            Document = document;
            Node = new XElement(Ns.Text + "s");
            Count = whiteSpacesCount.ToString();
        }

        #region IText Member

        /// <summary>
        /// The node that represent the text content.
        /// </summary>
        /// <value></value>
        public XElement Node { get; set; }

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
        /// A tab stop doesn't have a text.
        /// </summary>
        /// <value></value>
        public string Text
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// The document to which this text content belongs to.
        /// </summary>
        /// <value></value>
        public IDocument Document { get; set; }

        /// <summary>
        /// Is null no style is available.
        /// </summary>
        /// <value></value>
        public IStyle Style
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// No style name available
        /// </summary>
        /// <value></value>
        public string StyleName
        {
            get { return null; }
            set { }
        }

        #endregion

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        /// <value>The count.</value>
        public string Count
        {
            get { return (string) Node.Attribute(Ns.Text + "c"); }
            set { Node.SetAttributeValue(Ns.Text + "c", value); }
        }
    }


    /// <summary>
    /// WhiteSpace convesation
    /// </summary>
    public class WhiteSpaceHelper
    {
        /// <summary>
        /// Replace with the real Xml textnode &gt;text:s text:c="2" /&lt;
        /// </summary>
        private string _replacement;

        /// <summary>
        /// Value -> \ws3 which means 3 whitespace
        /// </summary>
        private string _value;

        /// <summary>
        /// Convert all whitespace groups "    "
        /// into OpenDocument Xml textnodes
        /// </summary>
        /// <param name="stringToConvert">The string to convert.</param>
        /// <returns>The parsed string</returns>
        public static string GetWhiteSpaceXml(string stringToConvert)
        {
            try
            {
                IList<WhiteSpaceHelper> matchList = new List<WhiteSpaceHelper>();
                const string pat = @"\s{2,}";
                Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                Match m = r.Match(stringToConvert);

                while (m.Success)
                {
                    WhiteSpaceHelper w = new WhiteSpaceHelper();
                    for (int i = 0; i < m.Length; i++)
                        w._value += " ";
                    w._replacement = string.Format("<ws id=\"{0}\"/>", m.Length); //GetXmlWhiteSpace(m.Length);
                    matchList.Add(w);
                    m = m.NextMatch();
                }

                foreach (WhiteSpaceHelper w in matchList)
                    stringToConvert = stringToConvert.Replace(w._value, w._replacement);
            }
            catch (Exception)
            {
                //unhandled, only whitespaces arent displayed correct
            }
            return stringToConvert;
        }

        /// <summary>
        /// Convert all AODL whitespace control character \ws3
        /// into their OpenDocument Xml textnodes
        /// </summary>
        /// <param name="text">The string to convert.</param>
        /// <returns>The parsed string</returns>
        public static string GetWhiteSpaceHtml(string text)
        {
            try
            {
                IList<WhiteSpaceHelper> matchList = new List<WhiteSpaceHelper>();
                const string pat =
                    "<text:s text:c=\"\\d+\" xmlns:text=\"urn:oasis:names:tc:opendocument:xmlns:text:1.0\" />";
                Regex r = new Regex(pat, RegexOptions.IgnoreCase);
                Match m = r.Match(text);

                while (m.Success)
                {
                    Regex r1 = new Regex(@"\d", RegexOptions.IgnoreCase);
                    Match m1 = r1.Match(m.Value);
                    string html = "";

                    while (m1.Success)
                    {
                        int cnt = Convert.ToInt32(m1.Value);
                        for (int i = 0; i < cnt; i++)
                            html += "&nbsp;";
                        //Console.WriteLine(html);
                        break;
                    }
                    if (html.Length > 0)
                    {
                        WhiteSpaceHelper w = new WhiteSpaceHelper {_value = html, _replacement = m.Value};
                        matchList.Add(w);
                    }
                    m = m.NextMatch();
                }

                foreach (WhiteSpaceHelper ws in matchList)
                    text = text.Replace(ws._replacement, ws._value);
            }
            catch (Exception)
            {
                //unhandled, only whitespaces arent displayed correct
            }
            return text;
        }
    }
}

/*
 * $Log: WhiteSpace.cs,v $
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
 * Revision 1.2  2006/02/05 20:02:25  larsbm
 * - Fixed several bugs
 * - clean up some messy code
 *
 * Revision 1.1  2006/01/29 11:28:22  larsbm
 * - Changes for the new version. 1.2. see next changelog for details
 *
 * Revision 1.2  2005/12/18 18:29:46  larsbm
 * - AODC Gui redesign
 * - AODC HTML exporter refecatored
 * - Full Meta Data Support
 * - Increase textprocessing performance
 *
 * Revision 1.1  2005/12/12 19:39:17  larsbm
 * - Added Paragraph Header
 * - Added Table Row Header
 * - Fixed some bugs
 * - better whitespace handling
 * - Implmemenation of HTML Exporter
 *
 */