using System;
using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects.Assets
{
    
    public class VisibleInModeAndView
    {
        
        public string Mode { get; set; }
        
        public string View { get; set; }



        #region System.Object overrides

        public override bool Equals(object obj)
        {
            var option = obj as VisibleInModeAndView;
            if (option != null) return option.Mode == Mode && option.View == View;
            return obj.Equals(this);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{0},{1}", Mode, View);
        }
        #endregion

        public bool HasMode(string mode)
        {
            return Mode.Equals(mode, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool HasView(string view)
        {
            return Mode.Equals(view, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
