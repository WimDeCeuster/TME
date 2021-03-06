﻿using System.Collections.Generic;
using FakeItEasy;
using System;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.CommandServices;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;

namespace TME.Carconfigurator.Tests.GivenAS3SpecificationsPublisher
{
    public class WhenPublishingSpecificationCategories : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedSpecificationCategories = "serialised specification categories";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _language1SpecificationsKey = "language 1 specification categories key";
        const String _language2SpecificationsKey = "language 2 specification categories key";
        IService _s3Service;
        ISpecificationsService _service;
        ISpecificationsPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var specificationCategory1 = new SpecificationCategoryBuilder().Build();
            var specificationCategory2 = new SpecificationCategoryBuilder().Build();
            var specificationCategory3 = new SpecificationCategoryBuilder().Build();
            var specificationCategory4 = new SpecificationCategoryBuilder().Build();

            var publication1 = new PublicationBuilder()
                                .WithID(Guid.NewGuid())
                                .Build();

            var publication2 = new PublicationBuilder()
                                .WithID(Guid.NewGuid())
                                .Build();

            _context = new ContextBuilder()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .WithLanguages(_language1, _language2)
                        .WithPublication(_language1, publication1)
                        .WithPublication(_language2, publication2)
                        .WithSpecificationCategories(_language1, specificationCategory1, specificationCategory2)
                        .WithSpecificationCategories(_language2, specificationCategory3, specificationCategory4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new SpecificationsService(_s3Service, serialiser, keyManager);
            _publisher = new SpecificationsPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Category>>._))
                .Returns(_serialisedSpecificationCategories);

            A.CallTo(() => keyManager.GetSpecificationCategoriesKey(publication1.ID)).Returns(_language1SpecificationsKey);
            A.CallTo(() => keyManager.GetSpecificationCategoriesKey(publication2.ID)).Returns(_language2SpecificationsKey);
        }

        protected override void Act()
        {
             _publisher.PublishCategoriesAsync(_context).Wait();
        }

        [Fact]
        public void ThenGenerationSpecificationCategoriesShouldBePutForAllLanguages()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(2));
        }

        [Fact]
        public void ThenGenerationSpecificationsShouldBePutForLanguage1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _language1SpecificationsKey, _serialisedSpecificationCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }                                                                                                   
                                                                                                            
        [Fact]                                                                                              
        public void ThenGenerationSpecificationsShouldBePutForLanguage2()                                 
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _language2SpecificationsKey, _serialisedSpecificationCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }
    }
}
