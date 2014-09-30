using System.Collections.Generic;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class Model : BaseObject
    {
        [DataMember]
        public string Brand { get; set; }
        [DataMember]
        public bool Promoted { get; set; }

        [DataMember]
        public List<PublicationInfo> Publications { get; set; }
    }
}