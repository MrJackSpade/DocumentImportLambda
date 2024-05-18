namespace DocumentImportLambda.Aws.Pocos
{
    public record IncomingFileData
    {
        public IncomingFileData(byte[] data, string fullName)
        {
            Data = data;
            FullName = fullName;
        }

        public IncomingFileData(byte[] data, Guid tenantId, string relativeName)
        {
            Data = data;
            FullName = $"{tenantId}/{relativeName}";
        }

        public string FullName { get; private set; }

        public byte[] Data { get; private set; }

        public string Name => Path.GetFileName(FullName);
    }
}