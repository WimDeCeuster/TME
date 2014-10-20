using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class GradeFactory : IGradeFactory
    {
        private readonly IGradeService _gradeService;

        public GradeFactory(IGradeService gradeService)
        {
            if (gradeService == null) throw new ArgumentNullException("gradeService");

            _gradeService = gradeService;
        }

        public IEnumerable<IGrade> GetGrades(Publication publication, Context context)
        {
            return _gradeService.GetGrades(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                 .Select(grade => new Grade(grade))
                                 .ToArray();
        }
    }
}
