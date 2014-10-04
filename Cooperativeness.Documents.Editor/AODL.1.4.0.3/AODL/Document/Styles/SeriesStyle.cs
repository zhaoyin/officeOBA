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
using AODL.Document.Styles.Properties;

namespace AODL.Document.Content.Charts
{
    /// <summary>
    /// Summary description for SeriesStyle.
    /// </summary>
    public class SeriesStyle : AbstractStyle
    {
        /// <summary>
        /// the constructor of the series style
        /// </summary>
        /// <param name="document"></param>
        /// <param name="node"></param>
        public SeriesStyle(IDocument document, XElement node)
        {
            Document = document;
            Node = node;
            InitStandards();
        }

        public SeriesStyle(IDocument document)
        {
            Document = document;
            Node = new XElement(Ns.Style + "style");
            InitStandards();
        }

        public SeriesStyle(IDocument document, string styleName)
        {
            Document = document;
            Node = new XElement(Ns.Style + "style");
            InitStandards();
            StyleName = styleName;
        }

        /// <summary>
        /// gets and sets chart graphic properties
        /// </summary>
        public ChartGraphicProperties ChartGraphicProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is ChartGraphicProperties)
                        return (ChartGraphicProperties) property;
                ChartGraphicProperties chartgraphicProperties = new ChartGraphicProperties(this);
                PropertyCollection.Add(chartgraphicProperties);
                return ChartGraphicProperties;
            }
            set
            {
                if (PropertyCollection.Contains(value))
                    PropertyCollection.Remove(value);
                PropertyCollection.Add(value);
            }
        }

        /// <summary>
        /// gets and sets the text properties
        /// </summary>
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
        /// gets and sets chart properties
        /// </summary>
        public ChartProperties ChartProperties
        {
            get
            {
                foreach (IProperty property in PropertyCollection)
                    if (property is ChartProperties)
                        return (ChartProperties) property;
                ChartProperties chartProperties = new ChartProperties(this);
                PropertyCollection.Add(chartProperties);
                return ChartProperties;
            }
            set
            {
                if (PropertyCollection.Contains(value))
                    PropertyCollection.Remove(value);
                PropertyCollection.Add(value);
            }
        }

        /// <summary>
        /// gets and sets the family style
        /// </summary>
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