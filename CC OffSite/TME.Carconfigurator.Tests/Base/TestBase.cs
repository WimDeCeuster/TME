namespace TME.Carconfigurator.Tests.Base
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