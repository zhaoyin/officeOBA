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
using AODL.Document.Styles.Properties;

namespace AODL.Document.Styles
{
    /// <summary>
    /// Summary description for LegendStyle.
    /// </summary>
    public class LegendStyle : AbstractStyle
    {
        /// <summary>
        /// the constructor of the legend style
        /// </summary>
        /// <param name="document"></param>
        public LegendStyle(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Style + "style");
            InitStandards();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStyle"/> class.
        /// </summary>
        /// <param name="document">The spreadsheet document.</param>
        /// <param name="styleName">Name of the style.</param>
        public LegendStyle(IDocument document, string styleName)
        {
            Document = document;
            Node = new XElement(Ns.Style + "style");
            InitStandards();
            StyleName = styleName;
        }

        /// <summary>
        /// Gets or sets the chart graphic properties.
        /// </summary>
        /// <value>The graphic properties.</value>
        public ChartGraphicProperties ChartGraphicProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is ChartGraphicProperties)
                        return (ChartGraphicProperties) property;
                ChartGraphicProperties chartgraphicProperties = new ChartGraphicProperties(this);
                PropertyCollection.Add(chartgraphicProperties);
                return chartgraphicProperties;
            }
            set
            {
                if (PropertyCollection.Contains(value))
                    PropertyCollection.Remove(value);
                PropertyCollection.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the text properties.
        /// </summary>
        /// <value>The text properties.</value>
        public TextProperties TextProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is TextProperties)
                        return (TextProperties) property;
                TextProperties textProperties = new TextProperties(this);
                PropertyCollection.Add(textProperties);
                return TextProperties;
            }
            set
            {
                if (PropertyCollection.Contains(value))
                    PropertyCollection.Remove(value);
                PropertyCollection.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the family style.
        /// </summary>
        /// <value>The family style.</value>
        public string FamilyStyle
        {
            get { return (string) Node.Attribute(Ns.Style + "family"); }
            set { Node.SetAttributeValue(Ns.Style + "family", value); }
        }

        /// <summary>
        /// Inits the standards.
        /// </summary>
        private void InitStandards()
        {
            PropertyCollection = new IPropertyCollection();
            PropertyCollection.Inserted += PropertyCollection_Inserted;
            PropertyCollection.Removed += PropertyCollection_Removed;
            FamilyStyle = "chart";
            //			this.Document.Styles.Add(this);
        }

        /// <summary>
        /// Properties the collection_ inserted.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private void PropertyCollection_Inserted(int index, object value)
        {
            Node.Add(((IProperty) value).Node);
        }

        /// <summary>
        /// Properties the collection_ removed.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        private static void PropertyCollection_Removed(int index, object value)
        {
            ((IProperty) value).Node.Remove();
        }
    }
}