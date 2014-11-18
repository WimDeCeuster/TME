Imports TME.CarConfigurator.Administration.Enums

Namespace Exceptions
    Public Class RuleEntityNotSupportedException
        Inherits Exception

        Public Sub New()
            MyBase.New("This rule entity is not supported")
        End Sub

        Public Sub New(ByVal entity As Entity)
            MyBase.New(String.Format("The entity {0} is not supported", entity.ToString()))
        End Sub
    End Class
End Namespace