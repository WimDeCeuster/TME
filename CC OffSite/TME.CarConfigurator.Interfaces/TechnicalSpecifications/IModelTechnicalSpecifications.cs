using System.Collections.Generic;

namespace TME.CarConfigurator.Interfaces.TechnicalSpecifications
{
    public interface IModelTechnicalSpecifications
    {
        /* NEAR FUTURE Enhancement
         * 
        IReadOnlyList<ITechnicalSpecification> TechnicalSpecifications { get; }
         */
// ReSharper disable ReturnTypeCanBeEnumerable.Global
        IReadOnlyList<ICategory> Categories { get; }
// ReSharper restore ReturnTypeCanBeEnumerable.Global
    }
}
