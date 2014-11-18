Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Units
    Inherits BaseObjects.ContextUniqueGuidListBase(Of Units, Unit)

#Region " Business Properties & Methods "
    Default Public Overloads Overrides ReadOnly Property Item(ByVal code As String) As Unit
        Get
            Return (From x In Me Where String.Compare(x.Code, code, StringComparison.InvariantCultureIgnoreCase) = 0).FirstOrDefault()
        End Get
    End Property
    Default Public Overloads ReadOnly Property Item(ByVal codeToCompare As String, ByVal isLocalCode As Boolean) As Unit
        Get
            Return (From x In Me Where x.Equals(codeToCompare, isLocalCode)).FirstOrDefault()
        End Get
    End Property
    Public Function FindByName(ByVal name As String) As Unit
        Return (From x In Me Where String.Compare(x.Name, name, StringComparison.InvariantCultureIgnoreCase) = 0).FirstOrDefault()
    End Function
#End Region

#Region " Shared Factory Methods "

    Public Shared Function GetUnits() As Units
        Dim _criteria As Criteria = New Criteria()
        _criteria.CommandText = "getSpecificationUnits"
        Return DataPortal.Fetch(Of Units)(_criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region


End Class
<Serializable()> Public NotInheritable Class Unit
    Inherits BaseObjects.LocalizeableBusinessBase

#Region " Business Properties & Methods "
    ' Declare variables to contain object state
    ' Declare variables for any child collections

    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _owner As String = String.Empty

    ' Implement properties and methods for interaction of the UI,
    ' or any other client code, with the object
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
    Public Property Owner() As String
        Get
            Return _owner
        End Get
        Set(ByVal value As String)
            If _owner <> value Then
                _owner = value
                PropertyHasChanged("Owner")
            End If
        End Set
    End Property

    Public Function GetInfo() As UnitInfo
        Return UnitInfo.GetUnitInfo(Me)
    End Function


    Public Shadows ReadOnly Property AllowEdit() As Boolean
        Get
            Return MyBase.AllowRemove
        End Get
    End Property
    Public Shadows ReadOnly Property AllowRemove() As Boolean
        Get
            Return MyBase.AllowRemove
        End Get
    End Property
#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Owner")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Owner", 2))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")
    End Sub
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function Equals(ByVal value As String) As Boolean
        If value Is Nothing Then Return False
        If String.Compare(value, Me.Name, True) = 0 Then Return True
        If Not Me.Equals(value, True) AndAlso Not Me.Equals(value, False) Then Return False
        Return True
    End Function
    Friend Overloads Function Equals(ByVal codeToCompare As String, ByVal isLocalCode As Boolean) As Boolean
        Dim sBuffer As String
        If isLocalCode Then
            sBuffer = ";" + Me.LocalCode.ToLower() + ";"
        Else
            sBuffer = ";" + Me.Code.ToLower() + ";"
        End If
        Return (sBuffer.IndexOf(";" & codeToCompare.ToLower & ";") > -1)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AutoDiscover = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _owner = MyContext.GetContext().CountryCode
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _owner = .GetString("OWNER")
        End With
        MyBase.FetchFields(dataReader)
        MyBase.AllowRemove = String.Compare(Me.Owner, MyContext.GetContext().CountryCode, True) = 0 OrElse String.Compare(MyContext.GetContext().CountryCode, Environment.GlobalCountryCode, True) = 0 AndAlso Not Me.ID.Equals(Guid.Empty)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        command.CommandText = "insertSpecificationUnit"
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        command.CommandText = "updateSpecificationUnit"
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "deleteSpecificationUnit"
        MyBase.AddDeleteCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@INTERNALCODE", Me.Code)
        command.Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
        command.Parameters.AddWithValue("@OWNER", Me.Owner)
    End Sub

#End Region

#Region " Base Object Overrides "
    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Protected Friend Overrides Function GetBaseCode() As String
        Return Me.Code
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.UNIT
        End Get
    End Property
#End Region

End Class
<Serializable()> Public Structure UnitInfo
#Region " Business Properties & Methods "
    Private _id As Guid
    Private _name As String

    Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
#End Region

#Region " System.Object Overrides "
    Public Overloads Function Equals(ByVal unit As UnitInfo) As Boolean
        Return unit.ID.Equals(Me.ID)
    End Function
    Public Overloads Function Equals(ByVal unit As Unit) As Boolean
        Return unit.ID.Equals(Me.ID)
    End Function
    Public Overloads Function Equals(ByVal unit As Guid) As Boolean
        Return unit.Equals(Me.ID)
    End Function
#End Region

#Region " Shared Factory Methods "
    Public Shared Function [Default]() As UnitInfo
        Dim _info As UnitInfo = New UnitInfo
        _info._id = Guid.Empty
        _info._name = ""
        Return _info
    End Function
    Public Shared Function GetUnitInfo(ByVal unit As Unit) As UnitInfo
        Dim _info As UnitInfo = New UnitInfo
        _info._id = unit.ID
        _info._name = unit.Name
        Return _info
    End Function
    Friend Shared Function GetUnitInfo(ByVal dataReader As SafeDataReader) As UnitInfo
        Dim _info As UnitInfo = New UnitInfo
        _info._id = dataReader.GetGuid("TECHSPECUNITID")
        _info._name = dataReader.GetString("TECHSPECUNITNAME")
        Return _info
    End Function
#End Region

End Structure
