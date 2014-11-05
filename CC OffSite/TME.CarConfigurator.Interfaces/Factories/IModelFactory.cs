using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IModelFactory
    {
// ReSharper disable once ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<IModel> GetModels(Context context);
    }
}