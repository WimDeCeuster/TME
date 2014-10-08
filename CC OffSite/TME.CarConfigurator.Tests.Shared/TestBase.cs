using System;
using Xunit;

namespace TME.CarConfigurator.Tests.Shared
{
    public abstract class TestBase : IUseFixture<SetUpFixture>, IDisposable
    {
        public void SetFixture(SetUpFixture data)
        {
            data.Setup(Setup);
            ExecuteTest();
        }

        protected virtual void Setup()
        {
        }

        protected virtual void ExecuteTest()
        {
            Arrange();
            Act();
        }

        protected abstract void Arrange();
        protected abstract void Act();

        public virtual void Dispose()
        {
            CleanUp();
        }

        protected virtual void CleanUp()
        {

        }
    }
}