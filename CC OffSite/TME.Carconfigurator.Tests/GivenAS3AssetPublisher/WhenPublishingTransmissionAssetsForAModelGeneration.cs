using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Publisher.Interfaces;
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

        private Publication _publication;

        private Transmission[] _transmissions;

        private List<Asset> _assetsForTransmission1;
        private List<Asset> _assetsForTransmission2;

        private const string VALUE = "serialised data";
        private const string ASSETS_KEY = "KeyForAssets";
        private const string LANGUAGE1 = "nl";
        private const string LANGUAGE2 = "en";
        private const string ASSETS_DEFAULT_KEY = "KeyForDefaultAssets";
        private const string MODE = "Mode";
        private const string VIEW = "View";


        private const string BRAND = "Toyota";
        private const string COUNTRY = "BE";

        protected override void Arrange()
        {
            var generationTransmission1 = new TransmissionBuilder().WithId(Guid.NewGuid()).Build();
            var generationTransmission2 = new TransmissionBuilder().WithId(Guid.NewGuid()).Build();

            _transmissions = new[] {generationTransmission1, generationTransmission2};

            _assetsForTransmission1 = new List<Asset>
            {
                new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(new AssetTypeBuilder().Build()).Build(),
                new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(new AssetTypeBuilder().WithMode(MODE).WithView(VIEW).Build()).Build()
            };

            _assetsForTransmission2 = new List<Asset>
            {
                new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(new AssetTypeBuilder().WithMode(null).WithView(null).Build()).Build(),
                new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(new AssetTypeBuilder().WithMode(MODE).WithView(VIEW).Build()).Build()
            };

            _publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();

            _context =
                new ContextBuilder()
                .WithBrand(BRAND)
                .WithCountry(COUNTRY)
                .WithLanguages(LANGUAGE1,LANGUAGE2)
                .WithPublication(LANGUAGE1, _publication)
                .WithPublication(LANGUAGE2, _publication)
                .WithAssets(LANGUAGE1, _assetsForTransmission1,generationTransmission1.ID)
                .WithAssets(LANGUAGE2, _assetsForTransmission1,generationTransmission1.ID)
                .WithAssets(LANGUAGE1, _assetsForTransmission2,generationTransmission2.ID)
                .WithAssets(LANGUAGE2, _assetsForTransmission2,generationTransmission2.ID)
                .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<Object>._)).Returns(VALUE);
            var keyManager = A.Fake<IKeyManager>();
            A.CallTo(() => keyManager.GetAssetsKey(A<Guid>._, A<Guid>._, A<String>._, A<String>._)).Returns(ASSETS_KEY);
            A.CallTo(() => keyManager.GetDefaultAssetsKey(A<Guid>._, A<Guid>._)).Returns(ASSETS_DEFAULT_KEY);

            _assetService = new AssetsService(_s3Service,serialiser,keyManager);
            _assetPublisher = new AssetPublisher(_assetService);
        }

        protected override void Act()
        {
             _assetPublisher.PublishAsync(_context).Wait();
        }

        [Fact]
        public void ThenAssetsForATransmissionShouldBePutForEachLanguageAndTimeFrames()
        {
            // 2 transmissions * 2 Assetputs (with modeview/without modeview) * 2 languages
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._,A<String>._,A<String>._,A<String>._)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Times(8)); 
        }
        
        [Fact]
        public void ThenAssetsForATransmissionShouldBePutForEachLanguageWithTheCorrectParameters()
        {
            // 2 transmissions * 2 languages
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, ASSETS_DEFAULT_KEY, VALUE)).MustHaveHappened(Repeated.Exactly.Times(4));
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, ASSETS_KEY, VALUE)).MustHaveHappened(Repeated.Exactly.Times(4));
        }
    }
}