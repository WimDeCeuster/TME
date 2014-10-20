using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ICarFactory
    {
        IEnumerable<ICar> GetCars(Publication publication, Context context);
    }
}
