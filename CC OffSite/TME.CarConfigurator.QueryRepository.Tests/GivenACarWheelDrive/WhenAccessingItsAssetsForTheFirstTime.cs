using System;
using System.Collections.Generic;
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
    public class WhenAccessingItsAssetsForTheFirstTime : TestBase
    {
        private IEnumerable<IAsset> _assets;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private IWheelDrive _wheelDrive;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repoWheelDrive = new WheelDriveBuilder().WithId(Guid.NewGuid()).Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder().WithID(Guid.NewGuid()).WithDateRange(DateTime.MinValue, DateTime.MaxValue).Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).AddTimeFrame(publicationTimeFrame).Build();

            var carID = Guid.NewGuid();

            var context = new ContextBuilder().Build();

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carID, repoWheelDrive.ID, context))
                .Returns(new[] { _asset1, _asset2 });

            var assetFactory = new AssetFactoryBuilder().WithAssetService(_assetService).Build();

            var wheelDriveFactory = new WheelDriveFactoryBuilder().WithAssetFactory(assetFactory).Build();

            _wheelDrive = wheelDriveFactory.GetCarWheelDrive(repoWheelDrive, carID, publication, context);
        }

        protected override void Act()
        {
            _assets = _wheelDrive.Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheService()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._))
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