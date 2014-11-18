Imports System.Runtime.CompilerServices
Imports TME.CarConfigurator.Administration.Enums

Namespace Extensions
    Public Module MasterEquipmentTypeExtensions

        <Extension()> Public Function SourceSystem(ByVal type As MasterEquipmentType) As String
            If type = MasterEquipmentType.Accessory Then Return "A2A"
            If type = MasterEquipmentType.PostProductionOption Then Return "PPO"
            Return "A2P"
        End Function


    End Module
End Namespace