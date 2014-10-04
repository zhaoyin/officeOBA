using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义字符串解析器
    /// </summary>
    public class StringTokenizer : IEnumeration<object>
    {
        #region 字段
        private int currentPosition;
        private int newPosition;
        private int maxPosition;
        private string str;
        private string delimiters;
        private bool retDelims;
        private bool delimsChanged;

        /**
         * maxDelimCodePoint stores the value of the delimiter character with the
         * highest value. It is used to optimize the detection of delimiter
         * characters.
         *
         * It is unlikely to provide any optimization benefit in the
         * hasSurrogates case because most string characters will be
         * smaller than the limit, but we keep it so that the two code
         * paths remain similar.
         */
        private int maxDelimCodePoint;

        /**
         * If delimiters include any surrogates (including surrogate
         * pairs), hasSurrogates is true and the tokenizer uses the
         * different code path. This is because String.indexOf(int)
         * doesn't handle unpaired surrogates as a single character.
         */
        private bool hasSurrogates = false;

        /**
         * When hasSurrogates is true, delimiters are converted to code
         * points and isDelimiter(int) is used to determine if the given
         * codepoint is a delimiter.
         */
        private int[] delimiterCodePoints;

        #endregion

        #region 构造函数
        public StringTokenizer(string str, string delim, bool returnDelims)
        {
            currentPosition = 0;
            newPosition = -1;
            delimsChanged = false;
            this.str = str;
            maxPosition = str.Length;
            delimiters = delim;
            retDelims = returnDelims;
            SetMaxDelimCodePoint();
        }

        public StringTokenizer(string str, string delim)
            :this(str,delim,false)
        {    
        }

        public StringTokenizer(string str)
            : this(str, " \t\n\r\f", false)
        {
        }
        #endregion

        #region 方法
        /// <summary>
        /// 设定最高的字符在分隔符集
        /// </summary>
        private void SetMaxDelimCodePoint()
        {
            if (delimiters == null)
            {
                maxDelimCodePoint = 0;
                return;
            }

            int m = 0;
            int c;
            int count = 0;
            for (int i = 0; i < delimiters.Length; i += c.CharCount())
            {
                c = delimiters.ReadChar(i);
                if (c >= StringExtension.MIN_HIGH_SURROGATE && c <= StringExtension.MAX_LOW_SURROGATE)
                {
                    c = delimiters.CodePointAt(i);;
                    hasSurrogates = true;
                }
                if (m < c)
                    m = c;
                count++;
            }
            maxDelimCodePoint = m;

            if (hasSurrogates)
            {
                delimiterCodePoints = new int[count];
                for (int i = 0, j = 0; i < count; i++, j += c.CharCount())
                {
                    c = delimiters.CodePointAt(j);
                    delimiterCodePoints[i] = c;
                }
            }
        }

        private int SkipDelimiters(int startPos)
        {
            if (delimiters == null)
                throw new ArgumentNullException();

            int position = startPos;
            while (!retDelims && position < maxPosition)
            {
                if (!hasSurrogates)
                {
                    char c = str.ReadChar(position);
                    if ((c > maxDelimCodePoint) || (delimiters.IndexOf(c) < 0))
                        break;
                    position++;
                }
                else
                {
                    int c = str.CodePointAt(position);
                    if ((c > maxDelimCodePoint) || !IsDelimiter(c))
                    {
                        break;
                    }
                    position += c.CharCount();
                }
            }
            return position;
        }

        private int ScanToken(int startPos)
        {
            int position = startPos;
            while (position < maxPosition)
            {
                if (!hasSurrogates)
                {
                    char c = str.ReadChar(position);
                    if ((c <= maxDelimCodePoint) && (delimiters.IndexOf(c) >= 0))
                        break;
                    position++;
                }
                else
                {
                    int c = str.CodePointAt(position);
                    if ((c <= maxDelimCodePoint) && IsDelimiter(c))
                        break;
                    position += c.CharCount();
                }
            }
            if (retDelims && (startPos == position))
            {
                if (!hasSurrogates)
                {
                    char c = str.ReadChar(position);
                    if ((c <= maxDelimCodePoint) && (delimiters.IndexOf(c) >= 0))
                        position++;
                }
                else
                {
                    int c = str.CodePointAt(position);
                    if ((c <= maxDelimCodePoint) && IsDelimiter(c))
                        position += c.CharCount();
                }
            }
            return position;
        }

        private bool IsDelimiter(int codePoint)
        {
            for (int i = 0; i < delimiterCodePoints.Length; i++)
            {
                if (delimiterCodePoints[i] == codePoint)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region 实现IEnumeration接口
        public bool HasMoreTokens()
        {
            /*
             * Temporarily store this position and use it in the following
             * nextToken() method only if the delimiters haven't been changed in
             * that nextToken() invocation.
             */
            newPosition = SkipDelimiters(currentPosition);
            return (newPosition < maxPosition);
        }

        public string NextToken()
        {
            /*
             * If next position already computed in hasMoreElements() and
             * delimiters have changed between the computation and this invocation,
             * then use the computed value.
             */

            currentPosition = (newPosition >= 0 && !delimsChanged) ?
                newPosition : SkipDelimiters(currentPosition);

            /* Reset these anyway */
            delimsChanged = false;
            newPosition = -1;

            if (currentPosition >= maxPosition)
                throw new NoSuchElementException();
            int start = currentPosition;
            currentPosition = ScanToken(currentPosition);
            return str.Substring(start, currentPosition-start);
        }

        public string NextToken(String delim)
        {
            delimiters = delim;

            /* delimiter string specified, so set the appropriate flag. */
            delimsChanged = true;

            SetMaxDelimCodePoint();
            return NextToken();
        }

        public bool HasMoreElements()
        {
            return HasMoreTokens();
        }

        public Object NextElement()
        {
            return NextToken();
        }

        public int CountTokens()
        {
            int count = 0;
            int currpos = currentPosition;
            while (currpos < maxPosition)
            {
                currpos = SkipDelimiters(currpos);
                if (currpos >= maxPosition)
                    break;
                currpos = ScanToken(currpos);
                count++;
            }
            return count;
        }
        #endregion
    }
}
