<Serializable()> Public NotInheritable Class ProductionPlants
    Inherits BaseObjects.ContextUniqueGuidReadOnlyListBase(Of ProductionPlants, ProductionPlant)

#Region " Shared Factory Methods "

    Public Shared Function GetProductionPlants() As ProductionPlants
        Return DataPortal.Fetch(Of ProductionPlants)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ProductionPlant
  Inherits BaseObjects.ContextUniqueGuidReadOnlyBase(Of ProductionPlant)

#Region " Business Properties & Methods "

  Private _code As String = String.Empty
    Private _a2PCode As String = String.Empty
  Private _productionCode As String = String.Empty
  Private _name As String = String.Empty

  Public ReadOnly Property Code() As String
    Get
      Return _code
    End Get
  End Property
  Public ReadOnly Property A2PCode() As String
    Get
      Return _A2PCode
    End Get
  End Property
  Public ReadOnly Property ProductionCode() As String
    Get
      Return _productionCode
    End Get
  End Property
  Public ReadOnly Property Name() As String
    Get
      Return _name
    End Get
  End Property
#End Region

#Region " Shared Factory Methods "
  Friend Shared Function GetProductionPlant(ByVal dataReader As SafeDataReader) As ProductionPlant
    Dim _productionPlant As ProductionPlant = New ProductionPlant
    _productionPlant.Fetch(dataReader)
    Return _productionPlant
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
    Me.FieldPrefix = "PRODUCTIONPLANT"
  End Sub
#End Region

#Region " Data Access "
  Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
    _code = dataReader.GetString(GetFieldName("INTERNALCODE"))
    _A2PCode = dataReader.GetString(GetFieldName("A2PCODE"))
    _productionCode = dataReader.GetString(GetFieldName("PRODUCTIONCODE"))
    _name = dataReader.GetString(GetFieldName("SHORTNAME"))
  End Sub
#End Region

End Class
