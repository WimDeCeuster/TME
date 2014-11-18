<Serializable()> Public NotInheritable Class CarConfiguratorVersions
    Inherits BaseObjects.ContextReadOnlyListBase(Of CarConfiguratorVersions, CarConfiguratorVersion)

#Region " Business Properties & Methods "

    Default Public Overloads ReadOnly Property Item(ByVal id As Int16) As CarConfiguratorVersion
        Get
            For Each _item As CarConfiguratorVersion In Me
                If _item.Equals(id) Then
                    Return _item
                End If
            Next
            Return Nothing
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Public Shared Function GetCarConfiguratorVersions() As CarConfiguratorVersions
        Return DataPortal.Fetch(Of CarConfiguratorVersions)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class CarConfiguratorVersion
  Inherits BaseObjects.ContextReadOnlyBase(Of CarConfiguratorVersion)

#Region " Business Properties & Methods "
  Private _id As Int16
  Private _name As String

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

  Friend Shared Function GetCarConfiguratorVersion(ByVal dataReader As SafeDataReader) As CarConfiguratorVersion
    Dim _version As CarConfiguratorVersion = New CarConfiguratorVersion
    _version.Fetch(dataReader)
    Return _version
  End Function

#End Region

#Region " Constructors "
  Private Sub New()
    'Prevent direct creation
    Me.FieldPrefix = "CARCONFIGURATORVERSION"
  End Sub
#End Region

#Region " Data Access "
  Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
    With dataReader
      _id = .GetInt16(GetFieldName("ID"))
      _name = .GetString(GetFieldName("NAME"))
    End With
  End Sub

#End Region

End Class