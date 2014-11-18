Namespace Configurations
    <Serializable()> Public Class DefaultConfigurationSelectedPackEquipment
        Inherits ContextListBase(Of DefaultConfigurationSelectedPackEquipment, DefaultConfigurationSelectedPackEquipmentItem)
#Region "Business Properties & Methods"
        Public Property Pack() As DefaultConfigurationSelectedPack
            Get
                Return DirectCast(Parent, DefaultConfigurationSelectedPack)
            End Get
            Set(value As DefaultConfigurationSelectedPack)
                SetParent(value)
            End Set
        End Property

        Public Overloads Function Add(ByVal equipmentId As Guid) As DefaultConfigurationSelectedPackEquipmentItem
            Dim newItem = DefaultConfigurationSelectedPackEquipmentItem.NewDefaultConfigurationSelectedPackEquipmentItem(equipmentId)
            Add(newItem)
            Return newItem
        End Function

        Private Overloads Function Add(ByVal selectedEquipment As DefaultConfigurationSelectedPackEquipmentItem) As DefaultConfigurationSelectedPackEquipmentItem
            MyBase.Add(selectedEquipment)
            Return selectedEquipment
        End Function

        Public Overloads Function Remove(ByVal equipmentId As Guid) As Boolean
            Dim selectedItem As DefaultConfigurationSelectedPackEquipmentItem = Me.Single(Function(x) x.EquipmentID.Equals(equipmentId))
            Return Remove(selectedItem)
        End Function

        Public Overloads Function Contains(ByVal equipmentId As Guid) As Boolean
            Return Any(Function(x) x.EquipmentID.Equals(equipmentId))
        End Function
#End Region

#Region "Shared Factory Methods"
        Public Shared Function GetSelectedPackEquipment(ByVal pack As DefaultConfigurationSelectedPack) As DefaultConfigurationSelectedPackEquipment
            Dim equipments As DefaultConfigurationSelectedPackEquipment = DataPortal.Fetch(Of DefaultConfigurationSelectedPackEquipment)(New PackCriteria(pack.DefaultConfiguration.ID, pack.PackID))
            equipments.SetParent(pack)
            Return equipments
        End Function

#Region "Criteria"
        Private Class PackCriteria
            Inherits Criteria

            Private Property ConfigurationID() As Guid
            Private Property PackID() As Guid

            Public Sub New(ByVal configurationId As Guid, ByVal packId As Guid)
                Me.ConfigurationID = configurationId
                Me.PackID = packId
            End Sub

            Public Overrides Sub AddCommandFields(ByVal command As SqlCommand)
                MyBase.AddCommandFields(command)

                command.Parameters.AddWithValue("DEFAULTCONFIGURATIONID", ConfigurationID.GetDbValue())
                command.Parameters.AddWithValue("PACKID", PackID.GetDbValue())
            End Sub
        End Class
#End Region

#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region
    End Class

    <Serializable()> Public Class DefaultConfigurationSelectedPackEquipmentItem
        Inherits ContextBusinessBase(Of DefaultConfigurationSelectedPackEquipmentItem)

#Region "Business Properties & Methods"
        Private _equipmentID As Guid

        Public Property EquipmentID As Guid
            Get
                Return _equipmentID
            End Get
            Set(value As Guid)
                If _equipmentID = value Then Return
                _equipmentID = value
                PropertyHasChanged("EquipmentID")
            End Set
        End Property

        Public ReadOnly Property Pack() As DefaultConfigurationSelectedPack
            Get
                Return DirectCast(Parent, DefaultConfigurationSelectedPackEquipment).Pack
            End Get
        End Property
#End Region
#Region "Shared Factory methods"
        Public Shared Function NewDefaultConfigurationSelectedPackEquipmentItem(ByVal equipmentId As Guid) As DefaultConfigurationSelectedPackEquipmentItem
            Dim equipmentItem As DefaultConfigurationSelectedPackEquipmentItem = New DefaultConfigurationSelectedPackEquipmentItem()
            equipmentItem.Create()
            equipmentItem.MarkAsChild()
            equipmentItem.EquipmentID = equipmentId
            Return equipmentItem
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Sub InitializeFields()
            MyBase.InitializeFields()
            _equipmentID = Guid.Empty
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            MyBase.FetchFields(dataReader)
            _equipmentID = dataReader.GetGuid("EQUIPMENTID")
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@DEFAULTCONFIGURATIONID", Pack.DefaultConfiguration.ID)
            command.Parameters.AddWithValue("@PACKID", Pack.PackID)
            command.Parameters.AddWithValue("@EQUIPMENTID", _equipmentID)
        End Sub
        Private Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@DEFAULTCONFIGURATIONID", Pack.DefaultConfiguration.ID)
            command.Parameters.AddWithValue("@PACKID", Pack.PackID)
            command.Parameters.AddWithValue("@EQUIPMENTID", _equipmentID)
        End Sub

#End Region

        Protected Overrides Function GetIdValue() As Object
            Return _equipmentID
        End Function
    End Class
End Namespace