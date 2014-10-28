using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Equipment;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using RepoGradeEquipment = TME.CarConfigurator.Repository.Objects.Equipment.GradeEquipment;
using RepoGradeAccessory = TME.CarConfigurator.Repository.Objects.Equipment.GradeAccessory;
using RepoGradeOption = TME.CarConfigurator.Repository.Objects.Equipment.GradeOption;

namespace TME.CarConfigurator.Factories
{
    public class GradeEquipmentFactory : IGradeEquipmentFactory
    {
        private readonly IGradeEquipmentService _gradeEquipmentService;
        
        public GradeEquipmentFactory(IGradeEquipmentService gradeEquipmentService)
        {
            if (gradeEquipmentService == null) throw new ArgumentNullException("gradeEquipmentService");

            _gradeEquipmentService = gradeEquipmentService;
        }

        public IGradeEquipment GetGradeEquipment(Publication publication, Context context, Guid gradeId)
        {
            var gradeEquipment = _gradeEquipmentService.GetGradeEquipment(publication.ID, publication.GetCurrentTimeFrame().ID, gradeId, context);

            return new GradeEquipment(
                gradeEquipment.Accessories.Select(GetGradeAccessory),
                gradeEquipment.Options.Select(option => GetGradeOption(option, gradeEquipment.Options)));
        }

        IGradeAccessory GetGradeAccessory(RepoGradeAccessory accessory)
        {
            return new GradeAccessory(accessory);
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local => no, because that would cause a multiple enumeration for repoGrades...
        IGradeOption GetGradeOption(RepoGradeOption repoGradeOption, IReadOnlyList<RepoGradeOption> repoGrades)
        {
            var parentGradeOption = repoGradeOption.ParentOptionShortID == 0
                ? null
                : repoGrades.Single(grd => grd.ShortID == repoGradeOption.ParentOptionShortID);

            var parentOptionInfo = parentGradeOption == null ? null : new OptionInfo(parentGradeOption.ShortID, parentGradeOption.Name);

            return new GradeOption(repoGradeOption, parentOptionInfo);
        }
    }
}
