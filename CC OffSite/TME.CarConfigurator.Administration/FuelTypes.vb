Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class FuelTypes
    Inherits BaseObjects.ContextUniqueGuidListBase(Of FuelTypes, FuelType)

#Region " Shared Factory Methods "
    Public Shared Function GetFuelTypes() As FuelTypes
        Return DataPortal.Fetch(Of FuelTypes)(New Criteria)
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AllowNew = False
        Me.AllowRemove = False
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class FuelType
    Inherits BaseObjects.LocalizeableBusinessBase

#Region " Business Properties & Methods "
    ' Declare variables to contain object state
    ' Declare variables for any child collections
    Private _code As String
    Private _name As String

    ' Implement read-only properties and methods for interaction of the UI,
    ' or any other client code, with the object
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    Public Function GetInfo() As FuelTypeInfo
        Return FuelTypeInfo.GetFuelTypeInfo(Me)
    End Function
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Me.Name.ToString()
    End Function
    Public Overloads Overrides Function Equals(ByVal type As String) As Boolean
        Return String.Compare(Me.Code, type, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetFuelType(ByVal dataReader As SafeDataReader) As FuelType
        Dim _type As FuelType = New FuelType
        _type.Fetch(dataReader)
        Return _type
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.FieldPrefix = "FUELTYPE"
        Me.AllowNew = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        _code = dataReader.GetString(GetFieldName("INTERNALCODE"))
        _name = dataReader.GetString(GetFieldName("SHORTNAME"))
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
    End Sub

#End Region

#Region "Base Object Overrides"

    Protected Friend Overrides Function GetBaseCode() As String
        Return Me.Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.FUELTYPE
        End Get
    End Property
#End Region


End Class
<Serializable()> Public NotInheritable Class FuelTypeInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
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
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As FuelType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As FuelTypeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is FuelTypeInfo Then
            Return Me.Equals(DirectCast(obj, FuelTypeInfo))
        ElseIf TypeOf obj Is FuelType Then
            Return Me.Equals(DirectCast(obj, FuelType))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is FuelTypeInfo Then
            Return DirectCast(objA, FuelTypeInfo).Equals(objB)
        ElseIf TypeOf objB Is FuelTypeInfo Then
            Return DirectCast(objB, FuelTypeInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetFuelTypeInfo(ByVal dataReader As SafeDataReader) As FuelTypeInfo
        Dim _fuelType As FuelTypeInfo = New FuelTypeInfo
        _fuelType.Fetch(dataReader)
        Return _fuelType
    End Function
    Friend Shared Function GetFuelTypeInfo(ByVal fuelType As FuelType) As FuelTypeInfo
        Dim _fuelType As FuelTypeInfo = New FuelTypeInfo
        _fuelType.Fetch(fuelType)
        Return _fuelType
    End Function

    Public Shared ReadOnly Property Empty() As FuelTypeInfo
        Get
            Return New FuelTypeInfo
        End Get
    End Property
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("FUELTYPEID")
            _code = .GetString("FUELTYPEINTERNALCODE")
            _name = .GetString("FUELTYPESHORTNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal fuelType As FuelType)
        With fuelType
            _id = .ID
            _code = .Code
            _name = .Name
        End With
    End Sub
#End Region

End Class