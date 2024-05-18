using DocumentImportLambda.Archive.Interfaces;
using DocumentImportLambda.Archive.Pocos;
using System.Collections;

namespace DocumentImportLambda.Tests.Mocks
{
    public class BulkArchiveFile : IArchiveFile
    {
        private readonly string _extension;

        private readonly int _fileCount;

        private readonly int _size;

        public BulkArchiveFile(int fileCount, int size, string extension)
        {
            _fileCount = fileCount;
            _extension = "." + extension.Trim('.');
            _size = size;
        }

        public void Dispose()
        {
        }

        public IEnumerator<ArchiveFileData> GetEnumerator()
        {
            for (int i = 0; i < _fileCount; i++)
            {
                yield return new ArchiveFileData(new MemoryStream(new byte[_size]), $"{i:0000000000}" + _extension);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}