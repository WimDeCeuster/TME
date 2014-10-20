using TME.CarConfigurator.Interfaces.Assets;

namespace TME.FrontEndViewer.Models
{
    public class AssetWithPrefix
    {
        public IAsset Asset { get; set; }
        public string Prefix { get; set; }

    }
}