using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarBodyType
{
    public class WhenAccessingItsAssetsASecondTime : TestBase
    {
        private IBodyType _bodyType;
        private IEnumerable<IAsset> _firstAssets;
        private IEnumerable<IAsset> _secondAssets;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private IAssetService _assetService;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoBodyType = new BodyTypeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var carId = Guid.NewGuid();

            var context = new ContextBuilder().Build();

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carId, repoBodyType.ID, context)).Returns(new List<Repository.Objects.Assets.Asset> {_asset1, _asset2});

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var bodyTypeFactory = new BodyTypeFactoryBuilder()
                .WithAssetFactory(assetFactory)
                .Build();

            _bodyType = bodyTypeFactory.GetCarBodyType(repoBodyType, carId, publication, context);

            _firstAssets = _bodyType.Assets;
        }

        protected override void Act()
        {
            _secondAssets = _bodyType.Assets;
        }

        [Fact]
        public void ThenItShouldNotFetchTheAssetsFromTheServiceAgain()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenTheReferenceToTheListOfAssetsShouldBeTheSameAsTheFirstList()
        {
            _secondAssets.Should().BeSameAs(_firstAssets);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectAssets()
        {
            _secondAssets.Should().HaveCount(2);

            _secondAssets.Should().Contain(a => a.ID == _asset1.ID);
            _secondAssets.Should().Contain(a => a.ID == _asset2.ID);
        }
    }
}