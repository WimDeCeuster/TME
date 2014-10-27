using System.Collections.Generic;

namespace TME.CarConfigurator.Repository.Objects.Packs
{
    public class GradePack : Pack
    {

        public bool Standard { get; set; }
        public bool Optional { get; set; }
        public bool NotAvailable { get; set; }

        public IEnumerable<CarInfo> StandardOn { get; set; }
        public IEnumerable<CarInfo> OptionalOn { get; set; }
        public IEnumerable<CarInfo> NotAvailableOn { get; set; }
    }
}
