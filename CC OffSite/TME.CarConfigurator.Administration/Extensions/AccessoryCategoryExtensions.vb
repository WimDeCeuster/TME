Imports System.Runtime.CompilerServices

Namespace Extensions
    Public Module AccessoryCategoryExtensions
        <Extension>
        Public Function IsDummyRoot(ByVal category As AccessoryCategory) As Boolean
            Return category.ID.Equals(Guid.Empty)
        End Function
    End Module
End Namespace