Imports System.Runtime.CompilerServices
Imports TME.CarConfigurator.Administration.Enums

Namespace Extensions

    Public Module EntityExtensions
        <Extension()> Public Function SupportsDeviations(ByVal entity As Entity) As Boolean

            If entity = Entity.MODELGENERATIONBODY Then Return True
            If entity = Entity.MODELGENERATIONENGINE Then Return True
            If entity = Entity.MODELGENERATIONTRANSMISSION Then Return True
            If entity = Entity.MODELGENERATIONWHEELDRIVE Then Return True
            If entity = Entity.MODELGENERATIONGRADE Then Return True
            If entity = Entity.MODELGENERATIONGRADESUBMODEL Then Return True
            If entity = Entity.SUBMODEL Then Return True
            If entity = Entity.CARPART Then Return True

            If entity = entity.FACTORYGENERATIONOPTIONVALUE Then Return True
            If entity = entity.CAREQUIPMENT Then Return True
            If entity = entity.CARACCESSORY Then Return True
            If entity = Entity.CAREXTERIORCOLOUR Then Return True
            If entity = Entity.CARUPHOLSTERY Then Return True
            Return False
        End Function


    End Module
End NameSpace