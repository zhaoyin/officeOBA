using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Drawing;
using Cooperativeness.OBA.Word.Tools;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义插件助手类
    /// </summary>
    public class BundleUtil
    {
        private static readonly Logger Log = new Logger(typeof(BundleUtil));
        /// <summary>
        /// 按照对象字符串大小进行快速升序排序
        /// </summary>
        /// <param name="array"></param>
        public static void SortByString(object[] array)
        {
            QSortByString(array, 0, array.Length - 1);
        }

        /// <summary>
        /// 按照对象字符串大小进行快速升序排序
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        public static void QSortByString(object[] array, int start, int stop)
        {
            if (start >= stop)
                return;

            int left = start; // left index
            int right = stop; // right index
            object temp; // for swapping

            // arbitrarily establish a partition element as the midpoint of the array
            string mid = array[(start + stop) / 2].ToString();

            // loop through the array until indices cross
            while (left <= right)
            {
                // find the first element that is smaller than the partition element from the left
                while ((left < stop) && (array[left].ToString().CompareTo(mid) < 0))
                {
                    ++left;
                }
                // find an element that is smaller than the partition element from the right
                while ((right > start) && (mid.CompareTo(array[right].ToString()) < 0))
                {
                    --right;
                }
                // if the indices have not crossed, Swap
                if (left <= right)
                {
                    temp = array[left];
                    array[left] = array[right];
                    array[right] = temp;
                    ++left;
                    --right;
                }
            }
            // Sort the left partition, if the right index has not reached the left side of array
            if (start < right)
            {
                QSortByString(array, start, right);
            }
            // Sort the right partition, if the left index has not reached the right side of array
            if (left < stop)
            {
                QSortByString(array, left, stop);
            }
        }

        /// <summary>
        /// 按照对象大小进行升序排序
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void Sort(object[] array, int start, int end)
        {
            int middle = (start + end) / 2;
            if (start + 1 < middle)
                Sort(array, start, middle);
            if (middle + 1 < end)
                Sort(array, middle, end);
            if (start + 1 >= end)
                return; // this case can only happen when this method is called by the user
            if (((IComparable)array[middle - 1]).CompareTo(array[middle]) <= 0)
                return;
            if (start + 2 == end)
            {
                object temp = array[start];
                array[start] = array[middle];
                array[middle] = temp;
                return;
            }
            int i1 = start, i2 = middle, i3 = 0;
            object[] merge = new object[end - start];
            while (i1 < middle && i2 < end)
            {
                merge[i3++] = ((IComparable)array[i1]).CompareTo(array[i2]) <= 0 ? array[i1++] : array[i2++];
            }
            if (i1 < middle)
                Array.Copy(array, i1, merge, i3, middle - i1);
            Array.Copy(merge, 0, array, start, i2 - start);
        }

        /// <summary>
        /// 按照对象大小进行降序排序
        /// </summary>
        /// <param name="array"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public static void DesSort(object[] array, int start, int end)
        {
            // first Sort in ascending order
            Sort(array, start, end);
            // then Swap the elements in the array
            Swap(array);
        }

        /// <summary>
        /// 反转对象数组
        /// </summary>
        /// <param name="array"></param>
        public static void Swap(object[] array)
        {
            int start = 0;
            int end = array.Length - 1;
            while (start < end)
            {
                object temp = array[start];
                array[start++] = array[end];
                array[end--] = temp;
            }
        }

        /// <summary>
        /// 在元数据列表中讲目标数据逐一删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Remove<T>(IList<T> source, IList<T> target)
        {
            if (target.Count == 0) return;
            foreach (var item in target)
            {
                if (source.Contains(item))
                    source.Remove(item);
            }
        }

        /// <summary>
        /// 集合合并操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public static void Combine<T>(IList<T> source, T[] target)
        {
            foreach (var item in target)
            {
                if (!source.Contains(item))
                    source.Add(item);
            }
        }

        /// <summary>
        /// 集合合并操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list1"></param>
        /// <param name="list1"></param>
        /// <returns></returns>
        public static void Combine<T>(IList<T> source, IList<T> target)
        {
            foreach (var item in target)
            {
                if (!source.Contains(item))
                    source.Add(item);
            }
        }

        /// <summary>
        /// 按用户指定的大小，返回对象的字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToString(object obj, int length)
        {
            bool onLeft = obj is decimal;
            return ToString(obj, length, ' ', onLeft);
        }

        /// <summary>
        /// 按用户指定的大小，返回对象的字符串
        /// 如果不够长度，则按照用户指定的左右位置填充
        /// 指定的字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="length"></param>
        /// <param name="pad"></param>
        /// <param name="onLeft"></param>
        /// <returns></returns>
        public static string ToString(object obj, int length, char pad, bool onLeft)
        {
            string input = obj.ToString();
            int size = input.Length;
            if (size >= length)
            {
                int start = (onLeft) ? size - length : 0;
                return input.Substring(start, length);
            }

            StringBuilder padding = new StringBuilder(length - size);
            for (int i = size; i < length; i++)
                padding.Append(pad);

            StringBuilder stringBuffer = new StringBuilder(length);
            if (onLeft)
                stringBuffer.Append(padding.ToString());
            stringBuffer.Append(input);
            if (!onLeft)
                stringBuffer.Append(padding.ToString());
            return stringBuffer.ToString();
        }

        /// <summary>
        /// 加载资源字符串
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string LoadResourceString(string fileName, Assembly assembly)
        {
            try
            {
                Assembly asm = assembly;
                Stream stream = asm.GetManifestResourceStream(fileName);
                if (stream == null)
                {
                    Log.Debug("加载资源字符串出现异常：无法根据文件名，取得资源字符串.FileName:"+fileName);
                    return "";
                }
                var readStream = new StreamReader(stream, Encoding.Default);
                string code = string.Empty;
                while (readStream.Peek() > -1)
                {
                    string input = readStream.ReadLine();
                    code += input + Environment.NewLine;
                }
                stream.Close();
                readStream.Close();
                return code;
            }
            catch (Exception ex)
            {
                Log.Debug(ex);
            }
            return null;
        }

        /// <summary>
        /// 按照用户指定路径加载图片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Image GetImageFromLocal(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");
                if (!File.Exists(name))
                    throw new FileNotFoundException(name);
                return Image.FromFile(name);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 按照用户指定流数据加载图片
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Image GetImageFromStream(Stream stream)
        {
            try
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");
                return Image.FromStream(stream);
            }
            catch
            {
                return null;
            }
        }
    }

}
