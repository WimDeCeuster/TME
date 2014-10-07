using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3Publisher : IPublisher
    {
        readonly IService _service;

        public S3Publisher(IService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            _service = service;
        }

        public async Task<Result> Publish(IContext context)
        {
            var languages = context.ContextData.Keys;
            var publishTasks = PublishPublicationForAllLanguages(context, languages);
            var s3ModelsOverview = ActivatePublicationForAllLanguages(context, languages);

            var results = await Task.WhenAll(publishTasks);

            var failure = results.SelectMany(xs => xs).FirstOrDefault(result => result is Failed);
            if (failure != null)
                return failure;

            return await _service.PutModelsOverviewPerLanguage(s3ModelsOverview);
        }

        private Languages ActivatePublicationForAllLanguages(IContext context, IEnumerable<string> languages)
        {
            var s3ModelsOverview = _service.GetModelsOverviewPerLanguage();
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
                return;
            }

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

        private async Task<IEnumerable<Result>> PublishPublicationForAllLanguages(IContext context, IEnumerable<string> languages)
        {
            var publishTasks = new List<Task<IEnumerable<Result>>>();
            foreach (var language in languages)
            {
                publishTasks.Add(PublishPublicationForLanguage(language, context));
            }

            var results = await Task.WhenAll(publishTasks);

            return results.SelectMany(xs => xs);
        }

        async Task<IEnumerable<Result>> PublishPublicationForLanguage(String language, IContext context)
        {
            var data = context.ContextData[language];
            var timeFrames = context.TimeFrames[language];
            var publication = CreateNewPublication(data, timeFrames);

            var tasks = new List<Task<Result>>
            {
                PublishPublication(data,publication),
            };
            
            // publish rest
            tasks.AddRange(PublishGenerationBodyTypesPerTimeFrame(publication, timeFrames, data));

            return await Task.WhenAll(tasks);
        }

        async Task<Result> PublishPublication(ContextData data, Publication publication)
        {
            data.Models.Single().Publications.Add(new PublicationInfo(publication));

            return await _service.PutPublication(publication);
        }

        private Publication CreateNewPublication(ContextData data, IReadOnlyList<TimeFrame> timeFrames)
        {
            return new Publication
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
        }

        IEnumerable<Task<Result>> PublishGenerationBodyTypesPerTimeFrame(Publication publication, IReadOnlyList<TimeFrame> timeFrames, ContextData data)
        {
            var bodyTypes = timeFrames.ToDictionary(
                                timeFrame => timeFrame,
                                timeFrame => data.GenerationBodyTypes.Where(bodyType =>
                                                                            timeFrame.Cars.Any(car => car.BodyType.ID == bodyType.ID))
                                                                     .ToList());

            var tasks = new List<Task<Result>>();

            foreach (var entry in bodyTypes)
                tasks.Add(_service.PutGenerationBodyTypes(publication, entry.Key, entry.Value));

            return tasks;
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
