Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.BaseObjects

<Serializable()>
Public MustInherit Class CarRule
    Inherits ContextUniqueGuidBusinessBase(Of CarRule)
    Implements IRule
    Implements IOverwritable

#Region "Business properties & methods"

    Private _ruleType As RuleType
    Private _name As String
    Private _category As RuleCategory
    Private _cleared As Boolean
    Private _overwritten As Boolean
    Private _inherited As Boolean?
    Private _calculated As Boolean
    
    Public ReadOnly Property Name() As String Implements IRule.Name
        Get
            Return _name
        End Get
    End Property

    Public Property RuleType() As RuleType Implements IRule.Type
        Get
            Return _ruleType
        End Get
        Set(ByVal value As RuleType)
            If _ruleType = value OrElse (Inherited AndAlso Not Overwritten) Then Return
            _ruleType = value
            PropertyHasChanged("RuleType")
        End Set
    End Property

    Public Property Category() As RuleCategory Implements IRule.Category
        Get
            Return _category
        End Get
        Set(ByVal value As RuleCategory)
            If _category <> value Then
                _category = value
                PropertyHasChanged("Category")
            End If
        End Set
    End Property

    Public MustOverride ReadOnly Property AllowedCategories() As IEnumerable(Of RuleCategory) Implements IRule.AllowedCategories

    Public Property Cleared As Boolean
        Get
            Return _cleared
        End Get
        Set(ByVal value As Boolean)
            If _cleared = value OrElse Overwritten = False Then Return
            _cleared = value
            PropertyHasChanged("Cleared")
        End Set
    End Property

    Public Property Overwritten As Boolean
        Get
            Return _overwritten AndAlso Inherited
        End Get
        Private Set(ByVal value As Boolean)
            If _overwritten = value Then Return
            _overwritten = value

            AllowEdit = True 'set true so that it can be changed internally
            PropertyHasChanged("Overwritten")
            AllowEdit = _overwritten 'keep this statement underneath property changed! otherwise, it breaks!
        End Set
    End Property

    Public ReadOnly Property Inherited As Boolean 'true if it was defined on a higher level, false if it is added here
        Get
            If _inherited IsNot Nothing Then Return CType(_inherited, Boolean)

            _inherited = DirectCast(Parent, CarRules).InheritedRules.Contains(ID)

            Return CType(_inherited, Boolean)
        End Get
    End Property

    Friend Sub MatchInheritedRule()
        _inherited = Nothing 'make sure the inherited attribute will be recalculated
        'revert properties to match inherited rule
        Dim modelGenerationRule = (DirectCast(Parent, CarRules).InheritedRules(ID))
        MatchInheritedRule(modelGenerationRule)
    End Sub

    Protected Overridable Sub MatchInheritedRule(ByVal modelGenerationRule As ModelGenerationRule)
        _ruleType = modelGenerationRule.Type
        _category = modelGenerationRule.Category
        _overwritten = False
    End Sub


    Friend Sub MarkForRemoval()
        AllowRemove = True
    End Sub

    Friend Sub MarkNotOverwritten()
        Overwritten = False
        _inherited = Nothing 'make sure the inherited attribute will be recalculated
    End Sub

    Friend Sub MarkOverwriten()
        Overwritten = True
        _inherited = True
    End Sub

    Public Function HasBeenOverwritten() As Boolean Implements IOverwritable.HasBeenOverwritten
        Return Overwritten
    End Function
    Public Sub Overwrite() Implements IOverwritable.Overwrite
        If Inherited Then Overwritten = True
    End Sub

    Public Sub Revert() Implements IOverwritable.Revert
        If Not Overwritten Then Return
        Cleared = False
        Overwritten = False 'set overwritten to false => rule just changed to be the same as parent, so it isn't overwritten
        MatchInheritedRule()
    End Sub

    Public Property Calculated As Boolean
        Get
            Return _calculated
        End Get
        Set(ByVal value As Boolean)
            If _calculated = value Then Return
            _calculated = value
            PropertyHasChanged("Calculated")
        End Set
    End Property


    Friend Overridable Function SameValuesAs(ByVal modelGenerationRule As ModelGenerationRule) As Boolean
        Return (modelGenerationRule.Type = RuleType)
    End Function


#End Region

#Region "System.Object overrides"
    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
#End Region

#Region "Business & Validation Rules"

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf CategoryValidationRule, Validation.RuleHandler), "Category")
    End Sub

    Private Shared Function CategoryValidationRule(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim rule As IRule = DirectCast(target, IRule)
        e.Description = String.Format("The category {0} is not allowed for the rule towards {1}", rule.Category, rule.Name)

        Return rule.AllowedCategories.Contains(rule.Category)
    End Function

#End Region

#Region "Shared factory methods"
    Friend Shared Function GetCarRule(ByVal modelGenerationRule As ModelGenerationRule) As CarRule
        Dim rule As CarRule

        If TypeOf modelGenerationRule Is ModelGenerationEquipmentRule Then
            rule = CarEquipmentItemRule.GetRule(modelGenerationRule)
        ElseIf TypeOf modelGenerationRule Is ModelGenerationPackRule Then
            rule = CarPackRule.GetRule(modelGenerationRule)
        Else
            Throw New ArgumentException(String.Format("Type of modelGenerationRule parameter ({0}) is not supported", modelGenerationRule.GetType()))
        End If

        Return rule
    End Function
#End Region

#Region "Constructors"
    Protected Sub New()
        'prevent direct creation
    End Sub
#End Region

#Region " Data Access "


    Protected Overloads Sub Create(ByVal objId As Guid, ByVal objName As String, ByVal objCategory As RuleCategory, ByVal type As RuleType)
        Create(objId)
        _name = objName
        _category = objCategory
        _ruleType = type
        ValidationRules.CheckRules()
    End Sub

    Public Shared Function GetObject(ByVal dataReader As SafeDataReader) As CarRule
        'get the entity
        'based on the entity, get the correct type of rule
        With dataReader
            Dim ruleEntity = .GetEntity("ENTITY")
            Dim rule As CarRule

            Select Case ruleEntity
                Case Entity.EQUIPMENT
                    rule = CarEquipmentItemRule.GetRule(dataReader)
                    rule.Fetch(dataReader)
                    Return rule
                Case Entity.PACK
                    rule = CarPackRule.GetRule(dataReader)
                    rule.Fetch(dataReader)
                    Return rule
                Case Else
                    Throw New Exceptions.InvalidEquipmentType("""" & ruleEntity & """ is not a valid rule entity!")
            End Select
        End With
    End Function


    Protected Overridable Overloads Sub Fetch(ByVal modelGenerationRule As ModelGenerationRule)
        ID = modelGenerationRule.ID
        _name = modelGenerationRule.Name
        _ruleType = modelGenerationRule.Type
        _category = modelGenerationRule.Category
        AllowEdit = False
        MarkOld()
    End Sub


    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        With dataReader
            _name = .GetString("SHORTNAME")
            _ruleType = CType(.GetValue("RULETYPE"), RuleType)
            _category = CType(.GetValue("RULECATEGORY"), RuleCategory)

            _cleared = dataReader.GetBoolean(GetFieldName("CLEARED"))
            _calculated = dataReader.GetBoolean(GetFieldName("CALCULATED"))
            _overwritten = True
        End With
        AllowEdit = True
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If Inherited AndAlso Not Overwritten Then
            command.CommandText = command.CommandText.Replace("update", "delete")
            Return
        End If

        AddCommandFields(command)
    End Sub

    Protected Overridable Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@RULETYPE", RuleType)
        command.Parameters.AddWithValue("@RULECATEGORY", Category)
        command.Parameters.AddWithValue("@CLEARED", Cleared)
        command.Parameters.AddWithValue("@CALCULATED", Calculated)
    End Sub

#End Region


End Class

<Serializable()>
Public Class CarEquipmentItemRule
    Inherits CarRule
    Implements IColouringModesRule

#Region "Business Properties & Methods"

    Private Shared ReadOnly AllowCategories As List(Of RuleCategory) = New List(Of RuleCategory) From {RuleCategory.PRODUCT, RuleCategory.MARKETING, RuleCategory.VISUAL}
    Public Overrides ReadOnly Property AllowedCategories() As IEnumerable(Of RuleCategory)
        Get
            Return AllowCategories
        End Get
    End Property

    Private _colouringMode As ColouringModes
    Public Property ColouringMode() As ColouringModes Implements IColouringModesRule.ColouringMode
        Get
            Return _colouringMode
        End Get
        Set(ByVal value As ColouringModes)
            If _colouringMode = value OrElse (Inherited AndAlso Not Overwritten) Then Return

            _colouringMode = value
            PropertyHasChanged("ColouringMode")
        End Set
    End Property
    Public ReadOnly Property Colour() As ExteriorColourInfo Implements IColouringModesRule.Colour
        Get
            Return DirectCast(Parent, CarRules).Car.Generation.Equipment(ID).Colour
        End Get
    End Property


    Protected Overrides Sub MatchInheritedRule(ByVal modelGenerationRule As ModelGenerationRule)
        MyBase.MatchInheritedRule(modelGenerationRule)
        _colouringMode = DirectCast(modelGenerationRule, ModelGenerationEquipmentRule).ColouringMode
    End Sub


    Friend Overrides Function SameValuesAs(ByVal modelGenerationRule As ModelGenerationRule) As Boolean
        If Not MyBase.SameValuesAs(modelGenerationRule) Then Return False
        Return DirectCast(modelGenerationRule, ModelGenerationEquipmentRule).ColouringMode.Equals(ColouringMode)
    End Function
#End Region

#Region "Shared Factory Methods"

    Friend Shared Function GetRule(ByVal dataReader As SafeDataReader) As CarEquipmentItemRule
        Dim rule = New CarEquipmentItemRule()
        rule.Fetch(dataReader)
        Return rule
    End Function

    Friend Shared Function GetRule(ByVal modelGenerationRule As ModelGenerationRule) As CarRule
        Dim rule = New CarEquipmentItemRule()
        rule.Fetch(modelGenerationRule)
        Return rule
    End Function

    Friend Shared Function NewRule(ByVal carEquipmentItem As CarEquipmentItem, ByVal ruleType As RuleType, ByVal colouringModes As ColouringModes) As CarRule
        Dim rule = New CarEquipmentItemRule()
        rule.Create(carEquipmentItem, ruleType, colouringModes)
        Return rule
    End Function

#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
        MarkAsChild() 'necessary to make it removable by its parent
    End Sub
#End Region

#Region " Data Access "

    Private Overloads Sub Create(ByVal obj As CarEquipmentItem, ByVal type As RuleType, ByVal objColouringModes As ColouringModes)
        Create(obj.ID, obj.Name, RuleCategory.PRODUCT, type)
        _colouringMode = objColouringModes
        ValidationRules.CheckRules()
    End Sub

    Protected Overrides Sub Fetch(ByVal modelGenerationRule As ModelGenerationRule)
        MyBase.Fetch(modelGenerationRule)
        _colouringMode = DirectCast(modelGenerationRule, ModelGenerationEquipmentRule).ColouringMode
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        _colouringMode = dataReader.GetColouringModes("COLOURINGMODES")
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddSpecializedCommandFields("insert", command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddSpecializedCommandFields("update", command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddSpecializedCommandFields("delete", command)
    End Sub

    Private Sub AddSpecializedCommandFields(ByVal commandNamePrefix As String, ByVal command As SqlCommand)
        If TypeOf Parent Is CarEquipmentRules Then
            Dim equipmentItem = DirectCast(Parent, CarEquipmentRules).EquipmentItem
            command.CommandText = commandNamePrefix & "CarEquipmentItemItemRule"
            command.Parameters.AddWithValue("@CARID", equipmentItem.Car.ID)
            command.Parameters.AddWithValue("@EQUIPMENT1ID", equipmentItem.ID)
            command.Parameters.AddWithValue("@EQUIPMENT2ID", ID)
        ElseIf TypeOf Parent Is CarPackRules Then
            Dim pack = DirectCast(Parent, CarPackRules).Pack
            command.CommandText = commandNamePrefix & "CarPackEquipmentItemRule"
            command.Parameters.AddWithValue("@CARID", pack.Car.ID)
            command.Parameters.AddWithValue("@PACKID", pack.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", ID)
        End If
    End Sub


    Protected Overrides Sub AddCommandFields(ByVal command As SqlCommand)
        MyBase.AddCommandFields(command)
        command.Parameters.AddWithValue("@COLOURINGMODES", ColouringMode)
    End Sub


#End Region
End Class

<Serializable()>
Public Class CarPackRule
    Inherits CarRule

#Region "Business Properties & Methods"
    Private Shared ReadOnly AllowCategories As List(Of RuleCategory) = New List(Of RuleCategory) From {RuleCategory.MARKETING, RuleCategory.PRODUCT}
    Public Overrides ReadOnly Property AllowedCategories As IEnumerable(Of RuleCategory)
        Get
            Return AllowCategories
        End Get
    End Property
#End Region

#Region "Shared Factory Methods"
    Friend Shared Function GetRule(ByVal modelGenerationRule As ModelGenerationRule) As CarPackRule
        Dim rule = New CarPackRule()
        rule.Fetch(modelGenerationRule)
        Return rule
    End Function

    Friend Shared Function GetRule(ByVal dataReader As SafeDataReader) As CarPackRule
        Dim rule = New CarPackRule()
        rule.Fetch(dataReader)
        Return rule
    End Function

    Friend Shared Function NewRule(ByVal carPack As CarPack, ByVal ruleType As RuleType) As CarRule
        Dim rule = New CarPackRule()
        rule.Create(carPack, ruleType)
        Return rule
    End Function
#End Region

#Region "Constructors"
    Private Sub New()
        ' Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region "Data Access"

    Private Overloads Sub Create(ByVal obj As CarPack, ByVal type As RuleType)
        Create(obj.ID, obj.Name, RuleCategory.PRODUCT, type)
        ValidationRules.CheckRules()
    End Sub
    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddSpecializedCommandFields("insert", command)
    End Sub

    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddSpecializedCommandFields("update", command)
    End Sub

    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As SqlCommand)
        AddSpecializedCommandFields("delete", command)
    End Sub

    Private Sub AddSpecializedCommandFields(ByVal commandNamePrefix As String, ByVal command As SqlCommand)
        If TypeOf Parent Is CarEquipmentRules Then
            Dim equipmentItem = DirectCast(Parent, CarEquipmentRules).EquipmentItem
            command.CommandText = commandNamePrefix & "CarEquipmentItemPackRule"
            command.Parameters.AddWithValue("@CARID", equipmentItem.Car.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", equipmentItem.ID)
            command.Parameters.AddWithValue("@PACKID", ID)
        ElseIf TypeOf Parent Is CarPackRules Then
            Dim pack = DirectCast(Parent, CarPackRules).Pack
            command.CommandText = commandNamePrefix & "CarPackPackRule"
            command.Parameters.AddWithValue("@CARID", pack.Car.ID)
            command.Parameters.AddWithValue("@PACK1ID", pack.ID)
            command.Parameters.AddWithValue("@PACK2ID", ID)
        End If
    End Sub
#End Region

End Class