using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface IKeyManager
    {
        String GetLanguagesKey();
        String GetPublicationKey(Guid publicationID);
        String GetGenerationBodyTypesKey(Guid publicationId, Guid timeFrameId);
        String GetGenerationEnginesKey(Guid publicationId, Guid timeFrameId);
    }
}
