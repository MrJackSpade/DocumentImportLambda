using DocumentImportLambda.Aws.Exceptions;
using DocumentImportLambda.Utilities;

namespace DocumentImportLambda.Aws.Pocos
{
    /// <summary>
    /// Breaks down the "Key" provided by S3 into its constituents
    /// </summary>
    public class ParsedFileKey
    {
        public ParsedFileKey(string path)
        {
            Ensure.NotNullOrWhiteSpace(path);

            path = path.Trim('/');

            int firstSlash = path.IndexOf('/');

            if (firstSlash == -1 || !Guid.TryParse(path[..firstSlash], out Guid tenantId))
            {
                throw new InvalidFileKeyException(path);
            }

            TenantId = tenantId;
            RelativeFilePath = path[(firstSlash + 1)..];
            Extension = Path.GetExtension(path);
        }

        public string Extension { get; private set; }

        public string RelativeFilePath { get; private set; }

        public Guid TenantId { get; private set; }
    }
}