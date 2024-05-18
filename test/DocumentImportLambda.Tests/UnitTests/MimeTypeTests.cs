using DocumentImportLambda.Document.Services;
using Xunit;

namespace DocumentImportLambda.Tests.UnitTests
{
    public class MimeTypeTests
    {
        [Fact]
        public void TestMimeType()
        {
            string extension = ".jpg";
            string result = "image/jpeg";

            Assert.Equal(result, MimeTypeService.GetMimeType(extension));
        }
    }
}