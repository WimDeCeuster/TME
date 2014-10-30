using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Equipment;
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
        private readonly IColourFactory _colourFactory;

        public GradeEquipmentFactory(IGradeEquipmentService gradeEquipmentService, IColourFactory colourFactory)
        {
            if (gradeEquipmentService == null) throw new ArgumentNullException("gradeEquipmentService");
            if (colourFactory == null) throw new ArgumentNullException("colourFactory");

            _gradeEquipmentService = gradeEquipmentService;
            _colourFactory = colourFactory;
        }

        public IGradeEquipment GetGradeEquipment(Publication publication, Context context, Guid gradeId)
        {
            var gradeEquipment = _gradeEquipmentService.GetGradeEquipment(publication.ID, publication.GetCurrentTimeFrame().ID, gradeId, context);

            return new GradeEquipment(
                gradeEquipment.Accessories.Select(accessory => GetGradeAccessory(accessory, publication, context)),
                gradeEquipment.Options.Select(option => GetGradeOption(option, gradeEquipment.Options, publication, context)));
        }

        IGradeAccessory GetGradeAccessory(RepoGradeAccessory accessory, Publication publication, Context context)
        {
            return new GradeAccessory(accessory, publication, context, _colourFactory);
        }

        // ReSharper disable once ParameterTypeCanBeEnumerable.Local => no, because that would cause a multiple enumeration for repoGrades...
        IGradeOption GetGradeOption(RepoGradeOption repoGradeOption, IReadOnlyList<RepoGradeOption> repoGrades, Publication publication, Context context)
        {
            var parentGradeOption = repoGradeOption.ParentOptionShortID == 0
                ? null
                : repoGrades.Single(grd => grd.ShortID == repoGradeOption.ParentOptionShortID);

            var parentOptionInfo = parentGradeOption == null ? null : new OptionInfo(parentGradeOption.ShortID, parentGradeOption.Name);

            return new GradeOption(repoGradeOption, parentOptionInfo, publication, context, _colourFactory);
        }
    }
}
