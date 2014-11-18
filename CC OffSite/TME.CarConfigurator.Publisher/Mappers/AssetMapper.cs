using System;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Administration.Assets;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Exceptions;
using Asset = TME.CarConfigurator.Repository.Objects.Assets.Asset;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class AssetMapper : IAssetMapper
    {
        readonly IAssetTypeMapper _assetTypeMapper;
        readonly IFileTypeMapper _fileTypeMapper;

        public AssetMapper(IAssetTypeMapper assetTypeMapper, IFileTypeMapper fileTypeMapper)
        {
            if (assetTypeMapper == null) throw new ArgumentNullException("assetTypeMapper");
            if (fileTypeMapper == null) throw new ArgumentNullException("fileTypeMapper");

            _assetTypeMapper = assetTypeMapper;
            _fileTypeMapper = fileTypeMapper;
        }

        public Asset MapLinkedAsset(LinkedAsset linkedAsset)
        {
            var assetInfo = DetailedAssetInfo.GetDetailedAssetInfo(linkedAsset.ID);

            if (linkedAsset.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a shortID for linkedAsset {0}", linkedAsset.Name));

            return new Asset
            {
                AlwaysInclude = false,
                AssetType = _assetTypeMapper.MapGenerationAssetType(linkedAsset.AssetType),
                FileName = linkedAsset.FileName,
                FileType = _fileTypeMapper.MapFileType(linkedAsset.FileType),
                Hash = DetailedAssetInfo.GetDetailedAssetInfo(linkedAsset.ID).Hash,
                Height = assetInfo.Height,
                ID = linkedAsset.ID,
                IsTransparent = linkedAsset.IsTransparent,
                Name = linkedAsset.Name,
                PositionX = assetInfo.PositionX,
                PositionY = assetInfo.PositionY,
                RequiresMatte = linkedAsset.RequiresMatte,
                ShortID = linkedAsset.ShortID.Value,
                StackingOrder = linkedAsset.StackingOrder,
                Width = assetInfo.Width
            };
        }

        public Asset MapAssetSetAsset(AssetSetAsset assetSetAsset, ModelGeneration modelGeneration)
        {
            return MapGenerationAssetSetAsset(assetSetAsset,modelGeneration);
        }

        public Asset MapCarAssetSetAsset(AssetSetAsset assetSetAsset, ModelGeneration modelGeneration)
        {
            var asset = MapGenerationAssetSetAsset(assetSetAsset,modelGeneration);
            //Custom mapping for car assets.
            asset.AssetType = _assetTypeMapper.MapCarAssetType(assetSetAsset,modelGeneration);

            return asset;
        }

        private Asset MapGenerationAssetSetAsset(AssetSetAsset assetSetAsset, ModelGeneration modelGeneration)
        {
            if (assetSetAsset.Asset.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a shortID for assetSetAsset {0}", assetSetAsset.Name));

            return new Asset()
            {
                AlwaysInclude = assetSetAsset.AlwaysInclude,
                AssetType = _assetTypeMapper.MapObjectAssetType(assetSetAsset, modelGeneration),
                FileName = assetSetAsset.Asset.FileName,
                FileType = _fileTypeMapper.MapFileType(assetSetAsset.Asset.FileType),
                Hash = assetSetAsset.Asset.Hash,
                Height = assetSetAsset.Asset.Height,
                ID = assetSetAsset.Asset.ID,
                IsTransparent = assetSetAsset.Asset.IsTransparent,
                Name = assetSetAsset.Asset.Name,
                PositionX = assetSetAsset.Asset.PositionX,
                PositionY = assetSetAsset.Asset.PositionY,
                RequiresMatte = assetSetAsset.Asset.RequiresMatte,
                ShortID = assetSetAsset.Asset.ShortID.Value,
                StackingOrder = assetSetAsset.Asset.StackingOrder,
                Width = assetSetAsset.Asset.Width
            };
        }
    }
}
