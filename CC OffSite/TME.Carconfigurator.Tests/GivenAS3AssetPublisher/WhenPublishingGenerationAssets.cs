using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Assets;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.S3.Shared.Result;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders.RepositoryObjects;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3AssetPublisher
{
    public class WhenPublishingGenerationAssets : TestBase
    {
        private IAssetPublisher _publisher;
        private IService _s3Service;
        private IContext _context;
        private IAssetService _assetService;
        private Generation _generation;
        private readonly List<Asset> _assets = new List<Asset>();
        private Publication _publication;
        private IEnumerable<string> _languages = new List<string>{"nl"};
        private const string GENERATIONASSETKEY = "generationAssetKey";
        private const string SERIALIZEDASSETS = "serializedAssets";
        public const String Country = "BE";
        private const String Brand = "Toyota";
        private const String Language = "nl";

        protected override void Arrange()
        {
            _publication = PublicationBuilder.Initialize().WithID(Guid.NewGuid()).Build();

            //SetupForAssets
            for (int i = 0; i < 3; i++)
            {
                _assets.Add(new Asset()
                {
                    AlwaysInclude = true,
                    AssetType = new AssetType(),
                    FileName = "Filename" + i,
                    FileType = new FileType(),
                    Hash = "Hash" + i,
                    Height = short.Parse(i.ToString()),
                    Width = short.Parse(i.ToString()),
                    PositionX = short.Parse(i.ToString()),
                    PositionY = short.Parse(i.ToString()),
                    ID = Guid.NewGuid(),
                    IsTransparent = true,
                    Name = "Name" + i,
                    RequiresMatte = true,
                    ShortID = i,
                    StackingOrder = i
                });
            }

            _context = ContextBuilder.InitialiseFakeContext()
                                     .WithBrand(Brand)
                                     .WithCountry(Country)
                                     .WithLanguages(Language)
                                     .WithPublication(Language, _publication)
                                     .WithAssets(Language,_assets)
                                     .Build();

            _s3Service = A.Fake<IService>();
            var serialiser = A.Fake<ISerialiser>();
            var keymanager = A.Fake<IKeyManager>();

            _assetService = new AssetsService(_s3Service, serialiser, keymanager);
            _publisher = new AssetPublisher(_assetService);

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Asset>>._)).Returns(SERIALIZEDASSETS);
            A.CallTo(() => keymanager.GetGenerationAssetKey(_publication.ID)).Returns(GENERATIONASSETKEY);
        }

        protected override void Act()
        {
            var result = _publisher.PublishAssets(_context);
        }

        [Fact]
        public void ThenAssetsForASpecificGenerationShouldBePut()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(null,null,null,null)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Times(_assets.Count));
        }
        
        [Fact]
        public void ThenAssetsForASpecificGenerationShouldBePutWithCorrectArguments()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(null,null,null,null)).WhenArgumentsMatch(args => ((args[0].Equals(Brand)) && (args[1].Equals(Country)) && (args[2].Equals(GENERATIONASSETKEY)) &&
                                                                                                       (args[3].Equals(SERIALIZEDASSETS)))).MustHaveHappened(Repeated.Exactly.Times(_assets.Count));
        }
    }
}