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
    public class WhenAccessingItsAssetsASecondTime : TestBase
    {
        private IEnumerable<IAsset> _firstAssets;
        private IEnumerable<IAsset> _secondAssets;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;
        private IAssetService _assetService;
        private IWheelDrive _wheelDrive;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoWheelDrive = new WheelDriveBuilder()
                .WithId(Guid.NewGuid())
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
            A.CallTo(() => _assetService.GetAssets(publication.ID, repoWheelDrive.ID, context)).Returns(new List<Repository.Objects.Assets.Asset> { _asset1, _asset2 });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var wheelDriveFactory = new WheelDriveFactoryBuilder()
                .WithWheelDriveService(wheelDriveService)
                .WithAssetFactory(assetFactory)
                .Build();

            _wheelDrive = wheelDriveFactory.GetWheelDrives(publication, context).Single();

            _firstAssets = _wheelDrive.Assets;
        }

        protected override void Act()
        {
            _secondAssets = _wheelDrive.Assets;
        }

        [Fact]
        public void ThenItShouldNotFetchTheAssetsFromTheServiceAgain()
        {
            A.CallTo(() => _assetService.GetAssets(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldReferToTheSameListOfAssetsAsTheFirstTime()
        {
            _secondAssets.Should().BeSameAs(_firstAssets);
        }
    }
}