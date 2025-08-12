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
    public void AppSettings_GetHashSetValue_String
    (
        string json,
        string key,
        bool isNull = false,
        params string[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        HashSet<string>? actual = AppSettings.GetHashSetValue<string>(config, key);

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
    public void AppSettings_GetHashSetValueValue_Int
    (
        string json,
        string key,
        bool isNull = false,
        params int[] values
    )
    {
        IConfiguration config = AppSettings.Load.FromJsonString(json);
        HashSet<int>? actual = AppSettings.GetHashSetValue<int>(config, key);

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
        Assert.All(actual, value => Assert.Contains(value, values));
    }
}
