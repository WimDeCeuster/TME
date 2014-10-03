using System;
using System.Linq;
using TME.CarConfigurator.Administration;
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
        IS3Service _service;
        IS3Serialiser _serialiser;

        String _publicationPathTemplate = "{0}/generation/{1}";

        public S3Publisher(IS3Service service, IS3Serialiser serialiser)
        {
            if (service == null) throw new ArgumentNullException("service");
            if (serialiser == null) throw new ArgumentNullException("serialiser");

            _service = service;
            _serialiser = serialiser;
        }

        public void Publish(IContext context)
        {
            var languages = context.ContextData.Keys;

            foreach (var language in languages)
            { 
                PublishLanguage(language, context);
            }

            var s3ModelsOverview = _service.GetModelsOverviewPerLanguage(context.Brand, context.Country);

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
            _service.PutModelsOverviewPerLanguage(context.Brand, context.Country, s3ModelsOverview);
        }

        void PublishLanguage(String language, IContext context)
        {
            PublishPublication(language, context);

            // publish rest
        }

        void PublishPublication(String language, IContext context)
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

            _service.PutObject(String.Format(_publicationPathTemplate, language, publication.ID),
                               _serialiser.Serialise(publication));

            data.Models.Single().Publications.Add(new PublicationInfo(publication));
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
