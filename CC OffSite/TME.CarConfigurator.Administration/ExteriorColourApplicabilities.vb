Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ExteriorColourApplicabilities
    Inherits ContextUniqueGuidListBase(Of ExteriorColourApplicabilities, ExteriorColourApplicability)
    Implements IExteriorColourApplicabilities

#Region " Delegates & Events "

    Friend Delegate Sub ApplicabilitiesChangedHandler(ByVal exteriorColourApplicability As ExteriorColourApplicability)
    Friend Event ApplicabilityAdded As ApplicabilitiesChangedHandler
    Friend Event ApplicabilityRemoved As ApplicabilitiesChangedHandler

    Protected Overrides Sub OnListChanged(ByVal e As ComponentModel.ListChangedEventArgs)
        MyBase.OnListChanged(e)
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
            Dim exteriorColourApplicability = Me(e.NewIndex)
            RaiseEvent ApplicabilityAdded(exteriorColourApplicability)
        End If
    End Sub

    Private Overloads Sub OnRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        Dim exteriorColourApplicability = DirectCast(e.RemovingItem, ExteriorColourApplicability)
        RaiseEvent ApplicabilityRemoved(exteriorColourApplicability)
    End Sub

#End Region

#Region " Business Properties & Methods "

    Public Function GetItem(ByVal id As Guid) As IApplicability Implements IExteriorColourApplicabilities.GetItem
        Return Item(id)
    End Function

    Public Overloads Function Add(ByVal exteriorColour As ExteriorColourInfo) As IApplicability Implements IExteriorColourApplicabilities.Add
        If Any(Function(x) x.Equals(exteriorColour)) Then Throw New Exceptions.ObjectAlreadyExists(Entity.EXTERIORCOLOUR)

        Dim applicability As ExteriorColourApplicability = ExteriorColourApplicability.NewExteriorColourApplicability(exteriorColour)
        Add(applicability)
        Return applicability
    End Function
    Friend Overloads Function Add(ByVal dataReader As SafeDataReader) As ExteriorColourApplicability
        Dim applicability As ExteriorColourApplicability = GetObject(dataReader)
        RaiseListChangedEvents = False
        Add(applicability)
        RaiseListChangedEvents = True
        Return applicability
    End Function

    Friend Overloads Sub Remove(ByVal id As Guid) Implements IExteriorColourApplicabilities.Remove
        MyBase.Remove(id)
    End Sub

    Public Overloads Function Contains(ByVal id As Guid) As Boolean Implements IExteriorColourApplicabilities.Contains
        Return MyBase.Contains(id)
    End Function

    Public Overloads Sub Clear() Implements IExteriorColourApplicabilities.Clear
        MyBase.Clear()
    End Sub

    Public Shadows Property Parent() As IUniqueGuid Implements IExteriorColourApplicabilities.Parent
        Get
            Return DirectCast(MyBase.Parent, IUniqueGuid)
        End Get
        Friend Set(ByVal value As IUniqueGuid)
            SetParent(value)
        End Set
    End Property

    Public Shadows ReadOnly Property Count() As Integer Implements IExteriorColourApplicabilities.Count
        Get
            Return MyBase.Count
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewExteriorColourApplicabilities(ByVal item As Accessory) As ExteriorColourApplicabilities
        Dim applicabilities As ExteriorColourApplicabilities = New ExteriorColourApplicabilities()
        applicabilities.Parent = item
        Return applicabilities
    End Function
    Friend Shared Function NewExteriorColourApplicabilities(ByVal item As ModelGenerationOption) As ExteriorColourApplicabilities
        Dim applicabilities As ExteriorColourApplicabilities = New ExteriorColourApplicabilities()
        applicabilities.Parent = item
        Return applicabilities
    End Function

    Friend Shared Function NewExteriorColourApplicabilities(ByVal item As ModelGenerationPack) As ExteriorColourApplicabilities
        Dim applicabilities As ExteriorColourApplicabilities = New ExteriorColourApplicabilities()
        applicabilities.Parent = item
        Return applicabilities
    End Function


    Friend Shared Function GetExteriorColourApplicabilities(ByVal item As Accessory) As ExteriorColourApplicabilities
        Dim applicabilities As ExteriorColourApplicabilities = DataPortal.Fetch(Of ExteriorColourApplicabilities)(New AccessoryCriteria(item))
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
    <Serializable()> Private Class AccessoryCriteria
        Inherits CommandCriteria

        Private ReadOnly _equipmentID As Guid = Guid.Empty

        Public Sub New(ByVal item As Accessory)
            _equipmentID = item.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@EQUIPMENTID", _equipmentID)
        End Sub

    End Class
#End Region
End Class

<Serializable()> Public Class ExteriorColourApplicability
    Inherits ContextUniqueGuidBusinessBase(Of ExteriorColourApplicability)
    Implements IExteriorColourApplicability

#Region " Business Properties & Methods "
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

    Public Function GetInfo() As ExteriorColourInfo
        Return ExteriorColourInfo.GetExteriorColourInfo(Me)
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As ExteriorColourApplicability) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ExteriorColour) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ExteriorColourInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ExteriorColourApplicability Then
            Return Equals(DirectCast(obj, ExteriorColourApplicability))
        ElseIf TypeOf obj Is ExteriorColour Then
            Return Equals(DirectCast(obj, ExteriorColour))
        ElseIf TypeOf obj Is ExteriorColourInfo Then
            Return Equals(DirectCast(obj, ExteriorColourInfo))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewExteriorColourApplicability(ByVal exteriorColour As ExteriorColourInfo) As ExteriorColourApplicability
        Dim applicability As ExteriorColourApplicability = New ExteriorColourApplicability
        applicability.Create(exteriorColour)
        Return applicability
    End Function
#End Region

#Region " Constructors "

    Protected Sub New()
        'Prevent direct creation
        MarkAsChild()
        AllowEdit = False
        FieldPrefix = "EXTERIORCOLOUR"
    End Sub
#End Region

#Region " Data Access "

    Protected Overloads Sub Create(ByVal exteriorColour As ExteriorColourInfo)
        Create(exteriorColour.ID)
        _code = exteriorColour.Code
        _name = exteriorColour.Name
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        ID = dataReader.GetGuid("EXTERIORCOLOURID")
        _code = dataReader.GetString(GetFieldName("CODE"))
        _name = dataReader.GetString(GetFieldName("NAME"))
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", ID)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", ID)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@EXTERIORCOLOURID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommmandFields(command, "insert")
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        AddCommmandFields(command, "delete")
    End Sub

    Private Sub AddCommmandFields(ByVal command As SqlCommand, ByVal action As String)
        Dim listParent As IUniqueGuid = DirectCast(Parent, ExteriorColourApplicabilities).Parent

        If TypeOf listParent Is ModelGenerationEquipmentItem Then
            command.CommandText = String.Format("{0}GenerationEquipmentExteriorColourApplicability", action)
            command.Parameters.AddWithValue("@GENERATIONID", DirectCast(listParent, ModelGenerationEquipmentItem).Generation.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", listParent.GetObjectID())
        ElseIf TypeOf listParent Is ModelGenerationPack Then
            command.CommandText = String.Format("{0}GenerationPackExteriorColourApplicability", action)
            command.Parameters.AddWithValue("@GENERATIONID", DirectCast(listParent, ModelGenerationPack).Generation.ID)
            command.Parameters.AddWithValue("@PACKID", listParent.GetObjectID())
        ElseIf TypeOf listParent Is Accessory Then
            command.CommandText = String.Format("{0}ExteriorColourApplicability", action)
            command.Parameters.AddWithValue("@EQUIPMENTID", listParent.GetObjectID())
        Else
            Throw New ApplicationException("AddCommandFields not support for object of type " & listParent.GetType().Name)
        End If
    End Sub

#End Region

 
End Class
