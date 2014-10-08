using System;
using System.Runtime.Serialization;
namespace TME.CarConfigurator.Repository.Objects.Assets
{
    [DataContract]
    public class Asset
    {
        public Guid ID { get; set; }
        public int ShortID { get; set; }
        public string Name { get; set; }
        public string FilePath{ get; set; }
        public FileType FileType { get; set; }
        public bool IsTransparent { get; set; }
        public bool RequiresMatte { get; set; }
        public int StackingOrder { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int PositionX { get; set; }
        public int PositionY { get; set; }
        public bool AlwaysInclude { get; set; }
        public AssetType AssetType { get; set; }
        public string Hash { get; set; }

    }
}
