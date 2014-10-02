using System;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class PublicationTimeFrame
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public DateTime LineOffFrom { get; set; }
        [DataMember]
        public DateTime LineOffTo { get; set; }
    }
}
