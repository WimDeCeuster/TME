Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public MustInherit Class ModelGenerationRule
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationRule)
    Implements IRule

#Region " Business Properties & Methods "

    Private _name As String
    Private _type As RuleType
    Private _category As RuleCategory
    Private _calculated As Boolean

    Public ReadOnly Property Name() As String Implements IRule.Name
        Get
            Return _name
        End Get
    End Property
    Public Property Type() As RuleType Implements IRule.Type
        Get
            Return _type
        End Get
        Set(ByVal value As RuleType)
            If _type <> value Then
                _type = value
                PropertyHasChanged("Type")
            End If
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

    Protected Overrides Sub OnPropertyChanged(ByVal propertyName As String)
        MyBase.OnPropertyChanged(propertyName)
        If Parent Is Nothing Then Return
        If String.IsNullOrEmpty(propertyName) Then Return

        If TypeOf Parent Is ModelGenerationEquipmentRules Then
            DirectCast(Parent, ModelGenerationEquipmentRules).RaiseRuleChangedEvent(Me)
        Else
            DirectCast(Parent, ModelGenerationPackRules).RaiseRuleChangedEvent(Me)
        End If

    End Sub

    Public Property Calculated As Boolean
        Get
            Return _Calculated
        End Get
        Set(ByVal value As Boolean)
            If _Calculated = value Then Return
            _Calculated = value
            PropertyHasChanged("Calculated")
        End Set
    End Property

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
#End Region

#Region " Shared Factory Method "

    Friend Shared Function GetRule(ByVal dataReader As SafeDataReader) As ModelGenerationRule
        Select Case dataReader.GetEntity("ENTITY")
            Case Entity.EQUIPMENT
                Return ModelGenerationEquipmentRule.GetRule(dataReader)
            Case Entity.PACK
                Return ModelGenerationPackRule.GetRule(dataReader)
            Case Else
                Throw New Exceptions.IndexItemOutOfRangeException("Unkwon target type " & dataReader.GetString("ENTITY") & "!")
        End Select
    End Function

#End Region

#Region " Data Access "

    Protected Overloads Sub Create(ByVal objId As Guid, ByVal objName As String, ByVal objCategory As RuleCategory)
        Create(objId)
        _name = objName
        _category = objCategory
        ValidationRules.CheckRules()
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _name = .GetString("SHORTNAME")
            _type = CType(.GetValue("RULETYPE"), RuleType)
            _category = CType(.GetValue("RULECATEGORY"), RuleCategory)
            _calculated = dataReader.GetBoolean(GetFieldName("CALCULATED"))
        End With
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'none
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'none
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'none
    End Sub


    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        SetCommand(command, "insert")
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        SetCommand(command, "update")
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        SetCommand(command, "delete")
    End Sub
    Protected Overridable Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@RULETYPE", Type)
        command.Parameters.AddWithValue("@RULECATEGORY", Category)
        command.Parameters.AddWithValue("@CALCULATED", Calculated)
    End Sub
    Protected MustOverride Sub SetCommand(ByVal command As System.Data.SqlClient.SqlCommand, ByVal commandNamePrefix As String)
    
#End Region

End Class

<Serializable()> Public NotInheritable Class ModelGenerationPackRule
    Inherits ModelGenerationRule

#Region " Business Properties & Methods "
    Private Shared ReadOnly AllowCategories As List(Of RuleCategory) = New List(Of RuleCategory) From {RuleCategory.MARKETING, RuleCategory.PRODUCT}
    Public Overrides ReadOnly Property AllowedCategories() As IEnumerable(Of RuleCategory)
        Get
            Return AllowCategories
        End Get
    End Property
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Name & " (+)"
    End Function
#End Region

#Region " Factory Methods "

    Friend Shared Function NewRule(ByVal pack As ModelGenerationPack, ByVal type As RuleType) As ModelGenerationPackRule
        Dim rule As ModelGenerationPackRule = New ModelGenerationPackRule()
        rule.Create(pack.ID, pack.Name, RuleCategory.MARKETING)
        rule.Type = type
        Return rule
    End Function

    Friend Shared Shadows Function GetRule(ByVal dataReader As SafeDataReader) As ModelGenerationPackRule
        Dim rule As ModelGenerationPackRule = New ModelGenerationPackRule()
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
    Protected Overrides Sub SetCommand(ByVal command As System.Data.SqlClient.SqlCommand, ByVal commandNamePrefix As String)
        If TypeOf Parent Is ModelGenerationEquipmentRules Then
            Dim equipmentItem = DirectCast(Parent, ModelGenerationEquipmentRules).EquipmentItem

            command.CommandText = commandNamePrefix & "GenerationEquipmentItemPackRule"
            command.Parameters.AddWithValue("@GENERATIONID", equipmentItem.Generation.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", equipmentItem.ID)
            command.Parameters.AddWithValue("@PACKID", ID)
        ElseIf TypeOf Parent Is ModelGenerationPackRules Then
            Dim pack = DirectCast(Parent, ModelGenerationPackRules).Pack
            command.CommandText = commandNamePrefix & "GenerationPackPackRule"
            command.Parameters.AddWithValue("@GENERATIONID", pack.Generation.ID)
            command.Parameters.AddWithValue("@PACK1ID", pack.ID)
            command.Parameters.AddWithValue("@PACK2ID", ID)
        End If
    End Sub
#End Region


End Class
<Serializable()> Public NotInheritable Class ModelGenerationEquipmentRule
    Inherits ModelGenerationRule
    Implements IColouringModesRule

#Region " Business Properties & Methods "

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
            If value.Equals(_colouringMode) Then Return

            _colouringMode = value
            PropertyHasChanged("ColouringMode")
        End Set
    End Property
    Public ReadOnly Property Colour() As ExteriorColourInfo Implements IColouringModesRule.Colour
        Get
            Return DirectCast(Parent, ModelGenerationRules).Generation.Equipment(ID).Colour
        End Get
    End Property
    
#End Region

#Region " Factory Methods "

    Friend Shared Function NewRule(ByVal item As ModelGenerationEquipmentItem, ByVal ruleType As RuleType, ByVal ruleCategory As RuleCategory, ByVal colouringModes As ColouringModes) As ModelGenerationEquipmentRule
        Dim rule As ModelGenerationEquipmentRule = New ModelGenerationEquipmentRule()
        rule.Create(item.ID, item.Name, ruleCategory, colouringModes)
        rule.Type = ruleType
        Return rule
    End Function
    Friend Shared Shadows Function GetRule(ByVal dataReader As SafeDataReader) As ModelGenerationEquipmentRule
        Dim rule As ModelGenerationEquipmentRule = New ModelGenerationEquipmentRule()
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

    Private Overloads Sub Create(ByVal objId As Guid, ByVal objName As String, ByVal objCategory As RuleCategory, ByVal colouringModes As ColouringModes)
        Create(objId, objName, objCategory)
        _colouringMode = colouringModes
        ValidationRules.CheckRules()
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        _colouringMode = dataReader.GetColouringModes("COLOURINGMODES")
    End Sub

    Protected Overrides Sub AddCommandFields(ByVal command As SqlCommand)
        MyBase.AddCommandFields(command)
        command.Parameters.AddWithValue("@COLOURINGMODES", ColouringMode)
    End Sub


    Protected Overrides Sub SetCommand(ByVal command As System.Data.SqlClient.SqlCommand, ByVal commandNamePrefix As String)
        If TypeOf Parent Is ModelGenerationEquipmentRules Then
            Dim equipmentItem = DirectCast(Parent, ModelGenerationEquipmentRules).EquipmentItem
            command.CommandText = commandNamePrefix & "GenerationEquipmentItemItemRule"
            command.Parameters.AddWithValue("@GENERATIONID", equipmentItem.Generation.ID)
            command.Parameters.AddWithValue("@EQUIPMENT1ID", equipmentItem.ID)
            command.Parameters.AddWithValue("@EQUIPMENT2ID", ID)

        ElseIf TypeOf Parent Is ModelGenerationPackRules Then
            Dim pack = DirectCast(Parent, ModelGenerationPackRules).Pack
            command.CommandText = commandNamePrefix & "GenerationPackEquipmentItemRule"
            command.Parameters.AddWithValue("@GENERATIONID", pack.Generation.ID)
            command.Parameters.AddWithValue("@PACKID", pack.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", ID)
        End If
    End Sub
#End Region


End Class