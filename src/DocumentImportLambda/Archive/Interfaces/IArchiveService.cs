namespace DocumentImportLambda.Archive.Interfaces
{
    /// <summary>
    /// Allows validating archive types and opening them
    /// </summary>
    public interface IArchiveService
    {
        /// <summary>
        /// Returns true if the archive type is supported by the implementation
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool IsSupported(string fileName);

        /// <summary>
        /// Returns an abstracted definition of an archive file
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IArchiveFile Open(byte[] fileData, string fileName);
    }
}