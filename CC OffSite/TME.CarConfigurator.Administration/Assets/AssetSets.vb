Imports TME.BusinessObjects.Server
Imports TME.BusinessObjects.Templates.Exceptions
Imports TME.CarConfigurator.Administration.Components.Interfaces
Imports TME.CarConfigurator.Administration.Enums

Namespace Assets

    <Serializable()> Public NotInheritable Class AssetSets
        Inherits ContextUniqueGuidListBase(Of AssetSets, AssetSet)

#Region " Shared Factory Methods "
        Public Shared Function GetLocalizedVersions(ByVal generationID As Guid, ByVal objectID As Guid) As AssetSets
            Return BusinessObjects.DataPortal.Fetch(Of AssetSets)(New CustomCriteria(generationID, objectID))
        End Function
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            'Add Data Portal criteria here
            Private ReadOnly _generationID As Guid
            Private ReadOnly _objectID As Guid

            Public Sub New(ByVal generationID As Guid, ByVal objectID As Guid)
                _generationID = generationID
                _objectID = objectID
                CommandText = "getLocalizedVersions"
            End Sub

            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@GENERATIONID", _generationID)
                command.Parameters.AddWithValue("@OBJECTID", _objectID)
            End Sub
        End Class
#End Region

    End Class
    <Serializable()> Public NotInheritable Class AssetSet
        Inherits ContextUniqueGuidBusinessBase(Of AssetSet)
        Implements IOwnedBy

#Region " Business Properties & Methods "
        Private _localizedID As Guid = Guid.Empty
        Private _wasNew As Boolean

        Private _generationID As Guid
        Private _objectID As Guid
        Private _entity As Entity
        Private _owner As String

        Private _numberOfLocalizedVersions As Short
        Private _numberOfAssets As Short
        Private _previewAvailable As Boolean
        Private _exteriorSpinAvailable As Boolean
        Private _interiorSpinAvailable As Boolean
        Private _xRayHybridSpinAvailable As Boolean
        Private _xRay4X4SpinAvailable As Boolean
        Private _xRaySafetySpinAvailable As Boolean
        Private WithEvents _assets As AssetSetAssets
        Private _ownerObject As IHasAssetSet

        Private Property LocalizedID() As Guid
            Get
                Return _localizedID
            End Get
            Set(ByVal value As Guid)
                _localizedID = value
            End Set
        End Property
        Friend Property WasNew() As Boolean
            Get
                Return _wasNew
            End Get
            Private Set(ByVal value As Boolean)
                _wasNew = value
            End Set
        End Property

        Private Property GenerationID() As Guid
            Get
                Return _generationID
            End Get
            Set(ByVal value As Guid)
                _generationID = value
            End Set
        End Property
        Public Property ObjectID() As Guid
            Get
                Return _objectID
            End Get
            Private Set(ByVal value As Guid)
                _objectID = value
            End Set
        End Property

        Public Property Entity() As Entity
            Get
                Return _entity
            End Get
            Private Set(ByVal value As Entity)
                _entity = value
            End Set
        End Property
        Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
            Get
                Return _owner
            End Get
        End Property

        Private ReadOnly Property ParentOwner() As String
            Get
                Return If(_ownerObject.Owner IsNot Nothing, _ownerObject.Owner, Environment.GlobalCountryCode)
            End Get
        End Property
        Public Property NumberOfAssets() As Short
            Get
                If _assets Is Nothing Then Return _numberOfAssets
                Return Convert.ToInt16(_assets.Count)
            End Get
            Private Set(ByVal value As Short)
                _numberOfAssets = value
            End Set
        End Property
        Public Property NumberOfLocalizedVersions() As Short
            Get
                Return _numberOfLocalizedVersions
            End Get
            Private Set(ByVal value As Short)
                _numberOfLocalizedVersions = value
            End Set
        End Property
        Public Property PreviewAvailable() As Boolean
            Get
                Return _previewAvailable
            End Get
            Private Set(ByVal value As Boolean)
                If _previewAvailable.Equals(value) Then Return

                _previewAvailable = value
                PropertyHasChanged("PreviewAvailable")
            End Set
        End Property
        Public Property ExteriorSpinAvailable() As Boolean
            Get
                If Not TypeOf _ownerObject Is ICanHaveComponentsWithAssets OrElse _exteriorSpinAvailable Then Return _exteriorSpinAvailable
                Return DirectCast(_ownerObject, ICanHaveComponentsWithAssets).Components.Any(Function(component) component.AssetSet.ExteriorSpinAvailable)
            End Get
            Private Set(ByVal value As Boolean)
                If _exteriorSpinAvailable.Equals(value) Then Return

                _exteriorSpinAvailable = value
                PropertyHasChanged("ExteriorSpinAvailable")
            End Set
        End Property
        Public Property InteriorSpinAvailable() As Boolean
            Get
                If Not TypeOf _ownerObject Is ICanHaveComponentsWithAssets OrElse _interiorSpinAvailable Then Return _interiorSpinAvailable
                Return DirectCast(_ownerObject, ICanHaveComponentsWithAssets).Components.Any(Function(component) component.AssetSet.InteriorSpinAvailable)
            End Get
            Private Set(ByVal value As Boolean)
                If _interiorSpinAvailable.Equals(value) Then Return

                _interiorSpinAvailable = value
                PropertyHasChanged("InteriorSpinAvailable")
            End Set
        End Property
        Public Property XRayHybridSpinAvailable() As Boolean
            Get
                If Not TypeOf _ownerObject Is ICanHaveComponentsWithAssets OrElse _xRayHybridSpinAvailable Then Return _xRayHybridSpinAvailable
                Return DirectCast(_ownerObject, ICanHaveComponentsWithAssets).Components.Any(Function(component) component.AssetSet.XRayHybridSpinAvailable)
            End Get
            Private Set(ByVal value As Boolean)
                If _xRayHybridSpinAvailable.Equals(value) Then Return

                _xRayHybridSpinAvailable = value
                PropertyHasChanged("XRayHybridSpinAvailable")
            End Set
        End Property
        Public Property XRay4X4SpinAvailable() As Boolean
            Get
                If Not TypeOf _ownerObject Is ICanHaveComponentsWithAssets OrElse _xRay4X4SpinAvailable Then Return _xRay4X4SpinAvailable
                Return DirectCast(_ownerObject, ICanHaveComponentsWithAssets).Components.Any(Function(component) component.AssetSet.XRay4X4SpinAvailable)
            End Get
            Private Set(ByVal value As Boolean)
                If _xRay4X4SpinAvailable.Equals(value) Then Return

                _xRay4X4SpinAvailable = value
                PropertyHasChanged("XRay4X4SpinAvailable")
            End Set
        End Property
        Public Property XRaySafetySpinAvailable() As Boolean
            Get
                If Not TypeOf _ownerObject Is ICanHaveComponentsWithAssets OrElse _xRaySafetySpinAvailable Then Return _xRaySafetySpinAvailable
                Return DirectCast(_ownerObject, ICanHaveComponentsWithAssets).Components.Any(Function(component) component.AssetSet.XRaySafetySpinAvailable)
            End Get
            Private Set(ByVal value As Boolean)
                If _xRaySafetySpinAvailable.Equals(value) Then Return

                _xRaySafetySpinAvailable = value
                PropertyHasChanged("XRaySafetySpinAvailable")
            End Set
        End Property


        Public Property Assets() As AssetSetAssets
            Get
                If _assets Is Nothing Then
                    If _numberOfAssets.Equals(0) Then
                        _assets = AssetSetAssets.NewAssetSetAssets(Me)
                    Else
                        _assets = AssetSetAssets.GetAssetSetAssets(Me)
                    End If
                End If
                Return _assets
            End Get
            Private Set(ByVal value As AssetSetAssets)
                _assets = value
                If _assets IsNot Nothing Then _assets.AssetSet = Me
            End Set
        End Property

        Private Sub AssetsListChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles _assets.ListChanged
            RefreshProperties()
        End Sub

        Private Sub RefreshProperties()
            PreviewAvailable = Assets.Any(Function(x) x.AssetType IsNot Nothing AndAlso x.AssetType.CanBeUsedAsPreview)
            ExteriorSpinAvailable = Assets.Any(Function(x) x.AssetType IsNot Nothing AndAlso x.AssetType.Group.UsedByExteriorSpin)
            InteriorSpinAvailable = Assets.Any(Function(x) x.AssetType IsNot Nothing AndAlso x.AssetType.Group.UsedByInteriorSpin)
            XRayHybridSpinAvailable = Assets.Any(Function(x) x.AssetType IsNot Nothing AndAlso x.AssetType.Group.UsedByXRayHybridSpin)
            XRay4X4SpinAvailable = Assets.Any(Function(x) x.AssetType IsNot Nothing AndAlso x.AssetType.Group.UsedByXRay4X4Spin)
            XRaySafetySpinAvailable = Assets.Any(Function(x) x.AssetType IsNot Nothing AndAlso x.AssetType.Group.UsedByXRaySafetySpin)

            If Not WasNew Then Exit Sub 'no need to change dirty bit
            If Assets.Any(Function(asset) asset.AllowEdit) Then
                If Not IsDirty Then MarkDirty()
            Else
                If IsDirty Then MarkOld()
            End If
        End Sub

        Public Function CanLocalize() As Boolean
            Return CanLocalize(False)
        End Function
        Private Function CanLocalize(ByVal raiseException As Boolean) As Boolean
            Dim message As String = String.Empty
            If MyContext.GetContext().IsGlobal Then
                message = "You can not localize an assetset for global"
            ElseIf Not Owner.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                message = "This assetset is already localized!"
            ElseIf Not LocalizedID.Equals(Guid.Empty) Then
                message = "This assetset was previously localized. The changes need to be commited before localizing again!"
            ElseIf Not ParentOwner.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                message = "The item is a local item and hence is localized by nature!"
            End If
            If raiseException AndAlso message.Length > 0 Then Throw New Exceptions.CanNotLocalizeData(message)
            Return (message.Length = 0)
        End Function

        Public Sub Localize()
            If Not CanLocalize(True) Then Return

            _owner = MyContext.GetContext().CountryCode
            AllowEdit = True
            AllowRemove = True
            Assets.Localize()

            ID = Guid.NewGuid()
            LocalizedID = ID
            MarkNew()
            MarkDirty()
        End Sub

        Public Function CanUnlocalize() As Boolean
            Return CanUnlocalize(False)
        End Function
        Private Function CanUnlocalize(ByVal raiseException As Boolean) As Boolean
            Dim message As String = String.Empty
            If MyContext.GetContext().IsGlobal Then
                message = "You can not unlocalize an assetset for global"
            ElseIf Owner.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                message = "This assetset is not localized!"
            ElseIf Not ParentOwner.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                message = "The item is a local item and hence can not be unlocalized!"
            End If
            If raiseException AndAlso message.Length > 0 Then Throw New Exceptions.CanNotUnLocalizeData(message)
            Return (message.Length = 0)
        End Function
        Public Sub Unlocalize()
            If Not CanUnlocalize(True) Then Return

            Try
                Dim globalAssetSet As AssetSet = GetGlobalAssetSet(GenerationID, ObjectID)
                ID = globalAssetSet.ID
                _owner = globalAssetSet.Owner
                NumberOfAssets = globalAssetSet.NumberOfAssets
                NumberOfLocalizedVersions = globalAssetSet.NumberOfLocalizedVersions
                _assets = Nothing
                RefreshProperties()
                AllowEdit = globalAssetSet.AllowEdit
                AllowRemove = globalAssetSet.AllowRemove
                Assets.SetSecurityRights()
                _

            Catch ex As NoDataReturnedException

                ID = Guid.NewGuid()
                _owner = Environment.GlobalCountryCode
                Assets = Nothing
                NumberOfAssets = 0
                NumberOfLocalizedVersions = 0
                AllowEdit = False
                AllowRemove = False
            End Try

            MarkDirty()
        End Sub

        Public Sub Copy(ByVal assetSet As AssetSet)
            If Not Owner.Equals(MyContext.GetContext().CountryCode) Then Localize()

            NumberOfLocalizedVersions = assetSet.NumberOfLocalizedVersions

            'Remove all the assets which are not from Cross Model
            Assets.RemoveRange(Assets.ToList().FindAll(Function(x) Not x.IsCrossModelAsset))

            For Each assetToBeCopied As AssetSetAsset In assetSet.Assets
                If Not assetToBeCopied.IsCrossModelAsset AndAlso Not Assets.Contains(assetToBeCopied.ID) Then
                    'check for an existing cross model asset
                    Dim existingCrossModelAsset = Assets.ToList().Find(Function(x) x.Key = assetToBeCopied.Key)
                    If Not existingCrossModelAsset Is Nothing AndAlso existingCrossModelAsset.IsCrossModelAsset Then
                        'asset is cross model, then update the values with the values from the asset being copied
                        existingCrossModelAsset.Asset = assetToBeCopied.Asset
                    Else
                        Assets.Add(assetToBeCopied.Copy())
                    End If
                End If
            Next
        End Sub

        Public Sub Merge(ByVal assetSet As AssetSet)
            If Not Owner.Equals(MyContext.GetContext().CountryCode) Then Localize()


            For Each asset As AssetSetAsset In assetSet.Assets
                Dim existingAsset = Assets.FirstOrDefault(Function(x) x.Key.Equals(asset.Key, StringComparison.InvariantCultureIgnoreCase))
                If existingAsset Is Nothing Then
                    Assets.Add(asset.Copy())
                Else
                    existingAsset.Asset = asset.Asset
                    existingAsset.AlwaysInclude = asset.AlwaysInclude
                End If
            Next
        End Sub


        Public Overrides Function Save() As AssetSet
            If Not IsChild Then Return MyBase.Save()
            If _ownerObject Is Nothing OrElse Not TypeOf _ownerObject Is IUpdatableAssetSet Then Return MyBase.Save()

            Dim theOwnerObject = _ownerObject

            MarkAsParent()
            Dim savedObject = MyBase.Save()
            savedObject.MarkAsChild()

            _ownerObject = theOwnerObject

            DirectCast(_ownerObject, IUpdatableAssetSet).ChangeReference(savedObject)
            Return savedObject

        End Function

#End Region

#Region " Framework Overrides "
        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If Not (_assets Is Nothing) AndAlso Not _assets.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If Not (_assets Is Nothing) AndAlso _assets.IsDirty Then Return True
                Return False
            End Get
        End Property
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function NewAssetSet(ByVal obj As IHasAssetSet) As AssetSet
            Dim assetSet As AssetSet = New AssetSet()
            assetSet.Create()
            assetSet.GenerationID = obj.GenerationID
            assetSet.ObjectID = obj.GetObjectID()
            assetSet.Entity = obj.Entity
            assetSet._ownerObject = obj
            Return assetSet
        End Function
        Friend Shared Function GetAssetSet(ByVal obj As IHasAssetSet, ByVal dataReader As SafeDataReader) As AssetSet
            Dim assetSet As AssetSet = New AssetSet()
            assetSet.FieldPrefix = "ASSETSET"
            assetSet.Fetch(dataReader)
            assetSet._ownerObject = obj
            Return assetSet
        End Function
        Friend Shared Function GetAssetSet(ByVal obj As IHasAssetSet) As AssetSet
            Try
                Return BusinessObjects.DataPortal.Fetch(Of AssetSet)(New CustomCriteria(obj.GenerationID, obj.GetObjectID(), MyContext.GetContext().CountryCode))
            Catch ex As BusinessObjects.DataPortalException
                If ex.InnerException IsNot Nothing AndAlso TypeOf ex.InnerException Is CallMethodException Then
                    If ex.InnerException.InnerException IsNot Nothing AndAlso TypeOf ex.InnerException.InnerException Is NoDataReturnedException Then
                        Return NewAssetSet(obj)
                    End If
                End If
                Throw
            End Try
        End Function

        Private Shared Function GetGlobalAssetSet(ByVal generationID As Guid, ByVal objectID As Guid) As AssetSet
            Try
                Return BusinessObjects.DataPortal.Fetch(Of AssetSet)(New CustomCriteria(generationID, objectID, Environment.GlobalCountryCode))
            Catch ex As BusinessObjects.DataPortalException
                Throw ex.InnerException.InnerException
            End Try
        End Function
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            Private ReadOnly _generationID As Guid
            Private ReadOnly _objectID As Guid
            Private ReadOnly _country As String

            Public Sub New(ByVal generationID As Guid, ByVal objectID As Guid, ByVal country As String)
                _generationID = generationID
                _objectID = objectID
                _country = country
                CommandText = "getAssetSet"
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@GENERATIONID", _generationID)
                command.Parameters.AddWithValue("@OBJECTID", _objectID)
                command.Parameters.AddWithValue("@COUNTRY", _country)
            End Sub

        End Class
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'AlwaysUpdateSelf = True
            MarkAsChild()
        End Sub
#End Region

#Region " Data Access "

        Protected Overrides Sub InitializeFields()
            _wasNew = True
            _numberOfAssets = 0
            _numberOfLocalizedVersions = 0
            _owner = MyContext.GetContext().CountryCode
            MyBase.InitializeFields()
            AllowEdit = True
            AllowRemove = True
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                _generationID = .GetGuid(GetFieldName("GENERATIONID"))
                _objectID = .GetGuid(GetFieldName("OBJECTID"))
                _entity = .GetEntity(GetFieldName("ENTITY"))
                _owner = .GetString(GetFieldName("OWNER"))
                _numberOfAssets = .GetInt16(GetFieldName("NUMBEROFASSETS"))
                _numberOfLocalizedVersions = .GetInt16(GetFieldName("NUMBEROFLOCALIZEDVERSIONS"))
                _previewAvailable = .GetBoolean(GetFieldName("PREVIEWAVAILABLE"))
                _exteriorSpinAvailable = .GetBoolean(GetFieldName("EXTERIORSPINAVAILABLE"))
                _interiorSpinAvailable = .GetBoolean(GetFieldName("INTERIORSPINAVAILABLE"))
                _xRayHybridSpinAvailable = .GetBoolean(GetFieldName("XRAYHYBRIDSPINAVAILABLE"))
                _xRay4X4SpinAvailable = .GetBoolean(GetFieldName("XRAY4X4SPINAVAILABLE"))
                _xRaySafetySpinAvailable = .GetBoolean(GetFieldName("XRAYSAFETYSPINAVAILABLE"))
            End With
            MyBase.FetchFields(dataReader)
            AllowRemove = MyContext.GetContext().IsGlobal OrElse _owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase)
            AllowEdit = AllowRemove

            LocalizedID = Guid.Empty
            If ID.Equals(Guid.Empty) Then 'The asset set does not exist yet
                ID = Guid.NewGuid()
                WasNew = True
                AlwaysUpdateSelf = True
            Else
                WasNew = False
                If Not Owner.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase) Then LocalizedID = ID
            End If

        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            AddCommandFields(command, "insertAssetSet")
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            If WasNew Then
                'Localize
                AddInsertCommandFields(command)
            ElseIf LocalizedID.Equals(Guid.Empty) OrElse LocalizedID.Equals(ID) Then
                AddCommandFields(command, "updateAssetSet")
            Else
                'Unlocalize
                If LocalizedID.Equals(ID) Then Throw New Exceptions.DevelopperException("This condition should not be reached!")
                command.CommandText = "deleteAssetSet"
                With command.Parameters
                    .Item("@ID").Value = LocalizedID
                End With
            End If

        End Sub

        Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand, ByVal commandText As String)
            command.CommandText = commandText
            With command.Parameters
                .AddWithValue("@GENERATIONID", GenerationID)
                .AddWithValue("@OBJECTID", ObjectID)
                .AddWithValue("@ENTITY", Entity.ToString())
                .AddWithValue("@OWNER", Owner)
                .AddWithValue("@PREVIEWAVAILABLE", _previewAvailable)
                .AddWithValue("@EXTERIORSPINAVAILABLE", _exteriorSpinAvailable)
                .AddWithValue("@INTERIORSPINAVAILABLE", _interiorSpinAvailable)
                .AddWithValue("@XRAYHYBRIDSPINAVAILABLE", _xRayHybridSpinAvailable)
                .AddWithValue("@XRAY4X4SPINAVAILABLE", _xRay4X4SpinAvailable)
                .AddWithValue("@XRAYSAFETYSPINAVAILABLE", _xRaySafetySpinAvailable)
            End With
        End Sub

        Private Sub AssetSetAfterUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.AfterUpdateCommand
            If Owner.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase) Then LocalizedID = Guid.Empty
            AlwaysUpdateSelf = False
            WasNew = False
        End Sub

        Private Sub AssetSetAfterInsertCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.AfterInsertCommand
            WasNew = False
        End Sub

        Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            If _assets IsNot Nothing Then _assets.Update(transaction)
            MyBase.UpdateChildren(transaction)
        End Sub
#End Region



    End Class
End NameSpace