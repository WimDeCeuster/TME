Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class InteriorColours
    Inherits BaseObjects.ContextUniqueGuidListBase(Of InteriorColours, InteriorColour)

#Region " Shared Factory Methods "

    Public Shared Function GetInteriorColours() As InteriorColours
        Return DataPortal.Fetch(Of InteriorColours)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class InteriorColour
    Inherits BaseObjects.TranslateableBusinessBase

#Region " Business Properties & Methods "
    ' Declare variables to contain object state
    ' Declare variables for any child collections

    Private _code As String = String.Empty
    Private _name As String = String.Empty

    ' Implement properties and methods for interaction of the UI,
    ' or any other client code, with the object
    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            value = value.Trim()
            If _code <> value Then
                _code = value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            value = value.Trim()
            If _name <> value Then
                _name = value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property

    Public Function GetInfo() As InteriorColourInfo
        Return InteriorColourInfo.GetInteriorColourInfo(Me)
    End Function
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 2))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return String.Format("{0} - {1}", Me.Code, Me.Name)
    End Function
    Public Overloads Overrides Function Equals(ByVal value As String) As Boolean
        Return String.Compare(Me.Code, value, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetInteriorColour(ByVal id As Guid) As InteriorColour
        Return DataPortal.Fetch(Of InteriorColour)(New Criteria(id))
    End Function
    Friend Shared Function GetInteriorColour(ByVal dataReader As SafeDataReader) As InteriorColour
        Dim _colour As InteriorColour = New InteriorColour()
        _colour.Fetch(dataReader)
        Return _colour
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AutoDiscover = False
        Me.FieldPrefix = "INTERIORCOLOUR"
        Me.MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _code = .GetString(GetFieldName("INTERNALCODE")).Trim()
            _name = .GetString(GetFieldName("SHORTNAME")).Trim()
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@INTERNALCODE", Me.Code)
        command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
    End Sub

#End Region

#Region " Base Object Overrides "
    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.INTERIORCOLOUR
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class InteriorColourInfo

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
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
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

    Public Overloads Function Equals(ByVal obj As InteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As InteriorColourInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Me.Code, obj, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is InteriorColourInfo Then
            Return Me.Equals(DirectCast(obj, InteriorColourInfo))
        ElseIf TypeOf obj Is InteriorColour Then
            Return Me.Equals(DirectCast(obj, InteriorColour))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is InteriorColourInfo Then
            Return DirectCast(objA, InteriorColourInfo).Equals(objB)
        ElseIf TypeOf objB Is InteriorColourInfo Then
            Return DirectCast(objB, InteriorColourInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetInteriorColourInfo(ByVal dataReader As SafeDataReader) As InteriorColourInfo
        Dim info As InteriorColourInfo = New InteriorColourInfo
        info.Fetch(dataReader)
        Return info
    End Function
    Friend Shared Function GetInteriorColourInfo(ByVal interiorColour As InteriorColour) As InteriorColourInfo
        Dim info As InteriorColourInfo = New InteriorColourInfo
        info.Fetch(interiorColour)
        Return info
    End Function
    Public Shared ReadOnly Property Empty() As InteriorColourInfo
        Get
            Return New InteriorColourInfo
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
            _id = .GetGuid("INTERIORCOLOURID")
            _code = .GetString("INTERIORCOLOURINTERNALCODE")
            _name = .GetString("INTERIORCOLOURSHORTNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal interiorColour As InteriorColour)
        With interiorColour
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub

#End Region

End Class
