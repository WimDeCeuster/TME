using System;
using TME.CarConfigurator.QueryRepository;

namespace TME.CarConfigurator.RepositoryFacades
{
    public class CarRepositoryFacade
    {

        public ICarRepository CarRepository { get; private set; }

        public CarRepositoryFacade(ICarRepository carRepository)
        {
            if (carRepository == null) throw new ArgumentNullException("carRepository");

            CarRepository = carRepository;
        }
    }
}
