Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Normalizers.Options
    Public Class ApplicabilitiesNormalizer
        Implements IGenerationNormalizer
        
        Public Sub Normalize(ByVal generation As ModelGeneration, ByVal allCars As Boolean) Implements IGenerationNormalizer.Normalize
            Dim cars = (From car In generation.Cars Where (allCars OrElse car.Approved OrElse car.Preview)).ToList()
            Dim options = (From x In generation.Equipment Where x.Type = EquipmentType.Option Select DirectCast(x, ModelGenerationOption))
            For Each generationOption As ModelGenerationOption In options
                Dim carOptions = (From car In cars
                        Where car.Equipment.Contains(generationOption.ID) AndAlso
                              Not car.Equipment(generationOption.ID).Availability = Availability.NotAvailable
                        Select DirectCast(car.Equipment(generationOption.ID), CarOption)).ToList()

                NormalizeExteriorColourApplicabilities(generationOption, carOptions)
                NormalizeUpholsteryApplicabilities(generationOption, carOptions)
            Next
        End Sub

        Private Shared Sub NormalizeExteriorColourApplicabilities(ByVal generationOption As ModelGenerationOption, ByVal carOptions As List(Of CarOption))
            Dim generationApplicabilities = (From applicability In generationOption.ExteriorColourApplicabilities Select applicability.ID)
            Dim carApplicabilities = (From carOption In carOptions
                    From applicability In carOption.ExteriorColourApplicabilities
                    Select applicability.ID)

            Dim allApplicabilities = generationApplicabilities.Union(carApplicabilities).ToList()
            For Each applicability In allApplicabilities

                Dim howManyCarsHaveTheApplicability = (From opt In carOptions Where opt.ExteriorColourApplicabilities.Contains(applicability) AndAlso Not opt.ExteriorColourApplicabilities(applicability).Cleared).Count()
                Dim howManyCarsDoNotHaveTheApplicability = (From opt In carOptions Where Not opt.ExteriorColourApplicabilities.Contains(applicability) OrElse opt.ExteriorColourApplicabilities(applicability).Cleared).Count()

                'In most of the cars have the applicability, then add it to the generation level (ifneed)
                'Otherwise delete it
                If howManyCarsHaveTheApplicability >= howManyCarsDoNotHaveTheApplicability Then
                    If Not generationOption.ExteriorColourApplicabilities.Contains(applicability) Then
                        Dim exteriorColourinfo = generationOption.Generation.ColourCombinations.ExteriorColours.First(Function(x) x.ID.Equals(applicability)).GetInfo()
                        generationOption.ExteriorColourApplicabilities.Add(exteriorColourinfo)
                    End If
                ElseIf generationOption.ExteriorColourApplicabilities.Contains(applicability) Then
                    generationOption.ExteriorColourApplicabilities.Remove(applicability)
                End If

            Next
        End Sub
        Private Shared Sub NormalizeUpholsteryApplicabilities(ByVal generationOption As ModelGenerationOption, ByVal carOptions As List(Of CarOption))
            Dim generationApplicabilities = (From applicability In generationOption.UpholsteryApplicabilities Select applicability.ID)
            Dim carApplicabilities = (From carOption In carOptions
                    From applicability In carOption.UpholsteryApplicabilities
                    Select applicability.ID)

            Dim allApplicabilities = generationApplicabilities.Union(carApplicabilities).ToList()
            For Each applicability In allApplicabilities

                Dim howManyCarsHaveTheApplicability = (From opt In carOptions Where opt.UpholsteryApplicabilities.Contains(applicability) AndAlso Not opt.UpholsteryApplicabilities(applicability).Cleared).Count()
                Dim howManyCarsDoNotHaveTheApplicability = (From opt In carOptions Where Not opt.UpholsteryApplicabilities.Contains(applicability) OrElse opt.UpholsteryApplicabilities(applicability).Cleared).Count()

                'If most of the cars have the applicability, then add it to the generation level (if needed)
                'Otherwise delete it
                If howManyCarsHaveTheApplicability >= howManyCarsDoNotHaveTheApplicability Then
                    If Not generationOption.UpholsteryApplicabilities.Contains(applicability) Then
                        Dim upholsteryInfo = generationOption.Generation.ColourCombinations.Upholsteries().First(Function(x) x.ID.Equals(applicability)).GetInfo()
                        generationOption.UpholsteryApplicabilities.Add(upholsteryInfo)
                    End If
                ElseIf generationOption.UpholsteryApplicabilities.Contains(applicability) Then
                    generationOption.UpholsteryApplicabilities.Remove(applicability)
                End If
            Next
        End Sub
        
    End Class
End Namespace