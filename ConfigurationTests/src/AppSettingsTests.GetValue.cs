// Ignore Spelling: json

using DotNetExtras.Configuration;
using Microsoft.Extensions.Configuration;

namespace ConfigurationLibTest;
public partial class AppSettingsTests
{
    [Theory]
    [InlineData("{}", "a", null)]
    [InlineData("{\"b\":\"a\"}", "a", null)]
    [InlineData("{\"a\":null}", "a", null)]
    [InlineData("{\"a\":\"\"}", "a", "")]
    [InlineData("{\"a\":\"b\"}", "a", "b")]
    [InlineData("{\"a\":100}", "a", "100")]
    [InlineData("{\"a\":true}", "a", "true")]
    [InlineData("{\"a\":false}", "a", "false")]
    [InlineData("{\"a\":{\"a\":\"b\"}}", "a:a", "b")]
    // Redirection tests - top level
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"x\"},\"x\":\"y\"}", "a", "y")]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":\"redirected\"}", "a", "redirected")]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"c\"}}", "a", null)]
    // Redirection tests - nested level
    [InlineData("{\"s\":{\"a\":null,\"$ref\":{\"a\":\"b\"}},\"b\":\"nested\"}", "s:a", "nested")]
    [InlineData("{\"s\":{\"x\":{\"a\":null,\"$ref\":{\"a\":\"b\"}}},\"b\":\"deep\"}", "s:x:a", "deep")]
    // Redirection tests - no double redirection
    public void AppSettings_GetValue_String
    (
        string json,
        string key,
        string? value
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);

        Assert.Equal(value, AppSettings.GetValue<string?>(config, key), StringComparer.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("{}", "a", null)]
    [InlineData("{\"b\":4}", "a", null)]
    [InlineData("{\"a\":null}", "a", null)]
    [InlineData("{\"a\":\"\"}", "a", null)]
    [InlineData("{\"a\":\"5\"}", "a", 5)]
    [InlineData("{\"a\":100}", "a", 100)]
    [InlineData("{\"a\":\"-1\"}", "a", -1)]
    [InlineData("{\"a\":{\"a\":9}}", "a:a", 9)]
    // Redirection tests - top level
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":42}", "a", 42)]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"x\"},\"x\":\"100\"}", "a", 100)]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"missing\"}}", "a", null)]
    // Redirection tests - nested level
    [InlineData("{\"s\":{\"a\":null,\"$ref\":{\"a\":\"b\"}},\"b\":99}", "s:a", 99)]
    [InlineData("{\"s\":{\"x\":{\"a\":null,\"$ref\":{\"a\":\"b\"}}},\"b\":-5}", "s:x:a", -5)]
    // Redirection tests - no double redirection
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":null}", "a", null)]
    public void AppSettings_GetValue_Int
    (
        string json,
        string key,
        int? value
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);

        Assert.Equal(value, AppSettings.GetValue<int?>(config, key));
    }

    [Theory]
    [InlineData("{}", "a", null)]
    [InlineData("{\"a\":null}", "a", null)]
    [InlineData("{\"a\":\"\"}", "a", null)]
    [InlineData("{\"a\":\"true\"}", "a", true)]
    [InlineData("{\"a\":\"True\"}", "a", true)]
    [InlineData("{\"a\":true}", "a", true)]
    [InlineData("{\"a\":\"false\"}", "a", false)]
    [InlineData("{\"a\":\"False\"}", "a", false)]
    [InlineData("{\"a\":false}", "a", false)]
    [InlineData("{\"a\":{\"a\":true}}", "a:a", true)]
    // Redirection tests - top level
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":true}", "a", true)]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"x\"},\"x\":false}", "a", false)]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"missing\"}}", "a", null)]
    // Redirection tests - nested level
    [InlineData("{\"s\":{\"a\":null,\"$ref\":{\"a\":\"b\"}},\"b\":true}", "s:a", true)]
    [InlineData("{\"s\":{\"x\":{\"a\":null,\"$ref\":{\"a\":\"b\"}}},\"b\":false}", "s:x:a", false)]
    public void AppSettings_GetValue_Bool
    (
        string json,
        string key,
        bool? value
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);

        Assert.Equal(value, AppSettings.GetValue<bool?>(config, key));
    }

    [Theory]
    [InlineData("{}", "a", null)]
    [InlineData("{\"a\":null}", "a", null)]
    [InlineData("{\"a\":\"\"}", "a", null)]
    [InlineData("{\"a\":\"Monday\"}", "a", DayOfWeek.Monday)]
    [InlineData("{\"a\":\"monday\"}", "a", DayOfWeek.Monday)]
    [InlineData("{\"a\":{\"a\":\"Tuesday\"}}", "a:a", DayOfWeek.Tuesday)]
    // Redirection tests - top level
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":\"Wednesday\"}", "a", DayOfWeek.Wednesday)]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"x\"},\"x\":\"Friday\"}", "a", DayOfWeek.Friday)]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"missing\"}}", "a", null)]
    // Redirection tests - nested level
    [InlineData("{\"s\":{\"a\":null,\"$ref\":{\"a\":\"b\"}},\"b\":\"Saturday\"}", "s:a", DayOfWeek.Saturday)]
    [InlineData("{\"s\":{\"x\":{\"a\":null,\"$ref\":{\"a\":\"b\"}}},\"b\":\"Sunday\"}", "s:x:a", DayOfWeek.Sunday)]
    public void AppSettings_GetValue_Enum
    (
        string json,
        string key,
        DayOfWeek? value
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);

        Assert.Equal(value, AppSettings.GetValue<DayOfWeek?>(config, key));
    }
}
