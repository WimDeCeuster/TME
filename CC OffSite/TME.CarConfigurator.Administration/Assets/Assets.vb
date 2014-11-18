Imports System.IO
Imports System.Text

Namespace Assets

    <Serializable()> Public NotInheritable Class Assets
        Inherits ContextUniqueGuidListBase(Of Assets, Asset)

#Region " Business Properties & Methods "
        Friend ReadOnly Property Group() As AssetGroup
            Get
                Return DirectCast(Parent, AssetGroup)
            End Get
        End Property

        Default Public Overloads ReadOnly Property Item(ByVal assetType As String) As Asset
            Get
                Return FirstOrDefault(Function(x) x.AssetType.Equals(assetType))
            End Get
        End Property
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

        Friend Shared Function NewAssets(ByVal group As AssetGroup) As Assets
            Dim assets As Assets = New Assets
            assets.SetParent(group)
            Return assets
        End Function
        Friend Shared Function GetAssets(ByVal group As AssetGroup) As Assets
            Dim assets As Assets = DataPortal.Fetch(Of Assets)(New CustomCriteria(group))
            assets.SetParent(group)
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
            Private ReadOnly _groupID As Guid

            Public Sub New(ByVal group As AssetGroup)
                _groupID = group.ID
                CommandText = "getAssetGroupAssets"
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@ASSETGROUPID", _groupID)
            End Sub
        End Class
#End Region

#Region " Data Access "
        Protected Overrides Sub Fetch(ByVal dataReader As Common.Database.SafeDataReader)
            Dim types As AssetTypes = AssetTypes.GetAssetTypes()

            RaiseListChangedEvents = RaiseListChangedEventsDuringFetch
            While dataReader.Read()
                Dim asset As Asset = GetObject(dataReader)
                If types.Contains(asset.AssetType) Then
                    asset.AssetType.Group = types(asset.AssetType.Code).Group
                    Add(asset)
                End If
            End While
            RaiseListChangedEvents = True
        End Sub
#End Region

    End Class
    <Serializable()> Public NotInheritable Class Asset
        Inherits ContextUniqueGuidBusinessBase(Of Asset)

#Region " Business Properties & Methods "
        Private _name As String = String.Empty
        Private _fileName As String = String.Empty
        Private _isTransparent As Boolean
        Private _stackingOrder As Integer
        Private _width As Short = 0
        Private _height As Short = 0
        Private _positionX As Short = 0
        Private _positionY As Short = 0
        Private _requiresMatte As Boolean
        Private _assetType As AssetType
        Private _fileType As FileType


        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                If _name <> value Then
                    _name = value
                    PropertyHasChanged("Name")
                End If
            End Set
        End Property

        Public Property AssetType() As AssetType
            Get
                Return _assetType
            End Get
            Set(ByVal value As AssetType)
                If _assetType Is Nothing OrElse Not _assetType.Equals(value) Then
                    _assetType = value
                    PropertyHasChanged("AssetType")
                    ValidationRules.CheckRules("FileName")
                End If
            End Set
        End Property

        <XmlInfo(XmlNodeType.None)> Public Property Group() As AssetGroup
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, Assets).Group
            End Get
            Set(ByVal value As AssetGroup)
                If value Is Nothing Then Throw New ArgumentNullException("The group" & " is mandatory")
                If Group.Equals(value) Then Return

                'Remove from old parent
                Group.Assets.Remove(Me)

                'Add to new parent
                value.Assets.Add(Me)

                'Since removal from the other group marked me for deletion,
                'I'll have to un mark myself for deletion
                MarkUnDeleted()
                PropertyHasChanged("Group")
            End Set
        End Property

        Public ReadOnly Property FileName() As String
            Get
                Return _fileName
            End Get
        End Property

        Public ReadOnly Property FileType() As FileType
            Get
                Return _fileType
            End Get
        End Property

        Public Sub ChangeFile(ByVal newFile As String, ByVal newFileType As FileType)
            _fileName = newFile
            _fileType = newFileType
            _width = 0
            _height = 0
            PropertyHasChanged("FileName")
        End Sub

        Public Property IsTransparent() As Boolean
            Get
                Return _isTransparent
            End Get
            Set(ByVal value As Boolean)
                If _isTransparent <> value Then
                    _isTransparent = value
                    PropertyHasChanged("IsTransparent")
                End If
            End Set
        End Property

        Public Property RequiresMatte() As Boolean
            Get
                Return _requiresMatte
            End Get
            Set(ByVal value As Boolean)
                If _requiresMatte <> value Then
                    _requiresMatte = value
                    PropertyHasChanged("RequiresMatte")
                End If
            End Set
        End Property
        Public Property StackingOrder() As Integer
            Get
                Return _stackingOrder
            End Get
            Set(ByVal value As Integer)
                If _stackingOrder <> value Then
                    _stackingOrder = value
                    PropertyHasChanged("StackingOrder")
                End If
            End Set
        End Property
        Public Property Width() As Short
            Get
                Return _width
            End Get
            Set(ByVal value As Short)
                If _width <> value Then
                    _width = value
                    PropertyHasChanged("Width")
                End If
            End Set
        End Property
        Public Property Height() As Short
            Get
                Return _height
            End Get
            Set(ByVal value As Short)
                If _height <> value Then
                    _height = value
                    PropertyHasChanged("Height")
                End If
            End Set
        End Property
        Public Property PositionX() As Short
            Get
                Return _positionX
            End Get
            Set(ByVal value As Short)
                If _positionX <> value Then
                    _positionX = value
                    PropertyHasChanged("PositionX")
                End If
            End Set
        End Property
        Public Property PositionY() As Short
            Get
                Return _positionY
            End Get
            Set(ByVal value As Short)
                If _positionY <> value Then
                    _positionY = value
                    PropertyHasChanged("PositionY")
                End If
            End Set
        End Property

        Public Function GetInfo() As AssetInfo
            Return AssetInfo.GetAssetInfo(Me)
        End Function
        Public Function GetDetailedInfo() As DetailedAssetInfo
            Return DetailedAssetInfo.GetDetailedAssetInfo(Me)
        End Function



#End Region

#Region " Business & Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "Name")
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "FileName")
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, Validation.RuleHandler), "AssetType")
            ValidationRules.AddRule(DirectCast(AddressOf FileNameValid, Validation.RuleHandler), "FileName")

            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Name", 255))
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("FileName", 255))
        End Sub
        Private Shared Function FileNameValid(ByVal target As Object, ByVal ruleArgs As Validation.RuleArgs) As Boolean
            Dim thisAsset = DirectCast(target, Asset)

            If thisAsset.FileName.Contains("\") Then
                ruleArgs.Description = "The FileName can not contain a ""\""."
                Return False
            End If

            If thisAsset.IsNew AndAlso thisAsset.AssetType IsNot Nothing AndAlso Not thisAsset.FileName.StartsWith(thisAsset.AssetType.Group.AssetFilePath) Then
                ruleArgs.Description = "The filename has to contain the path assigned to the assettype"
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

#Region " Framework Overrides "

        Public Shadows Sub Delete()
            Group.Assets.Remove(Me)
        End Sub

#End Region

#Region " Shared Factory Methods "
        Public Shared Function GetAsset(ByVal id As Guid) As Asset
            Return DataPortal.Fetch(Of Asset)(New Criteria(id))
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            AutoDiscover = False
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                _name = .GetString("NAME")
                _fileName = .GetString("FILENAME")
                _isTransparent = .GetBoolean("ISTRANSPARENT")
                _requiresMatte = .GetBoolean("REQUIRESMATTE")
                _stackingOrder = .GetInt32("STACKINGORDER")
                _width = .GetInt16("WIDTH")
                _height = .GetInt16("HEIGHT")
                _positionX = .GetInt16("POSITIONX")
                _positionY = .GetInt16("POSITIONY")
                _assetType = AssetType.GetAssetType(dataReader)
                _fileType = FileType.GetFileType(dataReader, "FILETYPE")
            End With
        End Sub


        Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.CommandText = "insertAssetGroupAsset"
            AddCommandFields(command)
            command.Parameters.AddWithValue("@CREATEDON", CreatedOn)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.CommandText = "updateAssetGroupAsset"
            AddCommandFields(command)
            command.Parameters.AddWithValue("@MODIFIEDON", ModifiedOn)
        End Sub
        Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.CommandText = "deleteAssetGroupAsset"
        End Sub
        Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            With command
                .Parameters.AddWithValue("@HASH", GetInfo().GetHashCode())
                .Parameters.AddWithValue("@NAME", Name)
                .Parameters.AddWithValue("@FILENAME", FileName)
                .Parameters.AddWithValue("@ISTRANSPARENT", IsTransparent)
                .Parameters.AddWithValue("@REQUIRESMATTE", RequiresMatte)
                .Parameters.AddWithValue("@STACKINGORDER", StackingOrder)
                .Parameters.AddWithValue("@WIDTH", Width)
                .Parameters.AddWithValue("@HEIGHT", Height)
                .Parameters.AddWithValue("@POSITIONX", PositionX)
                .Parameters.AddWithValue("@POSITIONY", PositionY)
                .Parameters.AddWithValue("@ASSETTYPE", Integer.Parse(AssetType.Code))
                .Parameters.AddWithValue("@FILECODE", FileType.Code)
                If Not Group Is Nothing Then .Parameters.AddWithValue("@ASSETGROUPID", Group.ID)
            End With
        End Sub
#End Region

    End Class
End Namespace