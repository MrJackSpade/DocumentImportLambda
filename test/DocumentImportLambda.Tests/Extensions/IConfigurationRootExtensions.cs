using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Text.Json.Serialization;

namespace DocumentImportLambda.Tests.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T Deserialize<T>(this IConfigurationRoot configuration) where T : new()
        {
            var instance = new T();
            Type type = typeof(T);

            foreach (PropertyInfo prop in type.GetProperties())
            {
                // Retrieve the JsonPropertyName attribute
                JsonPropertyNameAttribute? jsonPropNameAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
                string configKey = jsonPropNameAttr?.Name ?? prop.Name;

                string? value = configuration[configKey];
                if (value != null)
                {
                    // Check if the property is an enum
                    if (prop.PropertyType.IsEnum)
                    {
                        // Use Enum.Parse to convert the string to the enum type
                        object enumValue = Enum.Parse(prop.PropertyType, value);
                        prop.SetValue(instance, enumValue);
                    }
                    else
                    {
                        // For non-enum types, use Convert.ChangeType as before
                        prop.SetValue(instance, Convert.ChangeType(value, prop.PropertyType));
                    }
                }
            }

            return instance;
        }
    }
}