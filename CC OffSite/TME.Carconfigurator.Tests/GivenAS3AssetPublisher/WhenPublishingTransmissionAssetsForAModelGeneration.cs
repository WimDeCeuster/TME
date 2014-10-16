using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3AssetPublisher
{
    public class WhenPublishingTransmissionAssetsForAModelGeneration : TestBase
    {
        private IAssetPublisher _assetPublisher;
        private IContext _context;
        private IService _s3Service;
        private IAssetService _assetService;
        private readonly IEnumerable<string> _languages = new List<string> { "nl" };
        private Publication _publication;
        private Transmission[] _transmissions;
        private string _assets;
        private List<Asset> _assetsForTrans1;
        private List<Asset> _assetsForTrans2;


        private const string BRAND = "Toyota";
        private const string COUNTRY = "BE";

        protected override void Arrange()
        {
            var generationTransmission1 = new TransmissionBuilder().WithId(Guid.NewGuid()).Build();
            var generationTransmission2 = new TransmissionBuilder().WithId(Guid.NewGuid()).Build();

            _transmissions = new Transmission[] {generationTransmission1, generationTransmission2};

            _assetsForTrans1 = new List<Asset>
            {
                new AssetBuilder().WithId(Guid.NewGuid()).Build(),
                new AssetBuilder().WithId(Guid.NewGuid()).Build()
            };

            _assetsForTrans2 = new List<Asset>
            {
                new AssetBuilder().WithId(Guid.NewGuid()).Build(),
                new AssetBuilder().WithId(Guid.NewGuid()).Build()
            };

            _publication = PublicationBuilder.Initialize().WithID(Guid.NewGuid()).Build();

            _context =
                ContextBuilder.InitialiseFakeContext()
                .WithBrand(BRAND)
                .WithCountry(COUNTRY)
                .WithLanguages(_languages.ToArray())
                .WithPublication(_languages.First(), _publication)
                .WithTransmissions(_languages.First(),_transmissions)
                .WithAssets(_languages.First(), _assetsForTrans1,generationTransmission1.ID)
                .WithAssets(_languages.First(), _assetsForTrans2,generationTransmission2.ID)
                .Build();

            _s3Service = A.Fake<IService>();
            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _assetService = new AssetsService(_s3Service,serialiser,keyManager);
            _assetPublisher = new AssetPublisher(_assetService);
        }

        protected override void Act()
        {
            var result = _assetPublisher.PublishAssets(_context);
        }

        /*[Fact]
        public void ThenAssetsForATransmissionShouldBePutForEachLanguageAndTimeFrames()
        {
            foreach (var language in _languages)
            {
                A.CallTo(() => _s3Service.PutObjectAsync(BRAND,COUNTRY,_key,_value));    
            }
        }*/
    }
}