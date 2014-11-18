Imports System.ComponentModel
Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Exceptions
Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.Validation
Imports TME.CarConfigurator.Administration.Extensions.StringExtensions
Imports TME.CarConfigurator.Administration.Extensions.LegacyExtensions

<Serializable()> Public NotInheritable Class EquipmentItems
    Inherits ContextUniqueGuidListBase(Of EquipmentItems, EquipmentItem)

#Region " Business Properties & Methods "

    <NotUndoable()> Private _accessories As IList(Of Accessory)
    <NotUndoable()> Private _options As Options
    <NonSerialized()> Private _referenceMap As Dictionary(Of Guid, EquipmentItem)


    Friend Shadows ReadOnly Property Parent() As Object
        Get
            If MyBase.Parent Is Nothing Then Return Nothing
            Return MyBase.Parent
        End Get
    End Property

    Public ReadOnly Property Accessories() As IList(Of Accessory)
        Get
            _accessories = If(_accessories, OfType(Of Accessory)().OrderBy(Function(x) x.SortPath).ToList())
            Return _accessories
        End Get
    End Property
    Public ReadOnly Property Options() As Options
        Get
            _options = If(_options, Options.GetOptions(Me))
            Return _options
        End Get
    End Property

    Default Public Overloads Overrides ReadOnly Property Item(ByVal id As Guid) As EquipmentItem
        Get
            Return If(ReferenceMap.ContainsKey(id), ReferenceMap.Item(id), Nothing)
        End Get
    End Property

    Friend Overloads Sub Add(ByVal dataReader As Common.Database.SafeDataReader)
        RaiseListChangedEvents = False
        AllowNew = True
        MyBase.Add(GetObject(dataReader))
        AllowNew = False
        RaiseListChangedEvents = True
    End Sub

    Friend Overloads Sub Add(ByVal equipmentItem As EquipmentItem)
        AllowNew = True
        MyBase.Add(equipmentItem)
        AllowNew = False
    End Sub

    Public Overloads Function Add(ByVal equipmentType As EquipmentType) As EquipmentItem
        If equipmentType <> equipmentType.Option AndAlso equipmentType <> equipmentType.Accessory Then
            Throw New InvalidEquipmentType(
                String.Format("Only Options or Accessories can be added using this method, but it was called with {0}.", equipmentType.ToString()))
        End If

        Dim newItem As EquipmentItem = If(equipmentType = equipmentType.Option,
                                          DirectCast([Option].NewOption(), EquipmentItem),
                                          DirectCast(Accessory.NewAccessory(), EquipmentItem))

        Add(newItem)
        Return newItem
    End Function

    Private ReadOnly Property ReferenceMap() As Dictionary(Of Guid, EquipmentItem)
        Get
            If _referenceMap IsNot Nothing Then Return _referenceMap

            _referenceMap = New Dictionary(Of Guid, EquipmentItem)(Count)
            For Each _item In Me
                _referenceMap.Add(_item.ID, _item)
            Next

            Return _referenceMap
        End Get
    End Property
    Private Sub EquipmentItemsListChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType <> ListChangedType.ItemAdded Then Return

        Dim addedItem As EquipmentItem = Me(e.NewIndex)
        If Not ReferenceMap.ContainsKey(addedItem.ID) Then ReferenceMap.Add(addedItem.ID, addedItem)
        If addedItem.IsNew Then addedItem.CheckRules()
    End Sub
    Private Sub EquipmentItemsRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        ReferenceMap.Remove(DirectCast(e.RemovingItem, EquipmentItem).ID)
    End Sub

    Friend Sub PrepareForEagerLoadOfApplicabilities()
        For Each equipmentItem In Me
            equipmentItem.PrepareForEagerLoadOfApplicabilities()
        Next
    End Sub

#End Region

#Region " Framework Overrides "
    Public Shadows Function IsChild() As Boolean
        Return MyBase.IsChild
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewEquipmentItems(ByVal group As EquipmentGroup) As EquipmentItems
        Dim list As EquipmentItems = New EquipmentItems()
        list.SetParent(group)
        list.MarkAsChild()
        Return list
    End Function
    Friend Shared Function NewEquipmentItems(ByVal category As EquipmentCategory) As EquipmentItems
        Dim list As EquipmentItems = New EquipmentItems()
        list.SetParent(category)
        list.MarkAsChild()
        Return list
    End Function


    Public Shared Function GetMasterEquipmentItems(ByVal type As EquipmentType) As EquipmentItems
        Return DataPortal.Fetch(Of EquipmentItems)(New CustomCriteria(True, type))
    End Function
    Public Shared Function GetEquipmentItems() As EquipmentItems
        Return DataPortal.Fetch(Of EquipmentItems)(New CustomCriteria)
    End Function
    Public Shared Function GetEquipmentItems(ByVal type As EquipmentType) As EquipmentItems
        Return DataPortal.Fetch(Of EquipmentItems)(New CustomCriteria(type))
    End Function
    Friend Shared Function GetEquipmentItems(ByVal group As EquipmentGroup) As EquipmentItems
        Dim list As EquipmentItems = DataPortal.Fetch(Of EquipmentItems)(New CustomCriteria(group))
        list.SetParent(group)
        list.MarkAsChild()
        Return list
    End Function
    Friend Shared Function GetEquipmentItems(ByVal category As EquipmentCategory) As EquipmentItems
        Dim list As EquipmentItems = DataPortal.Fetch(Of EquipmentItems)(New CustomCriteria(category))
        list.SetParent(category)
        list.MarkAsChild()
        Return list
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
        AllowNew = False
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private _groupID As Guid = Guid.Empty
        Private _categoryID As Guid = Guid.Empty
        Private ReadOnly _typeCode As String
        Private ReadOnly _master As Boolean = False

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@EQUIPMENTGROUPID", _groupID.GetDbValue())
            command.Parameters.AddWithValue("@EQUIPMENTCATEGORYID", _categoryID.GetDbValue())
            command.Parameters.AddWithValue("@EQUIPMENTTYPE", _typeCode)
            command.Parameters.AddWithValue("@ONLYMASTERLIST", _master)
        End Sub

        'Add Data Portal Fetch criteria here
        Public Sub New()
        End Sub
        Public Sub New(ByVal group As EquipmentGroup)
            _groupID = group.ID
        End Sub
        Public Sub New(ByVal category As EquipmentCategory)
            _categoryID = category.ID
        End Sub
        Public Sub New(ByVal type As EquipmentType)
            Select Case type
                Case EquipmentType.Accessory
                    _typeCode = Environment.DBAccessoryCode
                Case EquipmentType.Option
                    _typeCode = Environment.DBOptionCode
            End Select
        End Sub
        Public Sub New(ByVal masterList As Boolean, ByVal type As EquipmentType)
            Me.New(type)
            _master = masterList
        End Sub

    End Class

#End Region

#Region " Data Access "

    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property
    Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As EquipmentItem
        Dim type As String = dataReader.GetString("TYPE")
        Select Case type
            Case Environment.DBAccessoryCode
                Return Accessory.GetAccessory(dataReader)
            Case Environment.DBOptionCode
                Return [Option].GetOption(dataReader)
            Case Else
                Throw New InvalidEquipmentType("""" & type & """ is not a valid equipment type!")
        End Select
    End Function

    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        PrepareForEagerLoadOfApplicabilities()

        FetchFittings(dataReader)

        dataReader.NextResult()
        FetchIncompatibilities(dataReader)
    End Sub


    Private Sub FetchFittings(ByVal dataReader As SafeDataReader)
        ' used to keep track of previous item => if the previous item was the same, we don't need to find it again, but can just reuse it
        Dim itemID As Guid = Guid.Empty
        Dim equipmentItem As EquipmentItem = Nothing

        While dataReader.Read
            'get the correct item (or keep the current one if it is the same)
            Dim databaseItemID = dataReader.GetGuid("EQUIPMENTID")
            If Not itemID.Equals(databaseItemID) Then
                itemID = databaseItemID
                equipmentItem = Item(itemID)
            End If

            'if the equipmentitem doesn't exist => skip
            If equipmentItem Is Nothing Then Continue While

            ' if the fitting is a suffix fitting, fetch the suffix fitting, otherwise, fetch the equipment fitting
            Dim isSuffixFitting = dataReader.GetBoolean("ISSUFFIXFITTING")
            If isSuffixFitting Then
                DirectCast(equipmentItem, [Option]).FetchSuffixFitting(dataReader)
                Continue While
            End If

            equipmentItem.FetchFitting(dataReader)
        End While
    End Sub
    Private Sub FetchIncompatibilities(ByVal dataReader As SafeDataReader)
        Dim itemID As Guid = Guid.Empty
        Dim equipmentItem As EquipmentItem = Nothing

        While dataReader.Read
            If Not itemID.Equals(dataReader.GetGuid("EQUIPMENTID")) Then
                itemID = dataReader.GetGuid("EQUIPMENTID")
                equipmentItem = Item(itemID)
            End If
            If Not equipmentItem Is Nothing Then
                equipmentItem.FetchIncompatibility(dataReader)
            End If
        End While
    End Sub

#End Region

End Class
<Serializable()> Public MustInherit Class EquipmentItem
    Inherits LocalizeableBusinessBase
    Implements ILinkedAssets
    Implements ILinks
    Implements IOwnedBy
    Implements IMasterObject
    Implements IEquipmentBestVisibleIn

#Region " Business Properties & Methods "

    Private _partNumber As String = String.Empty
    Private _name As String = String.Empty
    Private _description As String = String.Empty
    Private _type As EquipmentType
    Private _owner As String = String.Empty
    Private WithEvents _markets As Markets
    Private _status As Integer
    Private _colour As ExteriorColourInfo

    Private _masterId As Guid
    Private _masterDescription As String
    Private _masterType As MasterEquipmentType

    Private _bestVisibleInMode As String
    Private _bestVisibleInView As String
    Private _bestVisibleInAngle As Integer

    Private _group As EquipmentGroupInfo
    Private _category As EquipmentCategoryInfo
    Private _assets As LinkedAssets
    Private _rules As EquipmentRules
    Private _links As Links


    Protected BaseFittings As EquipmentFittings
    Protected BaseIncompatibilities As EquipmentIncompatibilities

    Public Property PartNumber() As String
        Get
            Return _partNumber
        End Get
        Set(ByVal value As String)
            If _partNumber.IsSameAs(value) Then Return

            _partNumber = value
            PropertyHasChanged("PartNumber")
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name.IsSameAs(value) Then Return

            _name = value
            PropertyHasChanged("Name")
        End Set
    End Property
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            If _description.IsSameAs(value) Then Return

            _description = value
            PropertyHasChanged("Description")
        End Set
    End Property


    Public Property Type() As EquipmentType
        Get
            Return _type
        End Get
        Protected Set(ByVal value As EquipmentType)
            _type = value
        End Set
    End Property

    Public ReadOnly Property TypeName() As String
        Get
            Return If(Type = EquipmentType.Accessory OrElse Type = EquipmentType.Option, Type.ToString(), "(undefined)")
        End Get
    End Property

    Public ReadOnly Property IOwnedBy_Owner() As String Implements IOwnedBy.Owner
        Get
            Return Owner
        End Get
    End Property

    Public Property Owner() As String
        Get
            Return _owner
        End Get
        Set(ByVal value As String)
            If Not _owner.Equals(value, StringComparison.InvariantCultureIgnoreCase) Then
                _owner = value
                PropertyHasChanged("Owner")
            End If
        End Set
    End Property

    Public ReadOnly Property Markets As Markets
        Get
            If _markets IsNot Nothing Then Return _markets
            _markets = If(IsNew, Markets.NewMarkets(Me), Markets.GetMarkets(Me))
            Return _markets
        End Get
    End Property

    Public Property Activated() As Boolean
        Get
            Return (_status And Status.AvailableToNmscs) = Status.AvailableToNmscs
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Activated) Then Return

            If value Then
                _status += Status.AvailableToNmscs
            Else
                _status -= Status.AvailableToNmscs
            End If
            PropertyHasChanged("Activated")
        End Set
    End Property
    Public Overridable Property Approved() As Boolean
        Get
            Return (_status And Status.ApprovedForLive) = Status.ApprovedForLive
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Approved) Then Return

            If value Then
                _status += Status.ApprovedForLive
            Else
                _status -= Status.ApprovedForLive
            End If
            Declined = Not (value)
            PropertyHasChanged("Approved")
        End Set
    End Property
    Public Property Declined() As Boolean
        Get
            Return (_status And Status.Declined) = Status.Declined
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Declined) Then Return

            If value Then
                _status += Status.Declined
            Else
                _status -= Status.Declined
            End If
            Approved = Not (value)
            PropertyHasChanged("Declined")
        End Set
    End Property
    Public Property Colour() As ExteriorColourInfo
        Get
            Return _colour
        End Get
        Set(ByVal value As ExteriorColourInfo)
            If value Is Nothing Then value = ExteriorColourInfo.Empty
            If _colour.Equals(value) Then Return

            _colour = value
            PropertyHasChanged("Colour")
        End Set
    End Property


    Public Property MasterID() As Guid Implements IMasterObject.MasterID
        Get
            Return _masterId
        End Get
        Set(ByVal value As Guid)
            If _masterId.Equals(value) Then Return

            _masterId = value
            If _masterId.Equals(Guid.Empty) Then _masterDescription = String.Empty
            PropertyHasChanged("Master")
        End Set
    End Property
    Public Property MasterDescription() As String Implements IMasterObject.MasterDescription
        Get
            Return _masterDescription
        End Get
        Set(ByVal value As String)
            If _masterDescription.Equals(value) Then Return

            _masterDescription = value
            PropertyHasChanged("Master")
        End Set
    End Property

    Private ReadOnly Property RefMasterID() As Guid Implements IMasterObjectReference.MasterID
        Get
            Return MasterID
        End Get
    End Property

    Private ReadOnly Property RefMasterDescription() As String Implements IMasterObjectReference.MasterDescription
        Get
            Return MasterDescription
        End Get
    End Property

    Public Property MasterType() As MasterEquipmentType
        Get
            Return _masterType
        End Get
        Set(ByVal value As MasterEquipmentType)
            If _masterType.Equals(value) Then Return

            _masterType = value
            PropertyHasChanged("Master")
        End Set
    End Property
    Public Function IsMasterObject() As Boolean
        Return Not MasterType = MasterEquipmentType.None
    End Function

    Friend Overloads ReadOnly Property GroupOrCategory() As Object
        Get
            If Parent Is Nothing Then Return Nothing
            Return Parent.Parent
        End Get
    End Property

    Friend Overloads ReadOnly Property Parent() As EquipmentItems
        Get
            If TypeOf MyBase.Parent Is Accessories Then Return Nothing
            Return DirectCast(MyBase.Parent, EquipmentItems)
        End Get
    End Property

    Public Overridable Property Group() As EquipmentGroupInfo
        Get
            If _group.IsEmpty() AndAlso TypeOf GroupOrCategory Is EquipmentGroup Then
                _group = EquipmentGroupInfo.GetEquipmentGroupInfo(DirectCast(GroupOrCategory, EquipmentGroup))
            End If
            Return _group
        End Get
        Set(ByVal value As EquipmentGroupInfo)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            If Group.Equals(value) Then Return

            MoveToAnotherParentGroupIfNeeded(value)
            _group = value
            PropertyHasChanged("Group")
        End Set
    End Property

    Public Property BestVisibleInMode() As String Implements IEquipmentBestVisibleIn.BestVisibleInMode
        Get
            Return _bestVisibleInMode
        End Get
        Set(ByVal value As String)

            If _bestVisibleInMode.Equals(value) Then Return
            _bestVisibleInMode = value
            PropertyHasChanged("BestVisibleInMode")
            ValidationRules.CheckRules("BestVisibleIn")
        End Set
    End Property

    Public Property BestVisibleInView() As String Implements IEquipmentBestVisibleIn.BestVisibleInView
        Get
            Return _bestVisibleInView
        End Get
        Set(ByVal value As String)
            If _bestVisibleInView.Equals(value) Then Return
            _bestVisibleInView = value
            PropertyHasChanged("BestVisibleInView")
            ValidationRules.CheckRules("BestVisibleIn")
        End Set
    End Property

    Public Property BestVisibleInAngle() As Integer Implements IEquipmentBestVisibleIn.BestVisibleInAngle
        Get
            Return _bestVisibleInAngle
        End Get
        Set(ByVal value As Integer)
            If _bestVisibleInAngle.Equals(value) Then Return
            _bestVisibleInAngle = value
            PropertyHasChanged("BestVisibleInAngle")
            ValidationRules.CheckRules("BestVisibleIn")
        End Set
    End Property

    Private Sub MoveToAnotherParentGroupIfNeeded(ByVal value As EquipmentGroupInfo)
        Dim parentGroup = TryCast(GroupOrCategory, EquipmentGroup)
        If (parentGroup IsNot Nothing) Then
            parentGroup.MoveEquipmentItemToAnotherGroup(Me, value)
            MarkUnDeleted()
        End If
    End Sub

    Public Overridable Property Category() As EquipmentCategoryInfo
        Get
            If _category.IsEmpty() AndAlso TypeOf GroupOrCategory Is EquipmentCategory Then
                _category = EquipmentCategoryInfo.GetEquipmentCategoryInfo(DirectCast(GroupOrCategory, EquipmentCategory))
            End If
            Return _category
        End Get
        Set(ByVal value As EquipmentCategoryInfo)
            If value Is Nothing Then Throw New ArgumentNullException("value")
            If Category.Equals(value) Then Return

            MoveToAnotherParentCategoryIfNeeded(value)
            _category = value
            PropertyHasChanged("Category")
        End Set
    End Property
    Private Sub MoveToAnotherParentCategoryIfNeeded(ByVal value As EquipmentCategoryInfo)
        Dim parentCategory = TryCast(GroupOrCategory, EquipmentCategory)
        If (parentCategory IsNot Nothing) Then
            parentCategory.MoveEquipmentItemToAnotherCategory(Me, value)
            MarkUnDeleted()
        End If
    End Sub
    Public ReadOnly Property Assets() As LinkedAssets Implements ILinkedAssets.Assets
        Get
            If _assets Is Nothing Then
                _assets = If(IsNew, LinkedAssets.NewLinkedAssets(ID), LinkedAssets.GetLinkedAssets(ID))
            End If
            Return _assets
        End Get
    End Property
    Public ReadOnly Property Links() As Links Implements ILinks.Links
        Get
            If _links Is Nothing Then
                _links = Links.GetLinks(Me)
            End If
            Return _links
        End Get
    End Property
    Friend Sub PrepareForEagerLoadOfApplicabilities()
        BaseFittings = EquipmentFittings.NewEquipmentFittings(Me)
        BaseIncompatibilities = EquipmentIncompatibilities.NewEquipmentIncompatibilities(Me)
    End Sub
    Public ReadOnly Property Fittings() As EquipmentFittings
        Get
            If BaseFittings IsNot Nothing Then Return BaseFittings
            BaseFittings = If(IsNew, EquipmentFittings.NewEquipmentFittings(Me), EquipmentFittings.GetEquipmentFittings(Me))
            Return BaseFittings
        End Get
    End Property
    Public ReadOnly Property Incompatibilities() As EquipmentIncompatibilities
        Get
            If BaseIncompatibilities IsNot Nothing Then Return BaseIncompatibilities
            BaseIncompatibilities = If(IsNew, EquipmentIncompatibilities.NewEquipmentIncompatibilities(Me), EquipmentIncompatibilities.GetEquipmentIncompatibilities(Me))
            Return BaseIncompatibilities
        End Get
    End Property

    Friend ReadOnly Property RulesFetched() As Boolean
        Get
            Return _rules IsNot Nothing
        End Get
    End Property
    Public ReadOnly Property Rules() As EquipmentRules
        Get
            If RulesFetched Then Return _rules

            _rules = If(IsNew, EquipmentRules.NewEquipmentRules(Me), EquipmentRules.GetEquipmentRules(Me))
            Return _rules
        End Get
    End Property

    Public Function CanBeFittedOn(ByVal car As Car) As Boolean
        Return CanBeFittedOn(car.FactoryCarSpecification)
    End Function
    Public Overridable Function CanBeFittedOn(ByVal partialConfiguration As PartialCarSpecification) As Boolean
        Return Not Incompatibilities.ContainsMatch(partialConfiguration) AndAlso Fittings.ContainsMatch(partialConfiguration)
    End Function

    Public Function GetInfo() As EquipmentItemInfo
        Return EquipmentItemInfo.GetEquipmentInfo(Me)
    End Function

    Public Overloads Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If Type = EquipmentType.Option AndAlso Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then Return True

        Select Case propertyName.ToLowerInvariant()
            Case "approved", "declined"
                Return True
            Case Else
                Return MyBase.CanWriteProperty(propertyName)
        End Select
    End Function

    Public Overridable ReadOnly Property SortPath() As String
        Get
            Return AlternateName
        End Get
    End Property
#End Region

#Region "Events"
    Private Sub MarketsChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs) Handles _markets.ListChanged
        If e.ListChangedType <> ListChangedType.ItemAdded AndAlso e.ListChangedType <> ListChangedType.ItemDeleted Then Return

        ValidationRules.CheckRules("Owner") 'when markets list changed, we should check whether or not the owner is still in the list
    End Sub
#End Region

#Region " Business & Validation Rules "
    Friend Sub CheckRules()
        ValidationRules.CheckRules()
    End Sub
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Administration.ValidationRules.MasterReference.Valid, RuleHandler), "Master")
        ValidationRules.AddRule(DirectCast(AddressOf MasterValid, RuleHandler), "Master")

        ValidationRules.AddRule(DirectCast(AddressOf ModeAndViewCombinationValid, RuleHandler), "BestVisibleIn")
        ValidationRules.AddRule(DirectCast(AddressOf AngleShouldBeInLimits, RuleHandler), "BestVisibleIn")
    End Sub

    Private Shared Function MasterValid(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, EquipmentItem)

        If obj.MasterID.Equals(Guid.Empty) Then Return True

        e.Description = "The type of the master is required!"

        Return obj.MasterType <> MasterEquipmentType.None
    End Function

    Private Shared Function ModeAndViewCombinationValid(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim obj = DirectCast(target, [EquipmentItem])
        If String.IsNullOrEmpty(obj.BestVisibleInMode) AndAlso String.IsNullOrEmpty(obj.BestVisibleInView) Then Return True
        If String.IsNullOrEmpty(obj.BestVisibleInMode) OrElse String.IsNullOrEmpty(obj.BestVisibleInView) Then
            e.Description = "Both mode and view should be filled in."
            Return False
        End If

        If Not MyContext.GetContext().AssetTypeGroups.Any(Function(x) x.Mode = obj.BestVisibleInMode AndAlso x.View = obj.BestVisibleInView) Then
            e.Description = String.Format("The mode/view combination {0}/{1} is not valid", obj.BestVisibleInMode, obj.BestVisibleInView)
            Return False
        End If
        Return True
    End Function

    Private Shared Function AngleShouldBeInLimits(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        If Not ModeAndViewCombinationValid(target, e) Then Return True

        Dim obj = DirectCast(target, [EquipmentItem])

        If String.IsNullOrEmpty(obj.BestVisibleInMode) AndAlso String.IsNullOrEmpty(obj.BestVisibleInView) Then Return True

        Dim assetTypeGroup = MyContext.GetContext().AssetTypeGroups.First(Function(x) x.Mode = obj.BestVisibleInMode AndAlso x.View = obj.BestVisibleInView)

        If (obj.BestVisibleInAngle < assetTypeGroup.MinimumAngle OrElse obj.BestVisibleInAngle > assetTypeGroup.MaximumAngle) Then
            e.Description = String.Format("Angle should be in the limits ({0},{1})", assetTypeGroup.MinimumAngle, assetTypeGroup.MaximumAngle)
            Return False
        End If

        Return True
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assets Is Nothing) AndAlso Not _assets.IsValid Then Return False
            If Not (_rules Is Nothing) AndAlso Not _rules.IsValid Then Return False
            If Not (_links Is Nothing) AndAlso Not _links.IsValid Then Return False
            If Not (BaseFittings Is Nothing) AndAlso Not BaseFittings.IsValid Then Return False
            If Not (BaseIncompatibilities Is Nothing) AndAlso Not BaseIncompatibilities.IsValid Then Return False
            If _markets IsNot Nothing AndAlso Not _markets.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assets Is Nothing) AndAlso _assets.IsDirty Then Return True
            If Not (_rules Is Nothing) AndAlso _rules.IsDirty Then Return True
            If Not (_links Is Nothing) AndAlso _links.IsDirty Then Return True
            If Not (BaseFittings Is Nothing) AndAlso BaseFittings.IsDirty Then Return True
            If Not (BaseIncompatibilities Is Nothing) AndAlso BaseIncompatibilities.IsDirty Then Return True
            If _markets IsNot Nothing AndAlso _markets.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Owner = Environment.GlobalCountryCode Then Return Name

        Return String.Format("{0} [{1}]", Name, Owner)
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        If obj.Equals(String.Empty) Then Return False

        If TypeOf Me Is [Option] Then
            Dim buffer As String = ";" + LocalCode.ToLower() + ";"
            If buffer.IndexOf(";" & obj.ToLower & ";", StringComparison.Ordinal) > -1 Then
                Return True
            Else
                buffer = ";" + DirectCast(Me, [Option]).Code.ToLower() + ";"
                If buffer.IndexOf(";" & obj.ToLower & ";", StringComparison.Ordinal) > -1 Then
                    Return True
                Else
                    Return False
                End If
            End If
        Else
            Return String.Compare(obj, PartNumber, True) = 0
        End If
    End Function

    Public Overloads Function Equals(ByVal code As String, ByVal isLocalCode As Boolean) As Boolean
        If isLocalCode Then
            Dim buffer As String = ";" + LocalCode.ToLower() + ";"
            If buffer.IndexOf(";" & code.ToLower & ";", StringComparison.Ordinal) > -1 Then
                Return True
            Else
                Return False
            End If
        Else
            Dim sBuffer As String = ";" + DirectCast(Me, [Option]).Code.ToLower() + ";"
            If sBuffer.IndexOf(";" & code.ToLower & ";", StringComparison.Ordinal) > -1 Then
                Return True
            Else
                Return False
            End If
        End If
    End Function

#End Region

#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation
        CanHaveLocalCode = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _owner = MyContext.GetContext.CountryCode
        _masterId = Guid.Empty
        _masterDescription = String.Empty
        _masterType = MasterEquipmentType.None

        _bestVisibleInMode = String.Empty
        _bestVisibleInView = String.Empty
        _bestVisibleInAngle = 0

        _group = EquipmentGroupInfo.Empty
        _category = EquipmentCategoryInfo.Empty
        _colour = ExteriorColourInfo.Empty
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _partNumber = .GetString("PARTNUMBER")
            _name = .GetString("SHORTNAME")
            _description = .GetString("DESCRIPTION")
            _owner = .GetString("OWNER")
            _status = .GetInt16("STATUSID")
            _masterId = .GetGuid("MASTERID")
            _masterDescription = .GetString("MASTERDESCRIPTION")
            _masterType = DirectCast(.GetInt16(GetFieldName("MASTERTYPE")), MasterEquipmentType)
            _bestVisibleInView = dataReader.GetString("BESTVISIBLEINVIEW")
            _bestVisibleInMode = dataReader.GetString("BESTVISIBLEINMODE")
            _bestVisibleInAngle = .GetInt16("BESTVISIBLEINANGLE")

            _group = EquipmentGroupInfo.GetEquipmentGroupInfo(dataReader)
            _category = EquipmentCategoryInfo.GetEquipmentCategoryInfo(dataReader)
            _colour = ExteriorColourInfo.GetExteriorColourInfo(dataReader)
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Friend Sub FetchFitting(ByVal dataReader As SafeDataReader)
        Fittings.FetchRow(dataReader)
    End Sub
    Friend Sub FetchIncompatibility(ByVal dataReader As SafeDataReader)
        Incompatibilities.FetchRow(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overridable Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command
            .Parameters.AddWithValue("@OWNER", Owner)
            .Parameters.AddWithValue("@PARTNUMBER", PartNumber)
            .Parameters.AddWithValue("@SHORTNAME", Name)
            .Parameters.AddWithValue("@DESCRIPTION", Description)
            .Parameters.AddWithValue("@STATUSID", _status)
            .Parameters.AddWithValue("@EXTERIORCOLOURID", If(Colour.IsEmpty(), DBNull.Value, DirectCast(Colour.ID, Object)))
            .Parameters.AddWithValue("@EQUIPMENTGROUPID", If(Group.IsEmpty(), DBNull.Value, DirectCast(Group.ID, Object)))
            .Parameters.AddWithValue("@EQUIPMENTCATEGORYID", If(Category.IsEmpty(), DBNull.Value, DirectCast(Category.ID, Object)))

            If MasterID.Equals(Guid.Empty) Then
                command.Parameters.AddWithValue("@MASTERID", DBNull.Value)
                command.Parameters.AddWithValue("@MASTERDESCRIPTION", DBNull.Value)
                command.Parameters.AddWithValue("@MASTERTYPE", DBNull.Value)
            Else
                command.Parameters.AddWithValue("@MASTERID", MasterID)
                command.Parameters.AddWithValue("@MASTERDESCRIPTION", MasterDescription)
                command.Parameters.AddWithValue("@MASTERTYPE", MasterType)
            End If

            .Parameters.AddWithValue("@BESTVISIBLEINMODE", BestVisibleInMode)
            .Parameters.AddWithValue("@BESTVISIBLEINVIEW", BestVisibleInView)
            .Parameters.AddWithValue("@BESTVISIBLEINANGLE", BestVisibleInAngle)

        End With
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _assets Is Nothing Then _assets.Update(transaction)
        If Not _rules Is Nothing Then _rules.Update(transaction)
        If Not _links Is Nothing Then _links.Update(transaction)
        If Not BaseFittings Is Nothing Then BaseFittings.Update(transaction)
        If Not BaseIncompatibilities Is Nothing Then BaseIncompatibilities.Update(transaction)
        If _markets IsNot Nothing Then _markets.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

#Region " Base Object Overrides"
    Protected Friend Overrides Function GetBaseCode() As String
        Return PartNumber
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Select Case Type
                Case EquipmentType.Accessory
                    Return Entity.ACCESSORY
                Case EquipmentType.Option
                    Return Entity.EQUIPMENT
                Case EquipmentType.ExteriorColourType
                    Return Entity.EXTERIORCOLOURTYPE
                Case EquipmentType.UpholsteryType
                    Return Entity.UPHOLSTERYTYPE
                Case Else
                    Throw New ArgumentException(String.Format("No case clause defined for {0}", Type.ToString()))
            End Select
        End Get
    End Property


#End Region
End Class



<Serializable()> Public NotInheritable Class EquipmentItemInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _partnumber As String = String.Empty
    Private _name As String = String.Empty
    Private _type As EquipmentType = EquipmentType.Empty

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Partnumber() As String
        Get
            Return _partnumber
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Type() As EquipmentType
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

    Public Overloads Function Equals(ByVal obj As EquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EquipmentItemInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return String.Compare(Partnumber, obj, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is EquipmentItemInfo Then
            Return Equals(DirectCast(obj, EquipmentItemInfo))
        ElseIf TypeOf obj Is EquipmentItem Then
            Return Equals(DirectCast(obj, EquipmentItem))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is EquipmentItemInfo Then
            Return DirectCast(objA, EquipmentItemInfo).Equals(objB)
        ElseIf TypeOf objB Is EquipmentItemInfo Then
            Return DirectCast(objB, EquipmentItemInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEquipmentInfo(ByVal dataReader As SafeDataReader) As EquipmentItemInfo
        Dim info As EquipmentItemInfo = New EquipmentItemInfo
        info.Fetch(dataReader)
        Return info
    End Function
    Friend Shared Function GetEquipmentInfo(ByVal item As EquipmentItem) As EquipmentItemInfo
        Dim info As EquipmentItemInfo = New EquipmentItemInfo
        info.Fetch(item)
        Return info
    End Function
    Friend Shared Function GetEquipmentInfo(ByVal item As ModelGenerationEquipmentItem) As EquipmentItemInfo
        Dim info As EquipmentItemInfo = New EquipmentItemInfo
        info.Fetch(item)
        Return info
    End Function

    Public Shared ReadOnly Property Empty() As EquipmentItemInfo
        Get
            Return New EquipmentItemInfo
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
            _id = .GetGuid("EQUIPMENTID")
            _partnumber = .GetString("EQUIPMENTPARTNUMBER")
            _name = .GetString("EQUIPMENTNAME")
            _type = .GetString("EQUIPMENTTYPE").GetEquipmentType()
        End With
    End Sub
    Private Sub Fetch(ByVal item As EquipmentItem)
        With item
            _id = .ID
            _partnumber = .PartNumber
            _name = .Name
            _type = .Type
        End With
    End Sub
    Private Sub Fetch(ByVal item As ModelGenerationEquipmentItem)
        With item
            _id = .ID
            _partnumber = .PartNumber
            _name = .Name
            _type = .Type
        End With
    End Sub
#End Region

End Class
