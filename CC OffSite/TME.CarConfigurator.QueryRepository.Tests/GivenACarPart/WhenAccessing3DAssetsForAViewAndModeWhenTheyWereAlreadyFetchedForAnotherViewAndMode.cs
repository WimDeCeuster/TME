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
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarPart
{
    public class WhenAccessing3DAssetsForAViewAndModeWhenTheyWereAlreadyFetchedForAnotherViewAndMode : TestBase
    {
        private string _view1;
        private string _mode1;
        private string _view2;
        private string _mode2;
        private Asset _asset1;
        private Asset _asset2;
        private Asset _asset3;
        private IAssetService _assetService;
        private IEnumerable<IAsset> _fetchedAssets1;
        private IEnumerable<IAsset> _fetchedAssets2;
        private ICarPart _carPart;

        protected override void Arrange()
        {
            _view1 = "the first view";
            _mode1 = "the first mode";
            _view2 = "the second view";
            _mode2 = "the second mode";

            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset3 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarPart = new CarPartBuilder()
                .WithId(Guid.NewGuid())
                .AddVisibleIn(_mode1, _view1)
                .AddVisibleIn(_mode2, _view2)
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

            A.CallTo(() => _assetService.GetCarPartsAssets(publication.ID, carId, context, _view1, _mode1))
                .Returns(new Dictionary<Guid, List<Asset>> { { repoCarPart.ID, new List<Asset> { _asset1, _asset2 } } });

            A.CallTo(() => _assetService.GetCarPartsAssets(publication.ID, carId, context, _view2, _mode2))
                .Returns( new Dictionary<Guid, List<Asset>> { { repoCarPart.ID, new List<Asset> { _asset3 } } });


            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var carPartService = A.Fake<ICarPartService>();
            A.CallTo(() => carPartService.GetCarParts(publication.ID, carId, context)).Returns(new[] {repoCarPart});

            var carPartFactory = new CarPartFactoryBuilder()
                .WithAssetFactory(assetFactory)
                .WithCarPartService(carPartService)
                .Build();

            _carPart = carPartFactory.GetCarParts(carId, publication, context).Single();

            _fetchedAssets1 = _carPart.VisibleIn.Single(v => v.Mode == _mode1 && v.View == _view1).Assets;
        }

        protected override void Act()
        {
            _fetchedAssets2 = _carPart.VisibleIn.Single(v => v.Mode == _mode2 && v.View == _view2).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheFirstModeAndView()
        {
            A.CallTo(() => _assetService.GetCarPartsAssets(A<Guid>._, A<Guid>._, A<Context>._, _view1, _mode1)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheSecondModeAndView()
        {
            A.CallTo(() => _assetService.GetCarPartsAssets(A<Guid>._, A<Guid>._, A<Context>._, _view2, _mode2)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectListOfAssetsForTheFirstModeAndView()
        {
            _fetchedAssets1.Should().HaveCount(2);

            _fetchedAssets1.Should().Contain(a => a.ID == _asset1.ID);
            _fetchedAssets1.Should().Contain(a => a.ID == _asset2.ID);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectListOfAssetsForTheSecondModeAndView()
        {
            _fetchedAssets2.Should().HaveCount(1);

            _fetchedAssets2.Should().Contain(a => a.ID == _asset3.ID);
        }
    }
}