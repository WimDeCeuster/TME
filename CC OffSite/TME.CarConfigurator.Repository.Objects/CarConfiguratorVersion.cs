using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class CarConfiguratorVersion
    {
        [DataMember]
        public short ID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}