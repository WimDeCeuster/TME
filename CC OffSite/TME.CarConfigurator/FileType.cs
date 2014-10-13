using System;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator
{
    public class FileType : IFileType
    {
        private readonly Repository.Objects.Assets.FileType _fileType;

        public FileType(Repository.Objects.Assets.FileType fileType)
        {
            if (fileType == null) throw new ArgumentNullException("fileType");
            _fileType = fileType;
        }

        public string Code { get { return _fileType.Code; } }
        public string Type { get { return _fileType.Type; } }
    }
}