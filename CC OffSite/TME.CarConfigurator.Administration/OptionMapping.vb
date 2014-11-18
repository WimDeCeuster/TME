Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class OptionMapping
    Inherits ContextUniqueGuidListBase(Of OptionMapping, OptionMappingLine)

#Region " Business Properties & Methods "
    Friend Property FactoryGeneration() As ModelGenerationFactoryGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationFactoryGeneration)
        End Get
        Private Set(ByVal value As ModelGenerationFactoryGeneration)
            SetParent(value)
        End Set
    End Property

    Public Sub MakeProposal()
        If Count > 0 Then Throw New Exceptions.ObjectAlreadyExists("The current schema already contains mapping lines. These need to be removed prior to creating a new proposal.")
        FactoryGeneration.OptionMapping = GetOptionMappingProposal(FactoryGeneration)
    End Sub

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewOptionMapping(ByVal factoryGeneration As ModelGenerationFactoryGeneration) As OptionMapping
        Dim mapping As OptionMapping = New OptionMapping()
        mapping.FactoryGeneration = factoryGeneration
        Return mapping
    End Function
    Friend Shared Function GetOptionMapping(ByVal factoryGeneration As ModelGenerationFactoryGeneration) As OptionMapping
        Dim mapping As OptionMapping = DataPortal.Fetch(Of OptionMapping)(New CustomCriteria(factoryGeneration))
        mapping.FactoryGeneration = factoryGeneration
        Return mapping
    End Function
    Private Shared Function GetOptionMappingProposal(ByVal factoryGeneration As ModelGenerationFactoryGeneration) As OptionMapping
        Dim mapping As OptionMapping = DataPortal.Fetch(Of OptionMapping)(New CustomCriteria(factoryGeneration, "getOptionMappingProposal"))
        mapping.FactoryGeneration = factoryGeneration
        Return mapping
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = MyContext.GetContext().IsGlobal()
        AllowNew = AllowEdit
        AllowRemove = AllowEdit
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid
        Private ReadOnly _factoryGenerationID As Guid

        Public Sub New(ByVal factoryGeneration As ModelGenerationFactoryGeneration)
            _generationID = factoryGeneration.Generation.ID
            _factoryGenerationID = factoryGeneration.ID
        End Sub
        Public Sub New(ByVal factoryGeneration As ModelGenerationFactoryGeneration, ByVal commandText As String)
            _generationID = factoryGeneration.Generation.ID
            _factoryGenerationID = factoryGeneration.ID
            MyBase.CommandText = commandText
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
            command.Parameters.AddWithValue("@FACTORYGENERATIONID", _factoryGenerationID)
        End Sub
    End Class
#End Region

End Class
<Serializable()> Public Class OptionMappingLine
    Inherits ContextUniqueGuidBusinessBase(Of OptionMappingLine)

#Region " Business Properties & Methods "

    Private _optionValue As FactoryGenerationOptionValueInfo
    Private _exteriorColourCode As String = String.Empty
    Private _marketingIrrelevant As Boolean
    Private _parked As Boolean
    Private _option As OptionInfo

    Public Property FactoryOptionValue() As FactoryGenerationOptionValueInfo
        Get
            Return _optionValue
        End Get
        Set(ByVal optionValue As FactoryGenerationOptionValueInfo)
            If (optionValue Is Nothing) Then Exit Property
            If FactoryGenerationOptionValueInfo.Equals(optionValue, _optionValue) Then Exit Property

            _optionValue = optionValue
            PropertyHasChanged("FactoryOptionValue")
            ValidationRules.CheckRules("Uniqueness")
        End Set
    End Property
    Public Property ExteriorColourCode() As String
        Get
            Return _exteriorColourCode
        End Get
        Set(ByVal newValue As String)
            newValue = newValue.Trim()
            If _exteriorColourCode.Equals(newValue, StringComparison.InvariantCultureIgnoreCase) Then Exit Property

            _exteriorColourCode = newValue
            PropertyHasChanged("ExteriorColourCode")
            ValidationRules.CheckRules("Uniqueness")
        End Set
    End Property
    Public Property MarketingIrrelevant() As Boolean
        Get
            Return _marketingIrrelevant
        End Get
        Set(ByVal newValue As Boolean)
            If _marketingIrrelevant.Equals(newValue) Then Exit Property

            _marketingIrrelevant = newValue
            PropertyHasChanged("MarketingIrrelevant")
            ValidationRules.CheckRules("OptionRequired")
        End Set
    End Property
    Public Property Parked() As Boolean
        Get
            Return _parked
        End Get
        Set(ByVal newValue As Boolean)
            If _parked.Equals(newValue) Then Exit Property

            _parked = newValue
            PropertyHasChanged("Parked")
            ValidationRules.CheckRules("OptionRequired")
        End Set
    End Property

    Public Property [Option]() As OptionInfo
        Get
            Return _option
        End Get
        Set(ByVal optionInfo As OptionInfo)
            If (optionInfo Is Nothing) Then optionInfo = optionInfo.Empty
            If _option.Equals(optionInfo) Then Exit Property

            _option = optionInfo
            PropertyHasChanged("Option")
            ValidationRules.CheckRules("OptionRequired")
            ValidationRules.CheckRules("Uniqueness")
        End Set
    End Property


    Public ReadOnly Property FactoryGeneration() As ModelGenerationFactoryGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, OptionMapping).FactoryGeneration
        End Get
    End Property
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "FactoryOptionValue")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("ExteriorColourCode", 3))

        ValidationRules.AddRule(DirectCast(AddressOf ObjectUnique, Validation.RuleHandler), "Uniqueness")
        ValidationRules.AddRule(DirectCast(AddressOf OptionRequired, Validation.RuleHandler), "OptionRequired")
    End Sub

    Private Shared Function ObjectUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim value As OptionMappingLine = DirectCast(target, OptionMappingLine)
        If value.Parent Is Nothing Then Return True
        If value.FactoryOptionValue Is Nothing Then Return True

        If DirectCast(value.Parent, OptionMapping).Any(Function(x) Not x.ID.Equals(value.ID) AndAlso FactoryGenerationOptionValueInfo.Equals(value.FactoryOptionValue, x.FactoryOptionValue) AndAlso x.ExteriorColourCode.Equals(value.ExteriorColourCode, StringComparison.InvariantCultureIgnoreCase) AndAlso OptionInfo.Equals(x.Option, value.Option)) Then
            e.Description = "There already is a mapping available for this combination [duplicate entry]"
            Return False
        End If

        Return True
    End Function
    Private Shared Function OptionRequired(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim value As OptionMappingLine = DirectCast(target, OptionMappingLine)

        If value.MarketingIrrelevant Then Return True
        If value.Parked Then Return True

        If value.Option.IsEmpty Then
            e.Description = "An option is required for this line!"
            Return False
        End If

        Return True
    End Function

#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        If ExteriorColourCode.Length = 0 Then
            Return String.Format("{0} -> {1}", ToSafeString(FactoryOptionValue), ToSafeString(Me.Option))
        Else
            Return String.Format("{0} - {1} -> {2}", ToSafeString(FactoryOptionValue), ExteriorColourCode, ToSafeString(Me.Option))
        End If
    End Function
    Private Function ToSafeString(ByVal obj As Object) As String
        If obj Is Nothing Then Return "???"
        Return obj.ToString()
    End Function

    Public Overloads Function Equals(ByVal obj As OptionMappingLine) As Boolean
        If obj Is Nothing Then Return False
        If Not OptionInfo.Equals(obj.Option, Me.Option) Then Return False
        If Not FactoryGenerationOptionValueInfo.Equals(obj.FactoryOptionValue, FactoryOptionValue) Then Return False
        Return True
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AutoDiscover = False
        AllowEdit = MyContext.GetContext().IsGlobal()
        AllowNew = AllowEdit
        AllowRemove = AllowEdit
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        _option = OptionInfo.Empty
    End Sub
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        If dataReader.FieldExists("ID") Then
            ID = dataReader.GetGuid("ID")
        Else
            ID = Guid.NewGuid()
            MarkNew()
        End If
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _optionValue = FactoryGenerationOptionValueInfo.GetInfo(dataReader)
        _exteriorColourCode = dataReader.GetString("EXTERIORCOLOURCODE")
        _marketingIrrelevant = dataReader.GetBoolean("MARKETINGIRRELEVANT")
        _parked = dataReader.GetBoolean("PARKED")
        _option = OptionInfo.GetOptionInfo(dataReader)
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@GENERATIONID", FactoryGeneration.Generation.ID)
        command.Parameters.AddWithValue("@FACTORYGENERATIONID", FactoryGeneration.ID)
        command.Parameters.AddWithValue("@FACTORYGENERATIONOPTIONVALUEID", FactoryOptionValue.ID)
        command.Parameters.AddWithValue("@EXTERIORCOLOURCODE", ExteriorColourCode)
        command.Parameters.AddWithValue("@MARKETINGIRRELEVANT", MarketingIrrelevant)
        command.Parameters.AddWithValue("@PARKED", Parked)
        command.Parameters.AddWithValue("@EQUIPMENTID", [Option].ID.GetDbValue())
    End Sub

#End Region

End Class