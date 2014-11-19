using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarEquipmentItem
{
    public class WhenAccessingIts3DAssetsForTheFirstTime : TestBase
    {
        private IEnumerable<IAsset> _assets;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private ICarEquipment _carEquipment;
        private string _mode;
        private string _view;

        protected override void Arrange()
        {
            _mode = "the mode";
            _view = "the view";

            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarOption = new CarOptionBuilder()
                .WithId(Guid.NewGuid())
                .WithVisibleIn(_mode, _view)
                .Build();

            var repoCarEquipmentItem = new CarEquipmentBuilder()
                .WithOptions(repoCarOption)
                .Build();

            var carID = Guid.NewGuid();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var equipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => equipmentService.GetCarEquipment(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new CarEquipment { Options = repoCarEquipmentItem.Options, Accessories = repoCarEquipmentItem.Accessories });

            _assetService = A.Fake<IAssetService>();
            A.CallTo(() => _assetService.GetCarEquipmentAssets(publication.ID, carID, context, _view, _mode))
                .Returns(new Dictionary<Guid, List<Asset>> { { repoCarOption.ID, new List<Asset> { _asset1, _asset2 } } });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var equipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(equipmentService)
                .WithAssetFactory(assetFactory)
                .Build();

            _carEquipment = equipmentFactory.GetCarEquipment(carID, publication, context);
        }

        protected override void Act()
        {
            _assets = _carEquipment.Options.First().VisibleIn.Single(v => v.Mode == _mode && v.View == _view).Assets;
        }

        [Fact]
        public void ThenItShouldNotFetchTheAssetsFromTheServiceAgain()
        {
            A.CallTo(() => _assetService.GetCarEquipmentAssets(A<Guid>._, A<Guid>._, A<Context>._, _view, _mode)).MustHaveHappened(Repeated.Exactly.Once);
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