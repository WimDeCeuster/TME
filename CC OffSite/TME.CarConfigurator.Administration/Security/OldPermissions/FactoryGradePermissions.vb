Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class FactoryGradePermissions
        Inherits Permissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            If context.IsGlobal() Then
                If Thread.CurrentPrincipal.IsInRole("BASE Administrator") OrElse Thread.CurrentPrincipal.IsInRole("ISG Administrator") Then
                    Create = True
                    Update = True
                    Delete = True
                    Activate = True
                    Approve = True
                    Sort = False
                    ViewAssets = True
                End If
            End If

            ViewAssets = context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")
            UsesNonVATPrice = context.UsesNonVATPrice
        End Sub
#End Region

    End Class
End Namespace