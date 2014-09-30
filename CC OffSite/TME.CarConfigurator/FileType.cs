using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class FileType : IFileType
    {
        public string Code { get; private set; }
        public string Type { get; private set; }
    }
}