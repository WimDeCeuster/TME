Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.ActiveFilterTool

Namespace Security.Interface
    Public Interface IPermissionBroker

#Region "Global Permissions"
        Function GetGlobalPermissions() As IGlobalPermissions
#End Region

#Region "Object Permissions"

        Function GetPermissions(ByVal bodyType As BodyType) As IPermissions
        Function GetPermissions(ByVal bodyTypes As BodyTypes) As IPermissions
        Function GetPermissions(ByVal bodyTypes As ModelGenerationBodyTypes) As IPermissions

        Function GetPermissions(ByVal car As Car) As ICarPermissions
        Function GetPermissions(ByVal carEquipment As CarEquipment) As IModelGenerationEquipmentPermissions
        Function GetPermissions(ByVal carEquipmentItem As CarEquipmentItem) As IPermissions
        Function GetPermissions(ByVal carPacks As CarPacks) As IPermissions
        Function GetPermissions(ByVal cars As Cars) As ICarPermissions
        Function GetPermissions(ByVal carSpecifications As CarSpecifications) As IPermissions

        Function GetPermissions(ByVal classification As Classification) As IPermissions
        Function GetPermissions(ByVal classifications As Classifications) As IPermissions

        Function GetPermissions(ByVal engine As Engine) As IPermissions
        Function GetPermissions(ByVal engines As Engines) As IPermissions
        Function GetPermissions(ByVal engines As ModelGenerationEngines) As IPermissions
        Function GetPermissions(ByVal engineType As EngineType) As IPermissions
        Function GetPermissions(ByVal engineTypes As EngineTypes) As IPermissions

        Function GetPermissions(ByVal equipment As ModelGenerationEquipment) As IEquipmentPermissions
        Function GetPermissions(ByVal equipmentCategories As EquipmentCategories) As IPermissions
        Function GetPermissions(ByVal equipmentCategory As EquipmentCategory) As IPermissions
        Function GetPermissions(ByVal equipmentGroup As EquipmentGroup) As IPermissions
        Function GetPermissions(ByVal equipmentItems As EquipmentItems) As IEquipmentPermissions

        Function GetPermissions(ByVal exteriorColour As ExteriorColour) As IPermissions
        Function GetPermissions(ByVal exteriorColours As ExteriorColours) As IPermissions
        Function GetPermissions(ByVal exteriorColourTypes As ExteriorColourTypes) As IPermissions

        Function GetPermissions(ByVal factoryGenerations As ModelGenerationFactoryGenerations) As IPermissions

        Function GetPermissions(ByVal grade As FactoryGrade) As IPermissions
        Function GetPermissions(ByVal grade As ModelGenerationGrade) As IPermissions
        Function GetPermissions(ByVal grades As FactoryGrades) As IPermissions
        Function GetPermissions(ByVal grades As ModelGenerationGrades) As IPermissions
        Function GetPermissions(ByVal gradeClassifications As ModelGenerationGradeClassifications) As IPermissions


        Function GetPermissions(ByVal filter As Filter) As IPermissions
        Function GetPermissions(ByVal filters As Filters) As IPermissions

        Function GetPermissions(ByVal interiorColour As InteriorColour) As IPermissions
        Function GetPermissions(ByVal interiorColours As InteriorColours) As IPermissions

        Function GetPermissions(ByVal models As Models) As IModelPermissions

        Function GetPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions
        Function GetPermissions(ByVal modelGenerations As ModelGenerations) As IPermissions

        Function GetPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions
        Function GetPermissions(ByVal modelSubSets As ModelSubSets) As IPermissions

        Function GetPermissions(ByVal pack As ModelGenerationPack) As IPermissions
        Function GetPermissions(ByVal packs As ModelGenerationPacks) As IPermissions

        Function GetPermissions(ByVal specification As Specification) As IPermissions
        Function GetPermissions(ByVal specificationCategories As SpecificationCategories) As ISpecificationPermissions
        Function GetPermissions(ByVal specificationCategory As SpecificationCategory) As IPermissions

        Function GetPermissions(ByVal subModel As ModelGenerationSubModel) As IPermissions
        Function GetPermissions(ByVal subModels As ModelGenerationSubModels) As IPermissions

        Function GetPermissions(ByVal transmission As Transmission) As IPermissions
        Function GetPermissions(ByVal transmissions As ModelGenerationTransmissions) As IPermissions
        Function GetPermissions(ByVal transmissions As Transmissions) As IPermissions

        Function GetPermissions(ByVal trim As Trimming) As IPermissions
        Function GetPermissions(ByVal trims As Trimmings) As IPermissions

        Function GetPermissions(ByVal unit As Unit) As IPermissions
        Function GetPermissions(ByVal units As Units) As IPermissions

        Function GetPermissions(ByVal upholsteries As Upholsteries) As IPermissions
        Function GetPermissions(ByVal upholsteries As IEnumerable(Of ModelGenerationUpholstery)) As IPermissions
        Function GetPermissions(ByVal upholstery As Upholstery) As IPermissions
        Function GetPermissions(ByVal upholsteryTypes As UpholsteryTypes) As IPermissions

        Function GetPermissions(ByVal wheelDrive As WheelDrive) As IPermissions
        Function GetPermissions(ByVal wheelDrives As ModelGenerationWheelDrives) As IPermissions
        Function GetPermissions(ByVal wheelDrives As WheelDrives) As IPermissions

#End Region

#Region "Assets Permissions"

        Function GetAssetPermissions(ByVal engine As Engine) As IPermissions
        Function GetAssetPermissions(ByVal engineCategory As EngineCategory) As IPermissions

        Function GetAssetPermissions(ByVal exteriorColour As ExteriorColour) As IPermissions

        Function GetAssetPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions

        Function GetAssetPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions

        Function GetAssetPermissions(ByVal packs As ModelGenerationPacks) As IPermissions

        Function GetAssetPermissions(ByVal transmission As Transmission) As IPermissions

        Function GetAssetPermissions(ByVal wheelDrive As WheelDrive) As IPermissions

#End Region

#Region "Link Permissions"

        Function GetLinkPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions

#End Region

#Region "Specialized Permissions"

#Region "Colours"

        Function GetColourPermissions(ByVal car As Car) As IPermissions
        Function GetColourPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions

        Function GetInteriorColourPermissions(ByVal upholstery As Upholstery) As IPermissions
        Function GetTrimPermissions(ByVal upholstery As Upholstery) As IPermissions

#End Region

#Region "Accessories"

        Function GetAccessoryAssetsPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions
        Function GetAccessoryPermissions(ByVal equipment As ModelGenerationEquipment) As IPermissions
        Function GetAccessoryPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions

#End Region

#Region "Options"

        Function GetOptionAssetsPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions
        Function GetOptionPermissions(ByVal equipment As ModelGenerationEquipment) As IPermissions
        Function GetOptionPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions

#End Region

#Region "Specifications"

        Function GetItemPermissions(ByVal specifications As ModelGenerationSpecifications) As IPermissions
        Function GetValuePermissions(ByVal specifications As ModelGenerationSpecifications) As IPermissions

#End Region

#End Region
    End Interface
End Namespace