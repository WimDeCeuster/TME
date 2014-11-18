Imports TME.CarConfigurator.Administration.Security.OldPermissions.Base
Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.OldPermissions
    <Serializable()> Public Class ModelPermissions
        Inherits Permissions
        Implements IModelPermissions

        Private ReadOnly _promote As Boolean

        Public ReadOnly Property Promote() As Boolean Implements IModelPermissions.Promote
            Get
                Return (_promote And False)  'Temporarily deactived.
            End Get
        End Property

#Region " Constructors "
        Friend Sub New(ByVal context As MyContext)
            MyBase.New(context)
            If Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("MKT Administrator") Then
                _promote = Approve
            End If
        End Sub
#End Region

    End Class
End Namespace