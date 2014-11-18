Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.ReadOnly

    Public Class ModelPermissions
        Inherits Permissions
        Implements IModelPermissions

        Public ReadOnly Property Promote() As Boolean Implements IModelPermissions.Promote
            Get
                Return False
            End Get
        End Property
    End Class
End Namespace