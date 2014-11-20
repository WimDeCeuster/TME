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
    public class WhenAccessingItsAssetsForTheSecondTime : TestBase
    {
        private IEnumerable<IAsset> _secondAssets;
        private Asset _asset1;
        private Asset _asset2;
        private IAssetService _assetService;
        private ICarEquipment _carEquipment;
        private IReadOnlyList<IAsset> _firstAssets;

        protected override void Arrange()
        {
            _asset1 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _asset2 = new AssetBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarOption = new CarOptionBuilder()
                .WithId(Guid.NewGuid())
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
            A.CallTo(() => _assetService.GetCarEquipmentAssets(publication.ID, carID, context))
                .Returns(new Dictionary<Guid, List<Asset>> { { repoCarOption.ID, new List<Asset> { _asset1, _asset2 } } });

            var assetFactory = new AssetFactoryBuilder()
                .WithAssetService(_assetService)
                .Build();

            var equipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(equipmentService)
                .WithAssetFactory(assetFactory)
                .Build();

            _carEquipment = equipmentFactory.GetCarEquipment(carID, publication, context);

            _firstAssets = _carEquipment.Options.First().Assets;
        }

        protected override void Act()
        {
            _secondAssets = _carEquipment.Options.First().Assets;
        }

        [Fact]
        public void ThenItShouldNotFetchTheAssetsFromTheServiceAgain()
        {
            A.CallTo(() => _assetService.GetCarEquipmentAssets(A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
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