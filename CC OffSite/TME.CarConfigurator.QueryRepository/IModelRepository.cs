using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository
{
    public interface IModelRepository
    {
        IEnumerable<Model> GetModels(Repository.Objects.Context.Base context);
    }
}

