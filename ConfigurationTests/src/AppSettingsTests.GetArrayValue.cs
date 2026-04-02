using DotNetExtras.Configuration;
using Microsoft.Extensions.Configuration;

namespace ConfigurationLibTest;
public partial class AppSettingsTests
{
    [Theory]
    [InlineData("{}", "a", true)]
    [InlineData("{\"b\":[\"a\"]}", "a", true)]
    [InlineData("{\"a\":null}", "a", true)]
    [InlineData("{\"a\":[]}", "a", false)]
    [InlineData("{\"a\":[\"b\",\"c\",\"d\"]}", "a", false, "b", "c", "d")]
    [InlineData("{\"a\":{\"a\":[\"b\",\"c\",\"d\"]}}", "a:a", false, "b", "c", "d")]
    // Redirection tests - top level
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"x\"},\"x\":[\"y\",\"z\"]}", "a", false, "y", "z")]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":[\"redirected1\",\"redirected2\"]}", "a", false, "redirected1", "redirected2")]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"c\"}}", "a", true)]
    // Redirection tests - nested level
    [InlineData("{\"s\":{\"a\":null,\"$ref\":{\"a\":\"b\"}},\"b\":[\"nested1\",\"nested2\"]}", "s:a", false, "nested1", "nested2")]
    [InlineData("{\"s\":{\"x\":{\"a\":null,\"$ref\":{\"a\":\"b\"}}},\"b\":[\"deep1\",\"deep2\"]}", "s:x:a", false, "deep1", "deep2")]
    // Redirection tests - no double redirection
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":null,\"c\":[\"final\"]}", "a", true)]
    public void AppSettings_GetArrayValue_String
    (
        string json,
        string key,
        bool isNull = false,
        params string[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        string[]? actual = config.GetArrayValue<string>(key);

        if (isNull)
        {
            Assert.Null(actual);
            return;
        }

        if (values.Length == 0)
        {
            Assert.Equal(0, actual?.Length);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(values.Length, actual.Length);
        Assert.All(actual, value => Assert.Contains(value, values));
    }

    [Theory]
    [InlineData("{}", "a", true)]
    [InlineData("{\"b\":[1]}", "a", true)]
    [InlineData("{\"a\":null}", "a", true)]
    [InlineData("{\"a\":[]}", "a", false)]
    [InlineData("{\"a\":[\"1\",\"2\",\"3\"]}", "a", false, 1, 2, 3)]
    [InlineData("{\"a\":[1,2,3]}", "a", false, 1, 2, 3)]
    [InlineData("{\"a\":{\"a\":[\"1\",\"2\",\"3\"]}}", "a:a", false, 1, 2, 3)]
    [InlineData("{\"a\":{\"a\":[1,2,3]}}", "a:a", false, 1, 2, 3)]
    // Redirection tests - top level
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":[10,20,30]}", "a", false, 10, 20, 30)]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"x\"},\"x\":[\"100\",\"200\"]}", "a", false, 100, 200)]
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"missing\"}}", "a", true)]
    // Redirection tests - nested level
    [InlineData("{\"s\":{\"a\":null,\"$ref\":{\"a\":\"b\"}},\"b\":[99,88,77]}", "s:a", false, 99, 88, 77)]
    [InlineData("{\"s\":{\"x\":{\"a\":null,\"$ref\":{\"a\":\"b\"}}},\"b\":[-5,-10]}", "s:x:a", false, -5, -10)]
    // Redirection tests - no double redirection
    [InlineData("{\"a\":null,\"$ref\":{\"a\":\"b\"},\"b\":null}", "a", true)]
    public void AppSettings_GetArrayValue_Int
    (
        string json,
        string key,
        bool isNull = false,
        params int[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        int[]? actual = config.GetArrayValue<int>(key);

        if (isNull)
        {
            Assert.Null(actual);
            return;
        }

        if (values.Length == 0)
        {
            Assert.Equal(0, actual?.Length);
            return;
        }

        Assert.NotNull(actual);
        Assert.Equal(values.Length, actual.Length);
        Assert.All(actual, value => Assert.Contains(value, values));
    }
}
