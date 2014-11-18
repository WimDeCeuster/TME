Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Normalizers

    Public Class Normalizer
        Public Sub Normalize(ByVal generation As ModelGeneration, ByVal cars As IList(Of Car), Optional ByVal allCars As Boolean = True)
            NormalizeAccessories(generation, cars, allCars)
            NormalizeOptions(generation, cars, allCars)
            NormalizePacks(generation, cars, allCars)
            NormalizePackContent(generation)
            NormalizeColours(generation, cars, allCars)
            NormalizeSpecifications(generation, cars, allCars)
        End Sub

#Region " Accessories "

#Region " Dependencies  "
        Private _accessoriesAvailabilityNormalizer As IGradeNormalizer

        Property AccessoriesAvailabilityNormalizer() As IGradeNormalizer
            Get
                If _accessoriesAvailabilityNormalizer Is Nothing Then _accessoriesAvailabilityNormalizer = New Accessories.AvailabilityNormalizer()
                Return _accessoriesAvailabilityNormalizer
            End Get
            Set(ByVal value As IGradeNormalizer)
                _accessoriesAvailabilityNormalizer = value
            End Set
        End Property
#End Region

        Public Sub NormalizeAccessories(ByVal generation As ModelGeneration, ByVal cars As IEnumerable(Of Car), Optional ByVal allCars As Boolean = True)
            Dim approvedGrades As IEnumerable(Of ModelGenerationGrade) = From x In generation.Grades Where cars.Any(Function(y) y.Grade.Equals(x)) AndAlso (allCars OrElse x.Preview OrElse x.Approved)
            For Each grade As ModelGenerationGrade In approvedGrades
                NormalizeAccessories(generation, grade, allCars)
            Next
        End Sub
        Public Sub NormalizeAccessories(ByVal generation As ModelGeneration, Optional ByVal allCars As Boolean = True)
            Dim approvedGrades As IEnumerable(Of ModelGenerationGrade) = From x In generation.Grades Where allCars OrElse x.Preview OrElse x.Approved
            For Each grade As ModelGenerationGrade In approvedGrades
                NormalizeAccessories(generation, grade, allCars)
            Next
        End Sub
        Public Sub NormalizeAccessories(ByVal generation As ModelGeneration, ByVal car As Car, Optional ByVal allCars As Boolean = True)
            NormalizeAccessories(generation, generation.Grades(car.Grade.ID), allCars)
        End Sub

        Private Sub NormalizeAccessories(ByVal generation As ModelGeneration, ByVal grade As ModelGenerationGrade, Optional ByVal allCars As Boolean = True)
            Dim cars = (From car In generation.Cars Where car.Grade.Equals(grade.ID) AndAlso (allCars OrElse car.Approved OrElse car.Preview)).ToList()
            AccessoriesAvailabilityNormalizer.Normalize(grade, cars)
        End Sub
#End Region

#Region " Options "

#Region " Dependencies  "
        Private _optionAvailabilityNormalizer As IGradeNormalizer
        Private _optionApplicabiltiesNormalizer As IGenerationNormalizer
        Private _optionRulesNormalizer As IGenerationNormalizer


        Property OptionAvailabilityNormalizer() As IGradeNormalizer
            Get
                If _optionAvailabilityNormalizer Is Nothing Then _optionAvailabilityNormalizer = New Options.AvailabilityNormalizer()
                Return _optionAvailabilityNormalizer
            End Get
            Set(ByVal value As IGradeNormalizer)
                _optionAvailabilityNormalizer = value
            End Set
        End Property
        Property OptionApplicabilitiesNormalizer() As IGenerationNormalizer
            Get
                If _optionApplicabiltiesNormalizer Is Nothing Then _optionApplicabiltiesNormalizer = New Options.ApplicabilitiesNormalizer()
                Return _optionApplicabiltiesNormalizer
            End Get
            Set(ByVal value As IGenerationNormalizer)
                _optionApplicabiltiesNormalizer = value
            End Set
        End Property
        Property OptionRulesNormalizer() As IGenerationNormalizer
            Get
                If _optionRulesNormalizer Is Nothing Then _optionRulesNormalizer = New Options.RuleNormalizer()
                Return _optionRulesNormalizer
            End Get
            Set(ByVal value As IGenerationNormalizer)
                _optionRulesNormalizer = value
            End Set
        End Property

#End Region

        Public Sub NormalizeOptions(ByVal generation As ModelGeneration, ByVal cars As IEnumerable(Of Car), Optional ByVal allCars As Boolean = True)
            Dim approvedGrades As IEnumerable(Of ModelGenerationGrade) = From x In generation.Grades Where cars.Any(Function(y) y.Grade.Equals(x)) AndAlso (allCars OrElse x.Preview OrElse x.Approved)
            For Each grade As ModelGenerationGrade In approvedGrades
                NormalizeOptions(generation, grade, allCars)
            Next
            OptionApplicabilitiesNormalizer.Normalize(generation, allCars)
            OptionRulesNormalizer.Normalize(generation, allCars)
        End Sub
        Public Sub NormalizeOptions(ByVal generation As ModelGeneration, Optional ByVal allCars As Boolean = True)
            Dim approvedGrades As IEnumerable(Of ModelGenerationGrade) = From x In generation.Grades Where allCars OrElse x.Preview OrElse x.Approved
            For Each grade As ModelGenerationGrade In approvedGrades
                NormalizeOptions(generation, grade, allCars)
            Next

            OptionApplicabilitiesNormalizer.Normalize(generation, allCars)
            OptionRulesNormalizer.Normalize(generation, allCars)
        End Sub
        Public Sub NormalizeOptions(ByVal generation As ModelGeneration, ByVal car As Car, Optional ByVal allCars As Boolean = True)

            NormalizeOptions(generation, generation.Grades(car.Grade.ID), allCars)

            OptionApplicabilitiesNormalizer.Normalize(generation, allCars)
            OptionRulesNormalizer.Normalize(generation, allCars)
        End Sub

        Private Sub NormalizeOptions(ByVal generation As ModelGeneration, ByVal grade As ModelGenerationGrade, ByVal allCars As Boolean)
            Dim cars = (From car In generation.Cars Where car.Grade.Equals(grade.ID) AndAlso (allCars OrElse car.Approved OrElse car.Preview)).ToList()

            OptionAvailabilityNormalizer.Normalize(grade, cars)
        End Sub


#End Region

#Region " Normalize Packs "
        Public Shared Sub NormalizePacks(ByVal generation As ModelGeneration, ByVal cars As IEnumerable(Of Car), Optional ByVal allCars As Boolean = True)
            Dim approvedGrades As IEnumerable(Of ModelGenerationGrade) = From x In generation.Grades Where cars.Any(Function(y) y.Grade.Equals(x)) AndAlso (allCars OrElse x.Preview OrElse x.Approved)
            For Each grade As ModelGenerationGrade In approvedGrades
                NormalizePacks(generation, grade, allCars)
            Next
        End Sub
        Public Shared Sub NormalizePacks(ByVal generation As ModelGeneration, Optional ByVal allCars As Boolean = True)
            For Each grade As ModelGenerationGrade In generation.Grades
                NormalizePacks(generation, grade, allCars)
            Next
        End Sub
        Public Shared Sub NormalizePacks(ByVal generation As ModelGeneration, ByVal car As Car, Optional ByVal allCars As Boolean = True)
            NormalizePacks(generation, generation.Grades(car.Grade.ID))
        End Sub
        Public Shared Sub NormalizePacks(ByVal generation As ModelGeneration, ByVal grade As ModelGenerationGrade, Optional ByVal allCars As Boolean = True)
            NormalizeGradePack(generation, grade, allCars)
        End Sub

        Private Shared Sub NormalizeGradePack(ByVal generation As ModelGeneration, ByVal grade As ModelGenerationGrade, ByVal allCars As Boolean)
            Dim gradePacks As Dictionary(Of Guid, GradePackAvailability) = grade.Packs.ToDictionary(Function(x) x.ID, Function(x) New GradePackAvailability(x))
            Dim cars As IEnumerable(Of Car) = From car In generation.Cars Where car.Grade.Equals(grade.ID) AndAlso (allCars OrElse car.Approved OrElse car.Preview)
            For Each car As Car In cars
                For Each carPack As CarPack In car.Packs
                    Select Case carPack.Availability
                        Case Availability.Standard
                            gradePacks.Item(carPack.ID).StandardOn.Add(carPack)
                        Case Availability.Optional
                            gradePacks.Item(carPack.ID).OptionalOn.Add(carPack)
                        Case Availability.NotAvailable
                            gradePacks.Item(carPack.ID).NotAvailableOn.Add(carPack)
                    End Select
                Next
            Next

            'Then delegate Normalize call to each equipment item
            For Each gradePack As GradePackAvailability In gradePacks.Values
                gradePack.Normalize()
            Next

        End Sub

#Region " Helper Classes "

        Private Class GradePackAvailability

#Region " Business Propererties & Methodes "
            Private ReadOnly _pack As ModelGenerationGradePack

            Private ReadOnly _standardOn As List(Of CarPack) = New List(Of CarPack)
            Private ReadOnly _optionalOn As List(Of CarPack) = New List(Of CarPack)
            Private ReadOnly _notAvailableOn As List(Of CarPack) = New List(Of CarPack)

            Private ReadOnly Property Pack() As ModelGenerationGradePack
                Get
                    Return _pack
                End Get
            End Property

            Private ReadOnly Property MostlyStandard() As Boolean
                Get
                    If _standardOn.Count = 0 AndAlso _optionalOn.Count = 0 AndAlso _notAvailableOn.Count = 0 Then Return False
                    Return (_standardOn.Count >= _optionalOn.Count AndAlso _standardOn.Count >= _notAvailableOn.Count)
                End Get
            End Property

            Private ReadOnly Property MostlyOptional() As Boolean
                Get
                    Return (_optionalOn.Count > _standardOn.Count AndAlso _optionalOn.Count >= _notAvailableOn.Count)
                End Get
            End Property

            Private ReadOnly Property MostlyNotAvailable() As Boolean
                Get
                    If _standardOn.Count = 0 AndAlso _optionalOn.Count = 0 AndAlso _notAvailableOn.Count = 0 Then Return True
                    Return (_notAvailableOn.Count > _standardOn.Count AndAlso _notAvailableOn.Count > _optionalOn.Count)
                End Get
            End Property

            Public ReadOnly Property StandardOn() As List(Of CarPack)
                Get
                    Return _standardOn
                End Get
            End Property
            Public ReadOnly Property OptionalOn() As List(Of CarPack)
                Get
                    Return _optionalOn
                End Get
            End Property
            Public ReadOnly Property NotAvailableOn() As List(Of CarPack)
                Get
                    Return _notAvailableOn
                End Get
            End Property

            Public Sub Normalize()
                Select Case True
                    Case MostlyStandard
                        MakeStandard()
                    Case MostlyOptional
                        MakeOptional()
                    Case MostlyNotAvailable
                        If StandardOn.Count = 0 AndAlso OptionalOn.Count = 0 Then
                            MakeNotAvailable()
                        Else
                            If StandardOn.Count > OptionalOn.Count Then
                                MakeStandard()
                            Else
                                MakeOptional()
                            End If
                        End If
                    Case Else
                        Throw New Exceptions.DevelopperException("Normalize Packs;Collection Status Retrival;undefined")
                End Select

            End Sub

            Private Shared Sub TakeOwnership(ByVal list As IEnumerable(Of CarPack))
                For Each pack As CarPack In (From x In list Where Not x.Overwritten)
                    pack.Overwrite()
                Next
            End Sub
            Private Shared Sub DropOwnership(ByVal list As IEnumerable(Of CarPack))
                For Each pack As CarPack In (From x In list Where x.Overwritten)
                    pack.Revert()
                Next
            End Sub

            Private Sub MakeStandard()
                If Pack.Availability = Availability.Optional Then TakeOwnership(OptionalOn)
                If Pack.Availability = Availability.NotAvailable Then TakeOwnership(NotAvailableOn)
                Pack.Availability = Availability.Standard
                DropOwnership(StandardOn)
            End Sub
            Private Sub MakeNotAvailable()
                If Pack.Availability = Availability.Standard Then TakeOwnership(StandardOn)
                If Pack.Availability = Availability.Optional Then TakeOwnership(OptionalOn)
                Pack.Availability = Availability.NotAvailable
                DropOwnership(NotAvailableOn)
            End Sub
            Private Sub MakeOptional()
                If Pack.Availability = Availability.Standard Then TakeOwnership(StandardOn)
                If Pack.Availability = Availability.NotAvailable Then TakeOwnership(NotAvailableOn)
                Pack.Availability = Availability.Optional


                'DROP OWNERSHIP OF ALL THE ITEMS WITH THE SAME PRICE!
                For Each carPack As CarPack In OptionalOn.Where(Function(x) x.Overwritten AndAlso x.Price.Equals(Pack.Price) AndAlso x.VatPrice.Equals(Pack.VatPrice))
                    carPack.Revert()
                Next



            End Sub


#End Region

#Region " Constructors "
            Public Sub New(ByVal pack As ModelGenerationGradePack)
                _pack = pack
            End Sub
#End Region

        End Class

#End Region

#End Region

#Region " Normalize Pack Content "
        Public Shared Sub NormalizePackContent(ByVal modelGeneration As ModelGeneration, Optional allCars As Boolean = True)
            NormalizeCarPackContent(modelGeneration, allCars)
        End Sub
        Private Shared Sub NormalizeCarPackContent(ByVal modelGeneration As ModelGeneration, ByVal allCars As Boolean)
            Dim cars As IEnumerable(Of Car) = From car In modelGeneration.Cars Where (allCars OrElse car.Approved OrElse car.Preview)
            Dim generationPackItems As Dictionary(Of PackEquipmentCombo, GenerationPackEquipmentAvailability) = modelGeneration.Packs.SelectMany(Function(x) x.Equipment).ToDictionary(Function(y) New PackEquipmentCombo(y), Function(y) New GenerationPackEquipmentAvailability(y))
            For Each car As Car In cars
                For Each carPack As CarPack In car.Packs
                    For Each carPackItem As CarPackItem In carPack.Equipment
                        Dim modelGenerationPackItem As ModelGenerationPackItem = modelGeneration.Packs(carPack.ID).Equipment(carPackItem.ID)
                        Dim combo As PackEquipmentCombo = New PackEquipmentCombo(modelGenerationPackItem)
                        Select Case carPackItem.Availability
                            Case Availability.Standard
                                generationPackItems(combo).StandardOn.Add(carPackItem)
                            Case Availability.Optional
                                generationPackItems(combo).OptionalOn.Add(carPackItem)
                            Case Availability.NotAvailable
                                generationPackItems(combo).NotAvailableOn.Add(carPackItem)
                        End Select
                    Next
                Next
            Next

            For Each generationPackEquipmentAvailability As GenerationPackEquipmentAvailability In generationPackItems.Values
                generationPackEquipmentAvailability.Normalize()
            Next
        End Sub

#Region "Helper classes"
        Private Class PackEquipmentCombo

#Region "Business properties and method"
            Private ReadOnly _packItem As ModelGenerationPackItem
            Private ReadOnly _pack As ModelGenerationPack

            Friend ReadOnly Property PackEquipmentItem() As ModelGenerationPackItem
                Get
                    Return _packItem
                End Get
            End Property

            Private ReadOnly Property Pack() As ModelGenerationPack
                Get
                    Return _pack
                End Get
            End Property
#End Region
#Region "System.object overrides"
            Private Overloads Function Equals(ByVal obj As PackEquipmentCombo) As Boolean
                Return Not (obj Is Nothing) AndAlso Pack.ID.Equals(obj.Pack.ID) AndAlso PackEquipmentItem.ID.Equals(obj.PackEquipmentItem.ID)
            End Function
            Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
                Return Equals(DirectCast(obj, PackEquipmentCombo))
            End Function
            Public Overrides Function GetHashCode() As Integer
                Return _packItem.ID.GetHashCode()
            End Function
#End Region

#Region "Constructors"
            Public Sub New(ByVal packItem As ModelGenerationPackItem)
                _packItem = packItem
                _pack = packItem.Pack
            End Sub
#End Region
        End Class

        Private Class GenerationPackEquipmentAvailability
#Region "Business properties and method"
            Private ReadOnly _packEquipmentCombo As PackEquipmentCombo

            Private ReadOnly _standardOn As List(Of CarPackItem) = New List(Of CarPackItem)
            Private ReadOnly _optionalOn As List(Of CarPackItem) = New List(Of CarPackItem)
            Private ReadOnly _notAvailableOn As List(Of CarPackItem) = New List(Of CarPackItem)

            Private ReadOnly Property PackEquipmentItem() As ModelGenerationPackItem
                Get
                    Return _packEquipmentCombo.PackEquipmentItem
                End Get
            End Property

            Private ReadOnly Property MostlyStandard() As Boolean
                Get
                    If _standardOn.Count = 0 AndAlso _optionalOn.Count = 0 AndAlso _notAvailableOn.Count = 0 Then Return False
                    Return (_standardOn.Count >= _optionalOn.Count AndAlso _standardOn.Count >= _notAvailableOn.Count)
                End Get
            End Property

            Private ReadOnly Property MostlyOptional() As Boolean
                Get
                    Return (_optionalOn.Count > _standardOn.Count AndAlso _optionalOn.Count >= _notAvailableOn.Count)
                End Get
            End Property

            Private ReadOnly Property MostlyNotAvailable() As Boolean
                Get
                    If _standardOn.Count = 0 AndAlso _optionalOn.Count = 0 AndAlso _notAvailableOn.Count = 0 Then Return True
                    Return (_notAvailableOn.Count > _standardOn.Count AndAlso _notAvailableOn.Count > _optionalOn.Count)
                End Get
            End Property

            Public ReadOnly Property StandardOn() As List(Of CarPackItem)
                Get
                    Return _standardOn
                End Get
            End Property
            Public ReadOnly Property OptionalOn() As List(Of CarPackItem)
                Get
                    Return _optionalOn
                End Get
            End Property
            Public ReadOnly Property NotAvailableOn() As List(Of CarPackItem)
                Get
                    Return _notAvailableOn
                End Get
            End Property

            Public Sub Normalize()
                Select Case True
                    Case MostlyStandard
                        MakeStandard()
                    Case MostlyOptional
                        MakeOptional()
                    Case MostlyNotAvailable
                        If StandardOn.Count = 0 AndAlso OptionalOn.Count = 0 Then
                            MakeNotAvailable()
                        Else
                            If StandardOn.Count > OptionalOn.Count Then
                                MakeStandard()
                            Else
                                MakeOptional()
                            End If
                        End If
                    Case Else
                        Throw New Exceptions.DevelopperException("Normalize PackContent;Collection Status Retrival;undefined")
                End Select

            End Sub

            Private Shared Sub TakeOwnership(ByVal list As IEnumerable(Of CarPackItem))
                For Each packItem As CarPackItem In (From x In list Where Not x.Overwritten)
                    packItem.Overwrite()
                Next
            End Sub
            Private Shared Sub DropOwnership(ByVal list As IEnumerable(Of CarPackItem))
                For Each packItem As CarPackItem In (From x In list Where x.Overwritten)
                    packItem.Revert()
                Next
            End Sub

            Private Sub MakeStandard()
                If PackEquipmentItem.Availability = Availability.Optional Then TakeOwnership(OptionalOn)
                If PackEquipmentItem.Availability = Availability.NotAvailable Then TakeOwnership(NotAvailableOn)
                PackEquipmentItem.Availability = Availability.Standard
                PackEquipmentItem.ColouringModes = GetMostOccuringColouringModes(StandardOn)
                DropOwnership(StandardOn)
            End Sub
            Private Sub MakeNotAvailable()
                If PackEquipmentItem.Availability = Availability.Standard Then TakeOwnership(StandardOn)
                If PackEquipmentItem.Availability = Availability.Optional Then TakeOwnership(OptionalOn)
                PackEquipmentItem.Availability = Availability.NotAvailable
                DropOwnership(NotAvailableOn)
            End Sub
            Private Sub MakeOptional()
                If PackEquipmentItem.Availability = Availability.Standard Then TakeOwnership(StandardOn)
                If PackEquipmentItem.Availability = Availability.NotAvailable Then TakeOwnership(NotAvailableOn)
                PackEquipmentItem.Availability = Availability.Optional
                PackEquipmentItem.Price = GetMostOccuringPrice(OptionalOn)
                PackEquipmentItem.VatPrice = GetMostOccuringVatPrice(OptionalOn)
                PackEquipmentItem.ColouringModes = GetMostOccuringColouringModes(OptionalOn)
            End Sub

            Private Shared Function GetMostOccuringPrice(ByVal carPackItems As IEnumerable(Of CarPackItem)) As Decimal
                Dim mostOccurences As Integer = 0
                Dim mostOccuredPrice As Decimal = 0D
                For Each carPackItem As CarPackItem In carPackItems
                    Dim price As Decimal = carPackItem.Price
                    Dim currentOccurences As Integer = carPackItems.Count(Function(x) x.Price.Equals(price) AndAlso x.Pack.ID.Equals(carPackItem.Pack.ID))

                    If currentOccurences > mostOccurences Then
                        mostOccurences = currentOccurences
                        mostOccuredPrice = price
                    End If
                Next

                Return mostOccuredPrice
            End Function

            Private Shared Function GetMostOccuringVatPrice(ByVal carPackItems As IEnumerable(Of CarPackItem)) As Decimal
                Dim mostOccurences As Integer = 0
                Dim mostOccuredPrice As Decimal = 0D
                For Each carPackItem As CarPackItem In carPackItems
                    Dim price As Decimal = carPackItem.VatPrice
                    Dim currentOccurences As Integer = carPackItems.Count(Function(x) x.VatPrice.Equals(price) AndAlso x.Pack.ID.Equals(carPackItem.Pack.ID))

                    If currentOccurences > mostOccurences Then
                        mostOccurences = currentOccurences
                        mostOccuredPrice = price
                    End If
                Next

                Return mostOccuredPrice
            End Function

            Private Shared Function GetMostOccuringColouringModes(ByVal carPackItems As IEnumerable(Of CarPackItem)) As ColouringModes
                Dim mostOccurences As Integer = 0
                Dim mostOccuredColouringModes As ColouringModes
                For Each carPackItem As CarPackItem In carPackItems
                    Dim colouringMode = carPackItem.ColouringModes
                    Dim currentOccurences As Integer = carPackItems.Count(Function(x) x.ColouringModes.Equals(colouringMode) AndAlso x.Pack.ID.Equals(carPackItem.Pack.ID))

                    If currentOccurences > mostOccurences Then
                        mostOccurences = currentOccurences
                        mostOccuredColouringModes = colouringMode
                    End If
                Next

                Return mostOccuredColouringModes
            End Function
#End Region
#Region "Constructors"
            Public Sub New(ByVal packItem As ModelGenerationPackItem)
                _packEquipmentCombo = New PackEquipmentCombo(packItem)
            End Sub
#End Region

        End Class
#End Region

#End Region

#Region " Normalize Colours "

        Public Shared Sub NormalizeColours(ByVal generation As ModelGeneration, ByVal cars As IEnumerable(Of Car), Optional ByVal allCars As Boolean = True)
            Dim approvedGrades As List(Of ModelGenerationGrade) = (From x In generation.Grades Where cars.Any(Function(y) y.GradeID.Equals(x.ID)) AndAlso (allCars OrElse x.Preview OrElse x.Approved)).ToList()
            For Each gradeFromIteration As ModelGenerationGrade In approvedGrades
                Dim grade = gradeFromIteration
                Dim approvedBodyTypes As List(Of ModelGenerationGradeBodyType) = (From x In grade.BodyTypes Where cars.Any(Function(y) y.BodyTypeID.Equals(x.ID)) AndAlso (allCars OrElse generation.BodyTypes(x.ID).Preview OrElse generation.BodyTypes(x.ID).Approved)).ToList()
                For Each bodyType As ModelGenerationGradeBodyType In approvedBodyTypes
                    NormalizeColours(generation, bodyType, allCars)
                Next
            Next
        End Sub

        Public Shared Sub NormalizeColours(ByVal generation As ModelGeneration, Optional ByVal allCars As Boolean = True)
            Dim approvedGrades As IEnumerable(Of ModelGenerationGrade) = From x In generation.Grades Where allCars OrElse x.Preview OrElse x.Approved
            For Each grade As ModelGenerationGrade In approvedGrades
                NormalizeColours(generation, grade, allCars)
            Next
        End Sub
        Public Shared Sub NormalizeColours(ByVal generation As ModelGeneration, ByVal car As Car, Optional ByVal allCars As Boolean = True)
            If Not car.ColourCombinations.IsDirty Then Exit Sub
            NormalizeColours(generation, generation.Grades(car.GradeID).BodyTypes(car.BodyTypeID), allCars)
        End Sub
        Public Shared Sub NormalizeColours(ByVal generation As ModelGeneration, ByVal grade As ModelGenerationGrade, Optional ByVal allCars As Boolean = True)
            Dim approvedBodyTypes As IEnumerable(Of ModelGenerationGradeBodyType) = From x In grade.BodyTypes Where allCars OrElse generation.BodyTypes(x.ID).Preview OrElse generation.BodyTypes(x.ID).Approved
            For Each bodyType As ModelGenerationGradeBodyType In approvedBodyTypes
                NormalizeColours(generation, bodyType, allCars)
            Next
        End Sub

        Private Shared Sub NormalizeColours(ByVal generation As ModelGeneration, ByVal gradeBody As ModelGenerationGradeBodyType, ByVal allCars As Boolean)

            'ADD MISSING COLOUR COMBINATIONS
            For Each oCombination As ModelGenerationColourCombination In (From x In generation.ColourCombinations Where Not gradeBody.ColourCombinations.Contains(x.ExteriorColour.ID, x.Upholstery.ID))
                gradeBody.ColourCombinations.Add(oCombination)
            Next

            Dim gradeBodyCombinations As IEnumerable(Of GradeBodyColourCombination) = (From x In gradeBody.ColourCombinations Select New GradeBodyColourCombination(x)).ToList()
            Dim gradeBodyExteriorColours As IEnumerable(Of GradeBodyExteriorColour) = (From x In gradeBody.ColourCombinations.ExteriorColours Select New GradeBodyExteriorColour(x)).ToList()
            Dim gradeBodyUpholsteries As IEnumerable(Of GradeBodyUpholstery) = (From x In gradeBody.ColourCombinations.Upholsteries Select New GradeBodyUpholstery(x)).ToList()

            'Next add car colour for each approved car to grade body colour(for the correct grade & body of course)

            Dim cars As List(Of Car) = (From car In generation.Cars Where car.GradeID.Equals(gradeBody.Grade.ID) AndAlso car.BodyTypeID.Equals(gradeBody.ID) AndAlso (allCars OrElse car.Approved OrElse car.Preview)).ToList()
            For Each carFromIteration As Car In cars
                Dim car = carFromIteration
                'ADD MISSING COLOUR COMBINATIONS
                For Each oCombination As ModelGenerationColourCombination In (From x In generation.ColourCombinations Where Not car.ColourCombinations.Contains(x.ExteriorColour.ID, x.Upholstery.ID))
                    car.ColourCombinations.Add(oCombination)
                Next

                'GATHER COLOUR COMBINATIONS
                For Each oCarItem As LinkedColourCombination In car.ColourCombinations
                    Dim oGradeBodyItem As GradeBodyColourCombination = gradeBodyCombinations.First(Function(x) x.Combination.ExteriorColour.ID = oCarItem.ExteriorColour.ID AndAlso x.Combination.Upholstery.ID.Equals(oCarItem.Upholstery.ID))
                    Select Case True
                        Case oCarItem.Approved
                            oGradeBodyItem.AvailableOn.Add(oCarItem)
                        Case Else
                            oGradeBodyItem.NotAvailableOn.Add(oCarItem)
                    End Select
                Next

                'GATHER EXTERIOR COLOURS
                For Each oCarItem As LinkedExteriorColour In car.ColourCombinations.ExteriorColours
                    Dim oGradeBodyItem As GradeBodyExteriorColour = gradeBodyExteriorColours.First(Function(x) x.ExteriorColour.ID.Equals(oCarItem.ID))
                    If car.ColourCombinations.Any(Function(combination) combination.Approved AndAlso combination.ExteriorColour.ID.Equals(oCarItem.ID)) Then
                        oGradeBodyItem.ApprovedOn.Add(oCarItem)
                    Else
                        oGradeBodyItem.DeclinedOn.Add(oCarItem)
                    End If
                Next

                'GATHER UPHOLSTERIES
                For Each oCarItem As LinkedUpholstery In car.ColourCombinations.Upholsteries
                    Dim oGradeBodyItem As GradeBodyUpholstery = gradeBodyUpholsteries.First(Function(x) x.Upholstery.ID.Equals(oCarItem.ID))
                    If car.ColourCombinations.Any(Function(combination) combination.Approved AndAlso combination.Upholstery.ID.Equals(oCarItem.ID)) Then
                        oGradeBodyItem.ApprovedOn.Add(oCarItem)
                    Else
                        oGradeBodyItem.DeclinedOn.Add(oCarItem)
                    End If
                Next

            Next

            'NORMALIZE COLOUR COMBINATIONS
            For Each oGradeBodyItem As GradeBodyColourCombination In gradeBodyCombinations
                oGradeBodyItem.Normalize()
            Next

            'NORMALIZE EXTERIOR COLOURS
            For Each oGradeBodyItem As GradeBodyExteriorColour In gradeBodyExteriorColours
                oGradeBodyItem.Normalize()
            Next

            'NORMALIZE UPHOLSTERIES
            For Each oGradeBodyItem As GradeBodyUpholstery In gradeBodyUpholsteries
                oGradeBodyItem.Normalize()
            Next

        End Sub

#Region " Helper Classes "
        Private Class GradeBodyColourCombination

#Region " Business Propererties & Methodes "
            Private ReadOnly _combination As LinkedColourCombination

            Private ReadOnly _availableOn As List(Of LinkedColourCombination) = New List(Of LinkedColourCombination)
            Private ReadOnly _notAvailableOn As List(Of LinkedColourCombination) = New List(Of LinkedColourCombination)

            Public ReadOnly Property Combination() As LinkedColourCombination
                Get
                    Return _combination
                End Get
            End Property

            Private ReadOnly Property MostlyAvailable() As Boolean
                Get
                    Return Not (_availableOn.Count = 0) AndAlso (_availableOn.Count >= _notAvailableOn.Count)
                End Get
            End Property

            Private ReadOnly Property MostlyNotAvailable() As Boolean
                Get
                    Return (_availableOn.Count = 0) OrElse (_notAvailableOn.Count > _availableOn.Count)
                End Get
            End Property

            Public ReadOnly Property AvailableOn() As List(Of LinkedColourCombination)
                Get
                    Return _availableOn
                End Get
            End Property
            Public ReadOnly Property NotAvailableOn() As List(Of LinkedColourCombination)
                Get
                    Return _notAvailableOn
                End Get
            End Property

            Public Sub Normalize()
                Select Case True
                    Case MostlyAvailable
                        MakeAvailable()
                    Case MostlyNotAvailable
                        If AvailableOn.Count > 0 Then
                            MakeAvailable()
                        Else
                            MakeNotAvailable()
                        End If
                    Case Else
                        Throw New Exceptions.DevelopperException("Normalize Colours;Collection Status Retrival;undefined")
                End Select

            End Sub

            Private Shared Sub DropOwnership(ByVal list As IEnumerable(Of LinkedColourCombination))
                For Each combination As LinkedColourCombination In (From x In list Where x.HasOwnership())
                    combination.Delete()
                Next
            End Sub
            Private Shared Sub TakeOwnership(ByVal list As IEnumerable(Of LinkedColourCombination))
                For Each combination As LinkedColourCombination In (From x In list Where Not x.HasOwnership())
                    combination.TakeOwnership()
                Next
            End Sub

            Private Sub MakeNotAvailable()
                If Combination.Approved Then TakeOwnership(AvailableOn)
                Combination.Approved = False
                DropOwnership(NotAvailableOn)
            End Sub
            Private Sub MakeAvailable()
                If Not Combination.Approved Then TakeOwnership(NotAvailableOn)
                Combination.Approved = True
                DropOwnership(AvailableOn)
            End Sub
#End Region

#Region " Constructors "
            Public Sub New(ByVal item As LinkedColourCombination)
                'Prevent direct creation
                _combination = item
            End Sub
#End Region

        End Class
        Private Class GradeBodyExteriorColour

#Region " Business Propererties & Methodes "
            Private ReadOnly _exteriorColour As LinkedExteriorColour
            Private ReadOnly _approvedOn As List(Of LinkedExteriorColour) = New List(Of LinkedExteriorColour)
            Private ReadOnly _declinedOn As List(Of LinkedExteriorColour) = New List(Of LinkedExteriorColour)

            Public ReadOnly Property ExteriorColour() As LinkedExteriorColour
                Get
                    Return _exteriorColour
                End Get
            End Property

            Public ReadOnly Property ApprovedOn() As List(Of LinkedExteriorColour)
                Get
                    Return _approvedOn
                End Get
            End Property
            Public ReadOnly Property DeclinedOn() As List(Of LinkedExteriorColour)
                Get
                    Return _declinedOn
                End Get
            End Property

            Public Sub Normalize()
                'DROP ALL DECLINED EXTERIORS
                DropOwnership(DeclinedOn)

                If ApprovedOn.Count > 0 Then
                    NormalizePrices()
                Else
                    'IF THIS EXTERIOR IS NOT APPROVED FOR ANY VEHICLE, DROP THE OWENRSHIP AS WELL
                    DropOwnership(ApprovedOn)
                End If


            End Sub
            Private Sub NormalizePrices()

                'FIRST TAKE OWNERSHIP OF ALL THE EXTERIOR COLOURS (IN CASE PRICE SHOULD CHANGE)
                TakeOwnership(ApprovedOn)

                'GET THE MOST OCCURING PRICE
                Dim mostOcurringPrice As LinkedExteriorColour = (From x In ApprovedOn Group x By k = GetPriceHash(x) Into Group Order By Group.Count() Descending Select Group).First().First()

                'SET THE PRICES ON THE GRADE OBJECT
                ExteriorColour.Price = mostOcurringPrice.Price
                ExteriorColour.VatPrice = mostOcurringPrice.VatPrice

                'REMOVE OWNERSHIP OF ALL THE ITEMS WITH THE SAME PRICE AND THE SAME ASSETS !
                Dim priceHash As String = GetPriceHash(mostOcurringPrice)
                For Each item As LinkedExteriorColour In (From x In ApprovedOn Where GetPriceHash(x).Equals(priceHash))
                    item.Revert()
                Next
            End Sub
            Private Shared Function GetPriceHash(ByVal item As LinkedExteriorColour) As String
                Return String.Format("{0}_{1}", item.Price.ToString(), item.VatPrice.ToString())
            End Function

            Private Shared Sub DropOwnership(ByVal list As IEnumerable(Of LinkedExteriorColour))
                For Each exteriorColour As LinkedExteriorColour In (From x In list Where x.Overwritten())
                    exteriorColour.Revert()
                Next
            End Sub
            Private Shared Sub TakeOwnership(ByVal list As IEnumerable(Of LinkedExteriorColour))
                For Each exteriorColour As LinkedExteriorColour In (From x In list Where Not x.Overwritten())
                    exteriorColour.Overwrite()
                Next
            End Sub

#End Region

#Region " Constructors "
            Public Sub New(ByVal item As LinkedExteriorColour)
                'Prevent direct creation
                _exteriorColour = item
            End Sub
#End Region

        End Class
        Private Class GradeBodyUpholstery

#Region " Business Propererties & Methodes "
            Private ReadOnly _upholstery As LinkedUpholstery
            Private ReadOnly _approvedOn As List(Of LinkedUpholstery) = New List(Of LinkedUpholstery)
            Private ReadOnly _declinedOn As List(Of LinkedUpholstery) = New List(Of LinkedUpholstery)

            Public ReadOnly Property Upholstery() As LinkedUpholstery
                Get
                    Return _upholstery
                End Get
            End Property

            Public ReadOnly Property ApprovedOn() As List(Of LinkedUpholstery)
                Get
                    Return _approvedOn
                End Get
            End Property
            Public ReadOnly Property DeclinedOn() As List(Of LinkedUpholstery)
                Get
                    Return _declinedOn
                End Get
            End Property

            Public Sub Normalize()
                'DROP ALL DECLINED UPHOLSTERIES
                DropOwnership(DeclinedOn)

                If ApprovedOn.Count > 0 Then
                    NormalizePrices()
                Else
                    'IF THIS UPHOLSTERY IS NOT APPROVED FOR ANY VEHICLE, DROP THE OWENRSHIP AS WELL
                    DropOwnership(ApprovedOn)
                End If
            End Sub
            Private Sub NormalizePrices()

                'FIRST TAKE OWNERSHIP OF ALL THE UPHOLSTERIES (IN CASE PRICE SHOULD CHANGE)
                TakeOwnership(ApprovedOn)

                'GET THE MOST OCCURING PRICE
                Dim mostOccuringPrice As LinkedUpholstery = (From x In ApprovedOn Group x By k = GetPriceHash(x) Into Group Order By Group.Count() Descending Select Group).First().First()

                'SET THE PRICES ON THE GRADE OBJECT
                Upholstery.Price = mostOccuringPrice.Price
                Upholstery.VatPrice = mostOccuringPrice.VatPrice

                'REMOVE OWNERSHIP OF ALL THE ITEMS WITH THE SAME PRICE AND ASSETS !
                Dim priceHash As String = GetPriceHash(mostOccuringPrice)
                For Each item As LinkedUpholstery In (From x In ApprovedOn Where GetPriceHash(x).Equals(priceHash))
                    item.Revert()
                Next
            End Sub
            Private Shared Function GetPriceHash(ByVal item As LinkedUpholstery) As String
                Return String.Format("{0}_{1}", item.Price.ToString(), item.VatPrice.ToString())
            End Function

            Private Shared Sub DropOwnership(ByVal list As IEnumerable(Of LinkedUpholstery))
                For Each item As LinkedUpholstery In (From x In list Where x.Overwritten())
                    item.Revert()
                Next
            End Sub
            Private Shared Sub TakeOwnership(ByVal list As IEnumerable(Of LinkedUpholstery))
                For Each item As LinkedUpholstery In (From x In list Where Not x.Overwritten())
                    item.Overwrite()
                Next
            End Sub
#End Region

#Region " Constructors "
            Public Sub New(ByVal item As LinkedUpholstery)
                'Prevent direct creation
                _upholstery = item
            End Sub
#End Region

        End Class
#End Region

#End Region

#Region " Normalize Technical Specifications "
        Public Shared Sub NormalizeSpecifications(ByVal generation As ModelGeneration, ByVal cars As IEnumerable(Of Car), Optional ByVal allCars As Boolean = True)
            NormalizeSpecifications(generation, allCars, cars.ToList())
        End Sub
        Public Shared Sub NormalizeSpecifications(ByVal generation As ModelGeneration, Optional ByVal allCars As Boolean = True)
            NormalizeSpecifications(generation, allCars, Nothing)
        End Sub
        Public Shared Sub NormalizeSpecifications(ByVal generation As ModelGeneration, ByVal car As Car, Optional ByVal allCars As Boolean = True)
            'If Not car.Specifications.IsDirty Then Exit Sub

            Dim cars As List(Of Car) = New List(Of Car)(1)
            cars.Add(car)

            NormalizeSpecifications(generation, allCars, cars)
        End Sub

        Private Shared Sub NormalizeSpecifications(ByVal generation As ModelGeneration, ByVal allCars As Boolean, ByVal onlyForCars As IList(Of Car))
            Dim specifications As IEnumerable(Of ModelGenerationSpecification) = From x In generation.Specifications Where x.Approved
            For Each specification As ModelGenerationSpecification In specifications
                NormalizeSpecification(generation, specification, allCars, onlyForCars)
            Next
        End Sub


        Private Shared Sub NormalizeSpecification(ByVal generation As ModelGeneration, ByVal specification As ModelGenerationSpecification, ByVal allCars As Boolean, ByVal onlyForCars As IList(Of Car))
            Dim partialCarSpecificationTemplate As PartialCarSpecification = PartialCarSpecification.NewPartialCarSpecification(generation.Model.ID, generation.ID)
            Dim partialCarSpecifications As ArrayList = New ArrayList

            Call GatherPartialCarSpecifications(generation, specification, GetMaxLevel(specification), 3, partialCarSpecificationTemplate, partialCarSpecifications)


            Dim items As List(Of SpecificationItem) = New List(Of SpecificationItem)
            For Each partialCarSpec As PartialCarSpecification In partialCarSpecifications
                If onlyForCars Is Nothing OrElse onlyForCars.Any(Function(x) partialCarSpec.Matches(x.PartialCarSpecification)) Then
                    Dim partialCarSpecification As PartialCarSpecification = partialCarSpec
                    Dim cars As IEnumerable(Of Car) = (From x In generation.Cars Where (allCars OrElse x.Approved OrElse x.Preview) AndAlso partialCarSpecification.Matches(x.PartialCarSpecification))
                    If cars.Count > 0 Then
                        Dim item As SpecificationItem = New SpecificationItem(specification, partialCarSpec)
                        For Each car As Car In cars
                            If car.Specifications(specification.ID) Is Nothing Then
                                Throw New ApplicationException(String.Format("The specification ""{0}"" ({1}) is not available for the vehicle ""{2}"" ({3}).", specification.Name, specification.ID, car.Name, car.ID))
                            End If
                            item.CarSpecifications.Add(car.Specifications(specification.ID))
                        Next
                        items.Add(item)
                    End If
                End If
            Next

            'Then delegate Normalize call to each equipment item
            For Each item As SpecificationItem In items
                item.Normalize()
            Next

        End Sub


        Private Shared Sub GatherPartialCarSpecifications(ByVal generation As ModelGeneration, ByVal specification As ModelGenerationSpecification, ByVal maxLevel As Integer, ByVal currentLevel As Integer, ByVal partialCarSpecificationTemplate As PartialCarSpecification, ByVal partialCarSpecifications As IList)

            If currentLevel > maxLevel Then
                partialCarSpecifications.Add(partialCarSpecificationTemplate.Clone())
                Exit Sub
            End If

            Select Case True
                Case specification.Dependency.BodyTypeOrder = currentLevel
                    For Each oBody As ModelGenerationBodyType In generation.BodyTypes
                        partialCarSpecificationTemplate.BodyTypeID = oBody.ID
                        GatherPartialCarSpecifications(generation, specification, maxLevel, (currentLevel + 1), partialCarSpecificationTemplate.Clone(), partialCarSpecifications)
                    Next

                Case specification.Dependency.EngineOrder = currentLevel
                    For Each oEngine As ModelGenerationEngine In generation.Engines
                        partialCarSpecificationTemplate.EngineID = oEngine.ID
                        GatherPartialCarSpecifications(generation, specification, maxLevel, (currentLevel + 1), partialCarSpecificationTemplate.Clone(), partialCarSpecifications)
                    Next

                Case specification.Dependency.TransmissionOrder = currentLevel
                    For Each oTransmission As ModelGenerationTransmission In generation.Transmissions
                        partialCarSpecificationTemplate.TransmissionID = oTransmission.ID
                        GatherPartialCarSpecifications(generation, specification, maxLevel, (currentLevel + 1), partialCarSpecificationTemplate.Clone(), partialCarSpecifications)
                    Next

                Case specification.Dependency.GradeOrder = currentLevel
                    For Each oGrade As ModelGenerationGrade In generation.Grades
                        partialCarSpecificationTemplate.GradeID = oGrade.ID
                        GatherPartialCarSpecifications(generation, specification, maxLevel, (currentLevel + 1), partialCarSpecificationTemplate.Clone(), partialCarSpecifications)
                    Next

                Case specification.Dependency.WheelDriveOrder = currentLevel
                    For Each oWheelDrive As ModelGenerationWheelDrive In generation.WheelDrives
                        partialCarSpecificationTemplate.WheelDriveID = oWheelDrive.ID
                        GatherPartialCarSpecifications(generation, specification, maxLevel, (currentLevel + 1), partialCarSpecificationTemplate.Clone(), partialCarSpecifications)
                    Next

                Case Else
                    Throw New ApplicationException("Could not find a dependency on level " + currentLevel.ToString + " for '" + specification.Name + "'")
            End Select

        End Sub
        Private Shared Function GetMaxLevel(ByVal specification As ModelGenerationSpecification) As Integer
            With specification.Dependency
                Dim iMaxLevel As Integer = .BodyTypeOrder
                If .EngineOrder > iMaxLevel Then iMaxLevel = .EngineOrder
                If .TransmissionOrder > iMaxLevel Then iMaxLevel = .TransmissionOrder
                If .GradeOrder > iMaxLevel Then iMaxLevel = .GradeOrder
                If .WheelDriveOrder > iMaxLevel Then iMaxLevel = .WheelDriveOrder
                Return iMaxLevel
            End With
        End Function


#Region " Helper Classes "

        Private Class SpecificationItem

#Region " Business Propererties & Methodes "
            Private ReadOnly _specification As ModelGenerationSpecification
            Private ReadOnly _partialCarSpecification As PartialCarSpecification

            Private ReadOnly _carSpecifications As List(Of CarSpecification) = New List(Of CarSpecification)

            Private ReadOnly Property Specification() As ModelGenerationSpecification
                Get
                    Return _specification
                End Get
            End Property

            Private ReadOnly Property PartialCarSpecification() As PartialCarSpecification
                Get
                    Return _partialCarSpecification
                End Get
            End Property

            Public ReadOnly Property CarSpecifications() As List(Of CarSpecification)
                Get
                    Return _carSpecifications
                End Get
            End Property


            Public Sub Normalize()
                Dim countrylanguages As IEnumerable(Of Language)
                If (MyContext.GetContext().Country.Region IsNot Nothing) Then
                    countrylanguages = (From country In MyContext.GetContext().Country.Region.Countries
                                            From language In country.Languages
                                            Select language).ToList()
                Else
                    countrylanguages = (From language In MyContext.GetContext().Country.Languages
                                        Select language).ToList()
                End If
                For Each countryLanguage In countrylanguages
                    Normalize(countryLanguage.Country.Code, countryLanguage.Code)
                Next
            End Sub
            Private Sub Normalize(ByVal country As String, ByVal language As String)

                'OPTIONALY CREATE THE VALUE SET
                Dim generationValue As ModelGenerationSpecificationValue = Specification.Values(country, language, PartialCarSpecification)
                If generationValue Is Nothing Then generationValue = Specification.Values.Add(country, language, PartialCarSpecification)

                'GET THE MAX VALUES
                Dim carValues = (From carSpecification In CarSpecifications
                                 From carValue In carSpecification.Values
                                 Where carValue.LanguageCode.Equals(language, StringComparison.InvariantCultureIgnoreCase)
                                 Select carValue).ToList()

                If (Not carValues.Any()) Then 'if no language specific values were found
                    generationValue.MasterValue = Nothing
                    generationValue.Value = Nothing
                    generationValue.Homologated = False

                    Return
                End If

                If Specification.HasMapping Then
                    Dim maxA2PValue As String = (From x In carValues Group x By key = x.MasterValue Into Group Order By Group.Count() Descending Select Group).First().First().MasterValue
                    Dim maxCustomValue = (From x In carValues
                                                    Where (maxA2PValue Is Nothing AndAlso x.MasterValue Is Nothing) OrElse
                                                          (Not maxA2PValue Is Nothing AndAlso maxA2PValue.Equals(x.MasterValue))
                                                    Group x By key = x.Value Into Group
                                                    Order By Group.Count(), (key Is Nothing) Descending
                                                    Select Group).FirstOrDefault()

                    generationValue.MasterValue = maxA2PValue
                    generationValue.Value = If(maxCustomValue IsNot Nothing, _
                                                   If(maxCustomValue.FirstOrDefault() IsNot Nothing, maxCustomValue.FirstOrDefault().Value, Nothing), _
                                                   Nothing)

                    If maxA2PValue Is Nothing Then
                        generationValue.Homologated = False
                    Else
                        generationValue.Homologated = (From x In carValues Where maxA2PValue.Equals(x.MasterValue)).All(Function(x) x.Homologated)
                    End If

                Else

                    Dim maxCustomValue = (From x In carValues Group x By key = GetString(x.Value) Into Group Order By Group.Count() Descending Select Group).FirstOrDefault()
                    generationValue.Value = If(maxCustomValue IsNot Nothing, _
                                               If(maxCustomValue.FirstOrDefault() IsNot Nothing, maxCustomValue.FirstOrDefault().Value, Nothing), _
                                               Nothing)
                End If


                'TODO: Fix this
                'For Each carValue As CarSpecificationValue In carValues
                '    If CompareValues(generationValue, carValue) Then
                '        If carValue.Overwritten Then carValue.Revert()
                '    Else
                '        If Not carValue.Overwritten Then carValue.Overwrite()
                '    End If
                'Next
            End Sub

            Private Function CompareValues(ByVal generationValue As ModelGenerationSpecificationValue, ByVal carValue As CarSpecificationValue) As Boolean
                If Not GetString(generationValue.Value).Equals(GetString(carValue.Value), StringComparison.InvariantCultureIgnoreCase) Then Return False
                If Specification.HasMapping AndAlso Not GetString(generationValue.MasterValue).Equals(GetString(carValue.MasterValue), StringComparison.InvariantCultureIgnoreCase) Then Return False
                If Specification.HasMapping AndAlso Not generationValue.Homologated.Equals(carValue.Homologated) Then Return False
                Return True
            End Function
            Private Shared Function GetString(ByVal value As String) As String
                If value Is Nothing Then Return String.Empty
                Return value
            End Function

#End Region

#Region " Constructors "

            Public Sub New(ByVal specification As ModelGenerationSpecification, ByVal partialCarSpecification As PartialCarSpecification)
                'Prevent direct creation
                _specification = specification
                _partialCarSpecification = partialCarSpecification
            End Sub
#End Region

        End Class

#End Region

#End Region

#Region "Shared Factory Methods"
        Public Shared Function GetNormalizer() As Normalizer
            Return New Normalizer()
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region
    End Class
End Namespace