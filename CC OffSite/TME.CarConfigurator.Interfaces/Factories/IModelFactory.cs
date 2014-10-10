using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IModelFactory
    {
        IEnumerable<IModel> GetModels(Context context);
    }
}