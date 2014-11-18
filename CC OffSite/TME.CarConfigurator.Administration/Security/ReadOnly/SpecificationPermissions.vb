Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.ReadOnly
    Public Class SpecificationPermissions
        Inherits LocalConfiguration.Permissions
        Implements ISpecificationPermissions

        Public ReadOnly Property SetCompare() As Boolean Implements ISpecificationPermissions.SetCompare
            Get
                Return False
            End Get
        End Property
    End Class
End Namespace