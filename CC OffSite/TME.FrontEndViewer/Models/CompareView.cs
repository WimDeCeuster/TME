using System.Collections.Generic;

namespace TME.FrontEndViewer.Models
{
    public class CompareView<T>
    {
        public IList<T> OldReaderModel { get; set; }
        public IList<T> NewReaderModel { get; set; }
    }
}