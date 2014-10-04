using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Cooperativeness.OBA.Word.AddIns.FrameWork.Service;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork.Core
{
    /// <summary>
    /// 定义过滤器对象
    /// </summary>
    public class FilterImpl : IFilter
    {
        private static readonly Logger Log = new Logger(typeof(FilterImpl));
        /// <summary>
        /// 创建过滤器实例
        /// </summary>
        /// <param name="filterString"></param>
        /// <returns></returns>
        public static FilterImpl NewInstance(string filterString)
        {
            return new Parser(filterString).Parse();
        }

        /// <summary>
        /// 使用服务的属性进行匹配
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public bool Match(IServiceReference reference)
        {
            try
            {
                if (reference is ServiceReferenceImpl)
                {
                    return Match0(((ServiceReferenceImpl)reference).Registration.GetProperties());
                }
                return Match0(new ServiceReferenceDictionary(reference));
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }
        }

        /// <summary>
        /// 根据属性集合进行匹配
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public bool Match(IDictionary<string, object> dictionary)
        {
            if (dictionary != null)
            {
                dictionary = new Dictionary<string, object>(dictionary);
            }

            return Match0(dictionary);
        }

        /// <summary>
        /// 根据属性集合进行大小写敏感匹配
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public bool MatchCase(IDictionary<string, object> dictionary)
        {
            return Match0(dictionary);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String result = filterString;
            if (result == null)
            {
                filterString = result = Normalize();
            }
            return result;
        }

        private string Normalize()
        {
            var sb = new StringBuilder();
            sb.Append('(');

            switch (op)
            {
                case AND:
                    {
                        sb.Append('&');

                        var filters = (FilterImpl[])value;
                        for (int i = 0, size = filters.Length; i < size; i++)
                        {
                            sb.Append(filters[i].Normalize());
                        }

                        break;
                    }
                case OR:
                    {
                        sb.Append('|');

                        var filters = (FilterImpl[])value;
                        for (int i = 0, size = filters.Length; i < size; i++)
                        {
                            sb.Append(filters[i].Normalize());
                        }

                        break;
                    }

                case NOT:
                    {
                        sb.Append('!');
                        var filter = (FilterImpl)value;
                        sb.Append(filter.Normalize());

                        break;
                    }

                case SUBSTRING:
                    {
                        sb.Append(attr);
                        sb.Append('=');

                        var substrings = (String[])value;

                        for (int i = 0, size = substrings.Length; i < size; i++)
                        {
                            String substr = substrings[i];

                            if (substr == null) /* * */
                            {
                                sb.Append('*');
                            }
                            else /* xxx */
                            {
                                sb.Append(EncodeValue(substr));
                            }
                        }

                        break;
                    }
                case EQUAL:
                    {
                        sb.Append(attr);
                        sb.Append('=');
                        sb.Append(EncodeValue((string)value));

                        break;
                    }
                case GREATER:
                    {
                        sb.Append(attr);
                        sb.Append(">="); //$NON-NLS-1$
                        sb.Append(EncodeValue((string)value));

                        break;
                    }
                case LESS:
                    {
                        sb.Append(attr);
                        sb.Append("<="); //$NON-NLS-1$
                        sb.Append(EncodeValue((String)value));

                        break;
                    }
                case APPROX:
                    {
                        sb.Append(attr);
                        sb.Append("~="); //$NON-NLS-1$
                        sb.Append(EncodeValue(ApproxString((string)value)));

                        break;
                    }

                case PRESENT:
                    {
                        sb.Append(attr);
                        sb.Append("=*"); //$NON-NLS-1$

                        break;
                    }
            }

            sb.Append(')');

            return sb.ToString();
        }

        /// <summary>
        /// 检查对象是否相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }

            if (!(obj is IFilter))
            {
                return false;
            }

            return this.ToString().Equals(obj.ToString());
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /* non public fields and methods for the Filter implementation */

        /** filter operation */
        private int op;
        private const int EQUAL = 1;
        private const int APPROX = 2;
        private const int GREATER = 3;
        private const int LESS = 4;
        private const int PRESENT = 5;
        private const int SUBSTRING = 6;
        private const int AND = 7;
        private const int OR = 8;
        private const int NOT = 9;

        /** filter attribute or null if operation AND, OR or NOT */
        private string attr;
        /** filter operands */
        private object value;

        /* normalized filter string for topLevel Filter object */
        private string filterString;

        FilterImpl(int operation, string attr, object value)
        {
            this.op = operation;
            this.attr = attr;
            this.value = value;
        }

        /**
         * Internal match routine.
         * Dictionary parameter must support case-insensitive get.
         *
         * @param property A dictionary whose
         * keys are used in the match.
         * @return If the Dictionary's keys match the filter,
         * return <code>true</code>. Otherwise, return <code>false</code>.
         */
        private bool Match0(IDictionary<string, object> properties)
        {
            switch (op)
            {
                case AND:
                    {
                        var filters = (FilterImpl[])value;
                        for (int i = 0, size = filters.Length; i < size; i++)
                        {
                            if (!filters[i].Match0(properties))
                            {
                                return false;
                            }
                        }

                        return true;
                    }

                case OR:
                    {
                        var filters = (FilterImpl[])value;
                        for (int i = 0, size = filters.Length; i < size; i++)
                        {
                            if (filters[i].Match0(properties))
                            {
                                return true;
                            }
                        }

                        return false;
                    }

                case NOT:
                    {
                        var filter = (FilterImpl)value;

                        return !filter.Match0(properties);
                    }

                case SUBSTRING:
                case EQUAL:
                case GREATER:
                case LESS:
                case APPROX:
                    {
                        Object prop = (properties == null) ? null : properties[attr];

                        return Compare(op, prop, value);
                    }

                case PRESENT:
                    {

                        Object prop = (properties == null) ? null : properties[attr];

                        return prop != null;
                    }
            }

            return false;
        }

        /**
         * Encode the value string such that '(', '*', ')'
         * and '\' are escaped.
         *
         * @param value unencoded value string.
         * @return encoded value string.
         */
        private static string EncodeValue(String value)
        {
            bool encoded = false;
            int inlen = value.Length;
            int outlen = inlen << 1; /* inlen * 2 */

            char[] output = value.ToCharArray(0, inlen);

            int cursor = 0;
            for (int i = inlen; i < outlen; i++)
            {
                char c = output[i];

                switch (c)
                {
                    case '(':
                    case '*':
                    case ')':
                    case '\\':
                        {
                            output[cursor] = '\\';
                            cursor++;
                            encoded = true;

                            break;
                        }
                }

                output[cursor] = c;
                cursor++;
            }

            return encoded ? new string(output, 0, cursor) : value;
        }

        private bool Compare(int operation, Object value1, Object value2)
        {
            try
            {
                if (value1 == null)
                {
                    return false;
                }

                if (value1 is string)
                {
                    return compare_String(operation, (String)value1, value2);
                }

                Type clazz = value1.GetType();

                if (clazz.IsArray)
                {
                    Type type = clazz.GetElementType();

                    if (type.IsPrimitive)
                    {
                        return compare_PrimitiveArray(operation, type, value1, value2);
                    }
                    return compare_ObjectArray(operation, (Object[])value1, value2);
                }

                if (value1 is ICollection)
                {
                    return compare_Collection(operation, (ICollection)value1, value2);
                }

                if (value1 is int)
                {
                    return compare_Integer(operation, (int)value1, value2);
                }

                if (value1 is long)
                {
                    return compare_Long(operation, (long)value1, value2);
                }

                if (value1 is Byte)
                {
                    return compare_Byte(operation, (Byte)value1, value2);
                }

                if (value1 is short)
                {
                    return compare_Short(operation, (short)value1, value2);
                }

                if (value1 is char)
                {
                    return compare_Character(operation, (char)value1, value2);
                }

                if (value1 is float)
                {
                    return compare_Float(operation, (float)value1, value2);
                }

                if (value1 is double)
                {
                    return compare_Double(operation, (double)value1, value2);
                }

                if (value1 is bool)
                {
                    return compare_Boolean(operation, (bool)value1, value2);
                }

                if (value1 is IComparable)
                {
                    return compare_Comparable(operation, (IComparable)value1, value2);
                }

                return compare_Unknown(operation, value1, value2); // RFC 59
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }
            
        }

        private bool compare_Collection(int operation, ICollection collection, Object value2)
        {
            for (IEnumerator iterator = collection.GetEnumerator(); iterator.MoveNext(); )
            {
                if (Compare(operation, iterator.Current, value2))
                {
                    return true;
                }
            }

            return false;
        }

        private bool compare_ObjectArray(int operation, Object[] array, Object value2)
        {
            for (int i = 0, size = array.Length; i < size; i++)
            {
                if (Compare(operation, array[i], value2))
                {
                    return true;
                }
            }

            return false;
        }

        private bool compare_PrimitiveArray(int operation, Type type, object primarray, object value2)
        {
            if (typeof(int).IsAssignableFrom(type))
            {
                var array = (int[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (compare_Integer(operation, array[i], value2))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (typeof(long).IsAssignableFrom(type))
            {
                var array = (long[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (compare_Long(operation, array[i], value2))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (typeof(Byte).IsAssignableFrom(type))
            {
                var array = (byte[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (compare_Byte(operation, array[i], value2))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (typeof(short).IsAssignableFrom(type))
            {
                var array = (short[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (compare_Short(operation, array[i], value2))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (typeof(char).IsAssignableFrom(type))
            {
                var array = (char[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (compare_Character(operation, array[i], value2))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (typeof(float).IsAssignableFrom(type))
            {
                var array = (float[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (compare_Float(operation, array[i], value2))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (typeof(double).IsAssignableFrom(type))
            {
                var array = (double[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (compare_Double(operation, array[i], value2))
                    {
                        return true;
                    }
                }

                return false;
            }

            if (typeof(bool).IsAssignableFrom(type))
            {
                var array = (bool[])primarray;
                for (int i = 0, size = array.Length; i < size; i++)
                {
                    if (compare_Boolean(operation, array[i], value2))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        private bool compare_String(int operation, string str, object value2)
        {
            switch (operation)
            {
                case SUBSTRING:
                    {

                        var substrings = (string[])value2;
                        int pos = 0;
                        for (int i = 0, size = substrings.Length; i < size; i++)
                        {
                            String substr = substrings[i];

                            if (i + 1 < size) /* if this is not that last substr */
                            {
                                if (substr == null) /* * */
                                {
                                    string substr2 = substrings[i + 1];

                                    if (substr2 == null) /* ** */
                                        continue; /* ignore first star */
                                    /* *xxx */
                                    int index = str.IndexOf(substr2, pos);
                                    if (index == -1)
                                    {
                                        return false;
                                    }

                                    pos = index + substr2.Length;
                                    if (i + 2 < size) // if there are more substrings, increment over the string we just matched; otherwise need to do the last substr check
                                        i++;
                                }
                                else /* xxx */
                                {
                                    int len = substr.Length;

                                    if (str.RegionMatches(pos, substr, 0, len))
                                    {
                                        pos += len;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            else /* last substr */
                            {
                                if (substr == null) /* * */
                                {
                                    return true;
                                }
                                /* xxx */
                                return str.EndsWith(substr);
                            }
                        }

                        return true;
                    }
                case EQUAL:
                    {
                        return str.Equals(value2);
                    }
                case APPROX:
                    {
                        str = ApproxString(str);
                        String string2 = ApproxString((String)value2);

                        return str.EqualsIgnoreCase(string2);
                    }
                case GREATER:
                    {
                        return str.CompareTo((string)value2) >= 0;
                    }
                case LESS:
                    {
                        return str.CompareTo((string)value2) <= 0;
                    }
            }

            return false;
        }

        private bool compare_Integer(int operation, int intval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            int intval2;
            try
            {
                intval2 = int.Parse(((string)value2).Trim());
            }
            catch (ArgumentException e)
            {
                Log.Debug(e);
                return false;
            }
            switch (operation)
            {
                case EQUAL:
                    {
                        return intval == intval2;
                    }
                case APPROX:
                    {
                        return intval == intval2;
                    }
                case GREATER:
                    {
                        return intval >= intval2;
                    }
                case LESS:
                    {
                        return intval <= intval2;
                    }
            }

            return false;
        }

        private bool compare_Long(int operation, long longval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            long longval2;
            try
            {
                longval2 = long.Parse(((string)value2).Trim());
            }
            catch (ArgumentException e)
            {
                Log.Debug(e);
                return false;
            }
            switch (operation)
            {
                case EQUAL:
                    {
                        return longval == longval2;
                    }
                case APPROX:
                    {
                        return longval == longval2;
                    }
                case GREATER:
                    {
                        return longval >= longval2;
                    }
                case LESS:
                    {
                        return longval <= longval2;
                    }
            }

            return false;
        }

        private bool compare_Byte(int operation, byte byteval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            byte byteval2;
            try
            {
                byteval2 = Byte.Parse(((string)value2).Trim());
            }
            catch (ArgumentException e)
            {
                Log.Debug(e);
                return false;
            }
            switch (operation)
            {
                case EQUAL:
                    {
                        return byteval == byteval2;
                    }
                case APPROX:
                    {
                        return byteval == byteval2;
                    }
                case GREATER:
                    {
                        return byteval >= byteval2;
                    }
                case LESS:
                    {
                        return byteval <= byteval2;
                    }
            }

            return false;
        }

        private bool compare_Short(int operation, short shortval, object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            short shortval2;
            try
            {
                shortval2 = short.Parse(((string)value2).Trim());
            }
            catch (ArgumentException e)
            {
                Log.Debug(e);
                return false;
            }
            switch (operation)
            {
                case EQUAL:
                    {
                        return shortval == shortval2;
                    }
                case APPROX:
                    {
                        return shortval == shortval2;
                    }
                case GREATER:
                    {
                        return shortval >= shortval2;
                    }
                case LESS:
                    {
                        return shortval <= shortval2;
                    }
            }

            return false;
        }

        private bool compare_Character(int operation, char charval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            char charval2;
            try
            {
                charval2 = ((string)value2).ReadChar(0);
            }
            catch (IndexOutOfRangeException e)
            {
                Log.Debug(e);
                return false;
            }
            switch (operation)
            {
                case EQUAL:
                    {
                        return charval == charval2;
                    }
                case APPROX:
                    {
                        return (charval == charval2) || (Char.ToUpper(charval) == Char.ToUpper(charval2)) || (Char.ToLower(charval) == Char.ToLower(charval2));
                    }
                case GREATER:
                    {
                        return charval >= charval2;
                    }
                case LESS:
                    {
                        return charval <= charval2;
                    }
            }

            return false;
        }

        private bool compare_Boolean(int operation, bool boolval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            bool boolval2;
            try
            {
                boolval2 = bool.Parse(((string)value2).Trim());
            }
            catch (ArgumentException e)
            {
                Log.Debug(e);
                return false;
            }
            switch (operation)
            {
                case EQUAL:
                    {
                        return boolval == boolval2;
                    }
                case APPROX:
                    {
                        return boolval == boolval2;
                    }
                case GREATER:
                    {
                        return boolval == boolval2;
                    }
                case LESS:
                    {
                        return boolval == boolval2;
                    }
            }

            return false;
        }

        private bool compare_Float(int operation, float floatval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            float floatval2;
            try
            {
                floatval2 = float.Parse(((String)value2).Trim());
            }
            catch (ArgumentException e)
            {
                Log.Debug(e);
                return false;
            }
            switch (operation)
            {
                case EQUAL:
                    {
                        return floatval.CompareTo(floatval2) == 0;
                    }
                case APPROX:
                    {
                        return floatval.CompareTo(floatval2) == 0;
                    }
                case GREATER:
                    {
                        return floatval.CompareTo(floatval2) >= 0;
                    }
                case LESS:
                    {
                        return floatval.CompareTo(floatval2) <= 0;
                    }
            }

            return false;
        }

        private bool compare_Double(int operation, double doubleval, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            double doubleval2;
            try
            {
                doubleval2 = Double.Parse(((String)value2).Trim());
            }
            catch (ArgumentException e)
            {
                Log.Debug(e);
                return false;
            }
            switch (operation)
            {
                case EQUAL:
                    {
                        return doubleval.CompareTo(doubleval2) == 0;
                    }
                case APPROX:
                    {
                        return doubleval.CompareTo(doubleval2) == 0;
                    }
                case GREATER:
                    {
                        return doubleval.CompareTo(doubleval2) >= 0;
                    }
                case LESS:
                    {
                        return doubleval.CompareTo(doubleval2) <= 0;
                    }
            }

            return false;
        }

        private static Type[] constructorType = new Type[] { typeof(string) };

        private bool compare_Comparable(int operation, IComparable value1, Object value2)
        {
            if (operation == SUBSTRING)
            {
                return false;
            }

            ConstructorInfo constructor;
            try
            {
                constructor = value1.GetType().GetConstructor(constructorType);
                if (constructor == null) return false;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }
            try
            {
                value2 = constructor.Invoke(new Object[] { ((String)value2).Trim() });
            }
            catch (MethodAccessException e)
            {
                Log.Debug(e);
                return false;
            }
            catch (MemberAccessException e)
            {
                Log.Debug(e);
                return false;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }

            switch (operation)
            {
                case EQUAL:
                    {
                        return value1.CompareTo(value2) == 0;
                    }
                case APPROX:
                    {
                        return value1.CompareTo(value2) == 0;
                    }
                case GREATER:
                    {
                        return value1.CompareTo(value2) >= 0;
                    }
                case LESS:
                    {
                        return value1.CompareTo(value2) <= 0;
                    }
            }

            return false;
        }

        private bool compare_Unknown(int operation, object value1, object value2)
        { //RFC 59
            if (operation == SUBSTRING)
            {
                return false;
            }

            ConstructorInfo constructor;
            try
            {
                constructor = value1.GetType().GetConstructor(constructorType);
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }
            try
            {
                value2 = value1.GetType().GetConstructor(constructorType);
                if (constructor == null) return false;
            }
            catch (MethodAccessException e)
            {
                Log.Debug(e);
                return false;
            }
            catch (MemberAccessException e)
            {
                Log.Debug(e);
                return false;
            }
            catch (Exception e)
            {
                Log.Debug(e);
                return false;
            }

            switch (operation)
            {
                case EQUAL:
                    {
                        return value1.Equals(value2);
                    }
                case APPROX:
                    {
                        return value1.Equals(value2);
                    }
                case GREATER:
                    {
                        return value1.Equals(value2);
                    }
                case LESS:
                    {
                        return value1.Equals(value2);
                    }
            }

            return false;
        }

        /**
         * Map a string for an APPROX (~=) comparison.
         *
         * This implementation removes white spaces.
         * This is the minimum implementation allowed by
         * the OSGi spec.
         *
         * @param input Input string.
         * @return String ready for APPROX comparison.
         */
        private static string ApproxString(string input)
        {
            bool changed = false;
            char[] output = input.ToCharArray();
            int cursor = 0;
            for (int i = 0, length = output.Length; i < length; i++)
            {
                char c = output[i];

                if (Char.IsWhiteSpace(c))
                {
                    changed = true;
                    continue;
                }

                output[cursor] = c;
                cursor++;
            }

            return changed ? new String(output, 0, cursor) : input;
        }


        /**
         * Returns all the attributes contained within this filter
         * @return all the attributes contained within this filter
         */
        public string[] GetAttributes()
        {
            var results = new ArrayList();
            GetAttributesInternal(results);
            var newArray = new String[results.Count];
            results.CopyTo(newArray);
            return newArray;
        }

        private void GetAttributesInternal(ArrayList results)
        {
            if (value is FilterImpl[])
            {
                var children = (FilterImpl[])value;
                foreach (var filterImpl in children)
                {
                    filterImpl.GetAttributesInternal(results);
                }
                return;
            }
            if (value is FilterImpl)
            {
                // The NOT operation only has one child filter (bug 188075)
                ((FilterImpl)value).GetAttributesInternal(results);
                return;
            }
            if (attr != null)
                results.Add(attr);
        }

        /**
         * Parser class for OSGi filter strings. This class parses
         * the complete filter string and builds a tree of Filter
         * objects rooted at the parent.
         */
        private class Parser
        {
            private string _filterstring;
            private char[] filterChars;
            private int pos;

            public Parser(string filterstring)
            {
                this._filterstring = filterstring;
                filterChars = filterstring.ToCharArray();
                pos = 0;
            }

            public FilterImpl Parse()
            {
                FilterImpl filter;
                try
                {
                    filter = parse_filter();
                }
                catch (IndexOutOfRangeException e)
                {
                    Log.Debug(e);
                    throw new Exception(e.Message);
                }

                if (pos != filterChars.Length)
                {
                    throw new Exception("大小和位置不相同！");
                }
                return filter;
            }

            private FilterImpl parse_filter()
            {
                FilterImpl filter;
                SkipWhiteSpace();

                if (filterChars[pos] != '(')
                {
                    throw new Exception();
                }

                pos++;

                filter = parse_filtercomp();

                SkipWhiteSpace();

                if (filterChars[pos] != ')')
                {
                    throw new Exception();
                }

                pos++;

                SkipWhiteSpace();

                return filter;
            }

            private FilterImpl parse_filtercomp()
            {
                SkipWhiteSpace();

                char c = filterChars[pos];

                switch (c)
                {
                    case '&':
                        {
                            pos++;
                            return parse_and();
                        }
                    case '|':
                        {
                            pos++;
                            return parse_or();
                        }
                    case '!':
                        {
                            pos++;
                            return parse_not();
                        }
                }
                return parse_item();
            }

            private FilterImpl parse_and()
            {
                int lookahead = pos;
                SkipWhiteSpace();

                if (filterChars[pos] != '(')
                {
                    pos = lookahead - 1;
                    return parse_item();
                }

                var operands = new ArrayList(10);

                while (filterChars[pos] == '(')
                {
                    FilterImpl child = parse_filter();
                    operands.Add(child);
                }
                var newArray = new FilterImpl[operands.Count];
                operands.CopyTo(newArray, 0);
                return new FilterImpl(FilterImpl.AND, null, newArray);
            }

            private FilterImpl parse_or()
            {
                int lookahead = pos;
                SkipWhiteSpace();

                if (filterChars[pos] != '(')
                {
                    pos = lookahead - 1;
                    return parse_item();
                }

                var operands = new ArrayList(10);

                while (filterChars[pos] == '(')
                {
                    FilterImpl child = parse_filter();
                    operands.Add(child);
                }
                var newArray = new FilterImpl[operands.Count];
                operands.CopyTo(newArray, 0);

                return new FilterImpl(FilterImpl.OR, null, newArray);
            }

            private FilterImpl parse_not()
            {
                int lookahead = pos;
                SkipWhiteSpace();

                if (filterChars[pos] != '(')
                {
                    pos = lookahead - 1;
                    return parse_item();
                }

                FilterImpl child = parse_filter();

                return new FilterImpl(FilterImpl.NOT, null, child);
            }

            private FilterImpl parse_item()  {
			string attr = parse_attr();

            SkipWhiteSpace();

			switch (filterChars[pos]) {
				case '~' : {
					if (filterChars[pos + 1] == '=') {
						pos += 2;
						return new FilterImpl(FilterImpl.APPROX, attr, parse_value());
					}
					break;
				}
				case '>' : {
					if (filterChars[pos + 1] == '=') {
						pos += 2;
						return new FilterImpl(FilterImpl.GREATER, attr, parse_value());
					}
					break;
				}
				case '<' : {
					if (filterChars[pos + 1] == '=') {
						pos += 2;
						return new FilterImpl(FilterImpl.LESS, attr, parse_value());
					}
					break;
				}
				case '=' : {
					if (filterChars[pos + 1] == '*') {
						int oldpos = pos;
						pos += 2;
                        SkipWhiteSpace();
						if (filterChars[pos] == ')') {
							return new FilterImpl(FilterImpl.PRESENT, attr, null);
						}
						pos = oldpos;
					}

					pos++;
					object str = parse_substring();

					if (str is string) {
						return new FilterImpl(FilterImpl.EQUAL, attr, str);
					}
                    return new FilterImpl(FilterImpl.SUBSTRING, attr, str);
				}
			}

			throw new Exception();
		}

            private String parse_attr()
            {
                SkipWhiteSpace();

                int begin = pos;
                int end = pos;

                char c = filterChars[pos];

                while (c != '~' && c != '<' && c != '>' && c != '=' && c != '(' && c != ')')
                {
                    pos++;

                    if (!Char.IsWhiteSpace(c))
                    {
                        end = pos;
                    }

                    c = filterChars[pos];
                }

                int length = end - begin;

                if (length == 0)
                {
                    throw new Exception();
                }

                return new String(filterChars, begin, length);
            }

            private String parse_value()
            {
                var sb = new StringBuilder(filterChars.Length - pos);

            parseloop: while (true)
                {
                    char c = filterChars[pos];

                    switch (c)
                    {
                        case ')':
                            {
                                goto parseloop;
                            }
                            
                        case '(':
                            {
                                throw new Exception();
                            }
                        case '\\':
                            {
                                pos++;
                                c = filterChars[pos];
                                /* fall through into default */
                            }
                            break;
                        default:
                            {
                                sb.Append(c);
                                pos++;
                                break;
                            }
                    }
                }

                if (sb.Length == 0)
                {
                    throw new Exception();
                }

                return sb.ToString();
            }

            private Object parse_substring()
            {
                var sb = new StringBuilder(filterChars.Length - pos);

                var operands = new ArrayList(10);

            parseloop: while (true)
                {
                    char c = filterChars[pos];

                    switch (c)
                    {
                        case ')':
                            {
                                if (sb.Length > 0)
                                {
                                    operands.Add(sb.ToString());
                                }

                                goto parseloop;
                            }

                        case '(':
                            {
                                throw new Exception();
                            }

                        case '*':
                            {
                                if (sb.Length > 0)
                                {
                                    operands.Add(sb.ToString());
                                }

                                sb.Length = 0;

                                operands.Add(null);
                                pos++;

                                break;
                            }

                        case '\\':
                            {
                                pos++;
                                c = filterChars[pos];
                                /* fall through into default */
                            }
                            break;
                        default:
                            {
                                sb.Append(c);
                                pos++;
                                break;
                            }
                    }
                }

                int size = operands.Count;

                if (size == 0)
                {
                    return "";
                }

                if (size == 1)
                {
                    Object single = operands[0];

                    if (single != null)
                    {
                        return single;
                    }
                }

                var newArray = new String[size];
                operands.CopyTo(newArray);

                return newArray;
            }

            private void SkipWhiteSpace()
            {
                for (int length = filterChars.Length; (pos < length) && Char.IsWhiteSpace(filterChars[pos]); )
                {
                    pos++;
                }
            }
        }

        /**
         * This Dictionary is used for key lookup from a ServiceReference during
         * filter evaluation. This Dictionary implementation only supports the get
         * operation using a String key as no other operations are used by the
         * Filter implementation.
         * 
         */
        private class ServiceReferenceDictionary : IDictionary<string,object>
        {
            private IServiceReference reference;

            public ServiceReferenceDictionary(IServiceReference reference)
            {
                this.reference = reference;
            }



            public void Add(string key, object value)
            {
                throw new NotImplementedException();
            }

            public bool ContainsKey(string key)
            {
                throw new NotImplementedException();
            }

            public ICollection<string> Keys
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(string key)
            {
                throw new NotImplementedException();
            }

            public bool TryGetValue(string key, out object value)
            {
                throw new NotImplementedException();
            }

            public ICollection<object> Values
            {
                get { throw new NotImplementedException(); }
            }

            public object this[string key]
            {
                get
                {
                    if (reference == null)
                    {
                        return null;
                    }
                    return reference.GetProperty((string)key);
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public void Add(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public int Count
            {
                get { throw new NotImplementedException(); }
            }

            public bool IsReadOnly
            {
                get { throw new NotImplementedException(); }
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                throw new NotImplementedException();
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
