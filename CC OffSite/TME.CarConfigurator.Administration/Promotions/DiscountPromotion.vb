Imports TME.CarConfigurator.Administration.Promotions.Enums
Imports TME.BusinessObjects.Templates

Namespace Promotions
    'run stored procedures against Promotion instead of DiscountPromotion (e.g. getPromotion instead of getDiscountPromotion)
    <Serializable(), CommandClassName("Promotion")>
    Public MustInherit Class DiscountPromotion
        Inherits Promotion

#Region "Business Promperties & Methods"

        Private _issuedBy As PromotionIssuer
        Protected DiscountValueBacking As Decimal
        Private _hasToBeAppliedToSection As DiscountSectionApplicability
        Private _isAlreadyApplied As Boolean
        Private _customerOptInRequired As Boolean
        Private _hasToBeAppliedToPrice As DiscountPriceApplicability

        Public Property IssuedBy As PromotionIssuer
            Get
                Return _issuedBy
            End Get
            Set(ByVal value As PromotionIssuer)
                If (_issuedBy Is Nothing AndAlso value Is Nothing) _
                   OrElse (Not _issuedBy Is Nothing AndAlso _issuedBy.Equals(value)) Then
                    Return
                End If

                _issuedBy = value
                PropertyHasChanged("IssuedBy")
            End Set
        End Property

        Public Property DiscountValue As Decimal
            Get
                Return DiscountValueBacking
            End Get
            Set(ByVal value As Decimal)
                If DiscountValueBacking = value Then Return
                DiscountValueBacking = value
                PropertyHasChanged("DiscountValue")
            End Set
        End Property

        Public Property HasToBeAppliedToSection As DiscountSectionApplicability
            Get
                Return _hasToBeAppliedToSection
            End Get
            Set(ByVal value As DiscountSectionApplicability)
                If _hasToBeAppliedToSection = value Then Return
                _hasToBeAppliedToSection = value
                PropertyHasChanged("HasToBeAppliedToSection")
            End Set
        End Property

        Public Property HasToBeAppliedToPrice As DiscountPriceApplicability
            Get
                Return _hasToBeAppliedToPrice
            End Get
            Set(ByVal value As DiscountPriceApplicability)
                If _hasToBeAppliedToPrice = value Then Return
                _hasToBeAppliedToPrice = value
                PropertyHasChanged("HasToBeAppliedToPrice")
            End Set
        End Property

        Public Property IsAlreadyApplied As Boolean
            Get
                Return _isAlreadyApplied
            End Get
            Set(ByVal value As Boolean)
                If Not _isAlreadyApplied Xor value Then Return
                _isAlreadyApplied = Not _isAlreadyApplied
                PropertyHasChanged("IsAlreadyApplied")
                ValidationRules.CheckRules("ValidateCustomerOptIn")
            End Set
        End Property

        Public Property CustomerOptInRequired As Boolean
            Get
                Return _customerOptInRequired
            End Get
            Set(ByVal value As Boolean)
                If Not _customerOptInRequired Xor value Then Return
                _customerOptInRequired = Not _customerOptInRequired
                PropertyHasChanged("CustomerOptInRequired")
                ValidationRules.CheckRules("ValidateCustomerOptIn")
            End Set
        End Property

#End Region

#Region "Business & Validation Rules"

        Protected Overrides Sub AddBusinessRules()
            MyBase.AddBusinessRules()

            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Object.Required, BusinessObjects.Validation.RuleHandler), "IssuedBy")
            ValidationRules.AddRule(DirectCast(AddressOf ValidateEntityAndAppliedTo, BusinessObjects.Validation.RuleHandler), "HasToBeAppliedToSection")
            ValidationRules.AddRule(DirectCast(AddressOf ValidateCustomerOptIn, BusinessObjects.Validation.RuleHandler), "ValidateCustomerOptIn")
        End Sub

        Private Shared Function ValidateEntityAndAppliedTo(ByVal target As Object, ByVal ruleArgs As BusinessObjects.Validation.RuleArgs) As Boolean
            Dim thisPromo = DirectCast(target, DiscountPromotion)

            If (thisPromo.PromotionEntity.IsNotACarEntity()) Then
                'if promotion's entity is not a car entity => hastobeappliedtosection should not be nothing
                ruleArgs.Description = "PromotionEntity is not a car entity (equipment, pack, color, ...), so HasToBeAppliedTo should be nothing."
                Return thisPromo.HasToBeAppliedToSection = DiscountSectionApplicability.Nothing
            Else
                'if promotion's entity is a car entity => hastobeappliedtosection should be nothing
                ruleArgs.Description = "PromotionEntity is a car entity (body, grade, engine, ...), so HasToBeAppliedTo should be filled out."
                Return Not thisPromo.HasToBeAppliedToSection = DiscountSectionApplicability.Nothing
            End If

        End Function

        Private Shared Function ValidateCustomerOptIn(ByVal target As Object, ByVal ruleArgs As BusinessObjects.Validation.RuleArgs) As Boolean
            Dim thisPromo = DirectCast(target, DiscountPromotion)

            ruleArgs.Description = "The promotion is already applied, so the customer cannot opt in on it anymore."

            'if the promotion is already applied, then customer cannot opt in on it anymore
            If (thisPromo.IsAlreadyApplied) Then Return Not thisPromo.CustomerOptInRequired

            Return True
        End Function
#End Region

#Region "Constructors"
        Protected Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region "Data Access"

        Protected Overrides Sub InitializeFields()
            MyBase.InitializeFields()

            _hasToBeAppliedToPrice = DiscountPriceApplicability.ListPrice 'default on listprice
            If PromotionEntity.IsNotACarEntity Then
                _hasToBeAppliedToSection = DiscountSectionApplicability.Nothing
            Else
                _hasToBeAppliedToSection = DiscountSectionApplicability.Vehicle
            End If

        End Sub

        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            With dataReader
                _issuedBy = PromotionIssuer.GetPromotionIssuer(dataReader)
                _hasToBeAppliedToSection = CType(.GetInt16("DISCOUNTSECTIONAPPLICABILITY"), DiscountSectionApplicability)
                _hasToBeAppliedToPrice = CType(.GetInt16("DISCOUNTPRICEAPPLICABILITY"), DiscountPriceApplicability)
                _isAlreadyApplied = .GetBoolean("DISCOUNTALREADYAPPLIED")
                _customerOptInRequired = .GetBoolean("CUSTOMEROPTINREQUIRED")
            End With

            MyBase.FetchFields(dataReader)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
            MyBase.AddInsertCommandFields(command)
        End Sub

        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
            MyBase.AddUpdateCommandFields(command)
        End Sub

        Protected Overridable Sub AddCommandFields(ByVal command As SqlCommand)
            With command.Parameters
                .AddWithValue("@ISSUEDBY", IssuedBy.ID)
                .AddWithValue("@DISCOUNTSECTIONAPPLICABILITY", HasToBeAppliedToSection)
                .AddWithValue("@DISCOUNTPRICEAPPLICABILITY", HasToBeAppliedToPrice)
                .AddWithValue("@DISCOUNTALREADYAPPLIED", IsAlreadyApplied)
                .AddWithValue("@CUSTOMEROPTINREQUIRED", CustomerOptInRequired)
                .AddWithValue("@DISCOUNTVALUE", DiscountValue)
            End With
        End Sub

#End Region
    End Class
End Namespace