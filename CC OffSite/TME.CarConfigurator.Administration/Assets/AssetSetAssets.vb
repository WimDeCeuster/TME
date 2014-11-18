Imports TME.CarConfigurator.Administration.Enums

Namespace Assets

    <Serializable()> Public NotInheritable Class AssetSetAssets
        Inherits ContextUniqueGuidListBase(Of AssetSetAssets, AssetSetAsset)

#Region " Business Properties & Methods "
        Friend Property AssetSet() As AssetSet
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, AssetSet)
            End Get
            Set(ByVal value As AssetSet)
                SetParent(value)
                SetSecurityRights()
            End Set
        End Property

        Friend Sub SetSecurityRights()
            With AssetSet
                AllowNew = .AllowRemove
                AllowEdit = .AllowRemove
                AllowRemove = .AllowRemove

                For Each asset In Me
                    asset.SetSecurityRights()
                Next
            End With
        End Sub

        Friend Sub Localize()
            AllowNew = True
            AllowEdit = True
            AllowRemove = True

            For Each asset In Me
                asset.Localize()
            Next

        End Sub
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function NewAssetSetAssets(ByVal assetSet As AssetSet) As AssetSetAssets
            Dim assets As AssetSetAssets = New AssetSetAssets
            assets.SetParent(assetSet)
            assets.SetSecurityRights()
            Return assets
        End Function
        Friend Shared Function GetAssetSetAssets(ByVal assetSet As AssetSet) As AssetSetAssets
            Dim assets As AssetSetAssets = DataPortal.Fetch(Of AssetSetAssets)(New CustomCriteria(assetSet))
            assets.SetParent(assetSet)
            assets.SetSecurityRights()
            Return assets
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            MarkAsChild()
        End Sub
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            'Add Data Portal criteria here
            Private ReadOnly _objectID As Guid
            Private _assetSetID As Guid

            Public Sub New(ByVal assetSet As AssetSet)
                _objectID = assetSet.ObjectID
                If assetSet.WasNew Then
                    _assetSetID = Guid.Empty
                Else
                    _assetSetID = assetSet.ID
                End If
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@ASSETSETID", _assetSetID.GetDbValue())
                command.Parameters.AddWithValue("@OBJECTID", _objectID)
            End Sub
        End Class
#End Region


    End Class
    <Serializable(), XmlInfo("asset")> Public NotInheritable Class AssetSetAsset
        Inherits ContextUniqueGuidBusinessBase(Of AssetSetAsset)

#Region " Business Properties & Methods "

        Private _assetType As AssetType = Nothing
        Private _assetInfo As AssetInfo = Nothing
        Private _name As String = String.Empty
        Private _bodyType As BodyTypeInfo = Nothing
        Private _engine As EngineInfo = Nothing
        Private _grade As GradeInfo = Nothing
        Private _transmission As TransmissionInfo = Nothing
        Private _steering As SteeringInfo = Nothing
        Private _wheelDrive As WheelDriveInfo = Nothing
        Private _exteriorColour As ExteriorColourInfo = Nothing
        Private _upholstery As UpholsteryInfo = Nothing
        Private _equipmentItem As EquipmentItemInfo = Nothing
        <NonSerialized()> Private _IsCrossModelAsset As Boolean = False
        Private _alwaysInclude As Boolean = True

        Public Property AssetType() As AssetType
            Get
                Return _assetType
            End Get
            Set(ByVal value As AssetType)
                If _assetType Is Nothing OrElse Not _assetType.Equals(value) Then
                    _assetType = value
                    PropertyHasChanged("AssetType")
                End If
            End Set
        End Property

        Public Property Asset() As AssetInfo
            Get
                Return _assetInfo
            End Get
            Set(ByVal value As AssetInfo)
                If _assetInfo Is Nothing OrElse Not _assetInfo.Equals(value) Then
                    _assetInfo = value
                    PropertyHasChanged("Asset")
                Else
                    _assetInfo = value
                End If
            End Set
        End Property

        Friend ReadOnly Property Key() As String 'Used for determining uniqueness
            Get
                If AssetType Is Nothing Then Return Name
                Return String.Format("{0}, {1}", AssetType.Name, Name)
            End Get
        End Property

        Public Property Name() As String
            Get
                If _name.Length = 0 Then
                    _name = AppendString(_name, GetPartialCarSpecification().ToString())
                    If Not ExteriorColour Is Nothing Then _name = AppendString(_name, ExteriorColour.Name)
                    If Not Upholstery Is Nothing Then _name = AppendString(_name, Upholstery.Name)
                    If Not EquipmentItem Is Nothing Then
                        If EquipmentItem.Type = EquipmentType.Accessory Then
                            _name = AppendString(_name, String.Format("{0} - {1}", EquipmentItem.Partnumber, EquipmentItem.Name))
                        Else
                            _name = AppendString(_name, EquipmentItem.Name)
                        End If
                    End If
                    If _name.Length = 0 Then _name = "(base)"
                End If
                Return _name
            End Get
            Private Set(ByVal value As String)
                _name = value
            End Set
        End Property

        Public Property BodyType() As BodyTypeInfo
            Get
                Return _bodyType
            End Get
            Set(ByVal value As BodyTypeInfo)
                If _bodyType Is Nothing OrElse Not _bodyType.Equals(value) Then
                    _bodyType = value
                    PropertyHasChanged("BodyType")
                End If
            End Set
        End Property

        Public Property Engine() As EngineInfo
            Get
                Return _engine
            End Get
            Set(ByVal value As EngineInfo)
                If _engine Is Nothing OrElse Not _engine.Equals(value) Then
                    _engine = value
                    PropertyHasChanged("Engine")
                End If
            End Set
        End Property

        Public Property Grade() As GradeInfo
            Get
                Return _grade
            End Get
            Set(ByVal value As GradeInfo)
                If _grade Is Nothing OrElse Not _grade.Equals(value) Then
                    _grade = value
                    PropertyHasChanged("Grade")
                End If
            End Set
        End Property

        Public Property Transmission() As TransmissionInfo
            Get
                Return _transmission
            End Get
            Set(ByVal value As TransmissionInfo)
                If _transmission Is Nothing OrElse Not _transmission.Equals(value) Then
                    _transmission = value
                    PropertyHasChanged("Transmission")
                End If
            End Set
        End Property

        Public Property WheelDrive() As WheelDriveInfo
            Get
                Return _wheelDrive
            End Get
            Set(ByVal value As WheelDriveInfo)
                If _wheelDrive Is Nothing OrElse Not _wheelDrive.Equals(value) Then
                    _wheelDrive = value
                    PropertyHasChanged("WheelDrive")
                End If
            End Set
        End Property

        Public Property Steering() As SteeringInfo
            Get
                Return _steering
            End Get
            Set(ByVal value As SteeringInfo)
                If _steering Is Nothing OrElse Not _steering.Equals(value) Then
                    _steering = value
                    PropertyHasChanged("Steering")
                End If
            End Set
        End Property

        Public Property ExteriorColour() As ExteriorColourInfo
            Get
                Return _exteriorColour
            End Get
            Set(ByVal value As ExteriorColourInfo)
                If _exteriorColour Is Nothing OrElse Not _exteriorColour.Equals(value) Then
                    _exteriorColour = value
                    PropertyHasChanged("ExteriorColour")
                End If
            End Set
        End Property

        Public Property Upholstery() As UpholsteryInfo
            Get
                Return _upholstery
            End Get
            Set(ByVal value As UpholsteryInfo)
                If _upholstery Is Nothing OrElse Not _upholstery.Equals(value) Then
                    _upholstery = value
                    PropertyHasChanged("Upholstery")
                End If
            End Set
        End Property

        Public Property EquipmentItem() As EquipmentItemInfo
            Get
                Return _equipmentItem
            End Get
            Set(ByVal value As EquipmentItemInfo)
                If _equipmentItem Is Nothing OrElse Not _equipmentItem.Equals(value) Then
                    _equipmentItem = value
                    PropertyHasChanged("EquipmentItem")
                End If
            End Set
        End Property

        Public ReadOnly Property IsCrossModelAsset() As Boolean
            Get
                Return _IsCrossModelAsset
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)>
        Public Property AlwaysInclude() As Boolean
            Get
                Return _alwaysInclude
            End Get
            Set(ByVal value As Boolean)
                If Not _alwaysInclude.Equals(value) Then
                    _alwaysInclude = value
                    PropertyHasChanged("AlwaysInclude")
                End If
            End Set
        End Property

        Public Function IsDeviation() As Boolean
            If Not BodyType.IsEmpty() Then Return True
            If Not Engine.IsEmpty() Then Return True
            If Not Grade.IsEmpty() Then Return True
            If Not Transmission.IsEmpty() Then Return True
            If Not WheelDrive.IsEmpty() Then Return True
            If Not Steering.IsEmpty() Then Return True
            If Not ExteriorColour.IsEmpty() Then Return True
            If Not Upholstery.IsEmpty() Then Return True
            If Not EquipmentItem.IsEmpty() Then Return True
            Return False
        End Function

        Private ReadOnly Property AssetSet() As AssetSet
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, AssetSetAssets).AssetSet
            End Get
        End Property

        Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
            If propertyName.Equals("Steering") AndAlso Not MyContext.GetContext().IsGlobal Then Return False
            Return MyBase.CanWriteProperty(propertyName)
        End Function

        Protected Overrides Sub PropertyHasChanged(ByVal propertyName As String)
            Dim ignoreNameChangeProps() As String = {"AssetType", "Asset", "AlwaysInclude"}
            If Not ignoreNameChangeProps.Any(Function(ignoreName) ignoreName.Equals(propertyName)) Then
                Name = String.Empty
                'trigger recalculation
            End If
            MyBase.PropertyHasChanged(propertyName)
            ValidationRules.CheckRules("Key")
        End Sub


        Friend Sub SetSecurityRights()
            With AssetSet
                AllowNew = AllowNew AndAlso .AllowRemove
                AllowEdit = AllowEdit AndAlso .AllowRemove
                AllowRemove = AllowRemove AndAlso .AllowRemove
            End With
        End Sub

        Public Sub Localize()
            AllowNew = True
            AllowEdit = True
            If Not IsCrossModelAsset Then
                AllowRemove = True
            End If
            ID = Guid.NewGuid()
            MarkNew()
        End Sub

        Friend Function Copy() As AssetSetAsset
            Dim clone = Me.Clone()
            clone.AllowNew = True
            clone.AllowEdit = True
            clone.AllowRemove = True
            clone.ID = Guid.NewGuid()
            clone.MarkNew()
            Return clone
        End Function


#Region " Helper Methods "

        Private Function AppendString(ByVal source As String, ByVal toAppend As String) As String
            If source.Length = 0 Then Return toAppend
            If toAppend.Length = 0 Then Return source

            Return String.Format("{0}, {1}", source, toAppend)
        End Function

        Private Function GetPartialCarSpecification() As PartialCarSpecification
            Dim carSpecification As PartialCarSpecification = PartialCarSpecification.NewPartialCarSpecification
            With carSpecification
                If Not BodyType Is Nothing Then
                    .BodyTypeID = BodyType.ID
                    .BodyTypeName = BodyType.Name
                End If
                If Not Engine Is Nothing Then
                    .EngineID = Engine.ID
                    .EngineName = Engine.Name
                End If
                If Not Grade Is Nothing Then
                    .GradeID = Grade.ID
                    .GradeName = Grade.Name
                End If
                If Not Transmission Is Nothing Then
                    .TransmissionID = Transmission.ID
                    .TransmissionName = Transmission.Name
                End If
                If Not WheelDrive Is Nothing Then
                    .WheelDriveID = WheelDrive.ID
                    .WheelDriveName = WheelDrive.Name
                End If
                If Not Steering Is Nothing Then
                    .SteeringID = Steering.ID
                    .SteeringName = Steering.Name
                End If
            End With
            Return (carSpecification)
        End Function

#End Region

#End Region

#Region " Business & Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, Validation.RuleHandler), "AssetType")
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, Validation.RuleHandler), "Asset")
            ValidationRules.AddRule(DirectCast(AddressOf KeyUnique, Validation.RuleHandler), "Key")
        End Sub

        Private Shared Function KeyUnique(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            Dim asset = DirectCast(target, AssetSetAsset)
            If asset.Parent Is Nothing Then Return True

            If DirectCast(asset.Parent, AssetSetAssets).Any(Function(x) x.Key.Equals(asset.Key) AndAlso Not x.ID.Equals(asset.ID)) Then
                e.Description = String.Format("An other asset already exists for the same parameters ({0})", asset.Key)
                Return False
            End If

            Return True
        End Function

#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Name
        End Function

#End Region

#Region " Constructors "

        Private Sub New()
            'Prevent direct creation
        End Sub

#End Region

#Region " Data Access "

        Protected Overrides Sub InitializeFields()
            _bodyType = BodyTypeInfo.Empty
            _engine = EngineInfo.Empty
            _grade = GradeInfo.Empty
            _transmission = TransmissionInfo.Empty
            _wheelDrive = WheelDriveInfo.Empty
            _steering = SteeringInfo.Empty
            _exteriorColour = ExteriorColourInfo.Empty
            _upholstery = UpholsteryInfo.Empty
            _equipmentItem = EquipmentItemInfo.Empty
            MyBase.InitializeFields()
        End Sub

        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                _assetType = AssetType.GetAssetType(dataReader)
                _assetInfo = AssetInfo.GetAssetInfo(dataReader)
                _bodyType = BodyTypeInfo.GetBodyTypeInfo(dataReader)
                _engine = EngineInfo.GetEngineInfo(dataReader)
                _grade = GradeInfo.GetGradeInfo(dataReader)
                _transmission = TransmissionInfo.GetTransmissionInfo(dataReader)
                _wheelDrive = WheelDriveInfo.GetWheelDriveInfo(dataReader)
                _steering = SteeringInfo.GetSteeringInfo(dataReader)
                _exteriorColour = ExteriorColourInfo.GetExteriorColourInfo(dataReader)
                _upholstery = UpholsteryInfo.GetUpholsteryInfo(dataReader)
                _equipmentItem = EquipmentItemInfo.GetEquipmentInfo(dataReader)
                _alwaysInclude = dataReader.GetBoolean(GetFieldName("ALWAYSINCLUDE"))
            End With
            MyBase.FetchFields(dataReader)

            If ID.Equals(Guid.Empty) Then
                'This is a cross model image
                ID = Guid.NewGuid()
                AllowRemove = False
                _IsCrossModelAsset = True
            End If

        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            AddCommandFields(command)
        End Sub

        Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            AddCommandFields(command)
        End Sub


        Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            With command.Parameters
                .AddWithValue("@ASSETSETID", AssetSet.ID)
                .AddWithValue("@ASSETTYPE", Integer.Parse(AssetType.Code))
                .AddWithValue("@BODYID", BodyType.ID.GetDbValue())
                .AddWithValue("@ENGINEID", Engine.ID.GetDbValue())
                .AddWithValue("@GRADEID", Grade.ID.GetDbValue())
                .AddWithValue("@TRANSMISSIONID", Transmission.ID.GetDbValue())
                .AddWithValue("@WHEELDRIVEID", WheelDrive.ID.GetDbValue())
                .AddWithValue("@STEERINGID", Steering.ID.GetDbValue())
                .AddWithValue("@EXTERIORCOLOURID", ExteriorColour.ID.GetDbValue())
                .AddWithValue("@UPHOLSTERYID", Upholstery.ID.GetDbValue())
                .AddWithValue("@EQUIPMENTID", EquipmentItem.ID.GetDbValue())
                .AddWithValue("@ASSETID", Asset.ID)
                .AddWithValue("@ALWAYSINCLUDE", AlwaysInclude)
            End With
        End Sub

        Private Sub AssetSetAfterUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.AfterUpdateCommand, Me.AfterInsertCommand
            _IsCrossModelAsset = False
            AllowRemove = True
            SetSecurityRights()
        End Sub
#End Region
    End Class
End NameSpace