using System;
using System.Collections.Generic;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Factories;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnAssetFactory
{
    public class WhenRequesting3DAssetsForCarPartsOfDifferentCars : TestBase
    {
        private IAssetFactory _assetFactory;

        private Publication _publication;
        private Context _context;
        private readonly Guid _carId1 = Guid.NewGuid();
        private readonly Guid _carId2 = Guid.NewGuid();
        private readonly Guid _equipmentId = Guid.NewGuid();

        private List<Asset> _expectedAssets1;
        private List<Asset> _expectedAssets2;
        private IReadOnlyList<IAsset> _assets1;
        private IReadOnlyList<IAsset> _assets2;
        private const string VIEW = "the view";
        private const string MODE = "The mode";

        protected override void Arrange()
        {
            _publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();
            _context = new ContextBuilder().WithLanguage("de").Build();

            _expectedAssets1 = new List<Asset>
            {
                new AssetBuilder().WithId(Guid.NewGuid()).Build(),
                new AssetBuilder().WithId(Guid.NewGuid()).Build()
            };

            _expectedAssets2 = new List<Asset>
            {
                new AssetBuilder().WithId(Guid.NewGuid()).Build()
            };

            _expectedAssets1.Should().NotBeSameAs(_expectedAssets2);

            var assetService = A.Fake<IAssetService>();

            A.CallTo(() => assetService.GetCarPartsAssets(_publication.ID, _carId1, _context, VIEW, MODE))
                .Returns(new Dictionary<Guid, List<Asset>> { { _equipmentId, _expectedAssets1 } });

            A.CallTo(() => assetService.GetCarPartsAssets(_publication.ID, _carId2, _context, VIEW, MODE))
                .Returns(new Dictionary<Guid, List<Asset>> { { _equipmentId, _expectedAssets2 } });

            _assetFactory = new AssetFactory(assetService);
        }

        protected override void Act()
        {
            _assets1 = _assetFactory.GetCarPartAssets(_publication, _carId1, _equipmentId, _context, VIEW, MODE);
            _assets2 = _assetFactory.GetCarPartAssets(_publication, _carId2, _equipmentId, _context, VIEW, MODE);
        }

        [Fact]
        public void ThenItShouldReceiveTheCorrectListOfAssetsForCar1()
        {
            _assets1.Should().HaveCount(2);

            _assets1.Should().Contain(a => a.ID == _expectedAssets1[0].ID);
            _assets1.Should().Contain(a => a.ID == _expectedAssets1[1].ID);
        }

        [Fact]
        public void ThenItShouldReceiveTheCorrectListOfAssetsForCar2()
        {
            _assets2.Should().HaveCount(1);

            _assets2.Should().Contain(a => a.ID == _expectedAssets2[0].ID);
        }
    }
}