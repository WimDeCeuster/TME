using System;

namespace TME.CarConfigurator.CommandRepository.Result
{
    public class Failed : Result
    {
        public string Reason { get; set; }
        public Exception Exception { get; set; }
    }
}