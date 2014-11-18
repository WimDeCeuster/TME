Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Security
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class SpecificationCategories
    Inherits BaseObjects.StronglySortedListBase(Of SpecificationCategories, SpecificationCategory)

#Region " Business Properties & Methods "

    <NotUndoable()> Private _referenceMap As Generic.Dictionary(Of Guid, SpecificationCategory)

    Public Function FindSpecification(ByVal id As Guid) As Specification
        If id.Equals(Guid.Empty) Then Return Nothing
        Return FindSpecification(id, GetRootObject())
    End Function
    Public Function FindSpecification(ByVal code As String, ByVal localCode As Boolean) As Specification
        Return FindSpecification(code, localCode, GetRootObject())
    End Function
    Private Shared Function FindSpecification(ByVal id As Guid, ByVal category As SpecificationCategory) As Specification
        Dim _specification As Specification = category.Specifications.Item(id)
        If _specification Is Nothing Then
            For Each _subCategory As SpecificationCategory In category.Categories
                _specification = FindSpecification(id, _subCategory)
                If Not _specification Is Nothing Then Return _specification
            Next
        End If
        Return _specification
    End Function
    Private Shared Function FindSpecification(ByVal code As String, ByVal localCode As Boolean, ByVal category As SpecificationCategory) As Specification
        Dim _specification As Specification = category.Specifications.Item(code, localCode)

        If _specification Is Nothing Then
            For Each _subCategory As SpecificationCategory In category.Categories
                _specification = FindSpecification(code, localCode, _subCategory)
                If Not _specification Is Nothing Then Return _specification
            Next
        End If

        Return _specification
    End Function

    Public Function Find(ByVal id As Guid) As SpecificationCategory
        If id.Equals(Guid.Empty) Then Return Nothing
        If Me.ReferenceMap.ContainsKey(id) Then
            Return Me.ReferenceMap.Item(id)
        Else
            Return Nothing
        End If
    End Function
    Public Function Find(ByVal code As String, ByVal localCode As Boolean) As SpecificationCategory
        Return Find(code, localCode, GetRootObject())
    End Function
    Private Shared Function Find(ByVal code As String, ByVal localCode As Boolean, ByVal category As SpecificationCategory) As SpecificationCategory
        If category.Equals(code, localCode) Then Return category

        Dim _category As SpecificationCategory = Nothing
        For Each _subCategory As SpecificationCategory In category.Categories
            _category = Find(code, localCode, _subCategory)
            If Not _category Is Nothing Then Return _category
        Next

        Return _category
    End Function

    Public ReadOnly Property ParentCategory() As SpecificationCategory
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, SpecificationCategory)
        End Get
    End Property
    Private ReadOnly Property ReferenceMap() As Generic.Dictionary(Of Guid, SpecificationCategory)
        Get
            If _referenceMap Is Nothing Then
                _referenceMap = Me.ParentCategory.ParentCategory.Categories.ReferenceMap()
            End If
            Return _referenceMap
        End Get
    End Property

    Private Function GetRootObject() As SpecificationCategory
        Dim _category As SpecificationCategory = Me.ParentCategory
        While Not _category.ID.Equals(Guid.Empty)
            _category = _category.ParentCategory
        End While
        Return _category
    End Function

    Private Sub SpecificationCategoriesListChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
            Dim _category As SpecificationCategory = Me(e.NewIndex)
            Me.ReferenceMap.Add(_category.ID, _category)
        End If
    End Sub
    Private Sub SpecificationCategoriesRemovingItem(ByVal sender As Object, ByVal e As BusinessObjects.Core.RemovingItemEventArgs) Handles Me.RemovingItem
        Me.ReferenceMap.Remove(DirectCast(e.RemovingItem, SpecificationCategory).ID)
    End Sub

    Public Function GetDescendants() As IEnumerable(Of SpecificationCategory)
        Dim _list As Generic.List(Of SpecificationCategory) = New Generic.List(Of SpecificationCategory)
        AddDescendants(Me.ParentCategory, _list)
        Return _list
    End Function
    Private Shared Sub AddDescendants(ByVal category As SpecificationCategory, ByVal list As ICollection(Of SpecificationCategory))
        For Each _category As SpecificationCategory In category.Categories
            list.Add(_category)
            AddDescendants(_category, list)
        Next
    End Sub
    Public Function GetDescendantSpecifications() As IEnumerable(Of Specification)
        Dim _list As Generic.List(Of Specification) = New Generic.List(Of Specification)
        AddDescendantSpecifications(Me.ParentCategory, _list)
        Return _list
    End Function
    Private Shared Sub AddDescendantSpecifications(ByVal category As SpecificationCategory, ByVal list As ICollection(Of Specification))
        For Each _specification As Specification In category.Specifications
            list.Add(_specification)
        Next
        For Each _category As SpecificationCategory In category.Categories
            AddDescendantSpecifications(_category, list)
        Next
    End Sub
#End Region

#Region " Shared Factory Methods "

    Public Shared Function GetSpecificationCategories(Optional ByVal lazyLoad As Boolean = False, Optional ByVal includeMapping As Boolean = False) As SpecificationCategories
        Return DataPortal.Fetch(Of SpecificationCategories)(New CustomCriteria(lazyLoad, includeMapping))
    End Function
    Friend Shared Function NewSpecificationCategories(ByVal parent As SpecificationCategory) As SpecificationCategories
        Dim _categories As SpecificationCategories = New SpecificationCategories()
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
        Private ReadOnly _includeMapping As Boolean

        Public Sub New(ByVal lazyLoad As Boolean, ByVal includeMapping As Boolean)
            _lazyLoad = lazyLoad
            _includeMapping = includeMapping
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@LAZYLOAD", _lazyLoad)
            command.Parameters.AddWithValue("@INCLUDEMAPPING", _includeMapping)
        End Sub
    End Class
#End Region

#Region " Data Access "
    Protected Overrides Sub Fetch(ByVal dataReader As Common.Database.SafeDataReader)
        Me.RaiseListChangedEvents = True
        Dim _rootCategory As SpecificationCategory = SpecificationCategory.DummyRootCategory(Me)
        Me.SetParent(_rootCategory)

        _referenceMap = New Generic.Dictionary(Of Guid, SpecificationCategory)
        _referenceMap.Add(_rootCategory.ID, _rootCategory)
        While dataReader.Read()
            Dim _category As SpecificationCategory = GetObject(dataReader)
            _referenceMap(dataReader.GetGuid("PARENTCATEGORYID")).Categories.Add(_category)
        End While
        Me.RaiseListChangedEvents = True
    End Sub
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        FetchSpecifications(dataReader)
        If dataReader.NextResult() Then FetchMapping(dataReader)
    End Sub
    Private Sub FetchSpecifications(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader

            For Each category In _referenceMap.Values
                category.PrepareSpecifications()
            Next

            Dim currentCategory As SpecificationCategory = Nothing
            While .Read()
                Dim currentcategoryId = .GetGuid("TECHSPECCATEGORYID")
                If currentCategory Is Nothing OrElse Not currentCategory.ID.Equals(currentcategoryId) Then currentCategory = Me.Find(currentcategoryId)
                If currentCategory IsNot Nothing Then
                    Call currentCategory.Specifications.Add(dataReader)
                End If
            End While
        End With
    End Sub
    Private Sub FetchMapping(ByVal dataReader As SafeDataReader)
        With dataReader

            For Each specification In (From c In _referenceMap.Values
                                       From s In c.Specifications
                                       Select s)
                specification.PrepareMapping()
            Next

            Dim currentCategory As SpecificationCategory = Nothing
            Dim currentSpecification As Specification = Nothing
            While .Read()
                Dim currentCategoryId = .GetGuid("TECHSPECCATEGORYID")
                If currentCategory Is Nothing OrElse Not currentCategory.ID.Equals(currentCategoryId) Then currentCategory = Me.Find(currentCategoryId)
                If currentCategory IsNot Nothing Then
                    Dim currentSpecificationId = .GetGuid("TECHSPECID")
                    If currentSpecification Is Nothing OrElse Not currentSpecification.ID.Equals(currentSpecificationId) Then currentSpecification = currentCategory.Specifications(currentSpecificationId)
                    If currentSpecification IsNot Nothing Then
                        Call currentSpecification.Mapping.Add(dataReader)
                    End If
                End If
            End While
        End With
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class SpecificationCategory
    Inherits BaseObjects.LocalizeableBusinessBase
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter

#Region " Business Properties & Methods "
    ' Declare variables for any child collections
    ' Declare variables to contain object state

    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _index As Integer

    Private _children As SpecificationCategories
    Private _specifications As Specifications

    ' Implement properties and methods for interaction of the UI, or any other client code, with the object
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
    <XmlInfo(XmlNodeType.None)> Public Property ParentCategory() As SpecificationCategory
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, SpecificationCategories).ParentCategory
        End Get
        Set(ByVal value As SpecificationCategory)

            If value Is Nothing Then
                value = Me.ParentCategory
                Do While Not value.ID.Equals(Guid.Empty)
                    value = value.ParentCategory
                Loop
            End If

            If Not Me.ParentCategory.Equals(value) Then
                Me.ParentCategory.Categories.Remove(Me)
                value.Categories.Add(Me)
                Me.MarkUnDeleted()
                PropertyHasChanged("ParentCategory")
            End If


        End Set
    End Property

    Public ReadOnly Property Categories() As SpecificationCategories
        Get
            If _children Is Nothing Then _children = SpecificationCategories.NewSpecificationCategories(Me)
            Return _children
        End Get
    End Property
    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Specifications() As Specifications
        Get
            If _specifications Is Nothing Then
                If Me.IsNew Then
                    _specifications = Specifications.NewSpecifications(Me)
                Else
                    _specifications = Specifications.GetSpecifications(Me)
                End If
            End If
            Return _specifications
        End Get
    End Property
    Friend Sub PrepareSpecifications()
        If _specifications Is Nothing Then _specifications = Specifications.NewSpecifications(Me)
    End Sub

    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Path() As String
        Get
            Dim _parent As SpecificationCategory = Me.ParentCategory
            Dim _path As String = Me.Name
            While (Not _parent Is Nothing) AndAlso (Not _parent.ID.Equals(Guid.Empty))
                _path = _parent.Name & "/" & _path
                _parent = _parent.ParentCategory
            End While
            Return _path
        End Get
    End Property
    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property AlternatePath() As String
        Get
            Dim _parent As SpecificationCategory = Me.ParentCategory
            Dim _path As String = Me.AlternateName

            While (_parent IsNot Nothing) AndAlso (Not _parent.ID.Equals(Guid.Empty))
                _path = _parent.AlternateName & "/" & _path
                _parent = _parent.ParentCategory
            End While
            Return _path
        End Get
    End Property

#End Region

#Region " Business  & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        If Not Me.Equals(obj, True) AndAlso Not Me.Equals(obj, False) Then Return False
        Return True
    End Function
    Public Overloads Function Equals(ByVal codeToCompare As String, ByVal isLocalCode As Boolean) As Boolean
        Dim _buffer As String
        If isLocalCode Then
            _buffer = ";" + Me.LocalCode.ToLower() + ";"
        Else
            _buffer = ";" + Me.Code.ToLower() + ";"
        End If
        Return (_buffer.IndexOf(";" & codeToCompare.ToLower & ";") > -1)
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_children Is Nothing) AndAlso Not _children.IsValid Then Return False
            If Not (_specifications Is Nothing) AndAlso Not _specifications.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_children Is Nothing) AndAlso _children.IsDirty Then Return True
            If Not (_specifications Is Nothing) AndAlso _specifications.IsDirty Then Return True
            Return False
        End Get
    End Property

    Public Shadows Sub Delete()
        Me.ParentCategory.Categories.Remove(Me)
    End Sub
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function DummyRootCategory(ByVal rootCategories As SpecificationCategories) As SpecificationCategory
        Dim _category As SpecificationCategory = New SpecificationCategory(rootCategories)
        _category.ID = Guid.Empty
        Return _category
    End Function

#End Region

#Region " Constructors "
    Private Sub New(ByVal rootCategories As SpecificationCategories)
        'Constructor for the dummy root Group
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
        If Not _specifications Is Nothing Then _specifications.Update(transaction)
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
            Return Entity.TECHSPECCATEGORY
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class SpecificationCategoryInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _name As String = String.Empty

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

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Me.ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As SpecificationCategory) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As SpecificationCategoryInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return Me.ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is EquipmentCategory Then
            Return Me.Equals(DirectCast(obj, SpecificationCategory))
        ElseIf TypeOf obj Is EquipmentCategoryInfo Then
            Return Me.Equals(DirectCast(obj, SpecificationCategoryInfo))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is SpecificationCategoryInfo Then
            Return DirectCast(objA, SpecificationCategoryInfo).Equals(objB)
        ElseIf TypeOf objB Is SpecificationCategoryInfo Then
            Return DirectCast(objB, SpecificationCategoryInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetSpecificationCategoryInfo(ByVal dataReader As SafeDataReader) As SpecificationCategoryInfo
        Dim _category As SpecificationCategoryInfo = New SpecificationCategoryInfo()
        _category.Fetch(dataReader)
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
            _id = .GetGuid("TECHSPECCATEGORYID")
            _name = .GetString("TECHSPECCATEGORYNAME")
        End With
    End Sub
#End Region

End Class