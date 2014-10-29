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
        private readonly IGradeEquipmentFactory _gradeEquipmentFactory;
        private readonly IPackFactory _packFactory;

        public GradeFactory(IGradeService gradeService, IAssetFactory assetFactory, IGradeEquipmentFactory gradeEquipmentFactory, IPackFactory packFactory)
        {
            if (gradeService == null) throw new ArgumentNullException("gradeService");
            if (assetFactory == null) throw new ArgumentNullException("assetFactory");
            if (gradeEquipmentFactory == null) throw new ArgumentNullException("gradeEquipmentFactory");
            if (packFactory == null) throw new ArgumentNullException("packFactory");

            _gradeService = gradeService;
            _assetFactory = assetFactory;
            _gradeEquipmentFactory = gradeEquipmentFactory;
            _packFactory = packFactory;
        }

        public IReadOnlyList<IGrade> GetGrades(Publication publication, Context context)
        {
            var repoGrades = _gradeService.GetGrades(publication.ID, publication.GetCurrentTimeFrame().ID, context).ToList();
            var grades = new List<IGrade>();

            return repoGrades.Select(repoGrade => GetGrade(repoGrade, repoGrades, grades, publication, context)).ToArray();
        }

        public IReadOnlyList<IGrade> GetSubModelGrades(Publication publication, Context context, Repository.Objects.SubModel subModel)
        {
            return GetGrades(publication, context)
                    .Where(mainGrade => subModel.Grades.Any(repositoryGrade => repositoryGrade.ID == mainGrade.ID))
                    .ToArray();
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local => no, because that would cause a multiple enumeration for repoGrades...
        IGrade GetGrade(RepoGrade repoGrade, IList<RepoGrade> repoGrades, ICollection<IGrade> grades, Publication publication, Context context)
        {
            var foundGrade = grades.SingleOrDefault(grd => grd.ID == repoGrade.ID);
            if (foundGrade != null)
                return foundGrade;

            var parentGrade = repoGrade.BasedUponGradeID == Guid.Empty 
                ? null 
                : GetGrade(repoGrades.Single(grd => grd.ID == repoGrade.BasedUponGradeID), repoGrades, grades, publication, context);

            var grade = new Grade(repoGrade, publication, context, parentGrade, _assetFactory, _gradeEquipmentFactory, _packFactory);
            grades.Add(grade);

            return grade;
        }
    }
}
