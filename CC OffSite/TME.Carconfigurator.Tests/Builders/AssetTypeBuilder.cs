using System;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.Carconfigurator.Tests.Builders
{
    public class AssetTypeBuilder
    {
        private readonly AssetType _assetType;

        public AssetTypeBuilder()
        {
            _assetType = new AssetType();
        }

        public AssetTypeBuilder WithView(String view)
        {
            _assetType.View = view;
            return this;
        }

        public AssetTypeBuilder WithMode(String mode)
        {
            _assetType.Mode = mode;
            return this;
        }

        public AssetType Build()
        {
            return _assetType;
        }
    }
}