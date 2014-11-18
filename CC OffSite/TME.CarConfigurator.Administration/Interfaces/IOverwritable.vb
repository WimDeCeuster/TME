Namespace Interfaces
    Public Interface IOverwritable
        Function HasBeenOverwritten() As Boolean
        Sub Overwrite()
        Sub Revert()
    End Interface
End Namespace