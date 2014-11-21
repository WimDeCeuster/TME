using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Exceptions
{
    public class UnrecognisedColouringModeException : Exception
    {
        public UnrecognisedColouringModeException(Administration.Enums.ColouringModes mode)
            : base(String.Format("Cannot map unknown colouring mode value {0}", mode.ToString("g")))
        {

        }
    }
}
