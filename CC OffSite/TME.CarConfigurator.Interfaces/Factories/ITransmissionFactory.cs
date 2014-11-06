using System;
using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Interfaces.Factories
{
    public interface ITransmissionFactory
    {
        IReadOnlyList<ITransmission> GetTransmissions(Publication publication, Context context);
        ITransmission GetCarTransmission(Transmission transmission, Guid carID, Publication repositoryPublication, Context repositoryContext);
    }
}
