using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using FileUploaderService.Diagnosis;

namespace FileUploaderService.Configuration
{
    public class ConfigurationLoader
    {
        private static readonly object s_syncLock = new object();
        private static IConfigurationFileReader s_configurationFileReader;
        private static ConfigurationLoader s_instance;
        private readonly Dictionary<string, System.Configuration.Configuration> m_loadedConfigurations;

        private ConfigurationLoader()
        {
            this.m_loadedConfigurations = new Dictionary<string, System.Configuration.Configuration>();
        }

        public static ConfigurationLoader GetInstance()
        {
            if (s_instance == null)
            {
                lock (s_syncLock)
                {
                    if (s_instance == null)
                    {
                        s_instance = new ConfigurationLoader();
                    }
                }
            }

            return s_instance;
        }

        public System.Configuration.Configuration GetConfigurationByProcessName(string processName)
        {
            const string AppString = ".config";
            System.Configuration.Configuration targetConfiguration = this.ReadConfigurationByFilePath(processName + AppString);
            return targetConfiguration;
        }

        public System.Configuration.Configuration GetConfigurationByFileName(string filePath)
        {
            return this.ReadConfigurationByFilePath(filePath);
        }

        public System.Configuration.Configuration GetConfigurationForRunningProcess()
        {
            return this.GetConfigurationByFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        private System.Configuration.Configuration ReadConfigurationByFilePath(string fileName)
        {
            if (this.m_loadedConfigurations.ContainsKey(fileName))
            {
                return this.m_loadedConfigurations[fileName];
            }

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = fileName };
            System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(
                fileMap, ConfigurationUserLevel.None);
            if (configuration.HasFile)
            {
                this.m_loadedConfigurations.Add(fileName, configuration);
                return configuration;
            }

            throw new InvalidOperationException(String.Format("{0} did not have a configuration file.", fileName));
        }

        public static string GetAppSettingsValue(string keyName, bool logNotFound)
        {
            s_configurationFileReader = s_configurationFileReader ?? ConfigurationFileReaderProvider.Service;
            string value = s_configurationFileReader.GetValue(keyName);
            if (logNotFound)
            {
                if (string.IsNullOrEmpty(value))
                {
                    Log.Error("Parameter {0} not found in appsettings", keyName);
                }
            }

            return value;
        }

        public static string GetAppSettingsValue(string keyName, LoggingLevels levelToLog)
        {
            s_configurationFileReader = s_configurationFileReader ?? ConfigurationFileReaderProvider.Service;
            string value = s_configurationFileReader.GetValue(keyName);

            if (string.IsNullOrEmpty(value))
            {
                switch (levelToLog)
                {
                    case LoggingLevels.Info:
                        Log.Info("Parameter {0} not found in appsettings", keyName);
                        break;
                    case LoggingLevels.Trace:
                        Log.Trace("Parameter {0} not found in appsettings", keyName);
                        break;
                    case LoggingLevels.Warning:
                        Log.Warning("Parameter {0} not found in appsettings", keyName);
                        break;
                    case LoggingLevels.Error:
                        Log.Error("Parameter {0} not found in appsettings", keyName);
                        break;
                    case LoggingLevels.None:
                        break;
                }
            }

            return value;
        }

        public static string GetAppSettingsValue(string keyName)
        {
            return GetAppSettingsValue(keyName, false);
        }
        public static void SetAppSettingsValue(string keyName,string value)
        {
            


            var fileMap = new ExeConfigurationFileMap
                { ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile };
            var config= ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            if (!setConfigValue(config, keyName, value))
            {
               
                config.AppSettings.Settings.Remove(keyName);
                config.AppSettings.Settings.Add(keyName, value);
                config.Save(ConfigurationSaveMode.Minimal);
                ConfigurationManager.RefreshSection("appSettings");
            }
            
        }

        private static bool setConfigValue(System.Configuration.Configuration config, string key, string val)
        {
            try
            {
                
                string filename = config.FilePath;

                //Load the config file as an XDocument
                XDocument document = XDocument.Load(filename, LoadOptions.PreserveWhitespace);
                if (document.Root == null)
                {
                    string errorMsg = "setConfigValue Document was null for XDocument load.";
                    Log.Error(errorMsg);
                    return false;
                }
                XElement appSettings = document.Root.Element("appSettings");
                if (appSettings == null)
                {
                    appSettings = new XElement("appSettings");
                    document.Root.Add(appSettings);
                }
                XElement appSetting = appSettings.Elements("add").FirstOrDefault(x => x.Attribute("key").Value == key);
                if (appSetting == null)
                {
                    //Create the new appSetting
                    appSettings.Add(new XElement("add", new XAttribute("key", key), new XAttribute("value", val)));
                }
                else
                {
                    //Update the current appSetting
                    appSetting.Attribute("value").Value = val;
                }


                //Format the appSetting section
                XNode lastElement = null;
                foreach (var elm in appSettings.DescendantNodes())
                {
                    if (elm.NodeType == System.Xml.XmlNodeType.Text)
                    {
                        if (lastElement?.NodeType == System.Xml.XmlNodeType.Element && elm.NextNode?.NodeType == System.Xml.XmlNodeType.Comment)
                        {
                            //Any time the last node was an element and the next is a comment add two new lines.
                            ((XText)elm).Value = "\n\n\t\t";
                        }
                        else
                        {
                            ((XText)elm).Value = "\n\t\t";
                        }
                    }
                    lastElement = elm;
                }

                //Make sure the end tag for appSettings is on a new line.
                var lastNode = appSettings.DescendantNodes().Last();
                if (lastNode.NodeType == System.Xml.XmlNodeType.Text)
                {
                    ((XText)lastNode).Value = "\n\t";
                }
                else
                {
                    appSettings.Add(new XText("\n\t"));
                }

                //Save the changes to the config file.
                document.Save(filename, SaveOptions.DisableFormatting);
                return true;
            }
            catch (Exception ex)
            {
                string errorMsg = "setConfigValue There was an exception while trying to update the config value for '" + key + "' with value '" + val + "' : " + ex.ToString();
                Log.Error(ex, errorMsg);
                return false;
            }
        }

    }
}
