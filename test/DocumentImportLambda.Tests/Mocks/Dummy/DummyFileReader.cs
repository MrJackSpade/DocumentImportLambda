using DocumentImportLambda.Aws.Interfaces;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    internal class DummyFileReader : IReadFileData
    {
        private readonly byte[] _bytes;

        public DummyFileReader(byte[]? bytes = null)
        {
            _bytes = bytes ?? Array.Empty<byte>();
        }

        public Task<byte[]> ReadFileData(string bucketName, string fileName)
        {
            return Task.FromResult(_bytes);
        }
    }
}