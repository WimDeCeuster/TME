using System;

namespace TME.CarConfigurator.RepositoryFacades
{
    public class PublicationTimeFrameRepositoryFacade
    {

        public PublicationTimeFramePartsRepositoryFacade PartsRepositoryFacade  { get; private set; }
        public CarRepositoryFacade CarRepositoryFacade  { get; private set; }

        public PublicationTimeFrameRepositoryFacade(PublicationTimeFramePartsRepositoryFacade partsRepositoryFacade, CarRepositoryFacade carRepositoryFacade)
        {
            if (partsRepositoryFacade == null) throw new ArgumentNullException("partsRepositoryFacade");
            if (carRepositoryFacade == null) throw new ArgumentNullException("carRepositoryFacade");

            PartsRepositoryFacade = partsRepositoryFacade;
            CarRepositoryFacade = carRepositoryFacade;
        }


    }
}
