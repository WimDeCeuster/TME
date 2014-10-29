using System.Collections.Generic;
using TME.CarConfigurator.Administration;
using TME.CarConfigurator.Repository.Objects;
using Car = TME.CarConfigurator.Repository.Objects.Car;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IGradeMapper
    {
        Grade MapGenerationGrade(Administration.ModelGenerationGrade grade, IEnumerable<Car> cars);
        Grade MapSubModelGrade(ModelGenerationGrade grade,ModelGenerationSubModel subModel);
    }
}
