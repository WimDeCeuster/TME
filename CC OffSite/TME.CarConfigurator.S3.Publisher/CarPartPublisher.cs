﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.Publisher
{
    public class CarPartPublisher : ICarPartPublisher
    {
        readonly ICarPartService _service;

        public CarPartPublisher(ICarPartService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            _service = service;
        }

        public async Task PublishCarPartsAsync(IContext context)
        {
            var tasks = context.ContextData.Values.Select(contextData => PublishCarPartsAsync(context.Brand, context.Country, contextData.Publication.ID, contextData.CarParts)).ToList();
            await Task.WhenAll(tasks);
        }

        private async Task PublishCarPartsAsync(string brand, string country, Guid publicationId, IEnumerable<KeyValuePair<Guid, IList<CarPart>>> carParts)
        {
            var tasks = carParts.Select(entry => _service.PutCarParts(brand, country, publicationId, entry.Key, entry.Value)).ToList();
            await Task.WhenAll(tasks);
        }
    }
}