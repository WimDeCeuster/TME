Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base

Namespace Security.OldPermissions
    <Serializable()> Public Class FeaturesPermissions
        Inherits Permissions

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            Create = True
            Update = True
            Delete = True
            ViewAssets = context.CanViewAssets OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator")
            UsesNonVATPrice = context.UsesNonVATPrice
        End Sub
#End Region

    End Class
End Namespace