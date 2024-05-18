using DocumentImportLambda.Document.Dtos.Json.Request;

namespace DocumentImportLambda.Document.Interfaces
{
    /// <summary>
    /// Defines an interface used to upload a document to the DocumentService
    /// </summary>
    public interface IDocumentService
    {
        Task<Guid> Upload(CreateDocumentRequest request, string authToken);
    }
}