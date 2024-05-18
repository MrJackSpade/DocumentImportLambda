using DocumentImportLambda.Aws.Pocos;

namespace DocumentImportLambda.Document.Pocos
{
    /// <summary>
    /// Represents a file definition to be uploaded to the document service and then stored in the client database
    /// </summary>
    public record FileUploadData(string ContentType, byte[] FileData, ParsedFileKey FileKey);
}