using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class Generation : BaseObject
    {
        [DataMember]
        public String SSN { get; set; }

        [DataMember]
        public CarConfiguratorVersion CarConfiguratorVersion { get; set; }

        [DataMember]
        public List<Link> Links { get; set; }

        [DataMember]
        public List<Asset> Assets { get; set; }
    }
}