using FluentAssertions;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Tests.Shared;
using Xunit;

namespace TME.CarConfigurator.Query.Tests.GivenAGradePack
{
    public class WhenAccessingItsLabels : TestBase
    {

        protected override void Arrange()
        {
            // irrelevant
        }

        protected override void Act()
        {
            // irrelevant
        }

        [Fact]
        public void ThenItShouldAccessThemThroughBaseObject()
        {
            var typeGetMethod = typeof(GradePack).GetProperty("Labels").GetGetMethod(false);

            typeGetMethod.DeclaringType.Should().Be(typeof(BaseObject<Repository.Objects.Packs.GradePack>));
        }
    }
}