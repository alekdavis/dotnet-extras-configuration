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
    // Redirection tests - top level
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"x\"},\"x\":{\"k1\":\"v1\",\"k2\":\"v2\"}}", "a", false, "k1:v1", "k2:v2")]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"dict\"},\"dict\":{\"name\":\"John\",\"city\":\"NYC\"}}", "a", false, "name:John", "city:NYC")]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"missing\"}}", "a", true)]
    // Redirection tests - nested level
    [InlineData("{\"s\":{\"a\":null,\"$ref\":{\"a\":\"b\"}},\"b\":{\"x\":\"y\",\"z\":\"w\"}}", "s:a", false, "x:y", "z:w")]
    [InlineData("{\"s\":{\"x\":{\"a\":null,\"$ref\":{\"a\":\"b\"}}},\"b\":{\"deep1\":\"val1\",\"deep2\":\"val2\"}}", "s:x:a", false, "deep1:val1", "deep2:val2")]
    public void AppSettings_GetDictionaryValue_StringString
    (
        string json,
        string key,
        bool isNull = false,
        params string[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        Dictionary<string, string?>? actual = AppSettings.GetDictionaryValue<string, string?>(config, key);

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
    // Redirection tests - top level
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"nums\"},\"nums\":{\"x\":10,\"y\":20,\"z\":30}}", "a", false, "x:10", "y:20", "z:30")]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":{\"first\":100,\"second\":200}}", "a", false, "first:100", "second:200")]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"missing\"}}", "a", true)]
    // Redirection tests - nested level
    [InlineData("{\"s\":{\"a\":null,\"$ref\":{\"a\":\"b\"}},\"b\":{\"n1\":99,\"n2\":88}}", "s:a", false, "n1:99", "n2:88")]
    [InlineData("{\"s\":{\"x\":{\"a\":null,\"$ref\":{\"a\":\"b\"}}},\"b\":{\"deep1\":-5,\"deep2\":-10}}", "s:x:a", false, "deep1:-5", "deep2:-10")]
    // Redirection tests - no double redirection
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":null}", "a", true)]
    public void AppSettings_GetDictionaryValue_StringInt
    (
        string json,
        string key,
        bool isNull = false,
        params string[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        Dictionary<string, int?>? actual = AppSettings.GetDictionaryValue<string, int?>(config, key);

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
