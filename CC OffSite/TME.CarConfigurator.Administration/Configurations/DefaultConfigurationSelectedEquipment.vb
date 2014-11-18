Namespace Configurations
    <Serializable()> Public Class DefaultConfigurationSelectedEquipment
        Inherits ContextListBase(Of DefaultConfigurationSelectedEquipment, DefaultConfigurationSelectedEquipmentItem)
#Region "Business Properties & Methods"
        Public Property DefaultConfiguration() As DefaultConfiguration
            Get
                Return DirectCast(Parent, DefaultConfiguration)
            End Get
            Set(value As DefaultConfiguration)
                SetParent(value)
            End Set
        End Property

        Public Overloads Function Add(ByVal equipmentId As Guid) As DefaultConfigurationSelectedEquipmentItem
            Dim newItem = DefaultConfigurationSelectedEquipmentItem.NewDefaultConfigurationSelectedEquipmentItem(equipmentId)
            Add(newItem)
            Return newItem
        End Function

        Private Overloads Function Add(ByVal selectedEquipment As DefaultConfigurationSelectedEquipmentItem) As DefaultConfigurationSelectedEquipmentItem
            MyBase.Add(selectedEquipment)
            Return selectedEquipment
        End Function

        Public Overloads Function Remove(ByVal equipmentId As Guid) As Boolean
            Dim selectedItem As DefaultConfigurationSelectedEquipmentItem = Me.Single(Function(x) x.EquipmentID.Equals(equipmentId))
            Return Remove(selectedItem)
        End Function

        Public Overloads Function Contains(ByVal equipmentId As Guid) As Boolean
            Return Any(Function(x) x.EquipmentID.Equals(equipmentId))
        End Function
#End Region

#Region "Shared Factory Methods"
        Public Shared Function GetSelectedEquipment(ByVal defaultConfiguration As DefaultConfiguration) As DefaultConfigurationSelectedEquipment
            Dim equipments As DefaultConfigurationSelectedEquipment = DataPortal.Fetch(Of DefaultConfigurationSelectedEquipment)(New ParentCriteria(defaultConfiguration.ID, "@DEFAULTCONFIGURATIONID"))
            equipments.SetParent(defaultConfiguration)
            Return equipments
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region
    End Class

    <Serializable()> Public Class DefaultConfigurationSelectedEquipmentItem
        Inherits ContextBusinessBase(Of DefaultConfigurationSelectedEquipmentItem)

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

        Public ReadOnly Property DefaultConfiguration() As DefaultConfiguration
            Get
                Return DirectCast(Parent, DefaultConfigurationSelectedEquipment).DefaultConfiguration
            End Get
        End Property
#End Region
#Region "Shared Factory methods"
        Public Shared Function NewDefaultConfigurationSelectedEquipmentItem(ByVal equipmentId As Guid) As DefaultConfigurationSelectedEquipmentItem
            Dim equipmentItem As DefaultConfigurationSelectedEquipmentItem = New DefaultConfigurationSelectedEquipmentItem()
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
            command.Parameters.AddWithValue("@DEFAULTCONFIGURATIONID", DefaultConfiguration.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", _equipmentID)
        End Sub
        Private Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@DEFAULTCONFIGURATIONID", DefaultConfiguration.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", _equipmentID)
        End Sub

#End Region

        Protected Overrides Function GetIdValue() As Object
            Return _equipmentID
        End Function
    End Class
End Namespace