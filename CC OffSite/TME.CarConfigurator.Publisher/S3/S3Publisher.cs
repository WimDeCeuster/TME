using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Enums.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using Language = TME.CarConfigurator.Repository.Objects.Language;
using Languages = TME.CarConfigurator.Repository.Objects.Languages;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3Publisher : IPublisher
    {
        IService _service;

        public S3Publisher(IService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            _service = service;
        }

        public Task<Result> Publish(IContext context)
        {
            var languages = context.ContextData.Keys;

            var publishTasks = new List<Task<Result>>();
            foreach (var language in languages)
            { 
                publishTasks.AddRange(PublishLanguage(language, context));
            }

            var s3ModelsOverview = _service.GetModelsOverviewPerLanguage();

            foreach (var language in languages)
            {
                var s3Language = GetS3Language(s3ModelsOverview, language);

                var s3Models = s3Language.Models;
                var contextModel = context.ContextData[language].Models.Single();
                var s3Model = s3Models.SingleOrDefault(m => m.ID == contextModel.ID);
               
                if (s3Model == null)
                {
                    s3Models.Add(contextModel);
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
            }

            return Task.Factory.StartNew(() =>
            {
                Task.WaitAll(publishTasks.ToArray());

                var failure = publishTasks.Select(task => task.Result).FirstOrDefault(result => result is Failed);
                if (failure != null)
                    return failure;

                return _service.PutModelsOverviewPerLanguage(s3ModelsOverview);
            });
        }

        IEnumerable<Task<Result>> PublishLanguage(String language, IContext context)
        {
            var tasks = new List<Task<Result>>();

            tasks.Add(PublishPublication(language, context));

            // publish rest

            return tasks;
        }

        Task<Result> PublishPublication(String language, IContext context)
        {
            var data = context.ContextData[language];
            var timeFrames = context.TimeFrames[language];
            var publication = new Publication
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

            data.Models.Single().Publications.Add(new PublicationInfo(publication));

            return _service.PutPublication(language, publication);
        }

        private static Language GetS3Language(Languages s3ModelsOverview, string language)
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
