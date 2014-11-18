Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class EngineCategories
    Inherits BaseObjects.StronglySortedListBase(Of EngineCategories, EngineCategory)

#Region " Shared Factory Methods "
    Public Shared Function GetEngineCategories() As EngineCategories
        Return DataPortal.Fetch(Of EngineCategories)(New Criteria)
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class EngineCategory
    Inherits BaseObjects.TranslateableBusinessBase
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter
    Implements ILinkedAssets

#Region " Business Properties & Methods "
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _index As Integer
    Private _assets As LinkedAssets

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
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer Implements BaseObjects.ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements BaseObjects.ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not CanWriteProperty("Index") Then Exit Property
            If Not _index.Equals(Value) Then
                _index = Value
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

    Public Function GetInfo() As EngineCategoryInfo
        Return EngineCategoryInfo.GetEngineCategoryInfo(Me)
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
        Return String.Compare(Me.Code, obj, StringComparison.InvariantCultureIgnoreCase) = 0
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
    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Index", StringComparison.InvariantCultureIgnoreCase) Then Return True
        Return MyBase.CanWriteProperty(propertyName)
    End Function
#End Region

#Region "Base Object Overrides"

    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.ENGINECATEGORY
        End Get
    End Property
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEngineCategory(ByVal dataReader As SafeDataReader) As EngineCategory
        Dim _type As EngineCategory = New EngineCategory
        _type.Fetch(dataReader)
        _type.MarkAsChild()
        Return _type
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AutoDiscover = False 'There is nothing to discover anyway
        With MyContext.GetContext()
            Me.AllowNew = .IsGlobal()
            Me.AllowEdit = Not .IsRegionCountry OrElse .IsMainRegionCountry
            Me.AllowRemove = .IsGlobal()
        End With
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _code = .GetString("CODE")
            _name = .GetString("NAME")
            _index = .GetInt16("SORTORDER")
        End With
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@CODE", Me.Code)
        command.Parameters.AddWithValue("@NAME", Me.Name)
        command.Parameters.AddWithValue("@SORTORDER", Me.Index)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _assets Is Nothing Then _assets.Update(transaction)
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class EngineCategoryInfo

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

    Public Overloads Function Equals(ByVal obj As EngineCategory) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EngineCategoryInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is EngineCategoryInfo Then
            Return Me.Equals(DirectCast(obj, EngineCategoryInfo))
        ElseIf TypeOf obj Is FuelType Then
            Return Me.Equals(DirectCast(obj, EngineCategory))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is EngineCategoryInfo Then
            Return DirectCast(objA, EngineCategoryInfo).Equals(objB)
        ElseIf TypeOf objB Is EngineCategoryInfo Then
            Return DirectCast(objB, EngineCategoryInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEngineCategoryInfo(ByVal dataReader As SafeDataReader) As EngineCategoryInfo
        Dim _engineType As EngineCategoryInfo = New EngineCategoryInfo
        _engineType.Fetch(dataReader)
        Return _engineType
    End Function
    Friend Shared Function GetEngineCategoryInfo(ByVal category As EngineCategory) As EngineCategoryInfo
        Dim _engineType As EngineCategoryInfo = New EngineCategoryInfo
        _engineType.Fetch(category)
        Return _engineType
    End Function

    Public Shared ReadOnly Property Empty() As EngineCategoryInfo
        Get
            Return New EngineCategoryInfo
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
            _id = .GetGuid("ENGINECATEGORYID")
            _code = .GetString("ENGINECATEGORYCODE")
            _name = .GetString("ENGINECATEGORYNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal engineType As EngineCategory)
        With engineType
            _id = .ID
            _code = .Code
            _name = .AlternateName
        End With
    End Sub
#End Region

End Class