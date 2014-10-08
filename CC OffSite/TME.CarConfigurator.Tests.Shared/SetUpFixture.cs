using System;

namespace TME.CarConfigurator.Tests.Shared
{
    public class SetUpFixture
    {
        public void Setup(Action task)
        {
            task();
        }
    }
}