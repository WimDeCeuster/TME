
Namespace Assets
    <Serializable()> Public NotInheritable Class LinkedAssets
        Inherits ContextUniqueGuidListBase(Of LinkedAssets, LinkedAsset)

#Region " Business Properties & Methods "

        <NotUndoable()> Private _objectID As Guid
        <NotUndoable()> Private _baseObjectID As Guid

        Friend ReadOnly Property ObjectID() As Guid
            Get
                Return _objectID
            End Get
        End Property

        Private ReadOnly Property BaseObjectID() As Guid
            Get
                Return _baseObjectID
            End Get
        End Property


        Default Public Shadows ReadOnly Property Item(ByVal assetType As AssetType) As LinkedAsset
            Get
                Return FirstOrDefault(Function(x) x.AssetType.Equals(assetType))
            End Get
        End Property
        Default Public Shadows ReadOnly Property Item(ByVal assetType As String) As LinkedAsset
            Get
                Return (From asset In Me Where asset.AssetType.Equals(assetType)).FirstOrDefault()
            End Get
        End Property

        Public Shadows Sub Add(ByVal assetInfo As DetailedAssetInfo)
            If Contains(assetInfo.AssetType) Then Throw New Exceptions.AssetTypeAlreadyExistsException(assetInfo.AssetType)
            MyBase.Add(LinkedAsset.NewLinkedAsset(ObjectID, BaseObjectID, assetInfo))
        End Sub
        Public Shadows Sub Add(ByVal assetInfo As DetailedAssetInfo, ByVal overRuleAssetType As AssetType)
            If Contains(overRuleAssetType) Then Throw New Exceptions.AssetTypeAlreadyExistsException(overRuleAssetType)
            MyBase.Add(LinkedAsset.NewLinkedAsset(ObjectID, BaseObjectID, assetInfo, overRuleAssetType))
        End Sub

        Friend Overloads Sub Restore(ByVal asset As LinkedAsset)
            MyBase.Restore(asset)
        End Sub

        Public Overloads Sub Remove(ByVal asset As LinkedAsset)
            MyBase.Remove(asset)
        End Sub
        Public Overloads Sub Remove(ByVal assetType As AssetType)
            Remove(Item(assetType))
        End Sub

#End Region

#Region " Contains Methods "

        Public Overloads Function Contains(ByVal assetType As AssetType) As Boolean
            Return Any(Function(x) x.AssetType.Equals(assetType))
        End Function
        Public Overloads Function Contains(ByVal assetType As String) As Boolean
            Return Any(Function(x) x.AssetType.Equals(assetType))
        End Function
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function NewLinkedAssets(ByVal objectID As Guid) As LinkedAssets
            Return NewLinkedAssets(objectID, objectID)
        End Function
        Friend Shared Function NewLinkedAssets(ByVal objectID As Guid, ByVal baseObjectID As Guid) As LinkedAssets
            Dim assets As LinkedAssets = New LinkedAssets
            assets._objectID = objectID
            assets._baseObjectID = baseObjectID
            Return assets
        End Function

        Friend Shared Function GetLinkedAssets(ByVal objectID As Guid, ByVal baseObjectID As Guid, ByVal dataReader As SafeDataReader) As LinkedAssets
            Dim assets As LinkedAssets = NewLinkedAssets(objectID, baseObjectID)
            assets.Fetch(dataReader)
            Return assets
        End Function

        Private Shared Function GetLinkedAssets(ByVal criteria As CustomCriteria) As LinkedAssets
            Dim assets As LinkedAssets = DataPortal.Fetch(Of LinkedAssets)(criteria)
            assets._objectID = criteria.ObjectID
            assets._baseObjectID = criteria.BaseObjectID
            Return assets
        End Function
        Friend Shared Function GetLinkedAssets(ByVal objectID As Guid) As LinkedAssets
            Return GetLinkedAssets(objectID, objectID)
        End Function
        Friend Shared Function GetLinkedAssets(ByVal objectID As Guid, ByVal baseObjectID As Guid) As LinkedAssets
            Return GetLinkedAssets(New CustomCriteria(objectID, baseObjectID))
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
            'Tell the framework we'are a child collection
            MarkAsChild()
        End Sub
#End Region

#Region " Criteria "
        <Serializable()> Private NotInheritable Class CustomCriteria
            Inherits CommandCriteria

            Public ReadOnly ObjectID As Guid
            Public ReadOnly BaseObjectID As Guid

            'Add Data Portal criteria here
            Public Sub New(ByVal objectID As Guid, ByVal baseObjectID As Guid)
                Me.ObjectID = objectID
                Me.BaseObjectID = baseObjectID
                CommandText = "getLinkedAssets"
            End Sub

            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@OBJECTID", ObjectID)
                command.Parameters.AddWithValue("@BASEOBJECTID", BaseObjectID)
            End Sub

        End Class
#End Region

#Region " Data Access "

        Private Shadows Sub Fetch(ByVal dataReader As SafeDataReader)
            MyBase.Fetch(dataReader)
        End Sub

        Friend Overloads Sub Update(ByVal transaction As SqlTransaction)
            RaiseListChangedEvents = False

            For i As Integer = (DeletedList.Count - 1) To 0 Step -1
                If DeletedList.Item(i).AllowRemove Then
                    DeletedList.Item(i).Delete(transaction)
                End If
            Next
            DeletedList.Clear()

            'Loop through each non-deleted object and call its Update Method
            For Each asset As LinkedAsset In Where(Function(x) x.AllowEdit)
                asset.Update(transaction)
            Next

            RaiseListChangedEvents = True
        End Sub

#End Region

    End Class
    <Serializable(), XmlInfo("asset")> Public NotInheritable Class LinkedAsset
        Inherits ContextUniqueGuidBusinessBase(Of LinkedAsset)

#Region " Business Properties & Methods "
        Private _shortID As Nullable(Of Integer) = Nothing
        Private _objectID As Guid
        Private _baseObjectID As Guid

        Private _name As String = String.Empty
        Private _fileName As String = String.Empty
        Private _isTransparent As Boolean = False
        Private _requiresMatte As Boolean = False
        Private _stackingOrder As Integer = 0
        Private _assetType As AssetType
        Private _fileType As FileType
        Private _owner As String


        Public ReadOnly Property ShortID() As Nullable(Of Integer)
            Get
                Return _shortID
            End Get
        End Property

        Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property

        Public ReadOnly Property FileName() As String
            Get
                Return _fileName
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property IsTransparent() As Boolean
            Get
                Return _isTransparent
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property RequiresMatte() As Boolean
            Get
                Return _requiresMatte
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property StackingOrder() As Integer
            Get
                Return _stackingOrder
            End Get
        End Property
        Public ReadOnly Property FileType() As FileType
            Get
                Return _fileType
            End Get
        End Property

        Public ReadOnly Property AssetType() As AssetType
            Get
                Return _assetType
            End Get
        End Property

        Public WriteOnly Property SetAsset() As DetailedAssetInfo
            Set(ByVal value As DetailedAssetInfo)
                With value
                    If Not ID.Equals(.ID) Then
                        ID = .ID
                        _name = .Name
                        _fileName = .FileName
                        _isTransparent = .IsTransparent
                        _requiresMatte = .RequiresMatte
                        _stackingOrder = .StackingOrder
                        _fileType = .FileType
                        _owner = MyContext.GetContext().CountryCode

                        MarkDirty()
                    End If
                End With
            End Set
        End Property

        <XmlInfo(XmlNodeType.None)> Public Overloads Overrides Property AllowEdit() As Boolean
            Get
                If _objectID.Equals(DirectCast(Parent, LinkedAssets).ObjectID) Then
                    Return MyBase.AllowEdit
                Else
                    Return False
                End If
            End Get
            Protected Set(ByVal value As Boolean)
                MyBase.AllowEdit = value
            End Set
        End Property
        Public Overloads Overrides Property AllowRemove() As Boolean
            Get
                If _objectID.Equals(DirectCast(Parent, LinkedAssets).ObjectID) AndAlso String.Compare(_owner, MyContext.GetContext().CountryCode, True) = 0 Then
                    Return MyBase.AllowRemove
                Else
                    Return False
                End If
            End Get
            Protected Set(ByVal value As Boolean)
                MyBase.AllowRemove = value
            End Set
        End Property

        Public Sub TakeOwnership()
            _objectID = DirectCast(Parent, LinkedAssets).ObjectID
            _owner = MyContext.GetContext().CountryCode
            MarkNew()
        End Sub

#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Name
        End Function

#End Region

#Region " Shared Factory Methods "

        Friend Shared Function NewLinkedAsset(ByVal objectID As Guid, ByVal baseObjectID As Guid, ByVal assetInfo As DetailedAssetInfo) As LinkedAsset
            Dim asset As LinkedAsset = New LinkedAsset
            asset._objectID = objectID
            asset._baseObjectID = baseObjectID
            asset._owner = MyContext.GetContext().CountryCode
            asset.Fetch(assetInfo)
            Return asset
        End Function
        Friend Shared Function NewLinkedAsset(ByVal objectID As Guid, ByVal baseObjectID As Guid, ByVal assetInfo As DetailedAssetInfo, ByVal overRuleAssetType As AssetType) As LinkedAsset
            Dim asset As LinkedAsset = New LinkedAsset
            asset._objectID = objectID
            asset._baseObjectID = baseObjectID
            asset._owner = MyContext.GetContext().CountryCode
            asset.Fetch(assetInfo, overRuleAssetType)
            Return asset
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
            'Tell the framework that we are a child object
            FieldPrefix = "ASSET"
            MarkAsChild()
        End Sub
#End Region

#Region " Data Access "

        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            MyBase.FetchFields(dataReader)
            With dataReader
                If .FieldExists("ASSETSHORTID") Then _shortID = dataReader.GetInt32("ASSETSHORTID")
                _objectID = .GetGuid("OBJECTID")
                _baseObjectID = .GetGuid("BASEOBJECTID")
                _name = .GetString("ASSETNAME")
                _fileName = .GetString("ASSETFILENAME")
                _isTransparent = .GetBoolean("ASSETISTRANSPARENT")
                _stackingOrder = .GetInt32("ASSETSTACKINGORDER")
                _assetType = AssetType.GetAssetType(dataReader)
                _fileType = FileType.GetFileType(dataReader, "FILETYPE")
                _requiresMatte = .GetBoolean("ASSETREQUIRESMATTE", False)
                _owner = .GetString("OWNER")
            End With
        End Sub
        Protected Overrides Sub FetchAuditFields(ByVal dataReader As Common.Database.SafeDataReader)
            CreatedBy = dataReader.GetString("CREATEDBY", String.Empty)
            CreatedOn = dataReader.GetDateTime("CREATEDON", DateTime.Now)
            ModifiedBy = dataReader.GetString("MODIFIEDBY", CreatedBy)
            ModifiedOn = dataReader.GetDateTime("MODIFIEDON", CreatedOn)
        End Sub
        Private Overloads Sub Fetch(ByVal assetInfo As DetailedAssetInfo)
            With assetInfo
                ID = .ID
                _name = .Name
                _fileName = .FileName
                _isTransparent = .IsTransparent
                _requiresMatte = .RequiresMatte
                _stackingOrder = .StackingOrder
                _assetType = .AssetType
                _fileType = .FileType
                CreatedBy = .CreatedBy
                CreatedOn = .CreatedOn
                ModifiedBy = .ModifiedBy
                ModifiedOn = .ModifiedOn
            End With
        End Sub
        Private Overloads Sub Fetch(ByVal assetInfo As DetailedAssetInfo, ByVal overRuleAssetType As AssetType)
            Fetch(assetInfo)
            _assetType = overRuleAssetType
        End Sub

        Protected Friend Overloads Sub Delete(ByVal transaction As SqlTransaction)
            MyBase.Delete(transaction)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            AddBaseCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            AddBaseCommandFields(command)
        End Sub
        Private Sub AddBaseCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@OBJECTID", _objectID)
            command.Parameters.AddWithValue("@BASEOBJECTID", _baseObjectID)
            command.Parameters.AddWithValue("@ASSETTYPE", AssetType.Code)
            command.Parameters.AddWithValue("@ASSETID", ID)
        End Sub

        Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@OBJECTID", _objectID)
            command.Parameters.AddWithValue("@ASSETTYPE", AssetType.Code)
        End Sub
        Protected Overrides Sub DeleteCommand(ByVal command As System.Data.SqlClient.SqlCommand)
            command.CommandType = CommandType.StoredProcedure
            command.CommandText = String.Format("deleteLinkedAsset")
            SqlDatabaseContext.AddLeadingParameters(command)
            AddDeleteCommandFields(command)
            AddAuditFields(command)
            SqlDatabaseContext.AddTrailingParameters(command)

            Dim dataReader As SafeDataReader = New SafeDataReader(command.ExecuteReader)
            Try
                If dataReader.Read AndAlso Not Parent Is Nothing Then
                    Fetch(dataReader)
                    DirectCast(Parent, LinkedAssets).Restore(Me)
                Else
                    MarkNew()
                End If
            Finally
                dataReader.Close()
            End Try

        End Sub


        Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
            'nothing
        End Sub
        Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
            'nothing
        End Sub
        Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
            'nothing
        End Sub

#End Region

    End Class
End NameSpace