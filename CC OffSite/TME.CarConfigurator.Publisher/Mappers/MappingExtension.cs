using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public static class MappingExtension
    {
        public static String DefaultIfEmpty(this String str, String defaultStr)
        {
            return String.IsNullOrWhiteSpace(str) ? defaultStr : str;
        }
    }
}
