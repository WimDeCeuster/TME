using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class Link
    {
        [DataMember]
        public short ID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public string Url { get; set; }
    }
}
