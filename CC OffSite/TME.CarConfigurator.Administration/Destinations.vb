<Serializable()> Public NotInheritable Class Destinations
    Inherits BaseObjects.ContextUniqueGuidReadOnlyListBase(Of Destinations, Destination)

#Region " Shared Factory Methods "

    Public Shared Function GetDestinations() As Destinations
        Return DataPortal.Fetch(Of Destinations)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class Destination
  Inherits BaseObjects.ContextUniqueGuidReadOnlyBase(Of Destination)

#Region " Business Properties & Methods "
  Private _code As String = String.Empty
  Private _name As String = String.Empty

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
#End Region

#Region " Shared Factory Methods "
  Friend Shared Function GetDestination(ByVal dataReader As SafeDataReader) As Destination
    Dim _destination As Destination = New Destination
    _destination.Fetch(dataReader)
    Return _destination
  End Function
#End Region

#Region " System.Object Overrides "
  Public Overloads Overrides Function ToString() As String
    Return Me.Name
  End Function
  Public Overloads Overrides Function Equals(ByVal value As String) As Boolean
    Return String.Compare(Me.Code, value, StringComparison.InvariantCultureIgnoreCase) = 0
  End Function
#End Region

#Region " Constructors "

  Private Sub New()
    'Prevent direct creation
    Me.FieldPrefix = "DESTINATION"
  End Sub
#End Region

#Region " Data Access "
  Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
    _code = dataReader.GetString(GetFieldName("INTERNALCODE"))
    _name = dataReader.GetString(GetFieldName("SHORTNAME"))
  End Sub
#End Region

End Class