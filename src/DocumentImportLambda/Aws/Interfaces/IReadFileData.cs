namespace DocumentImportLambda.Aws.Interfaces
{
    /// <summary>
    /// Defines and interface capable of reading file binary given a bucket path and
    /// file name, used to interface with S3
    /// </summary>
    public interface IReadFileData
    {
        /// <summary>
        /// Reads the file data from the specified location, with the specified file name
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<byte[]> ReadFileData(string bucketName, string fileName);
    }
}