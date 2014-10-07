using System.Collections.Generic;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    public class BodyType : BaseObject
    {

        public int NumberOfDoors { get; set; }
        
        public int NumberOfSeats { get; set; }

        
        public List<VisibleInModeAndView> VisibleIn { get; set; }

    }
}
