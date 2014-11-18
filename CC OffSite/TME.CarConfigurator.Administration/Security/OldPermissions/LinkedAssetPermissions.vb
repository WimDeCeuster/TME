Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class LinkedAssetPermissions
        Inherits Permissions

#Region " Constructors "
        Public Sub New(ByVal context As MyContext)
            MyBase.New(context)
            If context.CountryCode.Length = 0 Then Return

            Update = Not context.Country.IsRegionCountry OrElse context.Country.IsMainRegionCountry

            If Not (String.Compare(context.CountryCode, Environment.GlobalCountryCode, True) = 0) AndAlso context.CanManageAssets AndAlso (Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse Thread.CurrentPrincipal.IsInRole("CSG LTH Administrator") OrElse Thread.CurrentPrincipal.IsInRole("NMSC LTH Administrator")) Then
                Create = Update
                Delete = Update
            End If
            If Thread.CurrentPrincipal.IsInRole("CSG LTH Administrator") Then
                Create = Update
                Delete = Update
            End If
        End Sub
#End Region

    End Class
End Namespace