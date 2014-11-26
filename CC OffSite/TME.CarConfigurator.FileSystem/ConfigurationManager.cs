using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.FileSystem
{
    public class ConfigurationManager : IConfigurationManager
    {
        public string PathTemplate { get { return GetSection<string>("PathTemplate"); } }

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
