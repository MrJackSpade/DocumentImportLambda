using DocumentImportLambda.Aws.Pocos;
using Xunit;

namespace DocumentImportLambda.Tests.UnitTests
{
    public class FileKeyTests
    {
        [Fact]
        public void TestFileKeyParse()
        {
            var d = Guid.NewGuid();

            string testPath = $"{d}/test.pdf";

            var fileKey = new ParsedFileKey(testPath);

            Assert.Equal("test.pdf", fileKey.RelativeFilePath);

            Assert.Equal(d, fileKey.TenantId); ;
        }
    }
}