using System;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects.Assets
{
    [DataContract]
    public class Asset
    {
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public int ShortID { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string FilePath{ get; set; }
        [DataMember]
        public FileType FileType { get; set; }
        [DataMember]
        public bool IsTransparent { get; set; }
        [DataMember]
        public bool RequiresMatte { get; set; }
        [DataMember]
        public int StackingOrder { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int PositionX { get; set; }
        [DataMember]
        public int PositionY { get; set; }
        [DataMember]
        public bool AlwaysInclude { get; set; }
        [DataMember]
        public AssetType AssetType { get; set; }
        [DataMember]
        public string Hash { get; set; }

    }
}
