using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.S3.Publisher
{
    public class CarEquipmentPublisher : ICarEquipmentPublisher
    {
        private readonly ICarEquipmentService _service;

        public CarEquipmentPublisher(ICarEquipmentService service)
        {
            if (service == null) throw new ArgumentNullException("service");

            _service = service;
        }

        public async Task PublishCarEquipmentAsync(IContext context)
        {
            var tasks =
                context.ContextData.Values.Select(
                    contextData =>
                        PublishCarEquipmentAsync(context.Brand, context.Country, contextData.Publication.ID,
                            contextData.CarEquipment));
            await Task.WhenAll(tasks);
        }

        private async Task PublishCarEquipmentAsync(string brand, string country, Guid publicationID, IEnumerable<KeyValuePair<Guid, CarEquipment>> carEquipment)
        {
            var tasks =
                carEquipment.Select(
                    entry => _service.PutCarEquipment(brand, country, publicationID, entry.Key, entry.Value)).ToList();
            await Task.WhenAll(tasks);
        }
    }
}