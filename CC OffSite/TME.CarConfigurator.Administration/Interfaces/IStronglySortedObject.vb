Namespace Interfaces
    Public Interface IStronglySortedObject
        Inherits ISortedIndex

        Function CanMoveDown() As Boolean
        Function CanMoveUp() As Boolean
        Function MoveDown() As Boolean
        Function MoveUp() As Boolean
    End Interface
End Namespace
