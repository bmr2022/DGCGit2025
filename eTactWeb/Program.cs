using Serilog;
using Serilog.Formatting.Compact;

namespace eTactWeb
{
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
    public class Program
#pragma warning restore CA1052 // Static holder types should be Static or NotInheritable
    {
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(msg => Console.WriteLine($"Serilog error: {msg}"));

            var logFolder = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(logFolder))
            {
                Directory.CreateDirectory(logFolder);
            }

            // Set up Serilog
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(new Serilog.Formatting.Compact.RenderedCompactJsonFormatter())
                .WriteTo.Debug(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3} {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(Path.Combine(logFolder, "eTactWeb-.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }
    }
}