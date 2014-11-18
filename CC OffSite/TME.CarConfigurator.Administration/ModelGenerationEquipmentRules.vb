<Serializable()> Public Class ModelGenerationEquipmentRules
    Inherits ModelGenerationRules

#Region " Business Properties & Methods "

    Protected Friend Overrides ReadOnly Property Generation() As ModelGeneration
        Get
            Return EquipmentItem.Generation
        End Get
    End Property
    Public ReadOnly Property EquipmentItem() As ModelGenerationEquipmentItem
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationEquipmentItem)
        End Get
    End Property
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewRules(ByVal item As ModelGenerationEquipmentItem) As ModelGenerationEquipmentRules
        Dim rules As ModelGenerationEquipmentRules = New ModelGenerationEquipmentRules()
        rules.SetParent(item)
        Return rules
    End Function

#End Region
    
#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region

End Class

