using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects.Core
{
    [DataContract]
    public class Label
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Value { get; set; }
    
    }
}
