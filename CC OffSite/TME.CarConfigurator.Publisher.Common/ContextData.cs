using System.Collections.Generic;
using TME.CarConfigurator.Repository.Objects;

namespace TME.CarConfigurator.Publisher.Common
{
    public class ContextData
    {
        public IList<Car> Cars { get; private set; }
        public IList<Generation> Generations { get; private set; }
        public IList<Model> Models { get; private set; }
        public IList<BodyType> BodyTypes { get; private set; }
        public IList<Engine> Engines { get; private set; }
        public IList<Transmission> Transmissions { get; private set; }
        public Publication Publication { get; set; }

        public ContextData()
        {
            Cars = new List<Car>();
            Models = new List<Model>();
            Generations = new List<Generation>();
            BodyTypes = new List<BodyType>();
            Engines = new List<Engine>();
            Transmissions = new List<Transmission>();
        }
    }
}
