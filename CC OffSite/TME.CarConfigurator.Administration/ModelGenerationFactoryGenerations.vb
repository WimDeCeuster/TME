Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class ModelGenerationFactoryGenerations
    Inherits ContextUniqueGuidListBase(Of ModelGenerationFactoryGenerations, ModelGenerationFactoryGeneration)

#Region " Business Properties & Methods "

    Public Shadows Function Add(ByVal factoryGeneration As FactoryGeneration) As ModelGenerationFactoryGeneration
        Dim modelGenerationFactoryGeneration As ModelGenerationFactoryGeneration = modelGenerationFactoryGeneration.NewGenerationFactoryGeneration(factoryGeneration)
        MyBase.Add(modelGenerationFactoryGeneration)
        modelGenerationFactoryGeneration.CheckRules()
        Return modelGenerationFactoryGeneration
    End Function

    Public ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGeneration)
        End Get
    End Property

    Public Sub Refresh()
        For Each _generation In Me
            _generation.Refresh()
        Next
    End Sub
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewGenerationFactoryGenerations(ByVal modelGeneration As ModelGeneration) As ModelGenerationFactoryGenerations
        Dim modelGenerationFactoryGenerations As ModelGenerationFactoryGenerations = New ModelGenerationFactoryGenerations()
        modelGenerationFactoryGenerations.SetParent(modelGeneration)
        Return modelGenerationFactoryGenerations
    End Function

    Friend Shared Function GetGenerationFactoryGenerations(ByVal modelGeneration As ModelGeneration) As ModelGenerationFactoryGenerations
        Dim modelGenerationFactoryGenerations As ModelGenerationFactoryGenerations = DataPortal.Fetch(Of ModelGenerationFactoryGenerations)(New CustomCriteria(modelGeneration))
        modelGenerationFactoryGenerations.SetParent(modelGeneration)
        Return modelGenerationFactoryGenerations
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = MyContext.GetContext().IsGlobal
        AllowNew = AllowEdit
        AllowRemove = AllowEdit
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid

        Public Sub New(ByVal modelGeneration As ModelGeneration)
            _generationID = modelGeneration.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
        End Sub

    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationFactoryGeneration
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationFactoryGeneration)

#Region " Business Properties & Methods "
    Private _info As FactoryGenerationInfo
    Private WithEvents _mapping As OptionMapping
    Private _isFullyMapped As Nullable(Of Boolean)
    Private _factoryCars As ModelGenerationFactoryCars
    Private _options As ModelGenerationFactoryOptions

    Public ReadOnly Property FactoryModel() As FactoryModelInfo
        Get
            Return _info.FactoryModel
        End Get
    End Property
    Public ReadOnly Property SSN() As String
        Get
            Return _info.SSN
        End Get
    End Property
    Public ReadOnly Property FromDate() As Date
        Get
            Return _info.FromDate
        End Get
    End Property
    Public ReadOnly Property ToDate() As Date
        Get
            Return _info.ToDate
        End Get
    End Property
    Public ReadOnly Property ProjectID() As Guid
        Get
            Return _info.ProjectID
        End Get
    End Property
    Public ReadOnly Property ProjectCode() As String
        Get
            Return _info.ProjectCode
        End Get
    End Property
    Public ReadOnly Property CarFamilyCode() As String
        Get
            Return _info.CarFamilyCode
        End Get
    End Property
    Public ReadOnly Property Description() As String
        Get
            Return _info.Description
        End Get
    End Property
    Private WriteOnly Property Info() As FactoryGenerationInfo
        Set(ByVal value As FactoryGenerationInfo)
            _info = value
            ID = value.ID
        End Set
    End Property

    Public ReadOnly Property Generation() As ModelGeneration
        Get
            Return DirectCast(Parent, ModelGenerationFactoryGenerations).Generation
        End Get
    End Property
    Public ReadOnly Property FactoryCars() As ModelGenerationFactoryCars
        Get
            If _factoryCars Is Nothing Then
                _factoryCars = ModelGenerationFactoryCars.GetFactoryCars(Me)
            End If
            Return _factoryCars
        End Get
    End Property

    Public ReadOnly Property Options() As ModelGenerationFactoryOptions
        Get
            If _options Is Nothing Then
                _options = ModelGenerationFactoryOptions.GetModelGenerationFactoryOptions(Me)
                _options.ModelGenerationFactoryGeneration = Me
            End If
            Return _options
        End Get
    End Property

    Public ReadOnly Property IsFullyMapped() As Boolean
        Get
            If Not _isFullyMapped.HasValue Then
                _isFullyMapped = DirectCast(ContextCommand.ExecuteScalar(New IsFullyMappedCommand(Me)), Boolean)
            End If
            Return _isFullyMapped.Value
        End Get
    End Property
    Public Property OptionMapping() As OptionMapping
        Get
            If _mapping Is Nothing Then
                If IsNew Then
                    _mapping = OptionMapping.NewOptionMapping(Me)
                Else
                    _mapping = OptionMapping.GetOptionMapping(Me)
                End If
            End If
            Return _mapping
        End Get
        Friend Set(ByVal value As OptionMapping)
            _mapping = value
        End Set
    End Property


    Public Function GetInfo() As FactoryGenerationInfo
        Return _info
    End Function

    Public Sub Refresh()
        _isFullyMapped = Nothing
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Description.Length = 0 Then Return SSN
        Return String.Format("{0} : {1}", SSN, Description)
    End Function

    Private ReadOnly _hashCode As Integer = Guid.NewGuid().GetHashCode()

    Public Overloads Overrides Function GetHashCode() As Integer
        Return _hashCode
    End Function

#End Region

#Region "Framework Overrides"
    Friend Sub CheckRules()
        ValidationRules.CheckRules()
    End Sub

    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If Not _mapping Is Nothing AndAlso _mapping.IsDirty Then Return True
            If Not _factoryCars Is Nothing AndAlso _factoryCars.IsDirty Then Return True
            If Not _options Is Nothing AndAlso _options.IsDirty Then Return True
            Return MyBase.IsDirty
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not _mapping Is Nothing AndAlso Not _mapping.IsValid Then Return False
            If Not _factoryCars Is Nothing AndAlso Not _factoryCars.IsValid Then Return False
            If Not _options Is Nothing AndAlso Not _options.IsValid Then Return False
            Return MyBase.IsValid
        End Get
    End Property

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "ID")
    End Sub

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewGenerationFactoryGeneration(ByVal factoryGeneration As FactoryGeneration) As ModelGenerationFactoryGeneration
        Dim modelGenerationFactoryGeneration As ModelGenerationFactoryGeneration = New ModelGenerationFactoryGeneration()
        modelGenerationFactoryGeneration.ID = factoryGeneration.ID
        modelGenerationFactoryGeneration.Info = factoryGeneration.GetInfo()
        Return modelGenerationFactoryGeneration
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        AllowEdit = MyContext.GetContext().IsGlobal
        AllowNew = AllowEdit
        AllowRemove = AllowEdit
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("FACTORYGENERATIONID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _info = FactoryGenerationInfo.GetFactoryGenerationInfo(dataReader)
        _isFullyMapped = dataReader.GetBoolean("ISFULLYMAPPED")
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'nothing
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'nothing
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@GENERATIONID", Generation.ID)
            .Parameters.AddWithValue("@FACTORYGENERATIONID", ID)
        End With
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _mapping Is Nothing Then _mapping.Update(transaction)
        If Not _factoryCars Is Nothing Then _factoryCars.Update(transaction)
        If Not _options Is Nothing Then _options.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

    <Serializable()> Private Class IsFullyMappedCommand
        Inherits ContextCommand.CommandInfo

        Private ReadOnly _generationID As Guid
        Private ReadOnly _factoryGenerationID As Guid

        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "isFullyMapped"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
            command.Parameters.AddWithValue("@FACTORYGENERATIONID", _factoryGenerationID)
        End Sub

        Public Sub New(ByVal factoryGeneration As ModelGenerationFactoryGeneration)
            _generationID = factoryGeneration.Generation.ID
            _factoryGenerationID = factoryGeneration.ID
        End Sub
    End Class

    Private Sub MappingAfterUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles _mapping.AfterUpdateCommand
        _isFullyMapped = Nothing
    End Sub
End Class
