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

namespace TME.CarConfigurator.Query.Tests.GivenACarCarPart
{
    public class WhenAccessingIts3DAssetsForTheFirstTime : TestBase
    {
        private IEnumerable<IAsset> _assets;
        private string _view;
        private string _mode;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private IAssetService _assetService;
        private ICarPart _carPart;

        protected override void Arrange()
        {
            _view = "the view";
            _mode = "the mode";

            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarPart = new CarPartBuilder()
                .WithId(Guid.NewGuid())
                .AddVisibleIn(_mode, _view)
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
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carId, repoCarPart.ID, context, _view, _mode))
                .Returns(new List<Repository.Objects.Assets.Asset> { _asset1, _asset2 });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var carPartService = A.Fake<ICarPartService>();
            A.CallTo(() => carPartService.GetCarParts(publication.ID, carId, context)).Returns(new[] {repoCarPart});

            var carPartFactory = new CarPartFactoryBuilder()
                .WithAssetFactory(assetFactory)
                .WithCarPartService(carPartService)
                .Build();

            _carPart = carPartFactory.GetCarCarParts(carId, publication, context).Single();
        }

        protected override void Act()
        {
            _assets = _carPart.VisibleIn.Single(v => v.Mode == _mode && v.View == _view).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheService()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, A<string>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectAssets()
        {
            _assets.Should().HaveCount(2);

            _assets.Should().Contain(a => a.ID == _asset1.ID);
            _assets.Should().Contain(a => a.ID == _asset2.ID);
        }
    }
}