Imports TME.CarConfigurator.Administration.Enums

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class FilterValueEquipmentItem
        Inherits ContextBusinessBase(Of FilterValueEquipmentItem)

#Region " Business Properties & Methods "
        Private _info As EquipmentItemInfo
        Private _mustBeAvailable As Boolean

        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
            Get
                Return _info.ID
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Partnumber() As String
            Get
                Return _info.Partnumber
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Name() As String
            Get
                Return _info.Name
            End Get
        End Property
        <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Type() As EquipmentType
            Get
                Return _info.Type
            End Get
        End Property

        Public Property MustBeAvailable() As Boolean
            Get
                Return _mustBeAvailable
            End Get
            Set(ByVal value As Boolean)
                If _mustBeAvailable.Equals(value) Then Return

                _mustBeAvailable = value
                PropertyHasChanged("MustBeAvailable")
            End Set
        End Property


        Public ReadOnly Property FilterValue() As FilterValue
            Get
                If Parent Is Nothing Then Return Nothing
                Return DirectCast(Parent, FilterValueEquipment).FilterValue
            End Get
        End Property
        Friend Sub SetSecurityRights()
            Dim ownedByMe = FilterValue.Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase)
            AllowNew = ownedByMe
            AllowEdit = ownedByMe
            AllowRemove = ownedByMe
        End Sub

#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Name
        End Function
#End Region

#Region " Framework Overrides "
        Protected Overrides Function GetIdValue() As Object
            Return ID
        End Function
#End Region

#Region " Shared Factory Methods "
        Public Shared Function NewEquipmentFilterValueEquipmentItem(ByVal equipmentItem As EquipmentItemInfo) As FilterValueEquipmentItem
            Dim value As FilterValueEquipmentItem = New FilterValueEquipmentItem
            value.Create(equipmentItem)
            Return value
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            MarkAsChild()
        End Sub
#End Region

#Region " Data Access "

        Private Overloads Sub Create(ByVal equipmentItem As EquipmentItemInfo)
            _info = equipmentItem
            _mustBeAvailable = True
        End Sub

        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _info = EquipmentItemInfo.GetEquipmentInfo(dataReader)
            _mustBeAvailable = dataReader.GetBoolean("MUSTBEAVAILABLE")
        End Sub


        Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@FILTERVALUEID", FilterValue.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", ID)
        End Sub
        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            AddUpdateCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@FILTERVALUEID", FilterValue.ID)
            command.Parameters.AddWithValue("@EQUIPMENTID", ID)
            command.Parameters.AddWithValue("@MUSTBEAVAILABLE", MustBeAvailable)
        End Sub
#End Region

    End Class
End Namespace