using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace MailPreview.HttpHost
{
    static class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }
        public static string[] Hosts { get; }
        public static string ApiName { get; }
        public static string MailPath { get; }
        public static string PathBase { get; }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

            ApiName = AppSetting["ApiName"];
            Hosts = AppSetting.GetSection("Hosts").GetChildren().ToArray().Select(c => c.Value).ToArray();
            MailPath = AppSetting["MailPath"];
            PathBase = AppSetting["PathBase"];
        }
    }
}