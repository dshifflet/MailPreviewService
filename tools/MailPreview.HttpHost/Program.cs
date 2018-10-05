using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;

namespace MailPreview.HttpHost
{
    public class Program
    {
        //https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.1
        //To install the service
        //sc create MyService binPath= "c:\my_services\AspNetCoreService\bin\Release\netcoreapp2.1\win7-x64\publish\AspNetCoreService.exe"

        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Program));

        public static void Main(string[] args)
        {
            SetupLog4NetFromConfig();
            StartIt(!(Debugger.IsAttached || args.Contains("--console")), args);            
        }

        public static void SetupLog4NetFromConfig()
        {
            if (!new FileInfo("log4net.config").Exists) return;

            var log4NetConfig = new XmlDocument();

            log4NetConfig.Load(File.OpenRead("log4net.config"));

            var repo = log4net.LogManager.CreateRepository(
                Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));

            log4net.Config.XmlConfigurator.Configure(repo, log4NetConfig["log4net"]);
        }

        public static void StartIt(bool isService, string[] args)
        {
            Log.InfoFormat("Starting at {0} {1}", DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToShortTimeString());
            var builder = CreateWebHostBuilder(args.Where(arg => arg != "--console").ToArray());

            if (isService)
            {
                var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                var pathToContentRoot = Path.GetDirectoryName(pathToExe);
                builder.UseContentRoot(pathToContentRoot);
            }

            var host = builder.Build();
            if (isService)
            {
                host.RunAsService();
            }
            else
            {
                host.Run();
            }
            Log.InfoFormat("Shutting down at {0} {1}", DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToShortTimeString());
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var result = WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Configure the app here.
                });
            
            if (args.Any())
            {
                result.UseUrls(args.Where(o => o.StartsWith("http")).ToArray());
            }
            else if (ConfigurationManager.Hosts.Any())
            {
                result.UseUrls(ConfigurationManager.Hosts);
            }
            result.UseStartup<Startup>();
            return result;
        }
    }
}
