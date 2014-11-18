Imports TME.CarConfigurator.Administration.Security
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class EquipmentCategories
    Inherits BaseObjects.StronglySortedListBase(Of EquipmentCategories, EquipmentCategory)

#Region " Business Properties & Methods "

    <NotUndoable()> Private _referenceMap As Generic.Dictionary(Of Guid, EquipmentCategory)

    Public Function Find(ByVal id As Guid) As EquipmentCategory
        If id.Equals(Guid.Empty) Then Return Nothing
        If Me.ReferenceMap.ContainsKey(id) Then
            Return Me.ReferenceMap.Item(id)
        Else
            Return Nothing
        End If
    End Function
    Public Function FindEquipment(ByVal id As Guid) As EquipmentItem
        If id.Equals(Guid.Empty) Then Return Nothing
        For Each _category As EquipmentCategory In Me.ReferenceMap.Values
            If _category.Equipment.Contains(id) Then Return _category.Equipment(id)
        Next
        Return Nothing
    End Function
    Public Function Find(ByVal code As String) As EquipmentCategory
        If code.Equals(String.Empty) Then Return Nothing
        For Each _category As EquipmentCategory In Me.ReferenceMap.Values
            If _category.Equals(code) Then
                Return _category
            End If
        Next
        Return Nothing
    End Function

    Public ReadOnly Property ParentCategory() As EquipmentCategory
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, EquipmentCategory)
        End Get
    End Property
    Private ReadOnly Property ReferenceMap() As Generic.Dictionary(Of Guid, EquipmentCategory)
        Get
            If _referenceMap Is Nothing Then
                _referenceMap = Me.ParentCategory.ParentCategory.Categories.ReferenceMap()
            End If
            Return _referenceMap
        End Get
    End Property

    Private Sub EquipmentCategoriesListChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
            Dim _category As EquipmentCategory = Me(e.NewIndex)
            Me.ReferenceMap.Add(_category.ID, _category)
        End If
    End Sub
    Private Sub EquipmentCategoriesRemovingItem(ByVal sender As Object, ByVal e As BusinessObjects.Core.RemovingItemEventArgs) Handles Me.RemovingItem
        Me.ReferenceMap.Remove(DirectCast(e.RemovingItem, EquipmentCategory).ID)
    End Sub

    Public Function GetDescendants() As Generic.IList(Of EquipmentCategory)
        Dim _list As Generic.List(Of EquipmentCategory) = New Generic.List(Of EquipmentCategory)
        AddDescendants(Me.ParentCategory, _list)
        Return _list
    End Function
    Private Sub AddDescendants(ByVal category As EquipmentCategory, ByVal list As Generic.IList(Of EquipmentCategory))
        For Each _category As EquipmentCategory In category.Categories
            list.Add(_category)
            AddDescendants(_category, list)
        Next
    End Sub

#End Region

#Region " Shared Factory Methods "
    Public Shared Function GetEquipmentCategories() As EquipmentCategories
        Return GetEquipmentCategories(False)
    End Function
    Public Shared Function GetEquipmentCategories(ByVal lazyLoad As Boolean) As EquipmentCategories
        Return DataPortal.Fetch(Of EquipmentCategories)(New CustomCriteria(lazyLoad))
    End Function
    Friend Shared Function NewEquipmentCategories(ByVal parent As EquipmentCategory) As EquipmentCategories
        Dim _categories As EquipmentCategories = New EquipmentCategories()
        _categories.SetParent(parent)
        _categories.MarkAsChild()
        Return _categories
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _lazyLoad As Boolean
        Public Sub New(ByVal lazyLoad As Boolean)
            _lazyLoad = lazyLoad
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@LAZYLOAD", _lazyLoad)
        End Sub
    End Class
#End Region

#Region " Data Access "

    Protected Overrides Sub Fetch(ByVal dataReader As Common.Database.SafeDataReader)
        Me.RaiseListChangedEvents = True
        Dim _rootCategory As EquipmentCategory = EquipmentCategory.DummyRootCategory(Me)
        Me.SetParent(_rootCategory)

        _referenceMap = New Generic.Dictionary(Of Guid, EquipmentCategory)
        _referenceMap.Add(_rootCategory.ID, _rootCategory)
        While dataReader.Read()
            Dim _category As EquipmentCategory = GetObject(dataReader)
            _referenceMap(dataReader.GetGuid("PARENTCATEGORYID")).Categories.Add(_category)
        End While
        Me.RaiseListChangedEvents = True
    End Sub
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        FetchEquipment(dataReader)
        If dataReader.NextResult() Then FetchFittings(dataReader)
        If dataReader.NextResult() Then FetchIncompatibilities(dataReader)
        If dataReader.NextResult() Then FetchSuffixFittings(dataReader)
    End Sub
    Private Sub FetchEquipment(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            Dim _id As Guid = Guid.Empty
            Dim _category As EquipmentCategory = Nothing

            For Each _category In _referenceMap.Values
                _category.PrepareEquipment()
            Next

            While .Read()
                If Not _id.Equals(.GetGuid("EQUIPMENTCATEGORYID")) Then
                    _id = .GetGuid("EQUIPMENTCATEGORYID")
                    _category = Me.Find(_id)
                End If
                If _category IsNot Nothing Then
                    Call _category.Equipment.Add(dataReader)
                End If
            End While
        End With
    End Sub
    Private Sub FetchFittings(ByVal dataReader As SafeDataReader)
        PrepareForEagerLoadOfApplicabilities()

        Dim _id As Guid = Guid.Empty
        Dim _item As EquipmentItem = Nothing

        While dataReader.Read
            If Not _id.Equals(dataReader.GetGuid("EQUIPMENTID")) Then
                _id = dataReader.GetGuid("EQUIPMENTID")
                _item = Me.FindEquipment(_id)
            End If
            If Not _item Is Nothing Then
                _item.FetchFitting(dataReader)
            End If
        End While
    End Sub

    Private Sub PrepareForEagerLoadOfApplicabilities()
        For Each category In _referenceMap.Values
            category.Equipment.PrepareForEagerLoadOfApplicabilities()
        Next
    End Sub

    Private Sub FetchIncompatibilities(ByVal dataReader As SafeDataReader)
        Dim _id As Guid = Guid.Empty
        Dim _item As EquipmentItem = Nothing

        While dataReader.Read
            If Not _id.Equals(dataReader.GetGuid("EQUIPMENTID")) Then
                _id = dataReader.GetGuid("EQUIPMENTID")
                _item = Me.FindEquipment(_id)
            End If
            If Not _item Is Nothing Then
                _item.FetchIncompatibility(dataReader)
            End If
        End While
    End Sub
    Private Sub FetchSuffixFittings(ByVal dataReader As SafeDataReader)
        Dim _id As Guid = Guid.Empty
        Dim _item As [Option] = Nothing

        While dataReader.Read
            If Not _id.Equals(dataReader.GetGuid("EQUIPMENTID")) Then
                _id = dataReader.GetGuid("EQUIPMENTID")
                _item = DirectCast(Me.FindEquipment(_id), [Option])
            End If
            If Not _item Is Nothing Then
                _item.FetchSuffixFitting(dataReader)
            End If
        End While
    End Sub

#End Region

End Class
<Serializable()> Public NotInheritable Class EquipmentCategory
    Inherits BaseObjects.LocalizeableBusinessBase
    Implements BaseObjects.ISortedIndex
    Implements ISortedIndexSetter

#Region " Business Properties & Methods "
    ' Declare variables for any child collections
    ' Declare variables to contain object state

    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _index As Integer

    Private _children As EquipmentCategories
    Private _equipment As EquipmentItems

    ' Implement properties and methods for interaction of the UI, or any other client code, with the object
    Public Shadows ReadOnly Property ID() As System.Guid
        Get
            Return MyBase.ID
        End Get
    End Property

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
            If Not PermissionBroker.GetBroker().GetPermissions(Me).Sort Then Exit Property
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.None)> Public Property ParentCategory() As EquipmentCategory
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, EquipmentCategories).ParentCategory
        End Get
        Set(ByVal value As EquipmentCategory)
            If value Is Nothing Then
                value = Me.ParentCategory
                Do While Not value.ID.Equals(Guid.Empty)
                    value = value.ParentCategory
                Loop
            End If

            If Not Me.Parent.Equals(value) Then
                Me.ParentCategory.Categories.Remove(Me)
                value.Categories.Add(Me)
                Me.MarkUnDeleted()
                PropertyHasChanged("ParentCategory")
            End If
        End Set
    End Property
    Public Function IsRootCategory() As Boolean
        If Me.ParentCategory Is Nothing Then Return True
        If Me.ParentCategory.ID.Equals(Guid.Empty) Then Return True
        Return False
    End Function

    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Path() As String
        Get
            Dim _category As EquipmentCategory = Me.ParentCategory
            Dim _path As String = Me.Name

            While (Not _category Is Nothing) AndAlso (Not _category.ID.Equals(Guid.Empty))
                _path = _category.Name & "/" & _path
                _category = _category.ParentCategory
            End While

            Return _path
        End Get
    End Property
    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property SortPath() As String
        Get
            Dim _category As EquipmentCategory = Me.ParentCategory
            Dim _path As String = Me.Index.ToString()

            While (Not _category Is Nothing) AndAlso (Not _category.ID.Equals(Guid.Empty))
                _path = _category.Index.ToString() & "/" & _path
                _category = _category.ParentCategory
            End While

            Return _path
        End Get
    End Property

    Public ReadOnly Property Categories() As EquipmentCategories
        Get
            If _children Is Nothing Then _children = EquipmentCategories.NewEquipmentCategories(Me)
            Return _children
        End Get
    End Property

    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Equipment() As EquipmentItems
        Get
            If _equipment Is Nothing Then
                If IsNew Then
                    _equipment = EquipmentItems.NewEquipmentItems(Me)
                Else
                    _equipment = EquipmentItems.GetEquipmentItems(Me)
                End If
            End If
            Return _equipment
        End Get
    End Property
    Public Sub MoveEquipmentItemToAnotherCategory(ByVal equipmentItem As EquipmentItem, ByVal newLocation As EquipmentCategoryInfo)
        If newLocation Is Nothing Then Exit Sub
        If newLocation.IsEmpty() Then Exit Sub

        Equipment.Remove(equipmentItem)
        Categories.Find(newLocation.ID).Equipment.Add(equipmentItem)
    End Sub
    Friend Sub PrepareEquipment()
        _equipment = EquipmentItems.NewEquipmentItems(Me)
    End Sub

    Public Function GetInfo() As EquipmentCategoryInfo
        Return EquipmentCategoryInfo.GetEquipmentCategoryInfo(Me)
    End Function

#End Region

#Region " Business  & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
    End Sub

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_children Is Nothing) AndAlso Not _children.IsValid Then Return False
            If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_children Is Nothing) AndAlso _children.IsDirty Then Return True
            If Not (_equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
            Return False
        End Get
    End Property

    Public Overloads Overrides Sub Delete()
        Me.ParentCategory.Categories.Remove(Me)
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        If obj.Equals(String.Empty) Then Return False
        Dim _buffer As String = ";" + Me.LocalCode.ToLowerInvariant() + ";"
        If _buffer.IndexOf(";" & obj.ToLowerInvariant & ";", System.StringComparison.Ordinal) > -1 Then
            Return True
        Else
            Return String.Compare(Me.Code, obj, True) = 0
        End If
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        Return MyBase.Equals(obj)
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function DummyRootCategory(ByVal rootCategories As EquipmentCategories) As EquipmentCategory
        Return New EquipmentCategory(rootCategories)
    End Function

#End Region

#Region " Constructors "
    Private Sub New(ByVal rootCategories As EquipmentCategories)
        'Constructor for the dummy root Group
        MyBase.ID = Guid.Empty
        _children = rootCategories
        AllowRemove = False
        AllowEdit = False
        MarkAsChild()
    End Sub
    Private Sub New()
        'Concstructor for a new and Group
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _index = .GetInt16("SORTORDER")
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@INTERNALCODE", Me.Code)
            .Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
            .Parameters.AddWithValue("@SHORTNAME", Me.Name)
            .Parameters.AddWithValue("@SORTORDER", Me.Index)
            .Parameters.AddWithValue("@PARENTCATEGORYID", Me.ParentCategory.ID.GetDbValue())
        End With
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _children Is Nothing Then _children.Update(transaction)
        If Not _equipment Is Nothing Then _equipment.Update(transaction)
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
            Return Entity.EQUIPMENTCATEGORY
        End Get
    End Property
#End Region


End Class
<Serializable()> Public NotInheritable Class EquipmentCategoryInfo

#Region " Business Properties & Methods "

    ' Declare variables to contain object state
    ' Declare variables for any child collections

    Private _id As Guid = Guid.Empty
    Private _name As String = String.Empty

    ' Implement read-only properties and methods for interaction of the UI,
    ' or any other client code, with the object

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    Public Function IsEmpty() As Boolean
        Return ID.Equals(Guid.Empty)
    End Function

    Public Shared ReadOnly Property Empty As EquipmentCategoryInfo
        Get
            Return New EquipmentCategoryInfo()
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

    Public Overloads Function Equals(ByVal obj As EquipmentCategory) As Boolean
        'Compare two objects
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EquipmentCategoryInfo) As Boolean
        'Compare two objects
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        'Compare two objects
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is EquipmentCategory Then
            Return Me.Equals(DirectCast(obj, EquipmentCategory))
        ElseIf TypeOf obj Is EquipmentCategoryInfo Then
            Return Me.Equals(DirectCast(obj, EquipmentCategoryInfo))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is EquipmentCategoryInfo Then
            Return DirectCast(objA, EquipmentCategoryInfo).Equals(objB)
        ElseIf TypeOf objB Is EquipmentCategoryInfo Then
            Return DirectCast(objB, EquipmentCategoryInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetEquipmentCategoryInfo(ByVal dataReader As SafeDataReader) As EquipmentCategoryInfo
        If dataReader.GetGuid("EQUIPMENTCATEGORYID").Equals(Guid.Empty) Then Return Empty

        Dim _category As EquipmentCategoryInfo = New EquipmentCategoryInfo()
        _category.Fetch(dataReader)
        Return _category
    End Function
    Friend Shared Function GetEquipmentCategoryInfo(ByVal category As EquipmentCategory) As EquipmentCategoryInfo
        Dim _category As EquipmentCategoryInfo = New EquipmentCategoryInfo()
        _category.Fetch(category)
        Return _category
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("EQUIPMENTCATEGORYID")
            _name = .GetString("EQUIPMENTCATEGORYNAME")
        End With
    End Sub
    Private Sub Fetch(ByVal category As EquipmentCategory)
        With category
            _id = .ID
            _name = .Name
        End With
    End Sub

#End Region
End Class