namespace DocumentImportLambda.Archive.Exceptions
{
    public class UnsupportedArchiveException(string fileName) : Exception($"The file '{fileName}' is not a supported archive")
    {
        public string FileName { get; set; } = fileName;
    }
}