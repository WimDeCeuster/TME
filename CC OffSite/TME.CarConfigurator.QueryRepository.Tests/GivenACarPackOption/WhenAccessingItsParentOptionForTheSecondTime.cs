using FakeItEasy;
using FluentAssertions;
using System;
using System.Linq;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Equipment;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarPackOption
{
    public class WhenAccessingItsOptionInfoTheSecondTime : TestBase
    {
        ICarPackOption _carPackOption;
        IOptionInfo _firstOptionInfo;
        IOptionInfo _secondOptionInfo;
        OptionInfo _repoOptionInfo;

        protected override void Arrange()
        {
            _repoOptionInfo = new OptionInfoBuilder()
                .WithId(5)
                .Build();

            var repoCarPackEquipmentItem = new CarPackOptionBuilder()
                .WithParentOption(_repoOptionInfo)
                .Build();

            var repoCarPackEquipment = new CarPackEquipmentBuilder()
                .WithOptions(repoCarPackEquipmentItem)
                .Build();

            var repoCarPack = new CarPackBuilder()
                .WithCarPackEquipment(repoCarPackEquipment)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            var context = new ContextBuilder().Build();

            var packService = A.Fake<IPackService>();
            A.CallTo(() => packService.GetCarPacks(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new[] { repoCarPack });

            var equipmentFactory = new EquipmentFactoryBuilder().Build();

            var packFactory = new PackFactoryBuilder()
                .WithPackService(packService)
                .WithEquipmentFactory(equipmentFactory)
                .Build();

            var carPack = packFactory.GetCarPacks(publication, context, Guid.Empty).Single();
            _carPackOption = carPack.Equipment.Options.Single();

            _firstOptionInfo = _carPackOption.ParentOption;
        }

        protected override void Act()
        {
            _secondOptionInfo = _carPackOption.ParentOption;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheOptionInfo()
        {
            _secondOptionInfo.Should().Be(_firstOptionInfo);
        }

        [Fact]
        public void ThenTheOptionInfoShouldBeCorrect()
        {
            _secondOptionInfo.ShortID.Should().Be(_repoOptionInfo.ShortID);
        }
    }
}
