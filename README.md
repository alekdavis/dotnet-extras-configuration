# DotNetExtras.Configuration

`DotNetExtras.Configuration` is a .NET Core library that simplifies loading, reading, and transforming configuration settings of .NET applications.

Use the `DotNetExtras.Configuration` library to:

- Load configuration from JSON files, JSON strings, or `Dictionary` objects (useful for mocking application setting in unit tests).
- Retrieve application settings and assign them to strongly-typed variables including primitive types (`string`, `int`, `enum`, `boolean`), arrays, collections, lists, hash sets, and dictionaries.
- Reference configuration values from other configuration keys using the `$ref` key for value redirection.
- Build configuration values using templates with placeholders that are replaced by values from other configuration keys.
- Set configuration once during application startup and access it from anywhere in the code.
- Reload configuration settings.

## Usage

The following examples illustrate how to use the `DotNetExtras.Configuration` API: 

```cs
using Microsoft.Extensions.Configuration;
using DotNetExtras.Configuration;
...
IConfiguration? config = null;

// Load configuration from a JSON file.
config = AppSettings.Load.FromJsonFile("C:\\AppSettings.json");

// Load configuration from a JSON string.
config = AppSettings.Load.FromJsonString("{\"a\":{\"b\":\"c\"}}");

// Load configuration from a dictionary (keys of array elements must be indexed).
config = AppSettings.Load.FromDictionary(
    new Dictionary<string,string?>
    {
        {"A", "ValueA"},
        {"B:X", "ValueB"},
        {"C:Z:0", "ValueC"},
        {"C:Z:1", "ValueD"},
        {"C:Z:2", "ValueE"},
    }
);

// Save configuration in a static global variable.
AppSettings.Global.Set(config);

// Get configuration from the static global variable.
config = AppSettings.Global.Get();

// Reinitialize the configuration from the original provider.
AppSettings.Reload(config);

// Get a strongly-typed primitive value from the configuration.
string? a = AppSettings.GetValue<string>("KeyX:SubKeyA");
int?    b = AppSettings.GetValue<int>("KeyX:SubKeyB");
bool?   c = AppSettings.GetValue<bool>("KeyX:SubKeyC");

// Get a strongly typed array value from the configuration.
string[]? d1 = AppSettings.GetArrayValue<string[]>("KeyX:SubKeyD1");
int[]?    d2 = AppSettings.GetArrayValue<int[]>("KeyX:SubKeyD2");

// Get a strongly typed list value from the configuration.
List<string>? e1 = AppSettings.GetListValue<List<string>>("KeyX:SubKeyE1");
List<int>?    e2 = AppSettings.GetListValue<List<string>>("KeyX:SubKeyE2");

// Get a strongly typed hash set value from the configuration.
HashSet<string>? f1 = AppSettings.GetHashSetValue<string>("KeyX:SubKeyF1");
HashSet<int>?    f2 = AppSettings.GetHashSetValue<string>("KeyX:SubKeyF2");

// Get a strongly typed dictionary value from the configuration.
Dictionary<string, string>? g1 = AppSettings.GetDictionaryValue<string, string>("KeyX:SubKeyG1");
Dictionary<string, int>?    g2 = AppSettings.GetDictionaryValue<string, int>("KeyX:SubKeyG2");

// Get a strongly typed enum value from the configuration.
MyEnum? h = AppSettings.GetEnumValue<MyEnum>("KeyX:SubKeyH");
```
### Key redirection
The library supports configuration value redirection using the `$ref` key, which allows you to reference values from other configuration keys. This is useful for avoiding duplication and maintaining configuration consistency.

When a configuration key's value is `null` or does not exist, the library checks for a `$ref` key at the same hierarchical level. The `$ref` key contains the name of the original key as a property, with its value pointing to the target key from which to retrieve the actual value.

Here is an example of the JSON configuration that demonstrates the use of `$ref` for value redirection (the client secret value is provided for demonstration only, in a real application it should be protected):

```json
{
  "ServiceA": {
    "ClientId": "ab2309cd-1234-4ef0-9876-abcdef123456",
    "ClientSecret": "s3cr3tV@lu3"
  },
  "ServiceB": {
    "$ref": {
      // Redirect ServiceB's ClientId and ClientSecret to ServiceA.
      "ClientId": "ServiceA:ClientId" 
      "ClientSecret": "ServiceA:ClientSecret"
    },
  }"
}
```

No special code is needed to support `$ref` redirection; it is handled automatically by the `DotNetExtras.Configuration` library when retrieving configuration values.

### Template substitutions

The `$ref` redirection also supports template substitutions using placeholders in the format `{key}`. This allows you to compose configuration values by combining literal text with values from other configuration keys.

Placeholders are replaced with their corresponding configuration values when the setting is retrieved. You can use multiple placeholders in a single template and reference nested configuration keys using the colon (`:`) notation. 

Configuration settings that use template substitutions apply only to string data types, but the template placeholders may also reference numbers and boolean values.

To include literal braces in the template, use double braces: `{{` for `{` and `}}` for `}`.

Here is an example of the JSON configuration that demonstrates template substitutions:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "company.onmicrosoft.com",
    "TenantId": "12398def-e301-5432-10234-3aa74f2e244c",
    "ClientId": "def0b18c-611a-bcd5-f10d-44dc5f4ee5a"
  },
  "ServiceA": {
    "$ref": {
      "TokenUrl": "https://login.microsoftonline.com/{AzureAd:TenantId}/oauth2/v2.0/token",
      "Scope": "api://{AzureId:ClientId}/.default"
    }
    "BaseUrl": "https://api.company.com/v1"
  },
}

```
In this example, the `ServiceA:TokenUrl` and `ServiceA:Scope` configuration values are constructed using templates that reference the `AzureAd:TenantId` and `AzureAd:ClientId` keys. When retrieved, these values will be fully resolved to `https://login.microsoftonline.com/12398def-e301-5432-10234-3aa74f2e244c/oauth2/v2.0/token` and `api://def0b18c-611a-bcd5-f10d-44dc5f4ee5a/.default`, respectively.

As with simple `$ref` redirection, no special code is needed to support template substitutions; it is handled automatically by the `DotNetExtras.Configuration` library when retrieving configuration values.

## Documentation

For the complete documentation, usage details, and code samples, see:

- [Documentation](https://alekdavis.github.io/dotnet-extras-configuration)
- [Unit tests](https://github.com/alekdavis/dotnet-extras-configuration/tree/main/ConfigurationTests)

## Package

Install the latest version of the `DotNetExtras.Configuration` NuGet package from:

- [https://www.nuget.org/packages/DotNetExtras.Configuration](https://www.nuget.org/packages/DotNetExtras.Configuration)

## See also

Check out other `DotNetExtras` libraries at:

- [https://github.com/alekdavis/dotnet-extras](https://github.com/alekdavis/dotnet-extras)

