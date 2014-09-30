using System.Runtime.Serialization;

namespace TME.CarConfigurator.Repository.Objects.Assets
{
    [DataContract]
    public class VisibleInModeAndView
    {
        [DataMember]
        public string Mode { get; set; }
        [DataMember]
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

    }
}
