Namespace Configurations
    <Serializable()> Public Class DefaultConfigurationSelectedPacks
        Inherits ContextListBase(Of DefaultConfigurationSelectedPacks, DefaultConfigurationSelectedPack)
#Region "Business Properties & Methods"
        Public Property DefaultConfiguration() As DefaultConfiguration
            Get
                Return DirectCast(Parent, DefaultConfiguration)
            End Get
            Set(value As DefaultConfiguration)
                SetParent(value)
            End Set
        End Property

        Public Overloads Function Add(ByVal equipmentId As Guid) As DefaultConfigurationSelectedPack
            Dim newItem = DefaultConfigurationSelectedPack.NewDefaultConfigurationSelectedPack(equipmentId)
            Add(newItem)
            Return newItem
        End Function

        Private Overloads Function Add(ByVal selectedPack As DefaultConfigurationSelectedPack) As DefaultConfigurationSelectedPack
            MyBase.Add(selectedPack)
            Return selectedPack
        End Function

        Public Overloads Function Remove(ByVal equipmentId As Guid) As Boolean
            Dim selectedItem As DefaultConfigurationSelectedPack = Me.Single(Function(x) x.PackID.Equals(equipmentId))
            Return Remove(selectedItem)
        End Function

        Public Overloads Function Contains(ByVal equipmentId As Guid) As Boolean
            Return Any(Function(x) x.PackID.Equals(equipmentId))
        End Function
#End Region

#Region "Shared Factory Methods"
        Public Shared Function GetSelectedPacks(ByVal defaultConfiguration As DefaultConfiguration) As DefaultConfigurationSelectedPacks
            Dim packs As DefaultConfigurationSelectedPacks = DataPortal.Fetch(Of DefaultConfigurationSelectedPacks)(New ParentCriteria(defaultConfiguration.ID, "@DEFAULTCONFIGURATIONID"))
            packs.SetParent(defaultConfiguration)
            Return packs
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region
    End Class

    <Serializable()> Public Class DefaultConfigurationSelectedPack
        Inherits ContextBusinessBase(Of DefaultConfigurationSelectedPack)

#Region "Business Properties & Methods"
        Private _packID As Guid
        Private _equipment As DefaultConfigurationSelectedPackEquipment

        Public Property PackID As Guid
            Get
                Return _packID
            End Get
            Set(value As Guid)
                If _packID = value Then Return
                _packID = value
                PropertyHasChanged("PackID")
            End Set
        End Property

        Public ReadOnly Property DefaultConfiguration() As DefaultConfiguration
            Get
                Return DirectCast(Parent, DefaultConfigurationSelectedPacks).DefaultConfiguration
            End Get
        End Property

        Public ReadOnly Property Equipment() As DefaultConfigurationSelectedPackEquipment
            Get
                If _equipment Is Nothing Then
                    _equipment = DefaultConfigurationSelectedPackEquipment.GetSelectedPackEquipment(Me)
                End If
                Return _equipment
            End Get
        End Property
#End Region
#Region "Shared Factory methods"
        Public Shared Function NewDefaultConfigurationSelectedPack(ByVal packId As Guid) As DefaultConfigurationSelectedPack
            Dim pack As DefaultConfigurationSelectedPack = New DefaultConfigurationSelectedPack()
            pack.Create()
            pack.MarkAsChild()
            pack.PackID = packId
            Return pack
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region

#Region " Framework Overrides "

        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If (Not _equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
                Return False
            End Get
        End Property

#End Region

#Region " Data Access "
        Protected Overrides Sub InitializeFields()
            MyBase.InitializeFields()
            _packID = Guid.Empty
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            MyBase.FetchFields(dataReader)
            _packID = dataReader.GetGuid("PACKID")
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@DEFAULTCONFIGURATIONID", DefaultConfiguration.ID)
            command.Parameters.AddWithValue("@PACKID", _packID)
        End Sub
        Private Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@DEFAULTCONFIGURATIONID", DefaultConfiguration.ID)
            command.Parameters.AddWithValue("@PACKID", _packID)
        End Sub

        Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.UpdateChildren(transaction)

            If Not _equipment Is Nothing Then _equipment.Update(transaction)
        End Sub
#End Region

        Protected Overrides Function GetIdValue() As Object
            Return _packID
        End Function
    End Class
End Namespace