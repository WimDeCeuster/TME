using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Extensions;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Factories;
using TME.CarConfigurator.QueryServices;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Factories
{
    public class BodyTypeFactory : IBodyTypeFactory
    {
        private readonly IBodyTypeService _bodyTypeService;

        public BodyTypeFactory(IBodyTypeService bodyTypeService)
        {
            if (bodyTypeService == null) throw new ArgumentNullException("bodyTypeService");
            _bodyTypeService = bodyTypeService;
        }

        public IEnumerable<IBodyType> GetBodyTypes(Publication publication, Context context)
        {
            var currentTimeFrame = publication.GetCurrentTimeFrame();

            var repoBodyTypes = _bodyTypeService.GetBodyTypes(publication.ID, currentTimeFrame.ID, context);

            return repoBodyTypes.Select(bt => new BodyType(bt));
        }
    }
}