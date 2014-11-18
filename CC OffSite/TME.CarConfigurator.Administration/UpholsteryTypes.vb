Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class UpholsteryTypes
    Inherits BaseObjects.ContextUniqueGuidListBase(Of UpholsteryTypes, UpholsteryType)

#Region " Shared Factory Methods "
    Public Shared Function GetUpholsteryTypes() As UpholsteryTypes
        Return DataPortal.Fetch(Of UpholsteryTypes)(New Criteria)
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
<Serializable()> Public NotInheritable Class UpholsteryType
    Inherits BaseObjects.TranslateableBusinessBase

#Region " Business Properties & Methods "
    Private _code As String
    Private _name As String
    Private _index As Integer

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer
        Get
            Return _index
        End Get
    End Property

    Public Function GetInfo() As UpholsteryTypeInfo
        Return UpholsteryTypeInfo.GetUpholsteryTypeInfo(Me)
    End Function

#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Me.Name.ToString()
    End Function
    Public Overloads Overrides Function Equals(ByVal type As String) As Boolean
        Return String.Compare(Me.Code, type, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal type As UpholsteryTypeInfo) As Boolean
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
        _code = dataReader.GetString(GetFieldName("CODE"))
        _name = dataReader.GetString(GetFieldName("NAME"))
        _index = Convert.ToInt32(dataReader.GetInt16(GetFieldName("INDEX")))
        MyBase.FetchFields(dataReader)
    End Sub

#End Region

#Region "Base Object Overrides"

    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.UPHOLSTERYTYPE
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class UpholsteryTypeInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _index As Integer = 0

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
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer
        Get
            Return _index
        End Get
    End Property

    Public Function IsEmpty() As Boolean
        Return Me.ID.Equals(Guid.Empty)
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As UpholsteryType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As UpholsteryTypeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Me.Code, obj, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is UpholsteryTypeInfo Then
            Return Me.Equals(DirectCast(obj, UpholsteryTypeInfo))
        ElseIf TypeOf obj Is UpholsteryType Then
            Return Me.Equals(DirectCast(obj, UpholsteryType))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is UpholsteryTypeInfo Then
            Return DirectCast(objA, UpholsteryTypeInfo).Equals(objB)
        ElseIf TypeOf objB Is UpholsteryTypeInfo Then
            Return DirectCast(objB, UpholsteryTypeInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetUpholsteryTypeInfo(ByVal dataReader As SafeDataReader) As UpholsteryTypeInfo
        Dim info As UpholsteryTypeInfo = New UpholsteryTypeInfo
        info.Fetch(dataReader)
        Return info
    End Function
    Friend Shared Function GetUpholsteryTypeInfo(ByVal upholsteryType As UpholsteryType) As UpholsteryTypeInfo
        Dim info As UpholsteryTypeInfo = New UpholsteryTypeInfo
        info.Fetch(upholsteryType)
        Return info
    End Function
    Public Shared ReadOnly Property Empty() As UpholsteryTypeInfo
        Get
            Return New UpholsteryTypeInfo
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
            _id = .GetGuid("UPHOLSTERYTYPEID")
            _code = .GetString("UPHOLSTERYTYPECODE")
            _name = .GetString("UPHOLSTERYTYPENAME")
            _index = Convert.ToInt32(.GetInt16("UPHOLSTERYTYPEINDEX"))
        End With
    End Sub
    Private Sub Fetch(ByVal upholsteryType As UpholsteryType)
        With upholsteryType
            _id = .ID
            _code = .Code
            _name = .Name
            _index = .Index
        End With
    End Sub
#End Region

End Class