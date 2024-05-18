using DocumentImportLambda.Utilities;
using System.Text.Json;

namespace DocumentImportLambda.Authentication.Dtos.Json.Response
{
    /// <summary>
    /// Represents and parses a Tenant API response containing short codes for SEA/SCS for a tenant
    /// </summary>
    public class TenantApiResponse
    {
        private const string SCS_TENANT = "scsTenant";

        private const string SEA_TENANT = "seaTenant";

        public TenantApiResponse(string json)
        {
            Ensure.NotNullOrWhiteSpace(json);

            ParseJsonContent(json);
        }

        public TenantApiResponse(string scsCode, string seaCode)
        {
            ScsCode = scsCode;
            SeaCode = seaCode;
        }

        public string? ScsCode { get; private set; }

        public string? SeaCode { get; private set; }

        private void ParseJsonArray(JsonElement array)
        {
            if (array.GetArrayLength() > 0)
            {
                JsonElement firstItem = array[0];
                ParseJsonObject(firstItem);
            }
        }

        private void ParseJsonContent(string contentData)
        {
            using var doc = JsonDocument.Parse(contentData);
            JsonElement root = doc.RootElement;

            switch (root.ValueKind)
            {
                case JsonValueKind.Object:
                    ParseJsonObject(root);
                    break;

                case JsonValueKind.Array:
                    ParseJsonArray(root);
                    break;
            }
        }

        private void ParseJsonObject(JsonElement obj)
        {
            ScsCode = obj.TryGetProperty(SCS_TENANT, out JsonElement scsTenant) ? scsTenant.GetString() : "";
            SeaCode = obj.TryGetProperty(SEA_TENANT, out JsonElement seaTenant) ? seaTenant.GetString() : "";
        }
    }
}