using System;
using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects
{
    [DataContract]
    public class Car : BaseObject
    {
        public Guid ID { get; set; }

        [DataMember]
        public int ShortID { get; set; }
        [DataMember]
        public bool Promoted { get; set; }

        [DataMember]
        public bool WebVisible { get; set; }
        [DataMember]
        public bool ConfigVisible { get; set; }
        [DataMember]
        public bool FinanceVisible { get; set; }

        [DataMember]
        public Price BasePrice { get; set; }
        [DataMember]
        public Price StartingPrice { get; set; }

        [DataMember]
        public BodyType BodyType { get; set; }
        [DataMember]
        public Engine Engine { get; set; }
  }
}
