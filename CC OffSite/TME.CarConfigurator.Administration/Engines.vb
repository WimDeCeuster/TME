Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Engines
    Inherits BaseObjects.ContextUniqueGuidListBase(Of Engines, Engine)

#Region " Shared Factory Methods "

    Public Shared Function GetEngines() As Engines
        Return DataPortal.Fetch(Of Engines)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class Engine
    Inherits BaseObjects.LocalizeableBusinessBase
    Implements ILinkedAssets

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _owner As String = String.Empty
    Private _kw As Integer = 0
    Private _min1 As Integer = 0
    Private _assets As LinkedAssets
    Private _engineType As EngineTypeInfo
    Private _engineCategory As EngineCategoryInfo
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
    Private Sub AnyPropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If e.PropertyName.Equals("LocalCode", StringComparison.InvariantCultureIgnoreCase) Then
            If Me.Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then
                Me.Code = Me.LocalCode
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
    <XmlInfo(XmlNodeType.Attribute)> Public Property KW() As Integer
        Get
            Return _kw
        End Get
        Set(ByVal value As Integer)
            If Not _kw.Equals(Value) Then
                _kw = Value
                PropertyHasChanged("KW")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property MIN1() As Integer
        Get
            Return _min1
        End Get
        Set(ByVal value As Integer)
            If Not _min1.Equals(Value) Then
                _min1 = Value
                PropertyHasChanged("MIN1")
            End If
        End Set
    End Property

    Public Property Type() As EngineTypeInfo
        Get
            Return _engineType
        End Get
        Set(ByVal value As EngineTypeInfo)
            If Value Is Nothing Then Exit Property

            If (_engineType Is Nothing OrElse Not _engineType.Equals(Value.ID)) Then
                _engineType = Value
                PropertyHasChanged("Type")
            End If
        End Set
    End Property
    Public Property Category() As EngineCategoryInfo
        Get
            Return _engineCategory
        End Get
        Set(ByVal value As EngineCategoryInfo)
            If Value Is Nothing Then Exit Property

            If (_engineCategory Is Nothing OrElse Not _engineCategory.Equals(Value.ID)) Then
                _engineCategory = Value
                PropertyHasChanged("Category")
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
                If Me.IsNew Then
                    _assets = LinkedAssets.NewLinkedAssets(Me.ID)
                Else
                    _assets = LinkedAssets.GetLinkedAssets(Me.ID)
                End If
            End If
            Return _assets
        End Get
    End Property


    Public Function GetInfo() As EngineInfo
        Return EngineInfo.GetEngineInfo(Me)
    End Function

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Type")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
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
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _owner = MyContext.GetContext().CountryCode
        _engineCategory = EngineCategoryInfo.Empty
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        'Load object data	into variables
        With dataReader
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _owner = .GetString("OWNER").ToUpperInvariant()
            _kw = .GetInt16("KW")
            _min1 = .GetInt16("MIN1")
            _visible = .GetBoolean("VISIBLE", True)
        End With
        _engineType = EngineTypeInfo.GetEngineTypeInfo(dataReader)

        If dataReader.FieldExists("ENGINECATEGORYID") Then
            _engineCategory = EngineCategoryInfo.GetEngineCategoryInfo(dataReader)
        Else
            _engineCategory = EngineCategoryInfo.Empty
        End If

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
        command.Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
        command.Parameters.AddWithValue("@ENGINETYPEID", Me.Type.ID)
        command.Parameters.AddWithValue("@ENGINECATEGORYID", Me.Category.ID.GetDBValue)
        command.Parameters.AddWithValue("@KW", Me.KW)
        command.Parameters.AddWithValue("@MIN1", Me.MIN1)
        command.Parameters.AddWithValue("@OWNER", Me.Owner)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _assets Is Nothing Then _assets.Update(transaction)
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
            Return Entity.ENGINE
        End Get
    End Property
#End Region

End Class
<Serializable(), XmlInfo("engine")> Public NotInheritable Class EngineInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _type As EngineTypeInfo = EngineTypeInfo.Empty
    Private _kw As Integer = 0
    Private _min1 As Integer = 0
    
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
    Public ReadOnly Property Type() As EngineTypeInfo
        Get
            Return _type
        End Get
    End Property
    Public ReadOnly Property KW() As Integer
        Get
            Return _KW
        End Get
    End Property
    Public ReadOnly Property MIN1() As Integer
        Get
            Return _MIN1
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

    Public Overloads Function Equals(ByVal obj As ModelGenerationEngine) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Engine) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EngineInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is EngineInfo Then
            Return Me.Equals(DirectCast(obj, EngineInfo))
        ElseIf TypeOf obj Is ModelGenerationEngine Then
            Return Me.Equals(DirectCast(obj, ModelGenerationEngine))
        ElseIf TypeOf obj Is Engine Then
            Return Me.Equals(DirectCast(obj, Engine))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is EngineInfo Then
            Return DirectCast(objA, EngineInfo).Equals(objB)
        ElseIf TypeOf objB Is EngineInfo Then
            Return DirectCast(objB, EngineInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetEngineInfo(ByVal dataReader As SafeDataReader) As EngineInfo
        Dim _engine As EngineInfo = New EngineInfo
        _engine.Fetch(dataReader)
        Return _engine
    End Function
    Friend Shared Function GetFactoryEngineInfo(ByVal dataReader As SafeDataReader) As EngineInfo
        Dim _engine As EngineInfo = New EngineInfo
        _engine.Fetch(dataReader, "FACTORY")
        Return _engine
    End Function
    Friend Shared Function GetEngineInfo(ByVal engine As Engine) As EngineInfo
        Dim _engine As EngineInfo = New EngineInfo
        _engine.Fetch(engine)
        Return _engine
    End Function
    Friend Shared Function GetEngineInfo(ByVal engine As ModelGenerationEngine) As EngineInfo
        Dim _engine As EngineInfo = New EngineInfo
        _engine.Fetch(engine)
        Return _engine
    End Function

    Public Shared ReadOnly Property Empty() As EngineInfo
        Get
            Return New EngineInfo
        End Get
    End Property
#End Region

#Region " Constructors "
    Private Sub New()
        '  'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal dataReader As SafeDataReader, Optional ByVal fieldPrefix As String = "")
        With dataReader
            _id = .GetGuid(fieldPrefix & "ENGINEID")
            _code = .GetString(fieldPrefix & "ENGINECODE")
            _name = .GetString(fieldPrefix & "ENGINENAME")
            _type = EngineTypeInfo.GetEngineTypeInfo(dataReader)
            _kw = .GetInt16(fieldPrefix & "ENGINEKW")
            _min1 = .GetInt16(fieldPrefix & "ENGINEMIN1")
        End With
    End Sub
    Private Sub Fetch(ByVal engine As Engine)
        With engine
            _id = .ID
            _code = .Code
            _name = .Name
            _type = .Type
            _KW = .KW
            _MIN1 = .MIN1
        End With
    End Sub
    Private Sub Fetch(ByVal engine As ModelGenerationEngine)
        With engine
            _id = .ID
            _code = .Code
            _name = .Name
            _type = .Type
            _KW = .KW
            _MIN1 = .MIN1
        End With
    End Sub
#End Region

End Class