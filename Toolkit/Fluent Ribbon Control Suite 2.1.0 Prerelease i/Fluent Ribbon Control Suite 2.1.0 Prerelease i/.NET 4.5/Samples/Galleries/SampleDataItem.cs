#region Copyright and License Information

// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg., Weegen Patrick 2009-2013.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license

#endregion

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fluent.Sample.Galleries
{
	/// <summary>
    /// Represets sample data item
    /// </summary>
    public class SampleDataItem:DependencyObject
    {
        /// <summary>
        /// Gets or sets icon
        /// </summary>
        public ImageSource Icon { get; set; }
        
        /// <summary>
        /// Gets or sets large icon
        /// </summary>
        public ImageSource IconLarge { get; set; }
        
        /// <summary>
        /// Gets or sets text
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Gets or sets group name
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Creates new item
        /// </summary>
        /// <param name="icon">Icon</param>
        /// <param name="iconLarge">Large Icon</param>
        /// <param name="text">Text</param>
        /// <param name="group">Group</param>
        /// <returns>Item</returns>
        public SampleDataItem (string icon, string iconLarge, string text, string group)
        {
        	Icon = new BitmapImage(new Uri(icon, UriKind.Relative));
        	IconLarge = new BitmapImage(new Uri(iconLarge, UriKind.Relative));
        	Text = text;
        	Group = group;
        }
    }
}
