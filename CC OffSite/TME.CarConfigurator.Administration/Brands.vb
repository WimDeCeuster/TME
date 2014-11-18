<Serializable()> Public NotInheritable Class Brands
    Inherits BaseObjects.ContextUniqueGuidReadOnlyListBase(Of Brands, Brand)

#Region " Shared Factory Methods "

    Public Shared Function GetBrands() As Brands
        Return DataPortal.Fetch(Of Brands)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class Brand
  Inherits BaseObjects.ContextUniqueGuidReadOnlyBase(Of Brand)

#Region " Business Properties & Methods "

  ' Declare variables to contain object state
  ' Declare variables for any child collections

  Private _name As String

  ' Implement read-only properties and methods for interaction of the UI,
  ' or any other client code, with the object

  Public ReadOnly Property Name() As String
    Get
      Return _name
    End Get
  End Property

#End Region

#Region " System.Object Overrides "

  Public Overloads Overrides Function ToString() As String
    Return Me.Name.ToString()
  End Function

#End Region

#Region " Shared Factory Methods "
  Friend Shared Function GetBrand(ByVal dataReader As SafeDataReader, ByVal fieldPrefix As String) As Brand
    Dim _brand As Brand = New Brand
    _brand.FieldPrefix = fieldPrefix
    _brand.Fetch(dataReader)
    Return _brand
  End Function
#End Region

#Region " Constructors "
  Private Sub New()
    'Prevent direct creation
  End Sub
#End Region

#Region " Data Access "
  Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        _name = dataReader.GetString(GetFieldName("SHORTNAME"), dataReader.GetString(GetFieldName("NAME")))
  End Sub
#End Region

End Class