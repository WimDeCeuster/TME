Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.ReadOnly
    Public Class CarPermissions
        Inherits Permissions
        Implements ICarPermissions

        Public ReadOnly Property ApproveForFinance() As Boolean Implements ICarPermissions.ApproveForFinance
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property AssignSubModel() As Boolean Implements ICarPermissions.AssignSubModel
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Overrule() As Boolean Implements ICarPermissions.Overrule
            Get
                Return False
            End Get
        End Property

        Public ReadOnly Property Promote() As Boolean Implements ICarPermissions.Promote
            Get
                Return False
            End Get
        End Property
    End Class
End Namespace