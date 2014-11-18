Imports System.Collections.Generic

Namespace Interfaces
    Public Interface IMasterListObjectReference
        ReadOnly Property MasterCode() As String
        ReadOnly Property MasterIDs() As List(Of Guid)
    End Interface
End Namespace
