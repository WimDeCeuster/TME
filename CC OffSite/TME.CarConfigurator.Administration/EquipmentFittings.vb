Imports TME.BusinessObjects.Templates

<Serializable()>
<CommandClassName("EquipmentItemFittings")>
Public Class EquipmentFittings
    Inherits StronglySortedListBase(Of EquipmentFittings, EquipmentFitting)
    
#Region " Business Properties & Methods "
    Friend ReadOnly Property EquipmentItem() As EquipmentItem
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, EquipmentItem)
        End Get
    End Property

    Public Overridable Overloads Function Add(ByVal partialCarSpecification As PartialCarSpecification) As EquipmentFitting
        Dim _fitting As EquipmentFitting = MyBase.Add()
        With partialCarSpecification
            _fitting.PartialCarSpecification.ModelID = .ModelID
            _fitting.PartialCarSpecification.GenerationID = .GenerationID
            _fitting.PartialCarSpecification.EngineID = .EngineID
            _fitting.PartialCarSpecification.TransmissionID = .TransmissionID
            _fitting.PartialCarSpecification.WheelDriveID = .WheelDriveID
            _fitting.PartialCarSpecification.BodyTypeID = .BodyTypeID
            _fitting.PartialCarSpecification.FactoryGradeID = .FactoryGradeID
            _fitting.PartialCarSpecification.SteeringID = .SteeringID
            _fitting.PartialCarSpecification.GradeID = .GradeID
            _fitting.PartialCarSpecification.VersionID = .VersionID
        End With
        Return _fitting
    End Function
#End Region

#Region " Contains  Methods "

    Public Overloads Function Contains(ByVal partialSpecification As PartialCarSpecification) As Boolean
        Return Me.Any(Function(x) x.PartialCarSpecification.Equals(partialSpecification))
    End Function
    Public Overloads Function ContainsMatch(ByVal partialSpecification As PartialCarSpecification) As Boolean
        Return Me.Any(Function(x) x.PartialCarSpecification.Matches(partialSpecification))
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Me.Count = 0 Then Return String.Empty
        If Me.Count = 1 Then Return Me(0).ToString()

        Dim _builder As System.Text.StringBuilder = New System.Text.StringBuilder(Me.Count)
        For Each _fitting As EquipmentFitting In Me
            _builder.AppendLine(_fitting.ToString())
        Next
        Return _builder.ToString()
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewEquipmentFittings(ByVal equipmentItem As EquipmentItem) As EquipmentFittings
        Dim fittings As EquipmentFittings = If(TypeOf equipmentItem Is Accessory, _
                                               New AccessoryFittings(), _
                                               New EquipmentFittings())
        InitializeFittings(fittings, equipmentItem)
        Return fittings
    End Function

    Friend Shared Function GetEquipmentFittings(ByVal equipmentItem As EquipmentItem) As EquipmentFittings
        Dim criteria = New ParentCriteria(equipmentItem.ID, "@EQUIPMENTID")

        Dim fittings As EquipmentFittings = If(TypeOf equipmentItem Is Accessory, _
                                               DataPortal.Fetch(Of AccessoryFittings)(criteria), _
                                               DataPortal.Fetch(Of EquipmentFittings)(criteria))

        InitializeFittings(fittings, equipmentItem)

        Return fittings
    End Function

    Private Shared Sub InitializeFittings(ByVal fittings As EquipmentFittings, ByVal equipmentItem As EquipmentItem)
        fittings.SetParent(equipmentItem)
        fittings.AllowNew = equipmentItem.AllowNew
        fittings.AllowEdit = equipmentItem.AllowEdit
        fittings.AllowRemove = equipmentItem.AllowRemove
    End Sub
#End Region

#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Friend Overridable Sub FetchRow(ByVal dataReader As SafeDataReader)
        Dim _fitting As EquipmentFitting = GetObject(dataReader)
        Me.SupressSecurityCheck = True
        MyBase.Add(_fitting)
        Me.SupressSecurityCheck = False
        _fitting.SetSecurityRights()
    End Sub
#End Region

End Class
<Serializable()> Public Class EquipmentFitting
    Inherits ContextUniqueGuidBusinessBase(Of EquipmentFitting)
    Implements ISortedIndex
    Implements ISortedIndexSetter

#Region " Business Properties & Methods "
    Private _index As Integer
    Protected WithEvents _partialCarSpecification As PartialCarSpecification = PartialCarSpecification.NewPartialCarSpecification()
    Private _rules As EquipmentFittingRules
    Private _isCleared As Boolean

    Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not AllowEdit Then Exit Property
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property
    Public ReadOnly Property PartialCarSpecification() As PartialCarSpecification
        Get
            Return _partialCarSpecification
        End Get
    End Property

    Private Sub OnPartialCarSpecificationPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles _partialCarSpecification.PropertyChanged
        ValidationRules.CheckRules("PartialCarSpecification")
    End Sub

    Protected Friend ReadOnly Property EquipmentItem() As EquipmentItem
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, EquipmentFittings).EquipmentItem
        End Get
    End Property


    Protected Friend Sub SetSecurityRights()
        AllowNew = EquipmentItem.AllowNew
        AllowEdit = EquipmentItem.AllowEdit
        AllowRemove = EquipmentItem.AllowRemove
    End Sub

    Public ReadOnly Property Rules() As EquipmentFittingRules
        Get
            _rules = if(_rules, EquipmentFittingRules.GetRules(Me))
            Return _rules
        End Get
    End Property

    Public Property IsCleared() As Boolean
        Get
            Return _isCleared
        End Get
        Private Set(value As Boolean)
            If value = IsCleared Then Return

            _isCleared = value
            PropertyHasChanged("IsCleared")
        End Set
    End Property

    Public Sub Clear()
        IsCleared = True
    End Sub

    Public Sub UnClear()
        IsCleared = False
    End Sub

    friend sub CheckRules()
        ValidationRules.CheckRules()
    End Sub

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.PartialCarSpecification.ToString()
    End Function

    Public Overloads Function Equals(ByVal obj As EquipmentFitting) As Boolean
        Return Not (obj Is Nothing) AndAlso (Me.ID.Equals(obj.ID) OrElse Me.PartialCarSpecification.Equals(obj.PartialCarSpecification))
    End Function
    Public Overloads Function Equals(ByVal obj As PartialCarSpecification) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.PartialCarSpecification.Equals(obj)
    End Function

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_partialCarSpecification Is Nothing) AndAlso Not _partialCarSpecification.IsValid Then Return False
            If Not (_rules Is Nothing) AndAlso Not _rules.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_partialCarSpecification Is Nothing) AndAlso _partialCarSpecification.IsDirty Then Return True
            If Not (_rules Is Nothing) AndAlso _rules.IsDirty Then Return True
            Return False
        End Get
    End Property


#End Region

#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation

        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _index = dataReader.GetInt16("SORTORDER")
        _isCleared = dataReader.GetBoolean("CLEARED")
        _partialCarSpecification = PartialCarSpecification.GetPartialCarSpecification(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overridable Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@EQUIPMENTID", Me.EquipmentItem.ID)
        command.Parameters.AddWithValue("@SORTORDER", Me.Index)
        command.Parameters.AddWithValue("@CLEARED", IsCleared)
        Me.PartialCarSpecification.AppendParameters(command)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If _rules IsNot Nothing Then _rules.Update(transaction)
    End Sub

#End Region
End Class