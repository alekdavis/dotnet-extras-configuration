# DotNetExtras.Configuration

`DotNetExtras.Configuration` is a .NET Core library that simplifies loading, reading, and transforming configuration settings of .NET applications.

Use the `DotNetExtras.Configuration` library to:

- Load configuration from JSON files, JSON strings, or `Dictionary` objects (useful for mocking application setting in unit tests).
- Retrieve application settings and assign them to strongly-typed variables including primitive types (`string`, `int`, `enum`, `boolean`), arrays, collections, lists, hash sets, and dictionaries.
- Set configuration once during application startup and access it from anywhere in the code.
- Reload configuration settings.
- 
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
config = AppSettings.Load.FromJsonFile("{\"a\":{\"b\":\"c\"}}");

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
int? b = AppSettings.GetValue<int>("KeyX:SubKeyB");
bool? c = AppSettings.GetValue<bool>("KeyX:SubKeyC");

// Get a strongly typed collection value from the configuration.
string[]? d1 = AppSettings.GetArrayValue<string[]>("KeyX:SubKeyD1");
int[]? d2 = AppSettings.GetArrayValue<int[]>("KeyX:SubKeyD2");
List<string>? e1 = AppSettings.GetListValue<List<string>>("KeyX:SubKeyE1");
List<int>? e2 = AppSettings.GetListValue<List<string>>("KeyX:SubKeyE2");
HashSet<string>? f1 = AppSettings.GetHashSetValue<string>("KeyX:SubKeyF1");
HashSet<int>? f2 = AppSettings.GetHashSetValue<string>("KeyX:SubKeyF2");
Dictionary<string, string>? g1 = AppSettings.GetDictionaryValue<string, string>("KeyX:SubKeyG1");
Dictionary<string, int>? g2 = AppSettings.GetDictionaryValue<string, int>("KeyX:SubKeyG2");

// Get a strongly typed enum value from the configuration.
MyEnum? h = AppSettings.GetEnumValue<MyEnum>("KeyX:SubKeyH");
```

## Documentation

For complete documentation, usage details, and code samples, see:

- [Documentation](https://alekdavis.github.io/dotnet-extras-configuration)
- [Unit tests](https://github.com/alekdavis/dotnet-extras-configuration/tree/main/ConfigurationTests)

## Package

Install the latest version of the `DotNetExtras.Configuration` NuGet package from:

- [https://www.nuget.org/packages/DotNetExtras.Configuration](https://www.nuget.org/packages/DotNetExtras.Configuration)


## See also

Check out other `DotNetExtras` libraries at:

- [https://github.com/alekdavis/dotnet-extras](https://github.com/alekdavis/dotnet-extras)

