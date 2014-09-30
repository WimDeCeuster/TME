using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandRepository
{
    public interface IModelRepository
    {
        Result.Result Create(Repository.Objects.Context.Base context, Model model);
        Result.Result Update(Repository.Objects.Context.Base context, Model model);

        Result.Result ActivatePublication(Repository.Objects.Context.Base context, Model model, Publication publication);

    }
}
