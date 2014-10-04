#region Copyright and License Information

// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright © Degtyarev Daniel, Rikker Serg., Weegen Patrick 2009-2013.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL).
// The license is available online http://fluent.codeplex.com/license

#endregion

using System.Collections.Generic;

namespace Fluent.Sample.Galleries
{
	/// <summary>
	/// Represents the main window of the application
	/// </summary>
	public partial class Window : RibbonWindow
	{
		/// <summary>
		/// Gets data items (uses as DataContext)
		/// </summary>
		public IEnumerable<SampleDataItem> DataItems { get; private set; }

		/// <summary>
		/// Default constructor
		/// </summary>
		public Window()
		{
			InitializeComponent();
			
			var items = new List<SampleDataItem>();
			items.Add(new SampleDataItem("Images\\Blue.png", "Images\\BlueLarge.png","Blue", "Group A"));
			items.Add(new SampleDataItem("Images\\Brown.png", "Images\\BrownLarge.png","Brown", "Group A"));
			items.Add(new SampleDataItem("Images\\Gray.png", "Images\\GrayLarge.png","Gray", "Group A"));
			items.Add(new SampleDataItem("Images\\Green.png", "Images\\GreenLarge.png","Green", "Group A"));
			items.Add(new SampleDataItem("Images\\Orange.png", "Images\\OrangeLarge.png","Orange", "Group A"));
			items.Add(new SampleDataItem("Images\\Pink.png", "Images\\PinkLarge.png","Pink", "Group B"));
			items.Add(new SampleDataItem("Images\\Red.png", "Images\\RedLarge.png","Red", "Group B"));
			items.Add(new SampleDataItem("Images\\Yellow.png", "Images\\YellowLarge.png","Yellow", "Group B"));
			DataItems = items;
			
			DataContext = this;
		}
	}
}