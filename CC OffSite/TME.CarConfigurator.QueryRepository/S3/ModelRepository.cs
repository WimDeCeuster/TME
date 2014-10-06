using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository.Interfaces;
using TME.CarConfigurator.QueryRepository.Service.Interfaces;

namespace TME.CarConfigurator.QueryRepository.S3
{
    public class ModelRepository : IModelRepository
    {
        public ModelRepository(ILanguageService languageService)
        {

        }

        public IModels GetModels(IContext context)
        {

        }
    }
}