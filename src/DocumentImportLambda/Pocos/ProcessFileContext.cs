using DocumentImportLambda.Aws.Pocos;
using DocumentImportLambda.Database.Interfaces;

namespace DocumentImportLambda.Pocos
{
    /// <summary>
    /// This class encapsulates the current context for a file being processed so that it 
    /// can be persisted recursively, preventing unnecessary operations
    /// </summary>
    /// <param name="ClientCommandFactory"></param>
    /// <param name="IncomingFileData"></param>
    /// <param name="ShortCode"></param>
    public record ProcessFileContext(IDbCommandProvider ClientCommandFactory, IncomingFileData IncomingFileData, string ShortCode);
}