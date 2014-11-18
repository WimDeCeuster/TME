Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class FactoryGenerationOptionValues
    Inherits ContextUniqueGuidListBase(Of FactoryGenerationOptionValues, FactoryGenerationOptionValue)

#Region " Business Properties & Methods "

    Friend Overloads Function Add(ByVal dataReader As SafeDataReader) As FactoryGenerationOptionValue
        Dim value As FactoryGenerationOptionValue = GetObject(dataReader)
        AllowNew = True
        Add(value)
        AllowNew = AllowEdit
        Return value
    End Function

    Friend Property FactoryGenerationOption() As FactoryGenerationOption
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, FactoryGenerationOption)
        End Get
        Private Set(ByVal value As FactoryGenerationOption)
            SetParent(value)
        End Set
    End Property

    Public Overloads Function Contains(ByVal factoryMasterSpecValue As FactoryMasterSpecValueInfo) As Boolean
        Return Any(Function(x) x.FactoryMasterSpecValue.Equals(factoryMasterSpecValue))
    End Function


    Public Sub CheckRules()
        For Each value In Me
            value.CheckRules()
        Next
    End Sub

    Default Public Overloads Overrides ReadOnly Property Item(ByVal smsValue As String) As FactoryGenerationOptionValue
        Get
            Return FirstOrDefault(Function(x) x.SMSValue.Equals(smsValue, StringComparison.InvariantCultureIgnoreCase))
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewFactoryGenerationOptionValues(ByVal [option] As FactoryGenerationOption) As FactoryGenerationOptionValues
        Dim values As FactoryGenerationOptionValues = New FactoryGenerationOptionValues
        values.FactoryGenerationOption = [option]
        Return values
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = MyContext.GetContext().IsGlobal()
        AllowNew = AllowEdit
        AllowRemove = AllowEdit
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class FactoryGenerationOptionValue
    Inherits ContextUniqueGuidBusinessBase(Of FactoryGenerationOptionValue)

#Region " Business Properties & Methods "
    Private _factoryMasterSpecValue As FactoryMasterSpecValueInfo = Nothing
    Private _description As String = String.Empty
    Private _specvalue As String = String.Empty
    Private _smsvalue As String = String.Empty

    Public Property FactoryMasterSpecValue() As FactoryMasterSpecValueInfo
        Get
            Return _factoryMasterSpecValue
        End Get
        Set(ByVal value As FactoryMasterSpecValueInfo)
            If _factoryMasterSpecValue Is Nothing AndAlso value Is Nothing Then Exit Property
            If value Is Nothing OrElse _factoryMasterSpecValue Is Nothing OrElse Not _factoryMasterSpecValue.Equals(value) Then
                _factoryMasterSpecValue = value
                PropertyHasChanged("FactoryMasterSpecValue")
            End If
        End Set
    End Property
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            If _description.Equals(value) Then Return

            _description = value
            PropertyHasChanged("Description")
        End Set
    End Property
    Public Property SpecValue() As String
        Get
            Return _specvalue
        End Get
        Set(ByVal value As String)
            If _specvalue.Equals(value) Then Return

            _specvalue = value
            PropertyHasChanged("SpecValue")
        End Set
    End Property
    Public Property SMSValue() As String
        Get
            Return _smsvalue
        End Get
        Set(ByVal value As String)
            If _smsvalue.Equals(value) Then Return

            _smsvalue = value
            PropertyHasChanged("SMSValue")
        End Set
    End Property

    Public ReadOnly Property [Option]() As FactoryGenerationOption
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, FactoryGenerationOptionValues).FactoryGenerationOption
        End Get
    End Property


    Public Sub Remove()
        Me.Option.Values.Remove(Me)
    End Sub


    Public Function GetInfo() As FactoryGenerationOptionValueInfo
        Return FactoryGenerationOptionValueInfo.GetInfo(Me)
    End Function

#End Region

#Region " System.Object Overrides "


    Public Overloads Overrides Function ToString() As String
        Return String.Format("{0}{1} : {2} - {3}", Me.Option.SMSCode, SMSValue, Me.Option.Description, Description)
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryGenerationOptionValueInfo) As Boolean
        Return obj.ID.Equals(ID)
    End Function

    Public Overrides Function Equals(ByVal obj As String) As Boolean
        If SMSValue.Equals(obj, StringComparison.InvariantCultureIgnoreCase) Then Return True
        If SpecValue.Equals(obj, StringComparison.InvariantCultureIgnoreCase) Then Return True
        If FactoryMasterSpecValue.Description.Equals(obj, StringComparison.InvariantCultureIgnoreCase) Then Return True
        Return False
    End Function

#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "FactoryMasterSpecValue")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Description")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "SMSValue")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Description", 80))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("SpecValue", 1))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("SMSValue", 2))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "FactoryMasterSpecValue")
        ValidationRules.AddRule(DirectCast(AddressOf SpecValueUnique, Validation.RuleHandler), "SpecValue")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "SMSValue")
    End Sub
    Private Shared Function SpecValueUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim value As FactoryGenerationOptionValue = DirectCast(target, FactoryGenerationOptionValue)
        If value.SpecValue.Length = 0 Then Return True

        If value.Option.Values.Any(Function(x) x.SpecValue.Equals(value.SpecValue) AndAlso Not x.ID.Equals(value.ID)) Then
            e.Description = "The SpecValue '" & value.SpecValue.ToString() & "' has to be unique."
            Return False
        End If
        Return True
    End Function

    Friend Sub CheckRules()
        ValidationRules.CheckRules()
    End Sub
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowNew = MyContext.GetContext().IsGlobal()
        AllowEdit = AllowNew
        AllowRemove = AllowNew
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@FACTORYGENERATIONOPTIONID", Me.Option.ID)
        command.Parameters.AddWithValue("@FACTORYMASTERSPECVALUEID", FactoryMasterSpecValue.ID)
        AddUpdateCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@DESCRIPTION", Description)
        command.Parameters.AddWithValue("@SPECVALUE", SpecValue)
        command.Parameters.AddWithValue("@SMSVALUE", SMSValue)
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _factoryMasterSpecValue = FactoryMasterSpecValueInfo.GetFactoryMasterSpecValueInfo(dataReader)
        _description = dataReader.GetString(GetFieldName("DESCRIPTION")).Trim()
        _specvalue = dataReader.GetString(GetFieldName("SPECVALUE")).Trim()
        _smsvalue = dataReader.GetString(GetFieldName("SMSVALUE")).Trim()
    End Sub

#End Region


End Class
<Serializable(), XmlInfo("option")> Public NotInheritable Class FactoryGenerationOptionValueInfo

#Region " Business Properties & Methods "
    Private _id As Guid
    Private _factoryMasterSpec As FactoryMasterSpecInfo
    Private _factoryMasterSpecValue As FactoryMasterSpecValueInfo
    Private _description As String
    Private _smsCode As String
    Private _smsValue As String
    Private _specPos As Integer
    Private _specValue As String

    Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property FactoryMasterSpec() As FactoryMasterSpecInfo
        Get
            Return _factoryMasterSpec
        End Get
    End Property
    Public ReadOnly Property FactoryMasterSpecValue() As FactoryMasterSpecValueInfo
        Get
            Return _factoryMasterSpecValue
        End Get
    End Property
    Public ReadOnly Property SMSCode() As String
        Get
            Return _smsCode
        End Get
    End Property
    Public ReadOnly Property SMSValue() As String
        Get
            Return _smsValue
        End Get
    End Property
    Public ReadOnly Property SpecPos() As Integer
        Get
            Return _specPos
        End Get
    End Property
    Public ReadOnly Property SpecValue() As String
        Get
            Return _specValue
        End Get
    End Property

    Public ReadOnly Property Description() As String
        Get
            Return _description
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetInfo(ByVal optionValue As FactoryGenerationOptionValue) As FactoryGenerationOptionValueInfo
        Dim info As FactoryGenerationOptionValueInfo = New FactoryGenerationOptionValueInfo
        With info
            ._id = optionValue.ID
            ._factoryMasterSpec = optionValue.Option.FactoryMasterSpec
            ._factoryMasterSpecValue = optionValue.FactoryMasterSpecValue
            ._description = String.Format("{0} - {1}", optionValue.Option.Description, optionValue.Description)
            ._smsCode = optionValue.Option.SMSCode
            ._smsValue = optionValue.SMSValue
            ._specPos = optionValue.Option.SpecPos
            ._specValue = optionValue.SpecValue
        End With
        Return info
    End Function
    Public Shared Function GetInfo(ByVal optionValue As ModelGenerationFactoryOptionValue) As FactoryGenerationOptionValueInfo
        Dim info As FactoryGenerationOptionValueInfo = New FactoryGenerationOptionValueInfo
        With info
            ._id = optionValue.ID
            ._factoryMasterSpec = optionValue.Option.FactoryMasterSpec
            ._factoryMasterSpecValue = optionValue.FactoryMasterSpecValue
            ._description = String.Format("{0} - {1}", optionValue.Option.Description, optionValue.Description)
            ._smsCode = optionValue.Option.SMSCode
            ._smsValue = optionValue.SMSValue
            ._specPos = optionValue.Option.SpecPos
            ._specValue = optionValue.SpecValue
        End With
        Return info
    End Function

    Friend Shared Function GetInfo(ByVal dataReader As SafeDataReader) As FactoryGenerationOptionValueInfo
        Dim info As FactoryGenerationOptionValueInfo = New FactoryGenerationOptionValueInfo
        With info
            ._id = dataReader.GetGuid("FACTORYGENERATIONOPTIONVALUEID")
            ._factoryMasterSpec = FactoryMasterSpecInfo.GetFactoryMasterSpecInfo(dataReader)
            ._factoryMasterSpecValue = FactoryMasterSpecValueInfo.GetFactoryMasterSpecValueInfo(dataReader)
            ._description = String.Format("{0} - {1}", dataReader.GetString("FACTORYGENERATIONOPTIONDESCRIPTION"), dataReader.GetString("FACTORYGENERATIONOPTIONVALUEDESCRIPTION"))
            ._smsCode = dataReader.GetString("SMSCODE").Trim()
            ._smsValue = dataReader.GetString("SMSVALUE").Trim()
            ._specPos = dataReader.GetInt32("SPECPOS")
            ._specValue = dataReader.GetString("SPECVALUE")
        End With
        Return info
    End Function

#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return String.Format("{0}{1} : {2}", SMSCode, SMSValue, Description)
    End Function

    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryGenerationOptionValueInfo) As Boolean
        Return obj.ID.Equals(ID)
    End Function
    Public Overloads Function Equals(ByVal obj As FactoryGenerationOptionValue) As Boolean
        Return obj.ID.Equals(ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationFactoryOptionValue) As Boolean
        Return obj.ID.Equals(ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return obj.Equals(ID)
    End Function

    Public Overloads Function Equals(ByVal code As String, ByVal value As String) As Boolean
        Return SMSCode.Equals(code, StringComparison.InvariantCultureIgnoreCase) AndAlso SMSValue.Equals(value, StringComparison.InvariantCultureIgnoreCase)
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is FactoryGenerationOptionValueInfo Then
            Return Equals(DirectCast(obj, FactoryGenerationOptionValueInfo))
        ElseIf TypeOf obj Is FactoryGenerationOptionValue Then
            Return Equals(DirectCast(obj, FactoryGenerationOptionValue))
        ElseIf TypeOf obj Is ModelGenerationFactoryOptionValue Then
            Return Equals(DirectCast(obj, ModelGenerationFactoryOptionValue))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If objA Is Nothing AndAlso objB Is Nothing Then Return True
        If objA Is Nothing AndAlso Not objB Is Nothing Then Return False
        If Not objA Is Nothing AndAlso objB Is Nothing Then Return False

        If TypeOf objA Is FactoryGenerationOptionValueInfo Then
            Return DirectCast(objA, FactoryGenerationOptionValueInfo).Equals(objB)
        ElseIf TypeOf objB Is FactoryGenerationOptionValueInfo Then
            Return DirectCast(objB, FactoryGenerationOptionValueInfo).Equals(objA)
        Else
            Return False
        End If
    End Function
#End Region

End Class