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

namespace TME.Carconfigurator.Tests.Base
{
    public class PublicationTest : TestBase
    {
        protected TestS3Service Service;
        protected S3Publisher Publisher;
        protected IContext Context;
        protected String Brand = "Toyota";
        protected String Country = "BE";
        protected String[] Languages = new[] { "nl", "fr", "de", "en" };

        protected String GuidRegexPattern = @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b";

        protected override void Arrange()
        {
            Service = new TestS3Service();
            var serialiser = A.Fake<IS3Serialiser>();

            A.CallTo(() => serialiser.Serialise(null))
                .WhenArgumentsMatch(args =>
                    args[0] is Publication)
                .Returns("publicationInfo");

            Publisher = new S3Publisher(Service, serialiser);
            Context = ContextBuilder.GetDefaultContext();
        }

        protected override void Act()
        {
            Publisher.Publish(Context);
        }
    }
}
