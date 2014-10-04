using System;

namespace Cooperativeness.OBA.Word.AddIns.FrameWork
{
    public class NoSuchElementException : Exception
    {
        public NoSuchElementException()
            : base()
        {
        }

        public NoSuchElementException(string message)
            : base(message)
        {
        }

        public NoSuchElementException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
