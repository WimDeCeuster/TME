using System;

namespace TME.CarConfigurator.S3.Shared.Result
{
    public class Failed : Result
    {
        public string Reason { get; set; }
        public Exception Exception { get; set; }
    }
}