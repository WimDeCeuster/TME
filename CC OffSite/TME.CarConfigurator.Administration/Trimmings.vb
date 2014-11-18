Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Trimmings
    Inherits BaseObjects.ContextUniqueGuidListBase(Of Trimmings, Trimming)

#Region " Shared Factory Methods "

    Public Shared Function GetTrimmings() As Trimmings
        Return DataPortal.Fetch(Of Trimmings)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class Trimming
    Inherits BaseObjects.TranslateableBusinessBase

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty

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

    Public Function GetInfo() As TrimInfo
        Return TrimInfo.GetTrimInfo(Me)
    End Function
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 2))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")
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

    Friend Shared Function GetTrimming(ByVal id As Guid) As Trimming
        Return DataPortal.Fetch(Of Trimming)(New Criteria(id))
    End Function
    Friend Shared Function GetTrimming(ByVal dataReader As SafeDataReader) As Trimming
        Dim _trimming As Trimming = New Trimming()
        _trimming.Fetch(dataReader)
        Return _trimming
    End Function

#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        Me.AutoDiscover = False
        Me.FieldPrefix = "TRIMMING"
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
            Return Entity.TRIMMING
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class TrimInfo

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

    Public Overloads Function Equals(ByVal obj As Trimming) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As TrimInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Me.Code, obj, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is TrimInfo Then
            Return Me.Equals(DirectCast(obj, TrimInfo))
        ElseIf TypeOf obj Is Trimming Then
            Return Me.Equals(DirectCast(obj, Trimming))
        ElseIf TypeOf obj Is String Then
            Return Me.Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is TrimInfo Then
            Return DirectCast(objA, TrimInfo).Equals(objB)
        ElseIf TypeOf objB Is TrimInfo Then
            Return DirectCast(objB, TrimInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetTrimInfo(ByVal dataReader As SafeDataReader) As TrimInfo
        Dim info As TrimInfo = New TrimInfo
        info.Fetch(dataReader)
        Return info
    End Function
    Friend Shared Function GetTrimInfo(ByVal trim As Trimming) As TrimInfo
        Dim info As TrimInfo = New TrimInfo
        info.Fetch(trim)
        Return info
    End Function
    Public Shared ReadOnly Property Empty() As TrimInfo
        Get
            Return New TrimInfo
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
            _id = .GetGuid("TRIMMINGID")
            _code = .GetString("TRIMMINGINTERNALCODE")
            _name = .GetString("TRIMMINGSHORTNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal trim As Trimming)
        With trim
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub

#End Region

End Class
