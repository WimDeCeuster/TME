using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class CarMapper : ICarMapper
    {
        public Car MapCar(Administration.Car car, Repository.Objects.BodyType bodyType, Repository.Objects.Engine engine)
        {
            // TODO: complete implementation
            return new Car
            {
                BodyType = bodyType,
                Engine = engine,
                ID = car.ID,

                Description = car.Translation.Description,
                FootNote = car.Translation.FootNote,
                Name = car.Translation.Name.DefaultIfEmpty(car.Name),
                ToolTip = car.Translation.ToolTip,
                SortIndex = car.Index,
                LocalCode = car.LocalCode.DefaultIfEmpty(car.BaseCode)
            };
        }
    }
}
