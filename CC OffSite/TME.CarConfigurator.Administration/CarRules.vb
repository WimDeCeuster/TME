Imports TME.CarConfigurator.Administration.Enums

<Serializable()>
Public MustInherit Class CarRules
    Inherits ContextUniqueGuidListBase(Of CarRules, CarRule)

#Region "Business Properties & Methods"

    Private _inheritedRules As ModelGenerationRules

    Protected Friend MustOverride ReadOnly Property Car() As Car
   
    Friend Property InheritedRules As ModelGenerationRules
        Get
            Return _inheritedRules
        End Get
        Set(ByVal value As ModelGenerationRules)
            _inheritedRules = value
            With _inheritedRules
                AddHandler .RuleAdded, AddressOf OnModelGenerationRuleAdded
                AddHandler .RuleChanged, AddressOf OnModelGenerationRuleChanged
                AddHandler .RuleRemoved, AddressOf OnModelGenerationRuleRemoved
            End With
            SyncRules()
        End Set
    End Property
    
    Private Sub SyncRules()
        For Each modelGenerationRule In InheritedRules
            SyncRule(modelGenerationRule)
        Next
    End Sub

    Private Sub SyncRule(ByVal modelGenerationRule As ModelGenerationRule)
        'check if we have this rule already
        Dim rule = Item(modelGenerationRule.ID)

        'if the rule doesn't exist on car level yet, add it here so it can be overwritten later
        If rule Is Nothing Then
            AddRuleToCarLevel(modelGenerationRule)
            Return
        End If

        'otherwise, if the rule already exist on car level
        'if it isn't overwritten, revert it (it is now defined on higher level)
        If Not rule.Overwritten Then
            rule.MatchInheritedRule()
            Return
        End If

        'it is overwritten on car level already (and so already marked as inherited)

        'if it is cleared, keep it cleared but update the ruletype 
        If rule.Cleared Then
            rule.MatchInheritedRule()
            Return
        End If

        'otherwise, the rule is not cleared

        'if the rule's ruletype is not the same as the new ruletype of the model generation rule, do nothing
        If rule.RuleType <> modelGenerationRule.Type Then Return

        If TypeOf rule Is IColouringModesRule Then
            If DirectCast(rule, IColouringModesRule).ColouringMode <> DirectCast(modelGenerationRule, IColouringModesRule).ColouringMode Then Return
        End If

        
        'otherwise, if the ruletype is now the same as the one on model generation level, the rule isn't overwritten anymore
        rule.Revert()
    End Sub

    Private Sub AddRuleToCarLevel(ByVal modelGenerationRule As ModelGenerationRule)
        Dim inheritedCarRule = CarRule.GetCarRule(modelGenerationRule)
        MyBase.Add(inheritedCarRule)
    End Sub



    Public Shadows Function Add(ByVal id As Guid, ByVal type As RuleType, Optional calculated As Boolean = False, Optional colouringModes As ColouringModes = ColouringModes.None) As CarRule
        Dim newRule As CarRule
        If Car.Packs.Contains(id) Then
            newRule = Add(Car.Packs(id), type, calculated)
        Else
            newRule = Add(Car.Equipment(id), type, calculated, colouringModes)
        End If
        Return newRule
    End Function

    Public Shadows Function Add(ByVal id As Guid, ByVal type As RuleType, ByVal colouringModes As ColouringModes, Optional calculated As Boolean = False) As CarRule
        Dim newRule As CarRule
        If Car.Packs.Contains(id) Then
            newRule = Add(Car.Packs(id), type, calculated)
        Else
            newRule = Add(Car.Equipment(id), type, colouringModes, calculated)
        End If
        Return newRule
    End Function

    Public Shadows Function Add(ByVal carEquipmentItem As CarEquipmentItem, ByVal ruleType As RuleType, Optional calculated As Boolean = False, Optional colouringModes As ColouringModes = ColouringModes.None) As CarRule
        'if the rule exists already, just return that one instead of adding another one for the same equipment item
        If Contains(carEquipmentItem.ID) Then Return Item(carEquipmentItem.ID)

        Dim rule = CarEquipmentItemRule.NewRule(carEquipmentItem, ruleType, colouringModes)
        rule.Calculated = calculated
        MyBase.Add(rule)
        Return rule
    End Function

    Public Shadows Function Add(ByVal carEquipmentItem As CarEquipmentItem, ByVal ruleType As RuleType, ByVal colouringModes As ColouringModes, Optional calculated As Boolean = False) As CarRule
        'if the rule exists already, just return that one instead of adding another one for the same equipment item
        If Contains(carEquipmentItem.ID) Then Return Item(carEquipmentItem.ID)

        Dim rule = CarEquipmentItemRule.NewRule(carEquipmentItem, ruleType, colouringModes)
        rule.Calculated = calculated
        MyBase.Add(rule)
        Return rule
    End Function

    Public Shadows Function Add(ByVal carPack As CarPack, ByVal ruleType As RuleType, Optional calculated As Boolean = False) As CarRule
        'if the rule exists already, just return that one instead of adding another one for the same equipment item
        If Contains(carPack.ID) Then Return Item(carPack.ID)

        Dim rule = CarPackRule.NewRule(carPack, ruleType)
        rule.Calculated = calculated
        MyBase.Add(rule)
        Return rule
    End Function
    Friend Shadows Function Add(ByVal dataReader As SafeDataReader) As CarRule
        Dim rule = CarRule.GetObject(dataReader)
        MyBase.Add(rule)
        Return rule
    End Function

    Public Overloads Sub RemoveAt(ByVal index As Integer)
        Remove(Item(index))
    End Sub
    Public Shadows Sub Remove(ByVal id As Guid)
        Remove(Item(id))
    End Sub
    Public Shadows Sub Remove(ByVal carRule As CarRule)
        If Not carRule.Inherited Then
            MyBase.Remove(carRule)
            Return
        End If
        If Not carRule.Overwritten Then carRule.Overwrite()
        carRule.MatchInheritedRule()
        carRule.Cleared = True
    End Sub
#End Region

#Region "Constructors"
    Protected Sub New()
        'prevent direct creation
    End Sub
#End Region

#Region "Delegates & Event Handlers"
    Private Sub OnModelGenerationRuleAdded(ByVal mgRule As ModelGenerationRule)
        Dim carRule = Item(mgRule.ID)

        'if the rule doesn't exist yet, or it is the same, just sync up
        If carRule Is Nothing OrElse (carRule.SameValuesAs(mgRule)) Then
            SyncRule(mgRule)
            Return
        End If

        'otherwise, mark as overwrite
        carRule.MarkOverwriten()
    End Sub

    Private Sub OnModelGenerationRuleChanged(ByVal rule As ModelGenerationRule)
        SyncRule(rule)
    End Sub

    Private Sub OnModelGenerationRuleRemoved(ByVal mgRule As ModelGenerationRule)
        Dim carRule = Item(mgRule.ID)

        'if it is overwritten and has a different type, keep it
        If carRule.Overwritten AndAlso Not carRule.SameValuesAs(mgRule) Then
            carRule.MarkNotOverwritten()
            Return
        End If

        'otherwise, remove it
        Item(mgRule.ID).MarkForRemoval() 'make sure the rule can be removed now
        MyBase.Remove(mgRule.ID) 'remove it
    End Sub
#End Region
End Class