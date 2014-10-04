using System;

namespace Cooperativeness.FileTransfer.Downloader
{
    internal class Apply
    {
        public DateTime LastModified { get; set; }

        public long FileSize { get; set; }

        public bool AllowRanges { get; set; }

        public long BlockSize { get; set; }

        public int ActuallyChunks { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Apply)) return false;
            var apply = obj as Apply;
            return LastModified == apply.LastModified && FileSize == apply.FileSize && AllowRanges == apply.AllowRanges;
        }

        public override string ToString()
        {
            return string.Format(
                "Apply\tLastModified:{0},FileSize:{1},AllowRanges:{2},BlockSize:{3},ActuallyChunks{4}",
                LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
                FileSize.ToString(),
                AllowRanges.ToString(),
                BlockSize.ToString(),
                ActuallyChunks.ToString()
                );
        }
    }
}
