Namespace Exceptions
    <Serializable()> Public Class SpecificationValueAlreadyDefinedForLanguageException
        Inherits SystemException

        Public Sub New()
            MyBase.New("There is already a specification value available for this language.")
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
End Namespace