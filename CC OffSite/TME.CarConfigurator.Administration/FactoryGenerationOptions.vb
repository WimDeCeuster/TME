Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class FactoryGenerationOptions
    Inherits ContextUniqueGuidListBase(Of FactoryGenerationOptions, FactoryGenerationOption)


#Region " Business Properties & Methods "


    Friend Property FactoryGeneration() As FactoryGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, FactoryGeneration)
        End Get
        Private Set(ByVal value As FactoryGeneration)
            SetParent(value)
        End Set
    End Property
    

    Public Overloads Function Contains(ByVal factoryMasterSpec As Guid, ByVal factoryMasterSpecValue As Guid) As Boolean
        Return Any(Function(x) x.FactoryMasterSpec.ID.Equals(factoryMasterSpec) AndAlso x.Values.Contains(factoryMasterSpecValue))
    End Function

    Public Overloads Function Contains(ByVal smsCode As String, ByVal smsValue As String) As Boolean
        Return Any(Function(x) x.SMSCode.Equals(smsCode, StringComparison.InvariantCultureIgnoreCase) AndAlso x.Values.Contains(smsValue))
    End Function

    Default Public Overloads Overrides ReadOnly Property Item(ByVal smsCode As String) As FactoryGenerationOption
        Get
            Return FirstOrDefault(Function(x) x.SMSCode.Equals(smsCode, StringComparison.InvariantCultureIgnoreCase))
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewFactoryGenerationOptions(ByVal factoryGeneration As FactoryGeneration) As FactoryGenerationOptions
        Dim options As FactoryGenerationOptions = New FactoryGenerationOptions
        options.FactoryGeneration = factoryGeneration
        Return options
    End Function
    Friend Shared Function GetFactoryGenerationOptions(ByVal factoryGeneration As FactoryGeneration) As FactoryGenerationOptions
        Dim options As FactoryGenerationOptions = DataPortal.Fetch(Of FactoryGenerationOptions)(New CustomCriteria(factoryGeneration.ID))
        options.FactoryGeneration = factoryGeneration
        Return options
    End Function
    Friend Shared Function GetFactoryGenerationOptions(ByVal factoryGeneration As FactoryGeneration, ByVal dataReader As SafeDataReader) As FactoryGenerationOptions
        Dim options As FactoryGenerationOptions = New FactoryGenerationOptions
        options.Fetch(dataReader)
        options.FactoryGeneration = factoryGeneration
        Return options
    End Function

#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _factoryGenerationId As Guid

        Public Sub New(ByVal factoryGenerationId As Guid)
            _factoryGenerationId = factoryGenerationId
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@FACTORYGENERATIONID", _factoryGenerationId)
        End Sub

    End Class
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = MyContext.GetContext().IsGlobal()
        AllowNew = AllowEdit
        AllowRemove = AllowEdit
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        While dataReader.Read
            Me(dataReader.GetGuid("FACTORYGENERATIONOPTIONID")).Values.Add(dataReader)
        End While
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class FactoryGenerationOption
    Inherits ContextUniqueGuidBusinessBase(Of FactoryGenerationOption)

#Region " Business Properties & Methods "
    Private _factoryMasterSpec As FactoryMasterSpecInfo = Nothing
    Private _description As String = String.Empty
    Private _specPos As Integer = -1
    Private _smscode As String = String.Empty
    Private _values As FactoryGenerationOptionValues

    Public Property FactoryMasterSpec() As FactoryMasterSpecInfo
        Get
            Return _factoryMasterSpec
        End Get
        Set(ByVal value As FactoryMasterSpecInfo)
            If _factoryMasterSpec Is Nothing AndAlso value Is Nothing Then Exit Property
            If value Is Nothing OrElse _factoryMasterSpec Is Nothing OrElse Not _factoryMasterSpec.Equals(value) Then
                _factoryMasterSpec = value
                PropertyHasChanged("FactoryMasterSpec")
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
    Public Property SpecPos() As Integer
        Get
            Return _specPos
        End Get
        Set(ByVal value As Integer)
            If _specPos = value Then Return

            _specPos = value
            PropertyHasChanged("SpecPos")
        End Set
    End Property
    Public Property SMSCode() As String
        Get
            Return _smscode
        End Get
        Set(ByVal value As String)
            If _smscode.Equals(value) Then Return

            _smscode = value
            PropertyHasChanged("SMSCode")
        End Set
    End Property
    Public ReadOnly Property Values() As FactoryGenerationOptionValues
        Get
            If _values Is Nothing Then _values = FactoryGenerationOptionValues.NewFactoryGenerationOptionValues(Me)
            Return _values
        End Get
    End Property

    Public ReadOnly Property FactoryGeneration() As FactoryGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, FactoryGenerationOptions).FactoryGeneration
        End Get
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Description
    End Function

#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "FactoryMasterSpec")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Description")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "SMSCode")
        
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Description", 80))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("SMSCode", 3))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "FactoryMasterSpec")
        ValidationRules.AddRule(DirectCast(AddressOf SpecPosUnique, Validation.RuleHandler), "SpecPos")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "SMSCode")
    End Sub
    Private Shared Function SpecPosUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim [option] As FactoryGenerationOption = DirectCast(target, FactoryGenerationOption)
        If [option].SpecPos = -1 Then Return True

        If [option].FactoryGeneration.Options.Any(Function(x) x.SpecPos.Equals([option].SpecPos) AndAlso Not x.ID.Equals([option].ID)) Then
            e.Description = "The SpecPos '" & [option].SpecPos.ToString() & "' has to be unique."
            Return False
        End If
        Return True
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_values Is Nothing) AndAlso Not _values.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_values Is Nothing) AndAlso _values.IsDirty Then Return True
            Return False
        End Get
    End Property

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

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _values Is Nothing Then _values.Update(transaction)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@FACTORYGENERATIONID", FactoryGeneration.ID)
        command.Parameters.AddWithValue("@FACTORYMASTERSPECID", FactoryMasterSpec.ID)
        AddUpdateCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@DESCRIPTION", Description)
        command.Parameters.AddWithValue("@SPECPOS", SpecPos)
        command.Parameters.AddWithValue("@SMSCODE", SMSCode)
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _factoryMasterSpec = FactoryMasterSpecInfo.GetFactoryMasterSpecInfo(dataReader)
        _description = dataReader.GetString(GetFieldName("DESCRIPTION")).Trim()
        _specPos = dataReader.GetInt32(GetFieldName("SPECPOS"))
        _smscode = dataReader.GetString(GetFieldName("SMSCODE")).Trim()
    End Sub

#End Region

End Class
