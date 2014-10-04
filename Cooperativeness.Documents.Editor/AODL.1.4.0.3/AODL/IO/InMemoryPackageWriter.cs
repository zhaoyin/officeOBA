using System;
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace AODL.IO
{
    public class InMemoryPackageWriter : IPackageWriter
    {
        private readonly IList<string> _files;
        private readonly Stream _odfStream;
        private ZipOutputStream _zipOutputStream;

        public InMemoryPackageWriter(Stream odfStream)
        {
            _odfStream = odfStream;
            _files = new List<string>();
        }

        #region IPackageWriter Members

        public void Initialize(string odffilepath)
        {
            _zipOutputStream = new ZipOutputStream(_odfStream);
            _zipOutputStream.UseZip64 = UseZip64.Off;
            _files.Clear();
        }

        public Stream Open(string filepath)
        {
            filepath = filepath.Replace('\\', '/');
            if (_files.Contains(filepath))
                throw new ArgumentException(string.Format("File already added: {0}.", filepath), "filepath");

            ZipEntry entry = new ZipEntry(filepath);
            _zipOutputStream.PutNextEntry(entry);
            _files.Add(filepath);

            return new StreamWrapper(_zipOutputStream);
        }

        public bool FileExists(string filepath)
        {
            filepath = filepath.Replace('\\', '/');

            return _files.Contains(filepath);
        }


        public void EndWrite()
        {
            _zipOutputStream.Close();
        }

        public void Dispose()
        {
            try
            {
                if (_zipOutputStream != null)
                    ((IDisposable) _zipOutputStream).Dispose();
            }
            catch (Exception)
            {
            }
        }

        #endregion

        #region Nested type: 

        private class StreamWrapper : Stream
        {
            private readonly Stream _stream;

            public StreamWrapper(Stream stream)
            {
                _stream = stream;
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get { return false; }
            }

            public override bool CanWrite
            {
                get { return _stream.CanWrite; }
            }

            public override long Length
            {
                get { throw new NotSupportedException(); }
            }

            public override long Position
            {
                get { throw new NotSupportedException(); }
                set { throw new NotSupportedException(); }
            }

            public override void Flush()
            {
                _stream.Flush();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                _stream.Write(buffer, offset, count);
            }

            public override void Close()
            {
            }

            protected override void Dispose(bool disposing)
            {
            }
        }

        #endregion
    }
}