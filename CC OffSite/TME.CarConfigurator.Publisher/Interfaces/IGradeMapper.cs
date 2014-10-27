using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;
using Car = TME.CarConfigurator.Repository.Objects.Car;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IGradeMapper
    {
        Grade MapGrade(Administration.ModelGenerationGrade grade, IEnumerable<Car> cars);
    }
}
