using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.AutoComparer.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLine(this StringBuilder stringBuilder, String format, params object[] args)
        {
            stringBuilder.AppendFormat(format, args);
            stringBuilder.AppendLine();
            return stringBuilder;
        }

    }
}
