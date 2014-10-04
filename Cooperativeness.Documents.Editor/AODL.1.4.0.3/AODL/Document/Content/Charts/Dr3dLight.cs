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
using AODL.Document.Styles;

namespace AODL.Document.Content.Charts
{
    /// <summary>
    /// Summary description for Dr3dLight.
    /// </summary>
    public class Dr3dLight : IContent
    {
        private IStyle _style;

        /// <summary>
        /// the constructor of the dr3dlight
        /// </summary>
        /// <param name="chart"></param>
        public Dr3dLight(Chart chart)
        {
            Chart = chart;
            Node = new XElement(Ns.Dr3d + "light");
        }

        public Dr3dLight(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
        }

        public Chart Chart { get; set; }

        public ChartPlotArea PlotArea { get; set; }

        /// <summary>
        /// gets and sets the diffusecolor
        /// </summary>
        public string DiffuseColor
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "diffuse-color"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "diffuse-color", value); }
        }

        /// <summary>
        /// gets and sets the direction
        /// </summary>
        public string Direction
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "direction"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "direction", value); }
        }

        /// <summary>
        /// gets and sets the enabled
        /// </summary>
        public string Enabled
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "enabled"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "enabled", value); }
        }

        /// <summary>
        /// gets and sets the specular
        /// </summary>
        public string Specular
        {
            get { return (string) Node.Attribute(Ns.Dr3d + "specular"); }
            set { Node.SetAttributeValue(Ns.Dr3d + "specular", value); }
        }

        public XElement Node { get; set; }

        #region IContent Members

        /// <summary>
        /// gets and sets the name of the style
        /// </summary>
        public string StyleName
        {
            get { return (string) Node.Attribute(Ns.Chart + "style-name"); }
            set { Node.SetAttributeValue(Ns.Chart + "style-name", value); }
        }

        /// <summary>
        /// A Style class wich is referenced with the content object.
        /// If no style is available this is null.
        /// </summary>
        /// <value></value>
        public IStyle Style
        {
            get { return _style; }
            set
            {
                StyleName = value.StyleName;
                _style = value;
            }
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

        public IDocument Document { get; set; }

        #endregion
    }
}