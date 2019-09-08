using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Owin;

namespace Products.ReadModels.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }

    public class ProductReadModelSercie
    {

        private IWebHostBuilder webApp;
        const string baseUri = "http://localhost:8181";

        public void Start(string[] args)
        {
            Console.WriteLine("Starting Product Read Model Services...");
            webApp = WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
            webApp.Start(baseUri);
        }

        public void Stop()
        {
            

        }

    }
}
