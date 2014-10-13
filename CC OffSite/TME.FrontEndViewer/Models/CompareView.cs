using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.FrontEndViewer.Models
{
    public class CompareView<T>
    {
        public ModelWithMetrics<T> OldReaderModel { get; set; }
        public ModelWithMetrics<T> NewReaderModel { get; set; }       
    }
}