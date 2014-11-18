Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Interfaces
    Public Interface IRule
        ReadOnly Property Name As String
        Property Type As RuleType
        Property Category() As RuleCategory
        ReadOnly Property AllowedCategories() As IEnumerable(Of RuleCategory)
    End Interface
End Namespace
