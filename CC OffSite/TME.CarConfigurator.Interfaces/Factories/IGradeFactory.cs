using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IGradeFactory
    {
        IReadOnlyList<IGrade> GetGrades(Publication publication, Context context);
        IReadOnlyList<IGrade> GetSubModelGrades(Publication publication, Context context, SubModel subModel);
    }
}
