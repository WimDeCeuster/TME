Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class BodyTypes
    Inherits BaseObjects.ContextUniqueGuidListBase(Of BodyTypes, BodyType)

#Region " Shared Factory Methods "

    Public Shared Function GetBodyTypes() As BodyTypes
        Return DataPortal.Fetch(Of BodyTypes)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class BodyType
    Inherits BaseObjects.LocalizeableBusinessBase

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _a2PCode As String = String.Empty
    Private _name As String = String.Empty
    Private _owner As String = String.Empty
    Private _numberOfDoors As Integer = 0
    Private _visible As Boolean = True

    <XmlInfo(XmlNodeType.Attribute)> Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code <> Value Then
                _code = Value
                If Me.AllowEdit Then _localCode = Value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property A2PCode() As String
        Get
            Return _A2PCode
        End Get
        Set(ByVal value As String)
            If _A2PCode <> Value Then
                _A2PCode = Value
                PropertyHasChanged("A2PCode")
            End If
        End Set
    End Property
    Private Sub AnyPropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If e.PropertyName.Equals("LocalCode", StringComparison.InvariantCultureIgnoreCase) Then
            If Me.Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                _code = Me.LocalCode
            End If
        End If
    End Sub
    <XmlInfo(XmlNodeType.Attribute)> Public Property Name() As String
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
    <XmlInfo(XmlNodeType.Attribute)> Public Property Owner() As String
        Get
            Return _owner
        End Get
        Set(ByVal value As String)
            Value = Value.ToUpperInvariant()
            If _owner <> Value Then
                _owner = Value
                PropertyHasChanged("Owner")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property NumberOfDoors() As Integer
        Get
            Return _numberOfDoors
        End Get
        Set(ByVal value As Integer)
            If _numberOfDoors <> Value Then
                _numberOfDoors = Value
                PropertyHasChanged("NumberOfDoors")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Visible() As Boolean
        Get
            Return _visible
        End Get
    End Property
    Public Function GetInfo() As BodyTypeInfo
        Return BodyTypeInfo.GetBodyTypeInfo(Me)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Int32.GreaterThan, Validation.RuleHandler), New Rules.Int32.GreaterThanRuleArgs("NumberOfDoors", "number of doors", 0))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("A2PCode", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "A2PCode")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")
    End Sub

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
        Me.AutoDiscover = False 'There is nothing to discover anyway
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _owner = MyContext.GetContext().CountryCode
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _A2PCode = .GetString("A2PCODE")
            _name = .GetString("SHORTNAME")
            _numberOfDoors = .GetInt16("NUMBEROFDOORS")
            _owner = .GetString("OWNER")
            _visible = .GetBoolean("VISIBLE", True)
        End With
        MyBase.FetchFields(dataReader)
        MyBase.AllowEdit = Me.Owner.Equals(MyContext.GetContext().CountryCode) OrElse MyContext.GetContext().Country.Equals(Environment.GlobalCountryCode)
        MyBase.AllowRemove = MyBase.AllowEdit

        If Me.AllowEdit And Me.SupportsLocalCode Then
            _localCode = _code
        End If
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@INTERNALCODE", Me.Code)
        command.Parameters.AddWithValue("@A2PCODE", Me.A2PCode)
        command.Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
        command.Parameters.AddWithValue("@NUMBEROFDOORS", Me.NumberOfDoors)
        command.Parameters.AddWithValue("@OWNER", Me.Owner)
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
            Return Entity.BODY
        End Get
    End Property
#End Region

End Class
<Serializable(), XmlInfo("bodytype")> Public NotInheritable Class BodyTypeInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _name As String = String.Empty
    Private _numberOfDoors As Integer = 0

    <XmlInfo(XmlNodeType.Attribute)> Public Property ID() As Guid
        Get
            Return _id
        End Get
        Private Set(ByVal value As Guid)
            _id = value
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Private Set(ByVal value As String)
            _name = value
        End Set
    End Property
    Public Property NumberOfDoors() As Integer
        Get
            Return _numberOfDoors
        End Get
        Private Set(ByVal value As Integer)
            _numberOfDoors = value
        End Set
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

    Public Overloads Function Equals(ByVal obj As ModelGenerationBodyType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As BodyType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As BodyTypeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is BodyTypeInfo Then
            Return Me.Equals(DirectCast(obj, BodyTypeInfo))
        ElseIf TypeOf obj Is ModelGenerationBodyType Then
            Return Me.Equals(DirectCast(obj, ModelGenerationBodyType))
        ElseIf TypeOf obj Is BodyType Then
            Return Me.Equals(DirectCast(obj, BodyType))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is BodyTypeInfo Then
            Return DirectCast(objA, BodyTypeInfo).Equals(objB)
        ElseIf TypeOf objB Is BodyTypeInfo Then
            Return DirectCast(objB, BodyTypeInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetBodyTypeInfo(ByVal bodytype As BodyType) As BodyTypeInfo
        Dim _bodyType As BodyTypeInfo = New BodyTypeInfo
        _bodyType.ID = bodytype.ID
        _bodyType.Name = bodytype.Name
        _bodyType.NumberOfDoors = bodytype.NumberOfDoors
        Return _bodyType
    End Function
    Friend Shared Function GetBodyTypeInfo(ByVal bodytype As ModelGenerationBodyType) As BodyTypeInfo
        Dim _bodyType As BodyTypeInfo = New BodyTypeInfo
        _bodyType.ID = bodytype.ID
        _bodyType.Name = bodytype.Name
        _bodyType.NumberOfDoors = bodytype.NumberOfDoors
        Return _bodyType
    End Function

    Friend Shared Function GetBodyTypeInfo(ByVal dataReader As SafeDataReader) As BodyTypeInfo
        Dim _bodyType As BodyTypeInfo = New BodyTypeInfo
        With dataReader
            _bodyType.ID = .GetGuid("BODYTYPEID")
            _bodyType.Name = .GetString("BODYTYPENAME")
            _bodyType.NumberOfDoors = .GetInt16("BODYTYPENUMBEROFDOORS")
        End With
        Return _bodyType
    End Function

    Friend Shared Function GetFactoryBodyTypeInfo(ByVal dataReader As SafeDataReader) As BodyTypeInfo
        Dim _bodyType As BodyTypeInfo = New BodyTypeInfo
        With dataReader
            _bodyType.ID = .GetGuid("FACTORYBODYTYPEID")
            _bodyType.Name = .GetString("FACTORYBODYTYPENAME")
            _bodyType.NumberOfDoors = .GetInt16("FACTORYBODYTYPENUMBEROFDOORS")
        End With
        Return _bodyType
    End Function

    Public Shared ReadOnly Property Empty() As BodyTypeInfo
        Get
            Return New BodyTypeInfo
        End Get
    End Property

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class