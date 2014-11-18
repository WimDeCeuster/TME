Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class FactoryGrades
    Inherits BaseObjects.ContextUniqueGuidListBase(Of FactoryGrades, FactoryGrade)

#Region " Shared Factory Methods "

    Public Shared Function GetFactoryGrades() As FactoryGrades
        Return DataPortal.Fetch(Of FactoryGrades)(New Criteria())
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MyBase.AllowEdit = MyContext.GetContext().IsGlobal()
        MyBase.AllowNew = MyBase.AllowEdit
        MyBase.AllowRemove = MyBase.AllowEdit
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class FactoryGrade
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of FactoryGrade)

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code <> Value Then
                _code = Value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name <> Value Then
                _name = Value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property

    Public Function GetInfo() As FactoryGradeInfo
        Return FactoryGradeInfo.GetFactoryGradeInfo(Me)
    End Function
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Me.Code, obj, True) = 0
    End Function

#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        Me.FieldPrefix = "FACTORYGRADE"
        MyBase.AllowEdit = MyContext.GetContext().IsGlobal()
        MyBase.AllowNew = MyBase.AllowEdit
        MyBase.AllowRemove = MyBase.AllowEdit
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _code = dataReader.GetString(GetFieldName("INTERNALCODE"))
        _name = dataReader.GetString(GetFieldName("SHORTNAME"))
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

End Class
<Serializable(), XmlInfo("factorygrade")> Public NotInheritable Class FactoryGradeInfo

#Region " Business Properties & Methods "
    Private _id As Guid
    Private _code As String
    Private _name As String

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
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
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As FactoryGrade) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As FactoryGradeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is FactoryGradeInfo Then
            Return Me.Equals(DirectCast(obj, FactoryGradeInfo))
        ElseIf TypeOf obj Is FactoryGrade Then
            Return Me.Equals(DirectCast(obj, FactoryGrade))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is FactoryGradeInfo Then
            Return DirectCast(objA, FactoryGradeInfo).Equals(objB)
        ElseIf TypeOf objB Is FactoryGradeInfo Then
            Return DirectCast(objB, FactoryGradeInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetFactoryGradeInfo(ByVal dataReader As SafeDataReader) As FactoryGradeInfo
        Dim _grade As FactoryGradeInfo = New FactoryGradeInfo
        _grade.Fetch(dataReader)
        Return _grade
    End Function
    Friend Shared Function GetFactoryGradeInfo(ByVal grade As FactoryGrade) As FactoryGradeInfo
        Dim _grade As FactoryGradeInfo = New FactoryGradeInfo
        _grade.Fetch(grade)
        Return _grade
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        'Load object data	into variables
        With DataReader
            _id = .GetGuid("FACTORYGRADEID")
            _code = .GetString("FACTORYGRADECODE")
            _name = .GetString("FACTORYGRADENAME")
        End With
    End Sub
    Private Sub Fetch(ByVal grade As FactoryGrade)
        'Load object data	into variables
        With Grade
            _id = .ID
            _code = .Code
            _name = .Name
        End With
    End Sub

#End Region

End Class