﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

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

        public async Task PublishPublicationsAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var tasks = context.ContextData.Values.Select(data => _publicationService.PutPublication(context.Brand, context.Country, data.Publication)).ToList();

            await Task.WhenAll(tasks);
        }
    }
}
