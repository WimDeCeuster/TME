Imports TME.CarConfigurator.Administration.Promotions.Enums
Imports TME.BusinessObjects.Validation

Namespace Promotions
    <Serializable()>
    Public Class PromotionApplicabilities
        Inherits BaseObjects.ContextUniqueGuidListBase(Of PromotionApplicabilities, PromotionApplicability)

#Region "Business Properties & Methods"
        Public Property Promotion As Promotion
            Get
                Return DirectCast(Parent, Promotion)
            End Get
            Private Set(ByVal value As Promotion)
                SetParent(value)
            End Set
        End Property

        Public Shadows Function Add(ByVal generation As ModelGenerationInfo) As PromotionApplicability
            Dim applicability = PromotionApplicability.NewPromotionApplicability(generation)
            MyBase.Add(applicability)
            applicability.CheckRules() 'because there is a validation rule that requires parent to be present
            Return (applicability)
        End Function

        Friend Overloads Function Contains(ByVal generation As ModelGenerationInfo) As Boolean
            'check if any of my applicabilities has a generation that is equal to the one in the parameter list
            Return Any(Function(pa) pa.Generation.Equals(generation))
        End Function
#End Region

#Region "Shared Factory Methods"
        Friend Shared Function GetPromotionApplicabilities(ByVal promo As Promotion) As PromotionApplicabilities
            If promo.IsNew Then Return NewPromotionApplicabilities(promo)

            Dim applicabilities = DataPortal.Fetch(Of PromotionApplicabilities)(New ParentCriteria(promo.ID, "@PROMOTIONID"))
            applicabilities.Promotion = promo
            Return applicabilities
        End Function

        Private Shared Function NewPromotionApplicabilities(ByVal promo As Promotion) As PromotionApplicabilities
            Dim applicabilities = New PromotionApplicabilities
            applicabilities.Promotion = promo
            Return applicabilities
        End Function

#End Region

#Region "Constructors"
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
            MarkAsChild()
        End Sub
#End Region

#Region "Data Access"

        Protected Overrides Function GetObject(ByVal dataReader As SafeDataReader) As PromotionApplicability
            Return PromotionApplicability.GetPromotionApplicability(dataReader)
        End Function

#End Region

    End Class

    <Serializable()>
    Public Class PromotionApplicability
        Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of PromotionApplicability)

#Region "Business Properties & Methods"

        Private ReadOnly _generation As ModelGenerationInfo
        Private _bodyType As BodyTypeInfo
        Private _engine As EngineInfo
        Private _transmission As TransmissionInfo
        Private _wheelDrive As WheelDriveInfo
        Private _grade As GradeInfo
        Private _subModel As SubModelInfo
        Private _discountValue As Decimal?
        Private _key As String

        Public ReadOnly Property Generation As ModelGenerationInfo
            Get
                Return _generation
            End Get
        End Property

        Public Property BodyType As BodyTypeInfo
            Get
                Return _bodyType
            End Get
            Set(ByVal value As BodyTypeInfo)
                If _bodyType IsNot Nothing AndAlso _bodyType.Equals(value) Then Return
                _bodyType = value
                PropertyHasChanged("BodyType")
            End Set
        End Property

        Public Property Engine As EngineInfo
            Get
                Return _engine
            End Get
            Set(ByVal value As EngineInfo)
                If _engine IsNot Nothing AndAlso _engine.Equals(value) Then Return
                _engine = value
                PropertyHasChanged("Engine")
            End Set
        End Property

        Public Property Transmission As TransmissionInfo
            Get
                Return _transmission
            End Get
            Set(ByVal value As TransmissionInfo)
                If _transmission IsNot Nothing AndAlso _transmission.Equals(value) Then Return
                _transmission = value
                PropertyHasChanged("Transmission")
            End Set
        End Property

        Public Property WheelDrive As WheelDriveInfo
            Get
                Return _wheelDrive
            End Get
            Set(ByVal value As WheelDriveInfo)
                If _wheelDrive IsNot Nothing AndAlso _wheelDrive.Equals(value) Then Return
                _wheelDrive = value
                PropertyHasChanged("WheelDrive")
            End Set
        End Property

        Public Property Grade As GradeInfo
            Get
                Return _grade
            End Get
            Set(ByVal value As GradeInfo)
                If _grade IsNot Nothing AndAlso _grade.Equals(value) Then Return
                _grade = value
                PropertyHasChanged("Grade")
            End Set
        End Property

        Public Property SubModel() As SubModelInfo
            Get
                Return _subModel
            End Get
            Set(ByVal value As SubModelInfo)
                If _subModel IsNot Nothing AndAlso _subModel.Equals(value) Then Return
                _subModel = value
                PropertyHasChanged("SubModel")
            End Set
        End Property

        Public Property DiscountValue As Decimal?
            Get
                Return _discountValue
            End Get
            Set(ByVal value As Decimal?)
                'nothing is valid in the case of a nullable decimal ^^
                'If _discountValue IsNot Nothing AndAlso _discountValue.Equals(value) Then Return
                If _discountValue.Equals(value) Then Return
                _discountValue = value
                PropertyHasChanged("DiscountValue")
            End Set
        End Property

        Private Property Key As String
            Get
                If Parent Is Nothing Then Return _key
                If _key Is Nothing OrElse _key.Length = 0 Then _key = GetKey()
                Return _key
            End Get
            Set(ByVal value As String)
                _key = value
                ValidationRules.CheckRules("Key")
            End Set
        End Property

        Private ReadOnly Property Promotion As Promotion
            Get
                Return DirectCast(Parent, PromotionApplicabilities).Promotion
            End Get
        End Property

        Public ReadOnly Property Name As String
            Get
                Return Key
            End Get
        End Property

#End Region

#Region "Business & Validation Rules"
        Friend Sub CheckRules()
            ValidationRules.CheckRules()
        End Sub

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf KeyUnique, RuleHandler), "Key")
            ValidationRules.AddRule(DirectCast(AddressOf DiscountValidation, RuleHandler), "DiscountValue")

            MyBase.AddBusinessRules()
        End Sub

        Private Shared Function DiscountValidation(ByVal target As Object, ByVal ruleArgs As RuleArgs) As Boolean
            Dim thisApplicability = DirectCast(target, PromotionApplicability)
            If thisApplicability.Parent Is Nothing Then Return True 'Validation rule will not be checked if not added to list yet

            If Not thisApplicability.DiscountValue.HasValue Then Return True 'rule is valid when no alternative discount has been given

            If thisApplicability.Promotion.PromotionType.ID = PromotionTypeId.FixedPriceDiscount Then
                If thisApplicability.DiscountValue.Value <= 0 Then
                    ruleArgs.Description = "The discount value should be greater than 0. "
                    Return False
                End If
            ElseIf thisApplicability.Promotion.PromotionType.ID = PromotionTypeId.PercentageDiscount Then
                If thisApplicability.DiscountValue.Value <= 0 Then
                    ruleArgs.Description = "The discount value should be greater than 0%. "
                    Return False
                ElseIf thisApplicability.DiscountValue.Value > 1 Then
                    ruleArgs.Description = "The discount value should be less than 100%. "
                    Return False
                End If
            End If

            Return True
        End Function

        Private Shared Function KeyUnique(ByVal target As Object, ByVal ruleArgs As RuleArgs) As Boolean
            Dim thisApplicability = DirectCast(target, PromotionApplicability)

            If thisApplicability.Parent Is Nothing Then Return True 'Validation rule will not be checked if not added to list yet

            'Get all other applicabilities
            Dim otherApplicabilities = DirectCast(thisApplicability.Parent, PromotionApplicabilities).Where(Function(pa) Not pa.ID.Equals(thisApplicability.ID))

            'if any of the other applicabilities has the same key as this one, this applicability is not valid
            If otherApplicabilities.Any(Function(pa) pa.Key.Equals(thisApplicability.Key)) Then
                ruleArgs.Description = "This applicability has already been defined for this promotion."
                Return False
            End If

            Return True

        End Function

#End Region

#Region "Shared Factory Methods"

        Friend Shared Function NewPromotionApplicability(ByVal generation As ModelGenerationInfo) As PromotionApplicability
            Dim applicability = New PromotionApplicability(generation)
            applicability.Create()
            Return applicability
        End Function

        Friend Shared Function GetPromotionApplicability(ByVal datareader As SafeDataReader) As PromotionApplicability
            Dim applicability = New PromotionApplicability(ModelGenerationInfo.GetModelGenerationInfo(datareader))
            applicability.Fetch(datareader)
            Return applicability
        End Function

#End Region

#Region "Framework Overrides"

        Protected Overrides Sub PropertyHasChanged(ByVal propertyName As String)
            MyBase.PropertyHasChanged(propertyName)

            If IsBaseDirty Then Key = GetKey()
        End Sub

#End Region

#Region "Constructors"
        Private Sub New(ByVal generation As ModelGenerationInfo)
            _generation = generation

            MarkAsChild()
        End Sub
#End Region

#Region "Data Access"

        Protected Overrides Sub InitializeFields()
            _bodyType = BodyTypeInfo.Empty
            _engine = EngineInfo.Empty
            _grade = GradeInfo.Empty
            _transmission = TransmissionInfo.Empty
            _wheelDrive = WheelDriveInfo.Empty
            _subModel = SubModelInfo.Empty

            MyBase.InitializeFields()
        End Sub

        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            _bodyType = BodyTypeInfo.GetBodyTypeInfo(dataReader)
            _engine = EngineInfo.GetEngineInfo(dataReader)
            _grade = GradeInfo.GetGradeInfo(dataReader)
            _transmission = TransmissionInfo.GetTransmissionInfo(dataReader)
            _wheelDrive = WheelDriveInfo.GetWheelDriveInfo(dataReader)
            _subModel = SubModelInfo.GetSubModelInfo(dataReader)

            With dataReader
                If Not .IsDBNull("DISCOUNTVALUE") Then
                    Select Case .GetInt16("PROMOTIONTYPEID")
                        Case CShort(PromotionTypeId.FixedPriceDiscount)
                            _discountValue = Environment.ConvertPrice(.GetDecimal("DISCOUNTVALUE"), .GetString("DISCOUNTCURRENCY"))
                        Case CShort(PromotionTypeId.PercentageDiscount)
                            _discountValue = .GetDecimal("DISCOUNTVALUE")
                    End Select
                End If
            End With

            MyBase.FetchFields(dataReader)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            With command.Parameters
                .AddWithValue("@PROMOTIONID", Promotion.ID)
            End With

            AddCommandFields(command)

            MyBase.AddInsertCommandFields(command)
        End Sub

        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            AddCommandFields(command)

            MyBase.AddUpdateCommandFields(command)
        End Sub

        Private Sub AddCommandFields(ByVal sqlCommand As SqlCommand)
            With sqlCommand.Parameters
                .AddWithValue("@GENERATIONID", Generation.ID)
                .AddWithValue("@SUBMODELID", SubModel.ID.GetDbValue())
                .AddWithValue("@BODYID", BodyType.ID.GetDbValue())
                .AddWithValue("@ENGINEID", Engine.ID.GetDbValue())
                .AddWithValue("@GRADEID", Grade.ID.GetDbValue())
                .AddWithValue("@TRANSMISSIONID", Transmission.ID.GetDbValue())
                .AddWithValue("@WHEELDRIVEID", WheelDrive.ID.GetDbValue())

                'only add discount to db if it is a discount promotion
                If Promotion.PromotionType.ID = PromotionTypeId.FixedPriceDiscount OrElse Promotion.PromotionType.ID = PromotionTypeId.PercentageDiscount Then
                    .AddWithValue("@DISCOUNTVALUE", DiscountValue)
                    .AddWithValue("@DISCOUNTCURRENCY", MyContext.GetContext().Currency.Code)
                End If
            End With
        End Sub

#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Me.Name
        End Function

        Public Overloads Function Equals(ByVal otherApplicability As PromotionApplicability) As Boolean
            Return otherApplicability IsNot Nothing AndAlso _
                (ID.Equals(otherApplicability.ID) OrElse Key.Equals(otherApplicability.Key))
        End Function

#End Region

#Region "Helper Methods"
        Private Function GetKey() As String

            Dim _builder As System.Text.StringBuilder = New System.Text.StringBuilder
            _builder.Append(Me.Generation.Model.Name & ", ")
            _builder.Append(Me.Generation.Name & ", ")

            If Not SubModel.IsEmpty Then _builder.Append(SubModel.Name & ", ")
            If Not BodyType.IsEmpty Then _builder.Append(BodyType.Name & ", ")
            If Not Engine.IsEmpty Then _builder.Append(Engine.Name & ", ")
            If Not Grade.IsEmpty Then _builder.Append(Grade.Name & ", ")
            If Not Transmission.IsEmpty Then _builder.Append(Transmission.Name & ", ")
            If Not WheelDrive.IsEmpty Then _builder.Append(WheelDrive.Name & ", ")
            
            Dim _buffer As String = _builder.ToString()
            _buffer = _buffer.Remove(_buffer.Length - 2, 2)
            Return _buffer

        End Function
#End Region

    End Class
End Namespace