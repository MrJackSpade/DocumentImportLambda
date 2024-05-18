using DocumentImportLambda.Archive.Exceptions;
using DocumentImportLambda.Archive.Interfaces;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using System.Collections;
using System.IO.Compression;

namespace DocumentImportLambda.Archive.Pocos
{
    /// <summary>
    /// An abstraction of an archived file that can represent many different archive types
    /// </summary>
    public class ArchiveFile : IArchiveFile
    {
        /// <summary>
        /// Shared memory stream for accessing the provided byte data
        /// </summary>
        private readonly MemoryStream _archiveStream;

        private readonly byte[] _fileData;

        private readonly string _fileName;

        private readonly IArchive? _sourceArchive;

        private readonly ZipArchive? _zipArchive;

        private bool _disposedValue;

        public ArchiveFile(byte[] fileData, string fileName)
        {
            _fileData = fileData;
            _fileName = fileName;
            _archiveStream = new MemoryStream(_fileData);

            //We have to handle specific archive kinds explicitly, which
            //is why we have a hardcoded list of acceptable types
            if (_fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                _zipArchive = new ZipArchive(_archiveStream, ZipArchiveMode.Read);
            }
            else if (_fileName.EndsWith(".7z", StringComparison.OrdinalIgnoreCase))
            {
                _sourceArchive = SevenZipArchive.Open(_archiveStream);
            }
            else
            {
                throw new UnsupportedArchiveException(fileName);
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns an abstracted instance of a archived file
        /// for each file in the supported archive
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ArchiveFileData> GetEnumerator()
        {
            //I'm not using an if/else here because theres no reason to.
            //if both of these were somehow set to non-null, then theres
            //files in both. If not, then someone else messed up somewhere.

            if (_zipArchive != null)
            {
                foreach (ZipArchiveEntry entry in _zipArchive.Entries)
                {
                    if (!entry.FullName.EndsWith("/"))
                    {
                        yield return new ArchiveFileData(entry.Open(), entry.FullName);
                    }
                }
            }

            if (_sourceArchive != null)
            {
                foreach (IArchiveEntry entry in _sourceArchive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        yield return new ArchiveFileData(entry.OpenEntryStream(), entry.Key);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _zipArchive?.Dispose();
                    _archiveStream?.Dispose();
                    _sourceArchive?.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}