using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Exceptions
{
    public class CorruptDataException : Exception
    {
        public CorruptDataException(String message)
            : base (message)
        {

        }
    }
}
