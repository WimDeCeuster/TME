Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports TME.Common
Imports TME.CarConfigurator.Administration.Promotions.Enums
Imports TME.BusinessObjects.Validation

Namespace Promotions

    <Serializable()>
    Public NotInheritable Class Promotions
        Inherits StronglySortedListBase(Of Promotions, Promotion)

#Region "Business Properties & Methods"
        Public Shadows Function Add(ByVal promotionType As PromotionType, ByVal promotionEntity As PromotionEntity) As Promotion
            Dim promo = Promotion.NewPromotion(promotionType, promotionEntity)
            MyBase.Add(promo)
            promo.CheckRules() 'because there is a validationrule that needed parent to exist
            Return (promo)
        End Function
#End Region

#Region "Shared Factory Methods"
        Public Shared Function GetPromotions() As Promotions
            Dim promotions = DataPortal.Fetch(Of Promotions)(New Criteria)
            Return promotions
        End Function
#End Region

#Region "Data Access"
        Protected Overrides Function GetObject(ByVal datareader As SafeDataReader) As Promotion
            Return Promotion.GetPromotion(datareader)
        End Function
#End Region

#Region "Constructors"
        Private Sub New()
            'prevent direct creation
        End Sub
#End Region

    End Class

    <Serializable()>
    Public MustInherit Class Promotion
        Inherits TranslateableBusinessBase
        Implements ISortedIndex, ISortedIndexSetter, ILinks, ILinkedAssets

#Region "Business Properties & Methods"

        Private _index As Integer
        Private _name As String
        Private _entity As PromotionEntity
        Private _promotionType As PromotionType
        Private _status As Integer
        Private _featureAsOf As Nullable(Of Date)
        Private _validFrom As Date
        Private _validUntil As Date

        Private WithEvents _promotionItems As PromotionItems
        Private WithEvents _promotionApplicabilities As PromotionApplicabilities
        Private _assets As LinkedAssets
        Private _links As Links

        Public Property PromotionEntity As PromotionEntity
            Get
                Return _entity
            End Get
            Private Set(ByVal value As PromotionEntity)
                _entity = value
            End Set
        End Property
        Public Property PromotionType As PromotionType
            Get
                Return _promotionType
            End Get
            Private Set(ByVal value As PromotionType)
                _promotionType = value
            End Set
        End Property

        Public Property Name As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                If _name.Equals(value) Then Return
                _name = value
                PropertyHasChanged("Name")
                ValidationRules.CheckRules("NameAndDates")
            End Set
        End Property


        Public Property ValidFrom As DateTime
            Get
                Return _validFrom
            End Get
            Set(ByVal value As DateTime)
                Dim newValue = New DateTime(value.Year, value.Month, value.Day, 0, 0, 0) 'set from date to start of the day

                If _validFrom.CompareTo(newValue) = 0 Then Return
                _validFrom = newValue
                PropertyHasChanged("ValidFrom")
                ValidationRules.CheckRules("Dates")
                ValidationRules.CheckRules("NameAndDates")
            End Set
        End Property

        Public Property ValidUntil As DateTime
            Get
                Return _validUntil
            End Get
            Set(ByVal value As DateTime)
                Dim newValue = New DateTime(value.Year, value.Month, value.Day, 23, 59, 59) 'set until date to end of the day

                If _validUntil.CompareTo(newValue) = 0 Then Return
                _validUntil = newValue

                PropertyHasChanged("ValidUntil")
                ValidationRules.CheckRules("Dates")
                ValidationRules.CheckRules("NameAndDates")
            End Set
        End Property
        
        Public Property FeatureAsOf As Nullable(Of DateTime)
            Get
                Return _featureAsOf
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                If value.Equals(_featureAsOf) Then Return
                If value.HasValue Then value = New DateTime(value.Value.Year, value.Value.Month, value.Value.Day, 0, 0, 0) 'set from date to start of the day

                _featureAsOf = value
                PropertyHasChanged("FeatureAsOf")
                ValidationRules.CheckRules("Dates")
            End Set
        End Property

        Public Property Approved As Boolean
            Get
                Return (_status And Status.ApprovedForLive) = Status.ApprovedForLive
            End Get
            Set(ByVal value As Boolean)
                If Approved.Equals(value) Then Return

                If Approved Then 'it was approved
                    _status -= Status.ApprovedForLive 'not approved anymore
                Else 'it was not approved
                    _status += Status.ApprovedForLive 'approve it
                End If

                PropertyHasChanged("Approved")
                ValidationRules.CheckRules("Dates")
                ValidationRules.CheckRules("ApprovalGiven")
            End Set
        End Property

        Public Property Preview As Boolean
            Get
                Return (_status And Status.ApprovedForPreview) = Status.ApprovedForPreview
            End Get
            Set(ByVal value As Boolean)
                If Preview.Equals(value) Then Return

                If Preview Then 'it was in preview
                    _status -= Status.ApprovedForPreview 'not in preview anymore
                Else 'it was not in preview
                    _status += Status.ApprovedForPreview 'put it in preview
                End If

                PropertyHasChanged("Preview")
                ValidationRules.CheckRules("Dates")
                ValidationRules.CheckRules("ApprovalGiven")
            End Set
        End Property

        Public ReadOnly Property Items As PromotionItems
            Get
                If _promotionItems Is Nothing Then _promotionItems = PromotionItems.GetPromotionItems(Me)
                Return _promotionItems
            End Get
        End Property
        Private Sub ItemsChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles _promotionItems.ListChanged
            ValidationRules.CheckRules("ApprovalGiven")
        End Sub

        Public ReadOnly Property Applicabilities As PromotionApplicabilities
            Get
                If _promotionApplicabilities Is Nothing Then _promotionApplicabilities = PromotionApplicabilities.GetPromotionApplicabilities(Me)
                Return _promotionApplicabilities
            End Get
        End Property
        Private Sub ApplicabilitiesChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles _promotionApplicabilities.ListChanged
            ValidationRules.CheckRules("ApprovalGiven")
        End Sub

        Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
            Get
                Return _index
            End Get
        End Property

        Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
            Set(ByVal value As Integer)
                If Not _index.Equals(value) Then
                    _index = value
                    PropertyHasChanged("Index")
                End If
            End Set
        End Property

        Public ReadOnly Property Assets() As LinkedAssets Implements ILinkedAssets.Assets
            Get
                If _assets Is Nothing Then
                    If IsNew Then 'if promotion is new
                        _assets = LinkedAssets.NewLinkedAssets(ID)
                    Else
                        _assets = LinkedAssets.GetLinkedAssets(ID)
                    End If
                End If
                Return _assets
            End Get
        End Property

        Public ReadOnly Property Links() As Links Implements ILinks.Links
            Get
                If _links Is Nothing Then _links = Links.GetLinks(Me)
                Return _links
            End Get
        End Property

        Public ReadOnly Property ParentList As Promotions
            Get
                Return DirectCast(Parent, Promotions)
            End Get
        End Property

#End Region

#Region "Business & Validation Rules"

        Friend Sub CheckRules()
            ValidationRules.CheckRules()
        End Sub

        Protected Overrides Sub AddBusinessRules()
            MyBase.AddBusinessRules()

            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, RuleHandler), "Name")
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, RuleHandler),
                                    New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Name", 50))

            ValidationRules.AddRule(DirectCast(AddressOf ValidateDates, RuleHandler), "Dates")
            ValidationRules.AddRule(DirectCast(AddressOf ValidateFeatureAsOfDate, RuleHandler), "Dates")
            ValidationRules.AddRule(DirectCast(AddressOf ValidateName, RuleHandler), "NameAndDates")
            ValidationRules.AddRule(DirectCast(AddressOf ValidateApproval, RuleHandler), "ApprovalGiven")
        End Sub

        Private Shared Function ValidateApproval(ByVal target As Object, ByVal ruleArgs As RuleArgs) As Boolean
            Dim thisPromo = DirectCast(target, Promotion)

            'if not approved for live or preview => it is ok
            If Not (thisPromo.Approved OrElse thisPromo.Preview) Then Return True

            'otherwise, it is only valid if at least one applicability/promoteditem/image was added
            If thisPromo.Applicabilities.Count = 0 Then
                ruleArgs.Description = "When approving for live or preview, the promotion should have at least 1 applicability."
                Return False
            End If

            If thisPromo.PromotionEntity.IsNotACarEntity() AndAlso thisPromo.Items.Count = 0 Then
                ruleArgs.Description = "When approving for live or preview, the promotion should have at least 1 promoted item."
                Return False
            End If

            If thisPromo.Assets.Count = 0 Then
                ruleArgs.Description = "When approving for live or preview, the promotion should have an asset assigned to it."
                Return False
            End If

            Return True
        End Function

        Private Shared Function ValidateDates(ByVal target As Object, ByVal ruleArgs As RuleArgs) As Boolean
            Dim promo = DirectCast(target, Promotion)
            Dim errorMessage = String.Empty

            If promo.ValidUntil < DateTime.Now.Date AndAlso promo.IsNew Then
                errorMessage &= String.Format("New promotions should be current or in the future.{0}", System.Environment.NewLine)
            End If

            If (promo.Approved OrElse promo.Preview) AndAlso promo.ValidUntil < DateTime.Now.Date Then
                errorMessage &= "When approved for live or preview, the promotion should be current or in the future." & System.Environment.NewLine
            End If

            If promo.ValidUntil < promo.ValidFrom Then
                errorMessage &= String.Format("Promotion's end date should be after promotion's start date.{0}", System.Environment.NewLine)
            End If

            ruleArgs.Description = errorMessage

            Return errorMessage.Length = 0 'Valid if no errors
        End Function
        Private Shared Function ValidateFeatureAsOfDate(ByVal target As Object, ByVal ruleArgs As RuleArgs) As Boolean
            Dim promo = DirectCast(target, Promotion)
            
            
            If Not promo.FeatureAsOf.HasValue Then Return True
            If promo.FeatureAsOf.Value < promo.ValidFrom Then
                ruleArgs.Description = "The promotion is not valid yet when it should become a feature promotion"
                Return False
            End If
            If promo.FeatureAsOf.Value > promo.ValidUntil Then
                ruleArgs.Description = "The promotion is not valid anymore when it should become a feature promotion"
                Return False
            End If
            Return True
        End Function

        Private Shared Function ValidateName(ByVal target As Object, ByVal ruleArgs As RuleArgs) As Boolean
            Dim thisPromo = DirectCast(target, Promotion)

            'if thisPromo's parent isn't declared yet => do not check this validationrule
            If thisPromo.Parent Is Nothing Then Return True

            'dates aren't valid => doesn't make sense to check whether it overlaps with another promo or not
            'promo will be invalid anyway
            If thisPromo.ValidUntil < thisPromo.ValidFrom Then
                Return True
            End If

            'all promotions with the same name, excluding promo
            Dim allPromotionsWithSameName = DirectCast(thisPromo.Parent, Promotions) _
                                .Where(Function(p) _
                                    (Not p.ID.Equals(thisPromo.ID) AndAlso
                                     p.Name.Equals(thisPromo.Name, StringComparison.InvariantCultureIgnoreCase))) _
                                .ToList()

            If allPromotionsWithSameName.Count() = 0 Then Return True 'if there are no other promotions with this name, everything is ok

            ruleArgs.Description = "Promotion name should be unique in time." 'if ValidateName returns false, this is the error description

            Dim result = allPromotionsWithSameName.All(Function(promo) Not thisPromo.Overlaps(promo)) 'if it doesn't overlap, it is ok, otherwise, it isn't
            Return result
        End Function

        Private Function Overlaps(ByVal otherPromotion As Promotion) As Boolean
            Dim thisDateRange = New DateRange(ValidFrom, ValidUntil)
            Dim otherDateRange = New DateRange(otherPromotion.ValidFrom, otherPromotion.ValidUntil)
            Return thisDateRange.Overlaps(otherDateRange)
        End Function

#End Region

#Region "Framework Overrides"
        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If _promotionItems IsNot Nothing AndAlso Not _promotionItems.IsValid Then Return False
                If _promotionApplicabilities IsNot Nothing AndAlso Not _promotionApplicabilities.IsValid Then Return False
                If _assets IsNot Nothing AndAlso Not _assets.IsValid Then Return False
                If _links IsNot Nothing AndAlso Not _links.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If _promotionItems IsNot Nothing AndAlso _promotionItems.IsDirty Then Return True
                If _promotionApplicabilities IsNot Nothing AndAlso _promotionApplicabilities.IsDirty Then Return True
                If _assets IsNot Nothing AndAlso _assets.IsDirty Then Return True
                If _links IsNot Nothing AndAlso _links.IsDirty Then Return True
                Return False
            End Get
        End Property
#End Region

#Region "Base Overrides (Properties)"

        Public Overrides ReadOnly Property Entity() As Entity
            Get
                Return Entity.PROMOTION
            End Get
        End Property

#End Region

#Region "Base Overrides (Methods)"
        Protected Friend Overrides Function GetBaseName() As String
            Return Name
        End Function
#End Region

#Region "Shared Factory Methods"
        Friend Shared Function NewPromotion(ByVal promotionType As PromotionType, ByVal promotionEntity As PromotionEntity) As Promotion
            Dim promotion As Promotion
            Select Case promotionType.ID
                Case CShort(PromotionTypeId.Spotlight)
                    promotion = SpotlightPromotion.NewSpotlightPromotion()
                Case CShort(PromotionTypeId.FixedPriceDiscount)
                    promotion = FixedPriceDiscountPromotion.NewFixedPriceDiscountPromotion()
                Case CShort(PromotionTypeId.PercentageDiscount)
                    promotion = PercentageDiscountPromotion.NewPercentageDiscountPromotion()
                Case Else
                    Throw New ArgumentException("Promotion type with id " & promotionType.ID & " does not exist.")
            End Select
            promotion.PromotionType = promotionType
            promotion.PromotionEntity = promotionEntity
            promotion.InitializeFields()
            Return promotion
        End Function

        Friend Shared Function GetPromotion(ByVal dataReader As SafeDataReader) As Promotion
            Dim typeId = dataReader.GetInt16("PROMOTIONTYPEID")
            Select Case typeId
                Case CShort(PromotionTypeId.Spotlight)
                    Return SpotlightPromotion.GetSpotlightPromotion(dataReader)
                Case CShort(PromotionTypeId.FixedPriceDiscount)
                    Return FixedPriceDiscountPromotion.GetFixedPriceDiscountPromotion(dataReader)
                Case CShort(PromotionTypeId.PercentageDiscount)
                    Return PercentageDiscountPromotion.GetPercentageDiscountPromotion(dataReader)
                Case Else
                    Throw New NotSupportedException("Promotion type with id " & typeId & "does not exist.")
            End Select
        End Function

#End Region

#Region "Constructors"
        Protected Sub New()
            'prevent direct creation 
            MarkAsChild()
            AlwaysUpdateSelf = True
        End Sub
#End Region

#Region "Data Access"

        Protected Overrides Sub InitializeFields()
            MyBase.InitializeFields()
            _name = String.Empty
            _featureAsOf = New Nullable(Of Date)()
        End Sub

        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)

            
            If (dataReader.IsDBNull("ENTITY")) Then
                PromotionEntity = Nothing
            Else
                PromotionEntity = DirectCast([Enum].Parse(GetType(PromotionEntity), dataReader.GetString("ENTITY")), PromotionEntity)
            End If

            _promotionType = PromotionType.GetPromotionType(dataReader)
            _name = dataReader.GetString("NAME")
            _validFrom = dataReader.GetDateTime("VALIDFROM")
            _validUntil = dataReader.GetDateTime("VALIDUNTIL")
            _status = dataReader.GetInt16("STATUSID")
            _index = dataReader.GetInt16("SORTORDER")

            _featureAsOf = New Nullable(Of Date)()
            If Not dataReader.IsDBNull("FEATUREASOF") Then
                _featureAsOf = dataReader.GetDateTime("FEATUREASOF")
            End If


            MyBase.FetchFields(dataReader)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
            command.Parameters.AddWithValue("@TYPE", PromotionType.ID)
            If (PromotionEntity = Nothing) Then
                command.Parameters.AddWithValue("@ENTITY", DBNull.Value)
            Else
                command.Parameters.AddWithValue("@ENTITY", PromotionEntity.ToString())
            End If
        End Sub

        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)
        End Sub

        Private Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@NAME", Name)
            command.Parameters.AddWithValue("@VALIDFROM", ValidFrom)
            command.Parameters.AddWithValue("@VALIDUNTIL", ValidUntil)
            command.Parameters.AddWithValue("@STATUSID", _status)
            command.Parameters.AddWithValue("@SORTORDER", Index)
            If FeatureAsOf.HasValue Then
                command.Parameters.AddWithValue("@FEATUREASOF", FeatureAsOf.Value)
            Else
                command.Parameters.AddWithValue("@FEATUREASOF", DBNull.Value)
            End If
        End Sub

        Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
            If Not _promotionItems Is Nothing Then _promotionItems.Update(transaction)
            If Not _promotionApplicabilities Is Nothing Then _promotionApplicabilities.Update(transaction)
            If Not _assets Is Nothing Then _assets.Update(transaction)
            If Not _links Is Nothing Then _links.Update(transaction)
            MyBase.UpdateChildren(transaction)
        End Sub

#End Region


    End Class

End Namespace