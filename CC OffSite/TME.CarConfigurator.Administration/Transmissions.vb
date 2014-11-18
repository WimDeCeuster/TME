Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Transmissions
    Inherits ContextUniqueGuidListBase(Of Transmissions, Transmission)

#Region " Shared Factory Methods "
    Public Shared Function GetTransmissions() As Transmissions
        Return DataPortal.Fetch(Of Transmissions)(New Criteria)
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        Dim context = MyContext.GetContext()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class Transmission
    Inherits LocalizeableBusinessBase
    Implements ILinkedAssets

#Region " Business Properties & Methods "

    Private _code As String = String.Empty
    Private _a2PCode As String = String.Empty
    Private _name As String = String.Empty
    Private _owner As String = String.Empty
    Private _numberOfGears As Integer = 0
    Private _assets As LinkedAssets
    Private _transmissionType As TransmissionTypeInfo
    Private _visible As Boolean = True

    <XmlInfo(XmlNodeType.Attribute)> Public Property Code() As String
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
    <XmlInfo(XmlNodeType.Attribute)> Public Property A2PCode() As String
        Get
            Return _a2PCode
        End Get
        Set(ByVal value As String)
            If _a2PCode <> value Then
                _a2PCode = value
                PropertyHasChanged("A2PCode")
            End If
        End Set
    End Property
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
    <XmlInfo(XmlNodeType.Attribute)> Public Property NumberOfGears() As Integer
        Get
            Return _numberOfGears
        End Get
        Set(ByVal value As Integer)
            If _numberOfGears <> value Then
                _numberOfGears = value
                PropertyHasChanged("NumberOfGears")
            End If
        End Set
    End Property
    Public Property Type() As TransmissionTypeInfo
        Get
            Return _transmissionType
        End Get
        Set(ByVal value As TransmissionTypeInfo)
            If value Is Nothing Then Exit Property

            If (_transmissionType Is Nothing OrElse Not _transmissionType.Equals(value.ID)) Then
                _transmissionType = value
                PropertyHasChanged("Type")
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
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Visible() As Boolean
        Get
            Return _visible
        End Get
    End Property

    Public ReadOnly Property Assets() As LinkedAssets Implements ILinkedAssets.Assets
        Get
            If _assets Is Nothing Then
                If IsNew Then
                    _assets = LinkedAssets.NewLinkedAssets(ID)
                Else
                    _assets = LinkedAssets.GetLinkedAssets(ID)
                End If
            End If
            Return _assets
        End Get
    End Property

    Public Function GetInfo() As TransmissionInfo
        Return TransmissionInfo.GetTransmissionInfo(Me)
    End Function
#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Type")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Int32.GreaterThanOrEquals, Validation.RuleHandler), New Rules.Int32.GreaterThanOrEqualsRuleArgs("NumberOfGears", "number of gears", 0))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("A2PCode", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")
    End Sub
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function Equals(ByVal value As String) As Boolean
        Return String.Compare(Code, value, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assets Is Nothing) AndAlso Not _assets.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assets Is Nothing) AndAlso _assets.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _owner = MyContext.GetContext().CountryCode
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        'Load object data	into variables
        With dataReader
            _code = .GetString("INTERNALCODE")
            _a2PCode = .GetString("A2PCODE")
            _name = .GetString("SHORTNAME")
            _numberOfGears = .GetInt16("NUMBEROFGEARS")
            _owner = .GetString("OWNER").ToUpperInvariant()
            _visible = .GetBoolean("VISIBLE", True)
        End With
        _transmissionType = TransmissionTypeInfo.GetTransmissionTypeInfo(dataReader)
        MyBase.FetchFields(dataReader)

        If Owner.Equals(MyContext.GetContext().CountryCode) Then
            _localCode = _code
            AllowEdit = True
            AllowRemove = True
            Return
        End If

        If MyContext.GetContext().IsGlobal() Then
            AllowEdit = True
            AllowRemove = True
            Return
        End If

        AllowEdit = False
        AllowRemove = False

    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@INTERNALCODE", Code)
        command.Parameters.AddWithValue("@A2PCODE", A2PCode)
        command.Parameters.AddWithValue("@LOCALCODE", LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Name)
        command.Parameters.AddWithValue("@NUMBEROFGEARS", NumberOfGears)
        command.Parameters.AddWithValue("@TRANSMISSIONTYPEID", Type.ID)
        command.Parameters.AddWithValue("@OWNER", Owner)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _assets Is Nothing Then _assets.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

#Region "Base Object Overrides"

    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.TRANSMISSION
        End Get
    End Property
#End Region
End Class
<Serializable()> Public NotInheritable Class TransmissionInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _type As TransmissionTypeInfo = TransmissionTypeInfo.Empty
    Private _numberOfGears As Integer = 0

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
    Public ReadOnly Property NumberOfGears() As Integer
        Get
            Return _numberOfGears
        End Get
    End Property
    Public ReadOnly Property Type() As TransmissionTypeInfo
        Get
            Return _type
        End Get
    End Property

    Public Function IsEmpty() As Boolean
        Return ID.Equals(Guid.Empty)
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationTransmission) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Transmission) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As TransmissionInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is TransmissionInfo Then
            Return Equals(DirectCast(obj, TransmissionInfo))
        ElseIf TypeOf obj Is ModelGenerationTransmission Then
            Return Equals(DirectCast(obj, ModelGenerationTransmission))
        ElseIf TypeOf obj Is Transmission Then
            Return Equals(DirectCast(obj, Transmission))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is TransmissionInfo Then
            Return DirectCast(objA, TransmissionInfo).Equals(objB)
        ElseIf TypeOf objB Is TransmissionInfo Then
            Return DirectCast(objB, TransmissionInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetTransmissionInfo(ByVal dataReader As SafeDataReader) As TransmissionInfo
        Dim info As TransmissionInfo = New TransmissionInfo
        info.Fetch(dataReader)
        Return info
    End Function
    Friend Shared Function GetFactoryTransmissionInfo(ByVal dataReader As SafeDataReader) As TransmissionInfo
        Dim info As TransmissionInfo = New TransmissionInfo
        info.Fetch(dataReader, "FACTORY")
        Return info
    End Function
    Friend Shared Function GetTransmissionInfo(ByVal transmission As Transmission) As TransmissionInfo
        Dim info As TransmissionInfo = New TransmissionInfo
        info.Fetch(transmission)
        Return info
    End Function
    Friend Shared Function GetTransmissionInfo(ByVal transmission As ModelGenerationTransmission) As TransmissionInfo
        Dim info As TransmissionInfo = New TransmissionInfo
        info.Fetch(transmission)
        Return info
    End Function
    Public Shared ReadOnly Property Empty() As TransmissionInfo
        Get
            Return New TransmissionInfo
        End Get
    End Property
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal dataReader As SafeDataReader, Optional ByVal fieldPrefix As String = "")
        With dataReader
            _id = .GetGuid(fieldPrefix & "TRANSMISSIONID")
            _code = .GetString(fieldPrefix & "TRANSMISSIONCODE")
            _name = .GetString(fieldPrefix & "TRANSMISSIONNAME")
            _numberOfGears = .GetInt16(fieldPrefix & "TRANSMISSIONNUMBEROFGEARS")
            _type = TransmissionTypeInfo.GetTransmissionTypeInfo(dataReader)
        End With
    End Sub
    Private Sub Fetch(ByVal transmission As Transmission)
        With transmission
            _id = .ID
            _code = .Code
            _name = .Name
            _type = .Type
            _numberOfGears = .NumberOfGears
        End With
    End Sub
    Private Sub Fetch(ByVal transmission As ModelGenerationTransmission)
        With transmission
            _id = .ID
            _code = .Code
            _name = .Name
            _type = .Type
            _numberOfGears = .NumberOfGears
        End With
    End Sub
#End Region

End Class