using System;

namespace TME.CarConfigurator.Interfaces.Assets
{
    public interface IAsset
    {
        Guid ID { get; }
        int ShortID { get; }
        string Name { get; }

        string FilePath { get; }
        IFileType FileType { get; }

        bool IsTransparent { get; }
        bool RequiresMatte { get; }
        int StackingOrder { get; }
        short Width { get; }
        short Height { get; }
        short PositionX { get; }
        short PositionY { get; }
        
        bool AlwaysInclude { get; }
        IAssetType AssetType { get; }

        string Hash { get; }
    }
}
