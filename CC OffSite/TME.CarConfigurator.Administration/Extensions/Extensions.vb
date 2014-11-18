Imports System.Runtime.CompilerServices
Imports TME.Common.Extensions
Imports TME.CarConfigurator.Administration.Promotions.Enums
Imports TME.CarConfigurator.Administration.Enums

Namespace Extensions
    Public Module LegacyExtensions
        <Extension()> Public Function GetTitle(ByVal entity As Entity) As String
            Select Case entity
                Case entity.ACCESSORYCATEGORY
                    Return "Accessory category"

                Case entity.CAR
                    Return "Vehicles"

                Case entity.CARPART
                    Return "Parts"

                Case entity.MODELGENERATIONGRADE
                    Return "Grades"
                Case entity.MODELGENERATIONGRADESUBMODEL
                    Return "Grade Submodels"
                Case entity.BODY
                    Return "Body Types"
                Case entity.ENGINECATEGORY
                    Return "Engine Categories"
                Case entity.TRANSMISSIONTYPE
                    Return "Transmission Types"
                Case entity.FUELTYPE
                    Return "Fuel Types"

                Case entity.EXTERIORCOLOUR
                    Return "Exterior Colours"
                Case entity.EXTERIORCOLOURTYPE
                    Return "Exterior Colour Types"
                Case entity.UPHOLSTERY
                    Return "Upholsteries"
                Case entity.UPHOLSTERYTYPE
                    Return "Upholstery Types"
                Case entity.INTERIORCOLOUR
                    Return "Interior Colours"

                Case entity.EQUIPMENTCATEGORY
                    Return "Equipment Categories"
                Case entity.EQUIPMENTGROUP
                    Return "Equipment Group"
                Case entity.ACCESSORY
                    Return "Accessories"
                Case entity.EQUIPMENT
                    Return "Options"

                Case entity.FACTORYGENERATIONOPTIONVALUE
                    Return "Factory Options"

                Case entity.TECHSPEC
                    Return "Specifications"
                Case entity.TECHSPECCATEGORY
                    Return "Specification Categories"
                Case Else
                    Return entity.ToString().Substring(0, 1).ToUpper() & entity.ToString().Substring(1).ToLower() & "s"
            End Select
        End Function
        <Extension()> Public Function GetTitle(ByVal entity As Entity, ByVal multipleTitle As Boolean) As String
            If multipleTitle Then Return entity.GetTitle()

            Select Case entity
                Case entity.CAR
                    Return "Vehicle"

                Case entity.MODELGENERATIONGRADE
                    Return "Grade"
                Case entity.MODELGENERATIONGRADESUBMODEL
                    Return "Grade Submodel"
                Case entity.BODY
                    Return "Body Type"
                Case entity.ENGINECATEGORY
                    Return "Engine Category"

                Case entity.TRANSMISSIONTYPE
                    Return "Transmission Type"
                Case entity.FUELTYPE
                    Return "Fuel Type"

                Case entity.EXTERIORCOLOUR
                    Return "Exterior Colour"
                Case entity.EXTERIORCOLOURTYPE
                    Return "Exterior Colour Type"
                Case entity.UPHOLSTERYTYPE
                    Return "Upholstery Type"

                Case entity.EQUIPMENTCATEGORY
                    Return "Equipment Category"
                Case entity.EQUIPMENT
                    Return "Option"

                Case entity.TECHSPEC
                    Return "Specification"
                Case entity.TECHSPECCATEGORY
                    Return "Specification Category"
                Case Else
                    Return entity.ToString().Substring(0, 1).ToUpper() & entity.ToString().Substring(1).ToLower()
            End Select
        End Function

        <Extension()> Public Function HasDefaultTranslation(ByVal entity As Entity) As Boolean
            Return Translations.LabelDefinitions.GetLabelDefinitions().Any(Function(d) d.Entities.Contains(entity) AndAlso d.Entities(entity).DefaultValue.Length > 0)
        End Function

        <Extension()> Public Function GetEntity(ByVal type As EquipmentType) As Entity
            Select Case type
                Case EquipmentType.Accessory
                    Return Entity.ACCESSORY
                Case EquipmentType.Option
                    Return Entity.EQUIPMENT
                Case EquipmentType.ExteriorColourType
                    Return Entity.EXTERIORCOLOURTYPE
                Case EquipmentType.UpholsteryType
                    Return Entity.UPHOLSTERYTYPE
            End Select

        End Function
        <Extension()> Public Function GetEntity(ByVal title As String) As Entity

            Dim _buffer As String = title.ToUpperInvariant().Trim().Replace(" ", "")
            If _buffer.EndsWith("S") Then _buffer = _buffer.Substring(0, _buffer.Length - 1)

            Select Case _buffer
                Case "VEHICLE"
                    Return Entity.CAR
                Case "GRADE"
                    Return Entity.MODELGENERATIONGRADE
                Case "GRADESUBMODEL"
                    Return Entity.MODELGENERATIONGRADESUBMODEL
                Case "BODYTYPE"
                    Return Entity.BODY
                Case "ENGINECATEGORIE"
                    Return Entity.ENGINECATEGORY
                Case "UPHOLSTERIE"
                    Return Entity.UPHOLSTERY
                Case "EQUIPMENTCATEGORIE"
                    Return Entity.EQUIPMENTCATEGORY
                Case "ACCESSORIE"
                    Return Entity.ACCESSORY
                Case "OPTION"
                    Return Entity.EQUIPMENT
                Case "SPECIFICATION"
                    Return Entity.TECHSPEC
                Case "SPECIFICATIONCATEGORIE"
                    Return Entity.TECHSPECCATEGORY
                Case Else
                    Return DirectCast(Entity.Parse(Entity.NOTHING.GetType(), _buffer), Entity)
            End Select
        End Function


        <Extension()> Public Function GetTitle(ByVal type As EquipmentType) As String
            Select Case type
                Case EquipmentType.Accessory
                    Return "Accessories"
                Case EquipmentType.Option
                    Return "Options"
                Case EquipmentType.ExteriorColourType
                    Return "Paint"
                Case EquipmentType.UpholsteryType
                    Return "Seat Materials"
                Case Else
                    Return type.ToString()
            End Select
        End Function
        <Extension()> Public Function GetTitle(ByVal type As EquipmentType, ByVal multipleTitle As Boolean) As String
            If multipleTitle Then Return type.GetTitle()

            Select Case type
                Case EquipmentType.Accessory
                    Return "Accessory"
                Case EquipmentType.Option
                    Return "Option"
                Case EquipmentType.ExteriorColourType
                    Return "Paint"
                Case EquipmentType.UpholsteryType
                    Return "Seat Material"
                Case Else
                    Return type.ToString()
            End Select

        End Function
        <Extension()> Public Function GetEquipmentType(ByVal value As String) As EquipmentType
            If value.Equals(String.Empty) Then Return EquipmentType.Empty

            Select Case value
                Case Environment.DBAccessoryCode
                    Return EquipmentType.Accessory
                Case "Accessory"
                    Return EquipmentType.Accessory

                Case Environment.DBOptionCode
                    Return EquipmentType.Option
                Case "Option"
                    Return EquipmentType.Option

                Case Environment.DBExteriorColourTypeCode
                    Return EquipmentType.ExteriorColourType
                Case "Seat Material"
                    Return EquipmentType.ExteriorColourType

                Case Environment.DBUpholsteryTypeCode
                    Return EquipmentType.UpholsteryType
                Case "Paint"
                    Return EquipmentType.UpholsteryType

                Case Else
                    Throw New Exceptions.InvalidEquipmentType("""" & value & """ is not a valid equipment type!")
            End Select
        End Function

        <Extension()> Friend Function GetDbValue(ByVal value As Guid) As Object
            If value.Equals(Guid.Empty) Then Return DBNull.Value
            Return value
        End Function

        <Extension()>
        Public Function IsNotACarEntity(ByVal entity As PromotionEntity) As Boolean
            'If entity is one of these, it is not a car entity
            Return entity.In(PromotionEntity.EQUIPMENT, _
                             PromotionEntity.PACK, _
                             PromotionEntity.EXTERIORCOLOUR, _
                             PromotionEntity.EXTERIORCOLOURTYPE, _
                             PromotionEntity.UPHOLSTERY, _
                             PromotionEntity.UPHOLSTERYTYPE, _
                             PromotionEntity.ACCESSORY)
        End Function
    End Module
End NameSpace