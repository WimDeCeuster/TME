using System;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects.Core
{
    [DataContract]
    public class Price
    {
        [DataMember]
        public decimal ExcludingVat { get; set; }
        [DataMember]
        public decimal IncludingVat { get; set; }
    }
}
