Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class ActiveFilterToolPermissions
        Inherits PermissionsWithAssetPermissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            If context.CountryCode.Length = 0 Then Return

            If Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse Thread.CurrentPrincipal.IsInRole("NMSC Administrator") Then
                Create = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry
                Update = Create
                Delete = Create
                Activate = (Thread.CurrentPrincipal.IsInRole("MKT Administrator") AndAlso (String.Compare(context.CountryCode, Environment.GlobalCountryCode, True) = 0))
                Approve = Create
                Sort = Create
                ViewAssets = context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")
                Translate = True
            Else
                Sort = False
                ViewAssets = False
            End If

            _assetPermission = New LinkedAssetPermissions(context)
            _assetPermission.Create = _assetPermission.Create OrElse (Thread.CurrentPrincipal.IsInRole("NMSC Administrator") AndAlso (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry))
            _assetPermission.Update = _assetPermission.Create OrElse (Thread.CurrentPrincipal.IsInRole("NMSC Administrator") AndAlso (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry))
            _assetPermission.Delete = _assetPermission.Create OrElse (Thread.CurrentPrincipal.IsInRole("NMSC Administrator") AndAlso (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry))
            UsesNonVATPrice = context.UsesNonVATPrice

        End Sub
#End Region

    End Class
End Namespace