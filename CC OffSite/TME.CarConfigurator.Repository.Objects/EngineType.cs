using System;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class EngineType 
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public FuelType FuelType { get; set; }
    }
}