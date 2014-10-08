using FakeItEasy;
using System;
using System.Linq;
using System.Collections.Generic;
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

        protected Func<ArgumentCollection, Boolean> ArgumentMatchesList<T>(params T[] items)
        {
            return args =>
            {
                var argumentItems = (IEnumerable<T>)args[0];
                return argumentItems.Count() == items.Length &&
                       argumentItems.Zip(items, (item1, item2) => Object.Equals(item1, item2))
                                    .All(x => x);
            };
        }

    }
}