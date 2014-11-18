Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationWheelDrives
    Inherits StronglySortedListBase(Of ModelGenerationWheelDrives, ModelGenerationWheelDrive)

#Region " Business Properties & Methods "
    Friend Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGeneration)
        End Get
        Private Set(ByVal value As ModelGeneration)
            SetParent(value)
        End Set
    End Property

    Private Shadows Sub Add(ByVal wheelDrive As WheelDriveInfo)
        If Contains(wheelDrive) Then Throw New Exceptions.ObjectAlreadyExists(Entity.WHEELDRIVE, wheelDrive)
        MyBase.Add(ModelGenerationWheelDrive.NewModelGenerationWheelDrive(wheelDrive))
    End Sub
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationWheelDrives(ByVal generation As ModelGeneration) As ModelGenerationWheelDrives
        Dim wheelDrives As ModelGenerationWheelDrives = New ModelGenerationWheelDrives()
        wheelDrives.Generation = generation
        Return wheelDrives
    End Function
    Friend Shared Function GetModelGenerationWheelDrives(ByVal generation As ModelGeneration, ByVal dataReader As SafeDataReader) As ModelGenerationWheelDrives
        Dim wheelDrives As ModelGenerationWheelDrives = New ModelGenerationWheelDrives()
        wheelDrives.Generation = generation
        wheelDrives.Fetch(dataReader)
        Return wheelDrives
    End Function
    Friend Shared Function GetModelGenerationWheelDrives(ByVal generation As ModelGeneration) As ModelGenerationWheelDrives
        Dim wheelDrives As ModelGenerationWheelDrives = DataPortal.Fetch(Of ModelGenerationWheelDrives)(New CustomCriteria(generation))
        wheelDrives.Generation = generation
        Return wheelDrives
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AllowNew = False
        AllowRemove = False
        AllowEdit = Not MyContext.GetContext().IsSlaveRegionCountry
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid

        Public Sub New(ByVal generation As ModelGeneration)
            _generationID = generation.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
        End Sub

    End Class
#End Region

#Region " Data Access "

    Friend Sub Synchronize()
        If Not AllowEdit Then Exit Sub

        AddMissingObjects()
        RemoveObjectsThatNoLongerExist()
        UpdateObjectStatuses()
    End Sub
    Private Sub AddMissingObjects()
        Dim toBeAdded = Generation.Cars.Where(Function(car) Not Contains(car.WheelDriveID)).Select(Function(car) car.WheelDriveID).Distinct().ToList()
        If Not toBeAdded.Any() Then Return

        Dim initialAllowNewValue = AllowNew
        Dim list = WheelDrives.GetWheelDrives()

        AllowNew = True
        For Each id In toBeAdded
            Add(list(id).GetInfo())
        Next
        AllowNew = initialAllowNewValue
    End Sub
    Private Sub RemoveObjectsThatNoLongerExist()
        Dim toBeRemoved = Where(Function(wheelDrive) Not Generation.Cars.Any(Function(car) car.WheelDriveID.Equals(wheelDrive.ID))).ToList()
        Dim initialAllowRemoveValue = AllowRemove

        AllowRemove = True
        For Each obj In toBeRemoved
            obj.MakeRemovable()
            Remove(obj)
        Next
        AllowRemove = initialAllowRemoveValue
    End Sub
    Private Sub UpdateObjectStatuses()
        For Each wheelDrive In Me
            Dim availableCars = Generation.Cars.Where(Function(car) car.WheelDriveID.Equals(wheelDrive.ID)).ToList()
            wheelDrive.Approved = availableCars.Any(Function(car) car.Approved)
            wheelDrive.Preview = availableCars.Any(Function(car) car.Preview)
        Next
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationWheelDrive
    Inherits TranslateableBusinessBase
    Implements ISortedIndex
    Implements ISortedIndexSetter
    Implements IUpdatableAssetSet
    Implements IOwnedBy

#Region " Business Properties & Methods "
    Private _objectID As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _status As Integer
    Private _keyFeature As Boolean
    Private _brochure As Boolean
    Private _index As Integer
    Private _assetSet As AssetSet

    Public Sub MakeRemovable()
        AllowRemove = True
    End Sub
    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationWheelDrives).Generation
        End Get
    End Property

    Public ReadOnly Property ObjectID() As Guid
        Get
            Return _objectID
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
    Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property
    Public Property KeyFeature() As Boolean
        Get
            Return _keyFeature
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(KeyFeature) Then
                _keyFeature = value
                PropertyHasChanged("KeyFeature")
            End If
        End Set
    End Property
    Public Property Brochure() As Boolean
        Get
            Return _brochure
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Brochure) Then
                _brochure = value
                PropertyHasChanged("Brochure")
            End If
        End Set
    End Property

    Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Approved) Then
                If Approved Then
                    _status -= Status.ApprovedForLive
                    If Not Declined Then _status += Status.Declined
                Else
                    _status += Status.ApprovedForLive
                    If Declined Then _status -= Status.Declined
                End If
                PropertyHasChanged("Approved")
            End If
        End Set
    End Property
    Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Declined) Then
                If Declined Then
                    _status -= Status.Declined
                    If Not Approved Then _status += Status.ApprovedForLive
                Else
                    _status += Status.Declined
                    If Approved Then _status -= Status.ApprovedForLive
                End If
                PropertyHasChanged("Declined")
            End If
        End Set
    End Property
    Public Property Preview() As Boolean
        Get
            Return ((_status And Status.ApprovedForPreview) = Status.ApprovedForPreview)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Preview) Then
                If Preview Then
                    _status -= Status.ApprovedForPreview
                Else
                    _status += Status.ApprovedForPreview
                End If
                PropertyHasChanged("Preview")
            End If
        End Set
    End Property
    Public ReadOnly Property Visible() As Boolean
        Get
            Return Approved OrElse Preview
        End Get
    End Property

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

    Public Function GetInfo() As WheelDriveInfo
        Return WheelDriveInfo.GetWheelDriveInfo(Me)
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name.ToString()
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationWheelDrive) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As WheelDriveInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As WheelDrive) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj) OrElse ObjectID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationWheelDrive Then
            Return Equals(DirectCast(obj, ModelGenerationWheelDrive))
        ElseIf TypeOf obj Is WheelDriveInfo Then
            Return Equals(DirectCast(obj, WheelDriveInfo))
        ElseIf TypeOf obj Is WheelDrive Then
            Return Equals(DirectCast(obj, WheelDrive))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
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
    Friend Shared Function NewModelGenerationWheelDrive(ByVal wheelDrive As WheelDriveInfo) As ModelGenerationWheelDrive
        Dim modelGenerationWheelDrive = New ModelGenerationWheelDrive()
        modelGenerationWheelDrive.Create(wheelDrive)
        Return modelGenerationWheelDrive
    End Function
#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
        AlwaysUpdateSelf = True
        AllowNew = False
        AllowRemove = False
        With MyContext.GetContext()
            AllowEdit = Not .IsRegionCountry OrElse .IsMainRegionCountry
            Me.AlwaysUpdateSelf = Me.AllowEdit
        End With
    End Sub

#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal wheelDrive As WheelDriveInfo)
        Create(wheelDrive.ID)
        With wheelDrive
            _objectID = Guid.NewGuid()
            _code = .Code
            _name = .Name
            _status = (Status.AvailableToNmscs + Status.Declined)
            _index = 0
        End With
        AllowNew = True
        AllowRemove = True
        MarkNew()
    End Sub
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("WHEELDRIVEID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _objectID = .GetGuid("ID")
            _code = .GetString("WHEELDRIVECODE")
            _name = .GetString("WHEELDRIVENAME")
            _status = .GetInt16("STATUSID")
            _keyFeature = .GetBoolean("KEYFEATURE")
            _brochure = .GetBoolean("BROCHURE")
            _index = .GetInt16("SORTORDER")
            _assetSet = AssetSet.GetAssetSet(Me, dataReader)
        End With
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateModelGenerationWheelDrive"
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", ObjectID)
    End Sub


    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@GENERATIONID", Generation.ID)
        command.Parameters.AddWithValue("@WHEELDRIVEID", ID)
        command.Parameters.AddWithValue("@STATUSID", _status)
        command.Parameters.AddWithValue("@KEYFEATURE", KeyFeature)
        command.Parameters.AddWithValue("@BROCHURE", Brochure)
        command.Parameters.AddWithValue("@SORTORDER", Index)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _assetSet Is Nothing Then _assetSet.Update(transaction)
    End Sub
#End Region

#Region "Base Object Overrides"

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
        Return Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.MODELGENERATIONWHEELDRIVE
        End Get
    End Property
#End Region

End Class