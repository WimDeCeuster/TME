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
    public class WhenAccessingItsAvailableForUpholsteriesForTheSecondTime : TestBase
    {
        ICarPack _carPack;
        IEnumerable<IUpholsteryInfo> _secondUpholsteryInfos;
        IEnumerable<IUpholsteryInfo> _firstUpholsteryInfos;
        Repository.Objects.Colours.UpholsteryInfo _upholsteryInfo1;
        Repository.Objects.Colours.UpholsteryInfo _upholsteryInfo2;

        protected override void Arrange()
        {
            _upholsteryInfo1 = new UpholsteryInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            _upholsteryInfo2 = new UpholsteryInfoBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoCarPack = new CarPackBuilder()
                .WithAvailableForUpholsteries(_upholsteryInfo1, _upholsteryInfo2)
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

            _firstUpholsteryInfos = _carPack.AvailableForUpholsteries;
        }

        protected override void Act()
        {
            _secondUpholsteryInfos = _carPack.AvailableForUpholsteries;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheLabels()
        {
            _secondUpholsteryInfos.Should().BeSameAs(_firstUpholsteryInfos);
        }

        [Fact]
        public void ThenItShouldHaveTheLabels()
        {
            _secondUpholsteryInfos.Count().Should().Be(2);

            _secondUpholsteryInfos.Should().Contain(upholsteryInfo => upholsteryInfo.ID == _upholsteryInfo1.ID);
            _secondUpholsteryInfos.Should().Contain(upholsteryInfo => upholsteryInfo.ID == _upholsteryInfo2.ID);
        }
    }
}
