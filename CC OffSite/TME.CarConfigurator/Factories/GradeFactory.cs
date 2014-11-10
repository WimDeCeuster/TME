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
        private readonly IEquipmentFactory _gradeEquipmentFactory;
        private readonly IPackFactory _packFactory;
        private IList<RepoGrade> _repoGenerationGrades;


        public GradeFactory(IGradeService gradeService, IAssetFactory assetFactory, IEquipmentFactory gradeEquipmentFactory, IPackFactory packFactory)
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
            var repoGrades = GetRepoGrades(publication, context);
            var grades = new List<IGrade>();

            return repoGrades.Select(repoGrade => GetGrade(repoGrade, grades, publication, context)).ToArray();
        }

        public IReadOnlyList<IGrade> GetSubModelGrades(Guid subModelID, Publication publication, Context context)
        {
            var repoSubModelGrades = _gradeService.GetSubModelGrades(publication.ID, publication.GetCurrentTimeFrame().ID, subModelID, context).ToList();
            var grades = new List<IGrade>();

            return repoSubModelGrades.Select(repoGrade => GetSubModelGrade(repoGrade, grades, publication, context, subModelID)).ToArray();
        }

        public IGrade GetCarGrade(RepoGrade repoGrade, Guid carID, Publication publication, Context context)
        {
            return new CarGrade(repoGrade, publication, carID, context, _assetFactory, _gradeEquipmentFactory,
                _packFactory);
        }

        private IEnumerable<RepoGrade> GetRepoGrades(Publication publication, Context context)
        {
            return _repoGenerationGrades = _repoGenerationGrades ??_gradeService.GetGrades(publication.ID, publication.GetCurrentTimeFrame().ID, context).ToList();
        }

        private IGrade GetSubModelGrade(RepoGrade repoGrade, ICollection<IGrade> grades, Publication publication, Context context, Guid subModelID)
        {
            var foundGrade = grades.SingleOrDefault(grd => grd.ID == repoGrade.ID);
            if (foundGrade != null)
                return foundGrade;

            var subModelGrade = new SubModelGrade(repoGrade, publication, context, subModelID, _assetFactory, _gradeEquipmentFactory, _packFactory);
            grades.Add(subModelGrade);
            return subModelGrade;
        }

        IGrade GetGrade(RepoGrade repoGrade, ICollection<IGrade> grades, Publication publication, Context context)
        {
            var foundGrade = grades.SingleOrDefault(grd => grd.ID == repoGrade.ID);
            if (foundGrade != null)
                return foundGrade;

            var grade = new Grade(repoGrade, publication, context, _assetFactory, _gradeEquipmentFactory, _packFactory);
            grades.Add(grade);

            return grade;
        }

    }
}
