using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Publisher
{
    public class Publisher : IPublisher
    {
        readonly IPublicationPublisher _publicationPublisher;
        readonly IModelPublisher _modelPublisher;
        private readonly QueryServices.IModelService _modelService;
        readonly IBodyTypePublisher _bodyTypePublisher;
        readonly IEnginePublisher _enginePublisher;
        readonly ITransmissionPublisher _transmissionPublisher;
        readonly IWheelDrivePublisher _wheelDrivePublisher;
        readonly ISteeringPublisher _steeringPublisher;
        readonly IGradePublisher _gradePublisher;
        readonly ICarPublisher _carPublisher;
        readonly IAssetPublisher _assetPublisher;
        readonly ISubModelPublisher _subModelPublisher;
        readonly IGradeEquipmentPublisher _gradeEquipmentPublisher;
        private readonly IGradePackPublisher _gradePackPublisher;

        public Publisher(IPublicationPublisher publicationPublisher, IModelPublisher modelPublisher, IModelService modelService, IBodyTypePublisher bodyTypePublisher, IEnginePublisher enginePublisher, ITransmissionPublisher transmissionPublisher, IWheelDrivePublisher wheelDrivePublisher, ISteeringPublisher steeringPublisher, IGradePublisher gradePublisher, ICarPublisher carPublisher, IAssetPublisher assetPublisher, ISubModelPublisher subModelPublisher, IGradeEquipmentPublisher gradeEquipmentPublisher, IGradePackPublisher gradePackPublisher)
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
            if (gradeEquipmentPublisher == null) throw new ArgumentNullException("gradeEquipmentPublisher");
            if (gradePackPublisher == null) throw new ArgumentNullException("gradePackPublisher");

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
            _gradeEquipmentPublisher = gradeEquipmentPublisher;
            _gradePackPublisher = gradePackPublisher;
        }

        public async Task<Result> PublishAsync(IContext context)
        {
            var languageCodes = context.ContextData.Keys;

            var result = await PublishAsync(context, languageCodes);

            if (result is Failed) return result;

            return await ActivateAsync(context, languageCodes);
        }

        private async Task<Result> PublishAsync(IContext context, IEnumerable<string> languageCodes)
        {
            var results = await PublishPublicationForAllLanguages(context, languageCodes);

            return FindFirstFailure(results) ?? new Successfull();
        }

        private async Task<IEnumerable<Result>> PublishPublicationForAllLanguages(IContext context, IEnumerable<String> languages)
        {
            foreach (var language in languages)
            {
                var data = context.ContextData[language];
                var timeFrames = context.TimeFrames[language];
                CreateAndAddPublication(data, timeFrames);
            }

            var tasks = new List<Task<IEnumerable<Result>>>
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
                _gradeEquipmentPublisher.PublishAsync(context),
                _gradePackPublisher.PublishAsync(context),
                
                _assetPublisher.PublishAssetsAsync(context),
                _assetPublisher.PublishCarAssetsAsync(context)
            };

            var results = await Task.WhenAll(tasks);

            return results.SelectMany(xs => xs);
        }

        private static void CreateAndAddPublication(ContextData data, IReadOnlyList<TimeFrame> timeFrames)
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
                })
                    .ToList(),
                PublishedOn = DateTime.Now
            };

            data.Models.Single().Publications.Add(new PublicationInfo(data.Publication));
        }

        private static Result FindFirstFailure(IEnumerable<Result> results)
        {
            return results.FirstOrDefault(result => result is Failed);
        }

        private async Task<Result> ActivateAsync(IContext context, IEnumerable<String> languageCodes)
        {
            var languages = _modelService.GetModelsByLanguage(context.Brand, context.Country);

            foreach (var language in languageCodes.Select(languageCode => FindOrCreateLanguage(languages, languageCode)))
            {
                SetPublicationAsActiveForLanguage(context, language);
            }

            return await _modelPublisher.PublishModelsByLanguage(context, languages);
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
