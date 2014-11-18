Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class EquipmentGroupsPermissions
        Inherits Permissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            If context.IsGlobal() Then
                If Thread.CurrentPrincipal.IsInRole("CSG Administrator") Then
                    Create = True
                    Update = True
                    Delete = True
                    Activate = True
                    Approve = True
                    Sort = True
                    ViewAssets = True
                End If
            Else
                If context.IsEurope() Then
                    Price = False
                Else
                    Price = Thread.CurrentPrincipal.IsInRole("NMSC Administrator")
                End If
                ViewAssets = context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")
                If Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator") Then
                    Approve = True
                End If
            End If
            UsesNonVATPrice = context.UsesNonVATPrice
        End Sub
#End Region

    End Class
End Namespace