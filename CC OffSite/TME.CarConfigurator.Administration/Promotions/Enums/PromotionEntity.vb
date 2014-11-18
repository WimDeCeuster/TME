Imports TME.CarConfigurator.Administration.Enums

Namespace Promotions.Enums

    'NOTE: If a non-car entity is added, also add it to Extensions.vb's IsNotACarEntity() extension method

    Public Enum PromotionEntity
        MODELGENERATION = Entity.MODELGENERATION
        SUBMODEL = Entity.SUBMODEL
        BODY = Entity.BODY
        ENGINE = Entity.ENGINE
        TRANSMISSION = Entity.TRANSMISSION
        WHEELDRIVE = Entity.WHEELDRIVE
        GRADE = Entity.MODELGENERATIONGRADE
        PACK = Entity.PACK
        ACCESSORY = Entity.ACCESSORY
        EQUIPMENT = Entity.EQUIPMENT
        EXTERIORCOLOUR = Entity.EXTERIORCOLOUR
        EXTERIORCOLOURTYPE = Entity.EXTERIORCOLOURTYPE
        UPHOLSTERY = Entity.UPHOLSTERY
        UPHOLSTERYTYPE = Entity.UPHOLSTERYTYPE
    End Enum

End Namespace