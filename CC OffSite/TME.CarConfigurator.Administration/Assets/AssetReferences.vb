Imports TME.CarConfigurator.Administration.Enums

Namespace Assets

    <Serializable()> Public NotInheritable Class AssetReferences
        Inherits ContextReadOnlyListBase(Of AssetReferences, AssetReference)

#Region " Shared Factory Methods "

        Friend Shared Function GetAssetReferences(ByVal assetID As Guid) As AssetReferences
            Return DataPortal.Fetch(Of AssetReferences)(New CustomCriteria(assetID))
        End Function

#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            Private ReadOnly _assetID As Guid

            Public Sub New(ByVal assetID As Guid)
                _assetID = assetID
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@ASSETID", _assetID)
            End Sub
        End Class
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

    End Class
    <Serializable()> Public NotInheritable Class AssetReference
        Inherits ContextReadOnlyBase(Of AssetReference)

#Region " Business Properties & Methods "
        Private _generation As ModelGenerationInfo
        Private _assetType As String
        Private _owner As String
        Private _objectID As Guid
        Private _objectName As String
        Private _objectType As Entity

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Generation() As ModelGenerationInfo
            Get
                Return _generation
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property AssetType() As String
            Get
                Return _assetType
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Owner() As String
            Get
                Return _owner
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ObjectID() As Guid
            Get
                Return _objectID
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property ObjectName() As String
            Get
                Return _objectName
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property ObjectType() As Entity
            Get
                Return _objectType
            End Get
        End Property


#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return String.Format("{0} - {1} ({2}) - {3}", Generation.ToString(), ObjectName, Owner, AssetType)
        End Function
#End Region

#Region " Framework Overrides "
        Protected Overrides Function GetIdValue() As Object
            Return String.Format("{0}-{1}-{2}-{3}", Generation.ID, ObjectID, Owner, AssetType)
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _generation = ModelGenerationInfo.GetModelGenerationInfo(dataReader)
            _assetType = dataReader.GetString("ASSETTYPE")
            _owner = dataReader.GetString("OWNER")
            _objectID = dataReader.GetGuid("OBJECTID")
            _objectName = dataReader.GetString("OBJECTNAME")
            _objectType = DirectCast(Entity.Parse(Entity.NOTHING.GetType(), dataReader.GetString("OBJECTTYPE")), Entity)
        End Sub
#End Region

    End Class
End NameSpace