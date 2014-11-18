Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()>
Public Class EquipmentFittingRules
    Inherits ContextUniqueGuidListBase(Of EquipmentFittingRules, EquipmentFittingRule)

#Region "Business Properties & Methods"

    Public ReadOnly Property EquipmentFitting() As EquipmentFitting
        Get
            Return DirectCast(Parent, EquipmentFitting)
        End Get
    End Property

    Public Overloads Function Add(ByVal targetAccessory As Accessory, ByVal ruleType As RuleType) As EquipmentFittingRule
        If ruleType <> ruleType.Include Then Throw New ArgumentException("Can only make include rules on Accessory Fitting level.", "ruleType")

        Dim newRule = EquipmentFittingEquipmentRule.NewRule(targetAccessory, ruleType)
        Add(newRule)
        Return newRule
    End Function

    Public Overloads Function Add(ByVal targetOptionValue As FactoryGenerationOptionValue, ByVal ruleType As RuleType) As EquipmentFittingRule
        If ruleType <> ruleType.Include Then Throw New ArgumentException("Can only make include rules on Accessory Fitting level.", "ruleType")

        Dim newRule = EquipmentFittingFactoryOptionValueRule.NewRule(targetOptionValue, ruleType)
        Add(newRule)
        Return newRule
    End Function

#End Region

#Region "Shared Factory Methods"

    Public Shared Function GetRules(ByVal fitting As EquipmentFitting) As EquipmentFittingRules
        Dim rules = DataPortal.Fetch(Of EquipmentFittingRules)(New EquipmentFittingCriteria(fitting))
        rules.SetParent(fitting)
        Return rules
    End Function

#Region "Criteria"

    <Serializable()>
    Private Class EquipmentFittingCriteria
        Inherits CommandCriteria
        Private ReadOnly _fitting As EquipmentFitting

        Public Sub New(ByVal fitting As EquipmentFitting)
            _fitting = fitting
        End Sub

        Public Overrides Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@EQUIPMENTID", _fitting.EquipmentItem.ID)
            command.Parameters.AddWithValue("@FITTINGID", _fitting.ID)
        End Sub
    End Class

#End Region

#End Region

#Region "Constructors"

    Protected Sub New()
        'prevent direct creation
    End Sub

#End Region

#Region "Data Access"

    Protected Overrides Function GetObject(ByVal dataReader As SafeDataReader) As EquipmentFittingRule
        Return EquipmentFittingEquipmentRule.GetRule(dataReader)
    End Function

    Protected Overrides Sub FetchNextResult(ByVal dataReader As SafeDataReader)
        While dataReader.Read()
            Add(EquipmentFittingFactoryOptionValueRule.GetRule(dataReader))
        End While
    End Sub

#End Region
End Class

<Serializable()>
Public MustInherit Class EquipmentFittingRule
    Inherits ContextUniqueGuidBusinessBase(Of EquipmentFittingEquipmentRule)
    Implements IRule

#Region "Business Properties & Methods"

    Private ReadOnly _allowedCategories As IEnumerable(Of RuleCategory) = New List(Of RuleCategory) From {RuleCategory.PRODUCT}
    Private _category As RuleCategory
    Private _name As String
    Private _type As RuleType

    Public ReadOnly Property AllowedCategories() As IEnumerable(Of RuleCategory) Implements IRule.AllowedCategories
        Get
            Return _allowedCategories
        End Get
    End Property

    Public ReadOnly Property Name() As String Implements IRule.Name
        Get
            Return _name
        End Get
    End Property

    Public Property Type As RuleType Implements IRule.Type
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
        Set(value As RuleCategory)
            If _category = value Then Return

            _category = value
            PropertyHasChanged("Category")
        End Set
    End Property

    Public ReadOnly Property EquipmentFitting As EquipmentFitting
        Get
            Return DirectCast(Parent, EquipmentFittingRules).EquipmentFitting
        End Get
    End Property

#End Region

#Region "System.Object overrides"

    Public Overloads Overrides Function ToString() As String
        Return _name
    End Function

#End Region

#Region "Constructors"

    Protected Sub New()
        'prevent direct creation
        MarkAsChild()
    End Sub

#End Region

#Region "Data Access"

    Protected Overloads Sub Create(ByVal targetId As Guid, ByVal targetName As String, ByVal ruleCategory As RuleCategory)
        Create(targetId)
        _name = targetName
        _category = ruleCategory
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        _name = dataReader.GetString("SHORTNAME")
        _type = CType(dataReader.GetValue("RULETYPE"), RuleType)
        _category = CType(dataReader.GetValue("RULECATEGORY"), RuleCategory)
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        MyBase.AddInsertCommandFields(command)

        command.Parameters.AddWithValue("@RULETYPE", Type)
        command.Parameters.AddWithValue("@RULECATEGORY", Category)
    End Sub

    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub

    Protected Overridable Sub AddCommandSpecializedFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@FITTINGID", EquipmentFitting.ID)
    End Sub
#End Region
End Class

<Serializable>
Public Class EquipmentFittingEquipmentRule
    Inherits EquipmentFittingRule

#Region "Business Properties & Methods"

    Private _masterID As Guid

    Public ReadOnly Property MasterID() As Guid
        Get
            Return _masterID
        End Get
    End Property

#End Region

#Region "Shared Factory Methods"

    Friend Shared Function NewRule(ByVal targetAccessory As Accessory, ByVal ruleType As RuleType) As EquipmentFittingEquipmentRule
        Dim rule = New EquipmentFittingEquipmentRule()
        rule.Create(targetAccessory.ID, targetAccessory.Name, RuleCategory.PRODUCT)
        rule.Type = ruleType
        rule._masterID = targetAccessory.MasterID

        Return rule
    End Function

    Public Shared Function GetRule(ByVal dataReader As SafeDataReader) As EquipmentFittingEquipmentRule
        Dim rule = New EquipmentFittingEquipmentRule()
        rule.Fetch(dataReader)
        Return rule
    End Function

#End Region

#Region "Constructors"

    Private Sub New()
    End Sub

#End Region

#Region "Data Access"

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        _masterID = dataReader.GetGuid("MASTERID")
    End Sub

    Protected Overrides Sub AddCommandSpecializedFields(ByVal command As SqlCommand)
        MyBase.AddCommandSpecializedFields(command)
        command.Parameters.AddWithValue("@EQUIPMENT1ID", EquipmentFitting.EquipmentItem.ID)
        command.Parameters.AddWithValue("@EQUIPMENT2ID", ID)
    End Sub

#End Region
End Class

<Serializable()>
Public Class EquipmentFittingFactoryOptionValueRule
    Inherits EquipmentFittingRule

#Region "Shared Factory Methods"

    Public Shared Function NewRule(ByVal targetFactoryGenerationOptionValue As FactoryGenerationOptionValue, ByVal ruleType As RuleType) As EquipmentFittingFactoryOptionValueRule
        Dim rule = New EquipmentFittingFactoryOptionValueRule()
        rule.Create(targetFactoryGenerationOptionValue.ID, targetFactoryGenerationOptionValue.Description, RuleCategory.PRODUCT)
        rule.Type = ruleType
        Return rule
    End Function

    Public Shared function GetRule(ByVal dataReader As SafeDataReader) As EquipmentFittingFactoryOptionValueRule
        Dim rule = New EquipmentFittingFactoryOptionValueRule()
        rule.Fetch(dataReader)
        Return rule
    End Function

#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region

#Region "Data access"

    Protected Overrides Sub AddCommandSpecializedFields(ByVal command As SqlCommand)
        MyBase.AddCommandSpecializedFields(command)
        command.Parameters.AddWithValue("@EQUIPMENTID", EquipmentFitting.EquipmentItem.ID)
        command.Parameters.AddWithValue("@FACTORYOPTIONVALUEID", ID)
    End Sub

#End Region
End Class