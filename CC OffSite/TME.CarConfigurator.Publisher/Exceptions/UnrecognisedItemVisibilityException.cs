using System;

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
