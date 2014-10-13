using System;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class Asset : IAsset
    {
        private readonly Repository.Objects.Assets.Asset _asset;
        private FileType _fileType;
        private AssetType _assetType;

        public Asset(Repository.Objects.Assets.Asset asset)
        {
            if (asset == null) throw new ArgumentNullException("asset");
            _asset = asset;
        }

        public Guid ID { get { return _asset.ID; } }
        public int ShortID { get { return _asset.ShortID; } }
        public string Name { get { return _asset.Name; } }
        public string FilePath { get { return _asset.FileName; } }
        public IFileType FileType { get { return _fileType = _fileType ?? new FileType(_asset.FileType); } }
        public bool IsTransparent { get { return _asset.IsTransparent; } }
        public bool RequiresMatte { get { return _asset.RequiresMatte; } }
        public int StackingOrder { get { return _asset.StackingOrder; } }
        public short Width { get { return _asset.Width; } }
        public short Height { get { return _asset.Height; } }
        public short PositionX { get { return _asset.PositionX; } }
        public short PositionY { get { return _asset.PositionY; } }
        public bool AlwaysInclude { get { return _asset.AlwaysInclude; } }
        public IAssetType AssetType { get { return _assetType = _assetType ?? new AssetType(_asset.AssetType); } }
        public string Hash { get { return _asset.Hash; } }
    }
}