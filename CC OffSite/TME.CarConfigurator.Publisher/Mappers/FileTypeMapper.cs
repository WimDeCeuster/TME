using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TME.CarConfigurator.Publisher.Interfaces;
using TME.CarConfigurator.Repository.Objects.Assets;

namespace TME.CarConfigurator.Publisher.Mappers
{
    public class FileTypeMapper : IFileTypeMapper
    {
        public FileType MapFileType(Administration.FileType fileType)
        {
            return new FileType
            {
                Code = fileType.Code,
                Type = fileType.Type
            };
        }
    }
}
