using System;

namespace TME.CarConfigurator.Publisher.Extensions
{
    public static class LinkExtensions
    {
        public static Boolean IsApplicableFor(this Administration.Link link, Administration.ModelGeneration generation)
        {
            return link.Type.CarConfiguratorversionID == generation.ActiveCarConfiguratorVersion.ID || link.Type.CarConfiguratorversionID == 0;
        }
    }
}