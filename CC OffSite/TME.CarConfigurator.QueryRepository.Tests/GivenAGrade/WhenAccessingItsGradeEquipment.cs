using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Query.Tests.GivenAGrade
{
    public class WhenAccessingItsGradeEquipment : TestBase
    {
        private IEnumerable<IGradeEquipmentItem> _equipment;
        private Repository.Objects.Equipment.GradeEquipment _repositoryGradeEquipment;
        private IGradeEquipmentService _gradeEquipmentService;
        private IGrade _grade;

        protected override void Arrange()
        {
            _repositoryGradeEquipment = new GradeEquipmentBuilder()
                .WithAccessories(
                    new GradeAccessoryBuilder().WithId(Guid.NewGuid()).Build(),
                    new GradeAccessoryBuilder().WithId(Guid.NewGuid()).Build())
                .WithOptions(
                    new GradeOptionBuilder().WithId(Guid.NewGuid()).Build(),
                    new GradeOptionBuilder().WithId(Guid.NewGuid()).Build())
                .Build();

            var repoGrade = new GradeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var gradeService = A.Fake<IGradeService>();
            A.CallTo(() => gradeService.GetGrades(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new List<Repository.Objects.Grade> { repoGrade });

            _gradeEquipmentService = A.Fake<IGradeEquipmentService>(opt => opt.Strict());
            A.CallTo(() => _gradeEquipmentService.GetGradeEquipment(publication.ID, publicationTimeFrame.ID, repoGrade.ID, context)).Returns(_repositoryGradeEquipment);

            var gradeEquipmentFactory = new GradeEquipmentFactoryBuilder()
                .WithGradeEquipmentService(_gradeEquipmentService)
                .Build();

            var gradeFactory = new GradeFactoryBuilder()
                .WithGradeService(gradeService)
                .WithGradeEquipmentFactory(gradeEquipmentFactory)
                .Build();

            _grade = gradeFactory.GetGrades(publication, context).Single();
        }

        protected override void Act()
        {
            _equipment = _grade.Equipment;
        }

        [Fact]
        public void ThenItShouldFetchTheGradeEquipmentFromTheService()
        {
            A.CallTo(() => _gradeEquipmentService.GetGradeEquipment(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectGradeEquipment()
        {
            _equipment.Count().Should().Be(4);

            _equipment.OfType<IGradeAccessory>()
                      .All(accessory =>
                          _repositoryGradeEquipment.Accessories.Any(
                            repoAccessory => repoAccessory.ID == accessory.ID))
                      .Should().Be(true);

            _equipment.OfType<IGradeOption>()
                      .All(option =>
                          _repositoryGradeEquipment.Options.Any(
                            repoOption => repoOption.ID == option.ID))
                      .Should().Be(true);
        }
    }
}