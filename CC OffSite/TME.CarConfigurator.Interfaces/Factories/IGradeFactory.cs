using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IGradeFactory
    {
        IEnumerable<IGrade> GetGrades(Publication publication, Context context);
        IGrade GetGrade(Publication publication, Context context, Guid id);
    }
}
