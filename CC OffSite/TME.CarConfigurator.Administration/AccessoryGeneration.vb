Imports System.Collections.Generic
Imports System.ComponentModel

<Serializable()>
Public Class AccessoryGenerations
    Inherits ContextUniqueGuidListBase(Of AccessoryGenerations, AccessoryGeneration)

#Region "Business Properties & Methods"

    Private WithEvents _fittings As AccessoryFittings

    Public ReadOnly Property Accessory() As Accessory
        Get
            Return DirectCast(Parent, Accessory)
        End Get
    End Property


    Private Sub InitializeWith(ByVal accessoryFittings As AccessoryFittings)
        _fittings = accessoryFittings
        SynchronizeGenerations()
    End Sub

    Private Sub SynchronizeGenerations()
        Dim newGenerations = _fittings.Select(Function(fitting) DirectCast(fitting, AccessoryFitting).FactoryGeneration.ID) _
                                  .Where(Function(id) Not id.Equals(Guid.Empty)) _
                                  .SelectMany(Function(factoryGenerationID) ModelGenerationQueryResults.FindByFactoryGenerationID(factoryGenerationID)) _
                                  .Select(Function(modelGenerationQueryResult) AccessoryGeneration.NewAccessoryGeneration(modelGenerationQueryResult.ID)) _
                                  .Distinct() _
                                  .ToList()

        Dim generationsThatWereRemoved = Where(NotIn(newGenerations))
        Dim generationsThatWereAdded = newGenerations.Where(NotIn(Me))

        RemoveRange(generationsThatWereRemoved)
        AddRange(generationsThatWereAdded)
    End Sub

    Private Shared Function NotIn(ByVal otherGenerations As IEnumerable(Of AccessoryGeneration)) As Func(Of AccessoryGeneration, Boolean)
        Return Function(generation) Not otherGenerations.Any(Function(newGeneration) newGeneration.ID.Equals(generation.ID))
    End Function

    Private Sub AddRange(ByVal generations As IEnumerable(Of AccessoryGeneration))
        For Each accessoryGeneration In generations
            Add(accessoryGeneration)
        Next
    End Sub

    Private Sub OnFittingsListChanged(sender As Object, e As ListChangedEventArgs) Handles _fittings.ListChanged
        If e.ListChangedType <> ListChangedType.ItemAdded AndAlso e.ListChangedType <> ListChangedType.ItemDeleted Then Return

        SynchronizeGenerations()
    End Sub

    Private Sub OnFittingFactoryGenerationIdChanged(accessoryFitting As AccessoryFitting, oldValueOfFactoryGeneration As FactoryGenerationInfo, newValueOfFactoryGeneration As FactoryGenerationInfo) Handles _fittings.FittingFactoryGenerationChanged
        SynchronizeGenerations()
    End Sub

#End Region

#Region "Shared Factory Methods"
    Public Shared Function NewAccessoryGenerations(ByVal accessoryFittings As AccessoryFittings) As AccessoryGenerations
        Dim generations = New AccessoryGenerations()
        generations.SetParent(accessoryFittings.EquipmentItem)
        generations.InitializeWith(accessoryFittings)
        Return generations
    End Function
#End Region

#Region "Constructors"
    Private Sub New()
        MarkAsChild()
    End Sub
#End Region
End Class

<Serializable()>
Public Class AccessoryGeneration
    Inherits ContextUniqueGuidBusinessBase(Of AccessoryGeneration)

#Region "Business Properties & Methods"

    Private _interiorColourApplicabilities As InteriorColourApplicabilities

    Private ReadOnly Property Accessory() As Accessory
        Get
            Return DirectCast(Parent, AccessoryGenerations).Accessory
        End Get
    End Property

    Public ReadOnly Property InteriorColourApplicabilities() As InteriorColourApplicabilities
        Get
            _interiorColourApplicabilities = If(_interiorColourApplicabilities, InteriorColourApplicabilities.GetInteriorColourApplicabilities(Me))
            Return _interiorColourApplicabilities
        End Get
    End Property

    Protected Friend Overrides Function GetObjectID() As Guid
        Return Accessory.GetObjectID() 'this function is used for inserting interior colour applicabilities of the generation => we want it to use the accessory's id as equipmentid
    End Function

#End Region

#Region "Shared Factory Methods"
    Friend Shared Function NewAccessoryGeneration(ByVal generationID As Guid) As AccessoryGeneration
        Dim generation = New AccessoryGeneration()
        generation.Create(generationID)
        Return generation
    End Function

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If _interiorColourApplicabilities IsNot Nothing AndAlso Not _interiorColourApplicabilities.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If _interiorColourApplicabilities IsNot Nothing AndAlso _interiorColourApplicabilities.IsDirty Then Return True
            Return False
        End Get
    End Property


#End Region

#Region "Constructors"
    Private Sub New()
        MarkAsChild()
        MarkOld() 'do not ever persist this object to the database
    End Sub
#End Region

#Region "Data Access"
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)

        If _interiorColourApplicabilities IsNot Nothing Then _interiorColourApplicabilities.Update(transaction)
    End Sub
#End Region
End Class