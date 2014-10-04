using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义字符串扩展类
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 读取某个偏移位置的字符.如果超出则返回特殊字符"\0x0"
        /// </summary>
        /// <param name="text"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static char ReadChar(this string text, int offset)
        {
            if (offset < text.Length) return text[offset];
            return (char)0;
        }

        public static bool IsHighSurrogate(char ch)
        {
            // Help VM constant-fold; MAX_HIGH_SURROGATE + 1 == MIN_LOW_SURROGATE
            return ch >= MIN_HIGH_SURROGATE && ch < (MAX_HIGH_SURROGATE + 1);
        }

        public static bool IsLowSurrogate(char ch)
        {
            return ch >= MIN_LOW_SURROGATE && ch < (MAX_LOW_SURROGATE + 1);
        }

        public static int ToCodePoint(char high, char low)
        {
            // Optimized form of:
            // return ((high - MIN_HIGH_SURROGATE) << 10)
            //         + (low - MIN_LOW_SURROGATE)
            //         + MIN_SUPPLEMENTARY_CODE_POINT;
            return ((high << 10) + low) + (MIN_SUPPLEMENTARY_CODE_POINT
                                           - (MIN_HIGH_SURROGATE << 10)
                                           - MIN_LOW_SURROGATE);
        }

        private static int CodePointAtImpl(char[] a, int index, int limit)
        {
            char c1 = a[index++];
            if (IsHighSurrogate(c1))
            {
                if (index < limit)
                {
                    char c2 = a[index];
                    if (IsLowSurrogate(c2))
                    {
                        return ToCodePoint(c1, c2);
                    }
                }
            }
            return c1;
        }

        public static bool RegionMatches(this string value, int toffset, string other, int ooffset, int len)
        {
            char[] ta = value.ToCharArray();
            int to = toffset;
            char[] pa = other.ToCharArray();
            int po = ooffset;
            // Note: toffset, ooffset, or len might be near -1>>>1.
            if ((ooffset < 0) || (toffset < 0)
                || (toffset > (long)value.Length - len)
                || (ooffset > (long)other.Length - len))
            {
                return false;
            }
            while (len-- > 0)
            {
                if (ta[to++] != pa[po++])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool RegionMatches(this string value, bool ignoreCase, int toffset, string other, int ooffset, int len)
        {
            char[] ta = value.ToCharArray();
            int to = toffset;
            char[] pa = other.ToCharArray();
            int po = ooffset;
            // Note: toffset, ooffset, or len might be near -1>>>1.
            if ((ooffset < 0) || (toffset < 0)
                || (toffset > (long)value.Length - len)
                || (ooffset > (long)other.Length - len))
            {
                return false;
            }
            while (len-- > 0)
            {
                char c1 = ta[to++];
                char c2 = pa[po++];
                if (c1 == c2)
                {
                    continue;
                }
                if (ignoreCase)
                {
                    // If characters don't match but case may be ignored,
                    // try converting both characters to uppercase.
                    // If the results match, then the comparison scan should
                    // continue.
                    char u1 = Char.ToUpper(c1);
                    char u2 = Char.ToUpper(c2);
                    if (u1 == u2)
                    {
                        continue;
                    }
                    // Unfortunately, conversion to uppercase does not work properly
                    // for the Georgian alphabet, which has strange rules about case
                    // conversion.  So we need to make one last check before
                    // exiting.
                    if (Char.ToLower(u1) == Char.ToLower(u2))
                    {
                        continue;
                    }
                }
                return false;
            }
            return true;
        }

        public static bool EqualsIgnoreCase(this string value, string other)
        {
            return (value == other) ? true
                : (other != null)
                && (other.Length == value.Length)
                && value.RegionMatches(true, 0, other, 0, value.Length);
        }

        public static int CodePointAt(this string value, int index)
        {
            if ((index < 0) || (index >= value.Length))
            {
                throw new IndexOutOfRangeException("index");
            }
            return CodePointAtImpl(value.ToCharArray(), index, value.Length);
        }

        /// <summary>
        /// The minimum value of a Unicode supplementary code point, constant {@code U+10000}.
        /// </summary>
        public static readonly int MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;

        /// <summary>
        /// The minimum value of a 
        /// Unicode high-surrogate code unit in the UTF-16 encoding , constant {@code u005CuDBFF}.
        /// </summary>
        public static readonly char MAX_HIGH_SURROGATE = '\uDBFF';

        /// <summary>
        /// The minimum value of a 
        /// Unicode high-surrogate code unit in the UTF-16 encoding, constant {@code '\u005CuDFFF'}.
        /// </summary>
        public static readonly char MIN_HIGH_SURROGATE = '\uD800';

        /// <summary>
        /// The maximum value of a 
        /// Unicode low-surrogate code unit in the UTF-16 encoding, constant {@code '\u005CuDFFF'}.
        /// </summary>
        public static readonly char MAX_LOW_SURROGATE = '\uDFFF';

        /// <summary>
        /// The minimum value of a 
        /// Unicode low-surrogate code unit in the UTF-16 encoding, constant {@code '\u005CuDC00'}.
        /// </summary>
        public static readonly char MIN_LOW_SURROGATE = '\uDC00';

        /// <summary>
        /// 指定字符是否是等于或大于0x10000的，那么该方法返回2。否则，该方法返回1
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int CharCount(this int codePoint)
        {
            return codePoint >= MIN_SUPPLEMENTARY_CODE_POINT ? 2 : 1;
        }
    }
}
