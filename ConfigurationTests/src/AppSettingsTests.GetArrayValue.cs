using DotNetExtras.Configuration;
using Microsoft.Extensions.Configuration;

namespace ConfigurationLibTest;
public partial class AppSettingsTests
{
    [Theory]
    [InlineData("{}", "a", true)]
    [InlineData("{\"b\":[\"a\"]}", "a", true)]
    [InlineData("{\"a\":null}", "a", true)]
    [InlineData("{\"a\":[]}", "a", true)]
    [InlineData("{\"a\":[\"b\",\"c\",\"d\"]}", "a", false, "b", "c", "d")]
    [InlineData("{\"a\":{\"a\":[\"b\",\"c\",\"d\"]}}", "a:a", false, "b", "c", "d")]
    public void AppSettings_GetArrayValue_String
    (
        string json,
        string key,
        bool isNull = false,
        params string[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        string[]? actual = AppSettings.GetArrayValue<string>(config, key);

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
    [InlineData("{\"a\":[]}", "a", true)]
    [InlineData("{\"a\":[\"1\",\"2\",\"3\"]}", "a", false, 1, 2, 3)]
    [InlineData("{\"a\":[1,2,3]}", "a", false, 1, 2, 3)]
    [InlineData("{\"a\":{\"a\":[\"1\",\"2\",\"3\"]}}", "a:a", false, 1, 2, 3)]
    [InlineData("{\"a\":{\"a\":[1,2,3]}}", "a:a", false, 1, 2, 3)]
    public void AppSettings_GetArrayValue_Int
    (
        string json,
        string key,
        bool isNull = false,
        params int[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        int[]? actual = AppSettings.GetArrayValue<int>(config, key);

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
