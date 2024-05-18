namespace DocumentImportLambda.Archive.Pocos
{
    /// <summary>
    /// Represents a single abstracted file from within an archive
    /// </summary>
    /// <param name="dataSource"></param>
    /// <param name="fullName"></param>
    public class ArchiveFileData(Stream dataSource, string fullName) : IDisposable
    {
        private readonly Stream _dataSource = dataSource;

        private byte[]? _cachedBytes;

        private bool _disposedValue;

        public string FullName { get; private set; } = fullName;

        public string Name => Path.GetFileName(FullName);

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the file data from the underlying source
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            if (_cachedBytes == null)
            {
                using var memoryStream = new MemoryStream();

                _dataSource.CopyTo(memoryStream);

                _cachedBytes = memoryStream.ToArray();

                _dataSource.Dispose();
            }

            return _cachedBytes;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _dataSource?.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}