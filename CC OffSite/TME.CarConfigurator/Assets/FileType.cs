using System;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.Assets
{
    public class FileType : IFileType
    {
        private readonly Repository.Objects.Assets.FileType _repositoryFileType;

        public FileType(Repository.Objects.Assets.FileType repositoryFileType)
        {
            if (repositoryFileType == null) throw new ArgumentNullException("repositoryFileType");
            _repositoryFileType = repositoryFileType;
        }

        public string Code { get { return _repositoryFileType.Code; } }
        public string Type { get { return _repositoryFileType.Type; } }
    }
}