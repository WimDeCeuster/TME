using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects.Assets
{
    [DataContract]
    public class FileType
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Type { get; set; }
    }
}