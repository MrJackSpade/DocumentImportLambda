namespace DocumentImportLambda.Database.Models
{
    /// <summary>
    /// Represents a record in the FileData table used by SEA/RQS
    /// </summary>
    public struct FileData(long fileDataId, int fileStorageMethodInd, bool isCompressed, string? storageKey = null, byte[]? fileContents = null, bool? isEncrypted = null, Guid? salt = null, byte[]? iv = null)
    {
        public byte[]? FileContents { get; set; } = fileContents;

        public long FileDataId { get; set; } = fileDataId;

        public int FileStorageMethodInd { get; set; } = fileStorageMethodInd;

        public bool IsCompressed { get; set; } = isCompressed;

        public bool? IsEncrypted { get; set; } = isEncrypted;

        public byte[]? IV { get; set; } = iv;

        public Guid? Salt { get; set; } = salt;

        public string? StorageKey { get; set; } = storageKey;
    }
}