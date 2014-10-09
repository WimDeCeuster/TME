using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.Base;
using TME.CarConfigurator.S3.QueryServices;
using TME.CarConfigurator.S3.QueryServices.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class LanguageServiceBuilder : ServiceBuilderBase<IModelService>
    {

        public static LanguageServiceBuilder Initialize()
        {
            return new LanguageServiceBuilder();
        }

        public override IModelService Build()
        {
            return new ModelService(Serialiser, Service, KeyManager);
        }
    }
}