using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.QueryRepository;
using TME.CarConfigurator.RepositoryFacades;

namespace TME.CarConfigurator
{
    public class Models : ReadOnlyList<IModel>
    {

        public Models(Repository.Objects.Context.Base context, IModelRepository repository, PublicationRepositoryFacade publicationRepositoryFacade)
        {
            var models = repository.GetModels(context);
            List.AddRange(models.Select(x => new Model(x, null)));
        }

 
    }
}
