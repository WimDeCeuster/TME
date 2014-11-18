Imports TME.CarConfigurator.Administration.Configurations
Imports TME.CarConfigurator.Administration.Assets
Imports TME.BusinessObjects.Templates.SqlServer
Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.Normalizers
Imports TME.BusinessObjects.Validation
Imports Rules = TME.BusinessObjects.ValidationRules
Imports System.Collections.Generic

<Serializable()> Public NotInheritable Class ModelGenerations
    Inherits ContextUniqueGuidListBase(Of ModelGenerations, ModelGeneration)

#Region " Delegates & Events "
    Friend Delegate Sub GenerationAvailabilityChangedHandler(ByVal items As ModelGenerations)
    Friend Event GenerationAvailabilityChanged As GenerationAvailabilityChangedHandler
#End Region

#Region " Business Properties & Methods "

    Private Property Model() As Model
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, Model)
        End Get
        Set(ByVal value As Model)
            SetParent(value)
        End Set
    End Property
    Public Sub Approve(ByVal generation As ModelGeneration)
        If Not generation.Approved Then
            'Disapprove all others
            For Each obj As ModelGeneration In Me
                obj.Approved = False
            Next
            'Approve Me
            generation.Approved = True
            RaiseEvent GenerationAvailabilityChanged(Me)
        End If
    End Sub
    Public Sub Disapprove(ByVal generation As ModelGeneration)
        Dim generationFromList As ModelGeneration = Item(generation.ID)
        If generationFromList.Approved Then
            generationFromList.Approved = False
            RaiseEvent GenerationAvailabilityChanged(Me)
        End If
    End Sub

    Public WriteOnly Property SetPreview(ByVal generation As ModelGeneration) As Boolean
        Set(ByVal value As Boolean)
            If Not generation.Preview.Equals(value) Then
                For Each obj As ModelGeneration In Me
                    obj.Preview = False
                Next
                generation.Preview = value
                RaiseEvent GenerationAvailabilityChanged(Me)
            End If
        End Set
    End Property

    Public Overloads Overrides Function Add() As ModelGeneration
        Dim generation As ModelGeneration = ModelGeneration.NewModelGeneration(Model)
        Add(generation)
        Return generation
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerations(ByVal model As Model) As ModelGenerations
        Dim generations As ModelGenerations = New ModelGenerations
        generations.Model = model
        Return generations
    End Function
    Friend Shared Function GetModelGenerations(ByVal model As Model) As ModelGenerations
        Dim generations As ModelGenerations = DataPortal.Fetch(Of ModelGenerations)(New CustomCriteria(model))
        generations.Model = model
        Return generations
    End Function
    Friend Shared Function GetModelGenerations(ByVal model As Model, ByVal dataReader As SafeDataReader) As ModelGenerations
        Dim generations As ModelGenerations = New ModelGenerations
        generations.Fetch(dataReader)
        generations.Model = model
        Return generations
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = True
        AllowNew = MyContext.GetContext().IsGlobal
        AllowRemove = Thread.CurrentPrincipal.IsInRole("ISG Administrator")
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _modelID As Guid

        Public Sub New(ByVal model As Model)
            _modelID = model.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@MODELID", _modelID)
        End Sub

    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGeneration
    Inherits LocalizeableBusinessBase

#Region " Delegates & Events "
    Friend Delegate Sub CarAvailabilityChangedHandler()
    Friend Delegate Sub CarRemovedHandler()
    Friend Delegate Sub ColourAvailabilityChangedHandler()

    Friend Event CarAvailabilityChanged As CarAvailabilityChangedHandler
    Friend Event CarRemoved As CarRemovedHandler
    Friend Event ColourAvailabilityChanged As ColourAvailabilityChangedHandler

    Friend Sub RaiseColourAvailabilityChanged()
        RaiseEvent ColourAvailabilityChanged()
    End Sub

#End Region

#Region " Business Properties & Methods "
    Private _approvalChanged As Boolean = False
    Private _approvalCarsChanged As Boolean = False

    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _status As Integer
    Private _changeType As ChangeType


    Private _isFullyMapped As Boolean
    Private _suffixModeAvailable As Boolean
    Private _localConfigurationModeAvailable As Boolean
    Private _masterProjectID As Guid
    Private _masterProjectDescription As String
    Private _masterSubProjectID As Guid
    Private _masterSubProjectDescription As String
    Private _calculateOptionRules As Boolean
    Private _calculateOptionApplicabilities As Boolean
    Private _bluePrinted As Boolean

    Private _assets As LinkedAssets
    Private _colours As ModelGenerationColourCombinations
    Private _specifications As ModelGenerationSpecifications
    Private _equipment As ModelGenerationEquipment
    Private _packs As ModelGenerationPacks

    Private WithEvents _cars As Cars
    Private _bodyTypes As ModelGenerationBodyTypes
    Private _engines As ModelGenerationEngines
    Private _transmissions As ModelGenerationTransmissions
    Private _grades As ModelGenerationGrades
    Private _wheelDrives As ModelGenerationWheelDrives
    Private _subModels As ModelGenerationSubModels
    Private _gradeClassifications As ModelGenerationGradeClassifications

    Private _factoryGenerations As ModelGenerationFactoryGenerations
    Private _assetTypes As ModelGenerationAssetTypes
    Private _versions As ModelGenerationCarConfiguratorVersions
    Private _activeVersion As ModelGenerationCarConfiguratorVersion

    Private _carParts As ModelGenerationCarParts
    Private _defaultConfigurations As DefaultConfigurations
    Private _model As ModelInfo

    Private _mode As LocalizationMode
    Private _isPPOIntegrationEnabled As Boolean = False
    Private _isA2AIntegrationEnabled As Boolean = False

    Private _canSynchronizeVOMPrices As Boolean
    Private _canSynchronizePPOPrices As Boolean
    Private _canSynchronizeAccessoryPrices As Boolean



    Public ReadOnly Property Model() As ModelInfo
        Get
            Return _model
        End Get
    End Property

    Public Property Code() As String
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

    Public Property ChangeType() As ChangeType
        Get
            Return _changeType
        End Get
        Set(ByVal value As ChangeType)
            If _changeType Is Nothing Then
                If Not value Is Nothing Then
                    _changeType = value
                    PropertyHasChanged("ChangeType")
                End If
            Else
                If value Is Nothing OrElse Not value.Equals(_changeType) Then
                    _changeType = value
                    PropertyHasChanged("ChangeType")
                End If
            End If
        End Set
    End Property

    Public Property Activated() As Boolean
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
    Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Friend Set(ByVal value As Boolean)
            If value.Equals(Approved) Then Return

            _approvalChanged = True
            If Approved Then
                _status -= Status.ApprovedForLive
            Else
                _status += Status.ApprovedForLive
            End If
            PropertyHasChanged("Approved")
        End Set
    End Property
    Public Property Preview() As Boolean
        Get
            Return ((_status And Status.ApprovedForPreview) = Status.ApprovedForPreview)
        End Get
        Friend Set(ByVal value As Boolean)
            If value.Equals(Preview) Then Return

            _approvalChanged = True
            If Preview Then
                _status -= Status.ApprovedForPreview
            Else
                _status += Status.ApprovedForPreview
            End If
            PropertyHasChanged("Preview")
        End Set
    End Property
    Public ReadOnly Property BluePrinting() As Boolean
        Get
            Return ((_status And Status.BluePrinting) = Status.BluePrinting)
        End Get
    End Property
    Public ReadOnly Property Copying() As Boolean
        Get
            Return ((_status And Status.Copying) = Status.Copying)
        End Get
    End Property

    Public Property SuffixModeAvailable() As Boolean
        Get
            Return _suffixModeAvailable
        End Get
        Set(value As Boolean)
            If value.Equals(SuffixModeAvailable) Then Return
            If Not IsNew Then Throw New ApplicationException("SuffixModeAvailable is immutable and can not be changed")

            _suffixModeAvailable = value
            PropertyHasChanged("SuffixModeAvailable")
        End Set
    End Property

    Public Property LocalConfigurationModeAvailable() As Boolean
        Get
            Return _localConfigurationModeAvailable
        End Get
        Set(value As Boolean)
            If value.Equals(LocalConfigurationModeAvailable) Then Return
            If Not IsNew Then Throw New ApplicationException("LocalConfigurationModeAvailable is immutable and can not be changed")

            _localConfigurationModeAvailable = value
            PropertyHasChanged("LocalConfigurationModeAvailable")
        End Set
    End Property


    Public Property Mode As LocalizationMode
        Get
            Return _mode
        End Get
        Set(ByVal value As LocalizationMode)
            If value.Equals(_mode) Then Return
            _mode = value
            PropertyHasChanged("Mode")
        End Set
    End Property
    Public ReadOnly Property BluePrinted() As Boolean
        Get
            Return _bluePrinted
        End Get
    End Property

    Public Property MasterProjectID() As Guid
        Get
            Return _masterProjectID
        End Get
        Set(ByVal value As Guid)
            If _masterProjectID.Equals(value) Then Return

            _masterProjectID = value
            If _masterProjectID.Equals(Guid.Empty) Then _masterProjectDescription = String.Empty
            PropertyHasChanged("MasterProjectID")
            ValidationRules.CheckRules("Master")
        End Set
    End Property

    Public Property MasterProjectDescription() As String
        Get
            Return _masterProjectDescription
        End Get
        Set(ByVal value As String)
            If _masterProjectDescription.Equals(value) Then Return

            _masterProjectDescription = value
            PropertyHasChanged("MasterProjectDescription")
            ValidationRules.CheckRules("Master")
        End Set
    End Property
    Public Property MasterSubProjectID() As Guid
        Get
            Return _masterSubProjectID
        End Get
        Set(ByVal value As Guid)
            If _masterSubProjectID.Equals(value) Then Return
            _masterSubProjectID = value
            If _masterSubProjectID.Equals(Guid.Empty) Then _masterSubProjectDescription = String.Empty
            PropertyHasChanged("MasterSubProjectID")
            ValidationRules.CheckRules("Master")
        End Set
    End Property
    Public Property MasterSubProjectDescription() As String
        Get
            Return _masterSubProjectDescription
        End Get
        Set(ByVal value As String)
            If _masterSubProjectDescription.Equals(value) Then Return

            _masterSubProjectDescription = value
            PropertyHasChanged("MasterSubProjectDescription")
            ValidationRules.CheckRules("Master")
        End Set
    End Property



    Public Property CalculateOptionRules() As Boolean
        Get
            Return _calculateOptionRules AndAlso IsSuffixOrLocalConfigurationMode()
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(_calculateOptionRules) Then Exit Property
            _calculateOptionRules = value
            PropertyHasChanged("CalculateOptionRules")
            ValidationRules.CheckRules("Mode")
        End Set
    End Property
    Public Property CalculateOptionApplicabilities() As Boolean
        Get
            Return _calculateOptionApplicabilities AndAlso IsSuffixOrLocalConfigurationMode()
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(_calculateOptionApplicabilities) Then Exit Property
            _calculateOptionApplicabilities = value
            PropertyHasChanged("CalculateOptionApplicabilities")
            ValidationRules.CheckRules("Mode")
        End Set
    End Property

    Public Property PPOIntegrationEnabled As Boolean
        Get
            Return _isPPOIntegrationEnabled
        End Get
        Set(value As Boolean)
            If value = _isPPOIntegrationEnabled Then Return
            _isPPOIntegrationEnabled = value

            If Not _isPPOIntegrationEnabled Then CanSynchronizePPOPrices = False

            PropertyHasChanged("PPOIntegrationEnabled")
        End Set
    End Property
    Public Property A2AIntegrationEnabled As Boolean
        Get
            If Not MyContext.GetContext().Country.LocalizedInA2A Then Return False
            Return _isA2AIntegrationEnabled
        End Get
        Set(value As Boolean)
            If value = _isA2AIntegrationEnabled Then Return
            _isA2AIntegrationEnabled = value

            If Not _isA2AIntegrationEnabled Then CanSynchronizeAccessoryPrices = False

            PropertyHasChanged("A2AIntegrationEnabled")
        End Set
    End Property

    Public Property CanSynchronizeVOMPrices() As Boolean
        Get
            Return _canSynchronizeVOMPrices
        End Get
        Set(value As Boolean)
            If value.Equals(_canSynchronizeVOMPrices) Then Return
            _canSynchronizeVOMPrices = value
            PropertyHasChanged("CanSynchronizeVOMPrices")
        End Set
    End Property
    Public Property CanSynchronizePPOPrices() As Boolean
        Get
            If Not PPOIntegrationEnabled Then Return False
            Return _canSynchronizePPOPrices
        End Get
        Set(value As Boolean)
            If value.Equals(_canSynchronizePPOPrices) Then Return
            _canSynchronizePPOPrices = value
            PropertyHasChanged("CanSynchronizePPOPrices")
        End Set
    End Property
    Public Property CanSynchronizeAccessoryPrices() As Boolean
        Get
            If Not A2AIntegrationEnabled Then Return False
            Return _canSynchronizeAccessoryPrices
        End Get
        Set(value As Boolean)
            If value.Equals(_canSynchronizeAccessoryPrices) Then Return
            _canSynchronizeAccessoryPrices = value
            PropertyHasChanged("CanSynchronizeAccessoryPrices")
        End Set
    End Property



    Private ReadOnly Property IsSuffixOrLocalConfigurationMode() As Boolean
        Get
            Return Mode = LocalizationMode.Suffix OrElse Mode = LocalizationMode.LocalConfiguration
        End Get
    End Property

    Public ReadOnly Property IsFullyMapped() As Boolean
        Get
            If _factoryGenerations Is Nothing Then
                Return _isFullyMapped
            Else
                Return Not _factoryGenerations.Any(Function(g) Not g.IsFullyMapped)
            End If
        End Get
    End Property

    Public ReadOnly Property Cars() As Cars
        Get
            If _cars Is Nothing Then
                _cars = If(IsNew, Cars.NewCars(Me), Cars.GetCars(Me))
            End If
            Return _cars
        End Get
    End Property
    Public ReadOnly Property BodyTypes() As ModelGenerationBodyTypes
        Get
            If _bodyTypes Is Nothing Then
                If IsNew Then
                    _bodyTypes = ModelGenerationBodyTypes.NewModelGenerationBodyTypes(Me)
                Else
                    _bodyTypes = ModelGenerationBodyTypes.GetModelGenerationBodyTypes(Me)
                End If
                _bodyTypes.Synchronize()
            End If
            Return _bodyTypes
        End Get
    End Property
    Public ReadOnly Property Engines() As ModelGenerationEngines
        Get
            If _engines Is Nothing Then
                If IsNew Then
                    _engines = ModelGenerationEngines.NewModelGenerationEngines(Me)
                Else
                    _engines = ModelGenerationEngines.GetModelGenerationEngines(Me)
                End If
                _engines.Synchronize()
            End If
            Return _engines
        End Get
    End Property
    Public ReadOnly Property Transmissions() As ModelGenerationTransmissions
        Get
            If _transmissions Is Nothing Then
                If IsNew Then
                    _transmissions = ModelGenerationTransmissions.NewModelGenerationTransmissions(Me)
                Else
                    _transmissions = ModelGenerationTransmissions.GetModelGenerationTransmissions(Me)
                End If
                _transmissions.Synchronize()
            End If
            Return _transmissions
        End Get
    End Property
    Public ReadOnly Property Grades() As ModelGenerationGrades
        Get
            If _grades Is Nothing Then
                If IsNew Then
                    _grades = ModelGenerationGrades.NewModelGenerationGrades(Me)
                Else
                    _grades = ModelGenerationGrades.GetModelGenerationGrades(Me)
                End If
                _grades.Synchronize()
            End If
            Return _grades
        End Get
    End Property
    Public ReadOnly Property GradeClassifications() As ModelGenerationGradeClassifications
        Get
            If _gradeClassifications Is Nothing Then _gradeClassifications = ModelGenerationGradeClassifications.GetModelGenerationGradeClassifications(Me)
            Return _gradeClassifications
        End Get
    End Property

    Public ReadOnly Property WheelDrives() As ModelGenerationWheelDrives
        Get
            If _wheelDrives Is Nothing Then
                If IsNew Then
                    _wheelDrives = ModelGenerationWheelDrives.NewModelGenerationWheelDrives(Me)
                Else
                    _wheelDrives = ModelGenerationWheelDrives.GetModelGenerationWheelDrives(Me)
                End If
                _wheelDrives.Synchronize()
            End If
            Return _wheelDrives
        End Get
    End Property
    Public ReadOnly Property SubModels() As ModelGenerationSubModels
        Get
            If _subModels Is Nothing Then
                If IsNew Then
                    _subModels = ModelGenerationSubModels.NewModelGenerationSubModels(Me)
                Else
                    _subModels = ModelGenerationSubModels.GetModelGenerationSubModels(Me)
                End If
                _subModels.Synchronize()
            End If
            Return _subModels
        End Get
    End Property

    Public ReadOnly Property FactoryGenerations() As ModelGenerationFactoryGenerations
        Get
            If _factoryGenerations Is Nothing Then
                If IsNew Then
                    _factoryGenerations = ModelGenerationFactoryGenerations.NewGenerationFactoryGenerations(Me)
                Else
                    _factoryGenerations = ModelGenerationFactoryGenerations.GetGenerationFactoryGenerations(Me)
                End If
            End If
            Return _factoryGenerations
        End Get
    End Property

    Public Function GetFactoryOptionValues() As IEnumerable(Of ModelGenerationFactoryOptionValue)
        Return (From factoryGeneration In FactoryGenerations
         From factoryOption In factoryGeneration.Options
         From factoryOptionValue In factoryOption.Values
         Select factoryOptionValue)
    End Function


    Public ReadOnly Property AssetTypes() As ModelGenerationAssetTypes
        Get
            If _assetTypes Is Nothing Then
                If IsNew Then
                    _assetTypes = ModelGenerationAssetTypes.NewModelGenerationAssetTypes(Me)
                Else
                    _assetTypes = ModelGenerationAssetTypes.GetModelGenerationAssetTypes(Me)
                End If
            End If
            Return _assetTypes
        End Get
    End Property
    Public ReadOnly Property CarConfiguratorVersions() As ModelGenerationCarConfiguratorVersions
        Get
            If _versions Is Nothing Then
                If IsNew Then
                    _versions = ModelGenerationCarConfiguratorVersions.NewModelGenerationCarConfiguratorVersions(Me)
                Else
                    _versions = ModelGenerationCarConfiguratorVersions.GetModelGenerationCarConfiguratorVersions(Me)
                End If
            End If
            Return _versions
        End Get
    End Property
    Public Property ActiveCarConfiguratorVersion() As ModelGenerationCarConfiguratorVersion
        Get
            Return _activeVersion
        End Get
        Set(ByVal value As ModelGenerationCarConfiguratorVersion)
            If Not value.IsEmpty() AndAlso Not (Equals(value.Generation)) Then Throw New ArgumentException("You can only select a version that was assigned to the generation!")
            If Not value.Equals(_activeVersion) Then
                _activeVersion = value
                PropertyHasChanged("ActiveCarConfiguratorVersion")
            End If
        End Set
    End Property

    Public ReadOnly Property CarParts() As ModelGenerationCarParts
        Get
            If _carParts Is Nothing Then
                If IsNew Then
                    _carParts = ModelGenerationCarParts.NewModelGenerationCarParts(Me)
                Else
                    _carParts = ModelGenerationCarParts.GetModelGenerationCarParts(Me)
                End If
            End If
            Return _carParts
        End Get
    End Property
    
    Public Sub ChangeReference(ByVal updatedCarParts As ModelGenerationCarParts)
        _carParts = updatedCarParts
    End Sub

    Private Sub CarsCarAvailabilityChanged(ByVal changedCar As Car) Handles _cars.CarAvailabilityChanged
        'Could be the case when a car was approved / declined or a body type/submodel/engine/grade has changed...
        SynchronizeDescendants()

        _approvalCarsChanged = True
        ValidationRules.CheckRules()

        RaiseEvent CarAvailabilityChanged()
    End Sub
    Private Sub CarsListChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ListChangedEventArgs) Handles _cars.ListChanged
        If Not e.ListChangedType.Equals(ComponentModel.ListChangedType.ItemAdded) OrElse e.ListChangedType.Equals(ComponentModel.ListChangedType.ItemDeleted) Then Return

        SynchronizeDescendants()


        If e.ListChangedType.Equals(ComponentModel.ListChangedType.ItemDeleted) Then RaiseEvent CarRemoved()
    End Sub

    Private Sub SynchronizeDescendants()
        BodyTypes.Synchronize()
        SubModels.Synchronize()
        Engines.Synchronize()
        Grades.Synchronize()
        Transmissions.Synchronize()
        WheelDrives.Synchronize()
    End Sub

    Public ReadOnly Property Assets() As LinkedAssets
        Get
            If _assets Is Nothing Then
                If IsNew Then
                    _assets = LinkedAssets.NewLinkedAssets(ID, Model.ID)
                Else
                    _assets = LinkedAssets.GetLinkedAssets(ID, Model.ID)
                End If
            End If
            Return _assets
        End Get
    End Property
    Public ReadOnly Property ColourCombinations() As ModelGenerationColourCombinations
        Get
            If _colours Is Nothing Then
                If IsNew Then
                    _colours = ModelGenerationColourCombinations.NewModelGenerationColourCombinations(Me)
                Else
                    _colours = ModelGenerationColourCombinations.GetModelGenerationColourCombinations(Me)
                End If
            End If
            Return _colours
        End Get
    End Property
    Public ReadOnly Property Specifications() As ModelGenerationSpecifications
        Get
            If _specifications Is Nothing Then _specifications = ModelGenerationSpecifications.GetModelGenerationSpecifications(Me)
            Return _specifications
        End Get
    End Property
    Public ReadOnly Property Equipment() As ModelGenerationEquipment
        Get
            If _equipment Is Nothing Then
                If IsNew Then
                    _equipment = ModelGenerationEquipment.NewEquipment(Me)
                Else
                    _equipment = ModelGenerationEquipment.GetEquipment(Me)
                End If
            End If
            Return _equipment
        End Get
    End Property
    Public ReadOnly Property Packs() As ModelGenerationPacks
        Get
            If _packs Is Nothing Then
                If IsNew Then
                    _packs = ModelGenerationPacks.NewModelGenerationPacks(Me)
                Else
                    _packs = ModelGenerationPacks.GetModelGenerationPacks(Me)
                End If
            End If
            Return _packs
        End Get
    End Property
    Public ReadOnly Property DefaultConfigurations() As DefaultConfigurations
        Get
            If _defaultConfigurations Is Nothing Then _defaultConfigurations = DefaultConfigurations.GetDefaultConfigurations(Me)
            Return _defaultConfigurations
        End Get
    End Property
    Public Function GetInfo() As ModelGenerationInfo
        Return ModelGenerationInfo.GetModelGenerationInfo(Me)
    End Function
    Public Function GetFactoryCars() As IEnumerable(Of ModelGenerationFactoryCar)
        Return (
            From factorygeneration In FactoryGenerations
            From factoryCar In factorygeneration.FactoryCars
            Select factoryCar
          )
    End Function

    Public Overloads Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If MyContext.GetContext().IsGlobal Then Return MyBase.CanWriteProperty(propertyName)

        Select Case propertyName
            Case "CalculateOptionApplicabilities", "CalculateOptionRules", "Approved", "Preview", "Mode", "LocalCode", "ActiveCarConfiguratorVersion", "MasterProjectID", "MasterSubProjectID", "MasterProjectDescription", "MasterSubProjectDescription", "PPOIntegrationEnabled", "A2AIntegrationEnabled", "CanSynchronizeVOMPrices", "CanSynchronizePPOPrices", "CanSynchronizeAccessoryPrices"
                Return MyBase.CanWriteProperty(propertyName)
            Case Else
                Return False
        End Select

    End Function
#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, RuleHandler), "ChangeType")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf ApprovedRules, RuleHandler), "Approved")
        ValidationRules.AddRule(DirectCast(AddressOf PreviewRules, RuleHandler), "Preview")

        ValidationRules.AddRule(DirectCast(AddressOf CarConfiguratorVersionRules, RuleHandler), "Approved")
        ValidationRules.AddRule(DirectCast(AddressOf CarConfiguratorVersionRules, RuleHandler), "Preview")

        ValidationRules.AddRule(DirectCast(AddressOf CarConfiguratorVersionRules, RuleHandler), "ActiveCarConfiguratorVersion")
        ValidationRules.AddRule(DirectCast(AddressOf MasterValid, RuleHandler), "Master")

        ValidationRules.AddRule(DirectCast(AddressOf ModeValid, RuleHandler), "Mode")
    End Sub

    Private Shared Function ApprovedRules(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim generation As ModelGeneration = DirectCast(target, ModelGeneration)
        If (generation._approvalChanged OrElse generation._approvalCarsChanged) AndAlso generation.Approved Then
            If generation.Cars.Any(Function(car) car.Approved) Then
                Return True
            End If
            If generation._approvalChanged Then
                e.Description = "You can <b>not</b> approve " & generation.Name & " for ""LIVE"" at this moment." & System.Environment.NewLine & "You must approve at least one vehicle before you can continue."
            Else
                e.Description = "You can <b>not</b> unapprove all cars for ""LIVE"" as long as the generation is approved." & System.Environment.NewLine & "You must approve at least one vehicle or unapprove the generation."
            End If
            Return False
        End If
        Return True
    End Function
    Private Shared Function PreviewRules(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim generation As ModelGeneration = DirectCast(target, ModelGeneration)
        If (generation._approvalChanged OrElse generation._approvalCarsChanged) AndAlso generation.Preview Then
            If generation.Cars.Any(Function(car) car.Preview) Then
                Return True
            End If
            If generation._approvalChanged Then
                e.Description = "You can <b>not</b> approve " & generation.Name & " for ""PREVIEW"" at this moment." & System.Environment.NewLine & "You must approve at least one vehicle before you can continue."
            Else
                e.Description = "You can <b>not</b> unapprove all cars for ""PREVIEW"" as long as the generation is approved." & System.Environment.NewLine & "You must approve at least one vehicle or unapprove the generation."
            End If
            Return False
        End If
        Return True
    End Function
    Private Shared Function CarConfiguratorVersionRules(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim generation As ModelGeneration = DirectCast(target, ModelGeneration)
        If Not generation.ActiveCarConfiguratorVersion.IsEmpty() Then Return True
        If Not (generation.Preview OrElse generation.Approved) Then Return True

        e.Description = "No Car Configurator version has been selected for " & generation.Name & "." & System.Environment.NewLine & "You need to select a version before you can continue."
        Return False

    End Function
    Private Shared Function MasterValid(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim currentGeneration = DirectCast(target, ModelGeneration)

        e.Description = "If either the master project id or the master sub project id are filled out, the other one should be filled out too"
        If HasOnlyOneMasterIdFilledOut(currentGeneration) Then Return False

        e.Description = "The description of the master project ID is required"
        If HasIdButNoDescription(currentGeneration.MasterProjectID, currentGeneration.MasterProjectDescription) Then Return False

        e.Description = "The description of the master sub project ID is required"
        If HasIdButNoDescription(currentGeneration.MasterSubProjectID, currentGeneration.MasterSubProjectDescription) Then Return False

        Return True
    End Function
    Private Shared Function HasOnlyOneMasterIdFilledOut(ByVal currentGeneration As ModelGeneration) As Boolean
        Return currentGeneration.MasterProjectID.Equals(Guid.Empty) Xor currentGeneration.MasterSubProjectID.Equals(Guid.Empty)
    End Function
    Private Shared Function HasIdButNoDescription(ByVal id As Guid, ByVal description As String) As Boolean
        Return Not id.Equals(Guid.Empty) AndAlso String.IsNullOrEmpty(description)
    End Function
    Private Shared Function ModeValid(ByVal target As Object, ByVal e As RuleArgs) As Boolean
        Dim currentGeneration = DirectCast(target, ModelGeneration)

        e.Description = "Local configuration mode is not available on Global"
        Return Not (currentGeneration.Mode = LocalizationMode.LocalConfiguration AndAlso MyContext.GetContext().IsGlobal())
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If ChangeType Is Nothing Then Return Name
        Return ChangeType.Code & " " & Name
    End Function
#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_cars Is Nothing) AndAlso Not _cars.IsValid Then Return False
            If Not (_bodyTypes Is Nothing) AndAlso Not _bodyTypes.IsValid Then Return False
            If Not (_engines Is Nothing) AndAlso Not _engines.IsValid Then Return False
            If Not (_grades Is Nothing) AndAlso Not _grades.IsValid Then Return False
            If Not (_gradeClassifications Is Nothing) AndAlso Not _gradeClassifications.IsValid Then Return False
            If Not (_transmissions Is Nothing) AndAlso Not _transmissions.IsValid Then Return False
            If Not (_wheelDrives Is Nothing) AndAlso Not _wheelDrives.IsValid Then Return False
            If Not (_subModels Is Nothing) AndAlso Not _subModels.IsValid Then Return False

            If Not (_assets Is Nothing) AndAlso Not _assets.IsValid Then Return False
            If Not (_colours Is Nothing) AndAlso Not _colours.IsValid Then Return False
            If Not (_specifications Is Nothing) AndAlso Not _specifications.IsValid Then Return False
            If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
            If Not (_packs Is Nothing) AndAlso Not _packs.IsValid Then Return False

            If Not (_factoryGenerations Is Nothing) AndAlso Not _factoryGenerations.IsValid Then Return False
            If Not (_assetTypes Is Nothing) AndAlso Not _assetTypes.IsValid Then Return False
            If Not (_versions Is Nothing) AndAlso Not _versions.IsValid Then Return False

            If Not (_carParts Is Nothing) AndAlso Not _carParts.IsValid Then Return False
            If Not (_defaultConfigurations Is Nothing) AndAlso Not _defaultConfigurations.IsValid Then Return False

            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True

            If Not (_cars Is Nothing) AndAlso _cars.IsDirty Then Return True
            If Not (_bodyTypes Is Nothing) AndAlso _bodyTypes.IsDirty Then Return True
            If Not (_engines Is Nothing) AndAlso _engines.IsDirty Then Return True
            If Not (_grades Is Nothing) AndAlso _grades.IsDirty Then Return True
            If Not (_gradeClassifications Is Nothing) AndAlso _gradeClassifications.IsDirty Then Return True
            If Not (_transmissions Is Nothing) AndAlso _transmissions.IsDirty Then Return True
            If Not (_wheelDrives Is Nothing) AndAlso _wheelDrives.IsDirty Then Return True
            If Not (_subModels Is Nothing) AndAlso _subModels.IsDirty Then Return True

            If Not (_assets Is Nothing) AndAlso _assets.IsDirty Then Return True
            If Not (_colours Is Nothing) AndAlso _colours.IsDirty Then Return True
            If Not (_specifications Is Nothing) AndAlso _specifications.IsDirty Then Return True
            If Not (_equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
            If Not (_packs Is Nothing) AndAlso _packs.IsDirty Then Return True

            If (Not _factoryGenerations Is Nothing) AndAlso _factoryGenerations.IsDirty Then Return True
            If (Not _assetTypes Is Nothing) AndAlso _assetTypes.IsDirty Then Return True
            If (Not _versions Is Nothing) AndAlso _versions.IsDirty Then Return True

            If (Not _carParts Is Nothing) AndAlso _carParts.IsDirty Then Return True
            If (Not _defaultConfigurations Is Nothing) AndAlso _defaultConfigurations.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGeneration(ByVal model As Model) As ModelGeneration
        If model Is Nothing Then Throw New Exceptions.InvalidModelIdentifier
        If model.Brand.ID.Equals(Guid.Empty) Then Throw New Exceptions.InvalidBrandIdentifier
        If model.ID.Equals(Guid.Empty) Then Throw New Exceptions.InvalidModelIdentifier

        Dim generation As ModelGeneration = New ModelGeneration
        generation._model = model.GetInfo()
        generation.Create()
        generation.CreateSpecifications()
        generation.MarkAsChild()
        Return generation
    End Function
    Private Sub CreateSpecifications()
        _specifications = ModelGenerationSpecifications.GetModelGenerationSpecifications(Me)
    End Sub


    Public Shared Function GetModelGenerationOfPack(ByVal id As Guid) As ModelGeneration
        If id.Equals(Guid.Empty) Then Throw New Exceptions.InvalidModelGenerationIdentifier
        Return GetModelGeneration(DirectCast(ContextCommand.ExecuteScalar(New GetModelGenerationIDForPack(id)), Guid))
    End Function
    Public Shared Function GetModelGenerationOfKatashiki(ByVal katashiki As String) As ModelGeneration
        If katashiki.Equals(String.Empty) Then Throw New Exceptions.InvalidModelGenerationIdentifier
        Return GetModelGeneration(DirectCast(ContextCommand.ExecuteScalar(New GetModelGenerationID(katashiki)), Guid))
    End Function
    Public Shared Function GetModelGeneration(ByVal id As Guid) As ModelGeneration
        If id.Equals(Guid.Empty) Then Throw New Exceptions.InvalidModelGenerationIdentifier
        Dim generation As ModelGeneration = DataPortal.Fetch(Of ModelGeneration)(New Criteria(id))

        'Synchronize base object collections
        If Not generation._bodyTypes Is Nothing Then generation._bodyTypes.Synchronize()
        If Not generation._engines Is Nothing Then generation._engines.Synchronize()
        If Not generation._grades Is Nothing Then generation._grades.Synchronize()
        If Not generation._transmissions Is Nothing Then generation._transmissions.Synchronize()
        If Not generation._wheelDrives Is Nothing Then generation._wheelDrives.Synchronize()
        If Not generation._subModels Is Nothing Then generation._subModels.Synchronize()
        Return generation
    End Function

    Public Shared Sub RevertToClassicMode(ByVal generationID As Guid, ByVal deleteGlobalCars As Boolean)
        If Not Thread.CurrentPrincipal.IsInRole("ISG Administrator") Then Throw New Exceptions.GenerationCanNotBeChangedException("You do not have sufficient rights to perform this operation")

        ContextCommand.Execute(New RevertToClassicModeCommand(generationID, deleteGlobalCars))
    End Sub

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = True
        AllowNew = MyContext.GetContext().IsGlobal
        AllowRemove = Thread.CurrentPrincipal.IsInRole("ISG Administrator")
        AlwaysUpdateSelf = True
        AutoDiscover = True
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _suffixModeAvailable = True
        _mode = LocalizationMode.Suffix
        _masterProjectID = Guid.Empty
        _masterProjectDescription = String.Empty
        _masterSubProjectID = Guid.Empty
        _masterSubProjectDescription = String.Empty
        _calculateOptionRules = True
        _calculateOptionApplicabilities = True
        _bluePrinted = True
        _activeVersion = ModelGenerationCarConfiguratorVersion.Empty()
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _model = ModelInfo.GetModelInfo(dataReader)
            ID = .GetGuid("ID")
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _changeType = ChangeType.GetChangeType(dataReader)
            _status = .GetInt16("STATUSID")
            _suffixModeAvailable = .GetBoolean("SUFFIXMODEAVAILABLE")
            _localConfigurationModeAvailable = .GetBoolean("LOCALCONFIGURATIONMODEAVAILABLE")
            _mode = DirectCast(.GetInt16("LOCALIZATIONMODE"), LocalizationMode)
            _bluePrinted = .GetBoolean("BLUEPRINTED")
            _masterProjectID = .GetGuid("MASTERPROJECTID")
            _masterProjectDescription = .GetString("MASTERPROJECTDESCRIPTION")
            _masterSubProjectID = .GetGuid("MASTERSUBPROJECTID")
            _masterSubProjectDescription = .GetString("MASTERSUBPROJECTDESCRIPTION")
            _calculateOptionRules = .GetBoolean("CALCULATEOPTIONRULES")
            _calculateOptionApplicabilities = .GetBoolean("CALCULATEOPTIONAPPLICABILITIES")
            _isFullyMapped = .GetBoolean("ISFULLYMAPPED")
            _isPPOIntegrationEnabled = .GetBoolean("PPOINTEGRATIONENABLED")
            _isA2AIntegrationEnabled = .GetBoolean("A2AINTEGRATIONENABLED")
            _canSynchronizeVOMPrices = .GetBoolean("CANSYNCHRONIZEVOMPRICES")
            _canSynchronizePPOPrices = .GetBoolean("CANSYNCHRONIZEPPOPRICES")
            _canSynchronizeAccessoryPrices = .GetBoolean("CANSYNCHRONIZEACCESSORYPRICES")
        End With
        _activeVersion = ModelGenerationCarConfiguratorVersion.GetGenerationCarConfiguratorVersion(dataReader)
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        _assets = LinkedAssets.GetLinkedAssets(ID, Model.ID, dataReader)
        If dataReader.NextResult() Then _cars = Cars.GetCars(Me, dataReader)
        If dataReader.NextResult() Then _bodyTypes = ModelGenerationBodyTypes.GetModelGenerationBodyTypes(Me, dataReader)
        If dataReader.NextResult() Then _engines = ModelGenerationEngines.GetModelGenerationEngines(Me, dataReader)
        If dataReader.NextResult() Then _grades = ModelGenerationGrades.GetModelGenerationGrades(Me, dataReader)
        If dataReader.NextResult() Then _gradeClassifications = ModelGenerationGradeClassifications.GetModelGenerationGradeClassifications(Me, dataReader)
        If dataReader.NextResult() Then _transmissions = ModelGenerationTransmissions.GetModelGenerationTransmissions(Me, dataReader)
        If dataReader.NextResult() Then _wheelDrives = ModelGenerationWheelDrives.GetModelGenerationWheelDrives(Me, dataReader)
        If dataReader.NextResult() Then _subModels = ModelGenerationSubModels.GetModelGenerationSubModels(Me, dataReader)
        If dataReader.NextResult() Then 
            'PLACE HOLDER FOR  CLASSIFY GRADES
        End If
        If dataReader.NextResult() Then GetModelGenerationGradeSubModels(dataReader)
    End Sub

    Private Sub GetModelGenerationGradeSubModels(ByVal dataReader As SafeDataReader)
        If Not Grades.Any() Then Exit Sub

        Dim grade As ModelGenerationGrade = Nothing
        For Each grade In Grades
            grade.PrepareSubModelsForEagerLoad()
        Next

        With dataReader
            While .Read()
                Dim gradeId As Guid = .GetGuid("GRADEID")
                If grade Is Nothing OrElse Not grade.ID.Equals(gradeId) Then grade = Grades(gradeId)
                If grade IsNot Nothing Then
                    grade.AddSubModel(dataReader)
                End If
            End While
        End With
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
        command.Parameters.AddWithValue("@MODELID", Model.ID)
        command.Parameters.AddWithValue("@SUFFIXMODEAVAILABLE", SuffixModeAvailable)
        command.Parameters.AddWithValue("@LOCALCONFIGURATIONMODEAVAILABLE", LocalConfigurationModeAvailable)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If IsBaseDirty Then
            command.CommandText = "updateModelGeneration"
            AddCommandFields(command)
        Else
            command.CommandText = "updateModelGenerationTimeStamp"
        End If
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        With command.Parameters
            .AddWithValue("@INTERNALCODE", Code)
            .AddWithValue("@LOCALCODE", LocalCode)
            .AddWithValue("@SHORTNAME", Name)
            .AddWithValue("@CHANGETYPEID", ChangeType.ID)
            .AddWithValue("@STATUSID", _status)
            .AddWithValue("@LOCALIZATIONMODE", _mode)
            .AddWithValue("@PPOINTEGRATIONENABLED", _isPPOIntegrationEnabled)
            .AddWithValue("@A2AINTEGRATIONENABLED", _isA2AIntegrationEnabled)

            If MasterProjectID.Equals(Guid.Empty) OrElse MasterSubProjectID.Equals(Guid.Empty) Then
                .AddWithValue("@MASTERPROJECTID", DBNull.Value)
                .AddWithValue("@MASTERPROJECTDESCRIPTION", DBNull.Value)
                .AddWithValue("@MASTERSUBPROJECTID", DBNull.Value)
                .AddWithValue("@MASTERSUBPROJECTDESCRIPTION", DBNull.Value)
            Else
                .AddWithValue("@MASTERPROJECTID", MasterProjectID)
                .AddWithValue("@MASTERPROJECTDESCRIPTION", MasterProjectDescription)
                .AddWithValue("@MASTERSUBPROJECTID", MasterSubProjectID)
                .AddWithValue("@MASTERSUBPROJECTDESCRIPTION", MasterSubProjectDescription)
            End If

            .AddWithValue("@CALCULATEOPTIONRULES", _calculateOptionRules)
            .AddWithValue("@CALCULATEOPTIONAPPLICABILITIES", _calculateOptionApplicabilities)
            If Not ActiveCarConfiguratorVersion.IsEmpty() Then
                .AddWithValue("@CARCONFIGURATORVERSIONID", ActiveCarConfiguratorVersion.ID)
            Else
                .AddWithValue("@CARCONFIGURATORVERSIONID", DBNull.Value)
            End If

            .AddWithValue("@CANSYNCHRONIZEVOMPRICES", CanSynchronizeVOMPrices)
            .AddWithValue("@CANSYNCHRONIZEPPOPRICES", CanSynchronizePPOPrices)
            .AddWithValue("@CANSYNCHRONIZEACCESSORYPRICES", CanSynchronizeAccessoryPrices)
        End With
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)

        'First delete cars... (otherwise modelgenerationengines and so on.. can not be removed)
        If Not _cars Is Nothing Then _cars.Update(transaction, ChangesToUpdate.Delete)
        If Not _colours Is Nothing Then _colours.Update(transaction)

        If Not _subModels Is Nothing Then _subModels.Update(transaction)
        If Not _bodyTypes Is Nothing Then _bodyTypes.Update(transaction)
        If Not _engines Is Nothing Then _engines.Update(transaction)
        If Not _grades Is Nothing Then _grades.Update(transaction)
        If Not _gradeClassifications Is Nothing Then _gradeClassifications.Update(transaction)
        If Not _transmissions Is Nothing Then _transmissions.Update(transaction)
        If Not _wheelDrives Is Nothing Then _wheelDrives.Update(transaction)


        If Not _assets Is Nothing Then _assets.Update(transaction)

        If Not _specifications Is Nothing Then _specifications.Update(transaction)

        'first create equipment & packs (equipment could have rules towards packs and visa versa)
        If Not _equipment Is Nothing Then _equipment.Update(transaction, ChangesToUpdate.Insert)
        If Not _packs Is Nothing Then _packs.Update(transaction, ChangesToUpdate.Insert)

        'then process the childeren (triggered by  ChangesToUpdate.All)
        If Not _equipment Is Nothing Then _equipment.Update(transaction)
        If Not _packs Is Nothing Then _packs.Update(transaction)



        If Not _factoryGenerations Is Nothing Then _factoryGenerations.Update(transaction)
        If Not _assetTypes Is Nothing Then _assetTypes.Update(transaction)
        If Not _versions Is Nothing Then _versions.Update(transaction)

        If Not _carParts Is Nothing Then _carParts.Update(transaction)


        'Now create & Update cars... (otherwise modelgenerationengines and so on.. do not yet exist)
        If Not _cars Is Nothing Then _cars.Update(transaction)
        If Not _defaultConfigurations Is Nothing Then _defaultConfigurations.Update(transaction)
    End Sub

    Public Overloads Overrides Function Save() As TranslateableBusinessBase
        If Mode = LocalizationMode.Suffix AndAlso Not _cars Is Nothing AndAlso _cars.DeletedCars.Any() Then
            Normalizer.GetNormalizer().Normalize(Me, _cars.DeletedCars)
        End If

        Return MyBase.Save()
    End Function
#End Region

#Region " Commands "
    <Serializable()> Private Class GetModelGenerationID
        Inherits ContextCommand.CommandInfo

        Private ReadOnly _katashiki As String
        Public Sub New(ByVal katashiki As String)
            _katashiki = katashiki
        End Sub
        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "getModelGenerationID"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@KATASHIKI", _katashiki)
        End Sub
    End Class
    <Serializable()> Private Class GetModelGenerationIDForPack
        Inherits ContextCommand.CommandInfo

        Private ReadOnly _packId As Guid
        Public Sub New(ByVal packId As Guid)
            _packId = packId
        End Sub
        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "getModelGenerationIDForPack"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@PACKID", _packId)
        End Sub
    End Class
    <Serializable()> Private Class RevertToClassicModeCommand
        Inherits ContextCommand.CommandInfo

        Private ReadOnly _generationID As Guid
        Private ReadOnly _deleteGlobalCars As Boolean

        Public Sub New(ByVal generationID As Guid, ByVal deleteGlobalCars As Boolean)
            _generationID = generationID
            _deleteGlobalCars = deleteGlobalCars
        End Sub
        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "revertToClassicMode"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
            command.Parameters.AddWithValue("@DELETEGLOBALCARS", _deleteGlobalCars)
        End Sub
    End Class
#End Region

#Region " Base Object Overrides "
    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function

    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.MODELGENERATION
        End Get
    End Property

#End Region


End Class
<Serializable(), XmlInfo("generation")> Public NotInheritable Class ModelGenerationInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _name As String = String.Empty
    Private _model As ModelInfo = ModelInfo.Empty

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
    Public ReadOnly Property Model() As ModelInfo
        Get
            Return _model
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGeneration) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationInfo Then
            Return Equals(DirectCast(obj, ModelGenerationInfo))
        ElseIf TypeOf obj Is ModelGeneration Then
            Return Equals(DirectCast(obj, ModelGeneration))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is ModelGenerationInfo Then
            Return DirectCast(objA, ModelGenerationInfo).Equals(objB)
        ElseIf TypeOf objB Is ModelGenerationInfo Then
            Return DirectCast(objB, ModelGenerationInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "

    Public Shared Function Empty() As ModelGenerationInfo
        Return New ModelGenerationInfo
    End Function

    Friend Shared Function GetModelGenerationInfo(ByVal generation As ModelGeneration) As ModelGenerationInfo
        Dim info As ModelGenerationInfo = New ModelGenerationInfo
        info.Fetch(generation)
        Return info
    End Function
    Friend Shared Function GetModelGenerationInfo(ByVal dataReader As SafeDataReader) As ModelGenerationInfo
        Dim info As ModelGenerationInfo = New ModelGenerationInfo
        info.Fetch(dataReader)
        Return info
    End Function


#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Private Sub Fetch(ByVal generation As ModelGeneration)
        With generation
            _id = .ID
            _name = .Name
            _model = ModelInfo.GetModelInfo(generation)
        End With
    End Sub
    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("GENERATIONID")
            _name = .GetString("GENERATIONNAME")
            _model = ModelInfo.GetModelInfo(dataReader)
        End With
    End Sub

#End Region


End Class