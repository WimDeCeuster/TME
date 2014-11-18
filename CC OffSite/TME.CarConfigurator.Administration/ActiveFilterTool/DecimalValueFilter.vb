Imports TME.CarConfigurator.Administration.ActiveFilterTool.Enumerations

Namespace ActiveFilterTool
    <Serializable()> Public MustInherit Class DecimalValueFilter
        Inherits Filter

#Region " Business Properties & Methods "

        Private _sliderText As String = String.Empty
        Private _valueText As String = String.Empty
        Private _compareMode As FilterCompareMode = FilterCompareMode.Plus
        Private _defaultType As DefaultType = DefaultType.Max
        Private _defaultValue As Decimal = 0
        Private _minValue As Decimal = Decimal.MinValue
        Private _maxValue As Decimal = Decimal.MaxValue
        Private _step As Decimal = 0
        Private _roundingNumber As Decimal = 0

        Public Property SliderText() As String
            Get
                Return _sliderText
            End Get
            Set(ByVal value As String)
                If _sliderText <> value Then
                    _sliderText = value
                    PropertyHasChanged("SliderText")
                End If
            End Set
        End Property
        Public Property ValueText() As String
            Get
                Return _valueText
            End Get
            Set(ByVal value As String)
                If _valueText <> value Then
                    _valueText = value
                    PropertyHasChanged("ValueText")
                End If
            End Set
        End Property

        Public Property CompareMode() As FilterCompareMode
            Get
                Return _compareMode
            End Get
            Set(ByVal value As FilterCompareMode)
                If _compareMode <> value Then
                    _compareMode = value
                    PropertyHasChanged("CompareMode")
                End If
            End Set
        End Property
        Public Property DefaultType() As DefaultType
            Get
                Return _defaultType
            End Get
            Set(ByVal value As DefaultType)
                If _defaultType <> value Then
                    _defaultType = value
                    PropertyHasChanged("DefaultType")
                End If
            End Set
        End Property
        Public Property DefaultValue() As Decimal
            Get
                Return _defaultValue
            End Get
            Set(ByVal value As Decimal)
                If Not value.Equals(_defaultValue) Then
                    _defaultValue = value
                    PropertyHasChanged("DefaultValue")
                End If
            End Set
        End Property
        Public Property MinValue() As Decimal
            Get
                Return _minValue
            End Get
            Set(ByVal value As Decimal)
                If Not value.Equals(_minValue) Then
                    _minValue = value
                    PropertyHasChanged("MinValue")
                End If
            End Set
        End Property
        Public Property MaxValue() As Decimal
            Get
                Return _maxValue
            End Get
            Set(ByVal value As Decimal)
                If Not value.Equals(_maxValue) Then
                    _maxValue = value
                    PropertyHasChanged("MaxValue")
                End If
            End Set
        End Property
        Public Property [Step]() As Decimal
            Get
                Return _step
            End Get
            Set(ByVal value As Decimal)
                If Not value.Equals(_step) Then
                    _step = value
                    PropertyHasChanged("Step")
                End If
            End Set
        End Property
        Public Property RoundingNumber() As Decimal
            Get
                Return _roundingNumber
            End Get
            Set(ByVal value As Decimal)
                If Not value.Equals(_roundingNumber) Then
                    _roundingNumber = value
                    PropertyHasChanged("RoundingNumber")
                End If
            End Set
        End Property

#End Region

#Region " Business & Validation Rules "
        Protected Overrides Sub AddBusinessRules()
            MyBase.AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "SliderText")
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "ValueText")
            ValidationRules.AddRule(DirectCast(AddressOf CompareModeValid, Validation.RuleHandler), "CompareMode")
            ValidationRules.AddRule(DirectCast(AddressOf DefaultTypeValid, Validation.RuleHandler), "DefaultType")
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Decimal.NotEqualsTo, Validation.RuleHandler), New BusinessObjects.ValidationRules.Decimal.NotEqualsRuleArgs("Step", 0))

            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("SliderText", 255))
            ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("ValueText", 255))
        End Sub

        Private Shared Function CompareModeValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            If DirectCast(target, DecimalValueFilter).CompareMode = 0 Then
                e.Description = "You need to select a compare mode for the filter"
                Return False
            End If
            Return True
        End Function
        Private Shared Function DefaultTypeValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            If DirectCast(target, DecimalValueFilter).DefaultType = 0 Then
                e.Description = "You need to select a default value for the filter"
                Return False
            End If
            Return True
        End Function

#End Region

#Region " Constructors "
        Protected Sub New()
            MyBase.New()
        End Sub
#End Region
#Region " Data Access "
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            MyBase.FetchFields(dataReader)
            With dataReader
                _sliderText = .GetString("SLIDERTEXT")
                _valueText = .GetString("VALUETEXT")
                _compareMode = CType(.GetValue("COMPAREMODE"), FilterCompareMode)
                _defaultType = CType(.GetValue("DEFAULTTYPE"), DefaultType)
                _defaultValue = .GetDecimal("DEFAULTVALUE")
                If Not .IsDBNull("MINVALUE") Then _minValue = .GetDecimal("MINVALUE")
                If Not .IsDBNull("MAXVALUE") Then _maxValue = .GetDecimal("MAXVALUE")
                _step = .GetDecimal("STEP")
                _roundingNumber = .GetDecimal("ROUNDINGNUMBER")
            End With
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            MyBase.AddUpdateCommandFields(command)
            command.Parameters.AddWithValue("@SLIDERTEXT", SliderText)
            command.Parameters.AddWithValue("@VALUETEXT", ValueText)
            command.Parameters.AddWithValue("@COMPAREMODE", CompareMode)
            command.Parameters.AddWithValue("@DEFAULTTYPE", DefaultType)
            command.Parameters.AddWithValue("@DEFAULTVALUE", DefaultValue)

            If MinValue.Equals(Decimal.MinValue) Then
                command.Parameters.AddWithValue("@MINVALUE", DBNull.Value)
            Else
                command.Parameters.AddWithValue("@MINVALUE", MinValue)
            End If
            If MaxValue.Equals(Decimal.MaxValue) Then
                command.Parameters.AddWithValue("@MAXVALUE", DBNull.Value)
            Else
                command.Parameters.AddWithValue("@MAXVALUE", MaxValue)
            End If

            command.Parameters.AddWithValue("@STEP", Me.Step)
            command.Parameters.AddWithValue("@ROUNDINGNUMBER", RoundingNumber)
        End Sub
#End Region

    End Class
End Namespace