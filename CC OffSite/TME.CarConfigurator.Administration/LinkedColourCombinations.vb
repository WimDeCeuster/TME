Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class LinkedColourCombinations
    Inherits BaseObjects.ContextListBase(Of LinkedColourCombinations, LinkedColourCombination)

#Region " Business Properties & Methods "

    <NotUndoable()> Private _generation As ModelGeneration
    <NotUndoable()> Private _partialCarSpecification As PartialCarSpecification
    <NotUndoable()> Private _exteriorColours As LinkedExteriorColours  'Undo not support for properties of collections
    <NotUndoable()> Private _upholsteries As LinkedUpholsteries       'Undo not support for properties of collections

    Friend Property Generation() As ModelGeneration
        Get
            Return _generation
        End Get
        Set(ByVal value As ModelGeneration)
            _generation = value
            If Not _exteriorColours Is Nothing Then _exteriorColours.Generation = value
            If Not _upholsteries Is Nothing Then _upholsteries.Generation = value
        End Set
    End Property
    Friend Property PartialCarSpecification() As PartialCarSpecification
        Get
            Return _partialCarSpecification
        End Get
        Private Set(ByVal value As PartialCarSpecification)
            _partialCarSpecification = value
        End Set
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal id As Guid) As LinkedColourCombination
        Get
            For Each _colourCombination As LinkedColourCombination In Me
                If _colourCombination.Equals(id) Then Return _colourCombination
            Next
            Return Nothing
        End Get
    End Property
    Default Public Overloads ReadOnly Property Item(ByVal exteriorColour As Guid, ByVal upholstery As Guid) As LinkedColourCombination
        Get
            For Each _colourCombination As LinkedColourCombination In Me
                If _colourCombination.ExteriorColour.Equals(exteriorColour) AndAlso _colourCombination.Upholstery.Equals(upholstery) Then Return _colourCombination
            Next
            Return Nothing
        End Get
    End Property

    Public Overloads Function Add(ByVal combination As ModelGenerationColourCombination) As LinkedColourCombination
        If Me.Contains(combination.ExteriorColour.ID, combination.Upholstery.ID) Then Throw New Exceptions.ObjectAlreadyExists("colour combination", combination)

        If Not _exteriorColours.Contains(combination.ExteriorColour.ID) Then _exteriorColours.Add(LinkedExteriorColour.NewLinkedExteriorColour(Me.PartialCarSpecification, combination.ExteriorColour))
        If Not _upholsteries.Contains(combination.Upholstery.ID) Then _upholsteries.Add(LinkedUpholstery.NewLinkedUpholstery(Me.PartialCarSpecification, combination.Upholstery))
        Return Me.Add(combination.ExteriorColour.ID, combination.Upholstery.ID)
    End Function
    Private Overloads Function Add(ByVal exteriorColourID As Guid, ByVal upholsteryID As Guid) As LinkedColourCombination
        Dim _colourCombination As LinkedColourCombination = LinkedColourCombination.NewLinkedColourCombination(Me.PartialCarSpecification, _exteriorColours.Item(exteriorColourID), _upholsteries.Item(upholsteryID))
        MyBase.Add(_colourCombination)
        Return _colourCombination
    End Function

    Public Overloads Sub Remove(ByVal exteriorColour As Guid, ByVal upholstery As Guid)
        Me.Remove(Me(exteriorColour, Upholstery))
    End Sub
    Public Overloads Sub Remove(ByVal id As Guid)
        Me.Remove(Me(id))
    End Sub


    Public Function ExteriorColours() As IEnumerable(Of LinkedExteriorColour)
        Return (From colourCombination In Me Select colourCombination.ExteriorColour Distinct)
    End Function
    Public Function Upholsteries() As IEnumerable(Of LinkedUpholstery)
        Return (From colourCombination In Me Select colourCombination.Upholstery Distinct)
    End Function

#End Region

#Region " Copy Method "
    Friend Function Copy(ByVal newCarSpecification As PartialCarSpecification) As LinkedColourCombinations
        Dim _copy As LinkedColourCombinations = Me.Clone()

        For Each _colourCombination As LinkedColourCombination In _copy
            If _colourCombination.AllowEdit Then
                _colourCombination.TakeOwnership(newCarSpecification)
            End If
        Next

        For Each _exteriorColour As LinkedExteriorColour In _copy.ExteriorColours
            If _exteriorColour.AllowEdit Then
                _exteriorColour.TakeOwnership(newCarSpecification)
            End If
        Next

        For Each _upholstery As LinkedUpholstery In _copy.Upholsteries
            If _upholstery.AllowEdit Then
                _upholstery.TakeOwnership(newCarSpecification)
            End If
        Next
        _copy.PartialCarSpecification = newCarSpecification

        Return _copy
    End Function
#End Region

#Region " Contains Methods "
    Public Overloads Function Contains(ByVal exteriorColour As Guid, ByVal upholstery As Guid) As Boolean
        Return Me.Any(Function(x) x.ExteriorColour.Equals(exteriorColour) AndAlso x.Upholstery.Equals(upholstery))
    End Function
    Public Overloads Function Contains(ByVal exteriorColour As Guid) As Boolean
        Return Me.Any(Function(x) x.ExteriorColour.Equals(exteriorColour))
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetLinkedColourCombinations(ByVal generation As ModelGeneration, ByVal partialCarSpecification As PartialCarSpecification) As LinkedColourCombinations
        Dim _colourCombinations As LinkedColourCombinations = DataPortal.Fetch(Of LinkedColourCombinations)(New CustomCriteria(partialCarSpecification))
        _colourCombinations.Generation = generation
        Return _colourCombinations
    End Function
    Friend Shared Function GetLinkedColourCombinations(ByVal car As Car) As LinkedColourCombinations
        Dim _colourCombinations As LinkedColourCombinations = DataPortal.Fetch(Of LinkedColourCombinations)(New CustomCriteria(car))
        _colourCombinations.Generation = car.Generation
        Return _colourCombinations
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
        Me.MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Public ReadOnly PartialCarSpecification As PartialCarSpecification

        Public Sub New(ByVal partialCarSpecification As PartialCarSpecification)
            Me.CommandText = "getLinkedColourCombinations"
            Me.PartialCarSpecification = partialCarSpecification
        End Sub
        Public Sub New(ByVal car As Car)
            Me.CommandText = "getCarColourCombinations"
            Me.PartialCarSpecification = car.PartialCarSpecification
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            Me.PartialCarSpecification.AppendParameters(command)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Protected Overrides Sub Fetch(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.RaiseListChangedEvents = Me.RaiseListChangedEventsDuringFetch
        With dataReader
            _exteriorColours = LinkedExteriorColours.GetLinkedExteriorColours(Me.PartialCarSpecification, dataReader)

            .NextResult()
            _upholsteries = LinkedUpholsteries.GetLinkedUpholsteries(Me.PartialCarSpecification, dataReader)

            .NextResult()
            While .Read
                MyBase.Add(LinkedColourCombination.GetLinkedColourCombination(dataReader, _exteriorColours(.GetGuid("EXTERIORCOLOURID")), _upholsteries(.GetGuid("UPHOLSTERYID"))))
            End While
        End With
        MyBase.RaiseListChangedEvents = True
    End Sub
    Protected Overrides Sub Fetch(ByVal dataReader As SafeDataReader, ByVal criteria As CommandCriteria)
        Me.PartialCarSpecification = DirectCast(criteria, CustomCriteria).PartialCarSpecification
        Me.Fetch(dataReader)
    End Sub

    Private Sub BeforeUpdate(ByVal transaction As System.Data.SqlClient.SqlTransaction) Handles Me.BeforeUpdateCommand
        _exteriorColours.Update(transaction)
        _upholsteries.Update(transaction)
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("colourcombination")> Public NotInheritable Class LinkedColourCombination
    Inherits ContextBusinessBase(Of LinkedColourCombination)


#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _partialCarSpecification As PartialCarSpecification
    Private _upholstery As LinkedUpholstery
    Private _exteriorColour As LinkedExteriorColour
    Private _status As Integer

    Private ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, LinkedColourCombinations).Generation
        End Get
    End Property


    <XmlInfo(XmlNodeType.Attribute)> Public Property ID() As Guid
        Get
            Return _id
        End Get
        Private Set(ByVal value As Guid)
            _id = value
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Set(ByVal value As Boolean)
            If value.Equals(Approved) Then Return

            If value Then
                _status += Status.ApprovedForLive
            Else
                _status -= Status.ApprovedForLive
            End If
            PropertyHasChanged("Approved")
            Generation.RaiseColourAvailabilityChanged()
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute), Obsolete("Please use Approved instead", False)> Public Property Declined() As Boolean
        Get
            Return Not Approved
        End Get
        Set(ByVal value As Boolean)
            Approved = False
        End Set
    End Property

    Public Property Upholstery() As LinkedUpholstery
        Get
            Return _upholstery
        End Get
        Private Set(ByVal value As LinkedUpholstery)
            _upholstery = value
        End Set
    End Property
    Public Property ExteriorColour() As LinkedExteriorColour
        Get
            Return _exteriorColour
        End Get
        Private Set(ByVal value As LinkedExteriorColour)
            _exteriorColour = value
        End Set
    End Property

    <XmlInfo(XmlNodeType.None)> Public Overloads Overrides Property AllowEdit() As Boolean
        Get
            Return HasOwnership()
        End Get
        Protected Set(ByVal value As Boolean)
            MyBase.AllowEdit = value
        End Set
    End Property
    Public Overloads Overrides Property AllowRemove() As Boolean
        Get
            Return HasOwnership()
        End Get
        Protected Set(ByVal value As Boolean)
            MyBase.AllowRemove = value
        End Set
    End Property

    Public Function HasOwnership() As Boolean
        Return Me.PartialCarSpecification.Equals(DirectCast(Me.Parent, LinkedColourCombinations).PartialCarSpecification)
    End Function
    Public Sub TakeOwnership()
        If Not Me.HasOwnership() Then
            TakeOwnership(DirectCast(Me.Parent, LinkedColourCombinations).PartialCarSpecification)
        End If
    End Sub
    Friend Sub TakeOwnership(ByVal newCarSpecification As PartialCarSpecification)
        Me.ID = Guid.NewGuid()
        Me.PartialCarSpecification = newCarSpecification
        Me.AllowEdit = True
        Me.AllowRemove = True
        MarkNew()
    End Sub

    Friend Property PartialCarSpecification() As PartialCarSpecification
        Get
            Return _partialCarSpecification
        End Get
        Private Set(ByVal value As PartialCarSpecification)
            _partialCarSpecification = value
        End Set
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.ExteriorColour.ToString() & " - " & Me.Upholstery.ToString()
    End Function

    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return obj.Equals(Me.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As LinkedColourCombination) As Boolean
        Return Not (obj Is Nothing) AndAlso obj.ID.Equals(Me.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is LinkedColourCombination Then
            Return Me.Equals(DirectCast(obj, LinkedColourCombination))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_exteriorColour Is Nothing) AndAlso Not _exteriorColour.IsValid Then Return False
            If Not (_upholstery Is Nothing) AndAlso Not _upholstery.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_exteriorColour Is Nothing) AndAlso _exteriorColour.IsDirty Then Return True
            If Not (_upholstery Is Nothing) AndAlso _upholstery.IsDirty Then Return True
            Return False
        End Get
    End Property

    Protected Overrides Function GetIdValue() As Object
        Return Me.ID
    End Function
    Public Shadows Sub Delete()
        If Me.AllowRemove Then
            DirectCast(Me.Parent, LinkedColourCombinations).Remove(Me)
        End If
    End Sub
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewLinkedColourCombination(ByVal partialCarSpecification As PartialCarSpecification, ByVal exteriorColour As LinkedExteriorColour, ByVal upholstery As LinkedUpholstery) As LinkedColourCombination
        Dim _colourCombination As LinkedColourCombination = New LinkedColourCombination()
        _colourCombination.Create()
        _colourCombination.ID = Guid.NewGuid()
        _colourCombination.ExteriorColour = exteriorColour
        _colourCombination.Upholstery = upholstery
        _colourCombination.PartialCarSpecification = partialCarSpecification
        Return _colourCombination
    End Function
    Friend Shared Function GetLinkedColourCombination(ByVal dataReader As SafeDataReader, ByVal exteriorColour As LinkedExteriorColour, ByVal upholstery As LinkedUpholstery) As LinkedColourCombination
        Dim _colourCombination As LinkedColourCombination = New LinkedColourCombination()
        _colourCombination.Fetch(dataReader)
        _colourCombination.ExteriorColour = exteriorColour
        _colourCombination.Upholstery = upholstery
        Return _colourCombination
    End Function

#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _id = dataReader.GetGuid("ID")
        _status = CType(dataReader.GetValue("STATUSID"), Integer)
        _partialCarSpecification = PartialCarSpecification.GetPartialCarSpecification(dataReader)
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", Me.ID)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@ID", Me.ID)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", Me.ExteriorColour.ID)
        command.Parameters.AddWithValue("@UPHOLSTERYID", Me.Upholstery.ID)
        command.Parameters.AddWithValue("@STATUSID", _status)
        Me.PartialCarSpecification.AppendParameters(command)
    End Sub

#End Region

End Class