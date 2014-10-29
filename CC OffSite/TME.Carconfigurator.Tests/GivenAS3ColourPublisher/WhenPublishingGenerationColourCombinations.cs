using System;
using System.Collections.Generic;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Colours;
using TME.CarConfigurator.S3.CommandServices;
using TME.CarConfigurator.S3.Publisher.Interfaces;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.Carconfigurator.Tests.GivenAS3ColourCombinationPublisher
{
    public class WhenPublishingGenerationColourCombinations : TestBase
    {
        private IColourPublisher _colourPublisher;
        private IContext _context;
        private IService _s3Service;
        private const string TIME_FRAME1_EXTERIOR_COLOUR_KEY = "Key for Exterior Colour For TimeFrame 1";
        private const string TIME_FRAME2_EXTERIOR_COLOUR_KEY = "Key for Exterior Colour For TimeFrame 2";
        private const string TIME_FRAME3_EXTERIOR_COLOUR_KEY = "Key for Exterior Colour For TimeFrame 3";
        private const string TIME_FRAME4_EXTERIOR_COLOUR_KEY = "Key for Exterior Colour For TimeFrame 4";
        private const string TIME_FRAME1_EXTERIOR_COLOUR_VALUE = "Serialised Exterior Colour For TimeFrame 1";
        private const string TIME_FRAME2_EXTERIOR_COLOUR_VALUE = "Serialised Exterior Colour For TimeFrame 2";
        private const string TIME_FRAME3_EXTERIOR_COLOUR_VALUE = "Serialised Exterior Colour For TimeFrame 3";
        private const string TIME_FRAME4_EXTERIOR_COLOUR_VALUE = "Serialised Exterior Colour For TimeFrame 4";
        private const string BRAND = "Toyota";
        private const string COUNTRY = "DE";
        private const string LANGUAGE1 = "de";
        private const string LANGUAGE2 = "en";

        protected override void Arrange()
        {
            var colourCombination1 = new ColourCombinationBuilder().Build();
            var colourCombination2 = new ColourCombinationBuilder().Build();
            var colourCombination3 = new ColourCombinationBuilder().Build();
            var colourCombination4 = new ColourCombinationBuilder().Build();

            var timeFrame1 = new TimeFrameBuilder().WithColourCombinations(new [] { colourCombination1 }).WithDateRange(DateTime.MinValue,DateTime.MaxValue).Build();
            var timeFrame2 = new TimeFrameBuilder().WithColourCombinations(new [] { colourCombination1, colourCombination2 }).WithDateRange(DateTime.MinValue, DateTime.MaxValue).Build();
            var timeFrame3 = new TimeFrameBuilder().WithColourCombinations(new [] { colourCombination3, colourCombination4 }).WithDateRange(DateTime.MinValue, DateTime.MaxValue).Build();
            var timeFrame4 = new TimeFrameBuilder().WithColourCombinations(new [] { colourCombination4 }).WithDateRange(DateTime.MinValue,DateTime.MaxValue).Build();

            var publicationTimeFrame1 = new PublicationTimeFrameBuilder().WithDateRange(DateTime.MinValue,DateTime.MaxValue).WithID(timeFrame1.ID).Build();
            var publicationTimeFrame2 = new PublicationTimeFrameBuilder().WithDateRange(DateTime.MinValue,DateTime.MaxValue).WithID(timeFrame2.ID).Build();
            var publicationTimeFrame3 = new PublicationTimeFrameBuilder().WithDateRange(DateTime.MinValue,DateTime.MaxValue).WithID(timeFrame3.ID).Build();
            var publicationTimeFrame4 = new PublicationTimeFrameBuilder().WithDateRange(DateTime.MinValue,DateTime.MaxValue).WithID(timeFrame4.ID).Build();

            var publication1 = new PublicationBuilder()
                .WithTimeFrames(publicationTimeFrame1, publicationTimeFrame2)
                .WithID(Guid.NewGuid())
                .Build();

            var publication2 = new PublicationBuilder()
                .WithTimeFrames(publicationTimeFrame3, publicationTimeFrame4)
                .WithID(Guid.NewGuid())
                .Build();

            _context = new ContextBuilder()
                .WithBrand(BRAND)
                .WithCountry(COUNTRY)
                .WithLanguages(LANGUAGE1,LANGUAGE2)
                .WithPublication(LANGUAGE1,publication1)
                .WithPublication(LANGUAGE2,publication2)
                .WithTimeFrames(LANGUAGE1,timeFrame1,timeFrame2)
                .WithTimeFrames(LANGUAGE2,timeFrame3,timeFrame4)
                .Build();

            _s3Service = A.Fake<IService>();
            var serialiser = A.Fake<ISerialiser>();

            A.CallTo(() => serialiser.Serialise(A<IEnumerable<ColourCombination>>.That.IsSameSequenceAs(new List<ColourCombination> { colourCombination1 })))
                .Returns(TIME_FRAME1_EXTERIOR_COLOUR_VALUE);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<ColourCombination>>.That.IsSameSequenceAs(new List<ColourCombination> { colourCombination1, colourCombination2 })))
                .Returns(TIME_FRAME2_EXTERIOR_COLOUR_VALUE);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<ColourCombination>>.That.IsSameSequenceAs(new List<ColourCombination> { colourCombination3, colourCombination4 })))
                .Returns(TIME_FRAME3_EXTERIOR_COLOUR_VALUE);
            A.CallTo(() => serialiser.Serialise(A<IEnumerable<ColourCombination>>.That.IsSameSequenceAs(new List<ColourCombination> { colourCombination4 })))
                .Returns(TIME_FRAME4_EXTERIOR_COLOUR_VALUE);

            var keymanager = A.Fake<IKeyManager>();

            A.CallTo(() => keymanager.GetColourCombinationsKey(publication1.ID, publicationTimeFrame1.ID))
                .Returns(TIME_FRAME1_EXTERIOR_COLOUR_KEY);
            A.CallTo(() => keymanager.GetColourCombinationsKey(publication1.ID, publicationTimeFrame2.ID))
                .Returns(TIME_FRAME2_EXTERIOR_COLOUR_KEY);
            A.CallTo(() => keymanager.GetColourCombinationsKey(publication2.ID, publicationTimeFrame3.ID))
                .Returns(TIME_FRAME3_EXTERIOR_COLOUR_KEY);
            A.CallTo(() => keymanager.GetColourCombinationsKey(publication2.ID, publicationTimeFrame4.ID))
                .Returns(TIME_FRAME4_EXTERIOR_COLOUR_KEY);

            var colourCombinationService = new ColourCombinationService(_s3Service, serialiser,keymanager);
            _colourPublisher = new ColourCombinationPublisherBuilder().WithService(colourCombinationService).Build();
        }

        protected override void Act()
        {
            var result = _colourPublisher.PublishGenerationColourCombinations(_context).Result;
        }

        [Fact]
        public void ThenGenerationColourCombinationsShouldBePutForEveryLanguageAndTimeFrame()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<String>._,A<String>._,A<String>._,A<String>._)).MustHaveHappened(Repeated.Exactly.Times(4));
        }

        [Fact]
        public void ThenGenerationColourCombinationsShouldBePutForTimeFrame1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME1_EXTERIOR_COLOUR_KEY, TIME_FRAME1_EXTERIOR_COLOUR_VALUE))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        [Fact]
        public void ThenGenerationColourCombinationsShouldBePutForTimeFrame2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME2_EXTERIOR_COLOUR_KEY, TIME_FRAME2_EXTERIOR_COLOUR_VALUE))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationColourCombinationsShouldBePutForTimeFrame3()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME3_EXTERIOR_COLOUR_KEY, TIME_FRAME3_EXTERIOR_COLOUR_VALUE))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenGenerationColourCombinationsShouldBePutForTimeFrame4()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(BRAND, COUNTRY, TIME_FRAME4_EXTERIOR_COLOUR_KEY, TIME_FRAME4_EXTERIOR_COLOUR_VALUE))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}