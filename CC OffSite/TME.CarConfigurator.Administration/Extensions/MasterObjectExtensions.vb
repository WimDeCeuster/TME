Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module MasterObjectExtensions
        <Extension()> Public Function ContainsMasterCode(ByVal list As IEnumerable(Of IMasterListObjectReference), ByVal code As String) As Boolean
            Return list.Any(Function(item) item.MasterCode.Equals(code))
        End Function

        <Extension()> Public Function ContainsMasterID(ByVal list As IEnumerable(Of IMasterListObjectReference), ByVal code As String, ByVal id As Guid) As Boolean
            Return list.Any(Function(item) item.MasterCode.Equals(code) AndAlso item.MasterIDs.Contains(id))
        End Function

        <Extension()> Public Function ContainsMasterID(ByVal list As IEnumerable(Of IMasterListObjectReference), ByVal id As Guid) As Boolean
            Return list.Any(Function(item) item.MasterIDs.Contains(id))
        End Function
        <Extension()> Public Function GetByOneMasterID(Of T As IMasterListObjectReference)(ByVal list As IEnumerable(Of T), ByVal id As Guid) As T
            Return list.SingleOrDefault(Function(item) item.MasterIDs.Contains(id))
        End Function
        <Extension()> Public Function GetByMasterCode(Of T As IMasterListObjectReference)(ByVal list As IEnumerable(Of T), ByVal code As String) As T
            Return list.SingleOrDefault(Function(item) item.MasterCode.Equals(code))
        End Function



        <Extension()> Public Function ContainsMasterID(ByVal list As IEnumerable(Of IMasterObjectReference), ByVal id As Guid) As Boolean
            Return list.Any(Function(item) item.MasterID.Equals(id))
        End Function

        <Extension()> Public Function GetByMasterID(Of T As IMasterObjectReference)(ByVal list As IEnumerable(Of T), ByVal id As Guid) As T
            Return list.SingleOrDefault(Function(item) item.MasterID.Equals(id))
        End Function

    End Module
End Namespace