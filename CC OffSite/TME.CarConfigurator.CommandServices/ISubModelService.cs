using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.CommandServices
{
    public interface ISubModelService
    {
        Task<Result> PutTimeFrameGenerationSubModelsAsync(String brand, String country, Guid publicationID, Guid timeFrameID, IEnumerable<SubModel> subModels);
    }
}