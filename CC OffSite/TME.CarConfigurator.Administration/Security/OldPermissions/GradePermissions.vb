Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class GradePermissions
        Inherits Permissions

        Private ReadOnly _assetPermission As LinkedAssetPermissions
        Public ReadOnly Property Assets() As LinkedAssetPermissions
            Get
                Return _assetPermission
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            If context.CountryCode.Length = 0 Then Return

            If Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("ISG Administrator") Then
                Create = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry
                Update = Create
                Delete = Create
                Activate = Create
                Approve = Create
                Sort = Create
            End If
            'End If
            _assetPermission = New LinkedAssetPermissions(context)
            ViewAssets = context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")
            UsesNonVATPrice = context.UsesNonVATPrice
        End Sub
#End Region

    End Class
End Namespace