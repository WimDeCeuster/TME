﻿using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.CommandServices;
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
    public class WhenPublishingBodyTypeAssets : TestBase
    {
        private IAssetPublisher _publisher;
        private IService _s3Service;
        private IContext _context;
        private IAssetService _assetService;
        private Generation _generation;
        private Publication _publication;
        private readonly IEnumerable<string> _languages = new List<string>{"nl"};
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
            _publication = PublicationBuilder.Initialize().WithID(Guid.NewGuid()).Build();
            _bodyTypes = new List<BodyType>{
                new BodyTypeBuilder().WithId(Guid.NewGuid()).Build(),
                new BodyTypeBuilder().WithId(Guid.NewGuid()).Build()
            };

            //SetupForAssets
            _assets = new List<Asset>()
            {
                new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(new AssetTypeBuilder().WithMode(null).WithView(null).Build()).Build(),
                new AssetBuilder().WithId(Guid.NewGuid()).WithAssetType(new AssetTypeBuilder().WithMode(MODE).WithView(VIEW).Build()).Build()
            };

            _context = ContextBuilder.InitialiseFakeContext()
                                     .WithBrand(Brand)
                                     .WithCountry(Country)
                                     .WithLanguages(_languages.ToArray())
                                     .WithPublication(_languages.First(), _publication)
                                     .WithBodyTypes(_languages.First(),_bodyTypes)
                                     .WithAssets(_languages.First(), _assets, _bodyTypes[0].ID)
                                     .Build();

            _s3Service = A.Fake<IService>();
            var serialiser = A.Fake<ISerialiser>();
            var keymanager = A.Fake<IKeyManager>();

            _assetService = new AssetsService(_s3Service, serialiser, keymanager);
            _publisher = new AssetPublisher(_assetService);

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Asset>>._)).Returns(SERIALIZEDASSETS);

            A.CallTo(() => keymanager.GetDefaultAssetsKey(A<Guid>._,A<Guid>._))
                .Returns(BODYTYPE_DEFAULT_ASSETKEY);
            
            A.CallTo(() => keymanager.GetAssetsKey(A<Guid>._,A<Guid>._,VIEW,MODE))
                .Returns(BODYTYPE_ASSETKEY);
        }

        protected override void Act()
        {
            var result = _publisher.PublishAssets(_context);
        }

        [Fact]
        public void ThenAssetsForASpecificBodyTypeShouldBePutForEachLanguageAndBodyType()
        {
            foreach (var language in _languages)
            {
                A.CallTo(() => _s3Service.PutObjectAsync(null, null, null, null)).WithAnyArguments().MustHaveHappened(ForEachBodyType(language));
            }
        }

        [Fact]
        public void ThenAssetsForASpecificBodyTypeShouldBePutWithCorrectArgumentsForEachLanguageAndBodyType()
        {
            foreach (var language in _languages)
            {
                A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, BODYTYPE_ASSETKEY, SERIALIZEDASSETS)).MustHaveHappened(Repeated.Exactly.Once);
                A.CallTo(() => _s3Service.PutObjectAsync(Brand, Country, BODYTYPE_DEFAULT_ASSETKEY, SERIALIZEDASSETS)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void ThenDefaultAssetsShouldBePutInTheDefaultFolder()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand,Country,null,SERIALIZEDASSETS))
                .WhenArgumentsMatch(args => args[2].Equals(BODYTYPE_DEFAULT_ASSETKEY)).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenViewModeAssetsShouldBePutInTheViewModeFolder()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(Brand,Country,null,SERIALIZEDASSETS))
                .WhenArgumentsMatch(args => args[2].Equals(BODYTYPE_ASSETKEY)).MustHaveHappened(Repeated.Exactly.Once);
        }
        
        private Repeated ForEachBodyType(string language)
        {
            return Repeated.Exactly.Times((_context.ContextData[language].BodyTypes.Count));
        }
    }
}