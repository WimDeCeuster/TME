using System;
using FakeItEasy;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;
using TME.CarConfigurator.Interfaces.Equipment;

namespace TME.CarConfigurator.Query.Tests.GivenAGradeEquipmentItem
{
    public class WhenAccessingItsNotAvailableOnForTheSecondTime : TestBase
    {
        IGradeEquipmentItem _gradeEquipmentItem;
        IEnumerable<Interfaces.ICarInfo> _secondCarInfos;
        IEnumerable<Interfaces.ICarInfo> _firstCarInfos;
        Repository.Objects.CarInfo _carInfo1;
        Repository.Objects.CarInfo _carInfo2;

        protected override void Arrange()
        {
            _carInfo1 = new CarInfoBuilder()
                .WithShortId(5)
                .Build();

            _carInfo2 = new CarInfoBuilder()
                .WithShortId(8)
                .Build();

            var repoGradeEquipmentItem = new GradeAccessoryBuilder()
                .WithNotAvailableOn(_carInfo1, _carInfo2)
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

            var gradeEquipmentService = A.Fake<IEquipmentService>();
            A.CallTo(() => gradeEquipmentService.GetGradeEquipment(A<Guid>._, A<Guid>._, A<Guid>._, A<Context>._)).Returns(repoGradeEquipment);

            var gradeEquipmentFactory = new EquipmentFactoryBuilder()
                .WithEquipmentService(gradeEquipmentService)
                .Build();

            _gradeEquipmentItem = gradeEquipmentFactory.GetGradeEquipment(publication, context, Guid.Empty).Accessories.Single();

            _firstCarInfos = _gradeEquipmentItem.NotAvailableOn;
        }

        protected override void Act()
        {
            _secondCarInfos = _gradeEquipmentItem.NotAvailableOn;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheCarInfos()
        {
            _secondCarInfos.Should().BeSameAs(_firstCarInfos);
        }

        [Fact]
        public void ThenItShouldHaveTheCarInfos()
        {
            _secondCarInfos.Count().Should().Be(2);

            _secondCarInfos.Should().Contain(carInfo => carInfo.ShortID == _carInfo1.ShortID);
            _secondCarInfos.Should().Contain(carInfo => carInfo.ShortID == _carInfo2.ShortID);
        }
    }
}
