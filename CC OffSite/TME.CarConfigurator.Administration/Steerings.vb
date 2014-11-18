Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class Steerings
    Inherits BaseObjects.ContextUniqueGuidListBase(Of Steerings, Steering)


#Region " Shared Factory Methods "

    Public Shared Function GetSteerings() As Steerings
        If MyContext.GetContext() Is Nothing Then Return DataPortal.Fetch(Of Steerings)(New Criteria)
        Return MyContext.GetContext().Steerings
    End Function
    Friend Shared Function FetchSteerings() As Steerings
        Return DataPortal.Fetch(Of Steerings)(New Criteria)
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
<Serializable()> Public NotInheritable Class Steering
    Inherits BaseObjects.LocalizeableBusinessBase

#Region " Business Properties & Methods "

    ' Declare variables to contain object state
    ' Declare variables for any child collections

    Private _code As String = String.Empty
    Private _name As String = String.Empty

    ' Implement read-only properties and methods for interaction of the UI,
    ' or any other client code, with the object

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

    Public Function GetInfo() As SteeringInfo
        Return SteeringInfo.GetSteeringInfo(Me)
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
        Me.MarkAsChild()
        Me.AllowNew = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        If dataReader.FieldExists(GetFieldName("CODE")) Then
            _code = dataReader.GetString(GetFieldName("CODE"))
            _name = dataReader.GetString(GetFieldName("NAME"))
        Else
            _code = dataReader.GetString(GetFieldName("INTERNALCODE"))
            _name = dataReader.GetString(GetFieldName("SHORTNAME"))
        End If
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
    End Sub

#End Region

#Region " Base Object Overrides"

    Protected Friend Overrides Function GetBaseCode() As String
        Return Me.Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.STEERING
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class SteeringInfo
#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _name As String = String.Empty
    Private _code As String

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    Public ReadOnly Property Code As String 'TODO: implement this coming from the DB
        Get
            _code = If(_code, Steerings.GetSteerings()(ID).Code)
            Return _code
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

    Public Overloads Function Equals(ByVal obj As Steering) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As SteeringInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is SteeringInfo Then
            Return Me.Equals(DirectCast(obj, SteeringInfo))
        ElseIf TypeOf obj Is Steering Then
            Return Me.Equals(DirectCast(obj, Steering))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is SteeringInfo Then
            Return DirectCast(objA, SteeringInfo).Equals(objB)
        ElseIf TypeOf objB Is SteeringInfo Then
            Return DirectCast(objB, SteeringInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetSteeringInfo(ByVal dataReader As SafeDataReader) As SteeringInfo
        Dim _steering As SteeringInfo = New SteeringInfo
        _steering.Fetch(dataReader)
        Return _steering
    End Function
    Friend Shared Function GetSteeringInfo(ByVal steering As Steering) As SteeringInfo
        Dim _steering As SteeringInfo = New SteeringInfo
        _steering.Fetch(steering)
        Return _steering
    End Function
    Public Shared ReadOnly Property Empty() As SteeringInfo
        Get
            Return New SteeringInfo
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
            _id = .GetGuid("STEERINGID")
            _name = .GetString("STEERINGNAME")
            _code = .GetString("STEERINGCODE", Nothing)
        End With
    End Sub
    Private Sub Fetch(ByVal steering As Steering)
        With steering
            _id = .ID
            _name = .Name
            _code = .Code
        End With
    End Sub

#End Region

End Class