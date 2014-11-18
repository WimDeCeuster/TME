Imports System.ComponentModel
Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Exceptions
Imports TME.BusinessObjects.Core
Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.Translations
Imports TME.BusinessObjects.Validation
Imports TME.CarConfigurator.Administration.Extensions

<Serializable()> Public NotInheritable Class ModelGenerationPackItems
    Inherits StronglySortedListBase(Of ModelGenerationPackItems, ModelGenerationPackItem)

#Region " Delegates & Events "
    Friend Delegate Sub PackItemsChangedHandler(ByVal item As ModelGenerationPackItem)
    Friend Event PackItemAdded As PackItemsChangedHandler
    Friend Event PackItemRemoved As PackItemsChangedHandler
    Friend Event PackItemColouringModeChanged As PackItemsChangedHandler

    Protected Overrides Sub OnListChanged(ByVal e As ListChangedEventArgs)
        MyBase.OnListChanged(e)
        If e.ListChangedType = ListChangedType.ItemAdded Then
            RaiseEvent PackItemAdded(Me(e.NewIndex))
        End If
    End Sub
    Private Overloads Sub OnRemovingItem(ByVal sender As Object, ByVal e As RemovingItemEventArgs) Handles Me.RemovingItem
        RaiseEvent PackItemRemoved(DirectCast(e.RemovingItem, ModelGenerationPackItem))
    End Sub
    Public Sub RaisePackItemColouringModeChangedEvent(ByVal modelGenerationPackItem As ModelGenerationPackItem)
        RaiseEvent PackItemColouringModeChanged(modelGenerationPackItem)
    End Sub
#End Region

#Region " Business Properties & Methods "

    Friend Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPack)
        End Get
        Private Set(ByVal value As ModelGenerationPack)
            SetParent(value)
        End Set
    End Property

    Public Shadows Function Add(ByVal equipmentItem As ModelGenerationEquipmentItem) As ModelGenerationPackItem
        If Contains(equipmentItem.ID) Then Throw New ObjectAlreadyExists(String.Format("The item '{0}' is already a part of the pack '{1}'", equipmentItem.Name, Pack.Name))

        If Not equipmentItem.Type = EquipmentType.Option Then
            Dim packItem = ModelGenerationPackItem.NewModelGenerationPackItem(equipmentItem)
            MyBase.Add(packItem)
            Return packItem
        End If

        Dim generationOption = DirectCast(equipmentItem, ModelGenerationOption)
        If generationOption.HasParentOption Then Throw New ArgumentException(String.Format("The item '{0}' is a child option and can not be added directly to the pack '{1}'", equipmentItem.Name, Pack.Name))

        Dim packOption = AddTree(generationOption)
        Return packOption

    End Function

    Private Function AddTree(ByVal generationOption As ModelGenerationOption) As ModelGenerationPackItem
        Dim packItem = Item(generationOption.ID)
        If packItem Is Nothing Then
            packItem = ModelGenerationPackItem.NewModelGenerationPackItem(generationOption)
            MyBase.Add(packItem)
        End If

        For Each child In generationOption.ChildOptions
            AddTree(child)
        Next

        Return packItem
    End Function

    Public Shadows Sub Remove(ByVal packItem As ModelGenerationPackItem)
        If Not Contains(packItem.ID) Then Return

        If Not TypeOf packItem Is ModelGenerationPackOption Then
            MyBase.Remove(packItem)
            Return

        End If

        Dim packOption = DirectCast(packItem, ModelGenerationPackOption)
        Dim rootOption = packOption.GetRootOption()

        RemoveTree(rootOption)
    End Sub
    Private Sub RemoveTree(ByVal packOption As ModelGenerationPackOption)
        For Each childOption In packOption.ChildOptions
            RemoveTree(childOption)
        Next

        If Not Contains(packOption.ID) Then Return

        MakeSureNoGradeOptionsAreOfferedThroughThisPackAnymore(packOption)

        MyBase.Remove(packOption)
    End Sub

    Private Sub MakeSureNoGradeOptionsAreOfferedThroughThisPackAnymore(ByVal modelGenerationPackOption As ModelGenerationPackOption)
        Dim generation = Pack.Generation
        Dim generationOption = DirectCast(generation.Equipment(modelGenerationPackOption.ID), ModelGenerationOption)

        If Not generationOption.SuffixOption Then Return

        For Each grade In generation.Grades
            Dim gradeOption = DirectCast(grade.Equipment(modelGenerationPackOption.ID), ModelGenerationGradeOption)
            If gradeOption Is Nothing Then Continue For

            If not gradeOption.IsOfferedThrough(Pack) Then continue for

            gradeOption.OfferedThrough.Remove(Pack)
        Next
    End Sub

    Public Overrides Function CanMoveDown(ByVal packItem As ModelGenerationPackItem) As Boolean
        If Not TypeOf packItem Is ModelGenerationPackOption Then Return MyBase.CanMoveDown(packItem)
        If (packItem.Index + 1) = Count Then Return False

        Return ThereIsAnItemBelowMeWithTheSameParent(packItem)
    End Function
    Private Function ThereIsAnItemBelowMeWithTheSameParent(ByVal packItem As ModelGenerationPackItem) As Boolean
        Dim parentItem = GetParentItem(packItem)
        Dim indexBelow = packItem.Index + 1
        Do While indexBelow < Count
            Dim itemBelowMe = Me(indexBelow)
            Dim parentItemBelowMe = GetParentItem(itemBelowMe)
            If ModelGenerationPackItem.AreEqual(parentItem, parentItemBelowMe) Then Return True
            indexBelow += 1
        Loop
        Return False
    End Function


    Public Overrides Function CanMoveUp(ByVal packItem As ModelGenerationPackItem) As Boolean
        If Not TypeOf packItem Is ModelGenerationPackOption Then Return MyBase.CanMoveUp(packItem)
        If packItem.Index = 0 Then Return False

        Return ThereIsAnItemAboveMeWithTheSameParent(packItem)
    End Function
    Private Function ThereIsAnItemAboveMeWithTheSameParent(ByVal packItem As ModelGenerationPackItem) As Boolean
        Dim parentItem = GetParentItem(packItem)
        Dim indexAbove = packItem.Index - 1
        Do While indexAbove >= 0
            Dim itemAboveMe = Me(indexAbove)
            Dim parentItemAboveMe = GetParentItem(itemAboveMe)
            If ModelGenerationPackItem.AreEqual(parentItem, parentItemAboveMe) Then Return True
            indexAbove -= 1
        Loop
        Return False
    End Function





    Public Overrides Function MoveUp(ByVal packItem As ModelGenerationPackItem) As Boolean
        If Not CanMoveUp(packItem) Then Return False


        Dim targetLocation = FirstItemAboveMeThatHasSameParent(packItem)
        Dim targetIndex = targetLocation.Index


        Dim children = GetAllChildren(packItem).OrderBy(Function(x) x.Index).ToList()
        Do While packItem.Index > targetIndex
            Swap(packItem.Index, packItem.Index - 1)
            For Each child In children
                Swap(child.Index, child.Index - 1)
            Next
        Loop

        Return True
    End Function
    Public Overrides Function MoveDown(ByVal packItem As ModelGenerationPackItem) As Boolean
        If Not CanMoveDown(packItem) Then Return False


        Dim targetLocation = FirstItemBelowMeThatHasSameParent(packItem)
        Dim targetIndex = targetLocation.Index

        Dim children = GetAllChildren(packItem).OrderByDescending(Function(x) x.Index).ToList()
        targetIndex -= children.Count

        Do While packItem.Index < targetIndex
            For Each child In children
                Swap(child.Index, child.Index + 1)
            Next
            Swap(packItem.Index, packItem.Index + 1)
        Loop

        Return True
    End Function

    Private Function FirstItemAboveMeThatHasSameParent(ByVal packItem As ModelGenerationPackItem) As ModelGenerationPackItem
        Dim parentOfItem = GetParentItem(packItem)
        Dim indexAbove = packItem.Index - 1
        While indexAbove >= 0
            Dim packItemAbove = Me(indexAbove)
            Dim parentOfItemAboveMe = GetParentItem(packItemAbove)
            If ModelGenerationPackItem.AreEqual(parentOfItem, parentOfItemAboveMe) Then Return packItemAbove
            indexAbove -= 1
        End While
        Return Nothing
    End Function
    Private Function FirstItemBelowMeThatHasSameParent(ByVal packItem As ModelGenerationPackItem) As ModelGenerationPackItem
        Dim parentOfItem = GetParentItem(packItem)
        Dim indexBelow = packItem.Index + 1
        While indexBelow < Count
            Dim packItemBelow = Me(indexBelow)
            Dim parentOfItemBelowMe = GetParentItem(packItemBelow)
            If ModelGenerationPackItem.AreEqual(parentOfItem, parentOfItemBelowMe) Then Return packItemBelow
            indexBelow += 1
        End While
        Return Nothing
    End Function

    Private Shared Function GetParentItem(packItem As ModelGenerationPackItem) As ModelGenerationPackItem
        If Not packItem.Type = EquipmentType.Option Then Return Nothing
        Return DirectCast(packItem, ModelGenerationPackOption).ParentOption
    End Function

    Private Shared Function GetAllChildren(ByVal packItem As ModelGenerationPackItem) As IEnumerable(Of ModelGenerationPackOption)
        If Not packItem.Type = EquipmentType.Option Then Return New List(Of ModelGenerationPackOption)()

        Dim children = New List(Of ModelGenerationPackOption)()
        For Each child In DirectCast(packItem, ModelGenerationPackOption).ChildOptions
            children.Add(child)
            children.AddRange(GetAllChildren(child))
        Next
        Return children
    End Function



#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationPackItems(ByVal pack As ModelGenerationPack) As ModelGenerationPackItems
        Dim packItems As ModelGenerationPackItems = New ModelGenerationPackItems
        packItems.Pack = pack
        Return packItems
    End Function
    Friend Shared Function GetModelGenerationPackItems(ByVal pack As ModelGenerationPack) As ModelGenerationPackItems
        Dim packItems As ModelGenerationPackItems = DataPortal.Fetch(Of ModelGenerationPackItems)(New ParentCriteria(pack.ID, "@PACKID"))
        packItems.Pack = pack
        packItems.Synchronize()
        If packItems.IsDirty Then
            packItems = packItems.Save()
        End If
        packItems.MarkAsChild()
        Return packItems
    End Function


    Private Sub Synchronize()
        SynchronizeTree()
        RepositionItems()
    End Sub

    Private Sub SynchronizeTree()
        Dim rootOptions = OfType(Of ModelGenerationPackOption)().Select(Function(x) x.GenerationOption.GetRootOption()).Distinct().ToList()
        For Each rootOption In rootOptions
            AddTree(rootOption)
        Next
    End Sub
    Private Sub RepositionItems()
        If Count < 2 Then Return

        Dim list = ToList()
        For Each packItem In list
            If ItemIsPositionedUnderneathParent(packItem) Then Continue For

            If ParentIsBelowMe(packItem) Then
                MoveDownToParent(packItem)
            Else
                MoveUpToParent(packItem)
            End If
        Next
    End Sub

    Private Function ItemIsPositionedUnderneathParent(ByVal packItem As ModelGenerationPackItem) As Boolean
        Dim parentItem = GetParentItem(packItem)
        If parentItem Is Nothing Then Return True
        If packItem.Index = 0 Then Return False

        For i As Integer = packItem.Index - 1 To 0 Step -1
            Dim itemAbove = Item(i)
            Dim parentOfItemAbove = GetParentItem(itemAbove)

            If itemAbove.Equals(parentItem) Then Return True
            If parentOfItemAbove Is Nothing OrElse Not parentOfItemAbove.Equals(parentItem) Then Return False
        Next

        Return False
    End Function

    Private Function ParentIsBelowMe(ByVal packItem As ModelGenerationPackItem) As Boolean
        Dim parentItem = GetParentItem(packItem)

        For i As Integer = (packItem.Index + 1) To (Count - 1) Step 1
            If ModelGenerationPackItem.AreEqual(parentItem, Me(i)) Then Return True
        Next
        Return False
    End Function

    Private Sub MoveDownToParent(ByVal packItem As ModelGenerationPackItem)
        Do While Not ItemAboveMeIsMyParent(packItem)
            Swap(packItem.Index, (packItem.Index + 1))
        Loop
    End Sub
    Private Sub MoveUpToParent(ByVal packItem As ModelGenerationPackItem)
        Do While Not ItemAboveMeIsMyParent(packItem)
            Swap(packItem.Index, (packItem.Index - 1))
        Loop

        Do While ItemBelowMeHasTheSameParent(packItem)
            Swap(packItem.Index, (packItem.Index + 1))
        Loop
    End Sub
    Private Function ItemAboveMeIsMyParent(ByVal packItem As ModelGenerationPackItem) As Boolean
        If packItem.Index = 0 Then Return False
        Dim parentItem = GetParentItem(packItem)
        Dim itemAboveMe = Me(packItem.Index - 1)
        Return ModelGenerationPackItem.AreEqual(parentItem, itemAboveMe)
    End Function
    Private Function ItemBelowMeHasTheSameParent(ByVal packItem As ModelGenerationPackItem) As Boolean
        If packItem.Index = Count - 1 Then Return False
        Dim parentItem = GetParentItem(packItem)
        Dim parentItemBelowMe = GetParentItem(Me(packItem.Index + 1))
        Return ModelGenerationPackItem.AreEqual(parentItem, parentItemBelowMe)
    End Function




#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Function GetObject(ByVal dataReader As SafeDataReader) As ModelGenerationPackItem
        Return ModelGenerationPackItem.GetModelGenerationPackItem(dataReader)
    End Function

    Protected Overrides Sub FetchNextResult(ByVal dataReader As SafeDataReader)

        FetchPackExteriorColours(dataReader)

        Call dataReader.NextResult()
        FetchPackUpholsteries(dataReader)

        RemoveExteriorColoourTypesAndUphosterTypesWithoutData()
    End Sub

    Private Sub RemoveExteriorColoourTypesAndUphosterTypesWithoutData()
        If Not AllowRemove Then Return

        For i As Integer = Count() - 1 To 0 Step -1
            Dim packItem As ModelGenerationPackItem = Me(i)
            If TypeOf (packItem) Is ModelGenerationPackExteriorColourType Then
                If Not DirectCast(packItem, ModelGenerationPackExteriorColourType).ExteriorColours.Any Then
                    RemoveAt(i)
                End If
            ElseIf TypeOf (packItem) Is ModelGenerationPackUpholsteryType Then
                If Not DirectCast(packItem, ModelGenerationPackUpholsteryType).Upholsteries.Any Then
                    RemoveAt(i)
                End If
            End If
        Next
    End Sub

    Private Sub FetchPackExteriorColours(ByVal dataReader As SafeDataReader)
        While dataReader.Read
            Dim packItem As ModelGenerationPackItem = Item(dataReader.GetGuid("EXTERIORCOLOURTYPEID"))
            If Not packItem Is Nothing Then
                DirectCast(packItem, ModelGenerationPackExteriorColourType).ExteriorColours.Add(dataReader)
            End If
        End While
    End Sub
    Private Sub FetchPackUpholsteries(ByVal dataReader As SafeDataReader)
        While dataReader.Read
            Dim packItem As ModelGenerationPackItem = Item(dataReader.GetGuid("UPHOLSTERYTYPEID"))
            If Not packItem Is Nothing Then
                DirectCast(packItem, ModelGenerationPackUpholsteryType).Upholsteries.Add(dataReader)
            End If
        End While
    End Sub


#End Region


End Class
<Serializable()> Public MustInherit Class ModelGenerationPackItem
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationPackItem)
    Implements ISortedIndex
    Implements ISortedIndexSetter
    Implements IOwnedBy
    Implements IPrice
    Implements IMasterObjectReference

#Region " Business Properties & Methods "

    Private _owner As String
    Private _index As Integer
    Private _availability As Availability
    Private _colouringModes As ColouringModes
    Private _price As Decimal = 0D
    Private _vatPrice As Decimal = 0D

    Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If _index.Equals(value) Then Return

            _index = value
            PropertyHasChanged("Index")
        End Set
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Overridable Property Availability() As Availability
        Get
            Return _availability
        End Get
        Set(ByVal value As Availability)
            If value.Equals(Availability) Then Return
            _availability = value
            PropertyHasChanged("Availability")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property ColouringModes() As ColouringModes
        Get
            Return _colouringModes
        End Get
        Set(ByVal value As ColouringModes)
            If value.Equals(_colouringModes) Then Return

            _colouringModes = value
            PropertyHasChanged("ColouringModes")

            DirectCast(Parent, ModelGenerationPackItems).RaisePackItemColouringModeChangedEvent(Me)
        End Set
    End Property
    Public Property Price() As Decimal Implements IPrice.PriceExcludingVat
        Get
            Return _price
        End Get
        Set(ByVal value As Decimal)
            If _price.Equals(value) Then Return

            _price = value
            PropertyHasChanged("Price")
        End Set
    End Property
    Public Property VatPrice() As Decimal Implements IPrice.PriceIncludingVat
        Get
            Return _vatPrice
        End Get
        Set(ByVal value As Decimal)
            If _vatPrice.Equals(value) Then Return

            _vatPrice = value
            PropertyHasChanged("VatPrice")
        End Set
    End Property


#End Region

#Region " Reference Properties & Methods "
    Private _generationEquipmentItem As ModelGenerationEquipmentItem

    Public ReadOnly Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPackItems).Pack
        End Get
    End Property
    Private ReadOnly Property Generation() As ModelGeneration
        Get
            If Pack Is Nothing Then Return Nothing
            Return Pack.Generation
        End Get
    End Property
    Protected ReadOnly Property GenerationEquipmentItem() As ModelGenerationEquipmentItem
        Get
            If Generation Is Nothing Then Return Nothing
            If _generationEquipmentItem Is Nothing Then _generationEquipmentItem = Generation.Equipment(ID)
            Return _generationEquipmentItem
        End Get
    End Property


    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return GenerationEquipmentItem.ShortID
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return GenerationEquipmentItem.Owner
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property PartNumber() As String
        Get
            Return GenerationEquipmentItem.PartNumber
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return GenerationEquipmentItem.Name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Type() As EquipmentType
        Get
            Return GenerationEquipmentItem.Type
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Colour() As ExteriorColourInfo
        Get
            Return GenerationEquipmentItem.Colour
        End Get
    End Property

    Public ReadOnly Property Translation() As Translation
        Get
            Return GenerationEquipmentItem.Translation
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return GenerationEquipmentItem.AlternateName
        End Get
    End Property
    Public ReadOnly Property MasterID() As Guid Implements IMasterObjectReference.MasterID
        Get
            Return GenerationEquipmentItem.MasterID
        End Get
    End Property

    Public ReadOnly Property MasterDescription() As String Implements IMasterObjectReference.MasterDescription
        Get
            Return GenerationEquipmentItem.MasterDescription
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationPackOption) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID) AndAlso (Generation Is Nothing OrElse DirectCast(Me, ModelGenerationPackOption).MasterPath.Equals(obj.MasterPath))
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationPackItem) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationEquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationPackOption Then
            Return Equals(DirectCast(obj, ModelGenerationPackOption))
        ElseIf TypeOf obj Is ModelGenerationPackItem Then
            Return Equals(DirectCast(obj, ModelGenerationPackItem))
        ElseIf TypeOf obj Is ModelGenerationEquipmentItem Then
            Return Equals(DirectCast(obj, ModelGenerationEquipmentItem))
        ElseIf TypeOf obj Is EquipmentItem Then
            Return Equals(DirectCast(obj, EquipmentItem))
        ElseIf TypeOf obj Is Guid Then
            Return ID.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

    Public Shared Function AreEqual(ByVal objA As ModelGenerationPackItem, ByVal objB As ModelGenerationPackItem) As Boolean
        If objA Is Nothing AndAlso objB Is Nothing Then Return True
        If objA Is Nothing OrElse objB Is Nothing Then Return False
        Return objA.ID.Equals(objB.ID)
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationPackItem(ByVal item As ModelGenerationEquipmentItem) As ModelGenerationPackItem
        Dim packItem As ModelGenerationPackItem
        Select Case item.Type
            Case EquipmentType.Accessory
                packItem = New ModelGenerationPackAccessory
            Case EquipmentType.Option
                packItem = New ModelGenerationPackOption
            Case EquipmentType.ExteriorColourType
                packItem = New ModelGenerationPackExteriorColourType
            Case EquipmentType.UpholsteryType
                packItem = New ModelGenerationPackUpholsteryType
            Case Else
                Throw New InvalidEquipmentType("""" & item.Type & """ is not a valid equipment type for this routine!")
        End Select
        packItem.Create(item)
        Return packItem
    End Function
    Friend Shared Function GetModelGenerationPackItem(ByVal dataReader As SafeDataReader) As ModelGenerationPackItem
        Dim packItemType As String = dataReader.GetString("TYPE")
        Dim packItem As ModelGenerationPackItem
        Select Case packItemType
            Case Environment.DBAccessoryCode
                packItem = New ModelGenerationPackAccessory
            Case Environment.DBOptionCode
                packItem = New ModelGenerationPackOption
            Case Environment.DBExteriorColourTypeCode
                packItem = New ModelGenerationPackExteriorColourType
            Case Environment.DBUpholsteryTypeCode
                packItem = New ModelGenerationPackUpholsteryType
            Case Else
                Throw New InvalidEquipmentType("""" & packItemType & """ is not a valid equipment type for this routine!")
        End Select
        packItem.Fetch(dataReader)
        Return packItem
    End Function
#End Region

#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As SafeDataReader)
        ID = dataReader.GetGuid("EQUIPMENTID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _owner = .GetString("OWNER")
            _availability = .GetAvailability("AVAILABILITY")
            _colouringModes = .GetColouringModes("COLOURINGMODES")
            _index = .GetInt16("SORTORDER")
            _price = Environment.ConvertPrice(CType(.GetValue("PRICE"), Decimal), .GetString("CURRENCY"))
            _vatPrice = Environment.ConvertPrice(CType(.GetValue("PRICEVAT"), Decimal), .GetString("CURRENCY"))
        End With
        MyBase.FetchFields(dataReader)

        AllowRemove = _owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase)
    End Sub

    Private Overloads Sub Create(ByVal item As ModelGenerationEquipmentItem)
        Create(item.ID)
        _owner = MyContext.GetContext().CountryCode
        _colouringModes = ColouringModes.None
        With item
            If (item.Type = EquipmentType.Option OrElse item.Type = EquipmentType.Accessory) Then
                _availability = Availability.Standard
            Else
                _availability = Availability.NotAvailable
            End If
            _generationEquipmentItem = item
        End With
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As SqlCommand)
        command.CommandText = "insertModelGenerationPackItem"
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As SqlCommand)
        command.CommandText = "updateModelGenerationPackItem"
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As SqlCommand)
        command.CommandText = "deleteModelGenerationPackItem"
        AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@PACKID", Pack.ID)
        command.Parameters.AddWithValue("@EQUIPMENTID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@AVAILABILITY", Availability)
        command.Parameters.AddWithValue("@COLOURINGMODES", ColouringModes)
        command.Parameters.AddWithValue("@PRICE", Price)
        command.Parameters.AddWithValue("@PRICEVAT", VatPrice)
        command.Parameters.AddWithValue("@CURRENCY", MyContext.GetContext().Currency.Code)
        command.Parameters.AddWithValue("@SORTORDER", Index)
    End Sub

#End Region

End Class

<Serializable()> Public NotInheritable Class ModelGenerationPackAccessory
    Inherits ModelGenerationPackItem
End Class
<Serializable()> Public NotInheritable Class ModelGenerationPackOption
    Inherits ModelGenerationPackItem
    Implements IMasterPathObjectReference

#Region "Business Properties & Methods"


    Friend ReadOnly Property GenerationOption() As ModelGenerationOption
        Get
            If GenerationEquipmentItem Is Nothing Then Return Nothing
            Return DirectCast(GenerationEquipmentItem, ModelGenerationOption)
        End Get
    End Property
    Public ReadOnly Property MasterPath() As String Implements IMasterPathObjectReference.MasterPath
        Get
            If GenerationOption Is Nothing Then Return Nothing
            Return GenerationOption.MasterPath
        End Get
    End Property

    Public ReadOnly Property ParentOption() As ModelGenerationPackOption
        Get
            If GenerationOption Is Nothing Then Return Nothing
            If Not GenerationOption.HasParentOption Then Return Nothing

            Return DirectCast(DirectCast(Parent, ModelGenerationPackItems)(GenerationOption.ParentOption.ID), ModelGenerationPackOption)
        End Get
    End Property
    Public Function GetRootOption() As ModelGenerationPackOption
        Return GetRootOption(Me)
    End Function
    Private Shared Function GetRootOption(ByVal [option] As ModelGenerationPackOption) As ModelGenerationPackOption
        Dim rootOption As ModelGenerationPackOption
        If [option].HasParentOption Then
            rootOption = [option].ParentOption
            While rootOption.HasParentOption
                rootOption = rootOption.ParentOption
            End While
        Else
            rootOption = [option]
        End If
        Return rootOption
    End Function

    Public ReadOnly Property ChildOptions() As IEnumerable(Of ModelGenerationPackOption)
        Get
            If Parent Is Nothing Then Return New List(Of ModelGenerationPackOption)
            Return DirectCast(Parent, ModelGenerationPackItems).OfType(Of ModelGenerationPackOption).Where(Function(x) x.HasParentOption AndAlso x.ParentOption.ID.Equals(ID))
        End Get
    End Property

    Public ReadOnly Property HasParentOption() As Boolean
        Get
            Return Not ParentOption Is Nothing
        End Get
    End Property

    Public ReadOnly Property HasChildOptions() As Boolean
        Get
            Return ChildOptions.Any()
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Overrides Property Availability() As Availability
        Get
            Return MyBase.Availability
        End Get
        Set(ByVal value As Availability)
            If value.Equals(Availability) Then Return
            MyBase.Availability = value
            RecalculateParentAvailability()
            RevalidateChildAvailability()
        End Set
    End Property

    Private Sub RecalculateParentAvailability()
        If Not HasParentOption Then Return

        Dim parentAvailabilty = CalculateParentAvailability()
        If Not ParentOption.Availability.Equals(parentAvailabilty) Then
            ParentOption.Availability = parentAvailabilty
        End If
    End Sub
    Private Function CalculateParentAvailability() As Availability
        If ParentOption.ChildOptions.Any(Function(x) x.Availability = Availability.Standard) Then Return Availability.Standard
        If ParentOption.ChildOptions.Any(Function(x) x.Availability = Availability.Optional) Then Return Availability.Optional
        Return Availability.NotAvailable
    End Function

    Private Sub RevalidateChildAvailability()
        If Not HasChildOptions Then Return

        For Each carOption In ChildOptions
            carOption.ValidationRules.CheckRules("Availability")
        Next
    End Sub

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeStandardIfNonOfTheChildOptionsAreStandard, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeOptionalIfNonOfTheChildOptionsAreOptional, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeNotAvailableIfNotAllChildOptionsNotAvailable, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeStandardIfParentIsNotStandard, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeOptionalIfParentIsNotOptionalOrStandard, RuleHandler), "Availability")
    End Sub

    Private Shared Function OptionCanNotBeStandardIfNonOfTheChildOptionsAreStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim packOption As ModelGenerationPackOption = DirectCast(target, ModelGenerationPackOption)
        If Not packOption.Availability = Enums.Availability.Standard Then Return True
        If Not packOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made Standard if non of the child options are Standard", packOption.AlternateName)

        Return packOption.ChildOptions.Any(Function(x) x.Availability = Enums.Availability.Standard)
    End Function
    Private Shared Function OptionCanNotBeOptionalIfNonOfTheChildOptionsAreOptional(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim packOption As ModelGenerationPackOption = DirectCast(target, ModelGenerationPackOption)
        If Not packOption.Availability = Enums.Availability.Optional Then Return True
        If Not packOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made Optional if non of the child options are Optional", packOption.AlternateName)

        Return packOption.ChildOptions.Any(Function(x) x.Availability = Enums.Availability.Optional) AndAlso Not packOption.ChildOptions.Any(Function(x) x.Availability = Enums.Availability.Standard)
    End Function
    Private Shared Function OptionCanNotBeNotAvailableIfNotAllChildOptionsNotAvailable(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim packOption As ModelGenerationPackOption = DirectCast(target, ModelGenerationPackOption)
        If Not packOption.Availability = Enums.Availability.NotAvailable Then Return True
        If Not packOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made Not Available if not all of the child options are Not Available", packOption.AlternateName)

        Return packOption.ChildOptions.All(Function(x) x.Availability = Enums.Availability.NotAvailable)
    End Function
    Private Shared Function OptionCanNotBeStandardIfParentIsNotStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim packOption As ModelGenerationPackOption = DirectCast(target, ModelGenerationPackOption)
        If Not packOption.Availability = Enums.Availability.Standard Then Return True
        If Not packOption.HasParentOption Then Return True
        e.Description = String.Format("The option {0} can not be made Standard if its parent is not Standard.", packOption.AlternateName)

        Return packOption.ParentOption.Availability = Enums.Availability.Standard
    End Function
    Private Shared Function OptionCanNotBeOptionalIfParentIsNotOptionalOrStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim packOption As ModelGenerationPackOption = DirectCast(target, ModelGenerationPackOption)
        If Not packOption.Availability = Enums.Availability.Optional Then Return True
        If Not packOption.HasParentOption Then Return True
        e.Description = String.Format("The option {0} can not be made Optional if its parent is Not Available.", packOption.AlternateName)

        Return packOption.ParentOption.Availability = Enums.Availability.Optional OrElse packOption.ParentOption.Availability = Enums.Availability.Standard
    End Function
#End Region

End Class

<Serializable()> Public NotInheritable Class ModelGenerationPackExteriorColourType
    Inherits ModelGenerationPackItem

#Region " Business Properties & Methods "

    Private _exteriorColours As ModelGenerationPackExteriorColours

    Public ReadOnly Property ExteriorColours() As ModelGenerationPackExteriorColours
        Get
            If _exteriorColours Is Nothing Then _exteriorColours = ModelGenerationPackExteriorColours.GetModelGenerationPackExteriorColours(Me)
            Return _exteriorColours
        End Get
    End Property

#End Region

#Region " Business & Validation Rules "
    Friend Sub ValidateBusinessRules(ByVal propertyName As String)
        ValidationRules.CheckRules(propertyName)
    End Sub

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf ExteriorColourApproved, RuleHandler), "Availability")
    End Sub
    Private Shared Function ExteriorColourApproved(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim exteriorColourType As ModelGenerationPackExteriorColourType = DirectCast(target, ModelGenerationPackExteriorColourType)
        If exteriorColourType.Availability = Availability.NotAvailable Then Return True 'If I'm not available, then I don't care...
        If exteriorColourType.ExteriorColours.Any(Function(o) o.Approved) Then Return True 'If at least one car has been approved, then I'm fine 

        e.Description = String.Format("The item ""{0}"" could not be set to optional or standard.{1}At least one of the related exterior colours need be added and approved.", exteriorColourType.Name, System.Environment.NewLine)
        Return False
    End Function
#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not ExteriorColours.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If ExteriorColours.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        _exteriorColours = ModelGenerationPackExteriorColours.NewModelGenerationPackExteriorColours(Me)
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
        ExteriorColours.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region


End Class
<Serializable()> Public NotInheritable Class ModelGenerationPackUpholsteryType
    Inherits ModelGenerationPackItem

#Region " Business Properties & Methods "

    Private _upholsteries As ModelGenerationPackUpholsteries

    Public ReadOnly Property Upholsteries() As ModelGenerationPackUpholsteries
        Get
            If _upholsteries Is Nothing Then _upholsteries = ModelGenerationPackUpholsteries.GetModelGenerationPackUpholsteries(Me)
            Return _upholsteries
        End Get
    End Property

#End Region

#Region " Business & Validation Rules "
    Friend Sub ValidateBusinessRules(ByVal propertyName As String)
        ValidationRules.CheckRules(propertyName)
    End Sub
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf UpholsteryApproved, RuleHandler), "Availability")
    End Sub
    Private Shared Function UpholsteryApproved(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim upholsteryType As ModelGenerationPackUpholsteryType = DirectCast(target, ModelGenerationPackUpholsteryType)
        If upholsteryType.Availability = Availability.NotAvailable Then Return True 'If I'm not available, then I don't care...
        If upholsteryType.Upholsteries.Any(Function(o) o.Approved) Then Return True 'If at least one car has been approved, then I'm fine 

        e.Description = String.Format("The item ""{0}"" could not be set to optional or standard.{1}At least one of the related upholsteries need be added and approved.", upholsteryType.Name, System.Environment.NewLine)
        Return False
    End Function
#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not Upholsteries.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Upholsteries.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        _upholsteries = ModelGenerationPackUpholsteries.NewModelGenerationPackUpholsteries(Me)
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
        Upholsteries.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region



End Class