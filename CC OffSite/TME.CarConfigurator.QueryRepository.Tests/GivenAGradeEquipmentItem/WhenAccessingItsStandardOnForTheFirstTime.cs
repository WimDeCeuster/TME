using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGradeEquipmentItem
{
    public class WhenAccessingItsStandardOnForTheFirstTime : TestBase
    {
        IGradeEquipmentItem _gradeEquipmentItem;
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

            var repoGradeEquipmentItem = new GradeAccessoryBuilder()
                .WithStandardOn(_carInfo1, _carInfo2)
                .Build();

            var repoGradeEquipment = new GradeEquipmentBuilder()
                .WithAccessories(repoGradeEquipmentItem)
                .Build();

            var publicationTimeFrame = new PublicationTimeFrameBuilder()
                .WithDateRange(DateTime.MinValue, DateTime.MaxValue)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .AddTimeFrame(publicationTimeFrame)
                .Build();

            var context = new ContextBuilder().Build();

            var gradeEquipmentService = A.Fake<IGradeEquipmentService>();
            A.CallTo(() => gradeEquipmentService.GetGradeEquipment(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).Returns(repoGradeEquipment);

            var gradeEquipmentFactory = new GradeEquipmentFactoryBuilder()
                .WithGradeEquipmentService(gradeEquipmentService)
                .Build();

            _gradeEquipmentItem = gradeEquipmentFactory.GetGradeEquipment(publication, context, Guid.Empty).GradeAccessories.Single();
        }

        protected override void Act()
        {
            _carInfos = _gradeEquipmentItem.StandardOn;
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
