Imports TME.BusinessObjects.Templates

Namespace Promotions
    <Serializable(),CommandClassName("Promotion")>
    Public Class SpotlightPromotion
        Inherits Promotion

#Region "Shared Factory Methods"

        Friend Shared Function NewSpotlightPromotion() As SpotlightPromotion
            Dim promotion = New SpotlightPromotion()
            promotion.Create()
            Return promotion
        End Function

        Friend Shared Function GetSpotlightPromotion(ByVal dataReader As SafeDataReader) As SpotlightPromotion
            Dim promotion = New SpotlightPromotion()
            promotion.Fetch(dataReader)
            Return promotion
        End Function

#End Region

#Region "Constructors"

        Private Sub New()
            'Prevent direct creation
        End Sub

#End Region

    End Class
End Namespace