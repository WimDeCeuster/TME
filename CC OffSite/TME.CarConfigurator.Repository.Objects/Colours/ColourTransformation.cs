namespace TME.CarConfigurator.Repository.Objects.Colours
{

    public class ColourTransformation
    {
        public ColourTransformation()
        {
            RGB = string.Empty;
        }

        public string RGB { get; set; }
        public decimal Hue { get; set; }
        public decimal Saturation { get; set; }
        public decimal Brightness { get; set; }
        public decimal Contrast { get; set; }
        public decimal Alpha { get; set; }
    }
}
