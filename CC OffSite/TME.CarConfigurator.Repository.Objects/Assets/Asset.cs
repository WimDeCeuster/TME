using System;
namespace TME.CarConfigurator.Repository.Objects.Assets
{
    public class Asset
    {
        public Guid ID { get; set; }
        public int ShortID { get; set; }
        public string Name { get; set; }
        public string FileName{ get; set; }
        public FileType FileType { get; set; }
        public bool IsTransparent { get; set; }
        public bool RequiresMatte { get; set; }
        public int StackingOrder { get; set; }
        public short Width { get; set; }
        public short Height { get; set; }
        public short PositionX { get; set; }
        public short PositionY { get; set; }
        public bool AlwaysInclude { get; set; }
        public AssetType AssetType { get; set; }
        public string Hash { get; set; }
    }
}
