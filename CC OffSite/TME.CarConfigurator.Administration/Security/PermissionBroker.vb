Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Security.CrossModel
Imports TME.CarConfigurator.Administration.Security.OldPermissions
Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.ActiveFilterTool
Imports TME.CarConfigurator.Administration.Security.Interface
Imports TME.CarConfigurator.Administration.Extensions.CurrentPrincipalExtensions

Namespace Security
    Public Class PermissionBroker
        Implements IPermissionBroker

#Region "Shared Factory Methods"
        Public Shared Function GetBroker(Optional ByVal canUseLocalConfigurationBroker As Boolean = False) As IPermissionBroker
            Return GetBroker(MyContext.GetContext(), canUseLocalConfigurationBroker)
        End Function

        Public Shared Function GetBroker(ByVal context As MyContext, Optional ByVal canUseLocalConfigurationBroker As Boolean = False) As IPermissionBroker
            Return New PermissionBroker(context, canUseLocalConfigurationBroker)
        End Function

        Public Shared Function GetCrossModelBroker(Optional ByVal context As MyContext = Nothing) As CrossModelPermissionBroker
            Return CrossModelPermissionBroker.GetBroker(context)
        End Function

        Public Shared Function GetOldPermissions() As DefaultPermissions
            Return New DefaultPermissions(MyContext.GetContext())
        End Function
#End Region

#Region "Context"

        Private ReadOnly _context As MyContext

        Private ReadOnly Property Context As MyContext
            Get
                Return _context
            End Get
        End Property

#End Region

#Region "Specialized Brokers"

        Private ReadOnly _canUseLocalConfigurationBroker As Boolean

        Private _defaultBroker As IPermissionBroker
        Private _localConfigurationBroker As IPermissionBroker
        Private _readOnlyBroker As IPermissionBroker

        Private ReadOnly Property DefaultBroker As IPermissionBroker
            Get
                If _defaultBroker Is Nothing Then _defaultBroker = New DefaultPermissionBroker(Context)
                Return _defaultBroker
            End Get
        End Property

        Private ReadOnly Property LocalConfigurationBroker As IPermissionBroker
            Get
                If _localConfigurationBroker Is Nothing Then _localConfigurationBroker = New LocalConfigurationPermissionBroker()
                Return _localConfigurationBroker
            End Get
        End Property

        Private ReadOnly Property ReadOnlyBroker As IPermissionBroker
            Get
                If _readOnlyBroker Is Nothing Then _readOnlyBroker = New ReadOnlyPermissionBroker()
                Return _readOnlyBroker
            End Get
        End Property

        Private Function GetSpecializedBroker() As IPermissionBroker
            If Not Thread.CurrentPrincipal.IsInAnyRole() Then Return ReadOnlyBroker

            If _canUseLocalConfigurationBroker AndAlso ModelGenerationIsInLocalConfigurationMode Then
                Return LocalConfigurationBroker
            End If

            Return DefaultBroker
        End Function

        Private Function ModelGenerationIsInLocalConfigurationMode() As Boolean
            Return Context.ModelGeneration IsNot Nothing AndAlso Context.ModelGeneration.Mode = LocalizationMode.LocalConfiguration
        End Function

#End Region

#Region "Constructors"
        Private Sub New(ByVal context As MyContext, Optional ByVal canUseLocalConfigurationBroker As Boolean = False)
            _context = context
            _canUseLocalConfigurationBroker = canUseLocalConfigurationBroker
        End Sub
#End Region

        Public Function GetGlobalPermissions() As IGlobalPermissions Implements IPermissionBroker.GetGlobalPermissions
            Return GetSpecializedBroker().GetGlobalPermissions()
        End Function

        Public Function GetPermissions(ByVal bodyType As BodyType) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(bodyType)
        End Function

        Public Function GetPermissions(ByVal bodyTypes As BodyTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(bodyTypes)
        End Function

        Public Function GetPermissions(ByVal bodyTypes As ModelGenerationBodyTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(bodyTypes)
        End Function

        Public Function GetPermissions(ByVal car As Car) As ICarPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(car)
        End Function

        Public Function GetPermissions(ByVal cars As Cars) As ICarPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(cars)
        End Function

        Public Function GetPermissions(ByVal carEquipment As CarEquipment) As IModelGenerationEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(carEquipment)
        End Function

        Public Function GetPermissions(ByVal carEquipmentItem As CarEquipmentItem) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(carEquipmentItem)
        End Function

        Public Function GetPermissions(ByVal carPacks As CarPacks) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(carPacks)
        End Function

        Public Function GetPermissions(ByVal carSpecifications As CarSpecifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(carSpecifications)
        End Function

        Public Function GetPermissions(ByVal classification As Classification) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(classification)
        End Function

        Public Function GetPermissions(ByVal classifications As Classifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(classifications)
        End Function

        Public Function GetPermissions(ByVal engine As Engine) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(engine)
        End Function

        Public Function GetPermissions(ByVal engines As Engines) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(engines)
        End Function

        Public Function GetPermissions(ByVal engines As ModelGenerationEngines) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(engines)
        End Function

        Public Function GetPermissions(ByVal engineType As EngineType) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(engineType)
        End Function

        Public Function GetPermissions(ByVal engineTypes As EngineTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(engineTypes)
        End Function

        Public Function GetPermissions(ByVal equipment As ModelGenerationEquipment) As IEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(equipment)
        End Function

        Public Function GetPermissions(ByVal equipmentCategory As EquipmentCategory) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(equipmentCategory)
        End Function

        Public Function GetPermissions(ByVal equipmentCategories As EquipmentCategories) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(equipmentCategories)
        End Function

        Public Function GetPermissions(ByVal equipmentGroup As EquipmentGroup) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(equipmentGroup)
        End Function

        Public Function GetPermissions(ByVal equipmentItems As EquipmentItems) As IEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(equipmentItems)
        End Function

        Public Function GetPermissions(ByVal exteriorColour As ExteriorColour) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(exteriorColour)
        End Function

        Public Function GetPermissions(ByVal exteriorColours As ExteriorColours) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(exteriorColours)
        End Function

        Public Function GetPermissions(ByVal exteriorColourTypes As ExteriorColourTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(exteriorColourTypes)
        End Function

        Public Function GetPermissions(ByVal grade As FactoryGrade) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(grade)
        End Function

        Public Function GetPermissions(ByVal grade As ModelGenerationGrade) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(grade)
        End Function

        Public Function GetPermissions(ByVal grades As FactoryGrades) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(grades)
        End Function

        Public Function GetPermissions(ByVal grades As ModelGenerationGrades) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(grades)
        End Function

        Public Function GetPermissions(ByVal gradeClassifications As ModelGenerationGradeClassifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(gradeClassifications)
        End Function

        Public Function GetPermissions(ByVal filter As Filter) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(filter)
        End Function

        Public Function GetPermissions(ByVal filters As Filters) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(filters)
        End Function

        Public Function GetPermissions(ByVal factoryGenerations As ModelGenerationFactoryGenerations) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(factoryGenerations)
        End Function

        Public Function GetPermissions(ByVal interiorColour As InteriorColour) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(interiorColour)
        End Function

        Public Function GetPermissions(ByVal interiorColours As InteriorColours) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(interiorColours)
        End Function

        Public Function GetPermissions(ByVal models As Models) As IModelPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(models)
        End Function

        Public Function GetPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(modelGeneration)
        End Function

        Public Function GetPermissions(ByVal modelGenerations As ModelGenerations) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(modelGenerations)
        End Function

        Public Function GetPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(modelSubSet)
        End Function

        Public Function GetPermissions(ByVal modelSubSets As ModelSubSets) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(modelSubSets)
        End Function

        Public Function GetPermissions(ByVal pack As ModelGenerationPack) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(pack)
        End Function

        Public Function GetPermissions(ByVal packs As ModelGenerationPacks) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(packs)
        End Function

        Public Function GetPermissions(ByVal specification As Specification) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(specification)
        End Function

        Public Function GetPermissions(ByVal specificationCategories As SpecificationCategories) As ISpecificationPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(specificationCategories)
        End Function

        Public Function GetItemPermissions(ByVal specifications As ModelGenerationSpecifications) As IPermissions Implements IPermissionBroker.GetItemPermissions
            Return GetSpecializedBroker().GetItemPermissions(specifications)
        End Function

        Public Function GetValuePermissions(ByVal specifications As ModelGenerationSpecifications) As IPermissions Implements IPermissionBroker.GetValuePermissions
            Return GetSpecializedBroker().GetValuePermissions(specifications)
        End Function

        Public Function GetPermissions(ByVal specificationCategory As SpecificationCategory) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(specificationCategory)
        End Function

        Public Function GetPermissions(ByVal subModel As ModelGenerationSubModel) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(subModel)
        End Function

        Public Function GetPermissions(ByVal subModels As ModelGenerationSubModels) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(subModels)
        End Function

        Public Function GetPermissions(ByVal transmission As Transmission) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(transmission)
        End Function

        Public Function GetPermissions(ByVal transmissions As ModelGenerationTransmissions) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(transmissions)
        End Function

        Public Function GetPermissions(ByVal transmissions As Transmissions) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(transmissions)
        End Function

        Public Function GetPermissions(ByVal trim As Trimming) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(trim)
        End Function

        Public Function GetPermissions(ByVal trims As Trimmings) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(trims)
        End Function

        Public Function GetPermissions(ByVal unit As Unit) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(unit)
        End Function

        Public Function GetPermissions(ByVal units As Units) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(units)
        End Function

        Public Function GetPermissions(ByVal upholsteries As Upholsteries) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(upholsteries)
        End Function

        Public Function GetPermissions(ByVal upholsteries As IEnumerable(Of ModelGenerationUpholstery)) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(upholsteries)
        End Function

        Public Function GetPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(upholstery)
        End Function

        Public Function GetPermissions(ByVal upholsteryTypes As UpholsteryTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(upholsteryTypes)
        End Function

        Public Function GetPermissions(ByVal wheelDrive As WheelDrive) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(wheelDrive)
        End Function

        Public Function GetPermissions(ByVal wheelDrives As ModelGenerationWheelDrives) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(wheelDrives)
        End Function

        Public Function GetPermissions(ByVal wheelDrives As WheelDrives) As IPermissions Implements IPermissionBroker.GetPermissions
            Return GetSpecializedBroker().GetPermissions(wheelDrives)
        End Function

        Public Function GetAssetPermissions(ByVal engine As Engine) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return GetSpecializedBroker().GetAssetPermissions(engine)
        End Function

        Public Function GetAssetPermissions(ByVal engineCategory As EngineCategory) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return GetSpecializedBroker().GetAssetPermissions(engineCategory)
        End Function

        Public Function GetAssetPermissions(ByVal exteriorColour As ExteriorColour) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return GetSpecializedBroker().GetAssetPermissions(exteriorColour)
        End Function

        Public Function GetAssetPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return GetSpecializedBroker().GetAssetPermissions(modelGeneration)
        End Function

        Public Function GetAssetPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return GetSpecializedBroker().GetAssetPermissions(modelSubSet)
        End Function

        Public Function GetAssetPermissions(ByVal packs As ModelGenerationPacks) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return GetSpecializedBroker().GetAssetPermissions(packs)
        End Function

        Public Function GetAssetPermissions(ByVal transmission As Transmission) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return GetSpecializedBroker().GetAssetPermissions(transmission)
        End Function

        Public Function GetAssetPermissions(ByVal wheelDrive As WheelDrive) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return GetSpecializedBroker().GetAssetPermissions(wheelDrive)
        End Function

        Public Function GetLinkPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetLinkPermissions
            Return GetSpecializedBroker().GetLinkPermissions(modelSubSet)
        End Function

        Public Function GetColourPermissions(ByVal car As Car) As IPermissions Implements IPermissionBroker.GetColourPermissions
            Return GetSpecializedBroker().GetColourPermissions(car)
        End Function

        Public Function GetColourPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetColourPermissions
            Return GetSpecializedBroker().GetColourPermissions(modelGeneration)
        End Function

        Public Function GetInteriorColourPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetInteriorColourPermissions
            Return GetSpecializedBroker().GetInteriorColourPermissions(upholstery)
        End Function

        Public Function GetTrimPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetTrimPermissions
            Return GetSpecializedBroker().GetTrimPermissions(upholstery)
        End Function

        Public Function GetAccessoryAssetsPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetAccessoryAssetsPermissions
            Return GetSpecializedBroker().GetAccessoryAssetsPermissions(equipmentItems)
        End Function

        Public Function GetAccessoryPermissions(ByVal equipment As ModelGenerationEquipment) As IPermissions Implements IPermissionBroker.GetAccessoryPermissions
            Return GetSpecializedBroker().GetAccessoryPermissions(equipment)
        End Function

        Public Function GetAccessoryPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetAccessoryPermissions
            Return GetSpecializedBroker().GetAccessoryPermissions(equipmentItems)
        End Function

        Public Function GetOptionAssetsPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetOptionAssetsPermissions
            Return GetSpecializedBroker().GetOptionAssetsPermissions(equipmentItems)
        End Function

        Public Function GetOptionPermissions(ByVal equipment As ModelGenerationEquipment) As IPermissions Implements IPermissionBroker.GetOptionPermissions
            Return GetSpecializedBroker().GetOptionPermissions(equipment)
        End Function

        Public Function GetOptionPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetOptionPermissions
            Return GetSpecializedBroker().GetOptionPermissions(equipmentItems)
        End Function
    End Class
End Namespace