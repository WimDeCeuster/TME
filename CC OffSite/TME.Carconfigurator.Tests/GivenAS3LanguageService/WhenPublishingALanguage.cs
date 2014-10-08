using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenAS3LanguageService
{
    public class WhenPublishingALanguage : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedLanguages = "serialised languages";
        const string _language1 = "lang 1";
        const string _language2 = "lang 2";
        Guid _publicationId1 = Guid.NewGuid();
        Guid _publicationId2 = Guid.NewGuid();
        IService _s3Service;
        S3LanguageService _service;
        IContext _context;
        Languages _languages;

        protected override void Arrange()
        {
            _context = ContextBuilder.InitialiseFakeContext()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .Build();

            _languages = new Languages();

            _s3Service = A.Fake<IService>();

            var serialiser = A.Fake<ISerialiser>();

            _service = new S3LanguageService(_s3Service, serialiser);

            A.CallTo(() => serialiser.Serialise(_languages)).Returns(_serialisedLanguages);
            
        }

        protected override void Act()
        {
            var result = _service.PutModelsOverviewPerLanguage(_context, _languages).Result;
        }

        [Fact]
        public void ThenLanguagesShouldBePut()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, "models-per-language", _serialisedLanguages))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
