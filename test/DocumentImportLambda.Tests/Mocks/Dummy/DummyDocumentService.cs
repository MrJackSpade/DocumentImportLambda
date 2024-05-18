using DocumentImportLambda.Document.Dtos.Json.Request;
using DocumentImportLambda.Document.Interfaces;

namespace DocumentImportLambda.Tests.Mocks.Dummy
{
    internal class DummyDocumentService : IDocumentService
    {
        private readonly List<CreateDocumentRequest> _createDocumentRequests = new();

        public int UploadedCount => _createDocumentRequests.Count;

        public bool HasUploaded(string fileName)
        {
            foreach (CreateDocumentRequest request in _createDocumentRequests)
            {
                foreach (DocumentVersionRequest version in request.Versions)
                {
                    if (version.FileName == fileName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Task<Guid> Upload(CreateDocumentRequest request, string authToken)
        {
            _createDocumentRequests.Add(request);

            return Task.FromResult(Guid.NewGuid());
        }
    }
}