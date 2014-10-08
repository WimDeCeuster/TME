using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Publisher;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3BodyTypeService
{
    public class WhenPublishingGenerationBodyTypes : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedBodyType1 = "serialised body type 1";
        const String _serialisedBodyType12 = "serialised body type 1+2";
        const String _serialisedBodyType34 = "serialised body type 3+4";
        const String _serialisedBodyType4 = "serialised body type 4";
        const string _language1 = "lang 1";
        const string _language2 = "lang 2";
        Guid _publicationId1 = Guid.NewGuid();
        Guid _publicationId2 = Guid.NewGuid();
        Guid _timeFrameId1;
        Guid _timeFrameId2;
        Guid _timeFrameId3;
        Guid _timeFrameId4;
        IService _s3Service;
        S3BodyTypeService _service;
        IContext _context;

        protected override void Arrange()
        {
            var publication1 = new Publication { ID = _publicationId1 };
            var publication2 = new Publication { ID = _publicationId2 };

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

            _timeFrameId1 = timeFrame1.ID;
            _timeFrameId2 = timeFrame2.ID;
            _timeFrameId3 = timeFrame3.ID;
            _timeFrameId4 = timeFrame4.ID;

            var generationBodyType1 = new BodyType { ID = bodyTypeId1 };
            var generationBodyType2 = new BodyType { ID = bodyTypeId2 };
            var generationBodyType3 = new BodyType { ID = bodyTypeId3 };
            var generationBodyType4 = new BodyType { ID = bodyTypeId4 };

            _context = ContextBuilder.InitialiseFakeContext()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .WithLanguages(_language1, _language2)
                        .WithPublication(_language1, publication1)
                        .WithPublication(_language2, publication2)
                        .WithCars(_language1, car1, car2)
                        .WithCars(_language2, car3, car4)
                        .WithTimeFrames(_language1, timeFrame1, timeFrame2)
                        .WithTimeFrames(_language2, timeFrame3, timeFrame4)
                        .WithGenerationBodyTypes(_language1, generationBodyType1, generationBodyType2)
                        .WithGenerationBodyTypes(_language2, generationBodyType3, generationBodyType4)
                        .Build();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();

            _service = new S3BodyTypeService(_s3Service, serialiser);

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
        }

        Func<ArgumentCollection, Boolean> ArgumentMatchesList<T>(params T[] items)
        {
            return args => {
                var argumentItems = (IEnumerable<T>)args[0];
                return argumentItems.Count() == items.Length &&
                       argumentItems.Zip(items, (item1, item2) => Object.Equals(item1, item2))
                                    .All(x => x);
            };
        }

        protected override void Act()
        {
            var result = _service.PutGenerationBodyTypes(_context);
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
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, "publication/" + _publicationId1.ToString() + "/time-frame/" + _timeFrameId1.ToString() + "/body-types", _serialisedBodyType1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationBodyTypesShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, "publication/" + _publicationId1.ToString() + "/time-frame/" + _timeFrameId2.ToString() + "/body-types", _serialisedBodyType12))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationBodyTypesShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, "publication/" + _publicationId2.ToString() + "/time-frame/" + _timeFrameId3.ToString() + "/body-types", _serialisedBodyType34))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationBodyTypesShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, "publication/" + _publicationId2.ToString() + "/time-frame/" + _timeFrameId4.ToString() + "/body-types", _serialisedBodyType4))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
