using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;

namespace Cooperativeness.Documents.Editor
{
	public class AppHelper
	{
		public struct Margins
		{
			public Margins(Thickness t)
			{
				Left = Convert.ToInt32(t.Left);
				Right = Convert.ToInt32(t.Right);
				Top = Convert.ToInt32(t.Top);
				Bottom = Convert.ToInt32(t.Bottom);
			}
			public int Left;
			public int Right;
			public int Top;
			public int Bottom;
		}
		public struct blogInfo
		{
			public string title;
			public string description;
		}

		public interface IgetCatList
		{
			[CookComputing.XmlRpc.XmlRpcMethod("metaWeblog.newPost")]
			string NewPage(int blogId, string strUserName, string strPassword, AppHelper.blogInfo content, int publish);
		}

//		[DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
//		private static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins pMargins)
//		{
//		}
//
//		[DllImport("dwmapi.dll", CharSet = CharSet.Auto)]
//		private static extern bool DwmIsCompositionEnabled()
//		{
//		}

		public static bool IsGlassEnabled {
			get { return false; }
		}

		public static bool ExtendGlassFrame(Window window, Thickness margin)
		{
			return false;
//			if (!DwmIsCompositionEnabled()) {
//				return false;
//			}
//			IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
//			Margins margins = new Margins(margin);
//			SolidColorBrush background = new SolidColorBrush(Colors.Red);
//			if (hwnd == IntPtr.Zero) {
//				throw new InvalidOperationException("The Window must be shown before extending glass.");
//			}
//			background.Opacity = 0.5;
//			window.Background = Brushes.Transparent;
//			System.Windows.Interop.HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;
//			DwmExtendFrameIntoClientArea(hwnd, ref margins);
//			return true;
		}

		public static void DisableGlassFrame(Window window)
		{
			IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
			if (hwnd == IntPtr.Zero) {
				throw new InvalidOperationException("The Window must be shown before extending glass.");
			}
			System.Windows.Interop.HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.White;
		}
	}
}
