using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TME.CarConfigurator.Publisher
{
    public interface IContextFactory
    {
        IContext Get(Guid generationId, PublicationDataSubset dataSubset);
    }

    public class ContextFactory : IContextFactory
    {
        public IContext Get(Guid generationId, PublicationDataSubset dataSubset)
        {
            throw new NotImplementedException();
        }
    }
}
