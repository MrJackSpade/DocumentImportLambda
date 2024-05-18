using DocumentImportLambda.Aws.Interfaces;

namespace DocumentImportLambda.Tests.Mocks
{
    internal class ReadResourceFile : IReadFileData
    {
        private readonly byte[] _data;

        public ReadResourceFile(string fileName)
        {
            _data = File.ReadAllBytes(Path.Combine("Resources", fileName));
        }

        public Task<byte[]> ReadFileData(string bucketName, string fileName)
        {
            return Task.FromResult(_data);
        }
    }
}