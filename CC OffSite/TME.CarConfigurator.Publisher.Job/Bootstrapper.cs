using System;
using Spring.Context;
using Spring.Context.Support;

namespace TME.CarConfigurator.Publisher.Job
{
    internal class Bootstrapper : IDisposable
    {
        private IApplicationContext _springContext;

        public Bootstrapper()
        {
            _springContext = ContextRegistry.GetContext();
        }

        public void Dispose()
        {
            if (_springContext == null) 
                return;

            _springContext.Dispose();

            _springContext = null;
        }

        public IJob GetJob()
        {
            return (IJob) _springContext.GetObject("Job");
        }
    }
}