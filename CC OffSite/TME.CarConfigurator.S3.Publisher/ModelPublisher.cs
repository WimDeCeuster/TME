using System;
using System.Threading.Tasks;
using TME.CarConfigurator.CommandServices;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.Publisher
{
    public class ModelPublisher : IModelPublisher
    {
        readonly IModelService _modelService;

        public ModelPublisher(IModelService modelService)
        {
            if (modelService == null) throw new ArgumentNullException("modelService");

            _modelService = modelService;
        }


        public async Task PublishModelsByLanguage(IContext context, Languages languages)
        {
            if (context == null) throw new ArgumentNullException("context");

            await _modelService.PutModelsByLanguage(context.Brand, context.Country, languages);
        }
    }
}
