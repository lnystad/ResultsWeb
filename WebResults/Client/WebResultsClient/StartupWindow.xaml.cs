using System;
using System.Windows;

namespace WebResultsClient 
{
    using FileUploaderService.Diagnosis;
    using Microsoft.Extensions.Configuration;
    using WebResultsClient.Viewmodels;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        private MainViewModel m_model;
        public string DirectoryName { get; set; }
        private readonly IServiceProvider m_serviceProvider;
        private readonly IConfiguration m_configuration;

        public StartupWindow(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            m_serviceProvider = serviceProvider;
            m_configuration = configuration;

            InitializeComponent();
            m_model = this.InitViewModels();
            this.DataContext = m_model;

            var logfile = configuration["LogFile"];

            var Skytterlag = configuration["Skytterlag"];
            var Lisens = configuration["Lisens"];

            var LoggingLevelsString = configuration["LoggingLevels"];
            LoggingLevels enumLowestTrace = LoggingLevels.Info;
            if (!string.IsNullOrEmpty(LoggingLevelsString))
            {
                if (!Enum.TryParse(LoggingLevelsString, true, out enumLowestTrace))
                {
                    enumLowestTrace = LoggingLevels.Info;
                }
            }

            var fileAppsender = new FileAppender(logfile, enumLowestTrace, LoggingLevels.Trace);
            Log.AddAppender(fileAppsender);
            //if (!LisensChecker.Validate(Skytterlag, DateTime.Now, Lisens))
            //{
            //    Log.Error("Lisens not valid for {0}", Skytterlag);
            //    System.Windows.Application.Current.Shutdown();
            //}

            Log.Info("Sending Result Client started");

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomainUnhandledException;
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error(e.ExceptionObject as Exception, "Unhandled exception");
        }

        private MainViewModel InitViewModels()
        {
            var model = new MainViewModel(m_serviceProvider, m_configuration);
             return model;
            
        }

        
       
        private void TreeViewItem_OnItemSelected(object sender, RoutedEventArgs e)
        {
            m_model.BitMapSelectionViewModel.TreeViewItem_OnItemSelected(sender, e);
           
        }

        private void ExportFile_Click(object sender, RoutedEventArgs e)
        {
            m_model.BitMapSelectionViewModel.ExportFile_Click(sender, e);
        }

        private void ExportFolder_Click(object sender, RoutedEventArgs e)
        {
            m_model.BitMapSelectionViewModel.ExportFolder_Click(sender, e);
        }

        private void DeleteFolder_Click(object sender, RoutedEventArgs e)
        {
            m_model.BitMapSelectionViewModel.DeleteFolder_Click(sender, e);
        }
        private void RefreshFolder_Click(object sender, RoutedEventArgs e)
        {
            m_model.BitMapSelectionViewModel.RefreshFolder_Click(sender, e);
        }

        

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
