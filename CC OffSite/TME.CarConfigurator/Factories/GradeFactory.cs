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
        private Dictionary<Tuple<Guid, Context>, List<IGrade>> _grades = new Dictionary<Tuple<Guid,Context>,List<IGrade>>();

        public GradeFactory(IGradeService gradeService)
        {
            if (gradeService == null) throw new ArgumentNullException("gradeService");

            _gradeService = gradeService;
        }

        public IEnumerable<IGrade> GetGrades(Publication publication, Context context)
        {
            var key = Tuple.Create(publication.ID, context);
            if (!_grades.ContainsKey(key))
                _grades.Add(key, _gradeService.GetGrades(publication.ID, publication.GetCurrentTimeFrame().ID, context)
                                                .Select(grade => (IGrade)new Grade(grade, publication, context, this))
                                                .ToList());

            return _grades[key];
        }

        public IGrade GetGrade(Publication publication, Context context, Guid id)
        {
            if (id == Guid.Empty)
                return null;
            return GetGrades(publication, context).Single(grade => grade.ID == id);
        }
    }
}
