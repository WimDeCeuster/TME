<Serializable()> Public Class CarExteriorColourApplicabilities
    Inherits ContextUniqueGuidListBase(Of CarExteriorColourApplicabilities, CarExteriorColourApplicability)
    Implements IExteriorColourApplicabilities

#Region "Business properties and methods"
    Private _inheritedExteriorColourApplicabilities As ExteriorColourApplicabilities

    Public Shadows Property Parent() As IUniqueGuid Implements IApplicabilities.Parent
        Get
            Return DirectCast(MyBase.Parent, IUniqueGuid)
        End Get
        Friend Set(ByVal value As IUniqueGuid)
            SetParent(value)
        End Set
    End Property

    Public Overloads ReadOnly Property Count() As Integer Implements IExteriorColourApplicabilities.Count
        Get
            Return MyBase.Count
        End Get
    End Property
    Friend Property InheritedExteriorColourApplicabilities As ExteriorColourApplicabilities
        Get
            Return _inheritedExteriorColourApplicabilities
        End Get
        Set(ByVal value As ExteriorColourApplicabilities)
            _inheritedExteriorColourApplicabilities = value
            With _inheritedExteriorColourApplicabilities
                AddHandler .ApplicabilityAdded, AddressOf OnModelGenerationApplicabilityAdded
                AddHandler .ApplicabilityRemoved, AddressOf OnModelGenerationApplicabilityRemoved
            End With
            SyncExteriorColourApplicabilities()
        End Set
    End Property

    Private Sub SyncExteriorColourApplicabilities()
        For Each applicability In InheritedExteriorColourApplicabilities
            SyncExteriorColourApplicability(applicability)
        Next
    End Sub

    Private Sub SyncExteriorColourApplicability(ByVal exteriorColourApplicability As ExteriorColourApplicability)
        Dim applicability = Item(exteriorColourApplicability.ID)

        If applicability Is Nothing Then
            'if this applicability doesn't exist on car level yet, add it there, so that it can be overwritten
            MyBase.Add(CarExteriorColourApplicability.GetApplicability(exteriorColourApplicability))
        ElseIf Not applicability.Overwritten Then
            'otherwise, if the applicability is already on car level and it isn't overwritten, revert it (it is now defined on higher level)
            applicability.MarkInherited()
        End If

    End Sub


    Public Shadows Function Add(exteriorColourInfo As ExteriorColourInfo) As IApplicability Implements IExteriorColourApplicabilities.Add
        Dim applicability = CarExteriorColourApplicability.NewApplicability(exteriorColourInfo)
        MyBase.Add(applicability)
        Return applicability
    End Function
    Friend Shadows Function Add(ByVal dataReader As SafeDataReader) As CarExteriorColourApplicability
        Dim applicability = CarExteriorColourApplicability.GetApplicability(dataReader)
        MyBase.Add(applicability)
        Return applicability
    End Function

    Public Function GetItem(ByVal id As Guid) As IApplicability Implements IApplicabilities.GetItem
        Return Item(id)
    End Function
    Public Overloads Function Contains(ByVal id As Guid) As Boolean Implements IApplicabilities.Contains
        Return MyBase.Contains(id)
    End Function

    Public Shadows Sub Remove(ByVal id As Guid) Implements IExteriorColourApplicabilities.Remove
        Remove(Me(id))
    End Sub
    Private Shadows Sub Remove(ByVal applicability As CarExteriorColourApplicability)
        If applicability.Inherited Then
            If Not applicability.Overwritten Then applicability.Overwrite()
            applicability.Cleared = True
        Else
            applicability.MakeRemovable()
            MyBase.Remove(applicability)
        End If
    End Sub

    Public Shadows Sub Clear() Implements IExteriorColourApplicabilities.Clear
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

    Private Sub OnModelGenerationApplicabilityAdded(ByVal exteriorColourApplicability As ExteriorColourApplicability)
        SyncExteriorColourApplicability(exteriorColourApplicability)
    End Sub

    Private Sub OnModelGenerationApplicabilityRemoved(ByVal exteriorcolourapplicability As ExteriorColourApplicability)
        If Not Contains(exteriorcolourapplicability.ID) OrElse Not Item(exteriorcolourapplicability.ID).Inherited Then Return

        'make sure the applicability can be removed now
        Item(exteriorcolourapplicability.ID).MakeRemovable()
        'remove it
        MyBase.Remove(exteriorcolourapplicability.ID)
    End Sub
#End Region

#Region "Shared Factory Methods"

    Friend Shared Function NewApplicabilities(ByVal carEquipmentItem As CarEquipmentItem) As CarExteriorColourApplicabilities
        Dim applicabilities = New CarExteriorColourApplicabilities()
        applicabilities.SetParent(carEquipmentItem)
        Return applicabilities
    End Function
    Friend Shared Function NewApplicabilities(ByVal carPack As CarPack) As CarExteriorColourApplicabilities
        Dim applicabilities = New CarExteriorColourApplicabilities()
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
<Serializable()> Public Class CarExteriorColourApplicability
    Inherits ExteriorColourApplicability
    Implements IExteriorColourApplicability

#Region "Business properties and methods"
    Private _overwritten As Boolean
    Private _cleared As Boolean
    Private _inherited As Boolean?

    Public ReadOnly Property Inherited As Boolean 'true if it was defined on a higher level, false if it is added here
        Get
            If _inherited IsNot Nothing Then Return CType(_inherited, Boolean)

            _inherited = DirectCast(Parent, CarExteriorColourApplicabilities).InheritedExteriorColourApplicabilities.Contains(ID)

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
            AllowEdit = _overwritten 'keep underneath property changed statement! otherwise, it breaks!

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

    Public Sub MakeRemovable()
        AllowRemove = True
    End Sub

#End Region

#Region "System.Object overrides"

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is CarExteriorColourApplicability Then Return Equals(DirectCast(obj, CarExteriorColourApplicability))

        Return MyBase.Equals(obj)
    End Function
#End Region

#Region "Shared Factory Methods"
    Friend Shared Function GetApplicability(ByVal dataReader As SafeDataReader) As CarExteriorColourApplicability
        Dim applicability = New CarExteriorColourApplicability()
        applicability.Fetch(dataReader)
        applicability.AllowEdit = True
        Return applicability
    End Function

    Public Shared Function GetApplicability(ByVal exteriorColourApplicability As ExteriorColourApplicability) As CarExteriorColourApplicability
        Dim applicability = New CarExteriorColourApplicability()
        applicability.Fetch(exteriorColourApplicability)
        applicability.MarkInherited()
        Return applicability
    End Function

    Friend Shared Function NewApplicability(ByVal exteriorColourInfo As ExteriorColourInfo) As CarExteriorColourApplicability
        Dim applicability = New CarExteriorColourApplicability()
        applicability.Create(exteriorColourInfo)
        applicability.AllowEdit = True
        Return applicability
    End Function
#End Region

#Region "Constructors"

    Private Sub New()
        AllowEdit = False
    End Sub

#End Region

#Region "Data Access"
    Private Overloads Sub Fetch(ByVal exteriorColourApplicability As ExteriorColourApplicability)
        ID = exteriorColourApplicability.ID
        Code = exteriorColourApplicability.Code
        Name = exteriorColourApplicability.Name
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
        Dim listParent = DirectCast(Parent, CarExteriorColourApplicabilities).Parent

        If TypeOf listParent Is CarEquipmentItem Then
            command.CommandText = String.Format("{0}CarEquipmentExteriorColourApplicability", action)
            command.Parameters.AddWithValue("@CARID", DirectCast(listParent, CarEquipmentItem).Car.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", listParent.GetObjectID())
        ElseIf TypeOf listParent Is CarPack Then
            command.CommandText = String.Format("{0}CarPackExteriorColourApplicability", action)
            command.Parameters.AddWithValue("@CARID", DirectCast(listParent, CarPack).Car.ID)
            command.Parameters.AddWithValue("@PACKID", listParent.GetObjectID())
        Else
            Throw New ApplicationException("AddCommandFields not support for object of type " & listParent.GetType().Name)
        End If

        If action = "insert" Then command.Parameters.AddWithValue("@CLEARED", Cleared)

    End Sub
#End Region

End Class