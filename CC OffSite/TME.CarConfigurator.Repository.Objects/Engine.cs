using System.Collections.Generic;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class Engine : BaseObject
    {
        [DataMember]
        public EngineType Type { get; set; }
        [DataMember]
        public EngineCategory Category { get; set; }

        [DataMember]
        public bool KeyFeature { get; set; }
        [DataMember]
        public bool Brochure { get; set; }

        [DataMember]
        public List<VisibleInModeAndView> VisibleIn { get; set; }
    }
}
