using FakeItEasy;
using System;
using System.Collections.Generic;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.CommandServices;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.S3.Publisher;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.GivenAS3BodyTypePublisher
{
    public class WhenPublishingGenerationBodyTypes : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedBodyType1 = "serialised body type 1";
        const String _serialisedBodyType12 = "serialised body type 1+2";
        const String _serialisedBodyType34 = "serialised body type 3+4";
        const String _serialisedBodyType4 = "serialised body type 4";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _timeFrame1BodyTypesKey = "time frame 1 body types key";
        const String _timeFrame2BodyTypesKey = "time frame 2 body types key";
        const String _timeFrame3BodyTypesKey = "time frame 3 body types key";
        const String _timeFrame4BodyTypesKey = "time frame 4 body types key";
        IService _s3Service;
        IBodyTypeService _service;
        IBodyTypePublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var bodyTypeId1 = Guid.NewGuid();
            var bodyTypeId2 = Guid.NewGuid();
            var bodyTypeId3 = Guid.NewGuid();
            var bodyTypeId4 = Guid.NewGuid();

            var car1 = new Car { BodyType = new BodyType { ID = bodyTypeId1 } };
            var car2 = new Car { BodyType = new BodyType { ID = bodyTypeId2 } };
            var car3 = new Car { BodyType = new BodyType { ID = bodyTypeId3 } };
            var car4 = new Car { BodyType = new BodyType { ID = bodyTypeId4 } };

            var timeFrame1 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[] { car1 });
            var timeFrame2 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[] { car1, car2 });
            var timeFrame3 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[] { car3, car4 });
            var timeFrame4 = new TimeFrame(DateTime.MinValue, DateTime.MaxValue, new[] { car4 });

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

            var generationBodyType1 = new BodyType { ID = bodyTypeId1 };
            var generationBodyType2 = new BodyType { ID = bodyTypeId2 };
            var generationBodyType3 = new BodyType { ID = bodyTypeId3 };
            var generationBodyType4 = new BodyType { ID = bodyTypeId4 };

            _context = new ContextBuilder()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .WithLanguages(_language1, _language2)
                        .WithPublication(_language1, publication1)
                        .WithPublication(_language2, publication2)
                        .WithCars(_language1, car1, car2)
                        .WithCars(_language2, car3, car4)
                        .WithTimeFrames(_language1, timeFrame1, timeFrame2)
                        .WithTimeFrames(_language2, timeFrame3, timeFrame4)
                        .WithBodyTypes(_language1, generationBodyType1, generationBodyType2)
                        .WithBodyTypes(_language2, generationBodyType3, generationBodyType4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new BodyTypeService(_s3Service, serialiser, keyManager);
            _publisher = new BodyTypePublisher(_service);

            A.CallTo(() => serialiser.Serialise((IEnumerable<BodyType>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationBodyType1))
                .Returns(_serialisedBodyType1);
            A.CallTo(() => serialiser.Serialise((IEnumerable<BodyType>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationBodyType1, generationBodyType2))
                .Returns(_serialisedBodyType12);
            A.CallTo(() => serialiser.Serialise((IEnumerable<BodyType>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationBodyType3, generationBodyType4))
                .Returns(_serialisedBodyType34);
            A.CallTo(() => serialiser.Serialise((IEnumerable<BodyType>)null))
                .WhenArgumentsMatch(ArgumentMatchesList(generationBodyType4))
                .Returns(_serialisedBodyType4);

            A.CallTo(() => keyManager.GetBodyTypesKey(publication1.ID, publicationTimeFrame1.ID)).Returns(_timeFrame1BodyTypesKey);
            A.CallTo(() => keyManager.GetBodyTypesKey(publication1.ID, publicationTimeFrame2.ID)).Returns(_timeFrame2BodyTypesKey);
            A.CallTo(() => keyManager.GetBodyTypesKey(publication2.ID, publicationTimeFrame3.ID)).Returns(_timeFrame3BodyTypesKey);
            A.CallTo(() => keyManager.GetBodyTypesKey(publication2.ID, publicationTimeFrame4.ID)).Returns(_timeFrame4BodyTypesKey);
        }

        protected override void Act()
        {
            var result = _publisher.PublishGenerationBodyTypes(_context);
        }

        [Fact]
        public void ThenGenerationBodyTypesShouldBePutForAllLanguagesAndTimeFrames()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(null, null, null, null))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationBodyTypesShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame1BodyTypesKey, _serialisedBodyType1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationBodyTypesShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame2BodyTypesKey, _serialisedBodyType12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationBodyTypesShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame3BodyTypesKey, _serialisedBodyType34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationBodyTypesShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _timeFrame4BodyTypesKey, _serialisedBodyType4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
