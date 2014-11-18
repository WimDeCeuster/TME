Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Exceptions
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class EquipmentRules
    Inherits ContextUniqueGuidListBase(Of EquipmentRules, Rule)

#Region " Business Properties & Methods "

    <NotUndoable()> <NonSerialized()> Private _group As EquipmentGroup = Nothing
    Friend Property FetchingData() As Boolean

#Region "Parent Objects"

    Friend ReadOnly Property Group() As EquipmentGroup
        Get
            'no parent => no group
            If Parent Is Nothing Then Return Nothing

            'if group already found before => return it
            If _group IsNot Nothing Then Return _group

            ' -- start trying to find the group

            'if parent = accessorycategory => no group
            If AccessoryCategory IsNot Nothing Then Return Nothing

            'if parent = equipment group, return that as the group
            _group = TryCast(Parent, EquipmentGroup)
            If _group IsNot Nothing Then Return _group

            'if no equipmentItem => no group
            If EquipmentItem Is Nothing Then Return Nothing

            'use equipment item's parent as the group, if it exists
            _group = TryCast(EquipmentItem.GroupOrCategory, EquipmentGroup)
            If _group IsNot Nothing Then Return _group

            'if equipmentitem doesn't have a group, then group = nothing
            If EquipmentItem.Group Is Nothing Then Return Nothing

            'when all else fails, try to get the group from the context
            _group = MyContext.GetContext().EquipmentGroups.Find(EquipmentItem.Group.ID)

            Return _group
        End Get
    End Property

    Friend ReadOnly Property AccessoryCategory() As AccessoryCategory
        Get
            If Parent Is Nothing Then Return Nothing
            Return TryCast(Parent, AccessoryCategory)
        End Get
    End Property

    Friend ReadOnly Property EquipmentItem() As EquipmentItem
        Get
            If Parent Is Nothing Then Return Nothing
            Return TryCast(Parent, EquipmentItem)
        End Get
    End Property

#End Region

#Region "Add Rules"

    Public Overloads Function Add(ByVal targetEquipmentItem As EquipmentItem, ByVal ruleType As RuleType) As EquipmentItemRule
        If AccessoryCategory IsNot Nothing AndAlso targetEquipmentItem.Type <> EquipmentType.Accessory Then 'accessory categories can only contain rules towards accessories, not options etc.
            Throw New RuleNotSupportedException(Entity.ACCESSORYCATEGORY, targetEquipmentItem.Type)
        End If

        Dim newRule = GetNewEquipmentItemRuleBasedOnParent(targetEquipmentItem, ruleType)

        ThrowExceptionIfRuleExistsAlready(newRule)

        ' the rule already exists, but not with the same type (validated above) 
        ' => remove it to add the new rule (of the correct type)
        If Contains(newRule.ID) Then Remove(newRule.ID)

        MyBase.Add(newRule)
        Return newRule
    End Function

    Public Overloads Function Add(ByVal targetGroup As EquipmentGroup, ByVal ruleType As RuleType) As EquipmentGroupRule
        If AccessoryCategory IsNot Nothing Then Throw New RuleNotSupportedException(Entity.ACCESSORYCATEGORY, Entity.EQUIPMENTGROUP)

        Dim newRule = GetNewEquipmentGroupRuleBasedOnParent(targetGroup, ruleType)

        ThrowExceptionIfRuleExistsAlready(newRule)

        ' the rule already exists, but not with the same type (validated above) 
        ' => remove it (and all lower level rules) to add the new rule (of the correct type)
        If Any(Function(x) x.ID = newRule.ID AndAlso x.OwnerType = newRule.OwnerType) Then
            Remove(newRule.ID)
            RemoveAllSameLowerLevelRules(newRule)
        End If

        MyBase.Add(newRule)
        Return newRule
    End Function

    Private Overloads Sub Add(ByVal rule As Rule)
        'Check to see if I already have the rule
        If Contains(rule.ID) Then
            If Not Item(rule.ID).Type.Equals(rule.Type) Then Return 'The existing rule is of another type, so it was overruled... => Don't add the new rule....
            'The existing rule is of the same type, so remove this...(in order to clean up db)
            Remove(rule.ID)
        End If

        'Check to see if I already have a higher-level variant of the rule
        'If I do then no need to add this rule, since the higher-level rule already covers this...
        If EquipmentGroupTreeContainsRuleOnHigherLevel(rule) Then Return

        Dim newRule As Rule

        If TypeOf rule Is EquipmentItemRule Then
            newRule = EquipmentItemRule.GetRule(DirectCast(rule, EquipmentItemRule))
        Else
            newRule = EquipmentGroupRule.GetRule(DirectCast(rule, EquipmentGroupRule))
        End If

        MyBase.Add(newRule)
    End Sub

    Private Sub ThrowExceptionIfRuleExistsAlready(ByVal newRule As Rule)
        If Contains(newRule.ID) AndAlso Item(newRule.ID).Type.Equals(newRule.Type) Then Throw New RuleAlreadyDefinedException()
        If EquipmentGroupTreeContainsRuleOnHigherLevel(newRule) Then Throw New RuleAlreadyDefinedException("This rule is already is already defined at a higher level.")
    End Sub

#End Region

#Region "GetRuleBasedOnParent"

    Private Function GetNewEquipmentItemRuleBasedOnParent(ByVal targetEquipmentItem As EquipmentItem, ByVal ruleType As RuleType) As EquipmentItemRule
        If EquipmentItem IsNot Nothing Then Return EquipmentItemRule.NewRule(targetEquipmentItem, ruleType, EquipmentItem)
        If AccessoryCategory IsNot Nothing Then Return EquipmentItemRule.NewRule(DirectCast(targetEquipmentItem, Accessory), ruleType, AccessoryCategory)
        Return EquipmentItemRule.NewRule(targetEquipmentItem, ruleType, Group)
    End Function

    Private Function GetNewEquipmentGroupRuleBasedOnParent(ByVal targetGroup As EquipmentGroup, ByVal ruleType As RuleType) As EquipmentGroupRule
        If EquipmentItem IsNot Nothing Then Return EquipmentGroupRule.NewRule(targetGroup, ruleType, EquipmentItem)
        Return EquipmentGroupRule.NewRule(targetGroup, ruleType, Group)
    End Function

#End Region

#Region "ContainsSameHigherLevelRule"

    
    Private Function EquipmentGroupTreeContainsRuleOnHigherLevel(ByVal rule As Rule) As Boolean
        If Not TypeOf (rule) Is EquipmentItemRule AndAlso Not TypeOf (rule) Is EquipmentGroupRule Then
            Throw New Exception("Can only traverse the EquipmentGroup tree when an EquipmentItemRule or EquipmentGroupRule is created.")
        End If

        Dim equipmentItemRule = TryCast(rule, EquipmentItemRule)

        ' try to find the group of the target equipment item in the subgroups of the current group
        Dim groupOfRule = If(equipmentItemRule IsNot Nothing,
                             Group.Groups.Find(equipmentItemRule.GroupOrCategoryID),
                             Group.Groups.Find(rule.ID).ParentGroup)

        Dim existingRule = GetRulingGroupRule(groupOfRule)

        Return existingRule IsNot Nothing AndAlso existingRule.Type.Equals(rule.Type)
    End Function

    Private Function GetRulingGroupRule(ByVal currentGroup As EquipmentGroup) As EquipmentGroupRule
        If currentGroup Is Nothing Then Return Nothing

        Dim rule = DirectCast(Item(currentGroup.ID), EquipmentGroupRule) ' try to find a grouprule towards the currentGroup
        If rule Is Nothing AndAlso currentGroup.ParentGroup IsNot Nothing Then 'if no rule found, but the current group has a parent, do it for the parent
            rule = GetRulingGroupRule(currentGroup.ParentGroup)
        End If
        Return rule
    End Function

#End Region

#Region "Remove rules"

    Private Sub RemoveAllSameLowerLevelRules(ByVal rule As EquipmentGroupRule)
        'remove all rules towards subgroups of the ownergroup of the newly added rule that have the same ruletype as the newly added rule
        Dim ownerGroup = Group.Groups.Find(rule.OwnerID) 'ownerId will either be the EquipmentGroup containing these rules, or nothing
        RemoveSubGroupRulesWithSameType(ownerGroup, rule.Type)
    End Sub
    Private Sub RemoveSubGroupRulesWithSameType(ByVal parentGroup As EquipmentGroup, ByVal ruleType As RuleType)
        If parentGroup Is Nothing Then Return

        'remove all rules towards subgroups that are of the same type
        For Each subGroup As EquipmentGroup In parentGroup.Groups
            RemoveSubGroupRuleAndAllOfItsSubGroupRulesWithSameType(subGroup, ruleType)
        Next
    End Sub

    Private Sub RemoveSubGroupRuleAndAllOfItsSubGroupRulesWithSameType(ByVal subGroup As EquipmentGroup, ByVal ruleType As RuleType)
        'try to find a rule towards the subgroup
        Dim ruleTowardsSubGroup = DirectCast(Item(subGroup.ID), EquipmentGroupRule)

        'if the rule exists and has the same type, then remove it
        If ruleTowardsSubGroup IsNot Nothing Then
            If Not ruleTowardsSubGroup.Type.Equals(ruleType) Then Return 'if the rule is of another type, don't delete it or any rules on its subgroups
            Remove(ruleTowardsSubGroup)
        End If

        'remove all rules from the subgroups of this subgroup too
        RemoveSubGroupRulesWithSameType(subGroup, ruleType)
    End Sub

#End Region

#Region "List events"

    Private Sub RulesRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        Dim rule As Rule = DirectCast(e.RemovingItem, Rule)
        If Not rule.OwnerType.Equals(Entity.EQUIPMENTGROUP) Then Return

        For Each subGroup As EquipmentGroup In Group.Groups
            subGroup.Rules.Remove(rule.ID)
        Next
    End Sub
    Private Sub RulesListChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType <> ComponentModel.ListChangedType.ItemAdded Then Return

        Dim rule As Rule = Me(e.NewIndex)

        If Not rule.OwnerType.Equals(Entity.EQUIPMENTGROUP) Then Return

        If (TryCast(Me.Parent, EquipmentItem) Is Nothing) Then Return

        For Each subGroup As EquipmentGroup In Group.Groups
            subGroup.Rules.Add(rule)
        Next
    End Sub

#End Region


#End Region

#Region " System.Object overrides "

    Public Overloads Overrides Function ToString() As String
        Dim stringBuilder As Text.StringBuilder = New Text.StringBuilder
        For Each rule As Rule In Me
            stringBuilder.Append(rule.Name)
            stringBuilder.Append(rule.Type.ToString() & System.Environment.NewLine)
        Next
        Return stringBuilder.ToString()
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewEquipmentRules(ByVal item As EquipmentItem) As EquipmentRules
        Dim rules = NewRulesWithParent(item)
        rules.Synchronize()
        Return rules
    End Function
    Friend Shared Function NewEquipmentRules(ByVal group As EquipmentGroup) As EquipmentRules
        Return NewRulesWithParent(group)
    End Function

    Private Shared Function NewRulesWithParent(ByVal ruleParent As Object) As EquipmentRules
        Dim rules As EquipmentRules = New EquipmentRules()
        rules.SetParent(ruleParent)
        Return rules
    End Function

    Friend Shared Function GetEquipmentRules(ByVal item As EquipmentItem) As EquipmentRules
        Dim rules As EquipmentRules = DataPortal.Fetch(Of EquipmentRules)(New CustomCriteria(item))
        rules.SetParent(item)
        rules.Synchronize()
        Return rules
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _id As Guid
        Public Sub New(ByVal item As EquipmentItem)
            _id = item.ID
            CommandText = "getEquipmentItemRules"
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@EQUIPMENTID", _id)
        End Sub
    End Class
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return True
        End Get
    End Property
    Protected Overrides Sub Fetch(ByVal dataReader As Common.Database.SafeDataReader)
        FetchingData = True
        While dataReader.Read()
            FetchRule(dataReader)
        End While
        dataReader.NextResult()
        While dataReader.Read
            FetchRule(dataReader)
        End While
        dataReader.NextResult()
        While dataReader.Read
            FetchRule(dataReader)
        End While
        FetchingData = False
    End Sub
    Friend Sub FetchRule(ByVal dataReader As SafeDataReader)
        FetchingData = True
        Dim rule As Rule = rule.GetEquipmentRule(dataReader)
        If Contains(rule.ID) Then Remove(rule.ID)
        MyBase.Add(rule)
        FetchingData = False
    End Sub

    Friend Sub Synchronize()
        If Group Is Nothing Then Exit Sub

        Dim rulesFromGroup As Hashtable = New Hashtable

        'Add all non-existing rules from the group and take ownership of
        'the same rules
        For Each ruleOfGroup As Rule In Group.Rules

            If Contains(ruleOfGroup.ID) Then
                'This rule already exists in the current collection
                Dim ruleInCollection As Rule = Item(ruleOfGroup.ID)
                If ruleInCollection.OwnerID.Equals(ruleOfGroup.OwnerID) Then
                    'This is exactly the same rule (=same owner)
                    'update the ruletype (might have changed)
                    ruleInCollection.Type = ruleOfGroup.Type
                    rulesFromGroup.Add(ruleOfGroup.ID, Nothing)
                Else
                    'The rule in the collection is owned by someone else
                    If ruleInCollection.Type.Equals(ruleOfGroup.Type) Then
                        'The ruletype is the same, so we remove the existing rule
                        'and take the rule of the group
                        Remove(ruleInCollection)
                        Add(ruleOfGroup)
                        rulesFromGroup.Add(ruleOfGroup.ID, Nothing)
                    Else
                        'The rule is overriden here, so do nothing....
                    End If
                End If
            Else
                Add(ruleOfGroup)
                rulesFromGroup.Add(ruleOfGroup.ID, Nothing)
            End If
        Next

        'Delete all rules that are not set by the group or owned by the equipment item
        For index As Integer = (Count - 1) To 0 Step -1
            Dim ruleInCollection As Rule = Item(index)
            If Not (ruleInCollection.OwnerID.Equals(EquipmentItem.ID)) AndAlso Not (rulesFromGroup.ContainsKey(ruleInCollection.ID)) Then
                Remove(ruleInCollection)
            End If
        Next
    End Sub
#End Region
End Class

<Serializable()> Public MustInherit Class Rule
    Inherits ContextUniqueGuidBusinessBase(Of Rule)
    Implements IRule


#Region " Business Properties & Methods "
    Private _name As String = String.Empty
    Private _ruleType As RuleType
    Private _category As RuleCategory

    Private _ownerID As Guid = Guid.Empty
    Private _ownerType As Entity

    Public ReadOnly Property Name() As String Implements IRule.Name
        Get
            Return _name
        End Get
    End Property
    Public ReadOnly Property OwnerID() As Guid
        Get
            Return _ownerID
        End Get
    End Property
    Friend ReadOnly Property OwnerType() As Entity
        Get
            Return _ownerType
        End Get
    End Property

    Public Property Type() As RuleType Implements IRule.Type
        Get
            Return _ruleType
        End Get
        Set(ByVal value As RuleType)
            If _ruleType = value Then Return

            _ruleType = value
            PropertyHasChanged("Type")
        End Set
    End Property
    Public Property Category() As RuleCategory Implements IRule.Category
        Get
            Return _category
        End Get
        Set(ByVal value As RuleCategory)
            If _category = value Then Return

            _category = value
            PropertyHasChanged("Category")
        End Set
    End Property
    Public MustOverride ReadOnly Property AllowedCategories() As IEnumerable(Of RuleCategory) Implements IRule.AllowedCategories

    Public Overrides Property AllowEdit() As Boolean
        Get
            Return CanEdit()
        End Get
        Protected Set(ByVal value As Boolean)
            MyBase.AllowEdit = value
        End Set
    End Property

    Public Overrides Property AllowRemove() As Boolean
        Get
            Return CanRemove()
        End Get
        Protected Set(ByVal value As Boolean)
            MyBase.AllowRemove = value
        End Set
    End Property

    Private Function CanEdit() As Boolean
        If Parent Is Nothing Then Return False

        With DirectCast(Parent, EquipmentRules)
            If .FetchingData Then Return MyBase.AllowEdit

            If .AccessoryCategory IsNot Nothing Then Return MyBase.AllowEdit AndAlso OwnerID.Equals(.AccessoryCategory.ID)
            If .Group Is Nothing Then Return False
            If .EquipmentItem IsNot Nothing Then Return MyBase.AllowEdit AndAlso OwnerID.Equals(.EquipmentItem.ID)

            Return MyBase.AllowEdit AndAlso OwnerID.Equals(.Group.ID)
        End With
    End Function

    Private Function CanRemove() As Boolean
        If Parent Is Nothing Then Return False

        With DirectCast(Parent, EquipmentRules)
            If .FetchingData Then Return MyBase.AllowRemove

            If .AccessoryCategory IsNot Nothing Then Return MyBase.AllowRemove AndAlso OwnedByCategoryOrOneOfItsParentCategories(.AccessoryCategory)
            If .Group Is Nothing Then Return False
            If .EquipmentItem IsNot Nothing Then Return MyBase.AllowRemove AndAlso OwnedByItemOrOneOfItsParentGroups(.EquipmentItem)

            Return MyBase.AllowRemove AndAlso OwnedByGroupOrOneOfItsParentGroups(.Group)
        End With
    End Function

    Private Function OwnedByCategoryOrOneOfItsParentCategories(ByVal accessoryCategory As AccessoryCategory) As Boolean
        Return accessoryCategory IsNot Nothing AndAlso (OwnerID.Equals(accessoryCategory.ID) OrElse OwnedByCategoryOrOneOfItsParentCategories(accessoryCategory.ParentCategory))
    End Function

    Private Function OwnedByItemOrOneOfItsParentGroups(ByVal item As EquipmentItem) As Boolean
        Return OwnerID.Equals(item.ID) OrElse OwnedByGroupOrOneOfItsParentGroups(TryCast(item.GroupOrCategory, EquipmentGroup))
    End Function

    Private Function OwnedByGroupOrOneOfItsParentGroups(ByVal group As EquipmentGroup) As Boolean
        Return group IsNot Nothing AndAlso (OwnerID.Equals(group.ID) OrElse OwnedByGroupOrOneOfItsParentGroups(group.ParentGroup))
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf CategoryValidationRule, Validation.RuleHandler), "Category")
    End Sub
    Private Shared Function CategoryValidationRule(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim rule As IRule = DirectCast(target, IRule)
        If Not rule.AllowedCategories.Contains(rule.Category) Then
            e.Description = String.Format("The category {0} is not allowed for the rule towards {1}", rule.Category, rule.Name)
            Return False
        End If
        Return True
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return _name
    End Function

    Public Overloads Function Equals(ByVal obj As EquipmentGroup) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is Rule Then
            Return Equals(DirectCast(obj, Rule))
        ElseIf TypeOf obj Is Guid Then
            Return ID.Equals(DirectCast(obj, Guid))
        ElseIf TypeOf obj Is EquipmentGroup Then
            Return ID.Equals(DirectCast(obj, EquipmentGroup).ID)
        ElseIf TypeOf obj Is EquipmentItem Then
            Return ID.Equals(DirectCast(obj, EquipmentItem).ID)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Facory Method "

    Friend Shared Function GetEquipmentRule(ByVal dataReader As SafeDataReader) As Rule
        Select Case dataReader.GetEntity("ENTITY")
            Case Entity.EQUIPMENTGROUP
                Return EquipmentGroupRule.GetRule(dataReader)
            Case Entity.EQUIPMENT
                Return EquipmentItemRule.GetRule(dataReader)
            Case Else
                Throw New IndexItemOutOfRangeException("Unknown target type " & dataReader.GetValue("ENTITY").ToString() & "!")
        End Select
    End Function

#End Region

#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation
        MarkAsChild()
        AutoDiscover = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overloads Sub Create(ByVal objId As Guid, ByVal objName As String, ByVal objCategory As RuleCategory, ByVal idOfOwner As Guid, ByVal typeOfOwner As Entity)
        _name = objName
        _category = objCategory
        _ownerID = idOfOwner
        _ownerType = typeOfOwner
        Create(objId) 'validation rules should only be checked after ownertype is set
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _name = .GetString("SHORTNAME")
            _ruleType = CType(.GetValue("RULETYPE"), RuleType)
            _category = CType(.GetValue("RULECATEGORY"), RuleCategory)
            _ownerID = .GetGuid("OWNERID")
            _ownerType = DirectCast(Entity.Parse(Entity.NOTHING.GetType(), dataReader.GetString("OWNERENTITY")), Entity)
        End With
    End Sub
    Protected Overloads Sub Fetch(ByVal rule As Rule)
        With rule
            ID = .ID
            _name = .Name
            _ruleType = rule.Type
            _ownerID = .OwnerID
            _ownerType = .OwnerType
        End With
        MarkOld()
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'None
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'None
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'None
    End Sub


    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        SetCommand(command, "insert")
        command.Parameters.AddWithValue("@RULETYPE", Type)
        command.Parameters.AddWithValue("@RULECATEGORY", Category)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        SetCommand(command, "update")
        command.Parameters.AddWithValue("@RULETYPE", Type)
        command.Parameters.AddWithValue("@RULECATEGORY", Category)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        SetCommand(command, "delete")
    End Sub

    Protected MustOverride Sub SetCommand(ByVal command As System.Data.SqlClient.SqlCommand, ByVal commandNamePrefix As String)
#End Region

End Class
<Serializable()> Public NotInheritable Class EquipmentItemRule
    Inherits Rule

#Region " Business Properties & Methods "
    <NotUndoable()> Private _groupOrCategoryID As Guid


    Private Shared ReadOnly AllowCategoriesForEquipmentGroupParent As List(Of RuleCategory) = New List(Of RuleCategory) From {RuleCategory.VISUAL}
    Private Shared ReadOnly AllowCategoriesForEquipmentParent As List(Of RuleCategory) = New List(Of RuleCategory) From {RuleCategory.PRODUCT, RuleCategory.VISUAL}
    Private Shared ReadOnly AllowCategoriesForAccessoryCategoryParent As List(Of RuleCategory) = New List(Of RuleCategory) From {RuleCategory.PRODUCT}

    Public Overrides ReadOnly Property AllowedCategories() As IEnumerable(Of RuleCategory)
        Get
            Select Case OwnerType
                Case Entity.EQUIPMENTGROUP
                    Return AllowCategoriesForEquipmentGroupParent
                Case Entity.EQUIPMENT
                    Return AllowCategoriesForEquipmentParent
                Case Entity.ACCESSORYCATEGORY
                    Return AllowCategoriesForAccessoryCategoryParent
            End Select

            Throw New RuleEntityNotSupportedException(OwnerType)
        End Get
    End Property


    Friend Property GroupOrCategoryID() As Guid
        Get
            Return _groupOrCategoryID
        End Get
        Private Set(ByVal value As Guid)
            _groupOrCategoryID = value
        End Set
    End Property

#End Region

#Region " Factory Methods "
    Friend Shared Function NewRule(ByVal item As EquipmentItem, ByVal ruleType As RuleType, ByVal owner As EquipmentGroup) As EquipmentItemRule
        Dim rule As EquipmentItemRule = New EquipmentItemRule()
        rule.Create(item, owner)
        rule.Type = ruleType
        If item.Type = EquipmentType.Accessory Then
            rule.Category = RuleCategory.VISUAL
        Else
            rule.Category = RuleCategory.PRODUCT
        End If
        Return rule
    End Function
    Friend Shared Function NewRule(ByVal item As EquipmentItem, ByVal ruleType As RuleType, ByVal owner As EquipmentItem) As EquipmentItemRule
        Dim rule As EquipmentItemRule = New EquipmentItemRule()
        rule.Create(item, owner)
        rule.Type = ruleType
        If item.Type = EquipmentType.Accessory OrElse owner.Type = EquipmentType.Accessory Then
            rule.Category = RuleCategory.VISUAL
        Else
            rule.Category = RuleCategory.PRODUCT
        End If
        Return rule
    End Function

    Public Shared Function NewRule(ByVal targetAccessory As Accessory, ByVal ruleType As RuleType, ByVal category As AccessoryCategory) As EquipmentItemRule
        Dim rule = New EquipmentItemRule()
        rule.Create(targetAccessory, category)
        rule.Type = ruleType
        rule.Category = RuleCategory.PRODUCT 'accessory categories to accessory: only product rules!
        Return rule
    End Function

    Friend Shared Function GetRule(ByVal rule As EquipmentItemRule) As EquipmentItemRule
        Dim newRule As EquipmentItemRule = New EquipmentItemRule()
        newRule.Fetch(rule)
        newRule.GroupOrCategoryID = rule.GroupOrCategoryID
        Return newRule
    End Function
    Friend Shared Function GetRule(ByVal dataReader As SafeDataReader) As EquipmentItemRule
        Dim rule As EquipmentItemRule = New EquipmentItemRule()
        rule.Fetch(dataReader)
        Return rule
    End Function
#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AutoDiscover = False
    End Sub

#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal item As EquipmentItem, ByVal owner As EquipmentItem)
        Create(item.ID, item.Name, RuleCategory.PRODUCT, owner.ID, Entity.EQUIPMENT)
        GroupOrCategoryID = item.Group.ID
    End Sub
    Private Overloads Sub Create(ByVal item As EquipmentItem, ByVal owner As EquipmentGroup)
        Create(item.ID, item.Name, RuleCategory.VISUAL, owner.ID, Entity.EQUIPMENTGROUP)
        GroupOrCategoryID = item.Group.ID
    End Sub

    Private Overloads Sub Create(ByVal targetAccessory As Accessory, ByVal accessoryCategory As AccessoryCategory)
        Create(targetAccessory.ID, targetAccessory.Name, RuleCategory.PRODUCT, accessoryCategory.ID, Entity.ACCESSORYCATEGORY)
        GroupOrCategoryID = targetAccessory.AccessoryCategory.ID
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        GroupOrCategoryID = dataReader.GetGuid("TARGETGROUPID")
    End Sub

    Protected Overrides Sub SetCommand(ByVal command As System.Data.SqlClient.SqlCommand, ByVal commandNamePrefix As String)
        Select Case OwnerType
            Case Entity.EQUIPMENTGROUP
                command.CommandText = commandNamePrefix & "EquipmentGroupItemRule"
                command.Parameters.AddWithValue("@EQUIPMENTGROUPID", OwnerID)
                command.Parameters.AddWithValue("@EQUIPMENTID", ID)
            Case Entity.EQUIPMENT
                command.CommandText = commandNamePrefix & "EquipmentItemItemRule"
                command.Parameters.AddWithValue("@EQUIPMENT1ID", OwnerID)
                command.Parameters.AddWithValue("@EQUIPMENT2ID", ID)
            Case Entity.ACCESSORYCATEGORY
                command.CommandText = commandNamePrefix & "AccessoryCategoryItemRule"
                command.Parameters.AddWithValue("@ACCESSORYCATEGORYID", OwnerID)
                command.Parameters.AddWithValue("@EQUIPMENTID", ID)
            Case Else
                Throw New RuleEntityNotSupportedException()
        End Select
    End Sub
#End Region
End Class
<Serializable()> Public NotInheritable Class EquipmentGroupRule
    Inherits Rule

#Region " Business Properties & Methods "


    Private Shared ReadOnly AllowCategories As List(Of RuleCategory) = New List(Of RuleCategory) From {RuleCategory.VISUAL}

    Public Overrides ReadOnly Property AllowedCategories() As IEnumerable(Of RuleCategory)
        Get
            Return AllowCategories
        End Get
    End Property

#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Name & " (*)"
    End Function
#End Region

#Region " Factory Methods "
    Friend Shared Function NewRule(ByVal group As EquipmentGroup, ByVal ruleType As RuleType, ByVal owner As EquipmentGroup) As EquipmentGroupRule
        Dim rule As EquipmentGroupRule = New EquipmentGroupRule()
        rule.Create(group, owner)
        rule.Type = ruleType
        Return rule
    End Function
    Friend Shared Function NewRule(ByVal group As EquipmentGroup, ByVal ruleType As RuleType, ByVal owner As EquipmentItem) As EquipmentGroupRule
        Dim rule As EquipmentGroupRule = New EquipmentGroupRule()
        rule.Create(group, owner)
        rule.Type = ruleType
        Return rule
    End Function
    Friend Shared Function GetRule(ByVal rule As EquipmentGroupRule) As EquipmentGroupRule
        Dim newRule As EquipmentGroupRule = New EquipmentGroupRule
        newRule.Fetch(rule)
        Return newRule
    End Function
    Friend Shared Function GetRule(ByVal dataReader As SafeDataReader) As EquipmentGroupRule
        Dim rule As EquipmentGroupRule = New EquipmentGroupRule
        rule.Fetch(dataReader)
        Return rule
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AutoDiscover = False
    End Sub
#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal group As EquipmentGroup, ByVal owner As EquipmentItem)
        Create(group.ID, group.Name, RuleCategory.VISUAL, owner.ID, Entity.EQUIPMENT)
    End Sub
    Private Overloads Sub Create(ByVal group As EquipmentGroup, ByVal owner As EquipmentGroup)
        Create(group.ID, group.Name, RuleCategory.VISUAL, owner.ID, Entity.EQUIPMENTGROUP)
    End Sub

    Protected Overrides Sub SetCommand(ByVal command As System.Data.SqlClient.SqlCommand, ByVal commandNamePrefix As String)
        If OwnerType = Entity.EQUIPMENTGROUP Then
            command.CommandText = commandNamePrefix & "EquipmentGroupGroupRule"
            command.Parameters.AddWithValue("@EQUIPMENTGROUP1ID", OwnerID)
            command.Parameters.AddWithValue("@EQUIPMENTGROUP2ID", ID)
        Else
            command.CommandText = commandNamePrefix & "EquipmentItemGroupRule"
            command.Parameters.AddWithValue("@EQUIPMENTID", OwnerID)
            command.Parameters.AddWithValue("@EQUIPMENTGROUPID", ID)
        End If
    End Sub

#End Region

End Class
