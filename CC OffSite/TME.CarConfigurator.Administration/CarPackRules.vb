<Serializable()>
Public Class CarPackRules
    Inherits CarRules

#Region "Business Properties & Methods"

    Protected Friend Overrides ReadOnly Property Car() As Car
        Get
            Return Pack.Car
        End Get
    End Property

    Public Property Pack() As CarPack
        Get
            Return DirectCast(Parent, CarPack)
        End Get
        Private Set(ByVal value As CarPack)
            SetParent(value)
        End Set
    End Property
#End Region

#Region "Shared Factory Methods"

    Friend Shared Function NewRules(ByVal pack As CarPack) As CarPackRules
        Dim rules = New CarPackRules()
        rules.Pack = pack
        Return rules
    End Function

#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region


End Class