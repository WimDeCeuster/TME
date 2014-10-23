using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Common.Result;
using TME.CarConfigurator.Repository.Objects.Equipment;

namespace TME.CarConfigurator.CommandServices
{
    public interface IGradeAccessoryService
    {
        Task<Result> Put(String brand, String country, Guid publicationID, Guid timeFrameID, Guid gradeID, IEnumerable<GradeAccessory> GradeAccessories);
    }
}
