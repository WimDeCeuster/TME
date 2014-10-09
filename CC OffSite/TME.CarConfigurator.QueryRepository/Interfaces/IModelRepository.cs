using System.Collections.Generic;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Interfaces
{
    public interface IModelRepository
    {
        IEnumerable<Model> GetModels(Context context);
    }
}

