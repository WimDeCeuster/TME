Namespace Security.Interface

    Public Interface ICarPermissions
        Inherits IPermissions

        ReadOnly Property ApproveForFinance() As Boolean
        ReadOnly Property AssignSubModel() As Boolean
        ReadOnly Property Overrule() As Boolean
        ReadOnly Property Promote() As Boolean

    End Interface
End Namespace