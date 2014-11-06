using System;
using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.TechnicalSpecifications
{
    public class Category : BaseObject
    {
        public Guid? ParentCategoryID { get; set; }
        public String Path { get; set; }
    }
}
