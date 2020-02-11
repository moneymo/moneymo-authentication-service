using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.FileExtensions;
using Moneymo.AuthenticationService.Core;

namespace Moneymo.AuthenticationService.Core.Tests
{
    public class TestHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static AuthenticationServiceConfiguration GetApplicationConfiguration(string outputPath)
        {
            var configuration = new AuthenticationServiceConfiguration();

            var iConfig = GetIConfigurationRoot(outputPath);

            iConfig
                .GetSection("AuthenticationService")
                .Bind(configuration);

            return configuration;
        }
    }
}