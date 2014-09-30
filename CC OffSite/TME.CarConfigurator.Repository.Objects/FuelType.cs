using System;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class FuelType : BaseObject
    {
        [DataMember]
        public bool Hybrid { get; set; }
    }
}