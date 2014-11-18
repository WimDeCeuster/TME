Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class ExteriorColourPermissions
        Inherits PermissionsWithAssetPermissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            Dim _crudrights As Boolean = context.CountryCode.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase) AndAlso Thread.CurrentPrincipal.IsInRole("ISG Administrator")
            Create = _crudrights
            Update = _crudrights
            Delete = _crudrights
            _assetPermission = New LinkedAssetPermissions(context)
        End Sub
#End Region

    End Class
End Namespace