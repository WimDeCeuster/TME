using System.Collections.Generic;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
   [DataContract]
    public class EngineCategory : BaseObject
    {
       [DataMember]
       public List<Asset> Assets { get; set; }
    }
}
