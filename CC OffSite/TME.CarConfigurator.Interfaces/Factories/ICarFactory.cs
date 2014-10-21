using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ICarFactory
    {
        IReadOnlyList<ICar> GetCars(Publication publication, Context context);
    }
}
