using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;


namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IColourPublisher  
    {
        Task PublishGenerationColourCombinations(IContext context);
        Task PublishCarColourCombinations(IContext context);
    }
}