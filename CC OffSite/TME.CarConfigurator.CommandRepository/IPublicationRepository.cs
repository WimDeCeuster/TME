using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandRepository
{
    public  interface IPublicationRepository
    {
        Result.Result Create(Repository.Objects.Context.Base context, Publication publication);
    }
}
