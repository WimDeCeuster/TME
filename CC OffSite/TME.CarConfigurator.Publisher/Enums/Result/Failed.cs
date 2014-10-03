using System;

namespace TME.CarConfigurator.Publisher.Enums.Result
{
    public class Failed : Result
    {
        public string Reason { get; set; }
        public Exception Exception { get; set; }
    }
}