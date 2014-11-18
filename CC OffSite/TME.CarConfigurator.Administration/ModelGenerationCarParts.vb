Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationCarParts
    Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelGenerationCarParts, ModelGenerationCarPart)

#Region " Business Properties & Methods "

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGeneration)
        End Get
    End Property

    Public Shadows Function Add(ByVal part As CarPart) As ModelGenerationCarPart
        If Me.Any(Function(t) t.Equals(part)) Then Throw New Exceptions.ObjectAlreadyExists(Entity.CARPART, part)

        Dim _part As ModelGenerationCarPart = ModelGenerationCarPart.NewModelGenerationCarPart(part)
        MyBase.Add(_part)
        Return _part
    End Function

#End Region

#Region " Framework Overrides "
    Public Overrides Function Save() As ModelGenerationCarParts
        If Not IsChild Then Return MyBase.Save()

        MarkAsParent()
        Dim savedObject = MyBase.Save()
        savedObject.MarkAsChild()
        Generation.ChangeReference(savedObject)
        Return savedObject
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationCarParts(ByVal generation As ModelGeneration) As ModelGenerationCarParts
        Dim _parts As ModelGenerationCarParts = New ModelGenerationCarParts
        _parts.SetParent(generation)
        Return _parts
    End Function
    Friend Shared Function GetModelGenerationCarParts(ByVal generation As ModelGeneration) As ModelGenerationCarParts
        Dim _parts As ModelGenerationCarParts = DataPortal.Fetch(Of ModelGenerationCarParts)(New CustomCriteria(generation))
        _parts.SetParent(generation)
        Return _parts
    End Function

#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        'Add Data Portal criteria here
        Private ReadOnly _generationID As Guid

        Public Sub New(ByVal generation As ModelGeneration)
            _generationID = generation.ID
            CommandText = "getGenerationCarParts"
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

        Me.MarkAsChild()
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationCarPart
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of ModelGenerationCarPart)
    Implements IUpdatableAssetSet
    Implements IOwnedBy

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _index As Integer

    Private _assetSet As AssetSet

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationCarParts).Generation
        End Get
    End Property

    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    Public ReadOnly Property Entity() As Entity Implements IHasAssetSet.Entity
        Get
            Return Entity.CARPART
        End Get
    End Property
    Public ReadOnly Property Index() As Integer
        Get
            Return _index
        End Get
    End Property

    Public ReadOnly Property AssetSet() As AssetSet Implements IHasAssetSet.AssetSet
        Get
            If _assetSet Is Nothing Then
                _assetSet = AssetSet.NewAssetSet(Me)
            End If
            Return _assetSet
        End Get
    End Property

    Public Sub ChangeReference(ByVal updatedAssetSet As AssetSet) Implements IUpdatableAssetSet.ChangeReference
        _assetSet = updatedAssetSet
    End Sub

    Public ReadOnly Property Owner() As String Implements IHasAssetSet.Owner
        Get
            Return Environment.GlobalCountryCode
        End Get
    End Property
    Public ReadOnly Property GenerationID() As Guid Implements IHasAssetSet.GenerationID
        Get
            Return Generation.ID
        End Get
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationCarPart) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As CarPart) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationCarPart Then
            Return Me.Equals(DirectCast(obj, ModelGenerationCarPart))
        ElseIf TypeOf obj Is CarPart Then
            Return Me.Equals(DirectCast(obj, CarPart))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assetSet Is Nothing) AndAlso Not _assetSet.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assetSet Is Nothing) AndAlso _assetSet.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationCarPart(ByVal carPart As CarPart) As ModelGenerationCarPart
        Dim _part As ModelGenerationCarPart = New ModelGenerationCarPart()
        _part.Create(carPart)
        Return _part
    End Function
#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        With MyContext.GetContext()
            Me.AllowNew = .IsGlobal
            Me.AllowEdit = False
            Me.AllowRemove = Me.AllowNew
        End With

    End Sub

#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal part As CarPart)
        MyBase.Create(part.ID)
        With part
            _code = .Code
            _name = .Name
            _index = .Index
        End With
        MarkNew()
    End Sub

    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("CARPARTID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("CARPARTCODE")
            _name = .GetString("CARPARTNAME")
            _index = .GetInt16("CARPARTSORTORDER")
            _assetSet = AssetSet.GetAssetSet(Me, dataReader)
        End With
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "insertGenerationCarPart"
        Me.AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "deleteGenerationCarPart"
        Me.AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@CARPARTID", Me.ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@GENERATIONID", Me.Generation.ID)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _assetSet Is Nothing Then _assetSet.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region



End Class
