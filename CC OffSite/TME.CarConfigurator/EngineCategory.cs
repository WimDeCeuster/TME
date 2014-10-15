using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class EngineCategory : BaseObject, IEngineCategory
    {
        private readonly Repository.Objects.EngineCategory _repositoryEngineCategory;
        private readonly Repository.Objects.Context _repositoryContext;

        public EngineCategory(Repository.Objects.EngineCategory repositoryEngineCategory, Repository.Objects.Context repositoryContext)
            : base(repositoryEngineCategory)
        {
            if (repositoryEngineCategory == null) throw new ArgumentNullException("repositoryEngineCategory");
            if (repositoryContext == null) throw new ArgumentNullException("repositoryContext");

            _repositoryEngineCategory = repositoryEngineCategory;
            _repositoryContext = repositoryContext;
        }

        public IEnumerable<IAsset> Assets { get { throw new NotImplementedException(); } }
    }
}