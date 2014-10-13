using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface IKeyManager
    {
        String GetLanguagesKey();
        String GetPublicationKey(Guid publicationID);
        String GetGenerationBodyTypesKey(Guid publicationID, Guid timeFrameID);
        String GetGenerationEnginesKey(Guid publicationID, Guid timeFrameID);
        String GetGenerationAssetKey(Guid publicationID);
    }
}
