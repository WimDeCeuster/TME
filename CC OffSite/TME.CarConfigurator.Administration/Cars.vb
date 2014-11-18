Imports System.Collections.Generic
Imports TME.Common
Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.Exceptions
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Cars
    Inherits ContextUniqueGuidListBase(Of Cars, Car)

#Region " Delegates & Events "
    Friend Delegate Sub CarAvailabilityChangedHandler(ByVal car As Car)
    Friend Event CarAvailabilityChanged As CarAvailabilityChangedHandler

    Friend Sub RaiseCarAvailabilityChangedEvent(ByVal car As Car)
        RaiseEvent CarAvailabilityChanged(car)
    End Sub

#End Region

#Region " Business Properties & Methods "

    Friend Property Generation() As ModelGeneration
        Get
            Return DirectCast(Parent, ModelGeneration)
        End Get
        Private Set(ByVal value As ModelGeneration)
            SetParent(value)
        End Set
    End Property

    Public Shadows Function Add(ByVal factoryCar As ModelGenerationFactoryCar, ByVal grade As ModelGenerationGrade) As Car
        Dim newCar As Car = Car.NewCar(factoryCar, grade, Generation.Mode)
        MyBase.Add(newCar)
        'assign default submodel
        newCar.SubModelID = (From x In grade.Generation.SubModels Where x.DefaultFittings.Any(Function(y) y.Matches(newCar)) Select x.ID).FirstOrDefault()
        newCar.CheckRules() 'check the rules for the car => parent has been set now, so rules that need parent can be checked (like line off dates)
        RaiseCarAvailabilityChangedEvent(newCar)
        Return newCar
    End Function
    Public Shadows Function Add(ByVal katashiki As String, ByVal grade As ModelGenerationGrade) As Car
        Dim factoryCar = (From fg In Generation.FactoryGenerations
                           From fc In fg.FactoryCars
                           Where fc.Activated AndAlso fc.Katashiki.Equals(katashiki, StringComparison.InvariantCultureIgnoreCase)
                           Select fc).FirstOrDefault()

        If factoryCar Is Nothing Then Throw New KatashikiNotAvailable(String.Format("The katashiki {0} is not available on the {1} marketing generation", katashiki, Generation))
        Return Add(factoryCar, grade)
    End Function


    Friend ReadOnly Property DeletedCars() As List(Of Car)
        Get
            Return DeletedList
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewCars(ByVal generation As ModelGeneration) As Cars
        Dim theNewCars As Cars = New Cars()
        theNewCars.Generation = generation
        Return theNewCars
    End Function
    Friend Shared Function GetCars(ByVal generation As ModelGeneration) As Cars
        Dim fetchedCars As Cars = DataPortal.Fetch(Of Cars)(New ParentCriteria(generation.ID, "@GENERATIONID"))
        fetchedCars.Generation = generation
        Return fetchedCars
    End Function
    Friend Shared Function GetCars(ByVal generation As ModelGeneration, ByVal dataReader As SafeDataReader) As Cars
        Dim fetchedCars As Cars = New Cars
        fetchedCars.Fetch(dataReader)
        fetchedCars.Generation = generation
        Return fetchedCars
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region


End Class
<Serializable(), XmlInfo("car")> Public Class Car
    Inherits LocalizeableBusinessBase
    Implements IPrice

#Region " Business Properties & Methods "
    Private _shortID As Nullable(Of Integer) = Nothing
    Private _owner As String = String.Empty
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _status As Integer
    Private _index As Integer

    Private _webVisible As Boolean
    Private _configVisible As Boolean
    Private _financeVisible As Boolean

    Private _price As Decimal = (0D)
    Private _vatPrice As Decimal = (0D)

    Private _factoryGenerationID As Guid
    Private _factoryBodyTypeID As Guid
    Private _factoryEngineID As Guid
    Private _factoryGradeID As Guid
    Private _factoryTransmissionID As Guid

    Private _bodyTypeID As Guid
    Private _engineID As Guid
    Private _gradeID As Guid
    Private _transmissionID As Guid
    Private _wheelDriveID As Guid
    Private _steeringID As Guid
    Private _subModelID As Guid

    Private _versionID As Guid = Guid.NewGuid()
    Private _carSpecification As PartialCarSpecification
    Private _factoryCarSpecification As PartialCarSpecification

    Private _mode As LocalizationMode

    Private _colourCombinations As LinkedColourCombinations
    Private _equipment As CarEquipment
    Private _packs As CarPacks
    Private _specifications As CarSpecifications
    Private WithEvents _suffixes As CarSuffixes
    Private WithEvents _localConfigurations As LocalConfigurations

    Private _coloursCalculated As Boolean
    Private _optionsCalculated As Boolean
    Private _packsCalculated As Boolean
    Private _specificationsCalculated As Boolean
    Private _lineOffToDate As Date
    Private _lineOffFromDate As Date

    Public ReadOnly Property Generation() As ModelGeneration
        Get
            Return If(Parent Is Nothing, Nothing, DirectCast(Parent, Cars).Generation)
        End Get
    End Property


    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return _shortID
        End Get
    End Property
    Friend ReadOnly Property Owner() As String
        Get
            Return _owner
        End Get
    End Property

    Public Property Mode As LocalizationMode
        Get
            Return _mode
        End Get
        Private Set(value As LocalizationMode)
            If _mode = value Then Return
            _mode = value
            PropertyHasChanged("Mode")
        End Set
    End Property

    Private ReadOnly Property IsInSuffixOrLocalizationMode As Boolean
        Get
            Return Mode = LocalizationMode.Suffix OrElse Mode = LocalizationMode.LocalConfiguration
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Katashiki() As String
        Get
            Return (From fc In Generation.FactoryGenerations(FactoryGenerationID).FactoryCars
                   Where
                        fc.BodyType.Equals(FactoryBodyTypeID) AndAlso
                        fc.Engine.Equals(FactoryEngineID) AndAlso
                        fc.Transmission.Equals(FactoryTransmissionID) AndAlso
                        fc.Grade.Equals(FactoryGradeID) AndAlso
                        fc.WheelDrive.Equals(WheelDriveID) AndAlso
                        fc.Steering.Equals(SteeringID)
                   Select fc).First().Katashiki
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code.Equals(value) Then Return

            _code = value
            PropertyHasChanged("Code")
        End Set
    End Property
    Private Sub MePropertyChanged(ByVal sender As Object, ByVal e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged

        If e.PropertyName.Equals("LocalCode") Then
            If Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) AndAlso Not Code.Equals(LocalCode) Then
                Code = LocalCode
            End If
            Return
        End If

        If e.PropertyName.Equals("Code") Then
            If Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) AndAlso Not LocalCode.Equals(Code) Then
                _localCode = Code
            End If
            Return
        End If

    End Sub

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name = value Then Return

            _name = value
            PropertyHasChanged("Name")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Activated() As Boolean
        Get
            Return ((_status And Status.AvailableToNmscs) = Status.AvailableToNmscs)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Activated) Then Return

            If Activated Then
                _status -= Status.AvailableToNmscs
            Else
                _status += Status.AvailableToNmscs
            End If
            PropertyHasChanged("Activated")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Promoted() As Boolean
        Get
            Return ((_status And Status.Promoted) = Status.Promoted)
        End Get
        Private Set(ByVal value As Boolean)
            If value.Equals(Promoted) Then Return

            If value Then
                _status += Status.Promoted
            Else
                _status -= Status.Promoted
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Approved) Then Return
            If value Then
                _status += Status.ApprovedForLive
                If Declined Then _status -= Status.Declined
            Else
                _status -= Status.ApprovedForLive
                If Promoted Then _status -= Status.Promoted
                If Not Declined Then _status += Status.Declined
            End If
            PropertyHasChanged("Approved")
            RaiseCarAvailabilityChangedEvent()
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Declined) Then Return
            If value Then
                _status += Status.Declined
                If Approved Then _status -= Status.ApprovedForLive
                If Promoted Then _status -= Status.Promoted
            Else
                _status -= Status.Declined
                If Not Approved Then _status += Status.ApprovedForLive
            End If
            PropertyHasChanged("Declined")
            RaiseCarAvailabilityChangedEvent()
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Preview() As Boolean
        Get
            Return ((_status And Status.ApprovedForPreview) = Status.ApprovedForPreview)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Preview) Then Return

            If Preview Then
                _status -= Status.ApprovedForPreview
            Else
                _status += Status.ApprovedForPreview
            End If
            PropertyHasChanged("Preview")
            RaiseCarAvailabilityChangedEvent()
        End Set
    End Property
    Private Sub RaiseCarAvailabilityChangedEvent()
        If Parent Is Nothing Then Exit Sub
        DirectCast(Parent, Cars).RaiseCarAvailabilityChangedEvent(Me)
    End Sub

    <XmlInfo(XmlNodeType.Attribute)> Public Property WebVisible() As Boolean
        Get
            Return _webVisible
        End Get
        Set(ByVal value As Boolean)

            If value.Equals(WebVisible) Then Return
            _webVisible = value
            PropertyHasChanged("WebVisible")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property ConfigVisible() As Boolean
        Get
            Return _configVisible
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(ConfigVisible) Then Return

            _configVisible = value
            PropertyHasChanged("ConfigVisible")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property FinanceVisible() As Boolean
        Get
            Return _financeVisible
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(FinanceVisible) Then Return

            _financeVisible = value
            PropertyHasChanged("FinanceVisible")
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Index() As Integer
        Get
            Return _index
        End Get
        Set(ByVal value As Integer)
            If _index.Equals(value) Then Return

            _index = value
            PropertyHasChanged("Index")
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

    Public Property LineOffFromDate As Date
        Get
            Return _lineOffFromDate
        End Get
        Set(ByVal value As Date)
            Dim newValue = New Date(value.Year, value.Month, value.Day, 0, 0, 0) 'set time of from date to start of the day

            If _lineOffFromDate.CompareTo(newValue) = 0 Then Return
            _lineOffFromDate = newValue
            PropertyHasChanged("LineOffFromDate")
            ValidationRules.CheckRules("LineOffDate")
            RevalidatePreviewAndApprovedForAllCars()
        End Set
    End Property
    Public Property LineOffToDate As Date
        Get
            Return If(_lineOffToDate = DateTime.MaxValue, DateTime.MaxValue, _lineOffToDate - New TimeSpan(1, 0, 0, 0))
        End Get
        Set(ByVal value As Date)
            Dim newValue = If(value = DateTime.MaxValue, DateTime.MaxValue, New Date(value.Year, value.Month, value.Day, 0, 0, 0).AddDays(1)) 'set time of from date to start of the day

            If _lineOffToDate.CompareTo(newValue) = 0 Then Return
            _lineOffToDate = newValue
            PropertyHasChanged("LineOffToDate")
            ValidationRules.CheckRules("LineOffDate")
            RevalidatePreviewAndApprovedForAllCars()
        End Set
    End Property

    Public Property FactoryGenerationID() As Guid
        Get
            Return _factoryGenerationID
        End Get
        Private Set(ByVal value As Guid)
            _factoryGenerationID = value
        End Set
    End Property
    Public Property FactoryBodyTypeID() As Guid
        Get
            Return _factoryBodyTypeID
        End Get
        Private Set(ByVal value As Guid)
            _factoryBodyTypeID = value
        End Set
    End Property
    Public Property FactoryEngineID() As Guid
        Get
            Return _factoryEngineID
        End Get
        Private Set(ByVal value As Guid)
            _factoryEngineID = value
        End Set
    End Property
    Public Property FactoryGradeID() As Guid
        Get
            Return _factoryGradeID
        End Get
        Private Set(ByVal value As Guid)
            _factoryGradeID = value
        End Set
    End Property
    Public Property FactoryTransmissionID() As Guid
        Get
            Return _factoryTransmissionID
        End Get
        Private Set(ByVal value As Guid)
            _factoryTransmissionID = value
        End Set
    End Property

    Public ReadOnly Property FactoryGeneration() As FactoryGenerationInfo
        Get
            Return Generation.FactoryGenerations(FactoryGenerationID).GetInfo()
        End Get
    End Property
    Public ReadOnly Property FactoryBodyType() As BodyTypeInfo
        Get
            Return Generation.FactoryGenerations(FactoryGenerationID).FactoryCars.First(Function(x) x.BodyType.Equals(FactoryBodyTypeID)).BodyType
        End Get
    End Property
    Public ReadOnly Property FactoryEngine() As EngineInfo
        Get
            Return Generation.FactoryGenerations(FactoryGenerationID).FactoryCars.First(Function(x) x.Engine.Equals(FactoryEngineID)).Engine
        End Get
    End Property
    Public ReadOnly Property FactoryGrade() As FactoryGradeInfo
        Get
            Return Generation.FactoryGenerations(FactoryGenerationID).FactoryCars.First(Function(x) x.Grade.Equals(FactoryGradeID)).Grade
        End Get
    End Property
    Public ReadOnly Property FactoryTransmission() As TransmissionInfo
        Get
            Return Generation.FactoryGenerations(FactoryGenerationID).FactoryCars.First(Function(x) x.Transmission.Equals(FactoryTransmissionID)).Transmission
        End Get
    End Property


    Public Property BodyTypeID() As Guid
        Get
            Return _bodyTypeID
        End Get
        Set(ByVal value As Guid)
            If _bodyTypeID.Equals(value) Then Return

            _bodyTypeID = value
            _carSpecification = Nothing
            Generation.BodyTypes.Synchronize()
            Generation.Grades.Synchronize()
            PropertyHasChanged("BodyTypeID")
            RevalidatePreviewAndApprovedForAllCars()
        End Set
    End Property
    Public Property EngineID() As Guid
        Get
            Return _engineID
        End Get
        Set(ByVal value As Guid)
            If _engineID.Equals(value) Then Return

            _engineID = value
            _carSpecification = Nothing
            Generation.Engines.Synchronize()
            PropertyHasChanged("EngineID")
            RevalidatePreviewAndApprovedForAllCars()
        End Set
    End Property
    Public Property GradeID() As Guid
        Get
            Return _gradeID
        End Get
        Set(ByVal value As Guid)
            If _gradeID.Equals(value) Then Return

            _gradeID = value
            _carSpecification = Nothing
            Generation.Grades.Synchronize()
            PropertyHasChanged("GradeID")
            RevalidatePreviewAndApprovedForAllCars()
        End Set
    End Property
    Public Property TransmissionID() As Guid
        Get
            Return _transmissionID
        End Get
        Set(ByVal value As Guid)
            If _transmissionID.Equals(value) Then Return

            _transmissionID = value
            _carSpecification = Nothing
            Generation.Transmissions.Synchronize()
            PropertyHasChanged("TransmissionID")
            RevalidatePreviewAndApprovedForAllCars()
        End Set
    End Property
    Public ReadOnly Property WheelDriveID() As Guid
        Get
            Return _wheelDriveID
        End Get
    End Property
    Public ReadOnly Property SteeringID() As Guid
        Get
            Return _steeringID
        End Get
    End Property
    Public Property SubModelID() As Guid
        Get
            Return _subModelID
        End Get
        Set(ByVal value As Guid)
            If _subModelID.Equals(value) Then Return
            _subModelID = value
            Generation.SubModels.Synchronize()
            Generation.Grades.Synchronize()
            PropertyHasChanged("SubModelID")
        End Set
    End Property


    Public ReadOnly Property BodyType() As BodyTypeInfo
        Get
            Return Generation.BodyTypes(BodyTypeID).GetInfo()
        End Get
    End Property
    Public ReadOnly Property Engine() As EngineInfo
        Get
            Return Generation.Engines(EngineID).GetInfo()
        End Get
    End Property
    Public ReadOnly Property Grade() As GradeInfo
        Get
            Return Generation.Grades(GradeID).GetInfo()
        End Get
    End Property
    Public ReadOnly Property Transmission() As TransmissionInfo
        Get
            Return Generation.Transmissions(TransmissionID).GetInfo()
        End Get
    End Property
    Public ReadOnly Property WheelDrive() As WheelDriveInfo
        Get
            Return Generation.WheelDrives(WheelDriveID).GetInfo()
        End Get
    End Property
    Public ReadOnly Property Steering() As SteeringInfo
        Get
            Return Steerings.GetSteerings()(SteeringID).GetInfo()
        End Get
    End Property
    Public ReadOnly Property SubModel() As SubModelInfo
        Get
            If SubModelID.Equals(Guid.Empty) Then Return SubModelInfo.Empty
            Return Generation.SubModels(SubModelID).GetInfo()
        End Get
    End Property

    Public ReadOnly Property PartialCarSpecification() As PartialCarSpecification
        Get
            If _carSpecification Is Nothing Then _carSpecification = PartialCarSpecification.NewPartialCarSpecification(Me)
            Return _carSpecification
        End Get
    End Property
    Friend ReadOnly Property FactoryCarSpecification() As PartialCarSpecification
        Get
            If _factoryCarSpecification Is Nothing Then _factoryCarSpecification = PartialCarSpecification.NewFactoryCarSpecification(Me)
            Return _factoryCarSpecification
        End Get
    End Property


    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then Return MyBase.CanWriteProperty(propertyName)
        Select Case propertyName
            Case "BodyTypeID", "EngineID", "TransmissionID", "GradeID", "Name", "Code"
                Return False
            Case Else
                Return MyBase.CanWriteProperty(propertyName)
        End Select
    End Function

    Friend ReadOnly Property VersionID() As Guid
        Get
            Return _versionID
        End Get
    End Property

    Public Function HasSameComponents(ByVal car As Car) As Boolean
        Return BodyTypeID.Equals(car.BodyTypeID) AndAlso _
         EngineID.Equals(car.EngineID) AndAlso _
         GradeID.Equals(car.GradeID) AndAlso _
         TransmissionID.Equals(car.TransmissionID) AndAlso _
         WheelDriveID.Equals(car.WheelDriveID) AndAlso _
         SteeringID.Equals(car.SteeringID)
    End Function
    Public Function HasSameComponents(ByVal factoryCar As FactoryCar) As Boolean
        Return BodyTypeID.Equals(factoryCar.BodyType.ID) AndAlso _
         EngineID.Equals(factoryCar.Engine.ID) AndAlso _
         FactoryGradeID.Equals(factoryCar.Grade.ID) AndAlso _
         TransmissionID.Equals(factoryCar.Transmission.ID) AndAlso _
         WheelDriveID.Equals(factoryCar.WheelDrive.ID) AndAlso _
         SteeringID.Equals(factoryCar.Steering.ID)
    End Function


    Public ReadOnly Property ColourCombinations() As LinkedColourCombinations
        Get
            If _colourCombinations Is Nothing Then
                _colourCombinations = LinkedColourCombinations.GetLinkedColourCombinations(Me)
            End If
            Return _colourCombinations
        End Get
    End Property
    Public ReadOnly Property Equipment() As CarEquipment
        Get
            If _equipment Is Nothing Then _equipment = CarEquipment.GetEquipment(Me)
            Return _equipment
        End Get
    End Property
    Public Sub ResetEquipment()
        _equipment = Nothing
    End Sub
    Public ReadOnly Property Packs() As CarPacks
        Get
            If _packs Is Nothing Then _packs = CarPacks.GetPacks(Me)
            Return _packs
        End Get
    End Property
    Public Sub ResetPacks()
        _packs = Nothing
    End Sub

    Public ReadOnly Property Specifications() As CarSpecifications
        Get
            If _specifications Is Nothing Then
                _specifications = CarSpecifications.GetCarSpecifications(Me)
            End If
            Return _specifications
        End Get
    End Property
    Public ReadOnly Property Suffixes() As CarSuffixes
        Get
            If _suffixes Is Nothing Then
                If IsNew Then
                    _suffixes = CarSuffixes.NewCarSuffixes(Me)
                Else
                    _suffixes = CarSuffixes.GetCarSuffixes(Me)
                End If
            End If
            Return _suffixes
        End Get
    End Property
    Private Sub SuffixesListChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles _suffixes.ListChanged
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded OrElse e.ListChangedType = ComponentModel.ListChangedType.ItemDeleted Then
            RequiresRecalculation = True
            ValidationRules.CheckRules("Suffixes")
        End If
    End Sub
    Friend Sub DefaultSuffixChanged()
        RequiresRecalculation = True
        ValidationRules.CheckRules("Suffixes")
    End Sub


    Public ReadOnly Property LocalConfigurations() As LocalConfigurations
        Get
            If _localConfigurations Is Nothing Then _localConfigurations = LocalConfigurations.GetLocalConfigurations(Me)
            Return _localConfigurations
        End Get
    End Property
    Private Sub LocalConfigurationsListChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles _localConfigurations.ListChanged
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded OrElse e.ListChangedType = ComponentModel.ListChangedType.ItemDeleted Then
            RequiresRecalculation = True
            ValidationRules.CheckRules("LocalConfigurations")
        End If
    End Sub
    Friend Sub BaseLocalConfigurationChanged()
        RequiresRecalculation = True
        ValidationRules.CheckRules("LocalConfigurations")
    End Sub

    Public Property RequiresRecalculation() As Boolean
        Get
            If Not IsInSuffixOrLocalizationMode() Then Return False
            Return Not (ColoursCalculated AndAlso EquipmentCalculated AndAlso PacksCalculated AndAlso SpecificationsCalculated)
        End Get
        Set(ByVal value As Boolean)
            ColoursCalculated = Not value
            EquipmentCalculated = Not value
            PacksCalculated = Not value
            SpecificationsCalculated = Not value
        End Set
    End Property
    Public Property ColoursCalculated() As Boolean
        Get
            If Not IsInSuffixOrLocalizationMode() Then Return True
            Return _coloursCalculated
        End Get
        Set(ByVal value As Boolean)
            If Not IsInSuffixOrLocalizationMode() Then Throw New UpdateNotAllowed()
            _coloursCalculated = value
            PropertyHasChanged("ColoursCalculated")
        End Set
    End Property
    Public Property EquipmentCalculated() As Boolean
        Get
            If Not IsInSuffixOrLocalizationMode() Then Return True
            Return _optionsCalculated
        End Get
        Set(ByVal value As Boolean)
            If Not IsInSuffixOrLocalizationMode() Then Throw New UpdateNotAllowed()
            _optionsCalculated = value
            PropertyHasChanged("EquipmentCalculated")
        End Set
    End Property
    Public Property PacksCalculated() As Boolean
        Get
            If Not IsInSuffixOrLocalizationMode() Then Return True
            Return _packsCalculated
        End Get
        Set(ByVal value As Boolean)
            If Not IsInSuffixOrLocalizationMode() Then Throw New UpdateNotAllowed()
            _packsCalculated = value
            PropertyHasChanged("PacksCalculated")
        End Set
    End Property
    Public Property SpecificationsCalculated() As Boolean
        Get
            If Not IsInSuffixOrLocalizationMode() Then Return True
            Return _specificationsCalculated
        End Get
        Set(ByVal value As Boolean)
            If Not IsInSuffixOrLocalizationMode() Then Throw New UpdateNotAllowed()
            _specificationsCalculated = value
            PropertyHasChanged("SpecificationsCalculated")
        End Set
    End Property
    Public Function GenerateName() As String
        Dim builder As Text.StringBuilder = New Text.StringBuilder
        builder.Append(Generation.Model.Name & " ")
        builder.Append(BodyType.Name & " ")
        builder.Append(Engine.Name & " ")
        builder.Append(Transmission.Name & " ")
        builder.Append(Grade.Name & " ")
        Return builder.ToString().Trim()
    End Function

#End Region

#Region " Business & Validation Rules "

    Friend Sub CheckRules()
        ValidationRules.CheckRules()
    End Sub
    Private Sub RevalidatePreviewAndApprovedForAllCars()
        If Parent Is Nothing Then
            RevalidatePreviewAndApproved()
            Return
        End If

        For Each car As Car In DirectCast(Parent, Cars)
            car.RevalidatePreviewAndApproved()
        Next
    End Sub
    Private Sub RevalidatePreviewAndApproved()
        ValidationRules.CheckRules("Preview")
        ValidationRules.CheckRules("Approved")
    End Sub

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf DefaultSuffixRequired, Validation.RuleHandler), "Suffixes")
        ValidationRules.AddRule(DirectCast(AddressOf BaseLocalConfigurationRequired, Validation.RuleHandler), "LocalConfigurations")

        ValidationRules.AddRule(DirectCast(AddressOf EngineValid, Validation.RuleHandler), "EngineID")
        ValidationRules.AddRule(DirectCast(AddressOf TransmissionValid, Validation.RuleHandler), "TransmissionID")

        With MyContext.GetContext()
            If Not .SystemContext AndAlso .Brand.Equals("TOYOTA", StringComparison.InvariantCultureIgnoreCase) Then
                ValidationRules.AddRule(DirectCast(AddressOf SubModelRequired, Validation.RuleHandler), "SubModelID")
            End If
        End With

        ValidationRules.AddRule(DirectCast(AddressOf ValidateLineOffDates, Validation.RuleHandler), "LineOffDate")
        ValidationRules.AddRule(DirectCast(AddressOf PreviewValid, Validation.RuleHandler), "Preview")
        ValidationRules.AddRule(DirectCast(AddressOf ApprovedValid, Validation.RuleHandler), "Approved")

    End Sub


    Private Shared Function EngineValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim targetCar As Car = DirectCast(target, Car)

        If targetCar.EngineID.Equals(targetCar.FactoryEngineID) Then Return True
        If Not targetCar.Engine.Type.Equals(targetCar.FactoryEngine.Type) Then
            e.Description = String.Format("The marketing engine ({0}) has to be of the same type as the factory engine ({1}) for {2}", targetCar.Engine.Type.Name, targetCar.FactoryEngine.Type.Name, targetCar.Name)
            Return False
        End If

        Return True
    End Function
    Private Shared Function TransmissionValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim targetCar As Car = DirectCast(target, Car)
        Dim targetCarName As String = targetCar.Name
        If Not targetCar.Generation Is Nothing Then targetCarName = targetCar.AlternateName

        If targetCar.TransmissionID.Equals(targetCar.FactoryTransmissionID) Then Return True
        If Not targetCar.Transmission.Type.Equals(targetCar.FactoryTransmission.Type) Then
            e.Description = String.Format("The marketing transmission ({0}) has to be of the same type as the factory transmission ({1}) for {2}", targetCar.Transmission.Type.Name, targetCar.FactoryTransmission.Type.Name, targetCarName)
            Return False
        End If

        If Not targetCar.Transmission.NumberOfGears.Equals(targetCar.FactoryTransmission.NumberOfGears) Then
            e.Description = String.Format("The marketing transmission (#{0}) has to have the same number of gears as the factory transmission (#{1}) for {2}", targetCar.Transmission.NumberOfGears, targetCar.FactoryTransmission.NumberOfGears, targetCarName)
            Return False
        End If
        Return True
    End Function
    Private Shared Function DefaultSuffixRequired(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim targetCar As Car = DirectCast(target, Car)
        If Not targetCar.Mode = LocalizationMode.Suffix Then Return True
        If Not targetCar.IsNew AndAlso targetCar._suffixes Is Nothing Then Return True

        If Not targetCar.Suffixes.Any(Function(sfx) sfx.Default = True) Then
            If targetCar.Generation Is Nothing Then
                e.Description = "No defaut suffix has been selected for " & targetCar.Name
            Else
                e.Description = "No defaut suffix has been selected for " & targetCar.AlternateName
            End If
            Return False
        End If
        Return True
    End Function
    Private Shared Function BaseLocalConfigurationRequired(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim targetCar As Car = DirectCast(target, Car)
        If Not targetCar.Mode = LocalizationMode.LocalConfiguration Then Return True
        If Not targetCar.IsNew AndAlso targetCar._localConfigurations Is Nothing Then Return True

        If Not targetCar.LocalConfigurations.Any(Function(sfx) sfx.Base = True) Then
            If targetCar.Generation Is Nothing Then
                e.Description = "No base local configuration has been selected for " & targetCar.Name
            Else
                e.Description = "No base local configuration has been selected for " & targetCar.AlternateName
            End If
            Return False
        End If
        Return True
    End Function
    Private Shared Function SubModelRequired(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim otherCar As Car = DirectCast(target, Car)
        If otherCar.Generation Is Nothing Then Return True
        If otherCar.Generation.SubModels.Count = 0 Then Return True

        If otherCar.SubModel Is Nothing Then
            e.Description = "No submodel has been selected for " & otherCar.Name
            Return False
        End If
        Return True
    End Function
    Private Shared Function ValidateLineOffDates(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carToValidate = DirectCast(target, Car)
        e.Description = String.Format("The line-off start date should be before the line-off end date on car '{0}'", carToValidate.Name)
        Return carToValidate.LineOffFromDate <= carToValidate.LineOffToDate
    End Function

    Private Shared Function PreviewValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carToValidate = DirectCast(target, Car)
        Dim cars = DirectCast(carToValidate.Parent, Cars)

        'coming from database => parent hasn't been set yet, but we can trust that the data is correct => just return valid
        If cars Is Nothing Then Return True
        If Not carToValidate.Preview Then Return True

        Dim overlappingCars = cars.Where(Function(car) Not carToValidate.ID.Equals(car.ID) AndAlso carToValidate.HasSameComponents(car) AndAlso car.Preview).ToList()
        If overlappingCars.Any() Then
            e.Description = String.Format("The car can not be approved for PREVIEW because it has the same features/components as another car ('{0}').", overlappingCars.First().Name)
            Return False
        End If

        Return True
    End Function
    Private Shared Function ApprovedValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carToValidate = DirectCast(target, Car)
        Dim cars = DirectCast(carToValidate.Parent, Cars)

        'coming from database => parent hasn't been set yet, but we can trust that the data is correct => just return valid
        If cars Is Nothing Then Return True
        If Not carToValidate.Approved Then Return True

        Dim overlappingCars = cars.Where(Function(car) Not carToValidate.ID.Equals(car.ID) AndAlso carToValidate.HasSameComponents(car) AndAlso DatesOverlap(car, carToValidate) AndAlso car.Approved).ToList()
        If overlappingCars.Any() Then
            e.Description = String.Format("The car can not be approved for LIVE because it has the same features/components as another car ('{0}') with an overlapping date range ({1} - {2}).", overlappingCars.First().Name, overlappingCars.First().LineOffFromDate.ToShortDateString(), overlappingCars.First().LineOffToDate.ToShortDateString())
            Return False
        End If

        Return True
    End Function
    Private Shared Function DatesOverlap(ByVal car As Car, ByVal otherCar As Car) As Boolean
        If car.LineOffFromDate > car.LineOffToDate Then Return False
        If otherCar.LineOffFromDate > otherCar.LineOffToDate Then Return False

        Dim carDateRange = New DateRange(car.LineOffFromDate, car.LineOffToDate)
        Dim otherCarDateRange = New DateRange(otherCar.LineOffFromDate, otherCar.LineOffToDate)

        Return carDateRange.Overlaps(otherCarDateRange)
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As Car) As Boolean
        Return Not obj Is Nothing AndAlso Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is Car Then
            Return Equals(DirectCast(obj, Car))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_colourCombinations Is Nothing) AndAlso Not _colourCombinations.IsValid Then Return False
            If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
            If Not (_packs Is Nothing) AndAlso Not _packs.IsValid Then Return False
            If Not (_specifications Is Nothing) AndAlso Not _specifications.IsValid Then Return False
            If Not (_suffixes Is Nothing) AndAlso Not _suffixes.IsValid Then Return False
            If Not (_localConfigurations Is Nothing) AndAlso Not _localConfigurations.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_colourCombinations Is Nothing) AndAlso _colourCombinations.IsDirty Then Return True
            If Not (_equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
            If Not (_packs Is Nothing) AndAlso _packs.IsDirty Then Return True
            If Not (_specifications Is Nothing) AndAlso _specifications.IsDirty Then Return True
            If Not (_suffixes Is Nothing) AndAlso _suffixes.IsDirty Then Return True
            If Not (_localConfigurations Is Nothing) AndAlso _localConfigurations.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Shared Factory Method "
    Friend Shared Function NewCar(ByVal factoryCar As ModelGenerationFactoryCar, ByVal grade As ModelGenerationGrade, ByVal localizationMode As LocalizationMode) As Car
        Dim theNewCar = New Car()
        With theNewCar
            .Create()
            .FactoryGenerationID = factoryCar.FactoryGeneration.ID
            .FactoryBodyTypeID = factoryCar.BodyType.ID
            .FactoryEngineID = factoryCar.Engine.ID
            .FactoryTransmissionID = factoryCar.Transmission.ID
            .FactoryGradeID = factoryCar.Grade.ID
            ._bodyTypeID = factoryCar.BodyType.ID
            ._engineID = factoryCar.Engine.ID
            ._transmissionID = factoryCar.Transmission.ID
            ._gradeID = grade.ID
            ._wheelDriveID = factoryCar.WheelDrive.ID
            ._steeringID = factoryCar.Steering.ID
            .Mode = localizationMode
            .ValidationRules.CheckRules()
            .MarkAsChild()
        End With
        Return theNewCar
    End Function

    Public Shared Sub PromoteCar(ByVal car As Car)
        If car Is Nothing Then Throw New InvalidCarIdentifier
        If Not (car.Approved OrElse car.Preview) Then Throw New CarNeedsToBeApproved
        ContextCommand.Execute(New PromoteCarCommand(car.ID, True))
        car.Promoted = True
    End Sub
    Public Shared Sub DePromoteCar(ByVal car As Car)
        If car Is Nothing Then Throw New InvalidCarIdentifier
        If car.Promoted Then
            ContextCommand.Execute(New PromoteCarCommand(car.ID, False))
            car.Promoted = False
        End If
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _owner = MyContext.GetContext().CountryCode
        _coloursCalculated = False
        _optionsCalculated = False
        _specificationsCalculated = False
        _packsCalculated = False
        _lineOffFromDate = DateTime.MinValue
        _lineOffToDate = DateTime.MaxValue

        If MyContext.GetContext().IsGlobal Then
            _status = Status.Declined
        Else
            _status = Status.Declined + Status.AvailableToNmscs
        End If
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        With dataReader
            _shortID = dataReader.GetInt32("SHORTID")
            _owner = .GetString("OWNER")
            _mode = DirectCast(.GetInt16("LOCALIZATIONMODE"), LocalizationMode)
            _versionID = .GetGuid("VERSIONID")
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _status = .GetInt16("STATUSID")
            _index = .GetInt16("SORTORDER")
            _webVisible = .GetBoolean("WEBVISIBLE")
            _configVisible = .GetBoolean("CONFIGVISIBLE")
            _financeVisible = .GetBoolean("FINANCEVISIBLE")
            _price = Environment.ConvertPrice(CType(.GetValue("PRICEEXVAT"), Decimal), .GetString("CURRENCY"))
            _vatPrice = Environment.ConvertPrice(CType(.GetValue("PRICEINVAT"), Decimal), .GetString("CURRENCY"))

            _factoryGenerationID = .GetGuid("FACTORYGENERATIONID")
            _factoryBodyTypeID = .GetGuid("FACTORYBODYID")
            _factoryEngineID = .GetGuid("FACTORYENGINEID")
            _factoryTransmissionID = .GetGuid("FACTORYTRANSMISSIONID")
            _factoryGradeID = .GetGuid("FACTORYGRADEID")

            _bodyTypeID = .GetGuid("BODYID")
            _engineID = .GetGuid("ENGINEID")
            _gradeID = .GetGuid("GRADEID")
            _transmissionID = .GetGuid("TRANSMISSIONID")
            _wheelDriveID = .GetGuid("WHEELDRIVEID")
            _steeringID = .GetGuid("STEERINGID")
            _subModelID = .GetGuid("SUBMODELID")

            _coloursCalculated = .GetBoolean("COLOURSCALCULATED")
            _optionsCalculated = .GetBoolean("OPTIONSCALCULATED")
            _packsCalculated = .GetBoolean("PACKSCALCULATED")
            _specificationsCalculated = .GetBoolean("SPECIFICATIONSCALCULATED")
            _lineOffFromDate = .GetSmartDate("LINEOFFFROMDATE", True).Date
            _lineOffToDate = .GetSmartDate("LINEOFFTODATE", False).Date


        End With

        If Not (Approved OrElse Declined) Then Declined = True

        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command.Parameters
            .AddWithValue("@FACTORYGENERATIONID", FactoryGenerationID)
            .AddWithValue("@FACTORYBODYID", FactoryBodyTypeID)
            .AddWithValue("@FACTORYENGINEID", FactoryEngineID)
            .AddWithValue("@FACTORYGRADEID", FactoryGradeID)
            .AddWithValue("@FACTORYTRANSMISSIONID", FactoryTransmissionID)
        End With

        AddUpdateCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command.Parameters
            .AddWithValue("@INTERNALCODE", Code)
            .AddWithValue("@LOCALCODE", LocalCode)
            .AddWithValue("@SHORTNAME", Name)
            .AddWithValue("@PRICEEXVAT", Price)
            .AddWithValue("@PRICEINVAT", VatPrice)
            .AddWithValue("@CURRENCY", MyContext.GetContext().Currency.Code)
            .AddWithValue("@STATUSID", _status)
            .AddWithValue("@SORTORDER", Index)
            .AddWithValue("@WEBVISIBLE", WebVisible)
            .AddWithValue("@CONFIGVISIBLE", ConfigVisible)
            .AddWithValue("@FINANCEVISIBLE", FinanceVisible)
            .AddWithValue("@COLOURSCALCULATED", ColoursCalculated)
            .AddWithValue("@PACKSCALCULATED", PacksCalculated)
            .AddWithValue("@OPTIONSCALCULATED", EquipmentCalculated)
            .AddWithValue("@SPECIFICATIONSCALCULATED", SpecificationsCalculated)
            If SubModelID.Equals(Guid.Empty) Then
                .AddWithValue("@SUBMODELID", DBNull.Value)
            Else
                .AddWithValue("@SUBMODELID", SubModel.ID)
            End If
            .AddWithValue("@LINEOFFFROMDATE", New SmartDate(_lineOffFromDate, True).DBValue)
            .AddWithValue("@LINEOFFTODATE", New SmartDate(_lineOffToDate, False).DBValue)
            .AddWithValue("@MODELID", Generation.Model.ID)
            .AddWithValue("@GENERATIONID", Generation.ID)
            .AddWithValue("@BODYID", BodyTypeID)
            .AddWithValue("@ENGINEID", EngineID)
            .AddWithValue("@GRADEID", GradeID)
            .AddWithValue("@TRANSMISSIONID", TransmissionID)
            .AddWithValue("@WHEELDRIVEID", WheelDriveID)
            .AddWithValue("@STEERINGID", SteeringID)
            .AddWithValue("@VERSIONID", VersionID)
        End With
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not (_colourCombinations Is Nothing) Then _colourCombinations.Update(transaction)
        If Not (_equipment Is Nothing) Then _equipment.Update(transaction)
        If Not (_packs Is Nothing) Then _packs.Update(transaction)
        If Not (_specifications Is Nothing) Then _specifications.Update(transaction)
        If Not (_suffixes Is Nothing) Then _suffixes.Update(transaction)
        If Not (_localConfigurations Is Nothing) Then _localConfigurations.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

#Region " PromotedCar Helper Class "

    <Serializable()> Private Class PromoteCarCommand
        Inherits ContextCommand.CommandInfo

        Private ReadOnly _carId As Guid
        Private ReadOnly _promote As Boolean

        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "promoteCar"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CARID", _carId)
            command.Parameters.AddWithValue("@PROMOTE", _promote)
        End Sub

        Public Sub New(ByVal carId As Guid, ByVal promote As Boolean)
            _carId = carId
            _promote = promote
        End Sub

    End Class
#End Region

#Region " Base Object Overrides "

    Public Overrides ReadOnly Property ModelID() As Guid
        Get
            Return Generation.Model.ID
        End Get
    End Property
    Public Overrides ReadOnly Property GenerationID() As Guid
        Get
            Return Generation.ID
        End Get
    End Property

    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.CAR
        End Get
    End Property



#End Region



End Class