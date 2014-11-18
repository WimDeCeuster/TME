Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class ModelGenerationCarConfiguratorVersions
    Inherits BaseObjects.ContextListBase(Of ModelGenerationCarConfiguratorVersions, ModelGenerationCarConfiguratorVersion)

#Region " Business Properties & Methods "

    Default Public Overloads ReadOnly Property Item(ByVal id As Int16) As ModelGenerationCarConfiguratorVersion
        Get
            Return (From x In Me Where x.ID.Equals(id)).FirstOrDefault()
        End Get
    End Property

    Public Shadows Function Add(ByVal version As CarConfiguratorVersion) As ModelGenerationCarConfiguratorVersion
        Dim _version As ModelGenerationCarConfiguratorVersion = ModelGenerationCarConfiguratorVersion.NewGenerationCarConfiguratorVersion(version)
        MyBase.Add(_version)
        _version.CheckRules()
        Return _version
    End Function

    Public Overloads Function Contains(ByVal id As Int16) As Boolean
        Return Me.Any(Function(version) version.ID.Equals(id))
    End Function
    Public Overloads Sub Remove(ByVal id As Int16)
        Me.Remove(Me(id))
    End Sub

    Public ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGeneration)
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationCarConfiguratorVersions(ByVal modelGeneration As ModelGeneration) As ModelGenerationCarConfiguratorVersions
        Dim _versions As ModelGenerationCarConfiguratorVersions = New ModelGenerationCarConfiguratorVersions()
        _versions.SetParent(modelGeneration)
        Return _versions
    End Function
    Friend Shared Function GetModelGenerationCarConfiguratorVersions(ByVal modelGeneration As ModelGeneration) As ModelGenerationCarConfiguratorVersions
        Dim _versions As ModelGenerationCarConfiguratorVersions = DataPortal.Fetch(Of ModelGenerationCarConfiguratorVersions)(New CustomCriteria(modelGeneration))
        _versions.SetParent(modelGeneration)
        Return _versions
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MyBase.AllowEdit = MyContext.GetContext().IsGlobal
        MyBase.AllowNew = MyBase.AllowEdit
        MyBase.AllowRemove = MyBase.AllowEdit
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid

        Public Sub New(ByVal modelGeneration As ModelGeneration)
            _generationID = modelGeneration.ID
            CommandText = "getGenerationCarConfiguratorVersions"
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
        End Sub

    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationCarConfiguratorVersion
    Inherits BaseObjects.ContextBusinessBase(Of ModelGenerationCarConfiguratorVersion)

#Region " Business Properties & Methods "
    Private _id As Int16
    Private _name As String

    Public Property ID() As Int16
        Get
            Return _id
        End Get
        Private Set(ByVal value As Int16)
            _id = value
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Private Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Public Function IsEmpty() As Boolean
        Return (Me.ID = 0)
    End Function

    Public ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationCarConfiguratorVersions).Generation
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationCarConfiguratorVersion) As Boolean
        Return Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As CarConfiguratorVersion) As Boolean
        Return Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Int16) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is CarConfiguratorVersion Then
            Return Me.Equals(DirectCast(obj, CarConfiguratorVersion))
        ElseIf TypeOf obj Is ModelGenerationCarConfiguratorVersion Then
            Return Me.Equals(DirectCast(obj, ModelGenerationCarConfiguratorVersion))
        ElseIf TypeOf obj Is Int16 Then
            Return Me.Equals(DirectCast(obj, Int16))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
        Else
            Return False
        End If
    End Function

#End Region

#Region "Framework Overrides"
    Friend Sub CheckRules()
        Me.ValidationRules.CheckRules()
    End Sub
    Protected Overrides Function GetIdValue() As Object
        Return Me.ID
    End Function
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "ID")
    End Sub

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewGenerationCarConfiguratorVersion(ByVal version As CarConfiguratorVersion) As ModelGenerationCarConfiguratorVersion
        Dim _version As ModelGenerationCarConfiguratorVersion = New ModelGenerationCarConfiguratorVersion()
        _version.ID = version.ID
        _version.Name = version.Name
        Return _version
    End Function
    Friend Shared Function GetGenerationCarConfiguratorVersion(ByVal dataReader As SafeDataReader) As ModelGenerationCarConfiguratorVersion
        Dim _version As ModelGenerationCarConfiguratorVersion = New ModelGenerationCarConfiguratorVersion()
        _version.Fetch(dataReader)
        Return _version
    End Function
    Public Shared Function Empty() As ModelGenerationCarConfiguratorVersion
        Dim _version As ModelGenerationCarConfiguratorVersion = New ModelGenerationCarConfiguratorVersion()
        _version.ID = 0
        _version.Name = String.Empty
        _version.MarkOld()
        Return _version
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        MyBase.AllowEdit = False
        MyBase.AllowNew = MyContext.GetContext().IsGlobal
        MyBase.AllowRemove = MyBase.AllowNew
        Me.MarkAsChild()
        Me.FieldPrefix = "CARCONFIGURATORVERSION"
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            If .IsDBNull(GetFieldName("ID")) Then
                _id = 0
                _name = String.Empty
            Else
                _id = .GetInt16(GetFieldName("ID"))
                _name = .GetString(GetFieldName("NAME"))
            End If
        End With
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "deleteGenerationCarConfiguratorVersion"
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "insertGenerationCarConfiguratorVersion"
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@GENERATIONID", Me.Generation.ID)
            .Parameters.AddWithValue("@CARCONFIGURATORVERSIONID", Me.ID)
        End With
    End Sub

#End Region

End Class
