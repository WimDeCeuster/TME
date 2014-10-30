using TME.CarConfigurator.Interfaces.Assets;
using Legacy = TMME.CarConfigurator;

namespace TME.CarConfigurator.LegacyAdapter
{
    public class FileType : IFileType
    {
         #region Dependencies (Adaptee)
        private Legacy.FileType Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public FileType(Legacy.FileType adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion

        public string Code
        {
            get { return Adaptee.Code; }
        }

        public string Type
        {
            get { return Adaptee.Type; }
        }
    }
}
