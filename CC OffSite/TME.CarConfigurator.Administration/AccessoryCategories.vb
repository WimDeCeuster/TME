Imports System.Collections.Generic
Imports System.ComponentModel
Imports TME.BusinessObjects.Templates.SqlServer
Imports TME.BusinessObjects.Core
Imports TME.BusinessObjects.Validation
Imports TME.CarConfigurator.Administration.Extensions.StringExtensions

<Serializable> Public Class AccessoryCategories
    Inherits ContextUniqueGuidListBase(Of AccessoryCategories, AccessoryCategory)
#Region "Business Properties & Methods"
    <NotUndoable()> Private _referenceList As IList(Of AccessoryCategory)

    Public ReadOnly Property ReferenceList() As IList(Of AccessoryCategory)
        Get
            _referenceList = If(_referenceList, ParentCategory.ParentCategory.Categories.ReferenceList)
            Return _referenceList
        End Get
    End Property

    Public ReadOnly Property ParentCategory() As AccessoryCategory
        Get
            Return DirectCast(Parent, AccessoryCategory)
        End Get
    End Property

    Private Sub AccessoryCategoriesListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType <> ListChangedType.ItemAdded Then Return
        Dim newCategory = Me(e.NewIndex)
        ReferenceList.Add(newCategory)
    End Sub

    Private Sub AccessoryCategoriesRemovingItem(ByVal sender As Object, ByVal e As RemovingItemEventArgs) Handles Me.RemovingItem
        Dim removedCategory = DirectCast(e.RemovingItem, AccessoryCategory)
        ReferenceList.Remove(removedCategory)
    End Sub

#Region "Find methods"
    Public Function Find(ByVal id As Guid) As AccessoryCategory
        Return ReferenceList.SingleOrDefault(Function(category) category.ID.Equals(id))
    End Function
#End Region

#End Region

#Region "Shared Factory Methods"
    Public Shared Function GetAccessoryCategories() As AccessoryCategories
        Return DataPortal.Fetch(Of AccessoryCategories)(New Criteria())
    End Function

    Public Shared Function NewAccessoryCategories(ByVal accessoryCategory As AccessoryCategory) As AccessoryCategories
        Dim categories = New AccessoryCategories()
        categories.SetParent(accessoryCategory)
        categories.MarkAsChild()
        Return categories
    End Function

#End Region

#Region "Framework Overrides"

    Protected Overrides Sub MyUpdate(ByVal transaction As SqlTransaction, ByVal changesToUpdate As ChangesToUpdate)
        If changesToUpdate = changesToUpdate.All Then
            ' make sure to first do the inserts and updates, before doing deletes => otherwise, subcategories will be deleted (stored procedure) before they can be moved
            MyBase.MyUpdate(transaction, changesToUpdate.Insert Or changesToUpdate.Update)
            MyBase.MyUpdate(transaction, changesToUpdate.All) 'do not set this to delete only => it won't delete all children (only those of the root)
            ' NOTE: Sure, this last line iterates all children for inserts/updates again, but it will skip them anyway (they aren't dirty anymore). 
            '       ChangesToUpdate.Delete isn 't implemented correctly in the framework => it will only delete immediate children, and doesn't care about lower level children.
            '       This works fine as long as the root category has also been deleted (the Stored Procedure handles this), but not when the root's direct children aren't deleted.
        Else
            ' if my parent node has already set the changesToUpdate to a special change (insert/update/delete) instead of all, just take my parent node's setting
            MyBase.MyUpdate(transaction, changesToUpdate)
        End If
    End Sub

#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region

#Region "Data Access"

    Protected Overrides Sub Fetch(ByVal dataReader As SafeDataReader)
        ' make sure that every subcategory that gets added also ends up in the referencemap (by having the listchanged event handled)
        RaiseListChangedEvents = True

        ' create a dummy root => this will hold all other categories (Me) fetched from the db
        Dim dummyRootCategory = AccessoryCategory.DummyRootCategory(Me)
        SetParent(dummyRootCategory)

        ' create a referenceMap => this will be used to find accessory categories easily
        _referenceList = New List(Of AccessoryCategory)
        _referenceList.Add(dummyRootCategory)

        ' get all categories
        While dataReader.Read()
            Dim category = GetObject(dataReader)

            'find the parent category in the reference map
            Dim parentCategoryId = dataReader.GetGuid("PARENTCATEGORYID")
            Dim categoryParent = _referenceList.Single(Function(accessoryCategory) accessoryCategory.ID.Equals(parentCategoryId))

            ' add the category to the list of sub-categories of the parent
            categoryParent.Categories.Add(category)
        End While
    End Sub

#End Region
End Class

<Serializable> Public Class AccessoryCategory
    Inherits ContextUniqueGuidBusinessBase(Of AccessoryCategory)

#Region "Business Properties & Methods"
    Private _name As String
    Private _categories As AccessoryCategories

    Private _accessories As Accessories
    Private _mappedEquipmentGroup As EquipmentGroupInfo
    Private _mappedEquipmentCategory As EquipmentCategoryInfo

    Public Property Name As String
        Get
            Return _name
        End Get
        Set(value As String)
            If _name.IsSameAs(value) Then Return
            _name = value
            PropertyHasChanged("Name")
        End Set
    End Property

    Public Property ParentCategory() As AccessoryCategory
        Get
            Return If(Parent Is Nothing, Nothing, DirectCast(Parent, AccessoryCategories).ParentCategory)
        End Get
        Set(value As AccessoryCategory)
            If ParentCategory IsNot Nothing AndAlso ParentCategory.ID.Equals(value.ID) Then Return

            MoveTo(value)

            PropertyHasChanged("ParentCategory")
            ValidationRules.CheckRules("Name")
        End Set
    End Property

    Private Sub MoveTo(ByVal newParent As AccessoryCategory)
        ParentCategory.Categories.Remove(Me) 'remove me from my current parent's subcategories
        newParent.Categories.Add(Me) 'add me to my new parent's subcategories
        MarkUnDeleted() 'framework flag for deletion
    End Sub

    Public ReadOnly Property Categories() As AccessoryCategories
        Get
            _categories = If(_categories, AccessoryCategories.NewAccessoryCategories(Me))
            Return _categories
        End Get
    End Property

    Public ReadOnly Property Accessories() As Accessories
        Get
            _accessories = If(_accessories, Accessories.GetAccessoriesByAccessoryCategory(ID))
            Return _accessories
        End Get
    End Property

    Public Property MappedEquipmentGroup() As EquipmentGroupInfo
        Get
            Return _mappedEquipmentGroup
        End Get
        Set(value As EquipmentGroupInfo)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            If value.Equals(_mappedEquipmentGroup) Then Return
            _mappedEquipmentGroup = value
            PropertyHasChanged("MappedEquipmentGroup")
        End Set
    End Property

    Public Property MappedEquipmentCategory() As EquipmentCategoryInfo
        Get
            Return _mappedEquipmentCategory
        End Get
        Set(value As EquipmentCategoryInfo)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            If value.Equals(_mappedEquipmentCategory) Then Return
            _mappedEquipmentCategory = value
            PropertyHasChanged("MappedEquipmentCategory")
        End Set
    End Property

    Public Function GetInfo() As AccessoryCategoryInfo
        Return AccessoryCategoryInfo.GetAccessoryCategoryInfo(Me)
    End Function
#End Region

#Region "Business & Validation Rules"

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, RuleHandler),
                                New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Name", 255))
        ValidationRules.AddRule(DirectCast(AddressOf CategoryNameUniqueForParent, RuleHandler), "Name")
    End Sub

    Private Shared Function CategoryNameUniqueForParent(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim category = DirectCast(target, AccessoryCategory)

        'no parent category or no name set => do not validate this rule
        If category.ParentCategory Is Nothing OrElse category.Name = Nothing Then Return True

        e.Description = String.Format("The name of the category (""{0}"") should be unique within the subcategories of the parent category.", category.Name)

        Return category.ParentCategory.Categories.All(Function(siblingCategory) Not category.Name.Equals(siblingCategory.Name) OrElse
                                                                                    category.ID.Equals(siblingCategory.ID))
    End Function

#End Region

#Region "Shared Factory Methods"

    Friend Shared Function GetAccessoryCategory(ByVal dataReader As SafeDataReader) As AccessoryCategory
        Dim category = new AccessoryCategory()
        category.Fetch(dataReader)
        category.MarkAsChild()
        Return category
    End Function

    Public Shared Function GetAccessoryCategory(ByVal categoryId As Guid) As AccessoryCategory
        Return DataPortal.Fetch(Of AccessoryCategory)(New Criteria(categoryId))
    End Function

    Public Shared Function DummyRootCategory(ByVal rootCategories As AccessoryCategories) As AccessoryCategory
        Return New AccessoryCategory() With {
                                                .ID = Guid.Empty,
                                                ._categories = rootCategories,
                                                .AllowRemove = False,
                                                .AllowEdit = False
                                            }
    End Function

#End Region

#Region "System.Object overrides"

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is AccessoryCategory Then Return Equals(DirectCast(obj, AccessoryCategory))
        Return MyBase.Equals(obj)
    End Function

    Public Overloads Function Equals(ByVal otherCategory As AccessoryCategory) As Boolean
        Return ID.Equals(otherCategory.ID)
    End Function

#End Region

#Region "Framework Overrides"

    Public Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If _categories IsNot Nothing AndAlso Not _categories.IsValid Then Return False
            If _accessories IsNot Nothing AndAlso Not _accessories.IsValid Then Return False
            Return True
        End Get
    End Property

    Public Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If _categories IsNot Nothing AndAlso _categories.IsDirty Then Return True
            If _accessories IsNot Nothing AndAlso _accessories.IsDirty Then Return True
            Return False
        End Get
    End Property


#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region

#Region "Data Access"

    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _name = String.Empty 'make sure no null reference pops up when name never gets set and validation rules are checked
        _mappedEquipmentGroup = EquipmentGroupInfo.Empty
        _mappedEquipmentCategory = EquipmentCategoryInfo.Empty
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _name = .GetString("SHORTNAME")
        End With

        _mappedEquipmentGroup = EquipmentGroupInfo.GetEquipmentGroupInfo(dataReader)
        _mappedEquipmentCategory = EquipmentCategoryInfo.GetEquipmentCategoryInfo(dataReader)

        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        MyBase.AddDeleteCommandFields(command)
    End Sub

    Private Sub AddCommandFields(ByVal command As SqlCommand)
        With command.Parameters
            .AddWithValue("@SHORTNAME", Name)
            .AddWithValue("@PARENTCATEGORYID", If(ParentCategory Is Nothing, DBNull.Value, ParentCategory.ID.GetDbValue()))
            .AddWithValue("@MAPPEDEQUIPMENTGROUPID", If(MappedEquipmentGroup.IsEmpty(), DBNull.Value, MappedEquipmentGroup.ID.GetDbValue()))
            .AddWithValue("@MAPPEDEQUIPMENTCATEGORYID", If(MappedEquipmentCategory.IsEmpty(), DBNull.Value, MappedEquipmentCategory.ID.GetDbValue()))
        End With
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction, ByVal changesToUpdate As ChangesToUpdate)
        If _categories IsNot Nothing Then _categories.Update(transaction, changesToUpdate)
 If _accessories IsNot Nothing Then _accessories.Update(transaction)
    End Sub

#End Region
End Class

<Serializable> Public Class AccessoryCategoryInfo
#Region "Business Properties & Methods"

    Private _id As Guid = Guid.Empty
    Private _name As String = String.Empty

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID As Guid
        Get
            Return _id
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name As String
        Get
            Return _name
        End Get
    End Property

    Public Shared ReadOnly Property Empty() As AccessoryCategoryInfo
        Get
            Return New AccessoryCategoryInfo()
        End Get
    End Property

    Public Function IsEmpty() As Boolean
        Return Equals(Empty.ID)
    End Function

#End Region

#Region "Object overrides"

    Public Overloads Function Equals(ByVal category As AccessoryCategory) As Boolean
        Return category IsNot Nothing AndAlso Equals(category.ID)
    End Function

    Public Overloads Function Equals(ByVal info As AccessoryCategoryInfo) As Boolean
        Return info IsNot Nothing AndAlso Equals(info.ID)
    End Function

    Public Overloads Function Equals(ByVal otherId As Guid) As Boolean
        Return ID.Equals(otherId)
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is Guid Then Return Equals(DirectCast(obj, Guid))
        If TypeOf obj Is AccessoryCategoryInfo Then Return Equals(DirectCast(obj, AccessoryCategoryInfo))
        If TypeOf obj Is AccessoryCategory Then Return Equals(DirectCast(obj, AccessoryCategory))
        Return False
    End Function

#End Region

#Region "Shared Factory Methods"
    Friend Shared Function GetAccessoryCategoryInfo(ByVal category As AccessoryCategory) As AccessoryCategoryInfo
        If category Is Nothing Then Return Empty

        Dim info = New AccessoryCategoryInfo
        info.Fetch(category)
        Return info
    End Function

    Public Shared Function GetAccessoryCategoryInfo(ByVal dataReader As SafeDataReader) As AccessoryCategoryInfo
        If dataReader.GetGuid("ACCESSORYCATEGORYID").Equals(Guid.Empty) Then Return Empty

        Dim info = New AccessoryCategoryInfo
        info.Fetch(dataReader)
        Return info
    End Function

#End Region

#Region "Constructors"

    Private Sub New()
        'prevent direct creation
    End Sub

#End Region

#Region "Data Access"

    Private Sub Fetch(ByVal category As AccessoryCategory)
        _id = category.ID
        _name = category.Name
    End Sub

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        _id = dataReader.GetGuid("ACCESSORYCATEGORYID")
        _name = dataReader.GetString("ACCESSORYCATEGORYNAME")
    End Sub

#End Region
End Class