using System;
using System.Text;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    /// <summary>
    /// 定义版本范围对象
    /// </summary>
    public sealed class VersionRange
    {
        #region 字段
        // 定义最大的版本号
        private static readonly Version versionMax = new Version(int.MaxValue, int.MaxValue, int.MaxValue);
        // 定义一个空版本范围
        public static readonly VersionRange EmptyRange = new VersionRange(null);
        // 定义空版本
        public static readonly Version EmptyVersion = new Version(0, 0, 0);
        // 最小版本
        private readonly Version minVersion;
        // 指示是否包含最小版本(包含吗?']':')')
        private readonly bool includeMin;
        // 最大版本
        private readonly Version maxVersion;
        // 是否包含最大版本(包含吗?'[':'(')
        private readonly bool includeMax;

        #endregion

        #region 构造函数
        public VersionRange(Version minVersion, bool includeMin, Version maxVersion, bool includeMax)
        {
            this.minVersion = minVersion == null ? EmptyVersion : minVersion;
            this.includeMin = includeMin;
            this.maxVersion = maxVersion == null ? VersionRange.versionMax : maxVersion;
            this.includeMax = includeMax;
        }

        public VersionRange(string versionRange)
        {
            if (versionRange == null || versionRange.Length == 0)
            {
                minVersion = EmptyVersion;
                includeMin = true;
                maxVersion = VersionRange.versionMax;
                includeMax = true;
                return;
            }
            versionRange = versionRange.Trim();
            char first = versionRange.ReadChar(0);
            if (first == '[' || first == '(')
            {
                int comma = versionRange.IndexOf(',');
                if (comma < 0)
                    throw new ArgumentException();
                char last = versionRange.ReadChar(versionRange.Length - 1);
                if (last != ']' && last != ')')
                    throw new ArgumentException();

                minVersion = new Version(versionRange.Substring(1, comma).Trim());
                includeMin = (first == '[');
                maxVersion = new Version(versionRange.Substring(comma + 1, versionRange.Length - 1).Trim());
                includeMax = (last == ']');
            }
            else
            {
                minVersion = new Version(versionRange.Trim());
                includeMin = true;
                maxVersion = VersionRange.versionMax;
                includeMax = true;
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 获取最大版本
        /// </summary>
        public Version Minimum
        {
            get { return minVersion; }
        }

        /// <summary>
        /// 获取一个值，该值用来指示是否包含最小版本
        /// </summary>
        public bool IncludeMinimum
        {
            get { return includeMin; }
        }

        /// <summary>
        /// 获取最大版本值
        /// </summary>
        public Version Maximum
        {
            get { return maxVersion; }
        }

        /// <summary>
        /// 获取一个值，该值用来指示是否包含最大版本值
        /// </summary>
        public bool IncludeMaximum
        {
            get { return includeMax; }
        }
        #endregion

        #region 方法
      
        /// <summary>
        /// 判断用户给定的版本是否属于当前版本范围
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool IsIncluded(Version version)
        {
            if (version == null)
                version = EmptyVersion;
            int minCheck = includeMin ? 0 : 1;
            int maxCheck = includeMax ? 0 : -1;
            return version.CompareTo(minVersion) >= minCheck && version.CompareTo(maxVersion) <= maxCheck;
        }

        /// <summary>
        /// 判断用户给定的版本范围对象是否和当前版本范围相等
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is VersionRange))
                return false;
            VersionRange vr = (VersionRange)obj;
            if (minVersion.Equals(vr.minVersion) && includeMin == vr.includeMin)
                if (maxVersion.Equals(vr.maxVersion) && includeMax == vr.includeMax)
                    return true;
            return false;
        }

        /// <summary>
        /// 获取当前版本范围的哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + maxVersion.GetHashCode();
            result = prime * result + minVersion.GetHashCode();
            result = prime * result + (includeMax ? 1231 : 1237);
            result = prime * result + (includeMin ? 1231 : 1237);
            return result;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (VersionRange.versionMax.Equals(maxVersion))
                return minVersion.ToString(); // we assume infinity max; use simple version (i.e version="1.0")
            StringBuilder result = new StringBuilder();
            result.Append(includeMin ? '[' : '(');
            result.Append(minVersion);
            result.Append(',');
            result.Append(maxVersion);
            result.Append(includeMax ? ']' : ')');
            return result.ToString();
        }
        #endregion
    }
}
