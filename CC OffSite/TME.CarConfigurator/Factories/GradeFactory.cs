using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using RepoGrade = TME.CarConfigurator.Repository.Objects.Grade;

namespace TME.CarConfigurator.Factories
{
    public class GradeFactory : IGradeFactory
    {
        private readonly IGradeService _gradeService;
        private readonly IAssetFactory _assetFactory;
        
        public GradeFactory(IGradeService gradeService, IAssetFactory assetFactory)
        {
            if (gradeService == null) throw new ArgumentNullException("gradeService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");

            _gradeService = gradeService;
            _assetFactory = assetFactory;
        }

        public IReadOnlyList<IGrade> GetGrades(Publication publication, Context context)
        {
            var repoGrades = _gradeService.GetGrades(publication.ID, publication.GetCurrentTimeFrame().ID, context).ToList();
            var grades = new List<IGrade>();

            return repoGrades.Select(repoGrade => GetGrade(repoGrade, repoGrades, grades, publication, context)).ToArray();
        }

        IGrade GetGrade(RepoGrade repoGrade, IList<RepoGrade> repoGrades, IList<IGrade> grades, Publication publication, Context context)
        {
            var foundGrade = grades.SingleOrDefault(grd => grd.ID == repoGrade.ID);
            if (foundGrade != null)
                return foundGrade;

            var parentGrade = repoGrade.BasedUponGradeID == Guid.Empty ? null :
                              GetGrade(repoGrades.Single(grd => grd.ID == repoGrade.BasedUponGradeID), repoGrades, grades, publication, context);

            var grade = new Grade(repoGrade, publication, context, parentGrade, _assetFactory);
            grades.Add(grade);

            return grade;
        }
    }
}
