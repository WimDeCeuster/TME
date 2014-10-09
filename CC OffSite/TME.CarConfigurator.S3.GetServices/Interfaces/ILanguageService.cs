using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.GetServices.Interfaces
{
    public interface ILanguageService
    {
        Languages GetLanguages(Context context);
    }
}