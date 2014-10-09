using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.Base;
using TME.CarConfigurator.S3.GetServices;
using TME.CarConfigurator.S3.GetServices.Interfaces;

namespace TME.CarConfigurator.QueryRepository.Tests.TestBuilders
{
    public class LanguageServiceBuilder : ServiceBuilderBase<ILanguageService>
    {

        public static LanguageServiceBuilder Initialize()
        {
            return new LanguageServiceBuilder();
        }

        public override ILanguageService Build()
        {
            return new LanguageService(Serialiser, Service, KeyManager);
        }
    }
}