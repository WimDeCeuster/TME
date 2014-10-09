using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.S3.Shared.Result;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;
using TME.CarConfigurator.Repository.Objects.Enums;
using IContext = TME.CarConfigurator.Publisher.Interfaces.IContext;

namespace TME.CarConfigurator.Publisher.S3
{
    public class S3Publisher : IPublisher
    {
        readonly IS3PublicationService _publicationService;
        readonly IS3LanguageService _languageService;
        readonly IS3BodyTypeService _bodyTypeService;
        readonly IS3EngineService _engineService;

        public S3Publisher(IS3PublicationService publicationService, IS3LanguageService languageService, IS3BodyTypeService bodyTypeService, IS3EngineService engineService)
        {
            if (publicationService == null) throw new ArgumentNullException("publicationService");
            if (languageService == null) throw new ArgumentNullException("languageService");
            if (bodyTypeService == null) throw new ArgumentNullException("bodyTypeService");
            if (engineService == null) throw new ArgumentNullException("engineService");

            _publicationService = publicationService;
            _languageService = languageService;
            _bodyTypeService = bodyTypeService;
            _engineService = engineService;
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
            return await _languageService.PutModelsOverviewPerLanguage(context, s3ModelsOverview);
        }

        private Languages ActivatePublicationForAllLanguages(IContext context, IEnumerable<String> languages)
        {
            var s3ModelsOverview = _languageService.GetModelsOverviewPerLanguage(context);
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
            return await _publicationService.PutPublications(context);
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
            return _bodyTypeService.PutGenerationBodyTypes(context);
        }

        Task<IEnumerable<Result>> PublishGenerationEngines(IContext context)
        {
            return _engineService.PutGenerationEngines(context);
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
