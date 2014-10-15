using System;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects
{
    public class PublicationTimeFrame
    {
        public Guid ID { get; set; }
        public DateTime LineOffFrom { get; set; }
        public DateTime LineOffTo { get; set; }
    }
}
