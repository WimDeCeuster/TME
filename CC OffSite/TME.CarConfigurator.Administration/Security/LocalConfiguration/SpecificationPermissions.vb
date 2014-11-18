Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.LocalConfiguration
    Public Class SpecificationPermissions
        Inherits Permissions
        Implements ISpecificationPermissions

        Public ReadOnly Property SetCompare() As Boolean Implements ISpecificationPermissions.SetCompare
            Get
                Return False
            End Get
        End Property
    End Class
End NameSpace