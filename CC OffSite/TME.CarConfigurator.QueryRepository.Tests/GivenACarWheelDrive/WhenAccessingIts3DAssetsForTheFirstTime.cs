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

namespace TME.CarConfigurator.Query.Tests.GivenACarWheelDrive
{
    public class WhenAccessingIts3DAssetsForTheFirstTime : TestBase
    {
        private IEnumerable<IAsset> _assets;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private IWheelDrive _wheelDrive;
        private string _mode;
        private string _view;

        protected override void Arrange()
        {
            _view = "the view";
            _mode = "the mode";

            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repoWheelDrive = new WheelDriveBuilder().AddVisibleIn(_mode,_view).WithId(Guid.NewGuid()).Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder().WithID(Guid.NewGuid()).WithDateRange(DateTime.MinValue, DateTime.MaxValue).Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).AddTimeFrame(publicationTimeFrame).Build();

            var carID = Guid.NewGuid();

            var context = new ContextBuilder().Build();

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carID, repoWheelDrive.ID, context,_view,_mode))
                .Returns(new[] { _asset1, _asset2 });

            var assetFactory = new AssetFactoryBuilder().WithAssetService(_assetService).Build();

            var wheelDriveFactory = new WheelDriveFactoryBuilder().WithAssetFactory(assetFactory).Build();

            _wheelDrive = wheelDriveFactory.GetCarWheelDrive(repoWheelDrive, carID, publication, context);
        }

        protected override void Act()
        {
            _assets = _wheelDrive.VisibleIn.Single(visibleIn => visibleIn.Mode == _mode && visibleIn.View == _view).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheService()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._,A<String>._,A<String>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectAssets()
        {
            _assets.Should().HaveCount(2);
            _assets.Should().Contain(asset => asset.ID == _asset1.ID);
            _assets.Should().Contain(asset => asset.ID == _asset2.ID);
        }
    }
}