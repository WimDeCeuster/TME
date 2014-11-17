using System;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACar
{
    public class WhenAccessingItsGradeForTheSecondTime : TestBase
    {
        ICar _car;
        private Repository.Objects.Grade _repoGrade;
        private IGrade _secondGrade;
        private IGrade _firstGrade;
        private IGradeFactory _gradeFactory;

        protected override void Arrange()
        {
            _repoGrade = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCar = new CarBuilder()
                .WithGrade(_repoGrade)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var carService = A.Fake<ICarService>();
            A.CallTo(() => carService.GetCars(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new[] { repoCar });

            _gradeFactory = new GradeFactoryBuilder().Build();

            var carFactory = new CarFactoryBuilder()
                .WithCarService(carService)
                .WithGradeFactory(_gradeFactory)
                .Build();

            _car = carFactory.GetCars(publication, context).Single();

            _firstGrade = _car.Grade;
        }

        protected override void Act()
        {
            _secondGrade = _car.Grade;
        }

        [Fact]
        public void ThenTheGradeShouldNotBeRecalculated()
        {
            _secondGrade.Should().BeSameAs(_firstGrade);
        }

        [Fact]
        public void ThenTheGradeShouldBeCorrect()
        {
            _secondGrade.ID.Should().Be(_repoGrade.ID);
        }
    }
}