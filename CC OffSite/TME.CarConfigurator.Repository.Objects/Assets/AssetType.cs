using System.Runtime.Serialization;
using TME.CarConfigurator.Repository.Objects.Assets.Enums;

namespace TME.CarConfigurator.Repository.Objects.Assets
{
    [DataContract]
    public class AssetType
    {

        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Mode { get; set; }
        [DataMember]
        public string View { get; set; }
        [DataMember]
        public string Side { get; set; }
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string ExteriorColourCode { get; set; }
        [DataMember]
        public string UpholsteryCode { get; set; }
        [DataMember]
        public string EquipmentCode { get; set; }

        [DataMember]
        public Scope Scope { get; set; }
   }
}