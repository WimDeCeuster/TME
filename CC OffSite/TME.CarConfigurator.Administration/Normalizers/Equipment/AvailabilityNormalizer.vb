
Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Normalizers.Equipment
    Friend Class AvailabilityNormalizer
        Implements IGradeNormalizer

        Private Property Type() As EquipmentType
        Public Sub New(ByVal type As EquipmentType)
            Me.Type = type
        End Sub

        Public Sub Normalize(ByVal grade As ModelGenerationGrade, ByVal cars As IEnumerable(Of Car)) Implements IGradeNormalizer.Normalize

            If Type = EquipmentType.Option Then
                Dim options = grade.Equipment.OfType(Of ModelGenerationGradeOption).ToList()
                Dim optionsAtTheEndOfTheLine = options.Where(Function(x) Not x.HasChildOptions)
                Dim gradeOptionAvailabilities As Dictionary(Of Guid, GradeEquipmentAvailability) = GetGradeEquipmentAvailabilities(options, cars)
                NormalizeOptionsFromTheBottomUp(optionsAtTheEndOfTheLine, gradeOptionAvailabilities)
                Return
            End If


            Dim equipment = (From x In grade.Equipment Where x.Type = Type Select x).ToList()
            Dim gradeEquipmentAvailabilities As Dictionary(Of Guid, GradeEquipmentAvailability) = GetGradeEquipmentAvailabilities(equipment, cars)
            Normalize(equipment, gradeEquipmentAvailabilities)
        End Sub

        Private Shared Sub NormalizeOptionsFromTheBottomUp(ByVal childOptions As IEnumerable(Of ModelGenerationGradeOption), ByVal gradeEquipmentAvailabilities As Dictionary(Of Guid, GradeEquipmentAvailability))
            Normalize(childOptions, gradeEquipmentAvailabilities)

            Dim parentOptions = (From childOption In childOptions
                                Where childOption.HasParentOption
                                Select childOption.ParentOption).Distinct().ToList()

            If parentOptions.Any() Then
                NormalizeOptionsFromTheBottomUp(parentOptions, gradeEquipmentAvailabilities)
            End If
        End Sub
        Private Shared Sub Normalize(ByVal equipmentToNormalize As IEnumerable(Of ModelGenerationGradeEquipmentItem), ByVal gradeEquipmentAvailabilities As Dictionary(Of Guid, GradeEquipmentAvailability))

            Dim gradeEquipmentAvailabilitiesToNormalize = From equipmentItem In equipmentToNormalize Select gradeEquipmentAvailabilities.Item(equipmentItem.ID)
            For Each gradeEquipmentAvailability In gradeEquipmentAvailabilitiesToNormalize
                gradeEquipmentAvailability.Normalize()
            Next

        End Sub

        Private Shared Function GetGradeEquipmentAvailabilities(ByVal equipment As IEnumerable(Of ModelGenerationGradeEquipmentItem), ByVal cars As IEnumerable(Of Car)) As Dictionary(Of Guid, GradeEquipmentAvailability)

            Dim gradeEquipmentAvailabilities As Dictionary(Of Guid, GradeEquipmentAvailability) = equipment.ToDictionary(Function(x) x.ID, Function(x) New GradeEquipmentAvailability(x))

            ' add car equipment for each approved car to grade equipment (for the correct grade of course)
            For Each _car As Car In cars
                Dim _carEquipment As List(Of CarEquipmentItem) = (From x In _car.Equipment Where gradeEquipmentAvailabilities.ContainsKey(x.ID)).ToList()
                For Each _carItem As CarEquipmentItem In _carEquipment

                    Dim _availability As Availability = _carItem.Availability
                    If TypeOf _carItem Is CarOption AndAlso DirectCast(_carItem, CarOption).SuffixOption Then
                        _availability = DirectCast(_carItem, CarOption).SuffixAvailability
                    End If

                    Select Case _availability
                        Case Availability.Standard
                            gradeEquipmentAvailabilities.Item(_carItem.ID).StandardOn.Add(_carItem)
                        Case Availability.Optional
                            gradeEquipmentAvailabilities.Item(_carItem.ID).OptionalOn.Add(_carItem)
                        Case Availability.NotAvailable
                            gradeEquipmentAvailabilities.Item(_carItem.ID).NotAvailableOn.Add(_carItem)
                    End Select
                Next
            Next
            Return gradeEquipmentAvailabilities
        End Function


#Region " Helper Classes "

        Private Class GradeEquipmentAvailability

#Region " Business Propererties & Methodes "
            Private ReadOnly _equipmentItem As ModelGenerationGradeEquipmentItem
            Private ReadOnly _suffixOption As Boolean

            Private ReadOnly _standardOn As List(Of CarEquipmentItem) = New List(Of CarEquipmentItem)
            Private ReadOnly _optionalOn As List(Of CarEquipmentItem) = New List(Of CarEquipmentItem)
            Private ReadOnly _notAvailableOn As List(Of CarEquipmentItem) = New List(Of CarEquipmentItem)

            Private Property Availability() As Availability
                Get
                    If SuffixOption Then Return DirectCast(_equipmentItem, ModelGenerationGradeOption).SuffixAvailability
                    Return _equipmentItem.Availability
                End Get
                Set(ByVal value As Availability)
                    If SuffixOption Then
                        With DirectCast(_equipmentItem, ModelGenerationGradeOption)
                            .SuffixAvailability = value
                            .Availability = If(.IsReplaced(), Availability.NotAvailable, value)
                        End With
                    Else
                        _equipmentItem.Availability = value
                    End If
                End Set
            End Property


            Private ReadOnly Property SuffixOption() As Boolean
                Get
                    Return _suffixOption
                End Get
            End Property
            Private ReadOnly Property EquipmentItem() As ModelGenerationGradeEquipmentItem
                Get
                    Return _equipmentItem
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
                    If _standardOn.Count = 0 AndAlso _optionalOn.Count = 0 AndAlso _notAvailableOn.Count = 0 Then Return False
                    Return (_optionalOn.Count > _standardOn.Count AndAlso _optionalOn.Count >= _notAvailableOn.Count)
                End Get
            End Property
            Private ReadOnly Property MostlyNotAvailable() As Boolean
                Get
                    If _standardOn.Count = 0 AndAlso _optionalOn.Count = 0 AndAlso _notAvailableOn.Count = 0 Then Return True
                    Return (_notAvailableOn.Count > _standardOn.Count AndAlso _notAvailableOn.Count > _optionalOn.Count)
                End Get
            End Property

            Public ReadOnly Property StandardOn() As List(Of CarEquipmentItem)
                Get
                    Return _standardOn
                End Get
            End Property
            Public ReadOnly Property OptionalOn() As List(Of CarEquipmentItem)
                Get
                    Return _optionalOn
                End Get
            End Property
            Public ReadOnly Property NotAvailableOn() As List(Of CarEquipmentItem)
                Get
                    Return _notAvailableOn
                End Get
            End Property

            Public Sub Normalize()
                Dim replacement As ModelGenerationGradeOption = Nothing

                If SuffixOption AndAlso (StandardOn.Any() OrElse OptionalOn.Any()) Then
                    Dim carEquipmentItemsOnWhichItIsStandardOrOptional As List(Of CarEquipmentItem) = New List(Of CarEquipmentItem)
                    carEquipmentItemsOnWhichItIsStandardOrOptional.AddRange(StandardOn)
                    carEquipmentItemsOnWhichItIsStandardOrOptional.AddRange(OptionalOn)
                    replacement = FindGradeOptionByWhichItIsReplacedTheMost(carEquipmentItemsOnWhichItIsStandardOrOptional)
                End If

                Select Case True
                    Case MostlyStandard
                        MakeStandard(replacement)
                    Case MostlyOptional
                        MakeOptional(replacement)
                    Case MostlyNotAvailable
                        If StandardOn.Count = 0 AndAlso OptionalOn.Count = 0 Then
                            MakeNotAvailable()
                        Else
                            If StandardOn.Count > OptionalOn.Count Then
                                MakeStandard(replacement)
                            Else
                                MakeOptional(replacement)
                            End If
                        End If
                    Case Else
                        Throw New Exceptions.DevelopperException("Normalize Equipment;Collection Status Retrival;undefined")
                End Select
            End Sub

            Private Shared Function FindGradeOptionByWhichItIsReplacedTheMost(ByVal carEquipmentItemsOnWhichItIsStandardOrOptional As IEnumerable(Of CarEquipmentItem)) As ModelGenerationGradeOption
                Return GetCarOption(carEquipmentItemsOnWhichItIsStandardOrOptional).ReplacedBy
            End Function

            Private Shared Function GetCarOption(ByVal carEquipmentItemsOnWhichItIsStandardOrOptional As IEnumerable(Of CarEquipmentItem)) As CarOption
                Return CarOptionsGroupedByTheGradeOptionTheyAreReplacedByOrderedByMostUsedReplacement(carEquipmentItemsOnWhichItIsStandardOrOptional).First().First() 'take the first group's first caroption (group = {ReplacedBy, IEnumerable<CarOption>})
            End Function

            Private Shared Function CarOptionsGroupedByTheGradeOptionTheyAreReplacedByOrderedByMostUsedReplacement(ByVal carEquipmentItemsOnWhichItIsStandardOrOptional As IEnumerable(Of CarEquipmentItem)) As IEnumerable(Of IEnumerable(Of CarOption))
                Dim carOptionsOnWhichItIsStandardOrOptional  = carEquipmentItemsOnWhichItIsStandardOrOptional.Cast(of CarOption)
                Return _ 
                    From carOption In carOptionsOnWhichItIsStandardOrOptional 
                    Group carOption By groupingKey = carOption.ReplacedBy 
                    Into Group 
                    Order By Group.Count(), (groupingKey Is Nothing)
                    Select Group
            End Function

            Private Shared Sub MakeSureTheAvailabilityMathces(ByVal list As IEnumerable(Of CarEquipmentItem), ByVal availability As Availability)
                For Each item As CarEquipmentItem In (From x In list Where Not x.Availability = availability OrElse Not x.Overwritten)
                    item.Overwrite()
                    item.Availability = availability
                Next
            End Sub
            Private Shared Sub OverwriteToKeepOriginalValue(ByVal list As IEnumerable(Of CarEquipmentItem))
                For Each item As CarEquipmentItem In (From x In list Where Not x.Overwritten)
                    item.Overwrite()
                Next
            End Sub
            Private Shared Sub Revert(ByVal list As IEnumerable(Of CarEquipmentItem))
                For Each item As CarEquipmentItem In (From x In list Where x.Overwritten)
                    item.Revert()
                Next
            End Sub
            Private Sub MakeStandard(ByVal replacement As ModelGenerationGradeOption)
                MakeSureTheAvailabilityMathces(OptionalOn, Enums.Availability.Optional)
                MakeSureTheAvailabilityMathces(NotAvailableOn, Enums.Availability.NotAvailable)
                Availability = Availability.Standard

                Revert(From item In StandardOn Where ReplacedByIsSameAsReplacement(item, replacement))

                If Not replacement Is Nothing Then
                    DirectCast(EquipmentItem, ModelGenerationGradeOption).ReplacedBy = replacement
                End If
            End Sub
            Private Sub MakeNotAvailable()
                MakeSureTheAvailabilityMathces(StandardOn, Enums.Availability.Standard)
                MakeSureTheAvailabilityMathces(OptionalOn, Enums.Availability.Optional)
                Availability = Availability.NotAvailable

                Revert(NotAvailableOn)

            End Sub
            Private Sub MakeOptional(ByVal replacement As ModelGenerationGradeOption)
                MakeSureTheAvailabilityMathces(StandardOn, Enums.Availability.Standard)
                MakeSureTheAvailabilityMathces(NotAvailableOn, Enums.Availability.NotAvailable)
                Availability = Availability.Optional

                'GET THE MOST FREQUENT PRICE 
                Dim maxItem As CarEquipmentItem = (From x In OptionalOn Group x By key = GetPriceHash(x) Into Group Order By Group.Count() Descending Select Group).First().First()
                Dim priceHash As String = GetPriceHash(maxItem)


                'OVERWRITE OF ALL THE OPTIONAL ITEMS WITH A DIFFERENT PRICE
                OverwriteToKeepOriginalValue((From item In OptionalOn Where Not GetPriceHash(item).Equals(priceHash) AndAlso Not ReplacedByIsSameAsReplacement(item, replacement)))

                'AND SET IT ON THE GRADE OBJECT
                SetPrices(maxItem)

                'REMOVE OWNERSHIP OF ALL THE ITEMS WITH THE SAME PRICE!
                Revert(From item In OptionalOn Where GetPriceHash(item).Equals(priceHash) AndAlso ReplacedByIsSameAsReplacement(item, replacement))

                If Not replacement Is Nothing Then
                    DirectCast(EquipmentItem, ModelGenerationGradeOption).ReplacedBy = replacement
                End If
            End Sub

            Private Shared Function GetPriceHash(ByVal item As CarEquipmentItem) As String
                Select Case item.Type
                    Case EquipmentType.Accessory
                        Return GetPriceHash(DirectCast(item, CarAccessory))
                    Case EquipmentType.Option
                        Return GetPriceHash(DirectCast(item, CarOption))
                    Case Else
                        Throw New Exceptions.DevelopperException(String.Format("GetPriceHash is not support for type {0}", item.Type))
                End Select
            End Function
            Private Shared Function GetPriceHash(ByVal accessory As CarAccessory) As String
                Return accessory.FittingPriceExistingCar.ToString() & "_" & _
                       accessory.FittingPriceNewCar.ToString() & "_" & _
                       accessory.FittingTimeExistingCar.ToString() & "_" & _
                       accessory.FittingTimeNewCar.ToString() & "_" & _
                       accessory.FittingVatPriceExistingCar.ToString() & "_" & _
                       accessory.FittingVatPriceNewCar.ToString()
            End Function
            Private Shared Function GetPriceHash(ByVal [option] As CarOption) As String
                Return [option].FittingPrice.ToString() & "-" & [option].FittingVatPrice.ToString()
            End Function

            Private Function ReplacedByIsSameAsReplacement(ByVal item As CarEquipmentItem, ByVal replacement As ModelGenerationGradeOption) As Boolean
                If Not SuffixOption Then Return True

                Dim replacedBy = DirectCast(item, CarOption).ReplacedBy
                If replacement Is Nothing AndAlso replacedBy Is Nothing Then Return True
                If replacement Is Nothing OrElse replacedBy Is Nothing Then Return False
                Return replacement.Equals(replacedBy.ID)
            End Function

            Private Sub SetPrices(ByVal item As CarEquipmentItem)
                Select Case item.Type
                    Case EquipmentType.Accessory
                        SetPrices(DirectCast(item, CarAccessory))
                    Case EquipmentType.Option
                        SetPrices(DirectCast(item, CarOption))
                End Select
            End Sub
            Private Sub SetPrices(ByVal accessory As CarAccessory)
                With DirectCast(EquipmentItem, ModelGenerationGradeAccessory)
                    .FittingPriceExistingCar = accessory.FittingPriceExistingCar
                    .FittingPriceNewCar = accessory.FittingPriceNewCar
                    .FittingTimeExistingCar = accessory.FittingTimeExistingCar
                    .FittingTimeNewCar = accessory.FittingTimeNewCar
                    .FittingVatPriceExistingCar = accessory.FittingVatPriceExistingCar
                    .FittingVatPriceNewCar = accessory.FittingVatPriceNewCar
                End With
            End Sub
            Private Sub SetPrices(ByVal [option] As CarOption)
                With DirectCast(EquipmentItem, ModelGenerationGradeOption)
                    .FittingPrice = [option].FittingPrice
                    .FittingVatPrice = [option].FittingVatPrice
                End With
            End Sub
#End Region

#Region " Constructors "
            Public Sub New(ByVal item As ModelGenerationGradeEquipmentItem)
                'Prevent direct creation
                _equipmentItem = item
                _suffixOption = (item.Type = EquipmentType.Option AndAlso DirectCast(item, ModelGenerationGradeOption).SuffixOption)
            End Sub
#End Region

        End Class

#End Region



    End Class
End Namespace
