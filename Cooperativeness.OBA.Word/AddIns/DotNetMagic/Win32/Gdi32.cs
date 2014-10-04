// *****************************************************************************
// 
//  (c) Crownwood Software Ltd 2004-2005. All rights reserved. 
//	The software and associated documentation supplied hereunder are the 
//	proprietary information of Crownwood Software Ltd, Bracknell, 
//	Berkshire, England and are supplied subject to licence terms.
// 
//  Version 3.0.2.0 	www.crownwood.net
// *****************************************************************************

using System;
using System.Runtime.InteropServices;

namespace Crownwood.DotNetMagic.Win32
{
	/// <summary>
	/// Access to GDI32 functions.
	/// </summary>
    public class Gdi32
    {
		/// <summary>
		/// CombineRgn function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern int CombineRgn(IntPtr dest, IntPtr src1, IntPtr src2, int flags);

		/// <summary>
		/// CreateRectRgnIndirect function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr CreateRectRgnIndirect(ref Win32.RECT rect); 

		/// <summary>
		/// GetClipBox function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern int GetClipBox(IntPtr hDC, ref Win32.RECT rectBox); 

		/// <summary>
		/// GetClipRgn function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetClipRgn(IntPtr hDC, ref IntPtr hRgn); 

		/// <summary>
		/// SelectClipRgn function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern int SelectClipRgn(IntPtr hDC, IntPtr hRgn); 

		/// <summary>
		/// CreateBrushIndirect function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr CreateBrushIndirect(ref LOGBRUSH brush); 

		/// <summary>
		/// PatBlt function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern bool PatBlt(IntPtr hDC, int x, int y, int width, int height, uint flags); 

		/// <summary>
		/// DeleteObject function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr DeleteObject(IntPtr hObject);

		/// <summary>
		/// DeleteDC function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern bool DeleteDC(IntPtr hDC);

		/// <summary>
		/// SelectObject function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

		/// <summary>
		/// CreateCompatibleDC function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

		/// <summary>
		/// GetDeviceCaps function of GDI32
		/// </summary>
		[DllImport("gdi32.dll", CharSet=CharSet.Auto)]
		public static extern int GetDeviceCaps(IntPtr hDC, int nIndex); 
	}
}
