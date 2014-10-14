using System;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface IKeyManager
    {
        String GetModelsKey();
        String GetPublicationKey(Guid publicationID);
        String GetBodyTypesKey(Guid publicationId, Guid timeFrameId);
        String GetEnginesKey(Guid publicationID, Guid timeFrameId);
        String GetDefaultAssetsKey(Guid publicationId, Guid objectId);
        String GetAssetsKey(Guid publicationId, Guid objectId, String view, String mode);
    }
}
