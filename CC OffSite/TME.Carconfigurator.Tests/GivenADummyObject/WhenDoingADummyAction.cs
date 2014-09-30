using FluentAssertions;
using Xunit;

namespace TME.Carconfigurator.Tests.GivenADummyObject
{
    public class WhenDoingADummyAction
    {
        private readonly int _numberOfTestsExecuted;

        public WhenDoingADummyAction()
        {
            _numberOfTestsExecuted += 1;
        }

        [Fact]
        public void ThenItShouldDoADummyAssert()
        {
            _numberOfTestsExecuted.Should().Be(1);
        }
    }
}