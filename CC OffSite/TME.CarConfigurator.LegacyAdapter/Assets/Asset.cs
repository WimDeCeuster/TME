﻿using System;
using TME.CarConfigurator.Interfaces.Assets;

namespace TME.CarConfigurator.LegacyAdapter.Assets
{
    public class Asset : IAsset
    {

                                
        #region Dependencies (Adaptee)
        private TMME.CarConfigurator.Asset Adaptee
        {
            get;
            set;
        }
        #endregion

        #region Constructor
        public Asset(TMME.CarConfigurator.Asset adaptee)
        {
            Adaptee = adaptee;
        }
        #endregion


        public Guid ID
        {
            get { return Adaptee.ID; }
        }

        public int ShortID
        {
            get { return Adaptee.ShortID; }
        }

        public string Name
        {
            get { return Adaptee.Name; }
        }

        public string FilePath
        {
            get { return Adaptee.FileName; }
        }

        public IFileType FileType
        {
            get { return new FileType(Adaptee.FileType); }
        }

        public bool IsTransparent
        {
            get { return Adaptee.IsTransparent; }
        }

        public bool RequiresMatte
        {
            get { return Adaptee.RequiresMatte; }
        }

        public int StackingOrder
        {
            get { return Adaptee.StackingOrder; }
        }

        public short Width
        {
            get { return Adaptee.Width; }
        }

        public short Height
        {
            get { return Adaptee.Height; }
        }

        public short PositionX
        {
            get { return Adaptee.PositionX; }
        }

        public short PositionY
        {
            get { return Adaptee.PositionY; }
        }

        public bool AlwaysInclude
        {
            get { return Adaptee.AlwaysInclude; }
        }

        public IAssetType AssetType
        {
            get { return new AssetType(Adaptee.AssetType); }
        }

        public string Hash
        {
            get { return Adaptee.Hash; }
        }
    }
}
