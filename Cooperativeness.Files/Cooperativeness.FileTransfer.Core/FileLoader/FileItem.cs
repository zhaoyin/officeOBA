using System;

namespace Cooperativeness.FileTransfer.Core
{
    public class FileItem
    {
        public string Guid { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public bool IsCompleted { get; set; }

        public bool HasBreaked { get; set; }

        public string BreakInfo { get; set; }
    }
}
