Namespace Security.Interface
    Public Interface IPermissions
        Property Activate() As Boolean
        Property Approve() As Boolean
        Property Create() As Boolean
        Property Delete() As Boolean
        Property Preview() As Boolean
        Property Price() As Boolean
        Property Sort() As Boolean
        Property Translate() As Boolean
        Property Update() As Boolean
        Property UsesNonVATPrice() As Boolean
        Property ViewDetails() As Boolean
        Property ViewAssets() As Boolean
    End Interface
End Namespace