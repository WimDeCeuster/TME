Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Exceptions
Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.Validation

<Serializable()> Public NotInheritable Class ModelGenerationGradeEquipment
    Inherits ContextUniqueGuidListBase(Of ModelGenerationGradeEquipment, ModelGenerationGradeEquipmentItem)

#Region " Delegates & Events "
    Friend Delegate Sub EquipmentChangedHandler(ByVal item As ModelGenerationGradeEquipmentItem)
    Friend Event EquipmentAdded As EquipmentChangedHandler
    Friend Event EquipmentRemoved As EquipmentChangedHandler
#End Region

#Region " Business Properties & Methods "

    <NonSerialized()> Private _referenceMap As Dictionary(Of Guid, ModelGenerationGradeEquipmentItem)

    Friend Property Grade() As ModelGenerationGrade
        Get
            Return DirectCast(Parent, ModelGenerationGrade)
        End Get
        Private Set(ByVal value As ModelGenerationGrade)
            SetParent(value)
            AddHandler value.Generation.Equipment.EquipmentAdded, AddressOf OnEquipmentAdded
            AddHandler value.Generation.Equipment.EquipmentRemoved, AddressOf OnEquipmentRemoved
        End Set
    End Property
    Private Sub OnEquipmentAdded(ByVal equipmentItem As ModelGenerationEquipmentItem)
        If Contains(equipmentItem.ID) Then Throw New ApplicationException("The item already exists in this collection")

        Dim gradeEquipmentItem As ModelGenerationGradeEquipmentItem = GetObject(equipmentItem)

        AllowNew = True
        ReferenceMap.Add(gradeEquipmentItem.ID, gradeEquipmentItem)
        Add(gradeEquipmentItem)
        AllowNew = False

        RaiseEvent EquipmentAdded(gradeEquipmentItem)
    End Sub
    Private Sub OnEquipmentRemoved(ByVal equipmentItem As ModelGenerationEquipmentItem)
        Dim gradeEquipmentItem As ModelGenerationGradeEquipmentItem = Me(equipmentItem.ID)

        AllowRemove = True
        gradeEquipmentItem.Remove()
        ReferenceMap.Remove(gradeEquipmentItem.ID)
        AllowRemove = False

        RaiseEvent EquipmentRemoved(gradeEquipmentItem)
    End Sub

    Default Public Overloads Overrides ReadOnly Property Item(ByVal id As Guid) As ModelGenerationGradeEquipmentItem
        Get
            If id.Equals(Guid.Empty) Then Return Nothing
            If ReferenceMap.ContainsKey(id) Then
                Return ReferenceMap.Item(id)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Private ReadOnly Property ReferenceMap() As Dictionary(Of Guid, ModelGenerationGradeEquipmentItem)
        Get
            If _referenceMap Is Nothing Then
                _referenceMap = New Dictionary(Of Guid, ModelGenerationGradeEquipmentItem)(Count)
                For Each gradeEquipmentItem In Me
                    _referenceMap.Add(gradeEquipmentItem.ID, gradeEquipmentItem)
                Next
            End If
            Return _referenceMap
        End Get
    End Property

#End Region

#Region " Copy Method "
    Friend Function Copy(ByVal newGrade As ModelGenerationGrade) As ModelGenerationGradeEquipment
        Dim newGradeEquipment As ModelGenerationGradeEquipment = Clone()
        newGradeEquipment.Grade = newGrade
        Return newGradeEquipment
    End Function
#End Region

#Region " Contains Methods "
    Public Overloads Overrides Function Contains(ByVal id As Guid) As Boolean
        Return ReferenceMap.ContainsKey(id)
    End Function
    Public Overloads Function Contains(ByVal obj As ModelGenerationGradeEquipmentItem) As Boolean
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

    Friend Shared Function GetEquipment(ByVal grade As ModelGenerationGrade) As ModelGenerationGradeEquipment
        Dim gradeEquipment As ModelGenerationGradeEquipment = New ModelGenerationGradeEquipment
        If grade.IsNew Then
            gradeEquipment.Combine(grade.Generation.Equipment, Nothing)
        Else
            gradeEquipment.Combine(grade.Generation.Equipment, DataPortal.Fetch(Of ModelGenerationGradeEquipment)(New CustomCriteria(grade)))
        End If
        gradeEquipment.Grade = grade
        Return gradeEquipment
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

        Private ReadOnly _generationID As Guid
        Private ReadOnly _gradeID As Guid

        Public Sub New(ByVal grade As ModelGenerationGrade)
            _generationID = grade.Generation.ID
            _gradeID = grade.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
            command.Parameters.AddWithValue("@GRADEID", _gradeID)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property
    Protected Overloads Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As ModelGenerationGradeEquipmentItem
        Dim type As EquipmentType = dataReader.GetEquipmentType("TYPE")
        Select Case type
            Case EquipmentType.Accessory
                Return ModelGenerationGradeAccessory.GetModelGenerationGradeAccessory(dataReader)
            Case EquipmentType.Option
                Return ModelGenerationGradeOption.GetModelGenerationGradeOption(dataReader)
            Case EquipmentType.ExteriorColourType
                Return ModelGenerationGradeExteriorColourType.GetModelGenerationGradeExteriorColourType(dataReader)
            Case EquipmentType.UpholsteryType
                Return ModelGenerationGradeUpholsteryType.GetModelGenerationGradeUpholsteryType(dataReader)
            Case Else
                Throw New InvalidEquipmentType("""" & type & """ is not a valid equipment type!")
        End Select
    End Function
    Private Overloads Shared Function GetObject(ByVal equipmentItem As ModelGenerationEquipmentItem) As ModelGenerationGradeEquipmentItem
        Select Case equipmentItem.Type
            Case EquipmentType.Accessory
                Return ModelGenerationGradeAccessory.NewModelGenerationGradeAccessory(DirectCast(equipmentItem, ModelGenerationAccessory))
            Case EquipmentType.Option
                Return ModelGenerationGradeOption.NewModelGenerationGradeOption(DirectCast(equipmentItem, ModelGenerationOption))
            Case EquipmentType.ExteriorColourType
                Return ModelGenerationGradeExteriorColourType.NewModelGenerationGradeExteriorColourType(DirectCast(equipmentItem, ModelGenerationExteriorColourType))
            Case EquipmentType.UpholsteryType
                Return ModelGenerationGradeUpholsteryType.NewModelGenerationGradeUpholsteryType(DirectCast(equipmentItem, ModelGenerationUpholsteryType))
            Case Else
                Throw New InvalidEquipmentType("""" & equipmentItem.Type & """ is not a valid grade equipment type!")
        End Select
    End Function

    Private Sub OnBeforeUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.BeforeUpdateCommand
        'Clear the list of deleted objects. 
        'These database will take care of this for us via deleteGrenerationEquipmentItem
        DeletedList.Clear()
    End Sub


    Private Sub Combine(ByVal generationEquipment As IEnumerable(Of ModelGenerationEquipmentItem), ByVal gradeEquipment As ModelGenerationGradeEquipment)
        AllowNew = True
        For Each generationEquipmentItem As ModelGenerationEquipmentItem In generationEquipment
            If gradeEquipment IsNot Nothing AndAlso gradeEquipment.Contains(generationEquipmentItem.ID) Then
                Add(gradeEquipment(generationEquipmentItem.ID))
            Else
                Add(GetObject(generationEquipmentItem))
            End If
        Next
        AllowNew = False
    End Sub
#End Region


End Class
<Serializable()> Public MustInherit Class ModelGenerationGradeEquipmentItem
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationGradeEquipmentItem)
    Implements IOwnedBy

#Region " Business Properties & Methods "
    Private _gradeFeature As Boolean = False
    Private _optionalGradeFeature As Boolean = False
    Private _availability As Availability

    <XmlInfo(XmlNodeType.Attribute)> Public Overridable Property GradeFeature() As Boolean
        Get
            Return _gradeFeature
        End Get
        Set(ByVal value As Boolean)
            If GradeFeature = value Then Return

            _gradeFeature = value
            PropertyHasChanged("GradeFeature")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property OptionalGradeFeature() As Boolean
        Get
            Return _optionalGradeFeature
        End Get
        Set(ByVal value As Boolean)
            If OptionalGradeFeature = value Then Return

            _optionalGradeFeature = value
            PropertyHasChanged("OptionalGradeFeature")
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

    Friend Sub Remove()
        AllowRemove = True
        DirectCast(Parent, ModelGenerationGradeEquipment).Remove(Me)
        AllowRemove = False
    End Sub

#End Region

#Region " Reference Properties & Methods "
    Private _generationEquipmentItem As ModelGenerationEquipmentItem

    Public ReadOnly Property Grade() As ModelGenerationGrade
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationGradeEquipment).Grade
        End Get
    End Property
    Protected Friend ReadOnly Property GenerationEquipmentItem() As ModelGenerationEquipmentItem
        Get
            If _generationEquipmentItem Is Nothing Then
                If Grade Is Nothing Then Return Nothing
                _generationEquipmentItem = Grade.Generation.Equipment(ID)
            End If

            Return _generationEquipmentItem
        End Get
    End Property

    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return GenerationEquipmentItem.ShortID
        End Get
    End Property

    Public ReadOnly Property Type() As EquipmentType
        Get
            Return GenerationEquipmentItem.Type
        End Get
    End Property

    Public ReadOnly Property LocalCode() As String
        Get
            Return GenerationEquipmentItem.LocalCode
        End Get
    End Property
    Public ReadOnly Property PartNumber() As String
        Get
            Return GenerationEquipmentItem.PartNumber
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return GenerationEquipmentItem.Name
        End Get
    End Property
    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return GenerationEquipmentItem.Owner
        End Get
    End Property
    Public ReadOnly Property Index() As Integer
        Get
            Return GenerationEquipmentItem.Index
        End Get
    End Property


    Public ReadOnly Property SortPath() As String
        Get
            Return GenerationEquipmentItem.SortPath
        End Get
    End Property


    Public ReadOnly Property Category() As EquipmentCategoryInfo
        Get
            Return GenerationEquipmentItem.Category
        End Get
    End Property
    Public ReadOnly Property Group() As EquipmentGroupInfo
        Get
            Return GenerationEquipmentItem.Group
        End Get
    End Property
    Public ReadOnly Property Colour() As ExteriorColourInfo
        Get
            Return GenerationEquipmentItem.Colour
        End Get
    End Property

    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return GenerationEquipmentItem.Translation
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return GenerationEquipmentItem.AlternateName
        End Get
    End Property

    Public ReadOnly Property MasterID() As Guid
        Get
            Return GenerationEquipmentItem.MasterID
        End Get
    End Property
    Public ReadOnly Property MasterDescription() As String
        Get
            Return GenerationEquipmentItem.MasterDescription
        End Get
    End Property

    Public ReadOnly Property MasterType() As MasterEquipmentType
        Get
            Return GenerationEquipmentItem.MasterType
        End Get
    End Property

    Public ReadOnly Property MasterPath() As String
        Get
            If Type.Equals(EquipmentType.Option) Then
                Return DirectCast(GenerationEquipmentItem, ModelGenerationOption).MasterPath
            End If
            Return String.Empty
        End Get
    End Property

    Friend ReadOnly Property Rules() As ModelGenerationEquipmentRules
        Get
            Return GenerationEquipmentItem.Rules
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overrides Function Equals(ByVal obj As String) As Boolean
        Return Not (obj Is Nothing) AndAlso GenerationEquipmentItem.Equals(obj)
    End Function

#End Region

#Region " Constructors "
    Protected Sub New()
        MarkAsChild()
        AllowNew = False
        AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overloads Sub Create(ByVal equipmenItem As ModelGenerationEquipmentItem)
        Create(equipmenItem.ID)
        _availability = Availability.NotAvailable
        _generationEquipmentItem = equipmenItem
        MarkOld() 'These objects don't need be saved untill somebody actualy does something with them
    End Sub

    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("EQUIPMENTID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _availability = CType(.GetValue("AVAILABILITY"), Availability)
            _gradeFeature = .GetBoolean("GRADEFEATURE")
            _optionalGradeFeature = .GetBoolean("OPTIONALGRADEFEATURE", False)
        End With
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateModelGenerationGradeEquipmentItem"
        command.Parameters.AddWithValue("@GENERATIONID", Grade.Generation.ID)
        command.Parameters.AddWithValue("@GRADEID", Grade.ID)
        command.Parameters.AddWithValue("@EQUIPMENTID", ID)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@AVAILABILITY", Availability)
        command.Parameters.AddWithValue("@GRADEFEATURE", GradeFeature)
        command.Parameters.AddWithValue("@OPTIONALGRADEFEATURE", OptionalGradeFeature)
    End Sub
#End Region
End Class

<Serializable(), XmlInfo("accessory")> Public NotInheritable Class ModelGenerationGradeAccessory
    Inherits ModelGenerationGradeEquipmentItem

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
    Public Property FittingPriceNewCar() As Decimal
        Get
            Return _fittingPriceNewCar
        End Get
        Set(ByVal value As Decimal)
            If _fittingPriceNewCar.Equals(value) Then Return

            _fittingPriceNewCar = value
            PropertyHasChanged("FittingPriceNewCar")
        End Set
    End Property
    Public Property FittingVatPriceNewCar() As Decimal
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
            If FittingHoursExistingCar.Equals(value) Then Return

            _fittingTimeExistingCar = New TimeSpan(value, _fittingTimeExistingCar.Minutes, _fittingTimeExistingCar.Seconds)
            PropertyHasChanged("FittingTimeExistingCar")
        End Set
    End Property
    <XmlInfo(XmlNodeType.None)> Public Property FittingMinutesExistingCar() As Integer
        Get
            Return _fittingTimeExistingCar.Minutes
        End Get
        Set(ByVal value As Integer)
            If _fittingTimeExistingCar.Minutes.Equals(value) Then Return

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

#End Region

#Region " Reference Properties & Methods "
    Public ReadOnly Property BasePrice() As Decimal
        Get
            Return DirectCast(GenerationEquipmentItem, ModelGenerationAccessory).BasePrice
        End Get
    End Property
    Public ReadOnly Property BaseVatPrice() As Decimal
        Get
            Return DirectCast(GenerationEquipmentItem, ModelGenerationAccessory).BaseVatPrice
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

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationGradeAccessory(ByVal accessory As ModelGenerationAccessory) As ModelGenerationGradeAccessory
        Dim gradeAccessory As ModelGenerationGradeAccessory = New ModelGenerationGradeAccessory()
        gradeAccessory.Create(accessory)
        Return gradeAccessory
    End Function
    Friend Shared Function GetModelGenerationGradeAccessory(ByVal dataReader As SafeDataReader) As ModelGenerationGradeAccessory
        Dim gradeAccessory As ModelGenerationGradeAccessory = New ModelGenerationGradeAccessory()
        gradeAccessory.Fetch(dataReader)
        Return gradeAccessory
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
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddUpdateCommandFields(command)
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
<Serializable(), XmlInfo("option")> Public NotInheritable Class ModelGenerationGradeOption
    Inherits ModelGenerationGradeEquipmentItem
    Implements IPrice

#Region " Business Properties & Methods "
    Private _suffixAvailability As Availability = Availability.NotAvailable
    Private _fittingPrice As Decimal = 0D
    Private _fittingVatPrice As Decimal = 0D
    Private _replacedBy As Guid = Guid.Empty
    Private _offeredThrough As ModelGenerationGradeOptionPackRelations

    Public Property SuffixAvailability() As Availability
        Get
            Return _suffixAvailability
        End Get
        Set(ByVal value As Availability)
            If _suffixAvailability.Equals(value) Then Return

            _suffixAvailability = value
            Availability = _suffixAvailability
            PropertyHasChanged("SuffixAvailability")
        End Set
    End Property

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

    Public Function IsReplaced() As Boolean
        Return ReplacedBy IsNot Nothing
    End Function
    Public Property ReplacedBy() As ModelGenerationGradeOption
        Get
            If _replacedBy.Equals(Guid.Empty) Then Return Nothing
            Return DirectCast(DirectCast(Parent, ModelGenerationGradeEquipment)(_replacedBy), ModelGenerationGradeOption)
        End Get
        Set(ByVal value As ModelGenerationGradeOption)
            If _replacedBy.Equals(Guid.Empty) AndAlso value Is Nothing Then Return
            If (value IsNot Nothing) AndAlso Not _replacedBy.Equals(Guid.Empty) AndAlso _replacedBy.Equals(value.ID) Then Return

            If value Is Nothing Then
                Dim previousReplacement As ModelGenerationGradeOption = ReplacedBy
                _replacedBy = Guid.Empty
                Availability = SuffixAvailability
                If Not previousReplacement Is Nothing AndAlso Not previousReplacement.IsReplacement Then 'TODO: I don't think the is nothing check is necessary anymore => value cannot be nothing if _replacedby is guid.empty, since this was checked in the first line of this setter...
                    previousReplacement._suffixAvailability = Nothing
                End If
            Else
                If value.IsReplacement AndAlso value.SuffixAvailability <> SuffixAvailability Then
                    Dim message As String
                    If value.ReplacementFor.Count = 1 Then
                        message = "The non-suffix option {0} is already replacing the suffix option {1}, which has a different availability ({2}) then this option ({3})"
                    Else
                        message = "The non-suffix option {0} is already replacing the suffix options {1}, which have a different availability ({2}) then this option ({3})"
                    End If
                    Throw New UpdateNotAllowed(String.Format(message, value.AlternateName, String.Join(",", (From replacedOption In value.ReplacementFor Select replacedOption.AlternateName).ToArray()), value.SuffixAvailability, SuffixAvailability))
                End If

                _replacedBy = value.ID
                Availability = Availability.NotAvailable
                value.SuffixAvailability = SuffixAvailability
            End If

            PropertyHasChanged("ReplacedBy")
            ValidationRules.CheckRules("ReplacedOrOffered")
        End Set
    End Property
    Public Function IsReplacement() As Boolean
        If SuffixOption Then Return False 'Shortcut: SuffixOption can't be a replacement
        Return ReplacementFor.Any()
    End Function
    Public ReadOnly Property ReplacementFor() As IEnumerable(Of ModelGenerationGradeOption)
        Get
            Return From [option] In DirectCast(Parent, ModelGenerationGradeEquipment) _
                   Where [option].Type = EquipmentType.Option AndAlso _
                          Not DirectCast([option], ModelGenerationGradeOption).ReplacedBy Is Nothing AndAlso _
                          DirectCast([option], ModelGenerationGradeOption).ReplacedBy.ID.Equals(ID) _
                  Select DirectCast([option], ModelGenerationGradeOption)
        End Get
    End Property

    Public ReadOnly Property OfferedThrough() As ModelGenerationGradeOptionPackRelations
        Get
            If _offeredThrough IsNot Nothing Then Return _offeredThrough
            _offeredThrough = If(IsNew, ModelGenerationGradeOptionPackRelations.NewRelations(Me), ModelGenerationGradeOptionPackRelations.GetRelations(Me))
            Return _offeredThrough
        End Get
    End Property

    Public ReadOnly Property ExclusivelyOfferedThroughAPack() As Boolean
        Get
            Return OfferedThrough.Any() AndAlso Availability.Equals(Availability.NotAvailable)
        End Get
    End Property

    Public Function IsOfferedThrough(ByVal modelGenerationPack As ModelGenerationPack) As Boolean
        Return OfferedThrough.Contains(modelGenerationPack.ID)
    End Function

    Public Sub OfferThrough(ByVal pack As ModelGenerationPack, Optional exclusivelyThroughPack As Boolean = False)
        If Not SuffixOption Then Throw New NonSuffixOptionOfferedThroughPackException(Me, pack)
        If IsOfferedThrough(pack) Then Throw New SuffixOptionAlreadyOfferedThroughPackException(Me, pack)

        If Not pack.Equipment.Contains(Grade.Generation.Equipment(ID)) Then
            pack.Equipment.Add(Grade.Generation.Equipment(ID))
        End If

        OfferedThrough.Add(pack)
        OfferCarOptionsThrough(pack.ID, exclusivelyThroughPack)

        ValidationRules.CheckRules("ReplacedOrOffered")
    End Sub

    Private Sub OfferCarOptionsThrough(ByVal packId As Guid, ByVal exclusivelyThroughPack As Boolean)

        For Each car In Grade.Cars()
            Dim carEquipmentItem = car.Equipment(ID)

            If carEquipmentItem Is Nothing Then Continue For

            Dim carOption = DirectCast(carEquipmentItem, CarOption)
            Dim carPack = car.Packs(packId)

            If exclusivelyThroughPack Then
                carOption.Overwrite()
                carOption.Availability = Availability.NotAvailable
            End If

            carOption.OfferThrough(carPack)
        Next
    End Sub

    Public Sub OfferExclusivelyThrough(ByVal pack As ModelGenerationPack)
        Availability = Availability.NotAvailable
        OfferThrough(pack, True)
        ValidationRules.CheckRules("Availability")
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

#End Region

#Region " Reference Properties & Methods "

    Public ReadOnly Property SuffixOption() As Boolean
        Get
            Return DirectCast(GenerationEquipmentItem, ModelGenerationOption).SuffixOption
        End Get
    End Property
    Public ReadOnly Property Visible() As Boolean
        Get
            Return DirectCast(GenerationEquipmentItem, ModelGenerationOption).Visible
        End Get
    End Property

    Public ReadOnly Property Code() As String
        Get
            Return DirectCast(GenerationEquipmentItem, ModelGenerationOption).Code
        End Get
    End Property

    Public ReadOnly Property HasParentOption() As Boolean
        Get
            Return Not ParentOption Is Nothing
        End Get
    End Property

    Public ReadOnly Property ParentOption() As ModelGenerationGradeOption
        Get
            Dim referenceParentOption As ModelGenerationOption = DirectCast(GenerationEquipmentItem, ModelGenerationOption)
            If referenceParentOption.HasParentOption Then
                Return DirectCast(Grade.Equipment(referenceParentOption.ParentOption.ID), ModelGenerationGradeOption)
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property HasChildOptions() As Boolean
        Get
            Return ChildOptions.Any()
        End Get
    End Property

    Public ReadOnly Property ChildOptions() As IEnumerable(Of ModelGenerationGradeOption)
        Get
            Dim childIds As List(Of Guid) = DirectCast(GenerationEquipmentItem, ModelGenerationOption).ChildOptions.Select(Function(x) x.ID).ToList()
            Return Grade.Equipment.Where(Function(x) childIds.Contains(x.ID)).Select(Function(y) DirectCast(y, ModelGenerationGradeOption))
        End Get
    End Property
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        MyBase.AddBusinessRules()

        ValidationRules.AddRule(DirectCast(AddressOf IsNonSuffixOption, RuleHandler), "ReplacedBy")
        ValidationRules.AddRule(DirectCast(AddressOf AvailabilityRule, RuleHandler), "Availability")

        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeStandardIfNonOfTheChildOptionsAreStandard, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeOptionalIfNonOfTheChildOptionsAreOptional, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeNotAvailableIfNotAllChildOptionsNotAvailable, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeStandardIfParentIsNotStandard, RuleHandler), "Availability")
        ValidationRules.AddRule(DirectCast(AddressOf OptionCanNotBeOptionalIfParentIsNotOptionalOrStandard, RuleHandler), "Availability")
        

        ValidationRules.AddRule(DirectCast(AddressOf OptionCannotBeReplacedByAnotherOptionAndOfferedThroughAPackAtTheSameTime, RuleHandler), "ReplacedOrOffered")
    End Sub

    Private Shared Function IsNonSuffixOption(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim gradeOption As ModelGenerationGradeOption = DirectCast(target, ModelGenerationGradeOption)
        If gradeOption.ReplacedBy Is Nothing Then Return True
        If gradeOption.ReplacedBy.SuffixOption Then
            e.Description = "You can not use a suffix option as replacement!"
            Return False
        End If
        Return True
    End Function
    Private Shared Function AvailabilityRule(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim gradeOption As ModelGenerationGradeOption = DirectCast(target, ModelGenerationGradeOption)
        If gradeOption.GenerationEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not gradeOption.SuffixOption Then Return True
        If gradeOption.Availability.Equals(gradeOption.SuffixAvailability) Then Return True
        If gradeOption.ExclusivelyOfferedThroughAPack AndAlso gradeOption.Availability = Enums.Availability.NotAvailable Then Return True
        If Not gradeOption.ReplacedBy Is Nothing AndAlso gradeOption.Availability = Enums.Availability.NotAvailable Then Return True

        e.Description = String.Format("You can not change the availability of the suffix option ""{0}"" from {1} to {2}", gradeOption.Name, gradeOption.SuffixAvailability, gradeOption.Availability)
        Return False

    End Function

    Private Shared Function OptionCanNotBeStandardIfNonOfTheChildOptionsAreStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim gradeOption As ModelGenerationGradeOption = DirectCast(target, ModelGenerationGradeOption)
        If gradeOption.GenerationEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not gradeOption.Availability = Enums.Availability.Standard Then Return True
        If Not gradeOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made Standard if non of the child options are Standard", gradeOption.AlternateName)

        Return gradeOption.ChildOptions.Any(Function(x) x.Availability = Enums.Availability.Standard)
    End Function
    Private Shared Function OptionCanNotBeOptionalIfNonOfTheChildOptionsAreOptional(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim gradeOption As ModelGenerationGradeOption = DirectCast(target, ModelGenerationGradeOption)
        If gradeOption.GenerationEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not gradeOption.Availability = Enums.Availability.Optional Then Return True
        If Not gradeOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made Optional if non of the child options are Optional", gradeOption.AlternateName)

        Return gradeOption.ChildOptions.Any(Function(x) x.Availability = Enums.Availability.Optional) AndAlso Not gradeOption.ChildOptions.Any(Function(x) x.Availability = Enums.Availability.Standard)
    End Function
    Private Shared Function OptionCanNotBeNotAvailableIfNotAllChildOptionsNotAvailable(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim gradeOption As ModelGenerationGradeOption = DirectCast(target, ModelGenerationGradeOption)
        If gradeOption.GenerationEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not gradeOption.Availability = Enums.Availability.NotAvailable Then Return True
        If Not gradeOption.HasChildOptions Then Return True
        e.Description = String.Format("The option {0} can not be made Not Available if not all of the child options are Not Available", gradeOption.AlternateName)

        Return gradeOption.ChildOptions.All(Function(x) x.Availability = Enums.Availability.NotAvailable)
    End Function
    Private Shared Function OptionCanNotBeStandardIfParentIsNotStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim gradeOption As ModelGenerationGradeOption = DirectCast(target, ModelGenerationGradeOption)
        If gradeOption.GenerationEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not gradeOption.Availability = Enums.Availability.Standard Then Return True
        If Not gradeOption.HasParentOption Then Return True
        e.Description = String.Format("The option {0} can not be made Standard if its parent is not Standard.", gradeOption.AlternateName)

        Return gradeOption.ParentOption.Availability = Enums.Availability.Standard
    End Function
    Private Shared Function OptionCanNotBeOptionalIfParentIsNotOptionalOrStandard(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim gradeOption As ModelGenerationGradeOption = DirectCast(target, ModelGenerationGradeOption)
        If gradeOption.GenerationEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        If Not gradeOption.Availability = Enums.Availability.Optional Then Return True
        If Not gradeOption.HasParentOption Then Return True
        e.Description = String.Format("The option {0} can not be made Optional if its parent is Not Available.", gradeOption.AlternateName)

        Return gradeOption.ParentOption.Availability = Enums.Availability.Optional OrElse gradeOption.ParentOption.Availability = Enums.Availability.Standard
    End Function
   

    Private Shared Function OptionCannotBeReplacedByAnotherOptionAndOfferedThroughAPackAtTheSameTime(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim gradeOption = DirectCast(target, ModelGenerationGradeOption)
        If gradeOption.GenerationEquipmentItem Is Nothing Then Return True 'Rule can not be checked, but in this case (normally) this should not be a suffix option
        e.Description = String.Format("The option {0} cannot be replaced by another option and offered through one or more packs at the same time", gradeOption.AlternateName)
        Return gradeOption.ReplacedBy Is Nothing OrElse Not gradeOption.OfferedThrough.Any()
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Function Equals(ByVal obj As [Option]) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeOption) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_offeredThrough Is Nothing) AndAlso Not _offeredThrough.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_offeredThrough Is Nothing) AndAlso _offeredThrough.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationGradeOption(ByVal [option] As [ModelGenerationOption]) As ModelGenerationGradeOption
        Dim gradeOption As ModelGenerationGradeOption = New ModelGenerationGradeOption
        gradeOption.Create([option])
        Return gradeOption
    End Function
    Friend Shared Function GetModelGenerationGradeOption(ByVal dataReader As SafeDataReader) As ModelGenerationGradeOption
        Dim gradeOption As ModelGenerationGradeOption = New ModelGenerationGradeOption()
        gradeOption.Fetch(dataReader)
        Return gradeOption
    End Function
#End Region

#Region " Data Access "

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not (_offeredThrough Is Nothing) Then _offeredThrough.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            If Not .IsDBNull("SUFFIXAVAILABILITY") Then _suffixAvailability = CType(.GetValue("SUFFIXAVAILABILITY"), Availability)
            _fittingPrice = Environment.ConvertPrice(CType(.GetValue("FITTINGPRICENEW"), Decimal), .GetString("FITTINGCURRENCY"))
            _fittingVatPrice = Environment.ConvertPrice(CType(.GetValue("FITTINGPRICEVATNEW"), Decimal), .GetString("FITTINGCURRENCY"))
            _replacedBy = .GetGuid("REPLACEDBY")
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        MyBase.AddUpdateCommandFields(command)
        With command
            .Parameters.AddWithValue("@SUFFIXAVAILABILITY", SuffixAvailability)
            .Parameters.AddWithValue("@FITTINGCURRENCY", MyContext.GetContext().Currency.Code)
            .Parameters.AddWithValue("@FITTINGPRICENEW", FittingPrice)
            .Parameters.AddWithValue("@FITTINGPRICEVATNEW", FittingVatPrice)
            If _replacedBy.Equals(Guid.Empty) Then
                .Parameters.AddWithValue("@REPLACEDBY", DBNull.Value)
            Else
                .Parameters.AddWithValue("@REPLACEDBY", _replacedBy)
            End If

        End With
    End Sub
#End Region
End Class

<Serializable(), XmlInfo("exteriorcolourtype")> Public NotInheritable Class ModelGenerationGradeExteriorColourType
    Inherits ModelGenerationGradeEquipmentItem

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

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationGradeExteriorColourType(ByVal type As ModelGenerationExteriorColourType) As ModelGenerationGradeExteriorColourType
        Dim gradeExteriorColourType As ModelGenerationGradeExteriorColourType = New ModelGenerationGradeExteriorColourType
        gradeExteriorColourType.Create(type)
        Return gradeExteriorColourType
    End Function
    Friend Shared Function GetModelGenerationGradeExteriorColourType(ByVal dataReader As SafeDataReader) As ModelGenerationGradeExteriorColourType
        Dim gradeExteriorColourType As ModelGenerationGradeExteriorColourType = New ModelGenerationGradeExteriorColourType()
        gradeExteriorColourType.Fetch(dataReader)
        Return gradeExteriorColourType
    End Function
#End Region

End Class
<Serializable(), XmlInfo("upholsterytype")> Public NotInheritable Class ModelGenerationGradeUpholsteryType
    Inherits ModelGenerationGradeEquipmentItem

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

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationGradeUpholsteryType(ByVal type As ModelGenerationUpholsteryType) As ModelGenerationGradeUpholsteryType
        Dim gradeUpholsteryType As ModelGenerationGradeUpholsteryType = New ModelGenerationGradeUpholsteryType
        gradeUpholsteryType.Create(type)
        Return gradeUpholsteryType
    End Function
    Friend Shared Function GetModelGenerationGradeUpholsteryType(ByVal dataReader As SafeDataReader) As ModelGenerationGradeUpholsteryType
        Dim gradeUpholsteryType As ModelGenerationGradeUpholsteryType = New ModelGenerationGradeUpholsteryType()
        gradeUpholsteryType.Fetch(dataReader)
        Return gradeUpholsteryType
    End Function
#End Region

End Class