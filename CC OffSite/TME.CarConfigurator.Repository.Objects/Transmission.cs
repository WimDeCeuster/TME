using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    
    public class Transmission : BaseObject
    {
        
        public TransmissionType Type { get; set; }
        
        public bool KeyFeature { get; set; }       
        public bool Brochure { get; set; }
        public int NumberOfGears { get; set; }

        
        public List<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
