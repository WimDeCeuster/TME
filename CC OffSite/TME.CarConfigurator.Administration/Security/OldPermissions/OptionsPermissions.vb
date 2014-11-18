Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class OptionsPermissions
        Inherits PermissionsWithAssetPermissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            If context.CountryCode.Length = 0 Then Return

            If (Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse (Thread.CurrentPrincipal.IsInRole("NMSC Administrator")) AndAlso context.CanCreateLocalEquipment) Then
                Create = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry
                Update = Create
                Delete = Create
                Activate = (Thread.CurrentPrincipal.IsInRole("MKT Administrator") AndAlso (String.Compare(context.CountryCode, Environment.GlobalCountryCode, True) = 0))
                Approve = Create
                Sort = Create
                ViewAssets = (context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")) AndAlso (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry)

                If context.IsEurope() OrElse context.IsGlobal() Then
                    Price = False
                Else
                    Price = Thread.CurrentPrincipal.IsInRole("NMSC Administrator")
                End If
            Else
                Sort = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry AndAlso (Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator"))
                ViewAssets = (context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")) AndAlso (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry)

            End If

            _assetPermission = New LinkedAssetPermissions(context)
            UsesNonVATPrice = context.UsesNonVATPrice
        End Sub
#End Region

    End Class
End Namespace