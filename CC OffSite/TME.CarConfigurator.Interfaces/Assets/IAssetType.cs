
namespace TME.CarConfigurator.Interfaces.Assets
{
    public interface IAssetType
    {
        string Code { get; }
        string Name { get; }

        string Mode { get; }
        string View { get; }
        string Side { get; }
        string Type { get; }

        string ExteriorColourCode { get; }
        string UpholsteryCode { get; }
        string EquipmentCode { get; }
    }
}
