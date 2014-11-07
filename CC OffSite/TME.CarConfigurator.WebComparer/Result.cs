using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.WebComparer
{
    public class Result
    {
        public IList<PageResult> ProcessedPages { get; set; }
        public IList<PageFailure> FailedPages { get; set; }

        public Result()
        {
            ProcessedPages = new List<PageResult>();
            FailedPages = new List<PageFailure>();
        }
    }
}
