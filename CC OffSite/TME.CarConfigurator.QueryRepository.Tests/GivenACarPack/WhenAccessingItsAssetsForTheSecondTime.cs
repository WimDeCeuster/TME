using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarPack
{
    public class WhenAccessingItsAssetsForTheSecondTime : TestBase
    {
        private ICarPack _pack;
        private IReadOnlyList<IAsset> _secondAssets;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private IReadOnlyList<IAsset> _firstAssets;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repoCarPack = new CarPackBuilder().WithId(Guid.NewGuid()).Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder().WithDateRange(DateTime.MinValue, DateTime.MaxValue).WithID(Guid.NewGuid()).Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).AddTimeFrame(publicationTimeFrame).Build();

            var carID = Guid.NewGuid();

            var context = new ContextBuilder().Build();

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carID, repoCarPack.ID, context))
                .Returns(new[] { _asset1, _asset2 });

            var packService = A.Fake<IPackService>();
            A.CallTo(() => packService.GetCarPacks(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new[] { repoCarPack });

            var assetFactory = new AssetFactoryBuilder().WithAssetService(_assetService).Build();

            var packFactory = new PackFactoryBuilder()
                                .WithPackService(packService)
                                .WithAssetFactory(assetFactory).Build();

            _pack = packFactory.GetCarPacks(publication, context, carID).Single();

            _firstAssets = _pack.Assets;
        }

        protected override void Act()
        {
            _secondAssets = _pack.Assets;
        }

        [Fact]
        public void ThenItShouldNotFetchTheAssetsFromTheServiceAgain()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectAssets()
        {
            _secondAssets.Should().BeSameAs(_firstAssets);
            _secondAssets.Should().HaveCount(2);
            _secondAssets.Should().Contain(asset => asset.ID == _asset1.ID);
            _secondAssets.Should().Contain(asset => asset.ID == _asset2.ID);
        }
    }
}