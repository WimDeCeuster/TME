using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TME.FrontEndViewer.Models
{
    public class ModelWithMetrics<T>
    {
        public TimeSpan TimeToLoad { get; set; }
        public List<T> Model { get; set; }
    }
}