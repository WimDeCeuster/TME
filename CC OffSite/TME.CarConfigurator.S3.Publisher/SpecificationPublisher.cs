using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;
using TME.CarConfigurator.S3.Publisher.Interfaces;

namespace TME.CarConfigurator.S3.Publisher
{
    public class SpecificationsPublisher : ISpecificationsPublisher
    {
        readonly ISpecificationsService _specificationsService;
        readonly ITimeFramePublishHelper _timeFramePublishHelper;

        public SpecificationsPublisher(ISpecificationsService specificationsService, ITimeFramePublishHelper timeFramePublishHelper)
        {
            if (specificationsService == null) throw new ArgumentNullException("specificationsService");
            if (timeFramePublishHelper == null) throw new ArgumentNullException("timeFramePublishHelper");

            _specificationsService = specificationsService;
            _timeFramePublishHelper = timeFramePublishHelper;
        }

        public async Task PublishCategoriesAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            await Task.WhenAll(context.ContextData.Select(entry =>
                _specificationsService.PutCategoriesAsync(context.Brand, context.Country, entry.Value.Publication.ID, entry.Value.SpecificationCategories)));
        }

        public async Task PublishCarTechnicalSpecificationsAsync(IContext context)
        {
            if (context == null) throw new ArgumentNullException("context");


            var tasks = 
                context.ContextData.Values.Select(
                    contextData => PublishCarTechnicalSpecificationsAsync(context.Brand, context.Country, contextData.Publication.ID, contextData.CarTechnicalSpecifications));
            await Task.WhenAll(tasks);
        }

        private async Task PublishCarTechnicalSpecificationsAsync(string brand, string country, Guid publicationID, IEnumerable<KeyValuePair<Guid, IReadOnlyList<CarTechnicalSpecification>>> carTechnicalSpecifications)
        {
            var tasks =
             carTechnicalSpecifications.Select(
                 entry => _specificationsService.PutCarTechnicalSpecificationsAsync(brand, country, publicationID, entry.Key, entry.Value)).ToList();
            await Task.WhenAll(tasks);
        }
    }
}
