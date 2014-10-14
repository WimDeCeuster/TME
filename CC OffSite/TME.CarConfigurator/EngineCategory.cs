using System;
using System.Collections.Generic;
using System.Linq;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;
using TME.CarConfigurator.Interfaces.Core;

namespace TME.CarConfigurator
{
    public class EngineCategory : BaseObject, IEngineCategory
    {
        private readonly Repository.Objects.EngineCategory _engineCategory;
        private readonly Repository.Objects.Context _context;

        public EngineCategory(Repository.Objects.EngineCategory engineCategory, Repository.Objects.Context context)
            : base(engineCategory)
        {
            if (engineCategory == null) throw new ArgumentNullException("engineCategory");
            if (context == null) throw new ArgumentNullException("context");

            _engineCategory = engineCategory;
            _context = context;
        }

        public IEnumerable<IAsset> Assets { get { throw new NotImplementedException(); } }
    }
}