Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module MasterPathExtensions
        <Extension()> Public Function ContainsMasterPath(Of T As IMasterPathObjectReference)(ByVal list As IEnumerable(Of T), ByVal path As String, ByVal id As Guid) As Boolean
            Return list.Any(Function(item) Equals(item.MasterID, id) AndAlso Equals(item.MasterPath, path))
        End Function

        <Extension()> Public Function GetByMasterPath(Of T As IMasterPathObjectReference)(ByVal list As IEnumerable(Of T), ByVal path As String, ByVal id As Guid) As T
            Return list.SingleOrDefault(Function(item) Equals(item.MasterID, id) AndAlso Equals(item.MasterPath, path))
        End Function

        <Extension()> Public Function GetChildrenByMasterPath(Of T As IMasterPathObjectReference)(ByVal list As IEnumerable(Of T), ByVal path As String) As IEnumerable(Of T)
            Return list.Where(Function(item) item.MasterPath.StartsWith(path) Or Equals(item.MasterPath, path))
        End Function
    End Module
End Namespace