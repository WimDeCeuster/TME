using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository.Interfaces
{
    public interface IBodyTypeRepository
    {
        IEnumerable<BodyType> GetBodyTypes(Repository.Objects.Context.PublicationTimeFrame context);
    }
}
