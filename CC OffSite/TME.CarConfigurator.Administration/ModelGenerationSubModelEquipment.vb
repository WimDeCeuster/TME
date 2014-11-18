Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public Class ModelGenerationSubModelEquipment
    Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelGenerationSubModelEquipment, ModelGenerationSubModelEquipmentItem)

#Region " Business Properties & Methods "

    Friend Property SubModel() As ModelGenerationSubModel
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationSubModel)
        End Get
        Private Set(ByVal value As ModelGenerationSubModel)
            Me.SetParent(value)
            AddHandler value.Generation.Equipment.EquipmentAdded, AddressOf EquipmentAdded
            AddHandler value.Generation.Equipment.EquipmentRemoved, AddressOf EquipmentRemoved
        End Set
    End Property
    Private Sub EquipmentAdded(ByVal equipmentItem As ModelGenerationEquipmentItem)
        If Me.Contains(equipmentItem.ID) Then Throw New ApplicationException("The item already exists in this collection")

        Dim _item As ModelGenerationSubModelEquipmentItem = ModelGenerationSubModelEquipmentItem.GetEquipmentItem(equipmentItem)

        Me.AllowNew = True
        Me.ReferenceMap.Add(_item.ID, _item)
        Me.Add(_item)
        Me.AllowNew = False
    End Sub
    Private Sub EquipmentRemoved(ByVal equipmentItem As ModelGenerationEquipmentItem)
        Dim _submodelItem As ModelGenerationSubModelEquipmentItem = Me(equipmentItem.ID)

        Me.AllowRemove = True
        _submodelItem.Remove()
        Me.ReferenceMap.Remove(_submodelItem.ID)
        Me.AllowRemove = False
    End Sub

    Default Public Overloads Overrides ReadOnly Property Item(ByVal id As Guid) As ModelGenerationSubModelEquipmentItem
        Get
            If id.Equals(Guid.Empty) Then Return Nothing
            If Me.ReferenceMap.ContainsKey(id) Then
                Return Me.ReferenceMap.Item(id)
            Else
                Return Nothing
            End If
        End Get
    End Property

    <NonSerialized()> Private _referenceMap As Generic.Dictionary(Of Guid, ModelGenerationSubModelEquipmentItem)
    Private ReadOnly Property ReferenceMap() As Generic.Dictionary(Of Guid, ModelGenerationSubModelEquipmentItem)
        Get
            If _referenceMap Is Nothing Then
                _referenceMap = New Generic.Dictionary(Of Guid, ModelGenerationSubModelEquipmentItem)(Me.Count)
                For Each _item As ModelGenerationSubModelEquipmentItem In Me
                    _referenceMap.Add(_item.ID, _item)
                Next
            End If
            Return _referenceMap
        End Get
    End Property

#End Region

#Region " Contains Methods "
    Public Overloads Overrides Function Contains(ByVal id As Guid) As Boolean
        Return Me.ReferenceMap.ContainsKey(id)
    End Function
    Public Overloads Function Contains(ByVal obj As ModelGenerationEquipmentItem) As Boolean
        Return Me.ReferenceMap.ContainsKey(obj.ID)
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEquipment(ByVal submodel As ModelGenerationSubModel) As ModelGenerationSubModelEquipment
        Dim _equipment As ModelGenerationSubModelEquipment = New ModelGenerationSubModelEquipment()
        If submodel.IsNew Then
            _equipment.Fetch(submodel.Generation.Equipment)
        Else
            _equipment.Combine(submodel.Generation.Equipment, DataPortal.Fetch(Of ModelGenerationSubModelEquipment)(New CustomCriteria(submodel)))
        End If
        _equipment.SubModel = submodel
        Return _equipment
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AllowNew = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _suModelID As Guid

        Public Sub New(ByVal submodel As ModelGenerationSubModel)
            _suModelID = submodel.ID
            CommandText = "getModelGenerationSubModelEquipment"
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@SUBMODELID", _suModelID)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property
    Private Sub MeBeforeUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.BeforeUpdateCommand
        'Clear the list of deleted objects. 
        'The database will take care of this for us via deleteGrenerationEquipmentItem
        Me.DeletedList.Clear()
    End Sub

    Private Overloads Sub Fetch(ByVal generationEquipment As IEnumerable(Of ModelGenerationEquipmentItem))
        Me.AllowNew = True
        For Each _item In generationEquipment
            Me.Add(ModelGenerationSubModelEquipmentItem.GetEquipmentItem(_item))
        Next
        Me.AllowNew = False
    End Sub
    Private Sub Combine(ByVal generationEquipment As IEnumerable(Of ModelGenerationEquipmentItem), ByVal submodelEquipment As ModelGenerationSubModelEquipment)
        Me.AllowNew = True
        For Each _item In generationEquipment
            If Not submodelEquipment Is Nothing AndAlso submodelEquipment.Contains(_item.ID) Then
                Dim _submodelItem As ModelGenerationSubModelEquipmentItem = submodelEquipment(_item.ID)
                _submodelItem.RefObject = _item
                Me.Add(_submodelItem)
            Else
                Me.Add(ModelGenerationSubModelEquipmentItem.GetEquipmentItem(_item))
            End If
        Next
        Me.AllowNew = False
    End Sub
#End Region

End Class
<Serializable()> Public Class ModelGenerationSubModelEquipmentItem
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of ModelGenerationSubModelEquipmentItem)

#Region " Business Properties & Methods "
    Private _overwritten As Boolean = False
    Private _keyFeature As Boolean = False

    <XmlInfo(XmlNodeType.Attribute)> Public Property Overwritten() As Boolean
        Get
            Return _overwritten
        End Get
        Private Set(ByVal value As Boolean)
            If Not value.Equals(_overwritten) Then
                _overwritten = value
                MyBase.AllowEdit = value
                PropertyHasChanged("Overwritten")
            End If
        End Set
    End Property
    Public Function Overwrite() As Boolean
        If Me.Overwritten Then Return False
        Me.Overwritten = True
        Return True
    End Function
    Public Function Revert() As Boolean
        If Not Me.Overwritten Then Return False
        Me.SetRefProperties()
        Me.Overwritten = False
        Return True
    End Function

    <XmlInfo(XmlNodeType.Attribute)> Public Property KeyFeature() As Boolean
        Get
            Return _keyFeature
        End Get
        Set(ByVal value As Boolean)
            If Not Me.KeyFeature = value Then
                _keyFeature = value
                PropertyHasChanged("KeyFeature")
                Me.RefObject.SubModelKeyFeatureChanged(value)
            End If
        End Set
    End Property
    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Overwritten") Then Return True
        Return MyBase.CanWriteProperty(propertyName)
    End Function
    Friend Sub Remove()
        Me.AllowRemove = True
        DirectCast(Me.Parent, ModelGenerationSubModelEquipment).Remove(Me)
        Me.AllowRemove = False
    End Sub

    Private Sub SetRefProperties()
        _keyFeature = Me.RefObject.KeyFeature
    End Sub

#End Region

#Region " Reference Properties & Methods "

    Private _refObject As ModelGenerationEquipmentItem

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.SubModel Is Nothing Then Return Nothing
            Return Me.SubModel.Generation
        End Get
    End Property
    Friend ReadOnly Property SubModel() As ModelGenerationSubModel
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationSubModelEquipment).SubModel
        End Get
    End Property

    Friend Property RefObject() As ModelGenerationEquipmentItem
        Get
            If _refObject Is Nothing Then
                If Me.Generation Is Nothing Then Return Nothing
                _refObject = Me.Generation.Equipment(Me.ID)
            End If

            Return _refObject
        End Get
        Set(ByVal value As ModelGenerationEquipmentItem)
            _refObject = value
            _refObject = value
            AddHandler value.PropertyChanged, AddressOf ParentPropertyChanged
        End Set
    End Property
    Private Sub ParentPropertyChanged(ByVal sender As Object, ByVal e As System.ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If Not e.PropertyName.Equals("KeyFeature") Then Exit Sub 'Olny interested in one specifc property 
        If Not Me.Overwritten Then
            SetRefProperties()
        ElseIf Me.KeyFeature = Me.RefObject.KeyFeature Then
            Me.Revert()
        End If
    End Sub

    Public ReadOnly Property PartNumber() As String
        Get
            Return Me.RefObject.PartNumber
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return Me.RefObject.Name
        End Get
    End Property
    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Type() As EquipmentType
        Get
            Return Me.RefObject.Type
        End Get
    End Property
    Public ReadOnly Property Category() As EquipmentCategoryInfo
        Get
            Return Me.RefObject.Category
        End Get
    End Property
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetEquipmentItem(ByVal item As ModelGenerationEquipmentItem) As ModelGenerationSubModelEquipmentItem
        Dim _item As ModelGenerationSubModelEquipmentItem = New ModelGenerationSubModelEquipmentItem()
        _item.Create(item)
        Return _item
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.RefObject.ToString
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationSubModelEquipmentItem) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationSubModelEquipmentItem Then
            Return Me.Equals(DirectCast(obj, ModelGenerationSubModelEquipmentItem))
        Else
            Return Me.RefObject.Equals(obj)
        End If
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        Me.MarkAsChild()
        Me.AllowNew = False
        Me.AllowEdit = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overridable Overloads Sub Create(ByVal item As ModelGenerationEquipmentItem)
        MyBase.Create(item.ID)
        Me.RefObject = item
        Me.SetRefProperties()
        MyBase.MarkOld() 'These objects don't need be saved untill somebody actualy does something with them
    End Sub

    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("EQUIPMENTID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        _keyFeature = dataReader.GetBoolean("KEYFEATURE")
        _overwritten = True
        MyBase.AllowEdit = True
    End Sub

    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@SUBMODELID", Me.SubModel.ID)
        command.Parameters.AddWithValue("@EQUIPMENTID", Me.ID)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If Not Me.Overwritten Then
            command.CommandText = "deleteModelGenerationSubModelEquipmentItem"
            Exit Sub
        End If
        command.CommandText = "updateModelGenerationSubModelEquipmentItem"
        command.Parameters.AddWithValue("@KEYFEATURE", Me.KeyFeature)
    End Sub

#End Region

End Class