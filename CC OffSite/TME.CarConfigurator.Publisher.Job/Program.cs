using System;

namespace TME.CarConfigurator.Publisher.Job
{
    class Program
    {
        static void Main()
        {
            var startTime = DateTime.Now;
            Console.WriteLine("Job started at {0}\n", startTime);
            
            var bootstrapper = new Bootstrapper();

            try
            {
                bootstrapper.GetJob().Run().Wait();
            }
            catch (AggregateException e)
            {
                foreach (var exception in e.InnerExceptions)
                {
                    Console.WriteLine(exception);
                }
            }

            bootstrapper.Dispose();

            Console.WriteLine("\nJob finished after {0}", DateTime.Now.Subtract(startTime).ToString("h'h 'm'm 's's'"));
        }
    }
}
