using Microsoft.Extensions.Configuration;

namespace DruidsCornerUnitTests.TestHelpers
{
    public static class ConfiHelper
    {
        public static IConfiguration GenerateEmptyFakeConfig()
        {
            // Inspired from https://stackoverflow.com/a/55497919
            var myConfiguration = new Dictionary<string, string>
            {
                {"Key1", "Value1"},
                {"Nested:Key1", "NestedValue1"},
                {"Nested:Key2", "NestedValue2"}
            };

            // Disabling because this just some test code, won't pose any real issue.
            #pragma warning disable CS8620
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(myConfiguration)
                .Build();
            return configuration;
        }
    }
}
