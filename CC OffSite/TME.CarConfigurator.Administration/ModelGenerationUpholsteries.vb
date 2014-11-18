Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationUpholsteries
    Inherits BaseObjects.StronglySortedListBase(Of ModelGenerationUpholsteries, ModelGenerationUpholstery)

#Region " Business Properties & Methods "

    Friend Property Generation() As ModelGeneration
        Get
            Return DirectCast(Me.Parent, ModelGeneration)
        End Get
        Set(ByVal value As ModelGeneration)
            Me.SetParent(value)
        End Set
    End Property

    Default Public Overloads Overrides ReadOnly Property Item(ByVal code As String) As ModelGenerationUpholstery
        Get
            Return Me.FirstOrDefault(Function(x) x.Equals(code))
        End Get
    End Property
    Public Overloads Overrides Function Contains(ByVal code As String) As Boolean
        Return Me.Any(Function(x) x.Equals(code))
    End Function

#End Region

#Region " Move Methods "
    Public Overrides Function CanMoveUp(ByVal upholstery As ModelGenerationUpholstery) As Boolean
        If Not MyBase.CanMoveUp(upholstery) Then Return False
        Return Me.Item(Me.IndexOf(upholstery) - 1).Type.Equals(upholstery.Type)
    End Function
    Public Overrides Function CanMoveDown(ByVal upholstery As ModelGenerationUpholstery) As Boolean
        If Not MyBase.CanMoveDown(upholstery) Then Return False
        Return Me.Item(Me.IndexOf(upholstery) + 1).Type.Equals(upholstery.Type)
    End Function



    Friend Sub MoveToTheBottomOfTheType(ByVal upholstery As ModelGenerationUpholstery)
        If Me.Any(Function(x) x.Type.Equals(upholstery.Type) AndAlso Not x.ID.Equals(upholstery.ID)) Then

            Dim lastItemOfMyType = (From x In Me Where x.Type.Equals(upholstery.Type) AndAlso Not x.ID.Equals(upholstery.ID) Select x.Index).Max()
            If Me.IndexOf(upholstery) > lastItemOfMyType Then
                While CanMoveUp(upholstery)
                    MoveUp(upholstery)
                End While
            Else
                While CanMoveDown(upholstery)
                    MoveDown(upholstery)
                End While
            End If
        Else
            While CanMoveDown(upholstery)
                MoveDown(upholstery)
            End While
        End If
    End Sub
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationUpholsteriesy(ByVal generation As ModelGeneration) As ModelGenerationUpholsteries
        Dim _upholsteries As ModelGenerationUpholsteries = New ModelGenerationUpholsteries()
        _upholsteries.Generation = generation
        Return _upholsteries
    End Function
    Friend Shared Function GetModelGenerationUpholsteries(ByVal dataReader As SafeDataReader) As ModelGenerationUpholsteries
        Dim _upholsteries As ModelGenerationUpholsteries = New ModelGenerationUpholsteries()
        _upholsteries.Fetch(dataReader)
        Return _upholsteries
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("upholstery")> Public NotInheritable Class ModelGenerationUpholstery
    Inherits BaseObjects.TranslateableBusinessBase
    Implements IUpdatableAssetSet
    Implements IOwnedBy
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter
    Implements IStronglySortedObject

#Region " Business Properties & Methods "
    Private _objectId As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _aliases As Aliases
    Private _name As String = String.Empty
    Private _type As UpholsteryTypeInfo
    Private _interiorColour As InteriorColourInfo
    Private _trim As TrimInfo
    Private _index As Integer
    Private _assetSet As AssetSet

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationUpholsteries).Generation
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
    Public ReadOnly Property Aliases() As Aliases
        Get
            Return _aliases
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public Property Type() As UpholsteryTypeInfo
        Get
            Return _type
        End Get
        Set(ByVal value As UpholsteryTypeInfo)
            If Not Me.AllowEdit Then Exit Property
            If Not _type.Equals(value) Then
                _type = value

                DirectCast(Me.Parent, ModelGenerationUpholsteries).MoveToTheBottomOfTheType(Me)
                PropertyHasChanged("Type")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property InteriorColour() As InteriorColourInfo
        Get
            Return _interiorColour
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Trim() As TrimInfo
        Get
            Return _trim
        End Get
    End Property

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

    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return Environment.GlobalCountryCode
        End Get
    End Property

    Public Function GetInfo() As UpholsteryInfo
        Return UpholsteryInfo.GetUpholsteryInfo(Me)
    End Function
#End Region

#Region " Move Methods "
    Public Function CanMoveDown() As Boolean Implements IStronglySortedObject.CanMoveDown
        Return DirectCast(Me.Parent, ModelGenerationUpholsteries).CanMoveDown(Me)
    End Function
    Public Function CanMoveUp() As Boolean Implements IStronglySortedObject.CanMoveUp
        Return DirectCast(Me.Parent, ModelGenerationUpholsteries).CanMoveUp(Me)
    End Function
    Public Function MoveDown() As Boolean Implements IStronglySortedObject.MoveDown
        Return DirectCast(Me.Parent, ModelGenerationUpholsteries).MoveDown(Me)
    End Function
    Public Function MoveUp() As Boolean Implements IStronglySortedObject.MoveUp
        Return DirectCast(Me.Parent, ModelGenerationUpholsteries).MoveUp(Me)
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Code & " - " & Me.Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationUpholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As LinkedUpholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Upholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return (String.Compare(Me.Code, obj, True, Globalization.CultureInfo.InvariantCulture) = 0) OrElse Me.Aliases.Contains(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ObjectID.Equals(obj) OrElse Me.ID.Equals(obj)
    End Function

    Public Overloads Function Equals(ByVal interiorColourCode As String, ByVal trimCode As String) As Boolean
        Return InteriorColour.Equals(interiorColourCode) AndAlso Trim.Equals(trimCode)
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationUpholstery Then
            Return Me.Equals(DirectCast(obj, ModelGenerationUpholstery))
        ElseIf TypeOf obj Is LinkedUpholstery Then
            Return Me.Equals(DirectCast(obj, LinkedUpholstery))
        ElseIf TypeOf obj Is Upholstery Then
            Return Me.Equals(DirectCast(obj, Upholstery))
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

    Friend Shared Function NewModelGenerationUpholstery(ByVal generation As ModelGeneration, ByVal upholstery As Upholstery) As ModelGenerationUpholstery
        Dim _upholstery As ModelGenerationUpholstery = New ModelGenerationUpholstery()
        _upholstery.Create(generation, upholstery)
        Return _upholstery
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
    Private Overloads Sub Create(ByVal forGeneration As ModelGeneration, ByVal upholstery As Upholstery)
        MyBase.Create(upholstery.ID)
        _objectId = Guid.NewGuid()
        With upholstery
            _code = .Code
            _name = .Name
            _type = .Type
            _interiorColour = .InteriorColour
            _trim = .Trim
            _aliases = Aliases.GetAliases(.Aliases, True)
        End With
        MyBase.Copy(forGeneration, upholstery)
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _objectId = .GetGuid("OBJECTID")
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _index = .GetInt16("SORTORDER")
        End With
        _type = UpholsteryTypeInfo.GetUpholsteryTypeInfo(dataReader)
        _interiorColour = InteriorColourInfo.GetInteriorColourInfo(dataReader)
        _trim = TrimInfo.GetTrimInfo(dataReader)
        _assetSet = AssetSet.GetAssetSet(Me, dataReader)

        If _code.IndexOf(";") > 0 Then
            Dim _allCodes() As String = _code.Split(";"c)
            Dim _aliasCodes(_allCodes.GetUpperBound(0) - 1) As String
            _code = _allCodes(0)
            Array.Copy(_allCodes, 1, _aliasCodes, 0, _allCodes.GetUpperBound(0))
            _aliases = Aliases.GetAliases(_code, 6, _aliasCodes, True)
        Else
            _aliases = Aliases.NewAliases(_code, 6, True)
        End If
        Me.AllowEdit = Not (MyContext.GetContext().IsRegionCountry) OrElse MyContext.GetContext().IsMainRegionCountry
        If _objectId.Equals(Guid.Empty) Then
            _objectId = Guid.NewGuid()
            Me.AlwaysUpdateSelf = Me.AllowEdit
        End If
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "insertGenerationUpholstery"
        Me.AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateGenerationUpholstery"
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
        command.Parameters.AddWithValue("@UPHOLSTERYID", Me.ID)
        command.Parameters.AddWithValue("@UPHOLSTERYTYPEID", Me.Type.ID)
        command.Parameters.AddWithValue("@SORTORDER", Me.Index)
    End Sub
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
            Return Entity.CARUPHOLSTERY
        End Get
    End Property
#End Region

End Class
