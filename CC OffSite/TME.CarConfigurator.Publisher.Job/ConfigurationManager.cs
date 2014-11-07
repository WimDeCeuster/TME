using System;

namespace TME.CarConfigurator.Publisher.Job
{
    class ConfigurationManager : IConfigurationManager
    {
        public Configuration LoadConfiguration()
        {
            return new Configuration(GetSection<string>("brand"), GetSection<string>("countries"), GetSection<string>("environment"), GetSection<string>("datasubset"), GetSection<string>("target"));
        }

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