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
        public DateTime? LineOfFrom { get; set; }
        [DataMember]
        public DateTime? LineOfTo { get; set; }

        [DataMember]
        public Guid GenerationID { get; set; }

        [DataMember]
        public PublicationState State { get; set; }
    }
}