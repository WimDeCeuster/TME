using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Interfaces;

using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface IBodyTypeService
    {
        Task PutTimeFrameGenerationBodyTypes(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<BodyType> bodyTypes);
    }
}
