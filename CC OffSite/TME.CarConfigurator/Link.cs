using TME.CarConfigurator.Interfaces;

namespace TME.CarConfigurator
{
    public class Link : ILink
    {
        public short ID { get; private set; }
        public string Name { get; private set; }
        public string Label { get; private set; }
        public string Url { get; private set; }
    }
}
