using DocumentImportLambda.Archive.Interfaces;

namespace DocumentImportLambda.Tests.Mocks
{
    internal class BulkArchiveFileProvider : IArchiveService
    {
        private readonly string _extension;

        private readonly int _fileCount;

        private readonly int _fileSize;

        private readonly List<string> _supportedExtensions;

        public BulkArchiveFileProvider(int fileCount, int fileSize, string extension, params string[] supportedExtensions)
        {
            _extension = "." + extension.Trim('.');
            _fileCount = fileCount;
            _fileSize = fileSize;

            _supportedExtensions = new List<string>();

            foreach (string supportedExtension in supportedExtensions)
            {
                _supportedExtensions.Add(supportedExtension.ToLower().Trim('.'));
            }
        }

        public bool IsSupported(string fileName)
        {
            string extension = Path.GetExtension(fileName).Trim('.').ToLower();

            return _supportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        public IArchiveFile Open(byte[] fileData, string fileName)
        {
            return new BulkArchiveFile(_fileCount, _fileSize, _extension);
        }
    }
}