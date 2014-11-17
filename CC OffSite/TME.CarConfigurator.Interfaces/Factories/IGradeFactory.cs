using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface IGradeFactory
    {
        // ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<IGrade> GetGrades(Publication publication, Context context);
        IReadOnlyList<IGrade> GetSubModelGrades(Guid subModelID,Publication publication,Context context);
        IGrade GetCarGrade(Grade repoGrade, Guid carID, Publication publication, Context context);
        // ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}
