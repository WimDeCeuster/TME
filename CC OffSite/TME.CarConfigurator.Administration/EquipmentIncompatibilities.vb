Imports TME.BusinessObjects.Templates
Imports TME.CarConfigurator.Administration.Enums

<Serializable()>
<CommandClassName("EquipmentItemIncompatibilities")>
Public Class EquipmentIncompatibilities
    Inherits BaseObjects.ContextUniqueGuidListBase(Of EquipmentIncompatibilities, EquipmentIncompatibility)

#Region " Business Properties & Methods "
    Friend Property EquipmentItem() As EquipmentItem
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, EquipmentItem)
        End Get
        Set(value As EquipmentItem)
            SetParent(value)
        End Set
    End Property
    Public Overridable Overloads Function Add(ByVal partialCarSpecification As PartialCarSpecification) As EquipmentIncompatibility
        Dim _incompatibility As EquipmentIncompatibility = MyBase.Add()
        With partialCarSpecification
            _incompatibility.PartialCarSpecification.ModelID = .ModelID
            _incompatibility.PartialCarSpecification.GenerationID = .GenerationID
            _incompatibility.PartialCarSpecification.EngineID = .EngineID
            _incompatibility.PartialCarSpecification.TransmissionID = .TransmissionID
            _incompatibility.PartialCarSpecification.WheelDriveID = .WheelDriveID
            _incompatibility.PartialCarSpecification.BodyTypeID = .BodyTypeID
            _incompatibility.PartialCarSpecification.FactoryGradeID = .FactoryGradeID
            _incompatibility.PartialCarSpecification.SteeringID = .SteeringID
            _incompatibility.PartialCarSpecification.GradeID = .GradeID
            _incompatibility.PartialCarSpecification.VersionID = .VersionID
        End With
        Return _incompatibility
    End Function
#End Region

#Region " Contains  Methods "

    Public Overloads Function Contains(ByVal partialSpecification As PartialCarSpecification) As Boolean
        For Each _incompatibility As EquipmentIncompatibility In Me
            If _incompatibility.PartialCarSpecification.Equals(partialSpecification) Then Return True
        Next
        Return False
    End Function
    Public Overloads Function ContainsMatch(ByVal partialSpecification As PartialCarSpecification) As Boolean
        For Each _incompatibility As EquipmentIncompatibility In Me
            If _incompatibility.PartialCarSpecification.Matches(partialSpecification, PartialCarSpecification.MatchType.LeftJoin) Then Return True
        Next
        Return False
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Me.Count = 0 Then Return String.Empty
        If Me.Count = 1 Then Return Me(0).ToString()

        Dim _builder As System.Text.StringBuilder = New System.Text.StringBuilder(Me.Count)
        For Each _incompatibility As EquipmentIncompatibility In Me
            _builder.AppendLine(_incompatibility.ToString())
        Next
        Return _builder.ToString()
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewEquipmentIncompatibilities(ByVal equipmentItem As EquipmentItem) As EquipmentIncompatibilities
        Dim incompatibilities As EquipmentIncompatibilities = If(TypeOf equipmentItem Is Accessory, _
                                                                    New AccessoryIncompatibilities(), _
                                                                    New EquipmentIncompatibilities()
                                                                  )
        Return InitializeIncompatibilities(incompatibilities, equipmentItem)
    End Function

    Public Shared Function GetEquipmentIncompatibilities(ByVal equipmentItem As EquipmentItem) As EquipmentIncompatibilities
        Dim criteria = New ParentCriteria(equipmentItem.ID, "@EQUIPMENTID")

        Dim incompatibilities As EquipmentIncompatibilities = If(TypeOf equipmentItem Is Accessory, _
                                                                  DataPortal.Fetch(Of AccessoryIncompatibilities)(criteria), _
                                                                  DataPortal.Fetch(Of EquipmentIncompatibilities)(criteria))
        Return InitializeIncompatibilities(incompatibilities, equipmentItem)
    End Function

    Private Shared Function InitializeIncompatibilities(ByVal incompatibilities As EquipmentIncompatibilities, ByVal equipmentItem As EquipmentItem) As EquipmentIncompatibilities
        incompatibilities.EquipmentItem = equipmentItem

        Dim updatesAllowed =
                equipmentItem.Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) AndAlso
                (
                    Not equipmentItem.IsMasterObject() OrElse
                    equipmentItem.MasterType = MasterEquipmentType.PostProductionOption OrElse
                    equipmentItem.MasterType = MasterEquipmentType.Accessory
                )

        incompatibilities.AllowNew = updatesAllowed
        incompatibilities.AllowEdit = updatesAllowed
        incompatibilities.AllowRemove = updatesAllowed
        Return incompatibilities
    End Function

#End Region

#Region " Constructors "
    Protected Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Friend Overridable Sub FetchRow(ByVal dataReader As SafeDataReader)
        Dim resetAllowNew = AllowNew
        AllowNew = True
        MyBase.Add(GetObject(dataReader))
        AllowNew = resetAllowNew
    End Sub
#End Region
End Class
<Serializable()> Public Class EquipmentIncompatibility
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of EquipmentIncompatibility)

#Region " Business Properties & Methods "
    Private WithEvents _partialCarSpecification As PartialCarSpecification = PartialCarSpecification.NewPartialCarSpecification()

    Public ReadOnly Property PartialCarSpecification() As PartialCarSpecification
        Get
            Return _partialCarSpecification
        End Get
    End Property

    Private Sub OnPartialCarSpecificationPropertyChanged(sender As Object, e As ComponentModel.PropertyChangedEventArgs) Handles _partialCarSpecification.PropertyChanged
        ValidationRules.CheckRules("PartialCarSpecification")
    End Sub

    Protected ReadOnly Property EquipmentItem() As EquipmentItem
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, EquipmentIncompatibilities).EquipmentItem
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.PartialCarSpecification.ToString()
    End Function

    Public Overloads Function Equals(ByVal obj As EquipmentIncompatibility) As Boolean
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
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_partialCarSpecification Is Nothing) AndAlso _partialCarSpecification.IsDirty Then Return True
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
        Me.PartialCarSpecification.AppendParameters(command)
    End Sub
#End Region



End Class