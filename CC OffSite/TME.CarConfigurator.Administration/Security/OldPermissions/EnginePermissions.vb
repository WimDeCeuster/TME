Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class EnginePermissions
        Inherits PermissionsWithAssetPermissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            If context.CountryCode.Length = 0 Then Return
            If context.IsGlobal() Then
                If Thread.CurrentPrincipal.IsInRole("BASE Administrator") OrElse Thread.CurrentPrincipal.IsInRole("ISG Administrator") Then
                    Create = True
                    Update = True
                    Delete = True
                    Activate = True
                    Approve = True
                    Sort = True
                    ViewAssets = True
                End If
            End If
            _assetPermission = New LinkedAssetPermissions(context)
            Sort = (Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse Thread.CurrentPrincipal.IsInRole("NMSC Administrator")) AndAlso (Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry)
            Create = Sort
            Update = Sort
            Delete = Sort
            ViewAssets = context.CanViewAssets AndAlso Sort
            UsesNonVATPrice = context.UsesNonVATPrice
        End Sub
#End Region

    End Class
End Namespace