using System.Collections.Generic;
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
        const String _timeFrame1SpecificationsKey = "time frame 1 specification categories key";
        const String _timeFrame2SpecificationsKey = "time frame 2 specification categories key";
        const String _timeFrame3SpecificationsKey = "time frame 3 specification categories key";
        const String _timeFrame4SpecificationsKey = "time frame 4 specification categories key";
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

            var timeFrame1 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithSpecificationCategories(specificationCategory1)
                                .Build();
            var timeFrame2 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithSpecificationCategories(specificationCategory1, specificationCategory2)
                                .Build();
            var timeFrame3 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithSpecificationCategories(specificationCategory3, specificationCategory4)
                                .Build();
            var timeFrame4 = new TimeFrameBuilder()
                                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                                .WithSpecificationCategories(specificationCategory4)
                                .Build();
            
            var publicationTimeFrame1 = new PublicationTimeFrame { ID = timeFrame1.ID };
            var publicationTimeFrame2 = new PublicationTimeFrame { ID = timeFrame2.ID };
            var publicationTimeFrame3 = new PublicationTimeFrame { ID = timeFrame3.ID };
            var publicationTimeFrame4 = new PublicationTimeFrame { ID = timeFrame4.ID };

            var publication1 = PublicationBuilder.Initialize()
                                                 .WithTimeFrames(publicationTimeFrame1,
                                                                 publicationTimeFrame2)
                                                 .Build();

            var publication2 = PublicationBuilder.Initialize()
                                                 .WithTimeFrames(publicationTimeFrame3,
                                                                 publicationTimeFrame4)
                                                 .Build();

            _context = new ContextBuilder()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .WithLanguages(_language1, _language2)
                        .WithPublication(_language1, publication1)
                        .WithPublication(_language2, publication2)
                        .WithTimeFrames(_language1, timeFrame1, timeFrame2)
                        .WithTimeFrames(_language2, timeFrame3, timeFrame4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new SpecificationsService(_s3Service, serialiser, keyManager);
            _publisher = new SpecificationsPublisherBuilder().WithService(_service).Build();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<Category>>._))
                .Returns(_serialisedSpecificationCategories);

            A.CallTo(() => keyManager.GetSpecificationCategoriesKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1SpecificationsKey);
            A.CallTo(() => keyManager.GetSpecificationCategoriesKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2SpecificationsKey);
            A.CallTo(() => keyManager.GetSpecificationCategoriesKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3SpecificationsKey);
            A.CallTo(() => keyManager.GetSpecificationCategoriesKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4SpecificationsKey);
        }

        protected override void Act()
        {
             _publisher.PublishCategoriesAsync(_context).Wait();
        }

        [Fact]
        public void ThenGenerationSpecificationCategoriesShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationSpecificationsShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1SpecificationsKey, _serialisedSpecificationCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }                                                                                                   
                                                                                                            
        [Fact]                                                                                              
        public void ThenGenerationSpecificationsShouldBePutForTimeFrame2()                                 
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2SpecificationsKey, _serialisedSpecificationCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }                                                                                                   
                                                                                                            
        [Fact]                                                                                              
        public void ThenGenerationSpecificationsShouldBePutForTimeFrame3()                                 
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3SpecificationsKey, _serialisedSpecificationCategories))
                .MustHaveHappened(Repeated.Exactly.Once);                                                   
        }                                                                                                   
                                                                                                            
        [Fact]                                                                                              
        public void ThenGenerationSpecificationsShouldBePutForTimeFrame4()                                 
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4SpecificationsKey, _serialisedSpecificationCategories))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
