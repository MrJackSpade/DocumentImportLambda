using System.Text.Json.Serialization;

namespace DocumentImportLambda.Authentication.Dtos.Json.Response
{
    /// <summary>
    /// Represents the result of an auth service request for a root authentication token
    /// </summary>
    public record RootAuthToken(

        [property: JsonPropertyName("access_token")]
        string AccessToken,

        [property: JsonPropertyName("expires_in")]
        long ExpiresIn,

        [property: JsonPropertyName("token_type")]
        string TokenType
    );
}