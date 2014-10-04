using System;

namespace Cooperativeness.FileTransfer.Core
{
    public class TypeConvert
    {
        /// <summary>
        /// 将字符串转换成布尔型
        /// </summary>
        public static bool ToBool(string data, bool defaultValule)
        {
            bool result;
            if (bool.TryParse(data, out result))
            {
                return result;
            }
            return defaultValule;
        }

        /// <summary>
        /// 将字符串转换成整数
        /// </summary>
        public static int ToInt(string data, int defaultValule)
        {
            int result;
            if (int.TryParse(data, out result))
            {
                return result;
            }
            return defaultValule;
        }

        /// <summary>
        /// 将字符串转换成长整形数值
        /// </summary>
        public static long ToLong(string data, long defaultValule)
        {
            long result;
            if (long.TryParse(data, out result))
            {
                return result;
            }
            return defaultValule;
        }


        /// <summary>
        /// 将字符串转换成浮点形数值
        /// </summary>
        public static float ToFloat(string data, float defaultValule)
        {
            float result;
            if (float.TryParse(data, out result))
            {
                return result;
            }
            return defaultValule;
        }

        /// <summary>
        /// 将字符串转换成长浮点形数值
        /// </summary>
        public static double ToDouble(string data, double defaultValule)
        {
            double result;
            if (double.TryParse(data, out result))
            {
                return result;
            }
            return defaultValule;
        }

        /// <summary>
        /// 将字符串转换成长浮点形数值
        /// </summary>
        public static DateTime ToDateTime(string data, DateTime defaultValule)
        {
            DateTime result;
            if (DateTime.TryParse(data, out result))
            {
                return result;
            }
            return defaultValule;
        }
    }

}
