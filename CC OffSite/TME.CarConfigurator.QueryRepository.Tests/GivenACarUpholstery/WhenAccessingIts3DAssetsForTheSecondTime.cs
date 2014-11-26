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
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarUpholstery
{
    public class WhenAccessingIts3DAssetsForTheSecondTime : TestBase
    {
        private IEnumerable<IAsset> _secondAssets;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private IAssetService _assetService;
        private ICarUpholstery _upholstery;
        private IReadOnlyList<IAsset> _firstAssets;
        private const String MODE = "the mode";
        private const String VIEW = "the view";

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarUpholstery = new CarUpholsteryBuilder()
                .WithId(Guid.NewGuid())
                .AddVisibleIn(MODE, VIEW)
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
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carId, repoCarUpholstery.ID, context, VIEW, MODE)).Returns(new List<Repository.Objects.Assets.Asset> { _asset1, _asset2 });

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

            _firstAssets = _upholstery.VisibleIn.Single(vi => vi.Mode == MODE && vi.View == VIEW).Assets;
        }

        protected override void Act()
        {
            _secondAssets = _upholstery.VisibleIn.Single(vi => vi.Mode == MODE && vi.View == VIEW).Assets;
        }

        [Fact]
        public void ThenItShouldNotFetchTheAssetsFromTheServiceAgain()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, VIEW, MODE)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectAssets()
        {
            _secondAssets.Should().BeSameAs(_firstAssets);
            _secondAssets.Should().HaveCount(2);

            _secondAssets.Should().Contain(a => a.ID == _asset1.ID);
            _secondAssets.Should().Contain(a => a.ID == _asset2.ID);
        }
    }
}