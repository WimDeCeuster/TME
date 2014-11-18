Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class ModelGenerationSpecificationValues
    Inherits ContextUniqueGuidListBase(Of ModelGenerationSpecificationValues, ModelGenerationSpecificationValue)

#Region " Business Properties & Methods "

    Friend ReadOnly Property Specification() As ModelGenerationSpecification
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationSpecification)
        End Get
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal country As String, ByVal language As String, ByVal partialCarSpecification As PartialCarSpecification) As ModelGenerationSpecificationValue
        Get
            Return FirstOrDefault(Function(x) x.Equals(country, language) AndAlso x.PartialCarSpecification.Equals(partialCarSpecification))
        End Get
    End Property

    Public Function GetSpecifcOrDefaultValue(ByVal country As String, ByVal language As String, ByVal partialCarSpecification As PartialCarSpecification) As ModelGenerationSpecificationValue
        Dim value = FirstOrDefault(Function(x) x.Equals(country, language) AndAlso x.PartialCarSpecification.Equals(partialCarSpecification))
        If value IsNot Nothing Then Return value

        Return GetDefaultValue(partialCarSpecification)
    End Function
    Public Function GetDefaultValue(ByVal partialCarSpecification As PartialCarSpecification) As ModelGenerationSpecificationValue
        Dim matchedValues = Where(Function(x) x.PartialCarSpecification.Equals(partialCarSpecification)).ToList()
        Dim value As ModelGenerationSpecificationValue

        Dim context = MyContext.GetContext()
        If context.IsSlaveRegionCountry Then
            value = matchedValues.FirstOrDefault(Function(x) x.Equals(context.Country.MainRegionCountryCode, context.Country.MainRegionLanguageCode))
            If value IsNot Nothing Then Return value
        End If

        Return matchedValues.FirstOrDefault(Function(x) x.CountryCode.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase))
    End Function

    Public Shadows Function Add(ByVal country As String, ByVal language As String, ByVal partialCarSpecification As PartialCarSpecification) As ModelGenerationSpecificationValue
        If Not partialCarSpecification.ModelID.Equals(Specification.Generation.Model.ID) OrElse Not partialCarSpecification.GenerationID.Equals(Specification.Generation.ID) Then Throw New Exceptions.InvalidModelGenerationIdentifier("This identifier does not belong to the current model generation.")
        If Contains(country, language, partialCarSpecification) Then Throw New Exceptions.ObjectAlreadyExists("There already is a partialcarspec for this specification")

        Dim newValue = ModelGenerationSpecificationValue.NewModelGenerationSpecificationValue(country, language, partialCarSpecification)
        Dim defaultValue = GetDefaultValue(partialCarSpecification)
        MyBase.Add(newValue)


        If Not defaultValue Is Nothing Then
            newValue.Value = defaultValue.Value
            newValue.MasterValue = defaultValue.MasterValue
            newValue.Homologated = defaultValue.Homologated
        End If


        Return newValue
    End Function
    Friend Shadows Sub Add(ByVal dataReader As SafeDataReader)
        Dim specificationValue As ModelGenerationSpecificationValue = GetObject(dataReader)
        MyBase.Add(specificationValue)
        Return
    End Sub

    Public Overloads Function Contains(ByVal country As String, ByVal language As String, ByVal partialCarSpecification As PartialCarSpecification) As Boolean
        Return Not (Item(country, language, partialCarSpecification) Is Nothing)
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationSpecificationValues(ByVal specifciation As ModelGenerationSpecification) As ModelGenerationSpecificationValues
        Dim specificationValues As ModelGenerationSpecificationValues = New ModelGenerationSpecificationValues()
        specificationValues.SetParent(specifciation)
        Return specificationValues
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region


End Class
<Serializable()> Public NotInheritable Class ModelGenerationSpecificationValue
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationSpecificationValue)

#Region " Business Properties & Methods "

    Private _countryCode As String = String.Empty
    Private _languageCode As String = String.Empty

    Private _value As String = Nothing
    Private _masterValue As String = Nothing
    Private _homologated As Boolean = False
    Private _partialCarSpecification As PartialCarSpecification

    Public Property PartialCarSpecification() As PartialCarSpecification
        Get
            Return _partialCarSpecification
        End Get
        Private Set(ByVal spec As PartialCarSpecification)
            _partialCarSpecification = spec
        End Set
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

    Public Property Value() As String
        Get
            Return _value
        End Get
        Set(ByVal updatedValue As String)
            updatedValue = NormalizeValue(updatedValue)

            If Specification.IsMasterSpecification() AndAlso MasterValue.IsSameAs(updatedValue) Then
                updatedValue = Nothing
            End If
            If _value.IsSameAs(updatedValue) Then Exit Property

            _value = updatedValue
            PropertyHasChanged("Value")
        End Set
    End Property
    Public Property MasterValue() As String
        Get
            Return _masterValue
        End Get
        Set(ByVal updatedValue As String)
            updatedValue = NormalizeValue(updatedValue)
            If _masterValue.IsSameAs(updatedValue) Then Exit Property

            _masterValue = updatedValue
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
        Set(ByVal updatedValue As Boolean)
            If Not _homologated.Equals(updatedValue) Then
                _homologated = updatedValue
                PropertyHasChanged("Homologated")
            End If
        End Set
    End Property

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

    Public ReadOnly Property Specification() As ModelGenerationSpecification
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationSpecificationValues).Specification
        End Get
    End Property




    Friend Function Matches(ByVal car As Car) As Boolean
        If Not Matches(Specification.Dependency.BodyTypeOrder, PartialCarSpecification.BodyTypeID, car.BodyTypeID) Then Return False
        If Not Matches(Specification.Dependency.EngineOrder, PartialCarSpecification.EngineID, car.EngineID) Then Return False
        If Not Matches(Specification.Dependency.TransmissionOrder, PartialCarSpecification.TransmissionID, car.TransmissionID) Then Return False
        If Not Matches(Specification.Dependency.WheelDriveOrder, PartialCarSpecification.WheelDriveID, car.WheelDriveID) Then Return False
        If Not Matches(Specification.Dependency.GradeOrder, PartialCarSpecification.GradeID, car.GradeID) Then Return False
        Return True
    End Function
    Private Shared Function Matches(ByVal order As Integer, ByVal specProperty As Guid, ByVal carProperty As Guid) As Boolean
        If order = 0 Then
            Return specProperty.Equals(Guid.Empty)
        Else
            Return specProperty.Equals(carProperty)
        End If
    End Function


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
        Dim specValue As ModelGenerationSpecificationValue = DirectCast(target, ModelGenerationSpecificationValue)
        If specValue.Specification Is Nothing Then Return True 'Can not validate at this point

        Dim propertyValue As String = specValue.Value
        If e.PropertyName = "MasterValue" Then
            If Not specValue.Specification.HasMapping Then Return True 'No longer mapped? Then don't validate.
            propertyValue = specValue.MasterValue
        End If

        If propertyValue Is Nothing Then Return True
        If propertyValue.Length = 0 Then Return True

        If Not specValue.ValueTypeMatch(propertyValue, e) Then Return False
        If Not specValue.ValueExpressionMatch(propertyValue, e) Then Return False
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
            e.Description = String.Format("The value '{0}' could not be parsed to a {1}", propertyValue, Specification.TypeCode.ToString())
            Return False
        End Try
    End Function
    Private Function ValueExpressionMatch(ByVal propertyValue As String, ByVal e As Validation.RuleArgs) As Boolean
        If Specification.Expression.Length = 0 Then Return True

        Dim regEx As Text.RegularExpressions.Regex = New Text.RegularExpressions.Regex(Specification.Expression)
        If regEx.IsMatch(propertyValue) Then
            e.Description = ""
            Return True
        Else
            If Specification.HelpText.Length > 0 Then
                e.Description = "The value does not match the defined regular expression.<br/>" & Specification.HelpText.Replace("{", "{{").Replace("}", "}}")
            Else
                e.Description = "The value does not match the defined regular expression."
            End If
            Return False
        End If

    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        If Value Is Nothing AndAlso MasterValue Is Nothing Then Return String.Empty
        If Value Is Nothing Then Return MasterValue
        Return Value
    End Function
    Public Overloads Function Equals(ByVal country As String, ByVal language As String) As Boolean
        Return CountryCode.Equals(country, StringComparison.InvariantCultureIgnoreCase) AndAlso LanguageCode.Equals(language, StringComparison.InvariantCultureIgnoreCase)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationSpecificationValue) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationSpecificationValue Then
            Return Equals(DirectCast(obj, ModelGenerationSpecificationValue))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationSpecificationValue(ByVal country As String, ByVal language As String, ByVal partialCarSpecification As PartialCarSpecification) As ModelGenerationSpecificationValue
        Dim specificationValue As ModelGenerationSpecificationValue = New ModelGenerationSpecificationValue
        specificationValue.Create()
        specificationValue._countryCode = country
        specificationValue._languageCode = language
        specificationValue.PartialCarSpecification = partialCarSpecification
        Return specificationValue
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _countryCode = dataReader.GetString("COUNTRY")
        _languageCode = dataReader.GetString("LANGUAGE")
        _value = FetchValueField(dataReader, "VALUE")
        _masterValue = FetchValueField(dataReader, "MASTERVALUE")
        _homologated = dataReader.GetBoolean("HOMOLOGATED")
        _partialCarSpecification = PartialCarSpecification.GetPartialCarSpecification(dataReader)

        Dim context = MyContext.GetContext()

        If Not context.IsRegionCountry OrElse context.IsSlaveRegionCountry Then
            AllowEdit = _countryCode.Equals(context.CountryCode, StringComparison.InvariantCultureIgnoreCase)
            AllowRemove = AllowEdit
            Return
        End If


        If context.IsMainRegionCountry Then
            AllowEdit = Not _countryCode.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase)
            AllowRemove = AllowEdit
            Return
        End If


    End Sub
    Private Shared Function FetchValueField(ByVal dataReader As Common.Database.SafeDataReader, ByVal fieldName As String) As String
        If dataReader.IsDBNull(fieldName) Then Return Nothing
        Return dataReader.GetString(fieldName)
    End Function

    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "insertSpecificationValue"
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateSpecificationValue"
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "deleteSpecificationValue"
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@TECHSPECID", Specification.ID)
        command.Parameters.AddWithValue("@COUNTRY", CountryCode)
        command.Parameters.AddWithValue("@LANGUAGE", LanguageCode)
        command.Parameters.AddWithValue("@VALUE", GetDBValue(Value))
        command.Parameters.AddWithValue("@MASTERVALUE", GetDBValue(MasterValue))
        command.Parameters.AddWithValue("@HOMOLOGATED", Homologated)
        PartialCarSpecification.AppendParameters(command)
    End Sub
    Private Shared Function GetDBValue(ByVal potentialNullValue As String) As Object
        If potentialNullValue Is Nothing Then Return DBNull.Value
        Return potentialNullValue
    End Function

#End Region


End Class