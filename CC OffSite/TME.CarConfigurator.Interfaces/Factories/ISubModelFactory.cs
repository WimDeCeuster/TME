using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ISubModelFactory
    {
        IReadOnlyList<ISubModel> GetSubModels(Publication publication, Context context); 
    }
}