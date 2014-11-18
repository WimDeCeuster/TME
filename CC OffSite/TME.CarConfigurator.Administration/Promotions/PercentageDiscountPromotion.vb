Imports TME.BusinessObjects.Templates

Namespace Promotions
    <Serializable(),CommandClassName("Promotion")>
    Public Class PercentageDiscountPromotion
        Inherits DiscountPromotion
#Region "Business & Validation Rules"

        Protected Overrides Sub AddBusinessRules()
            MyBase.AddBusinessRules()

            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Decimal.GreaterThan, BusinessObjects.Validation.RuleHandler),
                                    New BusinessObjects.ValidationRules.Decimal.GreaterThanRuleArgs("DiscountValue", 0D, "The discount value should be greater than 0%."))
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Decimal.LesserThanOrEquals, BusinessObjects.Validation.RuleHandler),
                                    New BusinessObjects.ValidationRules.Decimal.LesserThanOrEqualsRuleArgs("DiscountValue", 1D, "The discount value should be equal to or less than 100%."))
        End Sub

#End Region

#Region "Shared Factory Methods"
        Public Shared Function NewPercentageDiscountPromotion() As PercentageDiscountPromotion
            Dim promotion = New PercentageDiscountPromotion()
            promotion.Create()
            Return promotion
        End Function

        Friend Shared Function GetPercentageDiscountPromotion(ByVal dataReader As SafeDataReader) As PercentageDiscountPromotion
            Dim promotion = New PercentageDiscountPromotion()
            promotion.Fetch(dataReader)
            Return promotion
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region "Data Access"

        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            With dataReader
                DiscountValueBacking = .GetDecimal("DISCOUNTVALUE")
            End With

            MyBase.FetchFields(dataReader)
        End Sub
#End Region
    End Class
End Namespace