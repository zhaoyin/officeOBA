#region Copyright and License Information

// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg., Weegen Patrick 2009-2013.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license

#endregion

using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Fluent.Sample.ColorGallery
{
    /// <summary>
    /// Represents the main window of the application
    /// </summary>
    public partial class Window : RibbonWindow
    {
        public IEnumerable<Color> ThemeColors { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Window()
        {
            InitializeComponent();
            
            var colors = new List<Color>();
            colors.Add(Colors.White);
            colors.Add(Colors.Tan);
            colors.Add(Colors.DarkBlue);
            colors.Add(Colors.Red);
            colors.Add(Colors.DarkOliveGreen);
            colors.Add(Colors.Aqua);
            colors.Add(Colors.Orange);
            colors.Add(Colors.Gray);
            colors.Add(Colors.Yellow);
            colors.Add(Colors.Black);
            
            ThemeColors = colors;
            
            DataContext = this;
        }
    }
}