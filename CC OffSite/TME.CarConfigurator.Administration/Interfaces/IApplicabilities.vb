Namespace Interfaces
    Public Interface IApplicabilities
        Inherits IEnumerable

        Property Parent As IUniqueGuid
        ReadOnly Property Count As Integer

        Function Contains(ByVal id As Guid) As Boolean
        Function GetItem(ByVal id As Guid) As IApplicability
        Sub Remove(ByVal id As Guid)
        Sub Clear()
    End Interface
End NameSpace