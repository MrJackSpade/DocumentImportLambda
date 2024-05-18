using DocumentImportLambda.Utilities;
using System.Text.Json.Serialization;

namespace DocumentImportLambda.Authentication.Dtos.Json.Response
{
    /// <summary>
    /// A token representing an individual tenant leval authentication with the OAuth service
    /// </summary>
    /// <param name="accessToken"></param>
    /// <param name="idToken"></param>
    /// <param name="refreshToken"></param>
    [method: JsonConstructor]
    public class TenantAuthToken(string accessToken, string idToken, string refreshToken)
    {
        public string AccessToken { get; } = Ensure.NotNullOrWhiteSpace(accessToken);

        public string IdToken { get; } = Ensure.NotNullOrWhiteSpace(idToken);

        public string RefreshToken { get; } = Ensure.NotNullOrWhiteSpace(refreshToken);
    }
}