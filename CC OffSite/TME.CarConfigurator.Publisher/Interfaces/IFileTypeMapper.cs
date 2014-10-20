using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Interfaces
{
    public interface IFileTypeMapper
    {
        FileType MapFileType(Administration.FileType fileType);
    }
}
