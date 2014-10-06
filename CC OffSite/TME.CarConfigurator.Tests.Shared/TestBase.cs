namespace TME.CarConfigurator.Tests.Shared
{
    public abstract class TestBase
    {
        protected TestBase()
        {
            Arrange();
            Act();
        }

        protected abstract void Arrange();

        protected abstract void Act();

        ~TestBase()
        {
            CleanUp();
        }

        protected virtual void CleanUp()
        {
            
        }
    }
}