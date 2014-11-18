Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class LinkPermissions
        Inherits Permissions


#Region " Constructors "
        Friend Sub New()

            If Thread.CurrentPrincipal.IsInRole("BASE Administrator") OrElse Thread.CurrentPrincipal.IsInRole("ISG Administrator") Then
                Create = True
                Update = True
                Delete = True
                Activate = True
                Approve = True
                Sort = True
                ViewAssets = True
            Else
                Create = False
                Update = False
                Delete = False
                Activate = False
                Approve = False
                Sort = False
                ViewAssets = False
            End If

        End Sub
#End Region

    End Class
End Namespace