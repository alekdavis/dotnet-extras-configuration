using DotNetExtras.Configuration;
using Microsoft.Extensions.Configuration;

namespace ConfigurationLibTest;
public partial class AppSettingsTests
{
    [Theory]
    [InlineData("{}", "a", true)]
    [InlineData("{\"b\":{\"a\":\"x\"}}", "a", true)]
    [InlineData("{\"a\":null}", "a", true)]
    [InlineData("{\"a\":{}}", "a", true)]
    [InlineData("{\"a\":{\"b\":\"x\",\"c\":\"y\",\"d\":\"z\"}}", "a", false, "b:x", "c:y", "d:z")]
    [InlineData("{\"a\":{\"a\":{\"b\":\"x\",\"c\":\"y\",\"d\":\"z\"}}}", "a:a", false, "b:x", "c:y", "d:z")]
    public void AppSettings_GetDictionaryValue_StringString
    (
        string json,
        string key,
        bool isNull = false,
        params string[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        Dictionary<string, string?>? actual = AppSettings.ToDictionary<string, string?>(config, key);

        if (isNull)
        {
            Assert.Null(actual);
            return;
        }

        if (values.Length == 0)
        {
            Assert.Equal(0, actual?.Count);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(values.Length, actual.Count);
        Assert.All(actual, kvp => Assert.Contains(kvp, values.Select(v => 
        {
            string[] parts = v.Split(':');
            return new KeyValuePair<string, string?>(parts[0], parts[1]);
        })));
    }

    [Theory]
    [InlineData("{}", "a", true)]
    [InlineData("{\"b\":{\"a\":\"x\"}}", "a", true)]
    [InlineData("{\"a\":null}", "a", true)]
    [InlineData("{\"a\":{}}", "a", true)]
    [InlineData("{\"a\":{\"b\":1,\"c\":2,\"d\":3}}", "a", false, "b:1", "c:2", "d:3")]
    [InlineData("{\"a\":{\"a\":{\"b\":1,\"c\":2,\"d\":3}}}", "a:a", false, "b:1", "c:2", "d:3")]
    public void AppSettings_GetDictionaryValue_StringInt
    (
        string json,
        string key,
        bool isNull = false,
        params string[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        Dictionary<string, int?>? actual = AppSettings.ToDictionary<string, int?>(config, key);

        if (isNull)
        {
            Assert.Null(actual);
            return;
        }

        if (values.Length == 0)
        {
            Assert.Equal(0, actual?.Count);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(values.Length, actual.Count);
        Assert.All(actual, kvp => Assert.Contains(kvp, values.Select(v => 
        {
            string[] parts = v.Split(':');
            return new KeyValuePair<string, int?>(parts[0], int.Parse(parts[1]));
        })));
    }
}
