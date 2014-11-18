Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class CarSpecificationValues
    Inherits ContextListBase(Of CarSpecificationValues, CarSpecificationValue)

#Region " Business Properties & Methods "

    Public Overloads Function Add(ByVal country As String, ByVal language As String) As CarSpecificationValue
        Dim defaultValue As CarSpecificationValue = GetDefaultValue()
        Dim specificationValue As CarSpecificationValue = CarSpecificationValue.NewCarSpecificationValue(country, language, defaultValue)
        Add(specificationValue)
        Return specificationValue
    End Function

    Default Public Shadows ReadOnly Property Item(ByVal country As String, ByVal language As String) As CarSpecificationValue
        Get
            Return FirstOrDefault(Function(x) x.Equals(country, language))
        End Get
    End Property

    Public Function GetSpecificOrDefaultValue(ByVal country As String, ByVal language As String) As CarSpecificationValue
        Dim value = FirstOrDefault(Function(x) x.Equals(country, language))
        If value IsNot Nothing Then Return value

        Return GetDefaultValue()
    End Function
    Public Function GetDefaultValue() As CarSpecificationValue
        Dim value As CarSpecificationValue

        Dim context = MyContext.GetContext()
        If context.IsSlaveRegionCountry Then
            value = FirstOrDefault(Function(x) x.Equals(context.Country.MainRegionCountryCode, context.Country.MainRegionLanguageCode))
            If value IsNot Nothing Then Return value
        End If

        Return FirstOrDefault(Function(x) x.CountryCode.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase))
    End Function

    Friend Property CarSpecification() As CarSpecification
        Get
            Return DirectCast(Parent, CarSpecification)
        End Get
        Private Set(ByVal value As CarSpecification)
            SetParent(value)
        End Set
    End Property

    Friend Overloads Sub Add(ByVal dataReader As SafeDataReader)
        Add(CarSpecificationValue.GetCarSpecificationValue(dataReader))
    End Sub
    Friend Overloads Sub Add(ByVal generationSpecificationValue As ModelGenerationSpecificationValue)
        Add(CarSpecificationValue.GetCarSpecificationValue(generationSpecificationValue))
    End Sub
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewCarSpecificationValues(ByVal carSpecification As CarSpecification) As CarSpecificationValues
        Dim values As CarSpecificationValues = New CarSpecificationValues()
        values.CarSpecification = carSpecification
        Return values
    End Function

#End Region

#Region " Constructors "

    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

End Class
<Serializable(), XmlInfo("value")> Public NotInheritable Class CarSpecificationValue
    Inherits ContextBusinessBase(Of CarSpecificationValue)

#Region " Business Properties & Methods "
    Private _countryCode As String
    Private _languageCode As String

    Private _overwritten As Boolean = False
    Private _value As String = Nothing
    Private _masterValue As String = Nothing
    Private _homologated As Boolean = False


    Public ReadOnly Property Specification() As CarSpecification
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarSpecificationValues).CarSpecification
        End Get
    End Property
    Public ReadOnly Property CountryCode() As String
        Get
            Return _countryCode
        End Get
    End Property
    Public ReadOnly Property LanguageCode() As String
        Get
            Return _languageCode
        End Get
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public Property Overwritten() As Boolean
        Get
            Return _overwritten
        End Get
        Private Set(ByVal newValue As Boolean)
            If newValue.Equals(_overwritten) Then Return

            _overwritten = newValue
            AllowEdit = newValue
            PropertyHasChanged("Overwritten")
        End Set
    End Property

    Public Function Overwrite() As Boolean
        If Overwritten Then Return False
        If Not CanOverwrite() Then Return False
        Overwritten = True
        Return True
    End Function
    Public Function Revert() As Boolean
        If Not Overwritten Then Return False
        If Not CanRevert() Then Return False
        RevertValues()
        Overwritten = False
        Return True
    End Function

    Private Sub RevertValues()
        Dim context = MyContext.GetContext()
        Dim country = context.Countries(CountryCode)

        If context.IsRegionCountry AndAlso country.IsSlaveRegionCountry Then
            If ResetFromCarValue(country.MainRegionCountryCode, country.MainRegionLanguageCode) Then Return
            If ResetFromGenerationValue(CountryCode, LanguageCode) Then Return
            If ResetFromGenerationValue(country.MainRegionCountryCode, country.MainRegionLanguageCode) Then Return
            If ResetFromGenerationValue(Environment.GlobalCountryCode, "EN") Then Return
            Return
        End If

        If ResetFromGenerationValue(CountryCode, LanguageCode) Then Return
        If ResetFromGenerationValue(Environment.GlobalCountryCode, "EN") Then Return
    End Sub
    Private Function ResetFromCarValue(ByVal country As String, ByVal language As String) As Boolean
        Dim carValue = Specification.Values.Item(country, language)
        If carValue Is Nothing Then Return False

        If carValue.MasterValue.IsSameAs(MasterValue) Then
            Parent.RemoveChild(Me)
            Return True
        End If
        Return False
    End Function
    Private Function ResetFromGenerationValue(ByVal country As String, ByVal language As String) As Boolean
        Dim generationValue = Specification.GenerationSpecification.GetValue(Specification.Car, country, language)
        If generationValue Is Nothing Then Return False

        If generationValue.MasterValue.IsSameAs(MasterValue) Then
            ResetValuesFrom(generationValue)
            Return True
        End If
        Return False
    End Function
    
    Public Function CanOverwrite() As Boolean
        If Overwritten Then Return False

        Dim context = MyContext.GetContext()
        If Not context.IsRegionCountry OrElse context.IsSlaveRegionCountry Then
            Return CountryCode.Equals(context.CountryCode, StringComparison.InvariantCultureIgnoreCase)
        End If
        Return Not CountryCode.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase)
    End Function
    Public Function CanRevert() As Boolean
        If Not Overwritten Then Return False

        Dim context = MyContext.GetContext()
        If Not context.IsRegionCountry OrElse context.IsSlaveRegionCountry Then
            Return CountryCode.Equals(context.CountryCode, StringComparison.InvariantCultureIgnoreCase)
        End If
        Return Not CountryCode.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase)
    End Function

    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal newValue As String)
            If Not Overwritten Then Throw New Exceptions.UpdateNotAllowed
            newValue = NormalizeValue(newValue)

            If Specification.IsMasterSpecification() AndAlso MasterValue.IsSameAs(newValue) Then
                newValue = Nothing
            End If
            If _value.IsSameAs(newValue) Then Exit Property

            _value = newValue
            PropertyHasChanged("Value")
        End Set
    End Property
    Public Property MasterValue() As String
        Get
            Return _masterValue
        End Get
        Set(ByVal newValue As String)
            newValue = NormalizeValue(newValue)
            If _masterValue.IsSameAs(newValue) Then Exit Property

            _masterValue = newValue
            PropertyHasChanged("MasterValue")
            If Specification.IsMasterSpecification() AndAlso _masterValue.IsSameAs(Value) Then
                Value = Nothing
            End If
        End Set
    End Property
   
    Public Property Homologated() As Boolean
        Get
            Return _homologated
        End Get
        Set(ByVal newValue As Boolean)
            If Not _homologated.Equals(newValue) Then
                _homologated = newValue
                PropertyHasChanged("Homologated")
            End If
        End Set
    End Property

    Private Sub ResetValuesFrom(ByVal obj As ModelGenerationSpecificationValue)
        If obj Is Nothing Then
            _value = Nothing
            _masterValue = Nothing
            _homologated = False
        Else
            _value = obj.Value
            _masterValue = obj.MasterValue
            _homologated = obj.Homologated

            CreatedBy = obj.CreatedBy
            CreatedOn = obj.CreatedOn
            ModifiedBy = obj.ModifiedBy
            ModifiedOn = obj.ModifiedOn
        End If

        AllowEdit = False
        AllowRemove = False
    End Sub

    Friend Sub NormalizeValues()
        _value = NormalizeValue(_value)
        _masterValue = NormalizeValue(_masterValue)
    End Sub

    Private Function NormalizeValue(ByVal valueToBeNormalized As String) As String
        If valueToBeNormalized Is Nothing Then Return Nothing

        If Specification.TypeCode = TypeCode.Decimal Then
            Return valueToBeNormalized.Replace(".", Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
        Else
            Return valueToBeNormalized
        End If
    End Function

    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Overwritten") Then Return True
        Return MyBase.CanWriteProperty(propertyName)
    End Function
    Private ReadOnly Property Car() As Car
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, CarSpecificationValues).CarSpecification.Car
        End Get
    End Property
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Value", 1024))
        ValidationRules.AddRule(DirectCast(AddressOf ValueValid, Validation.RuleHandler), "Value")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("MasterValue", 1024))
        ValidationRules.AddRule(DirectCast(AddressOf ValueValid, Validation.RuleHandler), "MasterValue")

    End Sub
    Public Sub CheckRules()
        ValidationRules.CheckRules()
    End Sub

    Private Shared Function ValueValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim carSpecificationValue As CarSpecificationValue = DirectCast(target, CarSpecificationValue)
        If carSpecificationValue.Specification Is Nothing Then Return True
        If Not carSpecificationValue.Overwritten() Then Return True

        Dim propertyValue As String = carSpecificationValue.Value
        If e.PropertyName = "MasterValue" Then
            If Not carSpecificationValue.Specification.HasMapping Then Return True 'No longer mapped? Then don't validate.
            propertyValue = carSpecificationValue.MasterValue
        End If

        If propertyValue Is Nothing Then Return True
        If propertyValue.Length = 0 Then Return True

        If Not carSpecificationValue.ValueTypeMatch(propertyValue, e) Then Return False
        If Not carSpecificationValue.ValueExpressionMatch(propertyValue, e) Then Return False
        Return True
    End Function
    Private Function ValueTypeMatch(ByVal propertyValue As String, ByVal e As Validation.RuleArgs) As Boolean
        If Specification.TypeCode = TypeCode.String Then Return True

        Try
            If Specification.TypeCode = TypeCode.Decimal Then
                Return Type.GetTypeCode(Decimal.Parse(propertyValue, Globalization.NumberStyles.AllowDecimalPoint).GetType()) = Specification.TypeCode
            Else
                Return (Type.GetTypeCode(Convert.ChangeType(propertyValue, Specification.TypeCode).GetType()) = Specification.TypeCode)
            End If
        Catch ex As Exception
            e.Description = "The value '" & propertyValue & "' for '" & Specification.Name & "' could not be parsed to a " & Specification.TypeCode.ToString()
            Return False
        End Try
    End Function
    Private Function ValueExpressionMatch(ByVal propertyValue As String, ByVal e As Validation.RuleArgs) As Boolean
        If Specification.Expression.Length = 0 Then Return True

        Dim oRegex As Text.RegularExpressions.Regex = New Text.RegularExpressions.Regex(Specification.Expression)
        If oRegex.IsMatch(propertyValue) Then
            e.Description = ""
            Return True
        Else
            e.Description = "The value of the specification does not match the defined regular expression."
            Return False
        End If

    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetCarSpecificationValue(ByVal dataReader As SafeDataReader) As CarSpecificationValue
        Dim carSpecificationValue As CarSpecificationValue = New CarSpecificationValue()
        carSpecificationValue.Fetch(dataReader)
        Return carSpecificationValue
    End Function
    Friend Shared Function GetCarSpecificationValue(ByVal generationSpecificationValue As ModelGenerationSpecificationValue) As CarSpecificationValue
        Dim carSpecificationValue As CarSpecificationValue = New CarSpecificationValue()
        carSpecificationValue.Create()
        carSpecificationValue._countryCode = generationSpecificationValue.CountryCode
        carSpecificationValue._languageCode = generationSpecificationValue.LanguageCode
        carSpecificationValue.ResetValuesFrom(generationSpecificationValue)
        carSpecificationValue.MarkOld()
        Return carSpecificationValue
    End Function
    Friend Shared Function NewCarSpecificationValue(ByVal country As String, ByVal language As String, ByVal initialValue As CarSpecificationValue) As CarSpecificationValue
        Dim carSpecificationValue As CarSpecificationValue = New CarSpecificationValue()
        carSpecificationValue.Create()
        carSpecificationValue._countryCode = country
        carSpecificationValue._languageCode = language
        If initialValue IsNot Nothing Then
            carSpecificationValue._value = initialValue.Value
            carSpecificationValue._masterValue = initialValue.MasterValue
            carSpecificationValue._homologated = initialValue.Homologated
        End If
        carSpecificationValue.AllowEdit = False
        carSpecificationValue.MarkOld()
        Return carSpecificationValue
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If String.IsNullOrEmpty(Value) Then Return MasterValue
        Return Value
    End Function

    Protected Overrides Function GetIdValue() As Object
        Return String.Format("{0}{1}", CountryCode, LanguageCode)
    End Function

    Public Overloads Function Equals(ByVal country As String, ByVal language As String) As Boolean
        Return CountryCode.Equals(country, StringComparison.InvariantCultureIgnoreCase) AndAlso LanguageCode.Equals(language, StringComparison.InvariantCultureIgnoreCase)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        MarkAsChild()
        AutoDiscover = False
        AllowNew = False
        AllowRemove = False
        AllowEdit = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
        With dataReader
            _countryCode = .GetString("COUNTRY")
            _languageCode = .GetString("LANGUAGE")
            _value = FetchValueField(dataReader, "VALUE")
            _masterValue = FetchValueField(dataReader, "MASTERVALUE")
            _homologated = .GetBoolean("HOMOLOGATED")

        End With
        _overwritten = True
        AllowEdit = True
    End Sub

    Private Shared Function FetchValueField(ByVal dataReader As Common.Database.SafeDataReader, ByVal fieldName As String) As String
        If dataReader.IsDBNull(fieldName) Then Return Nothing
        Return dataReader.GetString(fieldName)
    End Function


    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "deleteCarSpecificationValue"
        AddKeyCommendFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        If Not Overwritten Then
            AddDeleteCommandFields(command)
            Exit Sub
        End If

        command.CommandText = "updateCarSpecificationValue"
        AddKeyCommendFields(command)
        command.Parameters.AddWithValue("@VALUE", GetDBValue(Value))
        command.Parameters.AddWithValue("@MASTERVALUE", GetDBValue(MasterValue))
        command.Parameters.AddWithValue("@HOMOLOGATED", Homologated)

    End Sub

    Private Sub AddKeyCommendFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@CARID", Car.ID)
        command.Parameters.AddWithValue("@TECHSPECID", Specification.ID)
        command.Parameters.AddWithValue("@COUNTRY", CountryCode)
        command.Parameters.AddWithValue("@LANGUAGE", LanguageCode)
    End Sub

    Private Shared Function GetDBValue(ByVal obj As String) As Object
        If obj Is Nothing Then Return DBNull.Value
        Return obj
    End Function
#End Region


End Class