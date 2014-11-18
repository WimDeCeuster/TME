Imports TME.CarConfigurator.Administration.Enums

Namespace Exceptions
  <Serializable()> Public Class ObjectAlreadyExists
    Inherits SystemException

    Public Sub New(ByVal entity As Entity)
      MyBase.New(String.Format("This {0} is already available in the current collection.", entity.GetTitle(False)))
    End Sub
    Public Sub New(ByVal entity As Entity, ByVal [object] As Object)
      MyBase.New(String.Format("The {0} ""{1}"" is already available in the current collection.", entity.GetTitle(False), [object].ToString()))
    End Sub
    Public Sub New(ByVal entity As String, ByVal [object] As Object)
      MyBase.New(String.Format("The {0} ""{1}"" is already available in the current collection.", entity, [object].ToString()))
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