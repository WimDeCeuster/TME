Namespace BaseObjects
    <Serializable()> Public MustInherit Class StronglySortedListBase(Of T As StronglySortedListBase(Of T, C), C As {Templates.SqlServer.Specialized.Core.UniqueGuidBusinessBase, ISortedIndex})
        Inherits ContextUniqueGuidListBase(Of T, C)

        Public Overridable Function CanMoveDown(ByVal obj As C) As Boolean
            If Not CanMove(obj) Then Return False
            Return (Me.IndexOf(obj) < (Me.Count - 1))
        End Function
        Public Overridable Function CanMoveUp(ByVal obj As C) As Boolean
            If Not CanMove(obj) Then Return False
            Return (Me.IndexOf(obj) > 0)
        End Function
        Protected Overridable Function CanMove(ByVal obj As C) As Boolean
            If Not obj.AllowEdit Then Return False
            If Not obj.CanWriteProperty("Index") Then Return False
            Return True
        End Function
        Public Overridable Function MoveDown(ByVal obj As C) As Boolean
            If CanMoveDown(obj) Then
                Dim _sourceIndex As Integer = Me.IndexOf(obj)
                Me.Swap(_sourceIndex, (_sourceIndex + 1))
                Return True
            Else
                Return False
            End If
        End Function
        Public Overridable Function MoveUp(ByVal obj As C) As Boolean
            If CanMoveUp(obj) Then
                Dim _sourceIndex As Integer = Me.IndexOf(obj)
                Me.Swap(_sourceIndex, (_sourceIndex - 1))
                Return True
            Else
                Return False
            End If
        End Function

        Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
            Get
                Return True
            End Get
        End Property

        Protected Overrides Sub OnListChanged(ByVal e As System.ComponentModel.ListChangedEventArgs)
            If Not Me.AllowEdit Then Exit Sub

            If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
                DirectCast(Me(e.NewIndex), ISortedIndexSetter).SetIndex = e.NewIndex
            ElseIf e.ListChangedType = ComponentModel.ListChangedType.ItemMoved Then
                DirectCast(Me(e.NewIndex), ISortedIndexSetter).SetIndex = e.NewIndex
            ElseIf e.ListChangedType = ComponentModel.ListChangedType.ItemDeleted Then
                For i As Integer = e.NewIndex To (Me.Count - 1) Step 1
                    DirectCast(Me(i), ISortedIndexSetter).SetIndex = i
                Next
            End If
            MyBase.OnListChanged(e)
        End Sub
    End Class

    Friend Interface ISortedIndexSetter
        WriteOnly Property SetIndex() As Integer
    End Interface
    Public Interface ISortedIndex
        ReadOnly Property Index() As Integer
    End Interface
End Namespace