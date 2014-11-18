Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ExteriorColourTypes
    Inherits BaseObjects.ContextUniqueGuidListBase(Of ExteriorColourTypes, ExteriorColourType)

#Region " Shared Factory Methods "
    Public Shared Function GetExteriorColourTypes() As ExteriorColourTypes
        Return DataPortal.Fetch(Of ExteriorColourTypes)(New Criteria)
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
<Serializable()> Public NotInheritable Class ExteriorColourType
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

    Public Function GetInfo() As ExteriorColourTypeInfo
        Return ExteriorColourTypeInfo.GetExteriorColourTypeInfo(Me)
    End Function

#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Me.Name.ToString()
    End Function
    Public Overloads Overrides Function Equals(ByVal type As String) As Boolean
        Return String.Compare(Me.Code, type, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal type As ExteriorColourTypeInfo) As Boolean
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
            Return Entity.EXTERIORCOLOURTYPE
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class ExteriorColourTypeInfo

#Region " Business Properties & Methods "
    Private _id As Guid
    Private _code As String
    Private _name As String
    Private _index As Integer

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

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As ExteriorColourType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ExteriorColourTypeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Me.Code, obj, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ExteriorColourTypeInfo Then
            Return Me.Equals(DirectCast(obj, ExteriorColourTypeInfo))
        ElseIf TypeOf obj Is ExteriorColourType Then
            Return Me.Equals(DirectCast(obj, ExteriorColourType))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is ExteriorColourTypeInfo Then
            Return DirectCast(objA, ExteriorColourTypeInfo).Equals(objB)
        ElseIf TypeOf objB Is ExteriorColourTypeInfo Then
            Return DirectCast(objB, ExteriorColourTypeInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetExteriorColourTypeInfo(ByVal dataReader As SafeDataReader) As ExteriorColourTypeInfo
        Dim _info As ExteriorColourTypeInfo = New ExteriorColourTypeInfo
        _info.Fetch(dataReader)
        Return _info
    End Function
    Friend Shared Function GetExteriorColourTypeInfo(ByVal exteriorColourType As ExteriorColourType) As ExteriorColourTypeInfo
        Dim _info As ExteriorColourTypeInfo = New ExteriorColourTypeInfo
        _info.Fetch(exteriorColourType)
        Return _info
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("EXTERIORCOLOURTYPEID")
            _code = .GetString("EXTERIORCOLOURTYPECODE")
            _name = .GetString("EXTERIORCOLOURTYPENAME")
            _index = Convert.ToInt32(.GetInt16("EXTERIORCOLOURTYPEINDEX"))
        End With
    End Sub
    Private Sub Fetch(ByVal exteriorColourType As ExteriorColourType)
        With exteriorColourType
            _id = .ID
            _code = .Code
            _name = .Name
            _index = .Index
        End With
    End Sub
#End Region

End Class