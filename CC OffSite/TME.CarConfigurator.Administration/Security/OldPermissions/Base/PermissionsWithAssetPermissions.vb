Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.OldPermissions.Base
    Public Class PermissionsWithAssetPermissions
        Inherits Permissions

        Protected _assetPermission As LinkedAssetPermissions
        Public ReadOnly Property Assets() As LinkedAssetPermissions
            Get
                Return _assetPermission
            End Get
        End Property

        Protected Friend Sub New()
            'Empty
        End Sub
        Protected Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
        End Sub

    End Class
End Namespace