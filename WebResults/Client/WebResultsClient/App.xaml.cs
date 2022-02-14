using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebResultsClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider _serviceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // Create a service collection and configure our dependencies
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            // Build the our IServiceProvider and set our static reference to it
            _serviceProvider = serviceCollection.BuildServiceProvider();

            MainWindow = _serviceProvider.GetRequiredService<StartupWindow>();
            ShutdownMode = ShutdownMode.OnLastWindowClose;

            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            var jsonWriteOptions = new JsonSerializerOptions()
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            jsonWriteOptions.Converters.Add(new JsonStringEnumConverter());

            var configAsDict = Configuration.AsEnumerable().ToDictionary(c => c.Key, c => c.Value);
            var newJson = JsonSerializer.Serialize(configAsDict, jsonWriteOptions);

            var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            File.WriteAllText(appSettingsPath, newJson);

            base.OnExit(e);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddTransient(typeof(StartupWindow));
        }
    }
}
