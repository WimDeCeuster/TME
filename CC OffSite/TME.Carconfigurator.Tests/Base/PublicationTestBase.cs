using System;
using System.Linq;
using FakeItEasy;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Publisher.S3;
using TME.Carconfigurator.Tests.Builders;
using TME.Carconfigurator.Tests.TestImplementations;

namespace TME.Carconfigurator.Tests.Base
{
    public abstract class PublicationTestBase : TestBase
    {
        protected TestS3Service Service;
        protected S3Publisher Publisher;
        protected IS3Serialiser Serialiser;
        protected IContext Context;
        protected String Brand = "Toyota";
        protected String Country = "BE";
        protected String[] Languages = new[] { "nl", "fr", "de", "en" };

        protected String GuidRegexPattern = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b";


        protected override void Arrange()
        {
            Service = new TestS3Service();
            Serialiser = A.Fake<IS3Serialiser>();

            A.CallTo(() => Serialiser.Serialise(null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);

            Publisher = new S3Publisher(Service, Serialiser);
            Context = ContextBuilder.GetDefaultContext(Languages);
        }

        protected override void Act()
        {
            Publisher.Publish(Context);
        }
    }
}
