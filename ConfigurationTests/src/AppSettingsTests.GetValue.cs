// Ignore Spelling: json

using DotNetExtras.Configuration;
using Microsoft.Extensions.Configuration;

namespace ConfigurationLibTest;
public partial class AppSettingsTests
{
    [Theory]
    [InlineData("{}", "a", null)]
    [InlineData("{\"b\":\"a\"}", "a", null)]
    [InlineData("{\"a\":null}", "a", "")]
    [InlineData("{\"a\":\"\"}", "a", "")]
    [InlineData("{\"a\":\"b\"}", "a", "b")]
    [InlineData("{\"a\":100}", "a", "100")]
    [InlineData("{\"a\":true}", "a", "true")]
    [InlineData("{\"a\":false}", "a", "false")]
    [InlineData("{\"a\":{\"a\":\"b\"}}", "a:a", "b")]
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
