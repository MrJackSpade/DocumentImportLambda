using DocumentImportLambda.Archive.Exceptions;
using DocumentImportLambda.Archive.Interfaces;
using DocumentImportLambda.Archive.Pocos;
using DocumentImportLambda.Archive.Services;
using Xunit;

namespace DocumentImportLambda.Tests.UnitTests
{
    public class ArchiveServiceTests
    {
        private const string TestPayload = "(V,08£a}-tEA;C]0XLYG8gH5~`B}5m/b)x{)91W£bYEItP,Y'5";

        private readonly IArchiveService _archiveService;

        public ArchiveServiceTests()
        {
            _archiveService = new ArchiveService("7z", "zip");
        }

        [Fact]
        public void GetFileDatas_7zFile_ReturnsCorrectFileData()
        {
            // Arrange
            string fileName = "TextDoc.7z";
            byte[] fileData = File.ReadAllBytes(Path.Combine("Resources", fileName));

            // Act
            var archiveFile = _archiveService.Open(fileData, fileName).ToList();
            var archiveFiles = archiveFile.ToList();
            ArchiveFileData? archiveFileData = archiveFiles.FirstOrDefault();

            // Assert
            Assert.NotNull(archiveFiles);
            Assert.Single(archiveFiles);
            Assert.Equal("TextDoc.txt", archiveFileData!.Name);

            byte[] fileBytes = archiveFileData.GetBytes();
            string fileString = System.Text.Encoding.UTF8.GetString(fileBytes);

            Assert.Equal(TestPayload, fileString);
        }

        [Fact]
        public void GetFileDatas_RarFile_ThrowsUnsupportedArchiveException()
        {
            // Arrange
            string fileName = "TestDoc.rar";
            byte[] fileData = Array.Empty<byte>();

            // Act & Assert
            Assert.Throws<UnsupportedArchiveException>(() => _archiveService.Open(fileData, fileName).ToList());
        }

        [Fact]
        public void GetFileDatas_ZipFile_ReturnsCorrectFileData()
        {
            // Arrange
            string fileName = "TextDoc.zip";
            byte[] fileData = File.ReadAllBytes(Path.Combine("Resources", fileName));

            // Act
            var archiveFile = _archiveService.Open(fileData, fileName).ToList();
            var archiveFiles = archiveFile.ToList();
            ArchiveFileData? archiveFileData = archiveFiles.FirstOrDefault();

            // Assert
            Assert.NotNull(archiveFiles);
            Assert.Single(archiveFiles);
            Assert.Equal("TextDoc.txt", archiveFileData!.Name);

            byte[] fileBytes = archiveFileData.GetBytes();
            string fileString = System.Text.Encoding.UTF8.GetString(fileBytes);

            Assert.Equal(TestPayload, fileString);
        }

        [Fact]
        public void IsSupported_7zFile_ReturnsTrue()
        {
            // Arrange
            string fileName = "TestDoc.7z";

            // Act
            bool isSupported = _archiveService.IsSupported(fileName);

            // Assert
            Assert.True(isSupported);
        }

        [Fact]
        public void IsSupported_RarFile_ReturnsFalse()
        {
            // Arrange
            string fileName = "TestDoc.rar";

            // Act
            bool isSupported = _archiveService.IsSupported(fileName);

            // Assert
            Assert.False(isSupported);
        }

        [Fact]
        public void IsSupported_ZipFile_ReturnsTrue()
        {
            // Arrange
            string fileName = "TestDoc.zip";

            // Act
            bool isSupported = _archiveService.IsSupported(fileName);

            // Assert
            Assert.True(isSupported);
        }
    }
}