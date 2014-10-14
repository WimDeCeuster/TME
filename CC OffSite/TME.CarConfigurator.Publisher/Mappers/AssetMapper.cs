using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.Mappers.Exceptions;
using TME.CarConfigurator.Repository.Objects.Assets;

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

        public Asset MapLinkedAsset(Administration.Assets.LinkedAsset linkedAsset)
        {
            var assetInfo = Administration.Assets.DetailedAssetInfo.GetDetailedAssetInfo(linkedAsset.ID);

            if (linkedAsset.ShortID == null)
                throw new CorruptDataException(String.Format("Please provide a shortID for linkedAsset {0}", linkedAsset.ID));

            return new Asset
            {
                AlwaysInclude = false,
                AssetType = _assetTypeMapper.MapGenerationAssetType(linkedAsset.AssetType),
                FileName = linkedAsset.FileName,
                FileType = _fileTypeMapper.MapFileType(linkedAsset.FileType),
                Hash = assetInfo.Hash,
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
    }
}
