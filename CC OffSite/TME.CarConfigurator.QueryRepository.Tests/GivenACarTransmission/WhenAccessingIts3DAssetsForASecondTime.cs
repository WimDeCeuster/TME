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

namespace TME.CarConfigurator.Query.Tests.GivenACarTransmission
{
    public class WhenAccessingIts3DAssetsForASecondTime : TestBase
    {
        private ITransmission _transmission;
        private string _view;
        private string _mode;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private IEnumerable<IAsset> _secondAssets;
        private IEnumerable<IAsset> _firstAssets;

        protected override void Arrange()
        {
            _view = "the view";
            _mode = "the mode";

            _asset1 = new AssetBuilder().WithId(Guid.NewGuid()).Build();
            _asset2 = new AssetBuilder().WithId(Guid.NewGuid()).Build();

            var repoTransmission = new TransmissionBuilder()
                .WithId(Guid.NewGuid())
                .AddVisibleIn(_view, _mode)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder().WithID(Guid.NewGuid()).WithDateRange(DateTime.MinValue, DateTime.MaxValue).Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).AddTimeFrame(publicationTimeFrame).Build();

            var carID = Guid.NewGuid();

            var context = new ContextBuilder().Build();

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetCarAssets(publication.ID, carID, repoTransmission.ID, context, _view, _mode))
                .Returns(new[] { _asset1, _asset2 });

            var assetFactory = new AssetFactoryBuilder().WithAssetService(_assetService).Build();

            var transmissionFactory = new TransmissionFactoryBuilder().WithAssetFactory(assetFactory).Build();

            _transmission = transmissionFactory.GetCarTransmission(repoTransmission, carID, publication, context);

            _firstAssets = _transmission.VisibleIn.Single(v => v.Mode == _mode && v.View == _view).Assets;
        }

        protected override void Act()
        {
            _secondAssets = _transmission.VisibleIn.Single(v => v.Mode == _mode && v.View == _view).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheService()
        {
            A.CallTo(() => _assetService.GetCarAssets(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._, A<String>._, A<String>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldContainTheCorrectAssets()
        {
            _secondAssets.Should().BeSameAs(_firstAssets);
            _secondAssets.Should().HaveCount(2);

            _secondAssets.Should().Contain(asset => asset.ID == _asset1.ID);
            _secondAssets.Should().Contain(asset => asset.ID == _asset2.ID);
        }
    }
}