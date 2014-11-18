Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules
<Serializable()> Public NotInheritable Class WheelDrives
    Inherits BaseObjects.StronglySortedListBase(Of WheelDrives, WheelDrive)

#Region " Shared Factory Methods "

    Public Shared Function GetWheelDrives() As WheelDrives
        Return DataPortal.Fetch(Of WheelDrives)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class WheelDrive
    Inherits BaseObjects.LocalizeableBusinessBase
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter
    Implements ILinkedAssets

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _a2PCode As String = String.Empty
    Private _name As String = String.Empty
    Private _index As Integer
    Private _assets As LinkedAssets

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
    Public Property A2PCode() As String
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

    Public ReadOnly Property Index() As Integer Implements BaseObjects.ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements BaseObjects.ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not CanWriteProperty("Index") Then Exit Property
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property

    Public ReadOnly Property Assets() As LinkedAssets Implements ILinkedAssets.Assets
        Get
            If _assets Is Nothing Then
                If Me.IsNew Then
                    _assets = LinkedAssets.NewLinkedAssets(Me.ID)
                Else
                    _assets = LinkedAssets.GetLinkedAssets(Me.ID)
                End If
            End If
            Return _assets
        End Get
    End Property

    Public Function GetInfo() As WheelDriveInfo
        Return WheelDriveInfo.GetWheelDriveInfo(Me)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

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
        MyBase.AllowEdit = MyContext.GetContext().IsGlobal()
        MyBase.AllowNew = MyBase.AllowEdit
        MyBase.AllowRemove = MyBase.AllowEdit
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        'Load object data	into variables
        With dataReader
            _code = .GetString("INTERNALCODE")
            _a2PCode = .GetString("A2PCODE")
            _name = .GetString("SHORTNAME")
            _index = .GetInt16("SORTORDER")
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
        command.Parameters.AddWithValue("@A2PCODE", Me.A2PCode)
        command.Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
        command.Parameters.AddWithValue("@SORTORDER", Me.Index)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _assets Is Nothing Then _assets.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

#Region " Base Object Overrides "

    Protected Friend Overrides Function GetBaseCode() As String
        Return Me.Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.WHEELDRIVE
        End Get
    End Property
#End Region

    
End Class
<Serializable()> Public NotInheritable Class WheelDriveInfo

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
    Public ReadOnly Property Name() As String
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
    Public Overloads Function Equals(ByVal obj As ModelGenerationWheelDrive) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As WheelDrive) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As WheelDriveInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is WheelDriveInfo Then
            Return Me.Equals(DirectCast(obj, WheelDriveInfo))
        ElseIf TypeOf obj Is ModelGenerationWheelDrive Then
            Return Me.Equals(DirectCast(obj, ModelGenerationWheelDrive))
        ElseIf TypeOf obj Is WheelDrive Then
            Return Me.Equals(DirectCast(obj, WheelDrive))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is WheelDriveInfo Then
            Return DirectCast(objA, WheelDriveInfo).Equals(objB)
        ElseIf TypeOf objB Is WheelDriveInfo Then
            Return DirectCast(objB, WheelDriveInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetWheelDriveInfo(ByVal dataReader As SafeDataReader) As WheelDriveInfo
        Dim _wheelDrive As WheelDriveInfo = New WheelDriveInfo
        _wheelDrive.Fetch(dataReader)
        Return _wheelDrive
    End Function
    Friend Shared Function GetWheelDriveInfo(ByVal wheelDrive As WheelDrive) As WheelDriveInfo
        Dim _wheelDrive As WheelDriveInfo = New WheelDriveInfo
        _wheelDrive.Fetch(wheelDrive)
        Return _wheelDrive
    End Function
    Friend Shared Function GetWheelDriveInfo(ByVal wheelDrive As ModelGenerationWheelDrive) As WheelDriveInfo
        Dim _wheelDrive As WheelDriveInfo = New WheelDriveInfo
        _wheelDrive.Fetch(wheelDrive)
        Return _wheelDrive
    End Function
    Public Shared ReadOnly Property Empty() As WheelDriveInfo
        Get
            Return New WheelDriveInfo
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
            _id = .GetGuid("WHEELDRIVEID")
            _code = .GetString("WHEELDRIVECODE")
            _name = .GetString("WHEELDRIVENAME")
        End With
    End Sub
    Private Sub Fetch(ByVal wheelDrive As WheelDrive)
        With wheelDrive
            _id = .ID
            _code = .Code
            _name = .Name
        End With
    End Sub
    Private Sub Fetch(ByVal wheelDrive As ModelGenerationWheelDrive)
        With wheelDrive
            _id = .ID
            _code = .Code
            _name = .Name
        End With
    End Sub
#End Region

End Class
