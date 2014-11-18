Imports TME.CarConfigurator.Administration.Enums

Namespace Normalizers.Accessories
    Public Class AvailabilityNormalizer
        Implements IGradeNormalizer

        Public Sub Normalize(ByVal grade As ModelGenerationGrade, ByVal cars As System.Collections.Generic.IEnumerable(Of Car)) Implements IGradeNormalizer.Normalize
            Call New Equipment.AvailabilityNormalizer(EquipmentType.Accessory).Normalize(grade, cars)
        End Sub
    End Class
End Namespace