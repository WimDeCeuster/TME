Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class LinkTypes
    Inherits BaseObjects.ContextReadOnlyListBase(Of LinkTypes, LinkType)

#Region " Business Properties & Methods "

    Default Public Overloads ReadOnly Property Item(ByVal type As Int16) As LinkType
        Get
            For Each _item As LinkType In Me
                If _item.Equals(type) Then
                    Return _item
                End If
            Next
            Return Nothing
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Public Shared Function GetLinkTypes(ByVal entity As Entity) As LinkTypes
        Return DataPortal.Fetch(Of LinkTypes)(New CustomCriteria(entity))
    End Function

#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Public Entity As Entity = Entity.NOTHING

        Public Sub New(ByVal entity As Entity)
            Me.Entity = entity
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            If Me.Entity = Entity.NOTHING Then
                command.Parameters.AddWithValue("@ENTITY", System.DBNull.Value)
            Else
                command.Parameters.AddWithValue("@ENTITY", Me.Entity.ToString())
            End If

        End Sub

    End Class
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class LinkType
    Inherits BaseObjects.ContextReadOnlyBase(Of LinkType)

#Region " Business Properties & Methods "

    ' Declare variables to contain object state
    ' Declare variables for any child collections

    Private _id As Int16
    Private _name As String
    Private _previewBaseLinkPattern As String
    Private _liveBaseLinkPattern As String
    Private _objectLinkPattern As String
    Private _carConfiguratorversionID As Int16



    ' Implement read-only properties and methods for interaction of the UI,
    ' or any other client code, with the object

    Public ReadOnly Property ID() As Int16
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    Public ReadOnly Property PreviewBaseLinkPattern() As String
        Get
            Return _previewBaseLinkPattern
        End Get
    End Property
    Public ReadOnly Property LiveBaseLinkPattern() As String
        Get
            Return _liveBaseLinkPattern
        End Get
    End Property
    Public ReadOnly Property ObjectLinkPattern() As String
        Get
            Return _objectLinkPattern
        End Get
    End Property
    Public ReadOnly Property CarConfiguratorversionID() As Int16
        Get
            Return _carConfiguratorversionID
        End Get
    End Property
#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Me.ID
    End Function
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Function Equals(ByVal obj As Int16) As Boolean
        Return Me.ID.Equals(obj)
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetLinkType(ByVal dataReader As SafeDataReader) As LinkType
        Dim _type As LinkType = New LinkType
        _type.Fetch(dataReader)
        Return _type
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.FieldPrefix = "LINKTYPE"
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _id = .GetInt16(GetFieldName("ID"))
            _name = .GetString(GetFieldName("NAME"))
            _previewBaseLinkPattern = .GetString(GetFieldName("PREVIEWBASELINKPATTERN"))
            _liveBaseLinkPattern = .GetString(GetFieldName("LIVEBASELINKPATTERN"))
            _objectLinkPattern = .GetString(GetFieldName("OBJECTLINKPATTERN"))
            _carConfiguratorversionID = .GetInt16(GetFieldName("CARCONFIGURATORVERSIONID"))
        End With

    End Sub

#End Region

End Class