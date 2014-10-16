using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenATransmission
{
    public class WhenAccessingItsTransmissionTypeForTheSecondTime : TestBase
    {
        ITransmission _transmission;
        ITransmissionType _firstType;
        ITransmissionType _secondType;
        Repository.Objects.TransmissionType _repoType;

        protected override void Arrange()
        {
            _repoType = new CarConfigurator.Tests.Shared.TestBuilders.TransmissionTypeBuilder().Build();

            var repoTransmission = new TransmissionBuilder()
                .WithType(_repoType)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var transmissionService = A.Fake<ITransmissionService>();
            A.CallTo(() => transmissionService.GetTransmissions(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.Transmission> { repoTransmission });

            var transmissionFactory = new TransmissionFactoryBuilder()
                .WithTransmissionService(transmissionService)
                .Build();

            _transmission = transmissionFactory.GetTransmissions(publication, context).Single();

            _firstType = _transmission.Type;
        }

        protected override void Act()
        {
            _secondType = _transmission.Type;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheType()
        {
            _secondType.Should().Be(_firstType);
        }

        [Fact]
        public void ThenItShouldHaveTheType()
        {
            _secondType.ID.Should().Be(_repoType.ID);
        }


    }
}
