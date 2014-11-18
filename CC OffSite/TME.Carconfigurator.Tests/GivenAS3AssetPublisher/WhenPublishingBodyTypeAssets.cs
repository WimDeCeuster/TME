using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
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
    public class WhenPublishingBodyTypeAssets : TestBase
    {
        private IAssetPublisher _publisher;
        private IService _s3Service;
        private IContext _context;
        private IAssetService _assetService;
        private Publication _publication;
        private readonly IEnumerable<string> _languages = new List<string> { "nl" };
        private List<Asset> _assets;
        private List<BodyType> _bodyTypes;
        private const string VIEW = "view";
        private const string MODE = "mode";
        private const string BODYTYPE_ASSETKEY = "Body Type Asset Key";
        private const string BODYTYPE_DEFAULT_ASSETKEY = "Body Type Default Asset Key";
        private const string SERIALIZEDASSETS = "serializedAssets";
        public const String Country = "BE";
        private const String Brand = "Toyota";

        protected override void Arrange()
        {
            _publication = new PublicationBuilder().WithID(Guid.NewGuid()).Build();
            _bodyTypes = new List<BodyType>{
                new BodyTypeBuilder().WithId(Guid.NewGuid()).Build(),
                new BodyTypeBuilder().WithId(Guid.NewGuid()).Build()
            };

            //SetupForAssets
            _assets = new List<Asset>()
            {
                new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(new AssetTypeBuilder().Build()).Build(),
                new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(new AssetTypeBuilder().WithMode(MODE).WithView(VIEW).Build()).Build()
            };

            _context = new ContextBuilder()
                        .WithBrand(Brand)
                        .WithCountry(Country)
                        .WithLanguages(_languages.ToArray())
                        .WithPublication(_languages.First(), _publication)
                        .WithAssets(_languages.First(), _assets, _bodyTypes[0].ID)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Asset>>._)).Returns(SERIALIZEDASSETS);
            var keymanager = A.Fake<IKeyManager>();
            A.CallTo(() => keymanager.GetDefaultAssetsKey(A<Guid>._, A<Guid>._))
                .Returns(BODYTYPE_DEFAULT_ASSETKEY);
            A.CallTo(() => keymanager.GetAssetsKey(A<Guid>._, A<Guid>._, VIEW, MODE))
                .Returns(BODYTYPE_ASSETKEY);

            _assetService = new AssetsService(_s3Service, serialiser, keymanager);
            _publisher = new AssetPublisher(_assetService);
        }

        protected override void Act()
        {
            _publisher.PublishAsync(_context).Wait();
        }

        [Fact]
        public void ThenAssetsForASpecificBodyTypeShouldBePutForEachLanguageAndBodyType()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._)).MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void ThenAssetsForASpecificBodyTypeShouldBePutWithCorrectArgumentsForEachLanguageAndBodyType()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, BODYTYPE_ASSETKEY, SERIALIZEDASSETS)).MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, BODYTYPE_DEFAULT_ASSETKEY, SERIALIZEDASSETS)).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}