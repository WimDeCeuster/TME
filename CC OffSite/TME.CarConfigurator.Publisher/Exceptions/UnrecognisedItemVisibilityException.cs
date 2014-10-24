using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TME.CarConfigurator.Publisher.Exceptions
{
    public class UnrecognisedItemVisibilityException : Exception
    {
        public UnrecognisedItemVisibilityException(Administration.Enums.ItemVisibility visibility)
            : base(String.Format("Cannot map unknown visibility value {0}", visibility.ToString("g")))
        {

        }
    }
}
