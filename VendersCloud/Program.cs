using VendersCloud.WebApi;
using VendersCloud.Common.Logging;
namespace DocBuilder.Web.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); }).ConfigureLogging(
                    (hostBuilderContext, logging) => {
                        logging.AddDbLogger(options => { options.Bind(hostBuilderContext.Configuration); });
                    });
    };
}