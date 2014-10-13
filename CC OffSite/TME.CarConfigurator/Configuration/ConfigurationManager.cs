using System;
using TME.CarConfigurator.Interfaces.Configuration;

namespace TME.CarConfigurator.Configuration
{
    public class ConfigurationManager : IConfigurationManager
    {
        public string Environment { get { return GetSection<string>("Environment"); } }

        public string DataSubset { get { return GetSection<string>("DataSubset"); } }

        public string AmazonAccessKeyId { get { return GetSection<string>("AmazonAccessKeyId"); } }

        public string AmazonSecretAccessKey { get { return GetSection<string>("AmazonSecretAccessKey"); } }

        private static T GetSection<T>(string sectionName) where T : class
        {
            try
            {
                return System.Configuration.ConfigurationManager.AppSettings[sectionName] as T;
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}