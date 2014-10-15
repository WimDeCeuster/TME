using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using TME.CarConfigurator.S3.Shared.Result;

namespace TME.CarConfigurator.Publisher
{
    public class Publisher : IPublisher
    {
        readonly IPublicationPublisher _publicationPublisher;
        readonly IModelPublisher _modelPublisher;
        private readonly QueryServices.IModelService _modelService;
        readonly IBodyTypePublisher _bodyTypePublisher;
        readonly IEnginePublisher _enginePublisher;
        readonly ICarPublisher _carPublisher;
        readonly IAssetPublisher _assetPublisher;

        public Publisher(IPublicationPublisher publicationPublisher, 
            IModelPublisher modelPublisher, 
            QueryServices.IModelService modelService, 
            IBodyTypePublisher bodyTypePublisher,
            IEnginePublisher enginePublisher,
            ICarPublisher carPublisher,
            IAssetPublisher assetPublisher)
        {
            _publicationPublisher = publicationPublisher;
            _modelPublisher = modelPublisher;
            _modelService = modelService;
            _bodyTypePublisher = bodyTypePublisher;
            _enginePublisher = enginePublisher;
            _carPublisher = carPublisher;
            _assetPublisher = assetPublisher;
        }

        public async Task<Result> Publish(IContext context)
        {
            var languageCodes = context.ContextData.Keys;

            var result = await Publish(context, languageCodes);

            if (result is Failed) return result;

            return await Activate(context, languageCodes);
        }

        private async Task<Result> Publish(IContext context, IEnumerable<string> languageCodes)
        {
            var results = await Task.WhenAll(PublishPublicationForAllLanguages(context, languageCodes));

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
                _publicationPublisher.PublishPublications(context),
                _bodyTypePublisher.PublishGenerationBodyTypes(context),
                _enginePublisher.PublishGenerationEngines(context),
                _carPublisher.PublishGenerationCars(context),
                _assetPublisher.PublishAssets(context)
            };

            var results = await Task.WhenAll(tasks);

            return results.SelectMany(xs => xs.ToList());
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
        private static Result FindFirstFailure(IEnumerable<IEnumerable<Result>> results)
        {
            return results.SelectMany(xs => xs.ToList()).FirstOrDefault(result => result is Failed);
        }

        private Task<Result> Activate(IContext context, IEnumerable<String> languageCodes)
        {
            var languages = _modelService.GetModelsByLanguage(context.Brand, context.Country);

            foreach (var language in languageCodes.Select(languageCode => FindOrCreateLanguage(languages, languageCode)))
            {
                SetPublicationAsActiveForLanguage(context, language);
            }

            return _modelPublisher.PublishModelsByLanguage(context, languages);
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
