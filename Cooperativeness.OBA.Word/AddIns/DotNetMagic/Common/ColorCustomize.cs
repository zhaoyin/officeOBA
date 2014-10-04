using System.Drawing;

namespace Crownwood.DotNetMagic.Common
{
    /// <summary>
    /// Class used to help supply colours used for drawing elements.
    /// </summary>
    public sealed class ColorCustomize
    {
        private static Color backColor;
        /// <summary>
        /// 
        /// </summary>
        public static Color BackColor
        {
            get
            {
                return backColor;
            }
            set
            {
                backColor = value;
            }

        }

        private static Color borderColor;
        /// <summary>
        /// 
        /// </summary>
        public static Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
            }
        }

        private static Color wdcInactiveStartColor;
        /// <summary>
        /// 
        /// </summary>
        public static Color WdcInactiveStartColor
        {
            get
            {
                return wdcInactiveStartColor;
            }
            set
            {
                wdcInactiveStartColor = value;
            }
        }
        private static Color wdcInactiveEndColor;
        /// <summary>
        /// 
        /// </summary>
        public static Color WdcInactiveEndColor
        {
            get
            {
                return wdcInactiveEndColor;
            }
            set
            {
                wdcInactiveEndColor = value;
            }
        }

        #region weijz add 2009-07-09

        private static bool allowTabPageGradient;
        /// <summary>
        /// 
        /// </summary>
        public static bool AllowTabPageGradient
        {
            get { return allowTabPageGradient; }
            set { allowTabPageGradient = value; }
        }


        private static Color tabPageInactiveStartColor;
        /// <summary>
        /// 页签未选中时开始颜色
        /// </summary>
        public static Color TabPageInactiveStartColor
        {
            get
            {
                return tabPageInactiveStartColor;
            }
            set
            {
                tabPageInactiveStartColor = value;
            }
        }

        private static Color tabPageInactiveEndColor;
        /// <summary>
        /// 页签未选中时结束颜色
        /// </summary>
        public static Color TabPageInactiveEndColor
        {
            get
            {
                return tabPageInactiveEndColor;
            }
            set
            {
                tabPageInactiveEndColor = value;
            }
        }


        private static Color tabPageActiveStartColor;
        /// <summary>
        /// 页签选中时开始颜色
        /// </summary>
        public static Color TabPageActiveStartColor
        {
            get
            {
                return tabPageActiveStartColor;
            }
            set
            {
                tabPageActiveStartColor = value;
            }
        }


        private static Color tabPageActiveEndColor;
        /// <summary>
        /// 页签选中时结束颜色
        /// </summary>
        public static Color TabPageActiveEndColor
        {
            get
            {
                return tabPageActiveEndColor;
            }
            set
            {
                tabPageActiveEndColor = value;
            }
        }

        #endregion


    }
}