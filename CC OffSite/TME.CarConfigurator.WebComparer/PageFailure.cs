using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.WebComparer
{
    public class PageFailure
    {
        public String Url { get; private set; }
        public String Reason { get; private set; }

        public PageFailure(string url, string reason)
        {
            Url = url;
            Reason = reason;
        }
    }
}
