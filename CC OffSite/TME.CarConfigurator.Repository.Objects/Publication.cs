using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    public class Publication
    {
        public Guid ID { get; set; }
        public DateTime? LineOffFrom { get; set; }
        public DateTime? LineOffTo { get; set; }
        public Generation Generation { get; set; }

        public List<PublicationTimeFrame> TimeFrames { get; set; }

        public String PublishedBy { get; set; }
        public DateTime PublishedOn { get; set; }
    }
}