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

namespace TME.CarConfigurator.Query.Tests.GivenATransmission
{
    public class WhenAccessingItsAssetsForTheFirstTime : TestBase
    {
        private ITransmission _transmission;
        private IEnumerable<IAsset> _assets;
        private IAssetService _assetService;
        private Repository.Objects.Assets.Asset _asset1;
        private Repository.Objects.Assets.Asset _asset2;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repoTransmission = new TransmissionBuilder().WithId(Guid.NewGuid()).Build();

            var publicationTimeFrame =
                new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var transmissionService = A.Fake<ITransmissionService>();
            A.CallTo(() => transmissionService
                .GetTransmissions(A<Guid>._, A<Guid>._, A<Context>._))
                .Returns(new List<Repository.Objects.Transmission> {repoTransmission});

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService
                .GetAssets(publication.ID, repoTransmission.ID, context))
                .Returns(new List<Repository.Objects.Assets.Asset>{_asset1,_asset2});

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var transmissionFactory = new TransmissionFactoryBuilder()
                .WithAssetFactory(assetFactory)
                .WithTransmissionService(transmissionService)
                .Build();

            _transmission = transmissionFactory.GetTransmissions(publication, context).Single();
        }

        protected override void Act()
        {
            _assets = _transmission.Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheService()
        {
            A.CallTo(() => _assetService.GetAssets(A<Guid>._,A<Guid>._,A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
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