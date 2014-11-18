<Serializable()>
Public Class CarEquipmentRules
    Inherits CarRules

#Region "Business Properties & Methods"
    Protected Friend Overrides ReadOnly Property Car() As Car
        Get
            Return EquipmentItem.Car
        End Get
    End Property
    Public Property EquipmentItem() As CarEquipmentItem
        Get
            Return DirectCast(Parent, CarEquipmentItem)
        End Get
        Private Set(ByVal value As CarEquipmentItem)
            SetParent(value)
        End Set
    End Property
#End Region

#Region "Shared Factory Methods"

    Friend Shared Function NewRules(ByVal carEquipmentItem As CarEquipmentItem) As CarEquipmentRules
        Dim carEquipmentRules = New CarEquipmentRules()
        carEquipmentRules.EquipmentItem = carEquipmentItem
        Return carEquipmentRules
    End Function

#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region



End Class