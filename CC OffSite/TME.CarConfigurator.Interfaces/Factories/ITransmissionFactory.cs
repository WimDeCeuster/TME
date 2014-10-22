using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ITransmissionFactory
    {
        IReadOnlyList<ITransmission> GetTransmissions(Publication publication, Context context);
    }
}
