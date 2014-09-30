using System;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class Asset : IAsset
    {
        public Guid ID { get; private set; }
        public int ShortID { get; private set; }
        public string Name { get; private set; }
        public string FilePath { get; private set; }
        public IFileType FileType { get; private set; }
        public bool IsTransparent { get; private set; }
        public bool RequiresMatte { get; private set; }
        public int StackingOrder { get; private set; }
        public short Width { get; private set; }
        public short Height { get; private set; }
        public short PositionX { get; private set; }
        public short PositionY { get; private set; }
        public bool AlwaysInclude { get; private set; }
        public IAssetType AssetType { get; private set; }
        public string Hash { get; private set; }
    }
}