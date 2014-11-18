Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class TransmissionTypes
    Inherits BaseObjects.ContextUniqueGuidListBase(Of TransmissionTypes, TransmissionType)

#Region " Shared Factory Methods "
    Public Shared Function GetTransmissionTypes() As TransmissionTypes
        Return DataPortal.Fetch(Of TransmissionTypes)(New Criteria)
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
<Serializable()> Public NotInheritable Class TransmissionType
    Inherits BaseObjects.LocalizeableBusinessBase

#Region " Business Properties & Methods "
    Private _code As String
    Private _name As String

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

    Public Function GetInfo() As TransmissionTypeInfo
        Return TransmissionTypeInfo.GetTransmissionTypeInfo(Me)
    End Function

#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Me.Name.ToString()
    End Function
    Public Overloads Overrides Function Equals(ByVal type As String) As Boolean
        Return String.Compare(Me.Code, type, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal type As TransmissionTypeInfo) As Boolean
        Return Me.ID.Equals(type.ID)
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
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
            Return Entity.TRANSMISSIONTYPE
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class TransmissionTypeInfo

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

    Public Overloads Function Equals(ByVal obj As TransmissionType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As TransmissionTypeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is TransmissionType Then
            Return Me.Equals(DirectCast(obj, TransmissionTypeInfo))
        ElseIf TypeOf obj Is FuelType Then
            Return Me.Equals(DirectCast(obj, TransmissionType))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is TransmissionTypeInfo Then
            Return DirectCast(objA, TransmissionTypeInfo).Equals(objB)
        ElseIf TypeOf objB Is TransmissionTypeInfo Then
            Return DirectCast(objB, TransmissionTypeInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetTransmissionTypeInfo(ByVal dataReader As SafeDataReader) As TransmissionTypeInfo
        Dim _transmissionType As TransmissionTypeInfo = New TransmissionTypeInfo
        _transmissionType.Fetch(dataReader)
        Return _transmissionType
    End Function
    Friend Shared Function GetTransmissionTypeInfo(ByVal transmissionType As TransmissionType) As TransmissionTypeInfo
        Dim _transmissionType As TransmissionTypeInfo = New TransmissionTypeInfo
        _transmissionType.Fetch(transmissionType)
        Return _transmissionType
    End Function
    Public Shared ReadOnly Property Empty() As TransmissionTypeInfo
        Get
            Return New TransmissionTypeInfo
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
            _id = .GetGuid("TRANSMISSIONTYPEID")
            _code = .GetString("TRANSMISSIONTYPEINTERNALCODE")
            _name = .GetString("TRANSMISSIONTYPESHORTNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal transmissionType As TransmissionType)
        With transmissionType
            _id = .ID
            _code = .Code
            _name = .Name
        End With
    End Sub
#End Region

End Class