Namespace Exceptions
  <Serializable()> Public Class InvalidFactoryOption
    Inherits SystemException

    Public Sub New()
      MyBase.New("The factory option is invalid")
    End Sub
    Public Sub New(ByVal value As FactoryGenerationOptionValue, ByVal suffix As Suffix)
            MyBase.New(String.Format("The option value {0} - {1} (ssn={2}) and the suffix {3} (ssn={4}) do not belong to the same SSN.", value.Option.FactoryMasterSpec.Description, value.FactoryMasterSpecValue.Description, value.Option.FactoryGeneration.SSN, suffix.ToString(), suffix.FactoryCar.FactoryGeneration.SSN))
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
