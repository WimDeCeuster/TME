using FluentAssertions;
using TME.Carconfigurator.Tests.Base;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenADummyObject
{
    public class WhenDoingADummyAction : TestBase
    {
        private int _numberOfTestsExecuted;

        protected override void Arrange()
        {
            _numberOfTestsExecuted = 5;
        }

        protected override void Act()
        {
            _numberOfTestsExecuted += 1;
        }

        [Fact]
        public void ThenItShouldDoADummyAssert()
        {
            _numberOfTestsExecuted.Should().Be(6);
        }
    }
}