namespace DocumentImportLambda.Aws.Exceptions
{
    /// <summary>
    /// An exception thrown when the Key (returned by S3) doesn't match the format expected for processing files
    /// </summary>
    /// <param name="fileKey"></param>
    public class InvalidFileKeyException(string fileKey) : Exception($"The file key '{fileKey}' was not properly formatted")
    {
        public string FileKey { get; private set; } = fileKey;
    }
}