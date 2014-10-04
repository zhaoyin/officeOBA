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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fluent.Sample.ColorGallery
{
	/// <summary>
	/// Represents the main window of the application
    /// </summary>
    public partial class Window : RibbonWindow
    {
    	private IEnumerable<Color> _themeColors;
    	
    	public IEnumerable<Color> ThemeColors
        {
    		get
    		{
    			return _themeColors;
    		}
    	}

    	/// <summary>
        /// Default constructor
        /// </summary>
        public Window()
        {
        	InitializeComponent();
        	DataContext = this;
            
        	var themeColors = new List<Color>();
            themeColors.Add(Colors.White);
            themeColors.Add(Colors.Tan);
            themeColors.Add(Colors.DarkBlue);
            themeColors.Add(Colors.Red);
            themeColors.Add(Colors.DarkOliveGreen);
            themeColors.Add(Colors.Aqua);
            themeColors.Add(Colors.Orange);
            themeColors.Add(Colors.Gray);
            themeColors.Add(Colors.Yellow);
            themeColors.Add(Colors.Black);
            _themeColors = themeColors;
        }
    }
}