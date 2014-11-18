Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class EngineTypes
    Inherits BaseObjects.ContextUniqueGuidListBase(Of EngineTypes, EngineType)

#Region " Shared Factory Methods "
    Public Shared Function GetEngineTypes() As EngineTypes
        Return DataPortal.Fetch(Of EngineTypes)(New Criteria)
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class EngineType
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of EngineType)

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _description As String = String.Empty
    Private _fuelType As FuelTypeInfo

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
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
            If _name <> value Then
                _name = value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            If _description <> value Then
                _description = value
                PropertyHasChanged("Description")
            End If
        End Set
    End Property

    Public Property FuelType() As FuelTypeInfo
        Get
            Return _fuelType
        End Get
        Set(ByVal value As FuelTypeInfo)
            If value Is Nothing Then Exit Property

            If (_fuelType Is Nothing OrElse Not _fuelType.Equals(value.ID)) Then
                _fuelType = value
                PropertyHasChanged("FuelType")
            End If
        End Set
    End Property

    Public Function GetInfo() As EngineTypeInfo
        Return EngineTypeInfo.GetEngineTypeInfo(Me)
    End Function

#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "FuelType")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Description", 100))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
    End Sub
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Me.Code, obj, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEngineType(ByVal dataReader As SafeDataReader) As EngineType
        Dim _type As EngineType = New EngineType
        _type.Fetch(dataReader)
        _type.MarkAsChild()
        Return _type
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AutoDiscover = False 'There is nothing to discover anyway
        Me.FieldPrefix = "ENGINETYPE"
        MyBase.AllowEdit = MyContext.GetContext().IsGlobal()
        MyBase.AllowNew = MyBase.AllowEdit
        MyBase.AllowRemove = MyBase.AllowEdit
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _code = .GetString("ENGINETYPEINTERNALCODE")
            _name = .GetString("ENGINETYPESHORTNAME")
            _description = .GetString("ENGINETYPEDESCRIPTION")
        End With
        _fuelType = FuelTypeInfo.GetFuelTypeInfo(dataReader)
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
        command.Parameters.AddWithValue("@DESCRIPTION", Me.Description)
        command.Parameters.AddWithValue("@FUELTYPEID", Me.FuelType.ID)
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class EngineTypeInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _fuelType As FuelTypeInfo = FuelTypeInfo.Empty

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
    Public ReadOnly Property FuelType() As FuelTypeInfo
        Get
            Return _fuelType
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

    Public Overloads Function Equals(ByVal obj As EngineType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EngineTypeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is EngineTypeInfo Then
            Return Me.Equals(DirectCast(obj, EngineTypeInfo))
        ElseIf TypeOf obj Is FuelType Then
            Return Me.Equals(DirectCast(obj, EngineType))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is EngineTypeInfo Then
            Return DirectCast(objA, EngineTypeInfo).Equals(objB)
        ElseIf TypeOf objB Is EngineTypeInfo Then
            Return DirectCast(objB, EngineTypeInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEngineTypeInfo(ByVal dataReader As SafeDataReader) As EngineTypeInfo
        Dim _engineType As EngineTypeInfo = New EngineTypeInfo
        _engineType.Fetch(dataReader)
        Return _engineType
    End Function
    Friend Shared Function GetEngineTypeInfo(ByVal engineType As EngineType) As EngineTypeInfo
        Dim _engineType As EngineTypeInfo = New EngineTypeInfo
        _engineType.Fetch(engineType)
        Return _engineType
    End Function

    Public Shared ReadOnly Property Empty() As EngineTypeInfo
        Get
            Return New EngineTypeInfo
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
            _id = .GetGuid("ENGINETYPEID")
            _code = .GetString("ENGINETYPEINTERNALCODE")
            _name = .GetString("ENGINETYPESHORTNAME")
        End With
        _fuelType = FuelTypeInfo.GetFuelTypeInfo(dataReader)

    End Sub
    Private Sub Fetch(ByVal engineType As EngineType)
        With engineType
            _id = .ID
            _code = .Code
            _name = .Name
            _fuelType = .FuelType
        End With
    End Sub
#End Region

End Class