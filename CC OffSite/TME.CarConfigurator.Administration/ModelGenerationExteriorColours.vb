Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationExteriorColours
    Inherits BaseObjects.StronglySortedListBase(Of ModelGenerationExteriorColours, ModelGenerationExteriorColour)

#Region " Business Properties & Methods "

    Friend Property Generation() As ModelGeneration
        Get
            Return DirectCast(Me.Parent, ModelGeneration)
        End Get
        Set(ByVal value As ModelGeneration)
            Me.SetParent(value)
        End Set
    End Property

#End Region

#Region " Move Methods "
    Public Overrides Function CanMoveUp(ByVal upholstery As ModelGenerationExteriorColour) As Boolean
        If Not MyBase.CanMoveUp(upholstery) Then Return False
        Return Me.Item(Me.IndexOf(upholstery) - 1).Type.Equals(upholstery.Type)
    End Function
    Public Overrides Function CanMoveDown(ByVal upholstery As ModelGenerationExteriorColour) As Boolean
        If Not MyBase.CanMoveDown(upholstery) Then Return False
        Return Me.Item(Me.IndexOf(upholstery) + 1).Type.Equals(upholstery.Type)
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationExteriorColours(ByVal generation As ModelGeneration) As ModelGenerationExteriorColours
        Dim _exteriorColours As ModelGenerationExteriorColours = New ModelGenerationExteriorColours()
        _exteriorColours.Generation = generation
        Return _exteriorColours
    End Function
    Friend Shared Function GetModelGenerationExteriorColours(ByVal dataReader As SafeDataReader) As ModelGenerationExteriorColours
        Dim _exteriorColours As ModelGenerationExteriorColours = New ModelGenerationExteriorColours()
        _exteriorColours.Fetch(dataReader)
        Return _exteriorColours
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("exteriorcolour")> Public NotInheritable Class ModelGenerationExteriorColour
    Inherits BaseObjects.TranslateableBusinessBase
    Implements IUpdatableAssetSet
    Implements IOwnedBy
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter
    Implements IStronglySortedObject

#Region " Business Properties & Methods "

    Private _objectId As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _type As ExteriorColourTypeInfo
    Private _promoted As Boolean
    Private _index As Integer
    Private _assetSet As AssetSet

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationExteriorColours).Generation
        End Get
    End Property

    Private ReadOnly Property ObjectID() As Guid
        Get
            Return _objectId
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
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Type() As ExteriorColourTypeInfo
        Get
            Return _type
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Property Promoted() As Boolean
        Get
            Return _promoted
        End Get
        Set(ByVal value As Boolean)
            If _promoted.Equals(value) Then Return

            If (value) Then DepromoteAllOtherColours()
            _promoted = value
            PropertyHasChanged("Promoted")
        End Set
    End Property
    Private Sub DepromoteAllOtherColours()
        Dim promotedColours = DirectCast(Me.Parent, ModelGenerationExteriorColours).Where(Function(x) x.Promoted AndAlso Not x.ID.Equals(Me.ID))
        For Each promotedColour In promotedColours
            promotedColour.Promoted = False
        Next
    End Sub

    Public ReadOnly Property Index() As Integer Implements BaseObjects.ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements BaseObjects.ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not Me.AllowEdit Then Exit Property
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property

    Protected Overrides Sub OnPropertyChanged(ByVal propertyName As String)
        MyBase.OnPropertyChanged(propertyName)
    End Sub

    Public ReadOnly Property AssetSet() As AssetSet Implements IHAsAssetSet.AssetSet
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

    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return Environment.GlobalCountryCode
        End Get
    End Property

    Public Function GetInfo() As ExteriorColourInfo
        Return ExteriorColourInfo.GetExteriorColourInfo(Me)
    End Function
#End Region

#Region " Move Methods "
    Public Function CanMoveDown() As Boolean Implements IStronglySortedObject.CanMoveDown
        Return DirectCast(Me.Parent, ModelGenerationExteriorColours).CanMoveDown(Me)
    End Function
    Public Function CanMoveUp() As Boolean Implements IStronglySortedObject.CanMoveUp
        Return DirectCast(Me.Parent, ModelGenerationExteriorColours).CanMoveUp(Me)
    End Function
    Public Function MoveDown() As Boolean Implements IStronglySortedObject.MoveDown
        Return DirectCast(Me.Parent, ModelGenerationExteriorColours).MoveDown(Me)
    End Function
    Public Function MoveUp() As Boolean Implements IStronglySortedObject.MoveUp
        Return DirectCast(Me.Parent, ModelGenerationExteriorColours).MoveUp(Me)
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Code & " - " & Me.Name
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As LinkedExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return (String.Compare(Me.Code, obj, True, Globalization.CultureInfo.InvariantCulture) = 0)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ObjectID.Equals(obj) OrElse Me.ID.Equals(obj)
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationExteriorColour Then
            Return Me.Equals(DirectCast(obj, ModelGenerationExteriorColour))
        ElseIf TypeOf obj Is LinkedExteriorColour Then
            Return Me.Equals(DirectCast(obj, LinkedExteriorColour))
        ElseIf TypeOf obj Is ExteriorColour Then
            Return Me.Equals(DirectCast(obj, ExteriorColour))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
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

    Friend Shared Function NewModelGenerationExteriorColour(ByVal generation As ModelGeneration, ByVal exteriorColour As ExteriorColour) As ModelGenerationExteriorColour
        Dim _exteriorColour As ModelGenerationExteriorColour = New ModelGenerationExteriorColour()
        _exteriorColour.Create(generation, exteriorColour)
        _exteriorColour.MarkAsChild()
        Return _exteriorColour
    End Function


#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal forGeneration As ModelGeneration, ByVal exteriorColour As ExteriorColour)
        MyBase.Create(exteriorColour.ID)
        _objectId = Guid.NewGuid()
        With exteriorColour
            _code = .Code
            _name = .Name
            _type = .Type
            _promoted = False
        End With

        MyBase.Copy(forGeneration, exteriorColour)
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _objectId = .GetGuid("OBJECTID")
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _type = ExteriorColourTypeInfo.GetExteriorColourTypeInfo(dataReader)
            _promoted = ((.GetInt16("STATUSID") And Status.Promoted) = Status.Promoted)
            _index = .GetInt16("SORTORDER")
            _assetSet = AssetSet.GetAssetSet(Me, dataReader)
        End With
        Me.AllowEdit = Not (MyContext.GetContext().IsRegionCountry) OrElse MyContext.GetContext().IsMainRegionCountry
        If _objectId.Equals(Guid.Empty) Then
            _objectId = Guid.NewGuid()
            Me.AlwaysUpdateSelf = Me.AllowEdit
        End If
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "insertGenerationExteriorColour"
        Me.AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateGenerationExteriorColour"
        Me.AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", Me.ObjectID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@GENERATIONID", Me.Generation.ID)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", Me.ID)
        command.Parameters.AddWithValue("@STATUSID", GetStatusID())
        command.Parameters.AddWithValue("@SORTORDER", Me.Index)
    End Sub
    Private Function GetStatusID() As Integer
        If Me.Promoted Then Return (Status.ApprovedForLive + Status.AvailableToNMSCs + Status.ApprovedForPreview + Status.Promoted)
        Return (Status.ApprovedForLive + Status.AvailableToNMSCs + Status.ApprovedForPreview)
    End Function

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _assetSet Is Nothing Then _assetSet.Update(transaction)
    End Sub

#End Region

#Region " Base Object Overrides "

    Public Overrides ReadOnly Property ModelID() As Guid
        Get
            Return Generation.Model.ID
        End Get
    End Property

    Public Overrides ReadOnly Property GenerationID() As Guid
        Get
            Return Generation.ID
        End Get
    End Property

    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.CAREXTERIORCOLOUR
        End Get
    End Property
#End Region

End Class
