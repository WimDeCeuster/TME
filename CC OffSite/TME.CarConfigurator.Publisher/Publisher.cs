using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Publisher
{
    public class Publisher : IPublisher
    {
        private readonly IPublicationPublisher _publicationPublisher;
        private readonly IModelPublisher _modelPublisher;
        private readonly IModelService _modelService;
        private readonly IBodyTypePublisher _bodyTypePublisher;
        private readonly IEnginePublisher _enginePublisher;
        private readonly ITransmissionPublisher _transmissionPublisher;
        private readonly IWheelDrivePublisher _wheelDrivePublisher;
        private readonly ISteeringPublisher _steeringPublisher;
        private readonly IGradePublisher _gradePublisher;
        private readonly ICarPublisher _carPublisher;
        private readonly IAssetPublisher _assetPublisher;
        private readonly ISubModelPublisher _subModelPublisher;
        private readonly IEquipmentPublisher _equipmentPublisher;
        private readonly ISpecificationsPublisher _specificationsPublisher;
        private readonly IPackPublisher _packPublisher;
        private readonly IColourPublisher _colourCombinationPublisher;
        private readonly ICarPartPublisher _carPartPublisher;

        public Publisher(IPublicationPublisher publicationPublisher,
            IModelPublisher modelPublisher,
            IModelService modelService,
            IBodyTypePublisher bodyTypePublisher,
            IEnginePublisher enginePublisher,
            ITransmissionPublisher transmissionPublisher,
            IWheelDrivePublisher wheelDrivePublisher,
            ISteeringPublisher steeringPublisher,
            IGradePublisher gradePublisher,
            ICarPublisher carPublisher,
            IAssetPublisher assetPublisher,
            ISubModelPublisher subModelPublisher,
            IEquipmentPublisher equipmentPublisher,
            ISpecificationsPublisher specificationsPublisher,
            IPackPublisher packPublisher,
            IColourPublisher colourCombinationPublisher,
            ICarPartPublisher carPartPublisher
            )
        {
            if (publicationPublisher == null) throw new ArgumentNullException("publicationPublisher");
            if (modelPublisher == null) throw new ArgumentNullException("modelPublisher");
            if (modelService == null) throw new ArgumentNullException("modelService");
            if (bodyTypePublisher == null) throw new ArgumentNullException("bodyTypePublisher");
            if (enginePublisher == null) throw new ArgumentNullException("enginePublisher");
            if (transmissionPublisher == null) throw new ArgumentNullException("transmissionPublisher");
            if (wheelDrivePublisher == null) throw new ArgumentNullException("wheelDrivePublisher");
            if (steeringPublisher == null) throw new ArgumentNullException("steeringPublisher");
            if (gradePublisher == null) throw new ArgumentNullException("gradePublisher");
            if (carPublisher == null) throw new ArgumentNullException("carPublisher");
            if (assetPublisher == null) throw new ArgumentNullException("assetPublisher");
            if (subModelPublisher == null) throw new ArgumentNullException("subModelPublisher");
            if (equipmentPublisher == null) throw new ArgumentNullException("equipmentPublisher");
            if (specificationsPublisher == null) throw new ArgumentNullException("specificationsPublisher");
            if (packPublisher == null) throw new ArgumentNullException("packPublisher");
            if (colourCombinationPublisher == null) throw new ArgumentNullException("colourCombinationPublisher");
            if (carPartPublisher == null) throw new ArgumentNullException("carPartPublisher");

            
            _publicationPublisher = publicationPublisher;
            _modelPublisher = modelPublisher;
            _modelService = modelService;
            _bodyTypePublisher = bodyTypePublisher;
            _enginePublisher = enginePublisher;
            _transmissionPublisher = transmissionPublisher;
            _wheelDrivePublisher = wheelDrivePublisher;
            _steeringPublisher = steeringPublisher;
            _gradePublisher = gradePublisher;
            _carPublisher = carPublisher;
            _assetPublisher = assetPublisher;
            _subModelPublisher = subModelPublisher;
            _equipmentPublisher = equipmentPublisher;
            _specificationsPublisher = specificationsPublisher;
            _packPublisher = packPublisher;
            _colourCombinationPublisher = colourCombinationPublisher;
            _carPartPublisher = carPartPublisher;
        }

        public async Task PublishAsync(IContext context)
        {
            var languageCodes = context.ContextData.Keys;

            await PublishAsync(context, languageCodes);

            await ActivateAsync(context, languageCodes);
        }

        private async Task PublishAsync(IContext context, IEnumerable<string> languageCodes)
        {
            foreach (var lanuageCode in languageCodes)
            {
                var data = context.ContextData[lanuageCode];
                var timeFrames = context.TimeFrames[lanuageCode];
                CreateAndAddPublication(data, timeFrames, context.PublishedBy);
            }
            var tasks = new List<Task>
            {
                _publicationPublisher.PublishPublicationsAsync(context),
                _bodyTypePublisher.PublishGenerationBodyTypesAsync(context),
                _enginePublisher.PublishGenerationEnginesAsync(context),
                _transmissionPublisher.PublishGenerationTransmissionsAsync(context),
                _wheelDrivePublisher.PublishGenerationWheelDrivesAsync(context),
                _steeringPublisher.PublishGenerationSteeringsAsync(context),
                _gradePublisher.PublishGenerationGradesAsync(context),
                _subModelPublisher.PublishGenerationSubModelsAsync(context),
                _carPublisher.PublishGenerationCarsAsync(context),
                _colourCombinationPublisher.PublishGenerationColourCombinations(context),
                _colourCombinationPublisher.PublishCarColourCombinations(context),
                _gradePublisher.PublishSubModelGradesAsync(context),
                _equipmentPublisher.PublishAsync(context),
                _equipmentPublisher.PublishCategoriesAsync(context),
                _equipmentPublisher.PublishSubModelGradeEquipmentAsync(context),
                _specificationsPublisher.PublishCategoriesAsync(context),
                _packPublisher.PublishGradePacksAsync(context),
                _packPublisher.PublishCarPacksAsync(context),
                _packPublisher.PublishSubModelGradePacksAsync(context),
                _carPartPublisher.PublishCarPartsAsync(context),
                _equipmentPublisher.PublishCarEquipmentAsync(context),
                _specificationsPublisher.PublishCarTechnicalSpecificationsAsync(context),
                _assetPublisher.PublishAsync(context)
            };

            await Task.WhenAll(tasks);
        }

        private static void CreateAndAddPublication(ContextData data, IReadOnlyList<TimeFrame> timeFrames, string publishedBy)
        {
            data.Publication = new Publication
             {
                 ID = Guid.NewGuid(),
                 Generation = data.Generations.Single(),
                 LineOffFrom = timeFrames.First().From,
                 LineOffTo = timeFrames.Last().Until,
                 TimeFrames = timeFrames.Select(timeFrame => new PublicationTimeFrame
                 {
                     ID = timeFrame.ID,
                     LineOffFrom = timeFrame.From,
                     LineOffTo = timeFrame.Until
                 }).ToList(),
                 PublishedOn = DateTime.Now,
                 PublishedBy = publishedBy
             };

            data.Models.Single().Publications.Add(new PublicationInfo(data.Publication));
        }

        private async Task ActivateAsync(IContext context, IEnumerable<String> languageCodes)
        {
            var languages = _modelService.GetModelsByLanguage(context.Brand, context.Country);

            foreach (var language in languageCodes.Select(languageCode => FindOrCreateLanguage(languages, languageCode)))
            {
                SetPublicationAsActiveForLanguage(context, language);
            }

            await _modelPublisher.PublishModelsByLanguage(context, languages);
        }

        private static Language FindOrCreateLanguage(Languages languages, String languageCode)
        {
            var language = languages.SingleOrDefault(l => l.Code.Equals(languageCode, StringComparison.InvariantCultureIgnoreCase));

            if (language != null)
                return language;

            language = new Language(languageCode);

            languages.Add(language);

            return language;
        }

        private static void SetPublicationAsActiveForLanguage(IContext context, Language language)
        {
            var contextModel = context.ContextData[language.Code].Models.Single();
            var s3Model = language.Models.SingleOrDefault(m => m.ID == contextModel.ID);

            if (s3Model == null)
            {
                language.Models.Add(contextModel);
            }
            else
            {
                s3Model.Name = contextModel.Name;
                s3Model.InternalCode = contextModel.InternalCode;
                s3Model.LocalCode = contextModel.LocalCode;
                s3Model.Description = contextModel.Description;
                s3Model.FootNote = contextModel.FootNote;
                s3Model.ToolTip = contextModel.ToolTip;
                s3Model.SortIndex = contextModel.SortIndex;
                s3Model.Labels = contextModel.Labels;
                s3Model.Publications.Single(e => e.State == PublicationState.Activated).State = PublicationState.ToBeDeleted;
                s3Model.Publications.Add(contextModel.Publications.Single());
            }

            language.Models = language.Models.OrderBy(model => model.SortIndex)
                                                    .ThenBy(model => model.Name)
                                                    .ToList();
        }
    }
}
