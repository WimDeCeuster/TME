using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarUpholstery
{
    public class WhenAccessing3DAssetsForAModeAndViewWhenTheyWereAlreadyFetchedForAnotherModeAndView : TestBase 
    {
        private IEnumerable<IAsset> _secondAssets;
        private Asset _asset1;
        private Asset _asset2;
        private Asset _asset3;
        private IAssetService _assetService;
        private ICarUpholstery _upholstery;
        private IReadOnlyList<IAsset> _firstAssets;
        private const String MODE1 = "the first mode";
        private const String MODE2 = "the second mode";
        private const String VIEW1 = "the first view";
        private const String VIEW2 = "the second view";

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset3 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarUpholstery = new CarUpholsteryBuilder()
                .WithId(Guid.NewGuid())
                .AddVisibleIn(MODE1, VIEW1)
                .AddVisibleIn(MODE2, VIEW2)
                .Build();

            var repoColourCombination =
                new CarColourCombinationBuilder().WithUpholstery(repoCarUpholstery).Build();

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
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carId, repoCarUpholstery.ID, context, VIEW1, MODE1)).Returns(new List<Asset> { _asset1, _asset2 });
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carId, repoCarUpholstery.ID, context, VIEW2, MODE2)).Returns(new List<Asset> { _asset3 });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var colourService = A.Fake<IColourService>();
            A.CallTo(() => colourService.GetCarColourCombinations(publication.ID, context, carId))
                .Returns(new List<Repository.Objects.Colours.CarColourCombination> { repoColourCombination });

            var colourFactory = new ColourFactoryBuilder()
                .WithColourService(colourService)
                .WithAssetFactory(assetFactory)
                .Build();

            _upholstery = colourFactory.GetCarColourCombinations(publication, context, carId).First().Upholstery;

            _firstAssets = _upholstery.VisibleIn.Single(vi => vi.Mode == MODE1 && vi.View == VIEW1).Assets;
        }

        protected override void Act()
        {
            _secondAssets = _upholstery.VisibleIn.Single(vi => vi.Mode == MODE2 && vi.View == VIEW2).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheFirstModeAndView()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, VIEW1, MODE1)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheSecondModeAndView()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, VIEW2, MODE2)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectListOfAssetsForTheFirstModeAndView()
        {
            _firstAssets.Should().HaveCount(2);

            _firstAssets.Should().Contain(a => a.ID == _asset1.ID);
            _firstAssets.Should().Contain(a => a.ID == _asset2.ID);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectListOfAssetsForTheSecondModeAndView()
        {
            _secondAssets.Should().HaveCount(1);

            _secondAssets.Should().Contain(a => a.ID == _asset3.ID);
        }

    }
}