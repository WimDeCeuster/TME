Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Security.ReadOnly
Imports TME.CarConfigurator.Administration.ActiveFilterTool
Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security
    Public Class ReadOnlyPermissionBroker
        Implements IPermissionBroker

        Private Shared ReadOnly Permissions As IPermissions = New Permissions()
        Private Shared ReadOnly GlobalPermissions As IGlobalPermissions = New GlobalPermissions()
        Private Shared ReadOnly CarPermissions As ICarPermissions = New CarPermissions()
        Private Shared ReadOnly ModelGenerationEquipmentPermissions As IModelGenerationEquipmentPermissions = New ModelGenerationEquipmentPermissions()
        Private Shared ReadOnly EquipmentPermissions As IEquipmentPermissions = New EquipmentPermissions()
        Private Shared ReadOnly ModelPermissions As IModelPermissions = New ModelPermissions()
        Private Shared ReadOnly SpecificationPermissions As ISpecificationPermissions = New SpecificationPermissions()

        Public Function GetGlobalPermissions() As IGlobalPermissions Implements IPermissionBroker.GetGlobalPermissions
            Return GlobalPermissions
        End Function

        Public Function GetPermissions(ByVal bodyType As BodyType) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal bodyTypes As BodyTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal bodyTypes As ModelGenerationBodyTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal car As Car) As ICarPermissions Implements IPermissionBroker.GetPermissions
            Return CarPermissions
        End Function

        Public Function GetPermissions(ByVal carEquipment As CarEquipment) As IModelGenerationEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return ModelGenerationEquipmentPermissions
        End Function

        Public Function GetPermissions(ByVal carEquipmentItem As CarEquipmentItem) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal carPacks As CarPacks) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal cars As Cars) As ICarPermissions Implements IPermissionBroker.GetPermissions
            Return CarPermissions
        End Function

        Public Function GetPermissions(ByVal carSpecifications As CarSpecifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal classification As Classification) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal classifications As Classifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal engine As Engine) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal engines As Engines) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal engines As ModelGenerationEngines) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal engineType As EngineType) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal engineTypes As EngineTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal equipment As ModelGenerationEquipment) As IEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return EquipmentPermissions
        End Function

        Public Function GetPermissions(ByVal equipmentCategories As EquipmentCategories) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal equipmentCategory As EquipmentCategory) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal equipmentGroup As EquipmentGroup) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal equipmentItems As EquipmentItems) As IEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return EquipmentPermissions
        End Function

        Public Function GetPermissions(ByVal exteriorColour As ExteriorColour) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal exteriorColours As ExteriorColours) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal exteriorColourTypes As ExteriorColourTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal factoryGenerations As ModelGenerationFactoryGenerations) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal grade As FactoryGrade) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal grade As ModelGenerationGrade) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal grades As FactoryGrades) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal grades As ModelGenerationGrades) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function
        
        Public Function GetPermissions(ByVal gradeClassifications As ModelGenerationGradeClassifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function


        Public Function GetPermissions(ByVal filter As Filter) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal filters As Filters) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal interiorColour As InteriorColour) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal interiorColours As InteriorColours) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal models As Models) As IModelPermissions Implements IPermissionBroker.GetPermissions
            Return ModelPermissions
        End Function

        Public Function GetPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal modelGenerations As ModelGenerations) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal modelSubSets As ModelSubSets) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal pack As ModelGenerationPack) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal packs As ModelGenerationPacks) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal specification As Specification) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal specificationCategories As SpecificationCategories) As ISpecificationPermissions Implements IPermissionBroker.GetPermissions
            Return SpecificationPermissions
        End Function

        Public Function GetPermissions(ByVal specificationCategory As SpecificationCategory) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal subModel As ModelGenerationSubModel) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal subModels As ModelGenerationSubModels) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal transmission As Transmission) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal transmissions As ModelGenerationTransmissions) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal transmissions As Transmissions) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal trim As Trimming) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal trims As Trimmings) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal unit As Unit) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal units As Units) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal upholsteries As Upholsteries) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal upholsteries As IEnumerable(Of ModelGenerationUpholstery)) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal upholsteryTypes As UpholsteryTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal wheelDrive As WheelDrive) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal wheelDrives As ModelGenerationWheelDrives) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetPermissions(ByVal wheelDrives As WheelDrives) As IPermissions Implements IPermissionBroker.GetPermissions
            Return Permissions
        End Function

        Public Function GetAssetPermissions(ByVal engine As Engine) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return Permissions
        End Function

        Public Function GetAssetPermissions(ByVal engineCategory As EngineCategory) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return Permissions
        End Function

        Public Function GetAssetPermissions(ByVal exteriorColour As ExteriorColour) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return Permissions
        End Function

        Public Function GetAssetPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return Permissions
        End Function

        Public Function GetAssetPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return Permissions
        End Function

        Public Function GetAssetPermissions(ByVal packs As ModelGenerationPacks) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return Permissions
        End Function

        Public Function GetAssetPermissions(ByVal transmission As Transmission) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return Permissions
        End Function

        Public Function GetAssetPermissions(ByVal wheelDrive As WheelDrive) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return Permissions
        End Function

        Public Function GetLinkPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetLinkPermissions
            Return Permissions
        End Function

        Public Function GetColourPermissions(ByVal car As Car) As IPermissions Implements IPermissionBroker.GetColourPermissions
            Return Permissions
        End Function

        Public Function GetColourPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetColourPermissions
            Return Permissions
        End Function

        Public Function GetInteriorColourPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetInteriorColourPermissions
            Return Permissions
        End Function

        Public Function GetTrimPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetTrimPermissions
            Return Permissions
        End Function

        Public Function GetAccessoryAssetsPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetAccessoryAssetsPermissions
            Return Permissions
        End Function

        Public Function GetAccessoryPermissions(ByVal equipment As ModelGenerationEquipment) As IPermissions Implements IPermissionBroker.GetAccessoryPermissions
            Return Permissions
        End Function

        Public Function GetAccessoryPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetAccessoryPermissions
            Return Permissions
        End Function

        Public Function GetOptionAssetsPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetOptionAssetsPermissions
            Return Permissions
        End Function

        Public Function GetOptionPermissions(ByVal equipment As ModelGenerationEquipment) As IPermissions Implements IPermissionBroker.GetOptionPermissions
            Return Permissions
        End Function

        Public Function GetOptionPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetOptionPermissions
            Return Permissions
        End Function

        Public Function GetItemPermissions(ByVal specifications As ModelGenerationSpecifications) As IPermissions Implements IPermissionBroker.GetItemPermissions
            Return Permissions
        End Function

        Public Function GetValuePermissions(ByVal specifications As ModelGenerationSpecifications) As IPermissions Implements IPermissionBroker.GetValuePermissions
            Return Permissions
        End Function
    End Class
End Namespace