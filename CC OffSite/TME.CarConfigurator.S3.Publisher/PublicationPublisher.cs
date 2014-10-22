using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class PublicationPublisher : IPublicationPublisher
    {
        readonly IPublicationService _publicationService;

        public PublicationPublisher(IPublicationService publicationService)
        {
            if (publicationService == null) throw new ArgumentNullException("publicationService");

            _publicationService = publicationService;
        }

        public async Task<IEnumerable<Result>> PublishPublications(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = new List<Task<Result>>();

            foreach (var entry in context.ContextData)
            {
                var language = entry.Key;
                var data = entry.Value;

                tasks.Add(_publicationService.PutPublication(context.Brand, context.Country, data.Publication));
            }

            return await Task.WhenAll(tasks);
        }
    }
}
