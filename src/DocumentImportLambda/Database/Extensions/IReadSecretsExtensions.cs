using DocumentImportLambda.Aws.Interfaces;

namespace DocumentImportLambda.Database.Extensions
{
    public static class IReadSecretsExtensions
    {
        /// <summary>
        /// Reads a secret as an enum value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="readSecrets"></param>
        /// <param name="secretName"></param>
        /// <returns></returns>
        public static async Task<T> Read<T>(this IReadSecrets readSecrets, string secretName) where T : Enum
        {
            string value = await readSecrets.Read(secretName);

            return (T)Enum.Parse(typeof(T), value);
        }
    }
}