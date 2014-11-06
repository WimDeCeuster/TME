using System.Collections.Generic;
using TME.CarConfigurator.Administration;

namespace TME.FrontEndViewer.Models
{
    public class ModelCompare
    {
        public string SelectedCountry { get; set; }
        public string SelectedLanguage { get; set; }
        public List<Country> Countries { get; set; }

        public ModelWithMetrics<IReadOnlyList<TME.CarConfigurator.Interfaces.IModel>> OldReaderModel { get; set; }
        public ModelWithMetrics<IReadOnlyList<TME.CarConfigurator.Interfaces.IModel>> NewReaderModel { get; set; }       
    }
}