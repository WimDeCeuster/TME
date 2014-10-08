using TME.CarConfigurator.QueryRepository.Service;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;
using TME.CarConfigurator.QueryRepository.Tests.TestBuilders.Base;

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