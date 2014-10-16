using System;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders
{
    public class AssetBuilder
    {
        private readonly Asset _asset;

        public AssetBuilder()
        {
            _asset = new Asset();
        }

        public AssetBuilder WithId(Guid id)
        {
            _asset.ID = id;

            return this;
        }

        public AssetBuilder WithAssetType(AssetType assetType)
        {
            _asset.AssetType = assetType;
            return this;
        }

        public Asset Build()
        {
            return _asset;
        }
    }
}