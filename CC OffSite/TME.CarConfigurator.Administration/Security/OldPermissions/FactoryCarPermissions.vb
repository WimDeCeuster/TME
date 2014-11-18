Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class FactoryCarPermissions
        Inherits Permissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            If context.IsGlobal() Then
                If Thread.CurrentPrincipal.IsInRole("CORE Administrator") Then
                    Create = True
                    Update = True
                    Delete = True
                    Activate = True
                    Approve = True
                    Sort = False
                    ViewAssets = False
                End If
            End If
            UsesNonVATPrice = context.UsesNonVATPrice
        End Sub
#End Region

    End Class
End Namespace