using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    
    public class Engine : BaseObject
    {
        
        public EngineType Type { get; set; }       
        public EngineCategory Category { get; set; }
        
        public bool KeyFeature { get; set; }
        public bool Brochure { get; set; }
       
        public List<VisibleInModeAndView> VisibleIn { get; set; }

    }
}
