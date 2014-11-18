Imports System.Runtime.CompilerServices
Imports TME.CarConfigurator.Administration.Enums

Namespace Extensions
    Public Module MasterPackTypeExtensions

        <Extension()> Public Function SourceSystem(ByVal type As MasterPackType) As String
            If type = MasterPackType.MarketingPack Then Return "A2P"
            If type = MasterPackType.PpoPack Then Return "PPO"
            Return String.Empty
        End Function


    End Module
End Namespace