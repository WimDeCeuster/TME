using System;
using System.Linq;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

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
            var languages = context.ModelGenerations.Keys;

            foreach (var language in languages)
                PublishLanguage(language, context);
        }

        void PublishLanguage(String language, IContext context)
        {
            PublishPublication(language, context);

            // publish rest

            // update models file
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
        }
    }
}
