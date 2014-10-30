using System;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Assets
{
    public class Asset : IAsset
    {
        private readonly Repository.Objects.Assets.Asset _repositoryAsset;
        private FileType _fileType;
        private AssetType _assetType;

        public Asset(Repository.Objects.Assets.Asset repositoryAsset)
        {
            if (repositoryAsset == null) throw new ArgumentNullException("repositoryAsset");
            _repositoryAsset = repositoryAsset;
        }

        public Guid ID { get { return _repositoryAsset.ID; } }
        public int ShortID { get { return _repositoryAsset.ShortID; } }
        public string Name { get { return _repositoryAsset.Name; } }
        public string FilePath { get { return _repositoryAsset.FileName; } }
        public IFileType FileType { get { return _fileType = _fileType ?? new FileType(_repositoryAsset.FileType); } }
        public bool IsTransparent { get { return _repositoryAsset.IsTransparent; } }
        public bool RequiresMatte { get { return _repositoryAsset.RequiresMatte; } }
        public int StackingOrder { get { return _repositoryAsset.StackingOrder; } }
        public short Width { get { return _repositoryAsset.Width; } }
        public short Height { get { return _repositoryAsset.Height; } }
        public short PositionX { get { return _repositoryAsset.PositionX; } }
        public short PositionY { get { return _repositoryAsset.PositionY; } }
        public bool AlwaysInclude { get { return _repositoryAsset.AlwaysInclude; } }
        public IAssetType AssetType { get { return _assetType = _assetType ?? new AssetType(_repositoryAsset.AssetType); } }
        public string Hash { get { return _repositoryAsset.Hash; } }
    }
}