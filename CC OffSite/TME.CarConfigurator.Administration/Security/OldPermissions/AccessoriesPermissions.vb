Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class AccessoriesPermissions
        Inherits PermissionsWithAssetPermissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)

            _assetPermission = New LinkedAssetPermissions(context)
            If context.CountryCode.Length = 0 Then Return

            If context.IsGlobal() Then
                If Thread.CurrentPrincipal.IsInRole("CSG Administrator") Then
                    Create = True
                    Update = True
                    Delete = True
                    Activate = True
                    Approve = True
                    ViewAssets = True
                    Price = False
                    Sort = True
                    Translate = True

                    _assetPermission.Create = True
                    _assetPermission.ViewAssets = True
                    _assetPermission.Delete = True
                End If
            Else
                Sort = (Thread.CurrentPrincipal.IsInRole("NMSC ACCESSORY Administrator") AndAlso (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry)) OrElse (context.IsEurope() AndAlso Thread.CurrentPrincipal.IsInRole("CSG Administrator"))

                Translate = Thread.CurrentPrincipal.IsInRole("NMSC ACCESSORY Administrator")

                If context.IsEurope() Then
                    Price = False
                Else
                    Price = Thread.CurrentPrincipal.IsInRole("NMSC ACCESSORY Administrator")
                End If
                ViewAssets = (context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")) AndAlso (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry) OrElse (context.IsEurope() AndAlso Thread.CurrentPrincipal.IsInRole("CSG Administrator"))
                If Thread.CurrentPrincipal.IsInRole("NMSC ACCESSORY Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse (context.IsEurope() AndAlso Thread.CurrentPrincipal.IsInRole("CSG Administrator")) Then
                    Approve = (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry)
                End If
            End If
            UsesNonVATPrice = context.UsesNonVATPrice

        End Sub
#End Region

    End Class
End Namespace