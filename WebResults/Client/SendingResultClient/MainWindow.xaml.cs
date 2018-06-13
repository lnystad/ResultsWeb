﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SendingResultClient
{
    using System.Windows.Forms.Integration;

    using SendingResultClient.Common;
    using FileUploaderService.Diagnosis;
    using SendingResultClient.Viewmodels;
    using FileUploaderService.Configuration;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string DirectoryName { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            var model = this.InitViewModels();
            this.DataContext = model;
            this.InitializeComponent();

            var logfile = ConfigurationLoader.GetAppSettingsValue("LogFile");

            var Skytterlag = ConfigurationLoader.GetAppSettingsValue("Skytterlag");
            var Lisens = ConfigurationLoader.GetAppSettingsValue("Lisens");

            var LoggingLevelsString = ConfigurationLoader.GetAppSettingsValue("LoggingLevels");
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
            var model = new MainViewModel();
            return model;
        }
    }
}