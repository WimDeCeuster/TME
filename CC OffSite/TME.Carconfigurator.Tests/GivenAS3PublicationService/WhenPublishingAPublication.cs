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

namespace TME.Carconfigurator.Tests.GivenAS3PublicationService
{
    public class WhenPublishingAPublication : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedPublication1 = "serialised publication 1";
        const String _serialisedPublication2 = "serialised publication 2";
        const string _language1 = "lang 1";
        const string _language2 = "lang 2";
        Guid _publicationId1 = Guid.NewGuid();
        Guid _publicationId2 = Guid.NewGuid();
        IService _s3Service;
        S3PublicationService _service;
        IContext _context;

        protected override void Arrange()
        {
            _context = new Context(_brand, _country, Guid.Empty, CarConfigurator.Publisher.Enums.PublicationDataSubset.Live);

            var publication1 = new Publication { ID = _publicationId1 };
            var publication2 = new Publication { ID = _publicationId2 };

            _context.ContextData[_language1] = new ContextData();
            _context.ContextData[_language1].Publication = publication1;

            _context.ContextData[_language2] = new ContextData();
            _context.ContextData[_language2].Publication = publication2;

            _s3Service = A.Fake<IService>();
            
            var serialiser = A.Fake<ISerialiser>();

            _service = new S3PublicationService(_s3Service, serialiser);

            A.CallTo(() => serialiser.Serialise(publication1)).Returns(_serialisedPublication1);
            A.CallTo(() => serialiser.Serialise(publication2)).Returns(_serialisedPublication2);
        }

        protected override void Act()
        {
            var result = _service.PutPublications(_context);
        }

        [Fact]
        public void ThenAPublicationShouldBePutForAllLanguages()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(null, null, null, null))
                .WithAnyArguments()
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void ThenAPublicationShouldBePutForLanguage1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, "publication/" + _publicationId1.ToString(), _serialisedPublication1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublicationShouldBePutForLanguage2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, "publication/" + _publicationId2.ToString(), _serialisedPublication2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
