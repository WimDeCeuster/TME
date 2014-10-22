using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IModelFactory
    {
        IReadOnlyList<IModel> GetModels(Context context);
    }
}