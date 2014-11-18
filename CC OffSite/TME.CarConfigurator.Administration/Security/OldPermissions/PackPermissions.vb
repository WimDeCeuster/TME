Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class PackPermissions
        Inherits PermissionsWithAssetPermissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            If context.CountryCode.Length = 0 Then Return

            If Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator") Then
                Create = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry
                Update = Create
                Delete = Create
            End If

            _assetPermission = New LinkedAssetPermissions(context)
        End Sub
#End Region

    End Class
End Namespace