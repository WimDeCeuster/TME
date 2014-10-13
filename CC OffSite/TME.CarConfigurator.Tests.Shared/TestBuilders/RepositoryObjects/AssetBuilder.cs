using System;

namespace TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects
{
    public class AssetBuilder
    {
        private readonly Repository.Objects.Assets.Asset _asset;

        public AssetBuilder()
        {
            _asset = new Repository.Objects.Assets.Asset();
        }

        public AssetBuilder WithId(Guid id)
        {
            _asset.ID = id;

            return this;
        }

        public Repository.Objects.Assets.Asset Build()
        {
            return _asset;
        }
    }
}