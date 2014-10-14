using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
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
        readonly IModelPublisher _putModelPublisher            ;
        private readonly QueryServices.IModelService _getModelService;
        readonly IBodyTypePublisher _bodyTypePublisher;
        readonly IEnginePublisher _enginePublisher;

        public Publisher(IPublicationPublisher publicationPublisher, IModelPublisher putModelPublisher, QueryServices.IModelService getModelService,
                         IBodyTypePublisher bodyTypePublisher, IEnginePublisher enginePublisher)
        {
            _publicationPublisher = publicationPublisher;
            _putModelPublisher = putModelPublisher;
            _getModelService = getModelService;
            _bodyTypePublisher = bodyTypePublisher;
            _enginePublisher = enginePublisher;
        }

        public async Task<Result> Publish(IContext context)
        {
            var languages = context.ContextData.Keys;
            var publishTasks = PublishPublicationForAllLanguages(context, languages);

            var results = await Task.WhenAll(publishTasks);

            var failure = results.SelectMany(xs => xs).FirstOrDefault(result => result is Failed);
            if (failure != null)
                return failure;

            var s3ModelsOverview = ActivatePublicationForAllLanguages(context, languages);
            return await _putModelPublisher.PublishModelsByLanguage(context, s3ModelsOverview);
        }

        private Languages ActivatePublicationForAllLanguages(IContext context, IEnumerable<String> languages)
        {
            var s3ModelsOverview = _getModelService.GetModelsByLanguage(context.Brand, context.Country);

            foreach (var language in languages)
            {
                ActivatePublicationForLanguage(context, s3ModelsOverview, language);
            }

            return s3ModelsOverview;
        }

        private static void ActivatePublicationForLanguage(IContext context, Languages s3ModelsOverview, String language)
        {
            var s3Language = GetS3Language(s3ModelsOverview, language);

            var s3Models = s3Language.Models;
            var contextModel = context.ContextData[language].Models.Single();
            var s3Model = s3Models.SingleOrDefault(m => m.ID == contextModel.ID);

            if (s3Model == null)
            {
                s3Models.Add(contextModel);
            }
            else { 
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

            s3Language.Models = s3Language.Models.OrderBy(model => model.SortIndex)
                                                    .ThenBy(model => model.Name)
                                                    .ToList();
        }

        private async Task<IEnumerable<Result>> PublishPublicationForAllLanguages(IContext context, IEnumerable<String> languages)
        {
            foreach (var language in languages)
            {
                var data = context.ContextData[language];
                var timeFrames = context.TimeFrames[language];
                CreateNewPublication(data, timeFrames);
            }

            var tasks = new List<Task<IEnumerable<Result>>>();

            tasks.Add(PublishPublication(context));
            tasks.Add(PublishGenerationBodyTypes(context));
            tasks.Add(PublishGenerationEngines(context));

            var results = await Task.WhenAll(tasks);

            return results.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PublishPublication(IContext context)
        {
            return await _publicationPublisher.PublishPublications(context);
        }

        private Publication CreateNewPublication(ContextData data, IReadOnlyList<TimeFrame> timeFrames)
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

            return data.Publication;
        }

        Task<IEnumerable<Result>> PublishGenerationBodyTypes(IContext context)
        {
            return _bodyTypePublisher.PublishGenerationBodyTypes(context);
        }

        Task<IEnumerable<Result>> PublishGenerationEngines(IContext context)
        {
            return _enginePublisher.PublishGenerationEngines(context);
        }

        private static Language GetS3Language(Languages s3ModelsOverview, String language)
        {
            var s3Language = s3ModelsOverview.SingleOrDefault(l => l.Code.Equals(language, StringComparison.InvariantCultureIgnoreCase));

            if (s3Language != null)
            {
                return s3Language;
            }
            s3ModelsOverview.Add(new Language(language));
            return s3ModelsOverview.Single(l => l.Code.Equals(language, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
