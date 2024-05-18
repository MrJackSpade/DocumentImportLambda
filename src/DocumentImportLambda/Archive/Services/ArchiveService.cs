using DocumentImportLambda.Archive.Exceptions;
using DocumentImportLambda.Archive.Interfaces;
using DocumentImportLambda.Archive.Pocos;

namespace DocumentImportLambda.Archive.Services
{
    /// <summary>
    /// Checks if an archive is supported for unzipping, and exposes a method to open it.
    /// </summary>
    public class ArchiveService : IArchiveService
    {
        /// <summary>
        /// Only allow for opening these extensions
        /// </summary>
        private readonly List<string> _supportedExtensions;

        public ArchiveService(params string[] supportedExtensions)
        {
            _supportedExtensions = [];

            foreach (string extension in supportedExtensions)
            {
                _supportedExtensions.Add(extension.ToLower().Trim('.'));
            }
        }

        /// <summary>
        /// Returns true if the filename is a valid archive type supported by this service
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool IsSupported(string fileName)
        {
            string extension = Path.GetExtension(fileName).Trim('.').ToLower();

            return _supportedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Opens the archive based on the provided byte[] and returns an abstracted archive instance
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="UnsupportedArchiveException"></exception>
        public IArchiveFile Open(byte[] fileData, string fileName)
        {
            if (!IsSupported(fileName))
            {
                throw new UnsupportedArchiveException(fileName);
            }

            return new ArchiveFile(fileData, fileName);
        }
    }
}