
<Serializable()> Public Class ModelGenerationPackRules
    Inherits ModelGenerationRules

#Region " Business Properties & Methods "

    Protected Friend Overrides ReadOnly Property Generation() As ModelGeneration
        Get
            Return Pack.Generation
        End Get
    End Property
    Friend ReadOnly Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPack)
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationPackRules(ByVal pack As ModelGenerationPack) As ModelGenerationPackRules
        Dim rules As ModelGenerationPackRules = New ModelGenerationPackRules()
        rules.SetParent(pack)
        Return rules
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
