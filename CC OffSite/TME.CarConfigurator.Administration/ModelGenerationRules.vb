Imports System.Text
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public MustInherit Class ModelGenerationRules
    Inherits ContextUniqueGuidListBase(Of ModelGenerationRules, ModelGenerationRule)

#Region " Delegates & Events "
    Friend Delegate Sub RuleChangedHandler(ByVal rule As ModelGenerationRule)
    Friend Event RuleAdded As RuleChangedHandler
    Friend Event RuleRemoved As RuleChangedHandler
    Friend Event RuleChanged As RuleChangedHandler

    Protected Overrides Sub OnListChanged(ByVal e As ComponentModel.ListChangedEventArgs)
        MyBase.OnListChanged(e)
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
            RaiseEvent RuleAdded(Me(e.NewIndex))
        End If
    End Sub
    Private Overloads Sub OnRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        RaiseEvent RuleRemoved(DirectCast(e.RemovingItem, ModelGenerationRule))
    End Sub
    Friend Sub RaiseRuleChangedEvent(ByVal rule As ModelGenerationRule)
        RaiseEvent RuleChanged(rule)
    End Sub

#End Region

#Region " Business Properties & Methods "

    Protected Friend MustOverride ReadOnly Property Generation() As ModelGeneration

    Public Overloads Function Add(ByVal id As Guid, ByVal type As RuleType, Optional colouringModes As ColouringModes = ColouringModes.None) As ModelGenerationRule
        Return Add(id, type, colouringModes, Nothing)
    End Function
    Public Overloads Function Add(ByVal id As Guid, ByVal type As RuleType, calculated As Boolean, Optional colouringModes As ColouringModes = ColouringModes.None) As ModelGenerationRule
        Return Add(id, type, colouringModes, calculated)
    End Function
    Private Overloads Function Add(ByVal id As Guid, ByVal type As RuleType, colouringModes As ColouringModes, ByVal calculated As Boolean?) As ModelGenerationRule
        'if the rule already exists, update its type and return it    
        If Contains(id) Then
            Dim existingRule As ModelGenerationRule = Me(id)
            existingRule.Type = type

            If calculated.HasValue Then existingRule.Calculated = calculated.Value

            Dim colouringModesRule = TryCast(existingRule, IColouringModesRule)
            If (colouringModesRule IsNot Nothing) Then
                colouringModesRule.ColouringMode = colouringModes
            ElseIf Not colouringModes = colouringModes.None Then
                Throw New ApplicationException(String.Format("The colouring mode is not support for rules towards {0}", existingRule.Name))
            End If

            Return existingRule
        End If

        Dim newRule As ModelGenerationRule
        If Generation.Packs.Contains(id) Then
            Dim pack = Generation.Packs(id)
            If Not colouringModes = colouringModes.None Then
                Throw New ApplicationException(String.Format("The colouring mode is not support for rules towards {0}", pack.Name))
            End If
            newRule = ModelGenerationPackRule.NewRule(pack, type)
        Else
            newRule = ModelGenerationEquipmentRule.NewRule(Generation.Equipment(id), type, RuleCategory.PRODUCT, colouringModes)
        End If

        If calculated.HasValue Then newRule.Calculated = calculated.Value
        Add(newRule)
        Return newRule

    End Function

    Friend Overloads Function Add(ByVal dataReader As SafeDataReader) As ModelGenerationRule
        Dim rule As ModelGenerationRule = GetObject(dataReader)
        Dim tempAllowNew As Boolean = AllowNew
        RaiseListChangedEvents = False
        AllowNew = True
        Add(rule)
        AllowNew = tempAllowNew
        RaiseListChangedEvents = True
        Return rule
    End Function



#End Region

#Region " System.Object overrides "

    Public Overloads Overrides Function ToString() As String
        Dim stringBuilder = New StringBuilder
        For Each rule As ModelGenerationRule In Me
            stringBuilder.Append(rule.Name)
            stringBuilder.Append(rule.Type.ToString() & System.Environment.NewLine)
        Next
        Return stringBuilder.ToString()
    End Function

#End Region


#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation
        MarkAsChild()
        AllowEdit = Not MyContext.GetContext().IsRegionCountry OrElse MyContext.GetContext().IsMainRegionCountry
        AllowNew = AllowEdit
        AllowRemove = AllowEdit
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As ModelGenerationRule
        Return ModelGenerationRule.GetRule(dataReader)
    End Function
#End Region
End Class
