using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ICarMapper
    {
        Car MapCar(Administration.Car car, BodyType bodyType, Engine engine);
    }
}
