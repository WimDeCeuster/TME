using System.Collections.Generic;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class BodyType : BaseObject
    {
        [DataMember]
        public int NumberOfDoors { get; set; }
        [DataMember]
        public int NumberOfSeats { get; set; }

        [DataMember]
        public List<VisibleInModeAndView> VisibleIn { get; set; }

    }
}
