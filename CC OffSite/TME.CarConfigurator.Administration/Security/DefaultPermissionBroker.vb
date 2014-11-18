Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Security.OldPermissions
Imports TME.CarConfigurator.Administration.ActiveFilterTool
Imports TME.CarConfigurator.Administration.Security.Interface

Namespace Security
    Friend Class DefaultPermissionBroker
        Implements IPermissionBroker

#Region "Permissions"
        Private ReadOnly _permissions As DefaultPermissions
#End Region

#Region "Constructors"
        Public Sub New(ByVal context As MyContext)
            _permissions = New DefaultPermissions(context)
        End Sub
#End Region

        Public Function GetGlobalPermissions() As IGlobalPermissions Implements IPermissionBroker.GetGlobalPermissions
            Return _permissions
        End Function

        Public Function GetPermissions(ByVal bodyType As BodyType) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.BodyTypes
        End Function

        Public Function GetPermissions(ByVal bodyTypes As BodyTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.BodyTypes
        End Function

        Public Function GetPermissions(ByVal bodyTypes As ModelGenerationBodyTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.BodyTypes
        End Function

        Public Function GetPermissions(ByVal car As Car) As ICarPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Car
        End Function

        Public Function GetPermissions(ByVal cars As Cars) As ICarPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Car
        End Function

        Public Function GetPermissions(ByVal carEquipment As CarEquipment) As IModelGenerationEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Car.Equipment
        End Function

        Public Function GetPermissions(ByVal carEquipmentItem As CarEquipmentItem) As IPermissions Implements IPermissionBroker.GetPermissions
            Return DirectCast(_permissions.Car.Equipment, ModelGenerationEquipmentPermissions).Items
        End Function

        Public Function GetPermissions(ByVal carPacks As CarPacks) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Car.Packs
        End Function

        Public Function GetPermissions(ByVal carSpecifications As CarSpecifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Specifications.Values
        End Function

        Public Function GetPermissions(ByVal classification As Classification) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ActiveFilterTool
        End Function

        Public Function GetPermissions(ByVal classifications As Classifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ActiveFilterTool
        End Function

        Public Function GetPermissions(ByVal engine As Engine) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.Engines
        End Function

        Public Function GetPermissions(ByVal engines As Engines) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.Engines
        End Function

        Public Function GetPermissions(ByVal engines As ModelGenerationEngines) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.Engines
        End Function

        Public Function GetPermissions(ByVal engineType As EngineType) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.EngineTypes
        End Function

        Public Function GetPermissions(ByVal engineTypes As EngineTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.EngineTypes
        End Function

        Public Function GetPermissions(ByVal equipment As ModelGenerationEquipment) As IEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Equipment
        End Function

        Public Function GetPermissions(ByVal equipmentCategory As EquipmentCategory) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Equipment.Categories
        End Function

        Public Function GetPermissions(ByVal equipmentCategories As EquipmentCategories) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Equipment.Categories
        End Function

        Public Function GetPermissions(ByVal equipmentGroup As EquipmentGroup) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Equipment.Groups
        End Function

        Public Function GetPermissions(ByVal equipmentItems As EquipmentItems) As IEquipmentPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Equipment
        End Function

        Public Function GetPermissions(ByVal exteriorColour As ExteriorColour) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ExteriorColour
        End Function

        Public Function GetPermissions(ByVal exteriorColours As ExteriorColours) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ExteriorColour
        End Function

        Public Function GetPermissions(ByVal exteriorColourTypes As ExteriorColourTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ExteriorColour
        End Function

        Public Function GetPermissions(ByVal grade As FactoryGrade) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.FactoryGrades
        End Function

        Public Function GetPermissions(ByVal grade As ModelGenerationGrade) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.Grades
        End Function

        Public Function GetPermissions(ByVal grades As FactoryGrades) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.FactoryGrades
        End Function

        Public Function GetPermissions(ByVal grades As ModelGenerationGrades) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.Grades
        End Function
        Public Function GetPermissions(ByVal gradeClassifications As ModelGenerationGradeClassifications) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.GradeClassifications
        End Function

        Public Function GetPermissions(ByVal filter As Filter) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ActiveFilterTool
        End Function

        Public Function GetPermissions(ByVal filters As Filters) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ActiveFilterTool
        End Function

        Public Function GetPermissions(ByVal factoryGenerations As ModelGenerationFactoryGenerations) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ModelGeneration
        End Function

        Public Function GetPermissions(ByVal interiorColour As InteriorColour) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.InteriorColour
        End Function

        Public Function GetPermissions(ByVal interiorColours As InteriorColours) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.InteriorColour
        End Function

        Public Function GetPermissions(ByVal models As Models) As IModelPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Model
        End Function

        Public Function GetPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ModelGeneration
        End Function

        Public Function GetPermissions(ByVal modelGenerations As ModelGenerations) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ModelGeneration
        End Function

        Public Function GetPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ActiveFilterTool
        End Function

        Public Function GetPermissions(ByVal modelSubSets As ModelSubSets) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ActiveFilterTool
        End Function

        Public Function GetPermissions(ByVal pack As ModelGenerationPack) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ModelGeneration.Packs
        End Function

        Public Function GetPermissions(ByVal packs As ModelGenerationPacks) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ModelGeneration.Packs
        End Function

        Public Function GetPermissions(ByVal specification As Specification) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Specifications.Items
        End Function

        Public Function GetPermissions(ByVal specificationCategories As SpecificationCategories) As ISpecificationPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Specifications.Items
        End Function

        Public Function GetItemPermissions(ByVal specifications As ModelGenerationSpecifications) As IPermissions Implements IPermissionBroker.GetItemPermissions
            Return _permissions.ModelGeneration.Specifications.Items
        End Function

        Public Function GetValuePermissions(ByVal specifications As ModelGenerationSpecifications) As IPermissions Implements IPermissionBroker.GetValuePermissions
            Return _permissions.ModelGeneration.Specifications.Values
        End Function

        Public Function GetPermissions(ByVal specificationCategory As SpecificationCategory) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Specifications.Categories
        End Function

        Public Function GetPermissions(ByVal subModel As ModelGenerationSubModel) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.SubModels
        End Function

        Public Function GetPermissions(ByVal subModels As ModelGenerationSubModels) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.SubModels
        End Function

        Public Function GetPermissions(ByVal transmission As Transmission) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.Transmissions
        End Function

        Public Function GetPermissions(ByVal transmissions As ModelGenerationTransmissions) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.Transmissions
        End Function

        Public Function GetPermissions(ByVal transmissions As Transmissions) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.Transmissions
        End Function

        Public Function GetPermissions(ByVal trim As Trimming) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Trimming
        End Function

        Public Function GetPermissions(ByVal trims As Trimmings) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Trimming
        End Function

        Public Function GetPermissions(ByVal unit As Unit) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Specifications.Items
        End Function

        Public Function GetPermissions(ByVal units As Units) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Specifications.Items
        End Function

        Public Function GetPermissions(ByVal upholsteries As Upholsteries) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Upholstery
        End Function

        Public Function GetPermissions(ByVal upholsteries As IEnumerable(Of ModelGenerationUpholstery)) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.ModelGeneration.Colours
        End Function

        Public Function GetPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Upholstery
        End Function

        Public Function GetPermissions(ByVal upholsteryTypes As UpholsteryTypes) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Upholstery
        End Function

        Public Function GetPermissions(ByVal wheelDrive As WheelDrive) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.WheelDrives
        End Function

        Public Function GetPermissions(ByVal wheelDrives As ModelGenerationWheelDrives) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.WheelDrives
        End Function

        Public Function GetPermissions(ByVal wheelDrives As WheelDrives) As IPermissions Implements IPermissionBroker.GetPermissions
            Return _permissions.Core.WheelDrives
        End Function

        Public Function GetAssetPermissions(ByVal engine As Engine) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return _permissions.Core.Engines.Assets
        End Function

        Public Function GetAssetPermissions(ByVal engineCategory As EngineCategory) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return _permissions.Core.Engines.Assets
        End Function

        Public Function GetAssetPermissions(ByVal exteriorColour As ExteriorColour) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return _permissions.ExteriorColour.Assets
        End Function

        Public Function GetAssetPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return _permissions.ModelGeneration.Assets
        End Function

        Public Function GetAssetPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return _permissions.ActiveFilterTool.Assets
        End Function

        Public Function GetAssetPermissions(ByVal packs As ModelGenerationPacks) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return _permissions.ModelGeneration.Packs.Assets
        End Function

        Public Function GetAssetPermissions(ByVal transmission As Transmission) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return _permissions.Core.Transmissions.Assets
        End Function

        Public Function GetAssetPermissions(ByVal wheelDrive As WheelDrive) As IPermissions Implements IPermissionBroker.GetAssetPermissions
            Return _permissions.Core.WheelDrives.Assets
        End Function

        Public Function GetLinkPermissions(ByVal modelSubSet As ModelSubSet) As IPermissions Implements IPermissionBroker.GetLinkPermissions
            Return _permissions.Links
        End Function

        Public Function GetColourPermissions(ByVal car As Car) As IPermissions Implements IPermissionBroker.GetColourPermissions
            Return _permissions.Car.Colours
        End Function

        Public Function GetColourPermissions(ByVal modelGeneration As ModelGeneration) As IPermissions Implements IPermissionBroker.GetColourPermissions
            Return _permissions.ModelGeneration.Colours
        End Function

        Public Function GetInteriorColourPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetInteriorColourPermissions
            Return _permissions.InteriorColour
        End Function

        Public Function GetTrimPermissions(ByVal upholstery As Upholstery) As IPermissions Implements IPermissionBroker.GetTrimPermissions
            Return _permissions.Trimming
        End Function

        Public Function GetAccessoryAssetsPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetAccessoryAssetsPermissions
            Return _permissions.Equipment.Accessories.Assets
        End Function

        Public Function GetAccessoryPermissions(ByVal equipment As ModelGenerationEquipment) As IPermissions Implements IPermissionBroker.GetAccessoryPermissions
            Return _permissions.Equipment.Accessories
        End Function

        Public Function GetAccessoryPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetAccessoryPermissions
            Return _permissions.Equipment.Accessories
        End Function

        Public Function GetOptionAssetsPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetOptionAssetsPermissions
            Return _permissions.Equipment.Options.Assets
        End Function

        Public Function GetOptionPermissions(ByVal equipment As ModelGenerationEquipment) As IPermissions Implements IPermissionBroker.GetOptionPermissions
            Return _permissions.Equipment.Options
        End Function

        Public Function GetOptionPermissions(ByVal equipmentItems As EquipmentItems) As IPermissions Implements IPermissionBroker.GetOptionPermissions
            Return _permissions.Equipment.Options
        End Function
    End Class
End Namespace