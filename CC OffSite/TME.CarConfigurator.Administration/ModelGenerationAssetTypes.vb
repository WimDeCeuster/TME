Imports TME.CarConfigurator.Administration.Assets

<Serializable()> Public NotInheritable Class ModelGenerationAssetTypes
    Inherits BaseObjects.ContextListBase(Of ModelGenerationAssetTypes, ModelGenerationAssetType)

#Region " Business Properties & Methods "

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGeneration)
        End Get
    End Property

    Public Shadows Function Add(ByVal type As AssetType) As ModelGenerationAssetType
        If Me.Any(Function(t) t.Equals(type)) Then Throw New Exceptions.AssetTypeAlreadyExistsException()

        Dim _type As ModelGenerationAssetType = ModelGenerationAssetType.NewModelGenerationAssetType(type)
        MyBase.Add(_type)
        Return _type
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationAssetTypes(ByVal generation As ModelGeneration) As ModelGenerationAssetTypes
        Dim _types As ModelGenerationAssetTypes = New ModelGenerationAssetTypes
        _types.SetParent(generation)
        Return _types
    End Function
    Friend Shared Function GetModelGenerationAssetTypes(ByVal generation As ModelGeneration) As ModelGenerationAssetTypes
        Dim _types As ModelGenerationAssetTypes = DataPortal.Fetch(Of ModelGenerationAssetTypes)(New CustomCriteria(generation))
        _types.SetParent(generation)
        Return _types
    End Function
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid

        Public Sub New(ByVal generation As ModelGeneration)
            _generationID = generation.ID
            Me.CommandText = "getGenerationAssetTypes"
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
        End Sub
    End Class
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
        Me.AllowNew = MyContext.GetContext().IsGlobal
        Me.AllowEdit = False
        Me.AllowRemove = Me.AllowNew
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationAssetType
    Inherits BaseObjects.ContextBusinessBase(Of ModelGenerationAssetType)

#Region " Business Properties & Methods "
    Private _code As String
    Private _name As String
    Private _details As AssetTypeDetails

    Private ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationAssetTypes).Generation
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    Public ReadOnly Property Details() As AssetTypeDetails
        Get
            If _details Is Nothing Then _details = New AssetTypeDetails(Me)
            Return _details
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationAssetType(ByVal assetType As AssetType) As ModelGenerationAssetType
        Dim _type As ModelGenerationAssetType = New ModelGenerationAssetType()
        _type.Create()
        _type.Fetch(assetType)
        Return _type
    End Function
#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Me.Code
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Function Equals(ByVal obj As AssetType) As Boolean
        Return (String.Compare(Me.Code, obj.Code, True) = 0)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return (String.Compare(Me.Code, obj, True) = 0) OrElse (String.Compare(Me.Name, obj, True) = 0)
    End Function
    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is AssetType Then Return Me.Equals(DirectCast(obj, AssetType))
        Return MyBase.Equals(obj)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        Me.AllowNew = MyContext.GetContext().IsGlobal
        Me.AllowEdit = False
        Me.AllowRemove = Me.AllowNew
    End Sub
#End Region

#Region " Data Access "
    Private Overloads Sub Fetch(ByVal assetType As AssetType)
        _code = assetType.Code
        _name = assetType.Name
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _code = dataReader.GetInt16("TYPE").ToString()
        _name = dataReader.GetString("DESCRIPTION")
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "insertGenerationAssetType"
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "deleteGenerationAssetType"
        Me.AddCommandFields(command)
    End Sub

    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@GENERATIONID", Me.Generation.ID)
        command.Parameters.AddWithValue("@ASSETTYPE", Int16.Parse(Me.Code))
    End Sub

#End Region

End Class

