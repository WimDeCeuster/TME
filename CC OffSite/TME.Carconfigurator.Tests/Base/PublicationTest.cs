using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher;
using FakeItEasy;
using System.Collections.ObjectModel;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Core;
using TME.Carconfigurator.Tests.Builders;
using TME.Carconfigurator.Tests.TestImplementations;
using Newtonsoft.Json;

namespace TME.Carconfigurator.Tests.Base
{
    public abstract class PublicationTest : TestBase
    {
        protected TestS3Service Service;
        protected S3Publisher Publisher;
        protected IS3Serialiser Serialiser;
        protected IContext Context;
        protected String Brand = "Toyota";
        protected String Country = "BE";
        protected String[] Languages = new[] { "nl", "fr", "de", "en" };

        protected String GuidRegexPattern = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b";

        protected void BaseArrange()
        {
            Service = new TestS3Service();
            Serialiser = A.Fake<IS3Serialiser>();

            A.CallTo(() => Serialiser.Serialise(null)).WithAnyArguments().ReturnsLazily(args => args.Arguments.First().GetType().Name);

            Publisher = new S3Publisher(Service, Serialiser);
            Context = ContextBuilder.GetDefaultContext(Languages);
        }

        protected void BaseAct()
        {
            Publisher.Publish(Context);
        }
    }
}
