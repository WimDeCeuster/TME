using System.Collections;
using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;

namespace TME.FrontEndViewer.Models
{
    public class ComparingViewModel
    {
        public IList<IModel> OldReaderModel { get; set; }
        public IList<IModel> NewReaderModel { get; set; }
    }
}