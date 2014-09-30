using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class Publication
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public DateTime? LineOfFrom { get; set; }
        [DataMember]
        public DateTime? LineOfTo { get; set; }

        [DataMember]
        public Generation Generation { get; set; }

        [DataMember]
        public List<PublicationTimeFrame> TimeFrames { get; set; }

        [DataMember]
        public string PublishedBy { get; set; }
        [DataMember]
        public DateTime PublishedOn { get; set; }
    }
}