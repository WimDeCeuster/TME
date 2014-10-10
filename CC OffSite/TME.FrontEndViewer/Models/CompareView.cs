using System.Collections.Generic;

namespace TME.FrontEndViewer.Models
{
    public class CompareView<T>
    {
        public IEnumerable<T> OldReaderModel { get; set; }
        public IEnumerable<T> NewReaderModel { get; set; }
    }
}