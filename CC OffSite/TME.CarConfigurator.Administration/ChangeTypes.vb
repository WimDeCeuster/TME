<Serializable()> Public NotInheritable Class ChangeTypes
    Inherits BaseObjects.ContextUniqueGuidReadOnlyListBase(Of ChangeTypes, ChangeType)

#Region " Shared Factory Methods "
    Public Shared Function GetChangeTypes() As ChangeTypes
        Return DataPortal.Fetch(Of ChangeTypes)(New Criteria())
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ChangeType
  Inherits BaseObjects.ContextUniqueGuidReadOnlyBase(Of ChangeType)

#Region " Business Properties & Methods "
  Private _code As String
  Private _name As String

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

#Region " System.Object Overrides "

  Public Overloads Overrides Function ToString() As String
    Return Me.Name
  End Function
  Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
    Return (String.Compare(Me.Code, obj, True) = 0)
  End Function

#End Region

#Region " Shared Factory Methods "
  Friend Shared Function GetChangeType(ByVal dataReader As SafeDataReader) As ChangeType
    Dim _type As ChangeType = New ChangeType
    _type.Fetch(dataReader)
    Return _type
  End Function
#End Region

#Region " Constructors "
  Private Sub New()
    'Prevent direct creation
    Me.FieldPrefix = "CHANGETYPE"
  End Sub
#End Region

#Region " Data Access "
  Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
    _code = dataReader.GetString(GetFieldName("INTERNALCODE"))
    _name = dataReader.GetString(GetFieldName("SHORTNAME"))
  End Sub
#End Region

End Class