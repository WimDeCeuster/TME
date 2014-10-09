﻿using System.Collections.Generic;
using TME.CarConfigurator.Factories.Interfaces;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator
{
    public class Models : List<IModel>, IModels
    {
        public Models(IEnumerable<IModel> intermediateModels)
            : base(intermediateModels)
        {
        }

        public static IModels GetModels(Context context, IModelFactory modelFactory = null)
        {
            // as Models is the entry point into the library, it can call upon the DI container. All objects in the sub hierarchy (cars, grades, ...) should have the factories they need injected
            modelFactory = modelFactory ?? null; // TODO: Get from DI container (poor man's DI) instead of null

            return modelFactory.GetModels(context);
        }
    }
}
