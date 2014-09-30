using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.QueryRepository
{
    public interface IBodyTypeRepository
    {
        IEnumerable<BodyType> GetBodyTypes(Repository.Objects.Context.PublicationTimeFrame context);
    }
}
