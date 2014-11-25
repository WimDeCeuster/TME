Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class CarEquipment
    Inherits ContextUniqueGuidListBase(Of CarEquipment, CarEquipmentItem)


#Region " Business Properties & Methods "
    <NonSerialized()> Private _referenceMap As Dictionary(Of Guid, CarEquipmentItem)

    Friend Property Car() As Car
        Get
            Return DirectCast(Parent, Car)
        End Get
        Private Set(ByVal value As Car)
            SetParent(value)
            AddHandler value.Generation.Grades(value.Grade.ID).Equipment.EquipmentAdded, AddressOf OnEquipmentAdded
            AddHandler value.Generation.Grades(value.Grade.ID).Equipment.EquipmentRemoved, AddressOf OnEquipmentRemoved
        End Set
    End Property
    Private Sub OnEquipmentAdded(ByVal equipmentItem As ModelGenerationGradeEquipmentItem)
        If Contains(equipmentItem.ID) Then Throw New ApplicationException("The item already exists in this collection")

        Dim carEquipmentItem As CarEquipmentItem = GetObject(equipmentItem)

        AllowNew = True
        ReferenceMap.Add(carEquipmentItem.ID, carEquipmentItem)
        Add(carEquipmentItem)
        AllowNew = False
    End Sub
    Private Sub OnEquipmentRemoved(ByVal equipmentItem As ModelGenerationGradeEquipmentItem)
        Dim carEquipmentItem As CarEquipmentItem = Me(equipmentItem.ID)

        AllowRemove = True
        carEquipmentItem.Remove()
        ReferenceMap.Remove(carEquipmentItem.ID)
        AllowRemove = False
    End Sub

    Default Public Overloads Overrides ReadOnly Property Item(ByVal id As Guid) As CarEquipmentItem
        Get
            If id.Equals(Guid.Empty) Then Return Nothing
            If Not ReferenceMap.ContainsKey(id) Then Return Nothing

            Return ReferenceMap.Item(id)
        End Get
    End Property
    Default Public Overloads ReadOnly Property Item(ByVal code As String) As CarEquipmentItem
        Get
            For Each carEquipmentItem As CarEquipmentItem In Me
                If carEquipmentItem.Equals(code) Then
                    Return carEquipmentItem
                End If
            Next
            Return Nothing
        End Get
    End Property


    Private ReadOnly Property ReferenceMap() As Dictionary(Of Guid, CarEquipmentItem)
        Get
            If _referenceMap Is Nothing Then
                _referenceMap = New Dictionary(Of Guid, CarEquipmentItem)(Count)
                For Each carEquipmentItem As CarEquipmentItem In Me
                    _referenceMap.Add(carEquipmentItem.ID, carEquipmentItem)
                Next
            End If
            Return _referenceMap
        End Get
    End Property


#End Region

#Region " Contains Methods "
    Public Overloads Overrides Function Contains(ByVal id As Guid) As Boolean
        Return ReferenceMap.ContainsKey(id)
    End Function
    Public Overloads Function Contains(ByVal obj As CarEquipmentItem) As Boolean
        Return ReferenceMap.ContainsKey(obj.ID)
    End Function
    Public Overloads Function Contains(ByVal accessory As Accessory) As Boolean
        Return ReferenceMap.ContainsKey(accessory.ID)
    End Function
    Public Overloads Function Contains(ByVal [option] As [Option]) As Boolean
        Return ReferenceMap.ContainsKey([option].ID)
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetEquipment(ByVal car As Car) As CarEquipment
        Dim carEquipment As CarEquipment = New CarEquipment
        Dim gradeEquipment As ModelGenerationGradeEquipment = car.Generation.Grades(car.GradeID).Equipment
        carEquipment.Car = car
        If car.IsNew Then
            carEquipment.Combine(gradeEquipment, Nothing)
        Else
            carEquipment.Combine(gradeEquipment, DataPortal.Fetch(Of CarEquipment)(New CustomCriteria(car)))
        End If

        Return carEquipment
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

        Public Sub New(ByVal car As Car)
            _carID = car.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CARID", _carID)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property
    Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As CarEquipmentItem
        Dim type = dataReader.GetEquipmentType("TYPE")
        Select Case type
            Case EquipmentType.Accessory
                Return CarAccessory.GetCarAccessory(dataReader)
            Case EquipmentType.Option
                Return CarOption.GetCarOption(dataReader)
            Case EquipmentType.ExteriorColourType
                Return CarExteriorColourType.GetCarExteriorColourType(dataReader)
            Case EquipmentType.UpholsteryType
                Return CarUpholsteryType.GetCarUpholsteryType(dataReader)
            Case Else
                Throw New Exceptions.InvalidEquipmentType("""" & type & """ is not a valid equipment type!")
        End Select
    End Function
    Private Overloads Shared Function GetObject(ByVal equipmentItem As ModelGenerationGradeEquipmentItem) As CarEquipmentItem
        Select Case equipmentItem.Type
            Case EquipmentType.Accessory
                Return CarAccessory.NewCarAccessory(DirectCast(equipmentItem, ModelGenerationGradeAccessory))
            Case EquipmentType.Option
                Return CarOption.NewCarOption(DirectCast(equipmentItem, ModelGenerationGradeOption))
            Case EquipmentType.ExteriorColourType
                Return CarExteriorColourType.NewCarExteriorColourType(DirectCast(equipmentItem, ModelGenerationGradeExteriorColourType))
            Case EquipmentType.UpholsteryType
                Return CarUpholsteryType.NewCarUpholsteryType(DirectCast(equipmentItem, ModelGenerationGradeUpholsteryType))
            Case Else
                Throw New Exceptions.InvalidEquipmentType("""" & equipmentItem.Type & """ is not a valid grade equipment type!")
        End Select
    End Function

    Private Sub OnBeforeUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.BeforeUpdateCommand
        'Clear the list of deleted objects. 
        'The database will take care of this for us via deleteGrenerationEquipmentItem
        DeletedList.Clear()
    End Sub


    Private Sub Combine(ByVal gradeEquipment As ModelGenerationGradeEquipment, ByVal carEquipment As CarEquipment)
        AllowNew = True

        Dim context = MyContext.GetContext()
        If Not context.EquipmentGroupsInitialized() OrElse context.LazyLoad Then
            context.LazyLoad = False
            context.EquipmentGroups = EquipmentGroups.GetEquipmentGroups(False)
        End If

        Dim groups As EquipmentGroups = context.EquipmentGroups
        Dim fittableGradeEquipment = (From x In gradeEquipment Where
                                        x.Type = EquipmentType.ExteriorColourType OrElse
                                        x.Type = EquipmentType.UpholsteryType OrElse
                                        groups.FindEquipment(x).CanBeFittedOn(Car)
                                        )

        For Each modelGenerationGradeEquipmentItem In fittableGradeEquipment
            If carEquipment IsNot Nothing AndAlso carEquipment.Contains(modelGenerationGradeEquipmentItem.ID) Then
                Dim carEquipmentItem As CarEquipmentItem = carEquipment(modelGenerationGradeEquipmentItem.ID)
                carEquipmentItem.GradeEquipmentItem = modelGenerationGradeEquipmentItem
                Add(carEquipmentItem)
            Else
                Add(GetObject(modelGenerationGradeEquipmentItem))
            End If
        Next
        AllowNew = False
    End Sub

    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        AllowNew = True 'make it possible to add the applicability ghosts

        FetchCarExteriorColourApplicabilities(dataReader)

        dataReader.NextResult()
        FetchCarUpholsteryApplicabilities(dataReader)

        dataReader.NextResult()
        FetchCarRules(dataReader) 'carequipmentitem item rules

        dataReader.NextResult()
        FetchCarRules(dataReader) 'carequipmentitem pack rules

        AllowNew = False
    End Sub

    Private Sub FetchCarExteriorColourApplicabilities(ByVal dataReader As SafeDataReader)
        While dataReader.Read
            Dim carOption As CarOption = GetCarOptionFromReaderOrAsGhost(dataReader)
            carOption.ExteriorColourApplicabilities.Add(dataReader)
        End While
    End Sub
    Private Sub FetchCarUpholsteryApplicabilities(ByVal dataReader As SafeDataReader)
        While dataReader.Read
            Dim carOption As CarOption = GetCarOptionFromReaderOrAsGhost(dataReader)
            carOption.UpholsteryApplicabilities.Add(dataReader)
        End While
    End Sub

    Private Function GetCarOptionFromReaderOrAsGhost(ByVal dataReader As SafeDataReader) As CarOption

        Dim equipmentType As EquipmentType = dataReader.GetEquipmentType("EQUIPMENTTYPE")
        If Not equipmentType = equipmentType.Option Then Throw New ApplicationException("Car applicabilities are only supported on options")


        Dim optionId As Guid = dataReader.GetGuid("EQUIPMENTID")
        Dim carOption As CarOption
        If Contains(optionId) Then
            carOption = DirectCast(Me(optionId), CarOption)
        Else
            carOption = DirectCast(AddGhost(optionId, equipmentType), CarOption)
        End If
        Return carOption
    End Function

    Private Sub FetchCarRules(ByVal dataReader As SafeDataReader)
        While dataReader.Read
            Dim equipmentId As Guid = dataReader.GetGuid("EQUIPMENTID")
            Dim carEquipmentItem = Me(equipmentId)

            If carEquipmentItem Is Nothing Then
                carEquipmentItem = AddGhost(equipmentId, dataReader.GetEquipmentType("EQUIPMENTTYPE"))
            End If
            carEquipmentItem.Rules.Add(dataReader)
        End While
    End Sub


    Private Function AddGhost(ByVal id As Guid, ByVal type As EquipmentType) As CarEquipmentItem
        Dim ghost As CarEquipmentItem = CarEquipmentItem.GetGhost(id, type)
        Add(ghost)
        ReferenceMap.Add(id, ghost)
        Return ghost
    End Function

#End Region

End Class

<Serializable()> Public MustInherit Class CarEquipmentItem
    Inherits ContextUniqueGuidBusinessBase(Of CarEquipmentItem)
    Implements IOwnedBy
    Implements IOverwritable
    Implements IMasterObjectReference

#Region " Business Properties & Methods "
    Private _overwritten As Boolean = False
    Private _availability As Availability

    Private _rules As CarEquipmentRules
    Private _ghost As Boolean = False

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

        SetRefProperties()
        Overwritten = False

        If IsNew Then
            MarkClean()
        End If

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

    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Overwritten") Then Return True
        Return MyBase.CanWriteProperty(propertyName)
    End Function

    Friend Sub Remove()
        AllowRemove = True
        DirectCast(Parent, CarEquipment).Remove(Me)
        AllowRemove = False
    End Sub

    Protected Overridable Sub SetRefProperties()
        _ghost = False 'it is not a ghost anymore once it has a reference object
        _availability = GradeEquipmentItem.Availability
    End Sub

    <XmlInfo(XmlNodeType.None)> Public MustOverride ReadOnly Property Type() As EquipmentType

    Public ReadOnly Property Rules() As CarEquipmentRules
        Get
            If _rules IsNot Nothing Then Return _rules

            _rules = CarEquipmentRules.NewRules(Me)
            Return _rules
        End Get
    End Property


#End Region

#Region " Reference Properties & Methods "
    Private _grade As ModelGenerationGrade
    Private _gradeEquipmentItem As ModelGenerationGradeEquipmentItem

    Public ReadOnly Property Car() As Car
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarEquipment).Car
        End Get
    End Property
    Private ReadOnly Property Grade() As ModelGenerationGrade
        Get
            If _grade Is Nothing Then
                If Car Is Nothing Then Return Nothing
                _grade = Car.Generation.Grades(Car.GradeID)
            End If
            Return _grade
        End Get
    End Property


    Friend Overridable Property GradeEquipmentItem() As ModelGenerationGradeEquipmentItem
        Get
            If _gradeEquipmentItem Is Nothing Then
                If Car Is Nothing Then Return Nothing
                _gradeEquipmentItem = Grade.Equipment(ID)
            End If
            Return _gradeEquipmentItem
        End Get
        Set(ByVal value As ModelGenerationGradeEquipmentItem)
            _gradeEquipmentItem = value
            If _ghost Then SetRefProperties()
            Rules.InheritedRules = value.GenerationEquipmentItem.Rules

            AddHandler value.PropertyChanged, AddressOf ParentPropertyChanged
        End Set
    End Property
    Private Sub ParentPropertyChanged(ByVal sender As Object, ByVal e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If e.PropertyName.Length = 0 Then Exit Sub 'In case the property name was not known, then I'm not intersested
        If Not Overwritten Then
            SetRefProperties()
        End If
    End Sub
    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return GradeEquipmentItem.ShortID
        End Get
    End Property
    Public ReadOnly Property LocalCode() As String
        Get
            Return GradeEquipmentItem.LocalCode
        End Get
    End Property
    Public ReadOnly Property PartNumber() As String
        Get
            Return GradeEquipmentItem.PartNumber
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return GradeEquipmentItem.Name
        End Get
    End Property
    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return GradeEquipmentItem.Owner
        End Get
    End Property
    Public ReadOnly Property Index() As Integer
        Get
            Return GradeEquipmentItem.Index
        End Get
    End Property
    Public ReadOnly Property GradeFeature() As Boolean
        Get
            Return GradeEquipmentItem.GradeFeature
        End Get
    End Property
    Public ReadOnly Property OptionalGradeFeature() As Boolean
        Get
            Return GradeEquipmentItem.OptionalGradeFeature
        End Get
    End Property

    Public ReadOnly Property KeyFeature() As Boolean
        Get
            If Car.SubModelID.Equals(Guid.Empty) Then
                Return GradeEquipmentItem.GenerationEquipmentItem.KeyFeature
            Else
                Return GradeEquipmentItem.GenerationEquipmentItem.Generation.SubModels(Car.SubModelID).Equipment(ID).KeyFeature
            End If
        End Get
    End Property
    Public ReadOnly Property SortPath() As String
        Get
            Return GradeEquipmentItem.SortPath
        End Get
    End Property

    Public ReadOnly Property Category() As EquipmentCategoryInfo
        Get
            Return GradeEquipmentItem.Category
        End Get
    End Property
    Public ReadOnly Property Group() As EquipmentGroupInfo
        Get
            Return GradeEquipmentItem.Group
        End Get
    End Property
    Public ReadOnly Property Colour() As ExteriorColourInfo
        Get
            Return GradeEquipmentItem.Colour
        End Get
    End Property
    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return GradeEquipmentItem.Translation
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return GradeEquipmentItem.AlternateName
        End Get
    End Property

    Public ReadOnly Property MasterID() As Guid Implements IMasterObjectReference.MasterID
        Get
            Return GradeEquipmentItem.MasterID
        End Get
    End Property
    Public ReadOnly Property MasterDescription() As String Implements IMasterObjectReference.MasterDescription
        Get
            Return GradeEquipmentItem.MasterDescription
        End Get
    End Property

    Public ReadOnly Property MasterType() As MasterEquipmentType
        Get
            Return GradeEquipmentItem.MasterType
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overrides Function ToString() As String
        Return Name
    End Function


    Public Overloads Function Equals(ByVal obj As ModelGenerationEquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeEquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As CarEquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As EquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As String) As Boolean
        Return Not (obj Is Nothing) AndAlso GradeEquipmentItem.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationEquipmentItem Then
            Return Equals(DirectCast(obj, ModelGenerationEquipmentItem))
        ElseIf TypeOf obj Is ModelGenerationGradeEquipmentItem Then
            Return Equals(DirectCast(obj, ModelGenerationGradeEquipmentItem))
        ElseIf TypeOf obj Is CarEquipmentItem Then
            Return Equals(DirectCast(obj, CarEquipmentItem))
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
#End Region

#Region "Shared Factory Methods"

    Friend Shared Function GetGhost(ByVal id As Guid, ByVal type As EquipmentType) As CarEquipmentItem
        Dim item As CarEquipmentItem
        If type = EquipmentType.Accessory Then
            item = New CarAccessory()
        Else
            item = New CarOption()
        End If

        item.Create(id)
        item._ghost = True
        item.MarkOld()
        Return item
    End Function
#End Region

#Region "Framework Overrides"
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_rules Is Nothing) AndAlso Not _rules.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_rules Is Nothing) AndAlso _rules.IsDirty Then Return True
            Return False
        End Get
    End Property



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
    Protected Overloads Sub Create(ByVal equipmenItem As ModelGenerationGradeEquipmentItem)
        Create(equipmenItem.ID)
        GradeEquipmentItem = equipmenItem
        SetRefProperties()
        MarkClean() 'These objects don't need be saved untill somebody actualy does something with them
    End Sub
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("EQUIPMENTID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        _availability = CType(dataReader.GetValue("AVAILABILITY"), Availability)
        _overwritten = True
        AllowEdit = True
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
        command.Parameters.AddWithValue("@CARID", Car.ID)
        command.Parameters.AddWithValue("@EQUIPMENTID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateCarEquipmentItem"
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If Not Overwritten Then
            command.CommandText = "deleteCarEquipmentItem"
            Exit Sub
        End If

        command.CommandText = "updateCarEquipmentItem"
        AddCommandFields(command)
    End Sub
    Protected Overridable Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@AVAILABILITY", Availability)
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If _rules IsNot Nothing Then _rules.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region

End Class

<Serializable(), XmlInfo("accessory")> Public NotInheritable Class CarAccessory
    Inherits CarEquipmentItem
    Implements IPrice

#Region " Business Properties & Methods "
    Private _fittingTimeNewCar As TimeSpan
    Private _fittingPriceNewCar As Decimal = 0D
    Private _fittingVatPriceNewCar As Decimal = 0D

    Private _fittingTimeExistingCar As TimeSpan
    Private _fittingPriceExistingCar As Decimal = 0D
    Private _fittingVatPriceExistingCar As Decimal = 0D


    Public Property FittingTimeNewCar() As TimeSpan
        Get
            Return _fittingTimeNewCar
        End Get
        Set(ByVal value As TimeSpan)
            If _fittingTimeNewCar.Equals(value) Then Return

            _fittingTimeNewCar = value
            PropertyHasChanged("FittingTimeNewCar")
        End Set
    End Property
    <XmlInfo(XmlNodeType.None)> Public Property FittingHoursNewCar() As Integer
        Get
            Return (_fittingTimeNewCar.Hours + (_fittingTimeNewCar.Days * 24))
        End Get
        Set(ByVal value As Integer)
            If FittingHoursNewCar = value Then Return

            _fittingTimeNewCar = New TimeSpan(value, _fittingTimeNewCar.Minutes, _fittingTimeNewCar.Seconds)
            PropertyHasChanged("FittingTimeNewCar")
        End Set
    End Property
    <XmlInfo(XmlNodeType.None)> Public Property FittingMinutesNewCar() As Integer
        Get
            Return _fittingTimeNewCar.Minutes
        End Get
        Set(ByVal value As Integer)
            If _fittingTimeNewCar.Minutes = value Then Return

            _fittingTimeNewCar = New TimeSpan(_fittingTimeNewCar.Hours, value, _fittingTimeNewCar.Seconds)
            PropertyHasChanged("FittingTimeNewCar")
        End Set
    End Property
    Public Property FittingPriceNewCar() As Decimal Implements IPrice.PriceExcludingVat
        Get
            Return _fittingPriceNewCar
        End Get
        Set(ByVal value As Decimal)
            If _fittingPriceNewCar.Equals(value) Then Return
            _fittingPriceNewCar = value
            PropertyHasChanged("FittingPriceNewCar")
        End Set
    End Property
    Public Property FittingVatPriceNewCar() As Decimal Implements IPrice.PriceIncludingVat
        Get
            Return _fittingVatPriceNewCar
        End Get
        Set(ByVal value As Decimal)
            If _fittingVatPriceNewCar.Equals(value) Then Return
            _fittingVatPriceNewCar = value
            PropertyHasChanged("FittingVatPriceNewCar")
        End Set
    End Property

    Public Property FittingTimeExistingCar() As TimeSpan
        Get
            Return _fittingTimeExistingCar
        End Get
        Set(ByVal value As TimeSpan)
            If _fittingTimeExistingCar.Equals(value) Then Return
            _fittingTimeExistingCar = value
            PropertyHasChanged("FittingTimeExistingCar")
        End Set
    End Property
    <XmlInfo(XmlNodeType.None)> Public Property FittingHoursExistingCar() As Integer
        Get
            Return (_fittingTimeExistingCar.Hours + (_fittingTimeExistingCar.Days * 24))
        End Get
        Set(ByVal value As Integer)
            If FittingHoursExistingCar = value Then Return
            _fittingTimeExistingCar = New TimeSpan(value, _fittingTimeExistingCar.Minutes, _fittingTimeExistingCar.Seconds)
            PropertyHasChanged("FittingTimeExistingCar")
        End Set
    End Property
    <XmlInfo(XmlNodeType.None)> Public Property FittingMinutesExistingCar() As Integer
        Get
            Return _fittingTimeExistingCar.Minutes
        End Get
        Set(ByVal value As Integer)
            If _fittingTimeExistingCar.Minutes = value Then Return
            _fittingTimeExistingCar = New TimeSpan(_fittingTimeExistingCar.Hours, value, _fittingTimeExistingCar.Seconds)
            PropertyHasChanged("FittingTimeExistingCar")
        End Set
    End Property
    Public Property FittingPriceExistingCar() As Decimal
        Get
            Return _fittingPriceExistingCar
        End Get
        Set(ByVal value As Decimal)
            If _fittingPriceExistingCar.Equals(value) Then Return
            _fittingPriceExistingCar = value
            PropertyHasChanged("FittingPriceExistingCar")
        End Set
    End Property
    Public Property FittingVatPriceExistingCar() As Decimal
        Get
            Return _fittingVatPriceExistingCar
        End Get
        Set(ByVal value As Decimal)
            If _fittingVatPriceExistingCar.Equals(value) Then Return
            _fittingVatPriceExistingCar = value
            PropertyHasChanged("FittingVatPriceExistingCar")
        End Set
    End Property

    Protected Overrides Sub SetRefProperties()
        MyBase.SetRefProperties()
        With DirectCast(GradeEquipmentItem, ModelGenerationGradeAccessory)
            _fittingTimeNewCar = .FittingTimeNewCar
            _fittingPriceNewCar = .FittingPriceNewCar
            _fittingVatPriceNewCar = .FittingVatPriceNewCar

            _fittingTimeExistingCar = .FittingTimeExistingCar
            _fittingPriceExistingCar = .FittingPriceExistingCar
            _fittingVatPriceExistingCar = .FittingVatPriceExistingCar
        End With
    End Sub

    <XmlInfo(XmlNodeType.None)> Public Overrides ReadOnly Property Type() As EquipmentType
        Get
            Return EquipmentType.Accessory
        End Get
    End Property

#End Region

#Region " Reference Properties & Methods "

    Public ReadOnly Property BasePrice() As Decimal
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeAccessory).BasePrice
        End Get
    End Property
    Public ReadOnly Property BaseVatPrice() As Decimal
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeAccessory).BaseVatPrice
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Function Equals(ByVal obj As Accessory) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID) OrElse Equals(obj.PartNumber))
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationAccessory) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeAccessory) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function
    Public Overloads Function Equals(ByVal obj As CarAccessory) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewCarAccessory(ByVal accessory As ModelGenerationGradeAccessory) As CarAccessory
        Dim carAccessory As CarAccessory = New CarAccessory()
        carAccessory.Create(accessory)
        Return carAccessory
    End Function
    Friend Shared Function GetCarAccessory(ByVal dataReader As SafeDataReader) As CarAccessory
        Dim carAccessory As CarAccessory = New CarAccessory()
        carAccessory.Fetch(dataReader)
        Return carAccessory
    End Function
#End Region

#Region " Data Access "


    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _fittingTimeNewCar = New TimeSpan(0, Decimal.ToInt32(CType(.GetValue("FITTINGTIMENEW"), Decimal)), 0)
            _fittingPriceNewCar = Environment.ConvertPrice(CType(.GetValue("FITTINGPRICENEW"), Decimal), .GetString("FITTINGCURRENCY"))
            _fittingVatPriceNewCar = Environment.ConvertPrice(CType(.GetValue("FITTINGPRICEVATNEW"), Decimal), .GetString("FITTINGCURRENCY"))

            _fittingTimeExistingCar = New TimeSpan(0, Decimal.ToInt32(CType(.GetValue("FITTINGTIMEEXISTING"), Decimal)), 0)
            _fittingPriceExistingCar = Environment.ConvertPrice(CType(.GetValue("FITTINGPRICEEXISTING"), Decimal), .GetString("FITTINGCURRENCY"))
            _fittingVatPriceExistingCar = Environment.ConvertPrice(CType(.GetValue("FITTINGPRICEVATEXISTING"), Decimal), .GetString("FITTINGCURRENCY"))
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddCommandFields(command)
        With command
            .Parameters.AddWithValue("@FITTINGCURRENCY", MyContext.GetContext().Currency.Code)
            .Parameters.AddWithValue("@FITTINGTIMENEW", FittingTimeNewCar.TotalMinutes())
            .Parameters.AddWithValue("@FITTINGPRICENEW", FittingPriceNewCar)
            .Parameters.AddWithValue("@FITTINGPRICEVATNEW", FittingVatPriceNewCar)
            .Parameters.AddWithValue("@FITTINGTIMEEXISTING", FittingTimeExistingCar.TotalMinutes())
            .Parameters.AddWithValue("@FITTINGPRICEEXISTING", FittingPriceExistingCar)
            .Parameters.AddWithValue("@FITTINGPRICEVATEXISTING", FittingVatPriceExistingCar)
        End With
    End Sub

#End Region

End Class
<Serializable(), XmlInfo("option")> Public NotInheritable Class CarOption
    Inherits CarEquipmentItem
    Implements IPrice
    Implements IMasterPathObjectReference

#Region " Business Properties & Methods "
    Private _suffixAvailability As Availability
    Private _fittingPrice As Decimal = 0D
    Private _fittingVatPrice As Decimal = 0D

    Private _exteriorColourApplicabilities As CarExteriorColourApplicabilities
    Private _upholsteryApplicabilities As CarUpholsteryApplicabilities


    Public Property SuffixAvailability() As Availability
        Get
            Return _suffixAvailability
        End Get
        Set(ByVal value As Availability)
            If _suffixAvailability.Equals(value) Then Return
            _suffixAvailability = value

            If AvailableExclusivelyThroughAPack() AndAlso Availability.Equals(Availability.NotAvailable) Then
                'do nothing
            Else
                Availability = _suffixAvailability
            End If

            If OfferedThroughAPack Then
                RecalculateAllCarPackAvailabilities()
            End If

            PropertyHasChanged("SuffixAvailability")
        End Set
    End Property

    Private Function AvailableExclusivelyThroughAPack() As Boolean

    End Function

    Public Property FittingPrice() As Decimal Implements IPrice.PriceExcludingVat
        Get
            Return _fittingPrice
        End Get
        Set(ByVal value As Decimal)
            If _fittingPrice.Equals(value) Then Return

            _fittingPrice = value
            PropertyHasChanged("FittingPrice")
        End Set
    End Property
    Public Property FittingVatPrice() As Decimal Implements IPrice.PriceIncludingVat
        Get
            Return _fittingVatPrice
        End Get
        Set(ByVal value As Decimal)
            If _fittingVatPrice.Equals(value) Then Return

            _fittingVatPrice = value
            PropertyHasChanged("FittingVatPrice")
        End Set
    End Property

    Friend Overrides Property GradeEquipmentItem As ModelGenerationGradeEquipmentItem
        Get
            Return MyBase.GradeEquipmentItem
        End Get
        Set(ByVal value As ModelGenerationGradeEquipmentItem)
            MyBase.GradeEquipmentItem = value

            Dim modelGenerationOption = DirectCast(value.GenerationEquipmentItem, ModelGenerationOption)
            ExteriorColourApplicabilities.InheritedExteriorColourApplicabilities = modelGenerationOption.ExteriorColourApplicabilities
            UpholsteryApplicabilities.InheritedUpholsteryApplicabilities = modelGenerationOption.UpholsteryApplicabilities
        End Set
    End Property


    Public ReadOnly Property ExteriorColourApplicabilities As CarExteriorColourApplicabilities
        Get
            If _exteriorColourApplicabilities IsNot Nothing Then Return _exteriorColourApplicabilities

            _exteriorColourApplicabilities = CarExteriorColourApplicabilities.NewApplicabilities(Me)
            Return _exteriorColourApplicabilities
        End Get
    End Property
    Public ReadOnly Property UpholsteryApplicabilities As CarUpholsteryApplicabilities
        Get
            If _upholsteryApplicabilities IsNot Nothing Then Return _upholsteryApplicabilities

            _upholsteryApplicabilities = CarUpholsteryApplicabilities.NewApplicabilities(Me)
            Return _upholsteryApplicabilities
        End Get
    End Property

    <XmlInfo(XmlNodeType.None)> Public Overrides ReadOnly Property Type() As EquipmentType
        Get
            Return EquipmentType.Option
        End Get
    End Property

    Protected Overrides Sub SetRefProperties()
        MyBase.SetRefProperties()
        With DirectCast(GradeEquipmentItem, ModelGenerationGradeOption)
            _suffixAvailability = .SuffixAvailability
            _fittingPrice = .FittingPrice
            _fittingVatPrice = .FittingVatPrice
        End With
    End Sub

    Public Overrides Sub Revert()
        MyBase.Revert()
        RevalidateAvailability
    End Sub

    <XmlInfo(XmlNodeType.Attribute)> Public Overrides Property Availability() As Availability
        Get
            Return MyBase.Availability
        End Get
        Set(ByVal value As Availability)
            If value.Equals(Availability) Then Return
            MyBase.Availability = value
            RecalculateParentAvailability()
            RevalidateAvailability()
        End Set
    End Property


    Public ReadOnly Property HasParentOption() As Boolean
        Get
            Return ParentOption IsNot Nothing
        End Get
    End Property

    Public ReadOnly Property HasChildOptions() As Boolean
        Get
            Return ChildOptions.Any()
        End Get
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


    Private Sub RevalidateAvailability()
        ValidationRules.CheckRules("Availability")
        RevalidateParentAvailability()
        RevalidateChildAvailability()
    End Sub

    Private Sub RevalidateParentAvailability()
        If Not HasParentOption Then Return

        ParentOption.ValidationRules.CheckRules("Availability")
    End Sub
    Private Sub RevalidateChildAvailability()
        If Not HasChildOptions Then Return

        For Each carOption In ChildOptions
            carOption.ValidationRules.CheckRules("Availability")
        Next
    End Sub

    Friend Sub OfferThrough(ByVal carPack As CarPack)
        carPack.Overwrite()

        carPack.RecalculateAvailability()
        carPack.Equipment.RecalculateAvailabilities()
    End Sub

    Public ReadOnly Property OfferedThroughAPack() As Boolean
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).OfferedThrough.Any()
        End Get
    End Property

    Public ReadOnly Property ExclusivelyOfferedThroughAPack() As Boolean
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).ExclusivelyOfferedThroughAPack
        End Get
    End Property

    Private Sub RecalculateAllCarPackAvailabilities()
        For Each modelGenerationGradeOptionPackRelation As ModelGenerationGradeOptionPackRelation In DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).OfferedThrough
            Car.Packs(modelGenerationGradeOptionPackRelation.ID).Overwrite()
            Car.Packs(modelGenerationGradeOptionPackRelation.ID).RecalculateAvailability()
            Car.Packs(modelGenerationGradeOptionPackRelation.ID).Equipment.RecalculateAvailabilities()
        Next
    End Sub

#End Region

#Region " Suffix & FactoryOptions related methods "

    Private _suffixes As List(Of CarSuffix)
    Public Function GetSuffixes() As List(Of CarSuffix)
        If Not _suffixes Is Nothing Then Return _suffixes

        If SuffixOption Then
            _suffixes = Car.Suffixes.Where(Function(suffix) GetMappedFactoryOptions(suffix).Any()).ToList()
        Else
            _suffixes = New List(Of CarSuffix)
        End If
        Return _suffixes
    End Function

    Public Function GetMappedFactoryOptions(ByVal carSuffix As CarSuffix) As List(Of FactoryGenerationOptionValueInfo)
        If Not SuffixOption Then Return New List(Of FactoryGenerationOptionValueInfo)()

        If FactoryOptions.ContainsKey(carSuffix.ID) Then Return FactoryOptions(carSuffix.ID)

        Dim result = QueryFactoryOptions(carSuffix)
        FactoryOptions.Add(carSuffix.ID, result)
        Return result

    End Function

#Region " Query Functions "

    Private Function QueryFactoryOptions(ByVal carSuffix As CarSuffix) As List(Of FactoryGenerationOptionValueInfo)

        Dim mappedLines = (From line As OptionMappingLine In carSuffix.GetOptionMapping()
                             Where line.Option.Equals(Me) AndAlso
                                (line.ExteriorColourCode.Length = 0 OrElse carSuffix.HasExteriorColour(line.ExteriorColourCode)) AndAlso
                                Not line.Parked AndAlso
                                Not line.MarketingIrrelevant
                            Select line)

        Dim allExteriorColoursAreMapped = carSuffix.GetSuffix().ColourCombinations.All(Function(colourCombination) mappedLines.HasExteriorColour(colourCombination.ExteriorColour.Code))
        Return (From line In mappedLines
                Join so In carSuffix.GetSuffix().Options On line.FactoryOptionValue.ID Equals so.FactoryOptionValue.ID
                    Where Not allExteriorColoursAreMapped OrElse line.ExteriorColourCode.Length > 0
                Select line.FactoryOptionValue).Distinct().ToList()

    End Function

#End Region

#Region " Backing fields "
    Private _factoryOptions As Dictionary(Of Guid, List(Of FactoryGenerationOptionValueInfo))

    Private ReadOnly Property FactoryOptions() As Dictionary(Of Guid, List(Of FactoryGenerationOptionValueInfo))
        Get
            If _factoryOptions Is Nothing Then _factoryOptions = New Dictionary(Of Guid, List(Of FactoryGenerationOptionValueInfo))
            Return _factoryOptions
        End Get
    End Property
#End Region




#End Region

#Region " Reference Properties & Methods "

    Public ReadOnly Property SuffixOption() As Boolean
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).SuffixOption
        End Get
    End Property
    Public ReadOnly Property Visible() As Boolean
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).Visible
        End Get
    End Property

    Public ReadOnly Property Code() As String
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).Code
        End Get
    End Property


    Public Function IsReplaced() As Boolean
        Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).IsReplaced
    End Function
    Public ReadOnly Property ReplacedBy() As ModelGenerationGradeOption
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).ReplacedBy
        End Get
    End Property
    Public Function IsReplacement() As Boolean
        Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).IsReplacement
    End Function
    Public ReadOnly Property ReplacementFor() As IEnumerable(Of ModelGenerationGradeOption)
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).ReplacementFor
        End Get
    End Property
    Public ReadOnly Property MasterPath() As String Implements IMasterPathObjectReference.MasterPath
        Get
            Return DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).MasterPath
        End Get
    End Property

    Public ReadOnly Property ParentOption() As CarOption
        Get
            Dim generationGradeOption As ModelGenerationGradeOption = DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).ParentOption
            If generationGradeOption Is Nothing Then Return Nothing
            Return DirectCast(Car.Equipment(generationGradeOption.ID), CarOption)
        End Get
    End Property

    Public ReadOnly Property ChildOptions() As IEnumerable(Of CarOption)
        Get
            Dim childOptionIDs = DirectCast(GradeEquipmentItem, ModelGenerationGradeOption).ChildOptions.Select(Function(x) x.ID)

            Return Car.Equipment.OfType(Of CarOption).Where(Function(x) childOptionIDs.Contains(x.ID))
        End Get
    End Property

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        MyBase.AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf AvailabilityRule, Validation.RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeStandardIfNonOfTheChildOptionsAreStandard, Validation.RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeOptionalIfNonOfTheChildOptionsAreOptional, Validation.RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeNotAvailableIfNotAllChildOptionsNotAvailable, Validation.RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeStandardIfParentIsNotStandard, Validation.RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeOptionalIfParentIsNotOptionalOrStandard, Validation.RuleHandler), "Availability")

    End Sub
    Private Shared Function AvailabilityRule(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carOption As CarOption = DirectCast(target, CarOption)
        If carOption.GradeEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option

        If Not carOption.SuffixOption Then Return True
        If carOption.Availability.Equals(carOption.SuffixAvailability) Then Return True

        If carOption.ExclusivelyOfferedThroughAPack AndAlso carOption.Availability = Enums.Availability.NotAvailable Then Return True
        If carOption.ReplacedBy IsNot Nothing AndAlso carOption.Availability = Enums.Availability.NotAvailable Then Return True

        e.Description = String.Format("You can not change the availability of the suffix option ""{0}"" from {1} to {2}", carOption.Name, carOption.SuffixAvailability, carOption.Availability)
        Return False

    End Function
    Private Shared Function OptionCanNotBeStandardIfNonOfTheChildOptionsAreStandard(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carOption As CarOption = DirectCast(target, CarOption)
        If carOption.GradeEquipmentItem Is Nothing Then Return True 'Rule can not be checked yet
        If Not carOption.Availability = Enums.Availability.Standard Then Return True
        If Not carOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made standard because at least one child option needs to be standard as well", carOption.AlternateName)

        Return carOption.ChildOptions.Any(Function(x) x.Availability = Enums.Availability.Standard)
    End Function
    Private Shared Function OptionCanNotBeOptionalIfNonOfTheChildOptionsAreOptional(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carOption As CarOption = DirectCast(target, CarOption)
        If carOption.GradeEquipmentItem Is Nothing Then Return True 'Rule can not be checked yet
        If Not carOption.Availability = Enums.Availability.Optional Then Return True
        If Not carOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made optional unless all child options are optional", carOption.AlternateName)

        Return carOption.ChildOptions.All(Function(x) x.Availability = Enums.Availability.Optional OrElse x.Availability = Enums.Availability.NotAvailable)
    End Function
    Private Shared Function OptionCanNotBeNotAvailableIfNotAllChildOptionsNotAvailable(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carOption As CarOption = DirectCast(target, CarOption)
        If carOption.GradeEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not carOption.Availability = Enums.Availability.NotAvailable Then Return True
        If Not carOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made unavailable if one ore more child options are optional or standard", carOption.AlternateName)

        Return carOption.ChildOptions.All(Function(x) x.Availability = Enums.Availability.NotAvailable)
    End Function
    Private Shared Function OptionCanNotBeStandardIfParentIsNotStandard(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carOption As CarOption = DirectCast(target, CarOption)
        If carOption.GradeEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not carOption.Availability = Enums.Availability.Standard Then Return True
        If Not carOption.HasParentOption Then Return True
        e.Description = String.Format("The option {0} can not be made Standard if its parent is not Standard.", carOption.AlternateName)

        Return carOption.ParentOption.Availability = Enums.Availability.Standard
    End Function
    Private Shared Function OptionCanNotBeOptionalIfParentIsNotOptionalOrStandard(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carOption As CarOption = DirectCast(target, CarOption)
        If carOption.GradeEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not carOption.Availability = Enums.Availability.Optional Then Return True
        If Not carOption.HasParentOption Then Return True
        e.Description = String.Format("The option {0} can not be made Optional if its parent is Not Available.", carOption.AlternateName)

        Return carOption.ParentOption.Availability = Enums.Availability.Optional OrElse carOption.ParentOption.Availability = Enums.Availability.Standard
    End Function
#End Region

#Region " System.Object Overrides "
    Public Overloads Function Equals(ByVal obj As [Option]) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As CarOption) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
#End Region

#Region "Framework Overrides"
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_exteriorColourApplicabilities Is Nothing) AndAlso Not _exteriorColourApplicabilities.IsValid Then Return False
            If Not (_upholsteryApplicabilities Is Nothing) AndAlso Not _upholsteryApplicabilities.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_exteriorColourApplicabilities Is Nothing) AndAlso _exteriorColourApplicabilities.IsDirty Then Return True
            If Not (_upholsteryApplicabilities Is Nothing) AndAlso _upholsteryApplicabilities.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewCarOption(ByVal [option] As [ModelGenerationGradeOption]) As CarOption
        Dim carOption As CarOption = New CarOption
        carOption.Create([option])
        Return carOption
    End Function
    Friend Shared Function GetCarOption(ByVal dataReader As SafeDataReader) As CarOption
        Dim carOption As CarOption = New CarOption()
        carOption.Fetch(dataReader)
        Return carOption
    End Function
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            If .IsDBNull("SUFFIXAVAILABILITY") Then
                _suffixAvailability = Availability.NotAvailable
            Else
                _suffixAvailability = CType(.GetValue("SUFFIXAVAILABILITY"), Availability)
            End If
            _fittingPrice = Environment.ConvertPrice(CType(.GetValue("FITTINGPRICENEW"), Decimal), .GetString("FITTINGCURRENCY"))
            _fittingVatPrice = Environment.ConvertPrice(CType(.GetValue("FITTINGPRICEVATNEW"), Decimal), .GetString("FITTINGCURRENCY"))
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddCommandFields(command)
        With command
            .Parameters.AddWithValue("@SUFFIXAVAILABILITY", SuffixAvailability)
            .Parameters.AddWithValue("@FITTINGCURRENCY", MyContext.GetContext().Currency.Code)
            .Parameters.AddWithValue("@FITTINGPRICENEW", FittingPrice)
            .Parameters.AddWithValue("@FITTINGPRICEVATNEW", FittingVatPrice)
        End With
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _exteriorColourApplicabilities Is Nothing Then _exteriorColourApplicabilities.Update(transaction)
        If Not _upholsteryApplicabilities Is Nothing Then _upholsteryApplicabilities.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region
End Class
<Serializable(), XmlInfo("exteriorcolourtype")> Public NotInheritable Class CarExteriorColourType
    Inherits CarEquipmentItem
#Region "Business Properties & Methods"
    <XmlInfo(XmlNodeType.None)> Public Overrides ReadOnly Property Type() As EquipmentType
        Get
            Return EquipmentType.ExteriorColourType
        End Get
    End Property
#End Region
#Region " System.Object Overrides "

    Public Overloads Function Equals(ByVal obj As ExteriorColourType) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationExteriorColourType) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeExteriorColourType) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function
    Public Overloads Function Equals(ByVal obj As CarExteriorColourType) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewCarExteriorColourType(ByVal type As ModelGenerationGradeExteriorColourType) As CarExteriorColourType
        Dim carExteriorColourType As CarExteriorColourType = New CarExteriorColourType
        carExteriorColourType.Create(type)
        Return carExteriorColourType
    End Function
    Friend Shared Function GetCarExteriorColourType(ByVal dataReader As SafeDataReader) As CarExteriorColourType
        Dim carExteriorColourType As CarExteriorColourType = New CarExteriorColourType()
        carExteriorColourType.Fetch(dataReader)
        Return carExteriorColourType
    End Function
#End Region

End Class
<Serializable(), XmlInfo("upholsterytype")> Public NotInheritable Class CarUpholsteryType
    Inherits CarEquipmentItem

#Region "Business Properties & Methods"
    <XmlInfo(XmlNodeType.None)> Public Overrides ReadOnly Property Type() As EquipmentType
        Get
            Return EquipmentType.UpholsteryType
        End Get
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Function Equals(ByVal obj As UpholsteryType) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationUpholsteryType) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeUpholsteryType) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function
    Public Overloads Function Equals(ByVal obj As CarUpholsteryType) As Boolean
        Return Not (obj Is Nothing) AndAlso (Equals(obj.ID))
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewCarUpholsteryType(ByVal type As ModelGenerationGradeUpholsteryType) As CarUpholsteryType
        Dim carUpholsteryType As CarUpholsteryType = New CarUpholsteryType
        carUpholsteryType.Create(type)
        Return carUpholsteryType
    End Function
    Friend Shared Function GetCarUpholsteryType(ByVal dataReader As SafeDataReader) As CarUpholsteryType
        Dim carUpholsteryType As CarUpholsteryType = New CarUpholsteryType()
        carUpholsteryType.Fetch(dataReader)
        Return carUpholsteryType
    End Function
#End Region
End Class