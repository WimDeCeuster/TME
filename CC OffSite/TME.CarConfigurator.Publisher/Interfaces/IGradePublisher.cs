using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;


namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IGradePublisher
    {
        Task PublishGenerationGradesAsync(IContext context);
        Task  PublishSubModelGradesAsync(IContext context);
    }
}
