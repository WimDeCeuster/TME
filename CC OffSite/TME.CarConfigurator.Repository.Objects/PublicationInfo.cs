using System;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class PublicationInfo
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public DateTime? LineOffFrom { get; set; }
        [DataMember]
        public DateTime? LineOffTo { get; set; }

        [DataMember]
        public Guid GenerationID { get; set; }

        [DataMember]
        public PublicationState State { get; set; }
    }
}