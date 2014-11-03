using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using FluentAssertions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.S3.QueryServices;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenASubModelGrade
{
    public class WhenAccessingItsEquipmentForTheFirstTime : TestBase
    {
        private IGrade _subModelGrade;
        private GradeAccessory _accessory1;
        private GradeAccessory _accessory2;
        private GradeOption _option1;
        private GradeOption _option2;
        private IEnumerable<IGradeEquipmentItem> _equipment;
        private IGradeEquipmentService _gradeEquipmentService;

        protected override void Arrange()
        {
            _accessory1 = new GradeAccessoryBuilder().WithId(Guid.NewGuid()).Build();
            _accessory2 = new GradeAccessoryBuilder().WithId(Guid.NewGuid()).Build();

            _option1 = new GradeOptionBuilder().WithId(Guid.NewGuid()).Build();
            _option2 = new GradeOptionBuilder().WithId(Guid.NewGuid()).Build();

            var gradeEquipment =
                new GradeEquipmentBuilder()
                    .WithAccessories(_accessory1, _accessory2)
                    .WithOptions(_option1, _option2)
                    .Build();

            var repoGrade = new GradeBuilder().WithId(Guid.NewGuid()).Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder().WithDateRange(DateTime.MinValue,DateTime.MaxValue).Build();

            var publication = new PublicationBuilder().WithID(Guid.NewGuid()).AddTimeFrame(publicationTimeFrame).Build();

            var subModelID = Guid.NewGuid();

            var context = new ContextBuilder().Build();

            _gradeEquipmentService = A.Fake<IGradeEquipmentService>();
            A.CallTo(
                () =>
                    _gradeEquipmentService.GetSubModelGradeEquipment(publication.ID, publicationTimeFrame.ID,
                        repoGrade.ID, subModelID, context)).Returns(gradeEquipment);

            var gradeEquipmentFactory =
                new GradeEquipmentFactoryBuilder().WithGradeEquipmentService(_gradeEquipmentService).Build();

            var gradeService = A.Fake<IGradeService>();
            A.CallTo(() => gradeService.GetSubModelGrades(publication.ID, publicationTimeFrame.ID, subModelID, context)).Returns(new [] {repoGrade});

            var gradeFactory = new GradeFactoryBuilder().WithGradeService(gradeService).WithGradeEquipmentFactory(gradeEquipmentFactory).Build();

            _subModelGrade = gradeFactory.GetSubModelGrades(subModelID, publication, context).First();
        }

        protected override void Act()
        {
            _equipment = _subModelGrade.Equipment;
        }

        [Fact]
        public void ThenItShouldFetchItsEquipmentFromTheService()
        {
            A.CallTo(() => _gradeEquipmentService.GetSubModelGradeEquipment(A<Guid>._,A<Guid>._,A<Guid>._,A<Guid>._,A<Context>._))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public void ThenItShouldHaveTheCorrectEquipment()
        {
            _equipment.Should().HaveCount(4);

            _equipment.Should().Contain(e => e.ID == _option1.ID);
            _equipment.Should().Contain(e => e.ID == _option2.ID);
            _equipment.Should().Contain(e => e.ID == _accessory1.ID);
            _equipment.Should().Contain(e => e.ID == _accessory2.ID);
        }
    }
}