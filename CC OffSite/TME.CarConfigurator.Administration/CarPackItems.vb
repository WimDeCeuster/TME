Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Exceptions
Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.Validation


<Serializable()> Public NotInheritable Class CarPackItems
    Inherits ContextUniqueGuidListBase(Of CarPackItems, CarPackItem)


#Region " Business Properties & Methods "

    Friend Property CarPack() As CarPack
        Get
            Return DirectCast(Parent, CarPack)
        End Get
        Private Set(ByVal value As CarPack)
            SetParent(value)
            AddHandler value.GradePack.GenerationPack.Equipment.PackItemAdded, AddressOf OnPackItemAdded
            AddHandler value.GradePack.GenerationPack.Equipment.PackItemRemoved, AddressOf OnPackItemRemoved
        End Set
    End Property
    Private Sub OnPackItemAdded(ByVal packItem As ModelGenerationPackItem)
        If Contains(packItem.ID) Then Throw New ApplicationException("The item already exists in this collection")

        Dim carPackItem As CarPackItem = GetObject(packItem)

        AllowNew = True
        Add(carPackItem)
        AllowNew = False
    End Sub
    Private Sub OnPackItemRemoved(ByVal packItem As ModelGenerationPackItem)
        Dim carPackItem As CarPackItem = Me(packItem.ID)

        AllowRemove = True
        carPackItem.Remove()
        AllowRemove = False
    End Sub

    Public Sub RecalculateAvailabilities()
        For Each carOption In CarPack.OfferedSuffixOptions()
            CarPack.Equipment(carOption.ID).CalculatePackAvailability()
        Next
    End Sub

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetPackItems(ByVal carPack As CarPack) As CarPackItems
        Dim carPackItems As CarPackItems = New CarPackItems
        Dim generationPackItems As ModelGenerationPackItems = carPack.GradePack.GenerationPack.Equipment
        carPackItems.CarPack = carPack
        If carPack.IsNew Then
            carPackItems.Combine(generationPackItems, Nothing)
        Else
            carPackItems.Combine(generationPackItems, DataPortal.Fetch(Of CarPackItems)(New CustomCriteria(carPack)))
        End If

        Return carPackItems
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowNew = False
        AllowRemove = False
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _carID As Guid
        Private ReadOnly _packID As Guid

        Public Sub New(ByVal carPack As CarPack)
            _carID = carPack.Car.ID
            _packID = carPack.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CARID", _carID)
            command.Parameters.AddWithValue("@PACKID", _packID)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property
    Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As CarPackItem
        Dim type = dataReader.GetEquipmentType("EQUIPMENTTYPE")
        Select Case type
            Case EquipmentType.Accessory
                Return CarPackAccessory.GetCarPackAccessory(dataReader)
            Case EquipmentType.Option
                Return CarPackOption.GetCarPackOption(dataReader)
            Case EquipmentType.ExteriorColourType
                Return CarPackExteriorColourType.GetCarPackExteriorColourType(dataReader)
            Case EquipmentType.UpholsteryType
                Return CarPackUpholsteryType.GetCarPackUpholsteryType(dataReader)
            Case Else
                Throw New InvalidEquipmentType("""" & type & """ is not a valid equipment type!")
        End Select
    End Function
    Private Overloads Shared Function GetObject(ByVal packItem As ModelGenerationPackItem) As CarPackItem
        Select Case packItem.Type
            Case EquipmentType.Accessory
                Return CarPackAccessory.NewCarPackAccessory(DirectCast(packItem, ModelGenerationPackAccessory))
            Case EquipmentType.Option
                Return CarPackOption.NewCarPackOption(DirectCast(packItem, ModelGenerationPackOption))
            Case EquipmentType.ExteriorColourType
                Return CarPackExteriorColourType.NewCarPackExteriorColourType(DirectCast(packItem, ModelGenerationPackExteriorColourType))
            Case EquipmentType.UpholsteryType
                Return CarPackUpholsteryType.NewCarPackUpholsteryType(DirectCast(packItem, ModelGenerationPackUpholsteryType))
            Case Else
                Throw New InvalidEquipmentType("""" & packItem.Type & """ is not a valid grade equipment type!")
        End Select
    End Function

    Private Sub OnBeforeUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.BeforeUpdateCommand
        'Clear the list of deleted objects. 
        'The database will take care of this for us via deleteGrenerationEquipmentItem
        DeletedList.Clear()
    End Sub


    Private Sub Combine(ByVal generationPackItems As ModelGenerationPackItems, ByVal carPackItems As CarPackItems)
        AllowNew = True

        Dim context = MyContext.GetContext()
        If Not context.EquipmentGroupsInitialized() OrElse context.LazyLoad Then
            context.LazyLoad = False
            context.EquipmentGroups = EquipmentGroups.GetEquipmentGroups(False)
        End If

        Dim groups As EquipmentGroups = context.EquipmentGroups
        Dim fittablePackItems = (From x In generationPackItems Where
                                        x.Type = EquipmentType.ExteriorColourType OrElse
                                        x.Type = EquipmentType.UpholsteryType OrElse
                                        groups.FindEquipment(x.ID).CanBeFittedOn(CarPack.Car)
                                        )

        For Each generationPackItem In fittablePackItems
            If carPackItems IsNot Nothing AndAlso carPackItems.Contains(generationPackItem.ID) Then
                Dim carPackItem As CarPackItem = carPackItems(generationPackItem.ID)
                carPackItem.GenerationPackItem = generationPackItem
                Add(carPackItem)
            Else
                Add(GetObject(generationPackItem))
            End If
        Next
        AllowNew = False
    End Sub

#End Region
End Class
<Serializable()> Public MustInherit Class CarPackItem
    Inherits ContextUniqueGuidBusinessBase(Of CarPackItem)
    Implements IOwnedBy
    Implements IPrice
    Implements IOverwritable
    Implements IMasterObjectReference

#Region " Business Properties & Methods "
    Private _overwritten As Boolean = False
    Private _availability As Availability
    Private _colouringModes As ColouringModes
    Private _price As Decimal = 0D
    Private _vatPrice As Decimal = 0D


    <XmlInfo(XmlNodeType.Attribute)> Public Property Overwritten() As Boolean
        Get
            Return _overwritten
        End Get
        Private Set(ByVal value As Boolean)
            If value.Equals(_overwritten) Then Return

            _overwritten = value
            AllowEdit = value
            AllowNew = value
            PropertyHasChanged("Overwritten")
        End Set
    End Property


    Public Function HasBeenOverwritten() As Boolean Implements IOverwritable.HasBeenOverwritten
        Return Overwritten
    End Function
    Public Sub Overwrite() Implements IOverwritable.Overwrite
        Overwritten = True
    End Sub
    Public Overridable Sub Revert() Implements IOverwritable.Revert
        If Not Overwritten Then Return

        Overwritten = False
        RefreshParentProperties()

        If IsNew Then
            MarkClean()
        End If

    End Sub

    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Overwritten") Then Return True
        Return MyBase.CanWriteProperty(propertyName)
    End Function
    Friend Sub Remove()
        AllowRemove = True
        DirectCast(Parent, CarPackItems).Remove(Me)
        AllowRemove = False
    End Sub

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
    Private WithEvents _generationPackItem As ModelGenerationPackItem

    Public ReadOnly Property Pack() As CarPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarPackItems).CarPack
        End Get
    End Property
    Friend Property GenerationPackItem() As ModelGenerationPackItem
        Get
            Return _generationPackItem
        End Get
        Set(value As ModelGenerationPackItem)
            _generationPackItem = value
            RefreshParentProperties()
        End Set
    End Property
    Private Sub OnGenerationPackItemPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles _generationPackItem.PropertyChanged
        If e.PropertyName.Length = 0 Then Exit Sub
        RefreshParentProperties()
    End Sub
    Private Sub RefreshParentProperties()
        If Overwritten Then Return

        _availability = GenerationPackItem.Availability
        _colouringModes = GenerationPackItem.ColouringModes
        _price = GenerationPackItem.Price
        _vatPrice = GenerationPackItem.VatPrice
    End Sub

    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return GenerationPackItem.ShortID
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return GenerationPackItem.Owner
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
        Get
            Return GenerationPackItem.Name
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property PartNumber() As String
        Get
            Return GenerationPackItem.PartNumber
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Type() As EquipmentType
        Get
            Return GenerationPackItem.Type
        End Get
    End Property
    <XmlInfo(XmlNodeType.Element)> Public ReadOnly Property Colour() As ExteriorColourInfo
        Get
            Return GenerationPackItem.Colour
        End Get
    End Property
 Public ReadOnly Property Index() As Integer
        Get
            Return GenerationPackItem.Index
        End Get
    End Property

    Public ReadOnly Property MasterID() As Guid Implements IMasterObjectReference.MasterID
        Get
            Return GenerationPackItem.MasterID
        End Get
    End Property

    Public ReadOnly Property MasterDescription() As String Implements IMasterObjectReference.MasterDescription
        Get
            Return GenerationPackItem.MasterDescription
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return GenerationPackItem.AlternateName
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Function Equals(ByVal obj As CarPackItem) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As CarEquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
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
        If TypeOf obj Is CarPackItem Then
            Return Equals(DirectCast(obj, CarPackItem))
        ElseIf TypeOf obj Is CarEquipmentItem Then
            Return Equals(DirectCast(obj, CarEquipmentItem))
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

#End Region

#Region " Constructors "

    Protected Sub New()
        MarkAsChild()
        AllowNew = False
        AllowRemove = False
        AllowEdit = False
    End Sub

#End Region


#Region " Data Access "
    Protected Overloads Sub Create(ByVal packItem As ModelGenerationPackItem)
        Create(packItem.ID)
        GenerationPackItem = packItem
        RefreshParentProperties()
        MarkClean() 'These objects don't need be saved untill somebody actualy does something with them
    End Sub
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As SafeDataReader)
        ID = dataReader.GetGuid("EQUIPMENTID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _availability = .GetAvailability("AVAILABILITY")
            _colouringModes = .GetColouringModes("COLOURINGMODES")
            _price = Environment.ConvertPrice(CType(.GetValue("PRICE"), Decimal), .GetString("CURRENCY"))
            _vatPrice = Environment.ConvertPrice(CType(.GetValue("PRICEVAT"), Decimal), .GetString("CURRENCY"))
        End With
        _overwritten = True
        AllowEdit = True
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Private Sub AddCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@CARID", Pack.Car.ID)
        command.Parameters.AddWithValue("@PACKID", Pack.ID)
        command.Parameters.AddWithValue("@EQUIPMENTID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateCarPackItem"
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If Not Overwritten Then
            command.CommandText = "deleteCarPackItem"
            Exit Sub
        End If

        command.CommandText = "updateCarPackItem"
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@AVAILABILITY", Availability)
        command.Parameters.AddWithValue("@COLOURINGMODES", ColouringModes)
        command.Parameters.AddWithValue("@PRICE", Price)
        command.Parameters.AddWithValue("@PRICEVAT", VatPrice)
        command.Parameters.AddWithValue("@CURRENCY", MyContext.GetContext().Currency.Code)
    End Sub

#End Region


    Public Sub CalculatePackAvailability()
        Dim carOption = DirectCast(Pack.Car.Equipment(ID), CarOption)

        If carOption Is Nothing Then
            Availability = Availability.NotAvailable
            Return
        End If
        If Not carOption.SuffixOption  Then throw new OptionIsNotASuffixOptionException(Me)

        Overwrite()

        If carOption.SuffixAvailability = Availability.NotAvailable Then 
            Availability = Availability.NotAvailable
            Return
        End If

        If carOption.SuffixAvailability = Availability.Standard Then
            Availability = Availability.Standard
            Return
        End If

        If Pack.Availability = Availability.Standard Then
            Availability = Availability.Optional 'optional caroption can only be optional when offered through a standard pack
            Return
        End If

        If Availability <> Availability.NotAvailable Then Return 'in an optional pack, any availability other than not available is fine for an optional suffix option, so there is no need to adjust it

        Availability = Availability.Standard 'if a pack is optional, the default availability for an optional suffix option is standard
    End Sub
End Class

<Serializable()> Public NotInheritable Class CarPackAccessory
    Inherits CarPackItem

#Region " Shared Factory Methods "
    Public Shared Function NewCarPackAccessory(ByVal generationPackAccessory As ModelGenerationPackAccessory) As CarPackAccessory
        Dim packAccessory = New CarPackAccessory
        packAccessory.Create(generationPackAccessory)
        Return packAccessory
    End Function
    Public Shared Function GetCarPackAccessory(ByVal dataReader As SafeDataReader) As CarPackAccessory
        Dim packAccessory = New CarPackAccessory
        packAccessory.Fetch(dataReader)
        Return packAccessory
    End Function
#End Region

End Class
<Serializable()> Public NotInheritable Class CarPackOption
    Inherits CarPackItem

#Region "Properties & Methods"

    Public ReadOnly Property ParentOption() As CarPackOption
        Get
            Dim generationPackOption = DirectCast(GenerationPackItem, ModelGenerationPackOption)
            If Not generationPackOption.HasParentOption Then Return Nothing

            Return DirectCast(DirectCast(Parent, CarPackItems)(generationPackOption.ParentOption.ID), CarPackOption)
        End Get
    End Property

    Public ReadOnly Property ChildOptions() As IEnumerable(Of CarPackOption)
        Get
            Return DirectCast(Parent, CarPackItems).OfType(Of CarPackOption).Where(Function(x) Not x.ParentOption Is Nothing AndAlso x.ParentOption.ID.Equals(ID))
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
    Public Overrides Sub Revert()
        MyBase.Revert()
        ValidationRules.CheckRules("Availability")
        RevalidateChildAvailability()
    End Sub
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
            ParentOption.Overwrite()
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

        For Each carPackOption In ChildOptions
            carPackOption.ValidationRules.CheckRules("Availability")
        Next
    End Sub

#End Region


#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        MyBase.AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeStandardIfNonOfTheChildOptionsAreStandard, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeOptionalIfNonOfTheChildOptionsAreOptional, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeNotAvailableIfNotAllChildOptionsNotAvailable, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeStandardIfParentIsNotStandard, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeOptionalIfParentIsNotOptionalOrStandard, RuleHandler), "Availability")
    End Sub

    Private Shared Function OptionCanNotBeStandardIfNonOfTheChildOptionsAreStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim carPackOption As CarPackOption = DirectCast(target, CarPackOption)
        If carPackOption.GenerationPackItem Is Nothing Then Return True 'Rule can not be checked yet
        If Not carPackOption.Availability = Enums.Availability.Standard Then Return True
        If Not carPackOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made standard because at least one child option needs to be standard as well", carPackOption.AlternateName)

        Return carPackOption.ChildOptions.Any(Function(x) x.Availability = Enums.Availability.Standard)
    End Function
    Private Shared Function OptionCanNotBeOptionalIfNonOfTheChildOptionsAreOptional(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim carPackOption As CarPackOption = DirectCast(target, CarPackOption)
        If carPackOption.GenerationPackItem Is Nothing Then Return True 'Rule can not be checked yet

        If Not carPackOption.Availability = Enums.Availability.Optional Then Return True
        If Not carPackOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made optional unless all child options are optional", carPackOption.AlternateName)

        Return carPackOption.ChildOptions.All(Function(x) x.Availability = Enums.Availability.Optional OrElse x.Availability = Enums.Availability.NotAvailable)
    End Function
    Private Shared Function OptionCanNotBeNotAvailableIfNotAllChildOptionsNotAvailable(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim carPackOption As CarPackOption = DirectCast(target, CarPackOption)
        If carPackOption.GenerationPackItem Is Nothing Then Return True 'Rule can not be checked yet

        If Not carPackOption.Availability = Enums.Availability.NotAvailable Then Return True
        If Not carPackOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made unavailable if one ore more child options are optional or standard", carPackOption.AlternateName)

        Return carPackOption.ChildOptions.All(Function(x) x.Availability = Enums.Availability.NotAvailable)
    End Function
    Private Shared Function OptionCanNotBeStandardIfParentIsNotStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim carPackOption As CarPackOption = DirectCast(target, CarPackOption)
        If carPackOption.GenerationPackItem Is Nothing Then Return True 'Rule can not be checked yet

        If Not carPackOption.Availability = Enums.Availability.Standard Then Return True
        If Not carPackOption.HasParentOption Then Return True
        e.Description = String.Format("The option {0} can not be made Standard if its parent is not Standard.", carPackOption.AlternateName)

        Return carPackOption.ParentOption.Availability = Enums.Availability.Standard
    End Function
    Private Shared Function OptionCanNotBeOptionalIfParentIsNotOptionalOrStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim carPackOption As CarPackOption = DirectCast(target, CarPackOption)
        If carPackOption.GenerationPackItem Is Nothing Then Return True 'Rule can not be checked yet

        If Not carPackOption.Availability = Enums.Availability.Optional Then Return True
        If Not carPackOption.HasParentOption Then Return True
        e.Description = String.Format("The option {0} can not be made Optional if its parent is Not Available.", carPackOption.AlternateName)

        Return carPackOption.ParentOption.Availability = Enums.Availability.Optional OrElse carPackOption.ParentOption.Availability = Enums.Availability.Standard
    End Function
#End Region

#Region " Shared Factory Methods "
    Public Shared Function NewCarPackOption(ByVal generationPackOption As ModelGenerationPackOption) As CarPackItem
        Dim packOption = New CarPackOption
        packOption.Create(generationPackOption)
        Return packOption
    End Function
    Public Shared Function GetCarPackOption(ByVal dataReader As SafeDataReader) As CarPackOption
        Dim packOption = New CarPackOption
        packOption.Fetch(dataReader)
        Return packOption
    End Function

#End Region

End Class
<Serializable()> Public NotInheritable Class CarPackExteriorColourType
    Inherits CarPackItem

#Region " Business Properties & Methods "

    Private _exteriorColours As CarPackExteriorColours

    Public ReadOnly Property ExteriorColours() As CarPackExteriorColours
        Get
            If _exteriorColours Is Nothing Then _exteriorColours = CarPackExteriorColours.GetCarPackExteriorColours(Me)
            Return _exteriorColours
        End Get
    End Property

#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf ExteriorColourApproved, RuleHandler), "Availability")
    End Sub
    Private Shared Function ExteriorColourApproved(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim exteriorColourType As CarPackExteriorColourType = DirectCast(target, CarPackExteriorColourType)
        If exteriorColourType.Availability = Availability.NotAvailable Then Return True 'If I'm not available, then I don't care...
        If exteriorColourType.ExteriorColours.Any(Function(o) o.Approved) Then Return True 'If at least one colour has been approved, then I'm fine 

        e.Description = String.Format("The item ""{0}"" could not be set to optional or standard.{1}At least one of the related exterior colours need be added and approved.", exteriorColourType.Name, System.Environment.NewLine)
        Return False
    End Function
#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not _exteriorColours Is Nothing AndAlso Not _exteriorColours.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not _exteriorColours Is Nothing AndAlso _exteriorColours.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Shared Factory Methods "
    Public Shared Function NewCarPackExteriorColourType(ByVal generationPackExteriorColourType As ModelGenerationPackExteriorColourType) As CarPackItem
        Dim packExteriorColourType = New CarPackExteriorColourType
        packExteriorColourType.Create(generationPackExteriorColourType)
        Return packExteriorColourType
    End Function
    Public Shared Function GetCarPackExteriorColourType(ByVal dataReader As SafeDataReader) As CarPackExteriorColourType
        Dim packExteriorColourType = New CarPackExteriorColourType
        packExteriorColourType.Fetch(dataReader)
        Return packExteriorColourType
    End Function
#End Region

#Region " Data Access "
    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
        If Not _exteriorColours Is Nothing Then _exteriorColours.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class CarPackUpholsteryType
    Inherits CarPackItem

#Region " Business Properties & Methods "

    Private _upholsteries As CarPackUpholsteries

    Public ReadOnly Property Upholsteries() As CarPackUpholsteries
        Get
            If _upholsteries Is Nothing Then _upholsteries = CarPackUpholsteries.GetCarPackUpholsteries(Me)
            Return _upholsteries
        End Get
    End Property

#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf UpholsteryApproved, RuleHandler), "Availability")
    End Sub
    Private Shared Function UpholsteryApproved(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim upholsteryType As CarPackUpholsteryType = DirectCast(target, CarPackUpholsteryType)
        If upholsteryType.Availability = Availability.NotAvailable Then Return True 'If I'm not available, then I don't care...
        If upholsteryType.Upholsteries.Any(Function(o) o.Approved) Then Return True 'If at least one colour has been approved, then I'm fine 

        e.Description = String.Format("The item ""{0}"" could not be set to optional or standard.{1}At least one of the related exterior colours need be added and approved.", upholsteryType.Name, System.Environment.NewLine)
        Return False
    End Function
#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not _upholsteries Is Nothing AndAlso Not _upholsteries.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not _upholsteries Is Nothing AndAlso _upholsteries.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Shared Factory Methods "
    Public Shared Function NewCarPackUpholsteryType(ByVal generationPackUpholsteryType As ModelGenerationPackUpholsteryType) As CarPackItem
        Dim packUpholsteryType = New CarPackUpholsteryType
        packUpholsteryType.Create(generationPackUpholsteryType)
        Return packUpholsteryType
    End Function
    Public Shared Function GetCarPackUpholsteryType(ByVal dataReader As SafeDataReader) As CarPackUpholsteryType
        Dim packUpholsteryType = New CarPackUpholsteryType
        packUpholsteryType.Fetch(dataReader)
        Return packUpholsteryType
    End Function

#End Region

#Region " Data Access "
    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
        If Not _upholsteries Is Nothing Then _upholsteries.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region

End Class