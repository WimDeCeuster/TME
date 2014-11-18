Namespace Exceptions
    <Serializable()> Public Class ModelNeedsToBeApproved
        Inherits InvalidOperationException
        Public Sub New()
            MyBase.New("The model needs to be approved in order to execute this operation")
        End Sub
        Public Sub New(ByVal message As String)
            MyBase.New(message)
        End Sub
        Public Sub New(ByVal message As String, ByVal innerException As Exception)
            MyBase.New(message, innerException)
        End Sub
        Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
            MyBase.New(info, context)
        End Sub
    End Class
End NameSpace