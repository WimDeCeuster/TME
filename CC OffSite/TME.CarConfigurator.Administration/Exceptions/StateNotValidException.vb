Namespace Exceptions
    Friend Class StateNotValidException
        Inherits Exception

        Public Sub New(ByVal message As String)
            MyBase.New(Message)
        End Sub
    End Class
End NameSpace