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

namespace TME.CarConfigurator.Query.Tests.GivenAWheelDrive
{
    public class WhenAccessing3DAssetsForAViewAndModeWhenTheyWereAlreadyFetchedForAnotherViewAndMode : TestBase
    {
        private IEnumerable<IAsset> _fetchedAssets1;
        private IEnumerable<IAsset> _fetchedAssets2;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private Repository.Objects.Assets.Asset _asset3;
        private IAssetService _assetService;
        private IWheelDrive _wheelDrive;
        private string _view1;
        private string _mode1;
        private string _view2;
        private string _mode2;

        protected override void Arrange()
        {
            _view1 = "the first view";
            _mode1 = "the first mode";
            _view2 = "the other view";
            _mode2 = "the other mode";

            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset3 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoWheelDrive = new WheelDriveBuilder()
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

            var context = new ContextBuilder().Build();

            var wheelDriveService = A.Fake<IWheelDriveService>();
            A.CallTo(() => wheelDriveService.GetWheelDrives(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.WheelDrive> { repoWheelDrive });

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetAssets(publication.ID, repoWheelDrive.ID, context, _view1, _mode1))
                .Returns(new List<Repository.Objects.Assets.Asset> { _asset1, _asset2 });
            A.CallTo(() => _assetService.GetAssets(publication.ID, repoWheelDrive.ID, context, _view2, _mode2))
                .Returns(new List<Repository.Objects.Assets.Asset> { _asset3 });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var wheelDriveFactory = new WheelDriveFactoryBuilder()
                .WithWheelDriveService(wheelDriveService)
                .WithAssetFactory(assetFactory)
                .Build();

            _wheelDrive = wheelDriveFactory.GetWheelDrives(publication, context).Single();

            _fetchedAssets1 = _wheelDrive.VisibleIn.Single(x=> x.Mode == _mode1 && x.View == _view1).Assets;
        }

        protected override void Act()
        {
            _fetchedAssets2 = _wheelDrive.VisibleIn.Single(x => x.Mode == _mode2 && x.View == _view2).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheFirstModeAndView()
        {
            A.CallTo(() => _assetService.GetAssets(A<Guid>._, A<Guid>._, A<Context>._, _view1, _mode1)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheSecondModeAndView()
        {
            A.CallTo(() => _assetService.GetAssets(A<Guid>._, A<Guid>._, A<Context>._, _view2, _mode2)).MustHaveHappened(Repeated.Exactly.Once);
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