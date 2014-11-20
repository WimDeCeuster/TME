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
    public class WhenAccessing3DAssetsForAModeAndViewWhenTheWereAlreadyFetchedForAnotherModeAndView : TestBase
    {
        private IEnumerable<IAsset> _secondAssets;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private ICarEquipment _carEquipment;
        private IReadOnlyList<IAsset> _firstAssets;
        private string _mode1;
        private string _mode2;
        private string _view1;
        private string _view2;
        private Asset _asset3;

        protected override void Arrange()
        {
            _mode1 = "the first mode";
            _mode1 = "the second mode";
            _view1 = "the first view";
            _view2 = "the second view";

            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();
            
            _asset3 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarOption = new CarOptionBuilder()
                .WithId(Guid.NewGuid())
                .WithVisibleIn(_mode1, _view1)
                .WithVisibleIn(_mode2, _view2)
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
            A.CallTo(() => _assetService.GetCarEquipmentAssets(publication.ID, carID, context, _view1, _mode1))
                .Returns(new Dictionary<Guid, List<Asset>> { { repoCarOption.ID, new List<Asset> { _asset1, _asset2 } } });
            A.CallTo(() => _assetService.GetCarEquipmentAssets(publication.ID, carID, context, _view2, _mode2))
                .Returns(new Dictionary<Guid, List<Asset>> { { repoCarOption.ID, new List<Asset> { _asset3 } } });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var equipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(equipmentService)
                .WithAssetFactory(assetFactory)
                .Build();

            _carEquipment = equipmentFactory.GetCarEquipment(carID, publication, context);

            _firstAssets = _carEquipment.Options.First().VisibleIn.Single(v => v.Mode == _mode1 && v.View == _view1).Assets;
        }

        protected override void Act()
        {
            _secondAssets = _carEquipment.Options.First().VisibleIn.Single(v => v.Mode == _mode2 && v.View == _view2).Assets;
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheFirstModeAndView()
        {
            A.CallTo(() => _assetService.GetCarEquipmentAssets(A<Guid>._, A<Guid>._, A<Context>._, _view1, _mode1)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldFetchTheAssetsFromTheServiceForTheSecondModeAndView()
        {
            A.CallTo(() => _assetService.GetCarEquipmentAssets(A<Guid>._, A<Guid>._, A<Context>._, _view2, _mode2)).MustHaveHappened(Repeated.Exactly.Once);
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