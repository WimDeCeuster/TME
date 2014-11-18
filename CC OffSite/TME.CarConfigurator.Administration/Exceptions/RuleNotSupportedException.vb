Imports TME.CarConfigurator.Administration.Enums

Namespace Exceptions
    Public Class RuleNotSupportedException
        Inherits Exception
        Const MessageFormat = "{0} does not support rules towards {1}s."

        Public Sub New(ByVal parentEntity As Entity, ByVal ruleTarget As EquipmentType)
            MyBase.New(String.Format(MessageFormat, parentEntity.GetTitle(), ruleTarget.GetTitle().ToLower()))
        End Sub

        Public Sub New(ByVal parentEntity As Entity, ByVal entity As Entity)
            MyBase.New(String.Format(MessageFormat, parentEntity.GetTitle(), entity.GetTitle().ToLower()))
        End Sub
    End Class
End Namespace