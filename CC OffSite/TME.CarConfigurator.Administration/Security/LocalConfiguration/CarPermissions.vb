Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security.LocalConfiguration
    Public Class CarPermissions
        Inherits Permissions
        Implements ICarPermissions

        Private ReadOnly _isMasterCountry As Boolean = Not MyContext.GetContext().IsSlaveRegionCountry

        Public ReadOnly Property ApproveForFinance() As Boolean Implements ICarPermissions.ApproveForFinance
            Get
                Return _isMasterCountry
            End Get
        End Property

        Public ReadOnly Property AssignSubModel() As Boolean Implements ICarPermissions.AssignSubModel
            Get
                Return _isMasterCountry
            End Get
        End Property

        Public ReadOnly Property Overrule() As Boolean Implements ICarPermissions.Overrule
            Get
                Return _isMasterCountry
            End Get
        End Property

        Public ReadOnly Property Promote() As Boolean Implements ICarPermissions.Promote
            Get
                Return _isMasterCountry
            End Get
        End Property

        Friend Sub New()
            Approve = _isMasterCountry
            Preview = _isMasterCountry
        End Sub
    End Class
End Namespace