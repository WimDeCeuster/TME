using System;
using System.Collections.Generic;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.QueryServices
{
    public class BodyTypeService : IBodyTypeService
    {
        public IEnumerable<BodyType> GetBodyTypes(Guid publicationId, Guid publicationTimeFrameId, Context context)
        {
            throw new NotImplementedException();
        }
    }
}