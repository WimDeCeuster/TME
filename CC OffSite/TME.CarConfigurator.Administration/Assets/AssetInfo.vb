Imports System.Text
Imports TME.BusinessObjects.Templates

Namespace Assets

    <Serializable()> Public NotInheritable Class AssetInfo

#Region " Business Properties & Methods "
        Private _shortID As Nullable(Of Integer)
        Private _id As Guid
        Private _name As String
        Private _fileName As String
        Private _stackingOrder As Integer
        Private _width As Short
        Private _height As Short
        Private _positionX As Short
        Private _positionY As Short
        Private _isTransparent As Boolean
        Private _requiresMatte As Boolean
        Private _fileType As FileType
        Private _references As AssetReferences

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ShortID() As Nullable(Of Integer)
            Get
                Return _shortID
            End Get
        End Property

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
            Get
                Return _id
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
            Get
                Return _name
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property FileName() As String
            Get
                Return _fileName
            End Get
        End Property
        Public ReadOnly Property FileExtension() As String
            Get
                Return FileName.Substring(FileName.LastIndexOf(".", StringComparison.Ordinal) + 1)
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property StackingOrder() As Integer
            Get
                Return _stackingOrder
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
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Width() As Short
            Get
                Return _width
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Height() As Short
            Get
                Return _height
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property PositionX() As Short
            Get
                Return _positionX
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property PositionY() As Short
            Get
                Return _positionY
            End Get
        End Property
        Public ReadOnly Property FileType() As FileType
            Get
                Return _fileType
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Hash() As String
            Get
                Return GetHash()
            End Get
        End Property
        Public ReadOnly Property References() As AssetReferences
            Get
                If _references Is Nothing Then _references = AssetReferences.GetAssetReferences(ID)
                Return _references
            End Get
        End Property
        Private Function GetHash() As String
            Dim key = String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}", FileName, IsTransparent, RequiresMatte, StackingOrder, PositionX, PositionY, Width, Height)
            Dim tempHash = New System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(key))
            Return tempHash.Aggregate(String.Empty, Function(current, [byte]) current + [byte].ToString("x2").ToLower())
        End Function

#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Name
        End Function
        Public Overloads Overrides Function GetHashCode() As Integer
            Return ID.GetHashCode()
        End Function

        Public Overloads Function Equals(ByVal obj As Asset) As Boolean
            Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
        End Function
        Public Overloads Function Equals(ByVal obj As LinkedAsset) As Boolean
            Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
        End Function
        Public Overloads Function Equals(ByVal obj As DetailedAssetInfo) As Boolean
            Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
        End Function
        Public Overloads Function Equals(ByVal obj As AssetInfo) As Boolean
            Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
        End Function
        Public Overloads Function Equals(ByVal obj As String) As Boolean
            Return String.Compare(Name, obj, StringComparison.InvariantCultureIgnoreCase) = 0
        End Function
        Public Overloads Function Equals(ByVal obj As Guid) As Boolean
            Return ID.Equals(obj)
        End Function
        Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
            If TypeOf obj Is Asset Then
                Return Equals(DirectCast(obj, Asset))
            ElseIf TypeOf obj Is LinkedAsset Then
                Return Equals(DirectCast(obj, LinkedAsset))
            ElseIf TypeOf obj Is DetailedAssetInfo Then
                Return Equals(DirectCast(obj, DetailedAssetInfo))
            ElseIf TypeOf obj Is AssetInfo Then
                Return Equals(DirectCast(obj, AssetInfo))
            ElseIf TypeOf obj Is String Then
                Return Equals(DirectCast(obj, String))
            ElseIf TypeOf obj Is Guid Then
                Return Equals(DirectCast(obj, Guid))
            Else
                Return False
            End If
        End Function
        Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
            If TypeOf objA Is AssetInfo Then
                Return DirectCast(objA, AssetInfo).Equals(objB)
            ElseIf TypeOf objB Is AssetInfo Then
                Return DirectCast(objB, AssetInfo).Equals(objA)
            Else
                Return False
            End If
        End Function

#End Region

#Region " Shared Factory Methods "
        Friend Shared Function GetAssetInfo(ByVal dataReader As SafeDataReader) As AssetInfo
            Dim info As AssetInfo = New AssetInfo
            info.Fetch(dataReader)
            Return info
        End Function
        Friend Shared Function GetAssetInfo(ByVal asset As Asset) As AssetInfo
            Dim info As AssetInfo = New AssetInfo
            info.Fetch(asset)
            Return info
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Private Sub Fetch(ByVal dataReader As SafeDataReader)
            With dataReader
                _shortID = .GetInt32("ASSETSHORTID", Nothing)
                _id = .GetGuid("ASSETID")
                _name = .GetString("ASSETNAME")
                _fileName = .GetString("ASSETFILENAME")
                _isTransparent = .GetBoolean("ASSETISTRANSPARENT")
                _stackingOrder = .GetInt32("ASSETSTACKINGORDER")
                _width = .GetInt16("ASSETWIDTH")
                _height = .GetInt16("ASSETHEIGHT")
                _positionX = .GetInt16("ASSETPOSITIONX")
                _positionY = .GetInt16("ASSETPOSITIONY")
                _requiresMatte = .GetBoolean("ASSETREQUIRESMATTE")
                _fileType = FileType.GetFileType(dataReader, "FILETYPE")
            End With
        End Sub
        Private Sub Fetch(ByVal asset As Asset)
            With asset
                _id = .ID
                _name = .Name
                _fileName = .FileName
                _isTransparent = .IsTransparent
                _stackingOrder = .StackingOrder
                _width = .Width
                _height = .Height
                _positionX = .PositionX
                _positionY = .PositionY
                _requiresMatte = .RequiresMatte
                _fileType = asset.FileType
            End With
        End Sub
#End Region

    End Class
    <Serializable(), CommandClassName("Asset")> Public NotInheritable Class DetailedAssetInfo
        Inherits ContextUniqueGuidReadOnlyBase(Of DetailedAssetInfo)

#Region " Business Properties & Methods "

        Private _name As String = String.Empty
        Private _fileName As String = String.Empty
        Private _isTransparent As Boolean
        Private _requiresMatte As Boolean = False
        Private _stackingOrder As Integer
        Private _width As Short = 0
        Private _height As Short = 0
        Private _positionX As Short = 0
        Private _positionY As Short = 0
        Private _assetType As AssetType
        Private _fileType As FileType
        Private _createdBy As String
        Private _createdOn As DateTime = DateTime.Now
        Private _modifiedBy As String
        Private _modifiedOn As DateTime = DateTime.Now
        Private _references As AssetReferences

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
        Public ReadOnly Property IsTransparent() As Boolean
            Get
                Return _isTransparent
            End Get
        End Property
        Public ReadOnly Property RequiresMatte() As Boolean
            Get
                Return _requiresMatte
            End Get
        End Property
        Public ReadOnly Property StackingOrder() As Integer
            Get
                Return _stackingOrder
            End Get
        End Property
        Public ReadOnly Property Width() As Short
            Get
                Return _width
            End Get
        End Property
        Public ReadOnly Property Height() As Short
            Get
                Return _height
            End Get
        End Property
        Public ReadOnly Property PositionX() As Short
            Get
                Return _positionX
            End Get
        End Property
        Public ReadOnly Property PositionY() As Short
            Get
                Return _positionY
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
        Public ReadOnly Property References() As AssetReferences
            Get
                If _references Is Nothing Then _references = AssetReferences.GetAssetReferences(ID)
                Return _references
            End Get
        End Property
        Public ReadOnly Property CreatedBy() As String
            Get
                Return _createdBy
            End Get
        End Property
        Public ReadOnly Property CreatedOn() As DateTime
            Get
                Return _createdOn
            End Get
        End Property

        Public ReadOnly Property ModifiedBy() As String
            Get
                Return _modifiedBy
            End Get
        End Property
        Public ReadOnly Property ModifiedOn() As DateTime
            Get
                Return _modifiedOn
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Hash() As String
            Get
                Return GetHash()
            End Get
        End Property
        Private Function GetHash() As String
            Dim key = String.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}-{7}", FileName, IsTransparent, RequiresMatte, StackingOrder, PositionX, PositionY, Width, Height)
            Dim tempHash = New System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(key))
            Return tempHash.Aggregate(String.Empty, Function(current, [byte]) current + [byte].ToString("x2").ToLower())
        End Function

#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Name
        End Function

#End Region

#Region " Shared Factory Methods "
        Public Shared Function GetDetailedAssetInfo(ByVal id As Guid) As DetailedAssetInfo
            Return DataPortal.Fetch(Of DetailedAssetInfo)(New Criteria(id))
        End Function

        Public Shared Function GetDetailedAssetInfo(ByVal asset As Asset) As DetailedAssetInfo
            Dim info As DetailedAssetInfo = New DetailedAssetInfo
            info.Fetch(asset)
            Return info
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                _name = .GetString("NAME")
                _fileName = .GetString("FILENAME")
                _isTransparent = .GetBoolean("ISTRANSPARENT")
                _stackingOrder = .GetInt32("STACKINGORDER")
                _width = .GetInt16("WIDTH")
                _height = .GetInt16("HEIGHT")
                _positionX = .GetInt16("POSITIONX")
                _positionY = .GetInt16("POSITIONY")
                _assetType = AssetType.GetAssetType(dataReader)
                _fileType = FileType.GetFileType(dataReader, "FILETYPE")
                _isTransparent = .GetBoolean("ISTRANSPARENT")

                If .FieldExists("ASSETREQUIRESMATTE") Then _requiresMatte = .GetBoolean("ASSETREQUIRESMATTE")
                If .FieldExists("CREATEDON") Then _createdOn = .GetDateTime("CREATEDON")
                If .FieldExists("CREATEDBY") Then _createdBy = .GetString("CREATEDBY")
                If .FieldExists("MODIFIEDON") Then _modifiedOn = .GetDateTime("MODIFIEDON")
                If .FieldExists("MODIFIEDBY") Then _modifiedBy = .GetString("MODIFIEDBY")
            End With
        End Sub
        Private Overloads Sub Fetch(ByVal asset As Asset)
            With asset
                ID = .ID
                _name = .Name
                _fileName = .FileName
                _isTransparent = .IsTransparent
                _requiresMatte = .RequiresMatte
                _stackingOrder = .StackingOrder
                _width = .Width
                _height = .Height
                _positionX = .PositionX
                _positionY = .PositionY
                _assetType = .AssetType
                _fileType = .FileType
                _createdOn = .CreatedOn
                _createdBy = .CreatedBy
                _modifiedOn = .ModifiedOn
                _modifiedBy = .ModifiedBy
            End With
        End Sub
#End Region

    End Class
End NameSpace