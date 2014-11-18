Imports TME.CarConfigurator.Administration.Enums

<Serializable> Public NotInheritable Class InteriorColourApplicabilities
    Inherits ContextUniqueGuidListBase(Of InteriorColourApplicabilities, InteriorColourApplicability)
    Implements IInteriorColourApplicabilities

#Region "Delegates & Events"
    Friend Delegate Sub ApplicabilitiesChangedHandler(ByVal interiorColourApplicability As IInteriorColourApplicability)
    Friend Event ApplicabilityAdded As ApplicabilitiesChangedHandler
    Friend Event ApplicabilityRemoved As ApplicabilitiesChangedHandler

    Protected Overrides Sub OnListChanged(ByVal e As ComponentModel.ListChangedEventArgs)
        MyBase.OnListChanged(e)
        If e.ListChangedType <> ComponentModel.ListChangedType.ItemAdded Then Return

        Dim interiorColourApplicability = Me(e.NewIndex)
        RaiseEvent ApplicabilityAdded(interiorColourApplicability)
    End Sub

    Private Overloads Sub OnRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        Dim interiorColourApplicability = DirectCast(e.RemovingItem, InteriorColourApplicability)
        RaiseEvent ApplicabilityRemoved(interiorColourApplicability)
    End Sub
#End Region

#Region "Business Properties & Methods"

    Public Function GetItem(ByVal id As Guid) As IApplicability Implements IApplicabilities.GetItem
        Return Item(id)
    End Function

    Public Shadows Property Parent() As IUniqueGuid Implements IApplicabilities.Parent
        Get
            Return DirectCast(MyBase.Parent, IUniqueGuid)
        End Get
        Friend Set(value As IUniqueGuid)
            SetParent(value)
        End Set
    End Property

    Public Shadows ReadOnly Property Count() As Integer Implements IApplicabilities.Count
        Get
            Return MyBase.Count
        End Get
    End Property

    Public Shadows Function Contains(ByVal id As Guid) As Boolean Implements IApplicabilities.Contains
        Return MyBase.Contains(id)
    End Function

    Public Overloads Sub Remove(ByVal id As Guid) Implements IApplicabilities.Remove
        MyBase.Remove(id)
    End Sub

    Public Overloads Sub Clear() Implements IApplicabilities.Clear
        MyBase.Clear()
    End Sub

    Public Overloads Function Add(ByVal interiorColour As InteriorColourInfo) As IApplicability Implements IInteriorColourApplicabilities.Add
        If Any(Function(a) a.Equals(interiorColour)) Then Throw New Exceptions.ObjectAlreadyExists(Entity.INTERIORCOLOUR)

        Dim applicability = InteriorColourApplicability.NewInteriorColourApplicability(interiorColour)
        Add(applicability)
        Return applicability
    End Function

#End Region

#Region "Shared Factory Methods"
    Friend Shared Function NewInteriorColourApplicabilities(ByVal item As Accessory) As InteriorColourApplicabilities
        Dim applicabilities = New InteriorColourApplicabilities()
        applicabilities.Parent = item
        Return applicabilities
    End Function

    Friend Shared Function GetInteriorColourApplicabilities(ByVal item As AccessoryGeneration) As InteriorColourApplicabilities
        Dim applicabilities = DataPortal.Fetch(Of InteriorColourApplicabilities)(New AccessoryGenerationCriteria(item))
        applicabilities.Parent = item
        Return applicabilities
    End Function

    Friend Shared Function GetInteriorColourApplicabilities(ByVal item As Accessory) As InteriorColourApplicabilities
        Dim applicabilities = DataPortal.Fetch(Of InteriorColourApplicabilities)(New CustomCriteria(item))
        applicabilities.Parent = item
        Return applicabilities
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        MyBase.New()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _equipmentID As Guid = Guid.Empty

        Public Sub New(ByVal item As Accessory)
            _equipmentID = item.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@EQUIPMENTID", _equipmentID)
        End Sub

    End Class

    <Serializable()> Private Class AccessoryGenerationCriteria
        Inherits CommandCriteria
        Private ReadOnly _accessoryGeneration As AccessoryGeneration

        Public Sub New(ByVal accessoryGeneration As AccessoryGeneration)
            _accessoryGeneration = accessoryGeneration
        End Sub
        
Public Overrides Sub AddCommandFields(ByVal command As SqlCommand)
            command.CommandText = "getAccessoryGenerationInteriorColourApplicabilities"
            command.Parameters.AddWithValue("@ACCESSORYID", _accessoryGeneration.GetObjectID())
            command.Parameters.AddWithValue("@GENERATIONID", _accessoryGeneration.ID)
        End Sub
    End Class
#End Region
End Class

<Serializable> Public Class InteriorColourApplicability
    Inherits ContextUniqueGuidBusinessBase(Of InteriorColourApplicability)
    Implements IInteriorColourApplicability
#Region "Business Properties & Methods"
    Private _code As String = String.Empty
    Private _name As String = String.Empty

    Public Property Code() As String Implements IApplicability.Code
        Get
            Return _code
        End Get
        Protected Set(ByVal value As String)
            _code = value
        End Set
    End Property

    Public Property Name() As String Implements IApplicability.Name
        Get
            Return _name
        End Get
        Protected Set(ByVal value As String)
            _name = value
        End Set
    End Property
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As InteriorColourApplicability) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As InteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As InteriorColourInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is InteriorColourApplicability Then
            Return Equals(DirectCast(obj, InteriorColourApplicability))
        ElseIf TypeOf obj Is InteriorColour Then
            Return Equals(DirectCast(obj, InteriorColour))
        ElseIf TypeOf obj Is InteriorColourInfo Then
            Return Equals(DirectCast(obj, InteriorColourInfo))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region "Shared Factory Methods"
    Friend Shared Function NewInteriorColourApplicability(ByVal interiorColour As InteriorColourInfo) As InteriorColourApplicability
        Dim applicability = New InteriorColourApplicability()
        applicability.Create(interiorColour)
        Return applicability
    End Function
#End Region

#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation
        MarkAsChild()
        AllowEdit = False
        FieldPrefix = "INTERIORCOLOUR"
    End Sub
#End Region

#Region "Data Access"
    Protected Overloads Sub Create(ByVal interiorColour As InteriorColourInfo)
        Create(interiorColour.ID)
        _code = interiorColour.Code
        _name = interiorColour.Name
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        _code = dataReader.GetString(GetFieldName("CODE"))
        _name = dataReader.GetString(GetFieldName("NAME"))
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("INTERIORCOLOURID", ID)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@INTERIORCOLOURID", ID)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@INTERIORCOLOURID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommmandFields(command, "insert")
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        AddCommmandFields(command, "delete")
    End Sub

    Private Sub AddCommmandFields(ByVal command As SqlCommand, ByVal action As String)
        Dim listParent As IUniqueGuid = DirectCast(Parent, InteriorColourApplicabilities).Parent

        Dim parentAsModelGenerationEquipmentItem = TryCast(listParent, ModelGenerationEquipmentItem)
        Dim parentAsAccessoryGeneration = TryCast(listParent, AccessoryGeneration)

        If parentAsModelGenerationEquipmentItem IsNot Nothing OrElse parentAsAccessoryGeneration IsNot Nothing Then
            command.CommandText = String.Format("{0}GenerationInteriorColourApplicability", action)
        Else
            command.CommandText = String.Format("{0}InteriorColourApplicability", action)
        End If

        If parentAsModelGenerationEquipmentItem IsNot Nothing Then
            command.Parameters.AddWithValue("@GENERATIONID", DirectCast(listParent, ModelGenerationEquipmentItem).Generation.ID)
        ElseIf parentAsAccessoryGeneration IsNot Nothing Then
            command.Parameters.AddWithValue("@GENERATIONID", DirectCast(listParent, AccessoryGeneration).ID)
        End If
        
        command.Parameters.AddWithValue("@EQUIPMENTID", listParent.GetObjectID())
    End Sub
#End Region
End Class