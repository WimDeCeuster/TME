using System;
using System.Collections.Generic;
using TME.CarConfigurator.Core;
using TME.CarConfigurator.Interfaces.Equipment;
using RepositoryCategory = TME.CarConfigurator.Repository.Objects.Equipment.Category;

namespace TME.CarConfigurator.Equipment
{
    public class Category : BaseObject<RepositoryCategory>, ICategory
    {
        public Category(RepositoryCategory category)
            : base(category)
        { }

        public string Path { get { return RepositoryObject.Path; } }

        public ICategory Parent { get; internal set; }

        public IReadOnlyList<ICategory> Categories { get; internal set; }
    }
}
