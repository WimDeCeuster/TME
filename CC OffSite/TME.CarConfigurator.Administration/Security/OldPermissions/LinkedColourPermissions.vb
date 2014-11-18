Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class LinkedColourPermissions
        Inherits Permissions

        Private ReadOnly _assetPermission As LinkedAssetPermissions

        Public ReadOnly Property Assets() As LinkedAssetPermissions
            Get
                Return _assetPermission
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            Dim _crudrights As Boolean = context.IsGlobal() AndAlso Thread.CurrentPrincipal.IsInRole("MKT Administrator")
            Create = _crudrights
            Update = _crudrights OrElse Thread.CurrentPrincipal.IsInRole("NMSC Administrator")
            Delete = _crudrights
            _assetPermission = New LinkedAssetPermissions(context)
        End Sub
#End Region

    End Class
End Namespace