using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.S3.Shared.Interfaces
{
    public interface IKeyManager
    {
        String GetLanguagesKey();
        String GetPublicationKey(Publication publication);
        String GetGenerationBodyTypesKey(Publication publication, PublicationTimeFrame timeFrame);
    }
}
