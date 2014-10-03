using System;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IS3Serialiser
    {
        String Serialise(Publication publication);
    }
}