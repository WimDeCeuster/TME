Imports TME.CarConfigurator.Administration.Enums
Imports TME.CarConfigurator.Administration.Interfaces
Imports TME.Common.Database
Imports System.Data.SqlClient


<Serializable()> Public NotInheritable Class UpholsteryApplicabilities
    Inherits ContextUniqueGuidListBase(Of UpholsteryApplicabilities, UpholsteryApplicability)
    Implements IUpholsteryApplicabilities
#Region " Delegates & Events "

    Friend Delegate Sub ApplicabilitiesChangedHandler(ByVal exteriorColourApplicability As UpholsteryApplicability)
    Friend Event ApplicabilityAdded As ApplicabilitiesChangedHandler
    Friend Event ApplicabilityRemoved As ApplicabilitiesChangedHandler

    Protected Overrides Sub OnListChanged(ByVal e As ComponentModel.ListChangedEventArgs)
        MyBase.OnListChanged(e)
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
            Dim upholsteryApplicability = Me(e.NewIndex)
            RaiseEvent ApplicabilityAdded(upholsteryApplicability)
        End If
    End Sub

    Private Overloads Sub OnRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        Dim upholsteryApplicability = DirectCast(e.RemovingItem, UpholsteryApplicability)
        RaiseEvent ApplicabilityRemoved(upholsteryApplicability)
    End Sub

#End Region

#Region " Business Properties & Methods "

    Public Overloads Function Add(ByVal upholstery As UpholsteryInfo) As IApplicability Implements IUpholsteryApplicabilities.Add
        If Any(Function(x) x.Equals(upholstery)) Then Throw New Exceptions.ObjectAlreadyExists(Entity.UPHOLSTERY)

        Dim applicability As UpholsteryApplicability = UpholsteryApplicability.NewUpholsteryApplicability(upholstery)
        Add(applicability)
        Return applicability
    End Function
    Friend Overloads Function Add(ByVal dataReader As SafeDataReader) As UpholsteryApplicability
        Dim applicability As UpholsteryApplicability = GetObject(dataReader)
        RaiseListChangedEvents = False
        Add(applicability)
        RaiseListChangedEvents = True
        Return applicability
    End Function

    Public Shadows Property Parent() As IUniqueGuid Implements IUpholsteryApplicabilities.Parent
        Get
            Return DirectCast(MyBase.Parent, IUniqueGuid)
        End Get
        Friend Set(ByVal value As IUniqueGuid)
            SetParent(value)
        End Set
    End Property

    Friend Overloads Sub Remove(ByVal id As Guid) Implements IUpholsteryApplicabilities.Remove
        MyBase.Remove(id)
    End Sub

    Public Overloads Function Contains(ByVal id As Guid) As Boolean Implements IUpholsteryApplicabilities.Contains
        Return MyBase.Contains(id)
    End Function

    Public Overloads Sub Clear() Implements IUpholsteryApplicabilities.Clear
        MyBase.Clear()
    End Sub

    Public Overloads ReadOnly Property Count() As Integer Implements IUpholsteryApplicabilities.Count
        Get
            Return MyBase.Count
        End Get
    End Property

    Public Function GetItem(ByVal id As Guid) As IApplicability Implements IUpholsteryApplicabilities.GetItem
        Return Item(id)
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewUpholsteryApplicabilities(ByVal item As ModelGenerationOption) As UpholsteryApplicabilities
        Dim applicabilities As UpholsteryApplicabilities = New UpholsteryApplicabilities()
        applicabilities.Parent = item
        Return applicabilities
    End Function
    Friend Shared Function NewUpholsteryApplicabilities(ByVal item As ModelGenerationPack) As UpholsteryApplicabilities
        Dim applicabilities As UpholsteryApplicabilities = New UpholsteryApplicabilities()
        applicabilities.Parent = item
        Return applicabilities
    End Function
 #End Region

#Region " Constructors "
    Private Sub New()
        MyBase.New()
    End Sub
#End Region

End Class
<Serializable()> Public Class UpholsteryApplicability
    Inherits ContextUniqueGuidBusinessBase(Of UpholsteryApplicability)
    Implements IUpholsteryApplicability

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

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As UpholsteryApplicability) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Upholstery) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As UpholsteryInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is UpholsteryApplicability Then
            Return Equals(DirectCast(obj, UpholsteryApplicability))
        ElseIf TypeOf obj Is Upholstery Then
            Return Equals(DirectCast(obj, Upholstery))
        ElseIf TypeOf obj Is UpholsteryInfo Then
            Return Equals(DirectCast(obj, UpholsteryInfo))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewUpholsteryApplicability(ByVal upholstery As UpholsteryInfo) As UpholsteryApplicability
        Dim applicability As UpholsteryApplicability = New UpholsteryApplicability
        applicability.Create(upholstery)
        Return applicability
    End Function
#End Region

#Region " Constructors "

    Protected Sub New()
        'Prevent direct creation
        MarkAsChild()
        AllowEdit = False
        FieldPrefix = "UPHOLSTERY"
    End Sub
#End Region

#Region " Data Access "

    Protected Overloads Sub Create(ByVal upholstery As UpholsteryInfo)
        Create(upholstery.ID)
        _code = upholstery.Code
        _name = upholstery.Name
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        ID = dataReader.GetGuid("UPHOLSTERYID")
        _code = dataReader.GetString(GetFieldName("CODE"))
        _name = dataReader.GetString(GetFieldName("NAME"))
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@UPHOLSTERYID", ID)
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@UPHOLSTERYID", ID)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@UPHOLSTERYID", ID)
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommmandFields(command, "insert")
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        AddCommmandFields(command, "delete")
    End Sub
    Private Sub AddCommmandFields(ByVal command As SqlCommand, ByVal action As String)
        Dim listParent As IUniqueGuid = DirectCast(Parent, UpholsteryApplicabilities).Parent

        If TypeOf listParent Is ModelGenerationOption Then
            command.CommandText = String.Format("{0}GenerationEquipmentUpholsteryApplicability", action)
            command.Parameters.AddWithValue("@GENERATIONID", DirectCast(listParent, ModelGenerationOption).Generation.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", listParent.GetObjectID())
        ElseIf TypeOf listParent Is ModelGenerationPack Then
            command.CommandText = String.Format("{0}GenerationPackUpholsteryApplicability", action)
            command.Parameters.AddWithValue("@GENERATIONID", DirectCast(listParent, ModelGenerationPack).Generation.ID)
            command.Parameters.AddWithValue("@PACKID", listParent.GetObjectID())
        Else
            Throw New ApplicationException("AddCommandFields not support for object of type " & listParent.GetType().Name)
        End If

    End Sub


#End Region

End Class
