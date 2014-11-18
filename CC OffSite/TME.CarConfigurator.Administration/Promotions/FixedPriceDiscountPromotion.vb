Imports TME.BusinessObjects.Templates

Namespace Promotions
    <Serializable(), CommandClassName("Promotion")>
    Public Class FixedPriceDiscountPromotion
        Inherits DiscountPromotion

#Region "Business & Validation Rules"

        Protected Overrides Sub AddBusinessRules()
            MyBase.AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Decimal.GreaterThan, BusinessObjects.Validation.RuleHandler),
                                    New BusinessObjects.ValidationRules.Decimal.GreaterThanRuleArgs("DiscountValue", 0D))
        End Sub

#End Region

#Region "Shared Factory Methods"
        Public Shared Function NewFixedPriceDiscountPromotion() As FixedPriceDiscountPromotion
            Dim promotion = New FixedPriceDiscountPromotion()
            promotion.Create()
            Return promotion
        End Function

        Friend Shared Function GetFixedPriceDiscountPromotion(ByVal dataReader As SafeDataReader) As FixedPriceDiscountPromotion
            Dim promotion = New FixedPriceDiscountPromotion()
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
                DiscountValueBacking = Environment.ConvertPrice(.GetDecimal("DISCOUNTVALUE"), .GetString("DISCOUNTCURRENCY"))
            End With

            MyBase.FetchFields(dataReader)
        End Sub

        Protected Overrides Sub AddCommandFields(ByVal command As SqlCommand)
            With command.Parameters
                .AddWithValue("@DISCOUNTCURRENCY", MyContext.GetContext().Currency.Code)
            End With
            MyBase.AddCommandFields(command)
        End Sub
#End Region
    End Class
End Namespace