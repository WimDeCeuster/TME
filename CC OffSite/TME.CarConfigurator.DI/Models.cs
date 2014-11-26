using System.Collections.Generic;
using System.Configuration;
using TME.CarConfigurator.DI.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.DI
{
    public class Models
    {
        public static IEnumerable<IModel> GetModels(Context context)
        {
            var target = ConfigurationManager.AppSettings.Get("Target");

            IServiceFacade serviceFacade;

            switch ((target ?? "s3").ToLowerInvariant())
            {
                case "s3":
                    serviceFacade = new S3ServiceFacade();
                    break;
                case "filesystem":
                    serviceFacade = new FileSystemServiceFacade();
                    break;
                default:
                    throw new ConfigurationException("No valid publication target (\"Target\") specified in app settings.");
            }

            var modelFactoryFacade = new ModelFactoryFacade().WithServiceFacade(serviceFacade);

            return GetModels(context, modelFactoryFacade);
        }

        public static IEnumerable<IModel> GetModels(Context context, IModelFactoryFacade modelFactoryFacade)
        {
            var modelFactory = modelFactoryFacade.Create();

            return modelFactory.GetModels(context);
        }
    }
}
