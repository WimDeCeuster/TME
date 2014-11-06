using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.TechnicalSpecifications
{
    public interface IModelTechnicalSpecifications
    {
        /* NEAR FUTURE Enhancement
         * 
        IReadOnlyList<ITechnicalSpecification> TechnicalSpecifications { get; }
         */
        IReadOnlyList<ICategory> Categories { get; }
    }
}
