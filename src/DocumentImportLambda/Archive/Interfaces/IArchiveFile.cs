using DocumentImportLambda.Archive.Pocos;

namespace DocumentImportLambda.Archive.Interfaces
{
    /// <summary>
    /// An archive file is simply an IEnumerable of file data contained within, for now
    /// </summary>
    public interface IArchiveFile : IDisposable, IEnumerable<ArchiveFileData>
    {
    }
}