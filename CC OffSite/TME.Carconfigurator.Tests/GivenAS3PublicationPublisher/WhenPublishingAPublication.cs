using FakeItEasy;
using System;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.S3.CommandServices;
using TME.Carconfigurator.Tests.Builders;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.S3.Shared.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using Xunit;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.S3.Publisher;
namespace TME.Carconfigurator.Tests.GivenAS3PublicationPublisher
{
    public class WhenPublishingAPublication : TestBase
    {
        const String _brand = "Toyota";
        const String _country = "DE";
        const String _serialisedPublication1 = "serialised publication 1";
        const String _serialisedPublication2 = "serialised publication 2";
        const String _language1 = "lang 1";
        const String _language2 = "lang 2";
        const String _publication1Key = "publication 1 key";
        const String _publication2Key = "publication 2 key";
        Guid _publication1ID = Guid.NewGuid();
        Guid _publication2ID = Guid.NewGuid();
        IService _s3Service;
        IPublicationService _service;
        IPublicationPublisher _publisher;
        IContext _context;

        protected override void Arrange()
        {
            var publication1 = new Publication { ID = _publication1ID };
            var publication2 = new Publication { ID = _publication2ID };

            _context = new ContextBuilder()
                        .WithBrand(_brand)
                        .WithCountry(_country)
                        .WithLanguages(_language1, _language2)
                        .WithPublication(_language1, publication1)
                        .WithPublication(_language2, publication2)
                        .Build();

            _s3Service = A.Fake<IService>();
            
            var serialiser = A.Fake<ISerialiser>();
            var keyManager = A.Fake<IKeyManager>();

            _service = new PublicationService(_s3Service, serialiser, keyManager);
            _publisher = new PublicationPublisher(_service);

            A.CallTo(() => serialiser.Serialise(publication1)).Returns(_serialisedPublication1);
            A.CallTo(() => serialiser.Serialise(publication2)).Returns(_serialisedPublication2);
            A.CallTo(() => keyManager.GetPublicationKey(publication1.ID)).Returns(_publication1Key);
            A.CallTo(() => keyManager.GetPublicationKey(publication2.ID)).Returns(_publication2Key);
        }

        protected override void Act()
        {
            var result = _publisher.PublishPublicationsAsync(_context).Result;
        }

        [Fact]
        public void ThenAPublicationShouldBePutForAllLanguages()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(A<string>._, A<string>._, A<string>._, A<string>._))
                .MustHaveHappened(Repeated.Exactly.Twice);
        }

        [Fact]
        public void ThenAPublicationShouldBePutForLanguage1()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _publication1Key, _serialisedPublication1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenAPublicationShouldBePutForLanguage2()
        {
            A.CallTo(() => _s3Service.PutObjectAsync(_brand, _country, _publication2Key, _serialisedPublication2))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}
