<Serializable()> Public Class CarUpholsteryApplicabilities
    Inherits ContextUniqueGuidListBase(Of CarUpholsteryApplicabilities, CarUpholsteryApplicability)
    Implements IUpholsteryApplicabilities

#Region "Business Properties and methods"

    Private _inheritedUpholsteryApplicabilities As UpholsteryApplicabilities

    Public Shadows Property Parent() As IUniqueGuid Implements IUpholsteryApplicabilities.Parent
        Get
            Return DirectCast(MyBase.Parent, IUniqueGuid)
        End Get
        Friend Set(ByVal value As IUniqueGuid)
            SetParent(value)
        End Set
    End Property

    Public Shadows ReadOnly Property Count() As Integer Implements IUpholsteryApplicabilities.Count
        Get
            Return MyBase.Count
        End Get
    End Property

    Friend Property InheritedUpholsteryApplicabilities As UpholsteryApplicabilities
        Get
            Return _inheritedUpholsteryApplicabilities
        End Get
        Set(ByVal value As UpholsteryApplicabilities)
            _inheritedUpholsteryApplicabilities = value
            With _inheritedUpholsteryApplicabilities
                AddHandler .ApplicabilityAdded, AddressOf OnModelGenerationApplicabilityAdded
                AddHandler .ApplicabilityRemoved, AddressOf OnModelGenerationApplicabilityRemoved
            End With
            SyncUpholsteryApplicabilities()
        End Set
    End Property

    Private Sub SyncUpholsteryApplicabilities()
        For Each applicability In InheritedUpholsteryApplicabilities
            SyncUpholsteryApplicability(applicability)
        Next
    End Sub

    Private Sub SyncUpholsteryApplicability(ByVal upholsteryApplicability As UpholsteryApplicability)
        Dim applicability = Item(upholsteryApplicability.ID)

        If applicability Is Nothing Then
            'if this applicability doesn't exist on car level yet, add it there, so that it can be overwritten
            MyBase.Add(CarUpholsteryApplicability.GetCarUpholsteryApplicability(upholsteryApplicability))
        ElseIf Not applicability.Overwritten Then
            'otherwise, if the applicability is already on car level and it isn't overwritten, revert it (it is now defined on higher level)
            Item(upholsteryApplicability.ID).MarkInherited()
        End If
    End Sub

    Friend Shadows Function Add(ByVal dataReader As SafeDataReader) As CarUpholsteryApplicability
        Dim applicability = CarUpholsteryApplicability.GetCarUpholsteryApplicability(dataReader)
        MyBase.Add(applicability)
        Return applicability
    End Function

    Public Shadows Function Add(ByVal upholsteryInfo As UpholsteryInfo) As IApplicability Implements IUpholsteryApplicabilities.Add
        Dim applicability = CarUpholsteryApplicability.NewApplicability(upholsteryInfo)
        MyBase.Add(applicability)
        Return applicability
    End Function

    Public Function GetItem(ByVal id As Guid) As IApplicability Implements IUpholsteryApplicabilities.GetItem
        Return Item(id)
    End Function


    Public Overloads Function Contains(ByVal id As Guid) As Boolean Implements IUpholsteryApplicabilities.Contains
        Return MyBase.Contains(id)
    End Function
    Public Shadows Sub Remove(ByVal id As Guid) Implements IUpholsteryApplicabilities.Remove
        Remove(Item(id))
    End Sub
    Private Shadows Sub Remove(ByVal applicability As CarUpholsteryApplicability)
        If applicability.Inherited Then
            If Not applicability.Overwritten Then applicability.Overwrite()
            applicability.Cleared = True
        Else
            MyBase.Remove(applicability)
        End If
    End Sub

    Public Shadows Sub Clear() Implements IUpholsteryApplicabilities.Clear
        For index = (Count - 1) To 0 Step -1
            RemoveAt(index)
        Next
    End Sub
    Private Shadows Sub RemoveAt(ByVal index As Integer)
        Dim applicability = Item(index)
        Remove(applicability)
    End Sub


#End Region

#Region "Event handlers"
    Private Sub OnModelGenerationApplicabilityAdded(ByVal upholsteryApplicability As UpholsteryApplicability)
        SyncUpholsteryApplicability(upholsteryApplicability)
    End Sub

    Private Sub OnModelGenerationApplicabilityRemoved(ByVal upholsteryApplicability As UpholsteryApplicability)
        If Not Contains(upholsteryApplicability.ID) OrElse Not Item(upholsteryApplicability.ID).Inherited Then Return

        'make sure the applicability can be removed now
        Item(upholsteryApplicability.ID).MarkForRemoval()
        'remove it
        MyBase.Remove(upholsteryApplicability.ID)
    End Sub
#End Region

#Region "Shared Factory Methods"

    Friend Shared Function NewApplicabilities(ByVal carEquipmentItem As CarEquipmentItem) As CarUpholsteryApplicabilities
        Dim applicabilities = New CarUpholsteryApplicabilities()
        applicabilities.SetParent(carEquipmentItem)
        Return applicabilities
    End Function
    Friend Shared Function NewApplicabilities(ByVal carPack As CarPack) As CarUpholsteryApplicabilities
        Dim applicabilities = New CarUpholsteryApplicabilities()
        applicabilities.SetParent(carPack)
        Return applicabilities
    End Function

#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region

End Class

<Serializable()> Public Class CarUpholsteryApplicability
    Inherits UpholsteryApplicability
    Implements IUpholsteryApplicability

#Region "Business Properties and methods"

    Private _inherited As Boolean?
    Private _overwritten As Boolean
    Private _cleared As Boolean

    Public ReadOnly Property Inherited As Boolean 'true if it was defined on a higher level, false if it is added here
        Get
            If _inherited IsNot Nothing Then Return CType(_inherited, Boolean)

            _inherited = DirectCast(Parent, CarUpholsteryApplicabilities).InheritedUpholsteryApplicabilities.Contains(ID)

            Return CType(_inherited, Boolean)
        End Get
    End Property

    Public Property Overwritten As Boolean
        Get
            Return _overwritten AndAlso Inherited
        End Get
        Private Set(ByVal value As Boolean)
            If _overwritten = value Then Return
            _overwritten = value
            AllowEdit = True 'set true so that it can be changed internally
            PropertyHasChanged("Overwritten")
            AllowEdit = _overwritten 'keep underneath property changed! otherwise, it breaks!

        End Set
    End Property

    Public Property Cleared As Boolean
        Get
            Return _cleared
        End Get
        Set(ByVal value As Boolean)
            If _cleared = value OrElse Not Overwritten Then Return
            _cleared = value
            PropertyHasChanged("Cleared")
        End Set
    End Property

    Public Sub Overwrite()
        If Inherited Then Overwritten = True
    End Sub

    Public Sub Revert()
        If Not Overwritten Then Exit Sub

        Cleared = False 'first cleared false, then overwritten false => otherwise, cleared cannot be changed
        Overwritten = False
    End Sub

    Friend Sub MarkInherited()
        Cleared = False 'first cleared false, then overwritten false => otherwise, cleared cannot be changed
        Overwritten = False
        AllowRemove = False
    End Sub

    Friend Sub MarkForRemoval()
        AllowRemove = True
    End Sub
#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
        AllowEdit = False
    End Sub
#End Region

#Region "Shared Factory Methods"
    Friend Shared Function GetCarUpholsteryApplicability(ByVal dataReader As SafeDataReader) As CarUpholsteryApplicability
        Dim applicability = New CarUpholsteryApplicability()
        applicability.Fetch(dataReader)
        applicability.AllowEdit = True
        Return applicability
    End Function
    Public Shared Function GetCarUpholsteryApplicability(ByVal upholsteryApplicability As UpholsteryApplicability) As CarUpholsteryApplicability
        Dim applicability = New CarUpholsteryApplicability()
        applicability.Fetch(upholsteryApplicability)
        applicability.MarkInherited()
        Return applicability
    End Function

    Friend Shared Function NewApplicability(ByVal upholsteryInfo As UpholsteryInfo) As CarUpholsteryApplicability
        Dim applicability = New CarUpholsteryApplicability()
        applicability.Create(upholsteryInfo)
        applicability.AllowEdit = True
        Return applicability
    End Function
#End Region

#Region "Data Access"
    Private Overloads Sub Fetch(ByVal upholsteryApplicability As UpholsteryApplicability)
        ID = upholsteryApplicability.ID
        Code = upholsteryApplicability.Code
        Name = upholsteryApplicability.Name
        MarkOld()
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        _cleared = dataReader.GetBoolean(GetFieldName("CLEARED"))
        _overwritten = True
    End Sub


    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command, "insert")
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        If Not Overwritten AndAlso Inherited Then 'if it is inherited and not overwritten, just delete it
            AddDeleteCommandFields(command)
        Else 'all other cases => update it in DB
            AddCommandFields(command, "insert")
        End If
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command, "delete")
    End Sub

    Private Sub AddCommandFields(ByVal command As SqlCommand, ByVal action As String)
        Dim listParent = DirectCast(Parent, CarUpholsteryApplicabilities).Parent

        If TypeOf listParent Is CarEquipmentItem Then
            command.CommandText = String.Format("{0}CarEquipmentUpholsteryApplicability", action)
            command.Parameters.AddWithValue("@CARID", DirectCast(listParent, CarEquipmentItem).Car.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", listParent.GetObjectID())
        ElseIf TypeOf listParent Is CarPack Then
            command.CommandText = String.Format("{0}CarPackUpholsteryApplicability", action)
            command.Parameters.AddWithValue("@CARID", DirectCast(listParent, CarPack).Car.ID)
            command.Parameters.AddWithValue("@PACKID", listParent.GetObjectID())
        Else
            Throw New ApplicationException("AddCommandFields not support for object of type " & listParent.GetType().Name)
        End If

        If action = "insert" Then command.Parameters.AddWithValue("@CLEARED", Cleared)

    End Sub
#End Region

End Class