using System;
using System.Collections.Generic;
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

            var root = Configuration.GetChildren().ToList();
            var dictionary = new Dictionary<string, object>();
            RecursiveDictConfig(root, dictionary);

            var newJson = JsonSerializer.Serialize(dictionary, jsonWriteOptions);

            var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            File.WriteAllText(appSettingsPath, newJson);

            base.OnExit(e);
        }

        public void RecursiveListConfig(List<IConfigurationSection> children, List<object> list)
        {
            foreach (var child in children)
            {
                if (child.Value != null)
                {
                    list.Add(child.Value);
                }
                else
                {
                    var subChildren = child.GetChildren().ToList();
                    var isList = true;
                    for (int i = 0; i < subChildren.Count; i++)
                    {
                        if (subChildren[i].Key != i.ToString())
                        {
                            isList = false;
                        }
                    }

                    if (isList)
                    {
                        List<object> listValues = new List<object>();
                        RecursiveListConfig(subChildren, listValues);
                        list.Add(listValues);
                    }
                    else
                    {
                        Dictionary<string, object> dictValues = new Dictionary<string, object>();
                        RecursiveDictConfig(subChildren, dictValues);
                        list.Add(dictValues);
                    }
                }
            }
        }

        public void RecursiveDictConfig(List<IConfigurationSection> children, Dictionary<string, object> dict)
        {
            foreach(var child in children)
            {
                if (child.Value != null)
                {
                    dict[child.Key] = child.Value;
                }
                else
                {
                    var subChildren = child.GetChildren().ToList();
                    var isList = true;
                    for(int i = 0; i < subChildren.Count; i++)
                    {
                        if(subChildren[i].Key != i.ToString())
                        {
                            isList = false;
                        }
                    }

                    if(isList)
                    {
                        List<object> listValues = new List<object>();
                        RecursiveListConfig(subChildren, listValues);
                        dict[child.Key] = listValues;
                    }else
                    {
                        Dictionary<string, object> dictValues = new Dictionary<string, object>();
                        RecursiveDictConfig(subChildren, dictValues);
                        dict[child.Key] = dictValues;
                    }
                }
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Configuration);

            services.AddTransient(typeof(StartupWindow));
        }
    }
}
