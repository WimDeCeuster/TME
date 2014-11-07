namespace TME.CarConfigurator.Publisher.Job
{
    class Program
    {
        static void Main()
        {
            var bootstrapper = new Bootstrapper();

            bootstrapper.GetJob().Run();

            bootstrapper.Dispose();
        }
    }
}
