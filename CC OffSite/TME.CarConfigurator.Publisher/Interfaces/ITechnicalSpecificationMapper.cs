using TME.CarConfigurator.Repository.Objects.TechnicalSpecifications;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface ITechnicalSpecificationMapper
    {
        CarTechnicalSpecification MapTechnicalSpecification(Administration.CarSpecification carSpecification, Administration.Specification specification, Administration.Unit unit);
    }
}