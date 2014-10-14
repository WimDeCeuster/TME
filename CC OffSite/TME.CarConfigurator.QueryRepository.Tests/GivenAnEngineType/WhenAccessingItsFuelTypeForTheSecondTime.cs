using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Tests.Shared;
using TME.CarConfigurator.Tests.Shared.TestBuilders;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAnEngineType
{
    public class WhenAccessingItsFuelTypeForTheSecondTime : TestBase
    {
        IEngineType _engineType;
        IFuelType _secondFuelType;
        Repository.Objects.FuelType _repoFuelType;
        IFuelType _firstFuelType;

        protected override void Arrange()
        {
            _repoFuelType = new FuelTypeBuilder()
                .WithId(Guid.NewGuid())
                .Build();

            var repoEngineType = new EngineTypeBuilder()
                .WithFuelType(_repoFuelType)
                .Build();

            _engineType = new EngineType(repoEngineType);

            _firstFuelType = _engineType.FuelType;
        }

        protected override void Act()
        {
            _secondFuelType = _engineType.FuelType;
        }

        [Fact]
        public void ThenItShouldNotRecalculateTheFuelType()
        {
            _secondFuelType.Should().Be(_firstFuelType);
        }

        [Fact]
        public void ThenItShouldHaveTheFuelType()
        {
            _secondFuelType.ID.Should().Be(_repoFuelType.ID);
        }
    }
}
