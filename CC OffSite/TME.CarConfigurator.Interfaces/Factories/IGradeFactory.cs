using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IGradeFactory
    {
        IReadOnlyList<IGrade> GetGrades(Publication publication, Context context);
        IGrade GetSubModelGrade(Grade grade,Guid subModelID,Publication publication,Context context);
    }
}
