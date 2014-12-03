using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Administration.Assets;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class StringExtensions
    {
        public static String DefaultIfEmpty(this String str, String defaultStr)
        {
            return String.IsNullOrWhiteSpace(str) ? defaultStr : str;
        }


    }
}
