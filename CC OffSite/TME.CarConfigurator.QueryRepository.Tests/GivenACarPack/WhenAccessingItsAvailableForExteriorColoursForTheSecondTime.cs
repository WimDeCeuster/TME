using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Colours;
using TME.CarConfigurator.Interfaces.Packs;
using TME.CarConfigurator.Query.Tests.TestBuilders;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenACarPack
{
    public class WhenAccessingItsAvailableForExteriorColoursForTheSecondTime : TestBase
    {
        ICarPack _carPack;
        IEnumerable<IExteriorColourInfo> _secondExteriorColourInfos;
        IEnumerable<IExteriorColourInfo> _firstExteriorColourInfos;
        Repository.Objects.Colours.ExteriorColourInfo _exteriorColourInfo1;
        Repository.Objects.Colours.ExteriorColourInfo _exteriorColourInfo2;

        protected override void Arrange()
        {
            _exteriorColourInfo1 = new ExteriorColourInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _exteriorColourInfo2 = new ExteriorColourInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarPack = new CarPackBuilder()
                .WithAvailableForExteriorColours(_exteriorColourInfo1, _exteriorColourInfo2)
                .Build();

            var publication = new PublicationBuilder()
                .WithID(Guid.NewGuid())
                .Build();

            var context = new ContextBuilder().Build();

            var packService = A.Fake<IPackService>();
            A.CallTo(() => packService.GetCarPacks(A<Guid>._, A<Guid>._, A<Context>._)).Returns(new[] { repoCarPack });

            var packFactory = new PackFactoryBuilder()
                .WithPackService(packService)
                .Build();

            _carPack = packFactory.GetCarPacks(publication, context, Guid.Empty).Single();

            _firstExteriorColourInfos = _carPack.AvailableForExteriorColours;
        }

        protected override void Act()
        {
            _secondExteriorColourInfos = _carPack.AvailableForExteriorColours;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheItems()
        {
            _secondExteriorColourInfos.Should().BeSameAs(_firstExteriorColourInfos);
        }

        [Fact]
        public void ThenItShouldHaveTheItems()
        {
            _secondExteriorColourInfos.Count().Should().Be(2);

            _secondExteriorColourInfos.Should().Contain(exteriorColourInfo => exteriorColourInfo.ID == _exteriorColourInfo1.ID);
            _secondExteriorColourInfos.Should().Contain(exteriorColourInfo => exteriorColourInfo.ID == _exteriorColourInfo2.ID);
        }
    }
}
