using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher
{
    public interface IS3Serialiser
    {
        String Serialise(Publication publication);
    }

    public class S3Serialiser
    {
    }
}
