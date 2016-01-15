using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionLag.Common.Configuration
{
    using System.Configuration;

    using OrionLag.Common.Diagnosis;

    public class ConfigurationLoader
    {
        private static readonly object s_syncLock = new object();
        private static IConfigurationFileReader s_configurationFileReader;
        private static ConfigurationLoader s_instance;
        private readonly Dictionary<string, System.Configuration.Configuration> m_loadedConfigurations;

        private ConfigurationLoader()
        {
            m_loadedConfigurations = new Dictionary<string, System.Configuration.Configuration>();
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
            System.Configuration.Configuration targetConfiguration = ReadConfigurationByFilePath(processName + AppString);
            return targetConfiguration;
        }

        public System.Configuration.Configuration GetConfigurationByFileName(string filePath)
        {
            return ReadConfigurationByFilePath(filePath);
        }

        public System.Configuration.Configuration GetConfigurationForRunningProcess()
        {
            return GetConfigurationByFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        private System.Configuration.Configuration ReadConfigurationByFilePath(string fileName)
        {
            if (m_loadedConfigurations.ContainsKey(fileName))
            {
                return m_loadedConfigurations[fileName];
            }

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = fileName };
            System.Configuration.Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(
                fileMap, ConfigurationUserLevel.None);
            if (configuration.HasFile)
            {
                m_loadedConfigurations.Add(fileName, configuration);
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
    }
}
