using TME.CarConfigurator.Repository.Objects.Core;

namespace TME.CarConfigurator.Repository.Objects.Colours
{
    public class Upholstery : BaseObject
    {
        public string InteriorColourCode { get; set; }
        public string TrimCode { get; set; }
        public UpholsteryType Type { get; set; }
    }
}
