using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGradePack
{
    public class WhenAccessingItsOptionalOnForTheFirstTime : TestBase
    {
        IGradePack _gradePack;
        IEnumerable<Interfaces.ICarInfo> _carInfos;
        Repository.Objects.CarInfo _carInfo1;
        Repository.Objects.CarInfo _carInfo2;

        protected override void Arrange()
        {
            _carInfo1 = new CarInfoBuilder()
                .WithShortId(8)
                .Build();

            _carInfo2 = new CarInfoBuilder()
                .WithShortId(8)
                .Build();

            var repoGradePack = new GradePackBuilder()
                .AddOptionalOn(_carInfo1)
                .AddOptionalOn(_carInfo2)
                .Build();

            var repoGrade = new GradeBuilder().WithId(Guid.NewGuid()).Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var packService = A.Fake<IPackService>();
            A.CallTo(() => packService.GetGradePacks(publication.ID, publicationTimeFrame.ID, repoGrade.ID, context)).Returns(new List<Repository.Objects.Packs.GradePack> { repoGradePack });

            var gradeEquipmentFactory = new PackFactoryBuilder()
                .WithPackService(packService)
                .Build();

            _gradePack = gradeEquipmentFactory.GetGradePacks(publication, context, repoGrade.ID).Single();
        }

        protected override void Act()
        {
            _carInfos = _gradePack.OptionalOn;
        }

        [Fact]
        public void ThenItShouldHaveTheCarInfos()
        {
            _carInfos.Count().Should().Be(2);

            _carInfos.Should().Contain(carInfo => carInfo.ShortID == _carInfo1.ShortID);
            _carInfos.Should().Contain(carInfo => carInfo.ShortID == _carInfo2.ShortID);
        }
    }
}
