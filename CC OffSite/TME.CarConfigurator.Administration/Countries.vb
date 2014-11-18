Imports TME.BusinessObjects.Validation
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public Class Countries
    Inherits ContextUniqueGuidListBase(Of Countries, Country)

#Region " Business Properties & Methods "

    Public Shadows Function Add(ByVal country As TMME.Common.DataObjects.Country) As Country
        Dim newcountry As Country = Administration.Country.NewCountry(country)
        MyBase.Add(newcountry)
        Return newcountry
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetCountries() As Countries
        Return DataPortal.Fetch(Of Countries)(New Criteria)
    End Function
    Friend Shared Function GetCountries(ByVal dataReader As SafeDataReader) As Countries
        Dim countries As Countries = New Countries()
        countries.Fetch(dataReader)
        Return countries
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " SqlDatabaseContext "
    Protected Overrides ReadOnly Property SqlDatabaseContext() As BusinessObjects.Templates.SqlServer.ISqlDatabaseContext
        Get
            Return New BaseObjects.SqlDatabaseContext(False)
        End Get
    End Property
#End Region

End Class
<Serializable()> Public Class Country
    Inherits ContextUniqueGuidBusinessBase(Of Country)

#Region " Business Properties & Methods "

    Private _code As String = String.Empty
    Private _name As String = String.Empty

    Private _currency As TMME.Common.DataObjects.Currency
    Private _steering As SteeringInfo

    Private _createLocalEquipment As Boolean = False
    Private _addColourAndTrimToPacks As Boolean = False
    Private _manageAssets As Boolean = False
    Private _overruleAtCarLevel As Boolean = False
    Private _usesXmlImport As Boolean = False
    Private _usesBrochures As Boolean = False
    Private _usesEnergyEfficiencySpec As Boolean = False
    Private _usesNonVatPrice As Boolean = False
    Private _isLocalizedInA2A As Boolean = False
    Private _isRegionCountry As Boolean = False
    Private _mainRegionCountryCode As String = String.Empty
    Private _mainRegionLanguageCode As String = String.Empty
    Private _nmscCode As String

    Private _region As Region
    Private _languages As Languages

    Private _canSynchronizeVOMPrices As Boolean
    Private _canSynchronizePPOPrices As Boolean
    Private _canSynchronizeAccessoryPrices As Boolean

    Private _flatRate As Decimal
    Private _countries As Countries

    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    Public ReadOnly Property Countries() As Countries
        Get
            If _countries Is Nothing Then
                _countries = DirectCast(Parent, Countries)
            End If

            Return _countries
        End Get
    End Property


    Public Property Currency() As TMME.Common.DataObjects.Currency
        Get
            Return _currency
        End Get
        Set(ByVal value As TMME.Common.DataObjects.Currency)
            If Not value.Equals(_currency) Then
                _currency = value
                PropertyHasChanged("Currency")
            End If
        End Set
    End Property
    Public Property Steering() As SteeringInfo
        Get
            Return _steering
        End Get
        Set(ByVal value As SteeringInfo)
            If Not value.Equals(_steering) Then
                _steering = value
                PropertyHasChanged("Steering")
            End If
        End Set
    End Property

    Public Property CreateLocalEquipment() As Boolean
        Get
            Return _createLocalEquipment
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_createLocalEquipment) Then
                _createLocalEquipment = value
                PropertyHasChanged("CreateLocalEquipment")
            End If
        End Set
    End Property
    Public Property AddColourAndTrimToPacks() As Boolean
        Get
            Return _addColourAndTrimToPacks
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_addColourAndTrimToPacks) Then
                _addColourAndTrimToPacks = value
                PropertyHasChanged("AddColourAndTrimToPacks")
            End If
        End Set
    End Property
    Public Property ManageAssets() As Boolean
        Get
            Return _manageAssets
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_manageAssets) Then
                _manageAssets = value
                PropertyHasChanged("ManageAssets")
            End If
        End Set
    End Property
    Public Property OverruleAtCarLevel() As Boolean
        Get
            Return _overruleAtCarLevel
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_overruleAtCarLevel) Then
                _overruleAtCarLevel = value
                PropertyHasChanged("OverruleAtCarLevel")
            End If
        End Set
    End Property
    Public Property UsesXmlImport() As Boolean
        Get
            Return _usesXmlImport
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_usesXmlImport) Then
                _usesXmlImport = value
                PropertyHasChanged("UsesXmlImport")
            End If
        End Set
    End Property
    Public Property UsesBrochures() As Boolean
        Get
            Return _usesBrochures
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_usesBrochures) Then
                _usesBrochures = value
                PropertyHasChanged("UsesBrochures")
            End If
        End Set
    End Property
    Public Property UsesEnergyEfficiencySpecifications() As Boolean
        Get
            Return _usesEnergyEfficiencySpec
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_usesEnergyEfficiencySpec) Then
                _usesEnergyEfficiencySpec = value
                PropertyHasChanged("UsesEnergyEfficiencySpecifications")
            End If
        End Set
    End Property
    Public Property UsesNonVATPrice() As Boolean
        Get
            Return _usesNonVatPrice
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(_usesNonVatPrice) Then
                _usesNonVatPrice = value
                PropertyHasChanged("UsesNonVATPrice")
            End If
        End Set
    End Property

    Public Property LocalizedInA2A As Boolean
        Get
            If Code = Environment.GlobalCountryCode Then
                Return _isLocalizedInA2A
            End If

            Dim globalCountry = Countries.First(Function(country) country.Code = Environment.GlobalCountryCode)
            Return globalCountry.LocalizedInA2A AndAlso _isLocalizedInA2A
        End Get
        Set(value As Boolean)
            If value = _isLocalizedInA2A Then Return
            _isLocalizedInA2A = value
            PropertyHasChanged("LocalizedInA2A")
        End Set
    End Property

    'Public ReadOnly Property IsA2AIntegrationEnabled As Boolean
    '    Get
    '        Dim globalCountry = Countries.First(Function(country) country.Code = Environment.GlobalCountryCode)

    '        Return globalCountry.LocalizedInA2A AndAlso LocalizedInA2A
    '    End Get
    'End Property


    Public Property NmscCode As String
        Get
            Return _nmscCode
        End Get
        Set(ByVal value As String)
            If Not value.Equals(_nmscCode) Then
                _nmscCode = value
                PropertyHasChanged("NmscCode")
            End If
        End Set
    End Property
    Public ReadOnly Property Region As Region
        Get
            If (_region Is Nothing AndAlso IsRegionCountry) Then _region = Region.GetRegion(MainRegionCountryCode)
            Return _region
        End Get
    End Property
    Public ReadOnly Property Languages As Languages
        Get
            If (_languages Is Nothing) Then _languages = Languages.GetLanguages(Me)
            Return _languages
        End Get
    End Property

    Public ReadOnly Property IsRegionCountry() As Boolean
        Get
            Return _isRegionCountry
        End Get
    End Property
    Public ReadOnly Property IsMainRegionCountry() As Boolean
        Get
            Return String.Compare(Code, MainRegionCountryCode, True) = 0
        End Get
    End Property

    Public ReadOnly Property IsSlaveRegionCountry() As Boolean
        Get
            Return IsRegionCountry AndAlso Not IsMainRegionCountry
        End Get
    End Property

    Public Function IsGlobal() As Boolean
        Return Code.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase)
    End Function
    Public ReadOnly Property MainRegionCountryCode() As String
        Get
            Return _mainRegionCountryCode
        End Get
    End Property
    Public ReadOnly Property MainRegionLanguageCode() As String
        Get
            Return _mainRegionLanguageCode
        End Get
    End Property

    Public Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If Not IsRegionCountry Then Return MyBase.CanWriteProperty(propertyName)
        If IsMainRegionCountry Then Return MyBase.CanWriteProperty(propertyName)

        Select Case propertyName.ToLowerInvariant()
            Case "currency", "steering"
                Return True
            Case Else
                Return False
        End Select

    End Function

    Public Property CanSynchronizeVOMPrices() As Boolean
        Get
            Return _canSynchronizeVOMPrices
        End Get
        Set(value As Boolean)
            If value.Equals(_canSynchronizeVOMPrices) Then Return
            _canSynchronizeVOMPrices = value
            PropertyHasChanged("CanSynchronizeVOMPrices")
        End Set
    End Property
    Public Property CanSynchronizePPOPrices() As Boolean
        Get
            Return _canSynchronizePPOPrices
        End Get
        Set(value As Boolean)
            If value.Equals(_canSynchronizePPOPrices) Then Return
            _canSynchronizePPOPrices = value
            PropertyHasChanged("CanSynchronizePPOPrices")
        End Set
    End Property
    Public Property CanSynchronizeAccessoryPrices() As Boolean
        Get
            Return _canSynchronizeAccessoryPrices
        End Get
        Set(value As Boolean)
            If value.Equals(_canSynchronizeAccessoryPrices) Then Return
            _canSynchronizeAccessoryPrices = value
            PropertyHasChanged("CanSynchronizeAccessoryPrices")
        End Set
    End Property

    Public Property FlatRate() As Decimal
        Get
            Return _flatRate
        End Get
        Set(value As Decimal)
            If value.Equals(_flatRate) Then Return
            _flatRate = value
            PropertyHasChanged("FlatRate")
        End Set
    End Property
#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, RuleHandler), "Currency")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, RuleHandler), "Steering")
        ValidationRules.AddRule(DirectCast(AddressOf RequiredIfNotGlobalOrEurope, RuleHandler), "NmscCode")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Decimal.LesserThanOrEquals, RuleHandler),
                                    New BusinessObjects.ValidationRules.Decimal.LesserThanOrEqualsRuleArgs("FlatRate", 50D, "The flatrate can not be greater than 50%."))
    End Sub

    Private Shared Function RequiredIfNotGlobalOrEurope(ByVal target As Object, ByVal ruleArgs As RuleArgs) As Boolean
        Dim thisCountry = DirectCast(target, Country)
        If thisCountry.Code.Equals("ZZ", StringComparison.InvariantCultureIgnoreCase) OrElse thisCountry.Code.Equals("EU", StringComparison.InvariantCultureIgnoreCase) OrElse Not String.IsNullOrEmpty(thisCountry.NmscCode) Then Return True

        ruleArgs.Description = "NMSC code is required."
        Return False
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return Code.GetHashCode()
    End Function

    Public Overloads Overrides Function Equals(ByVal type As String) As Boolean
        Return String.Compare(Code, type, StringComparison.InvariantCultureIgnoreCase) = 0
    End Function
    Public Overloads Function Equals(ByVal value As Country) As Boolean
        Return Not (value Is Nothing) AndAlso Equals(value.Code)
    End Function
    Public Overloads Function Equals(ByVal value As TMME.Common.DataObjects.Country) As Boolean
        Return Not (value Is Nothing) AndAlso Equals(value.Code)
    End Function

#End Region

#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_languages Is Nothing) AndAlso Not _languages.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_languages Is Nothing) AndAlso _languages.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewCountry(ByVal country As TMME.Common.DataObjects.Country) As Country
        Return New Country(country)
    End Function

#End Region

#Region " Constructors "
    Private Sub New(ByVal country As TMME.Common.DataObjects.Country)
        'Prevent direct creation
        _code = country.Code
        _name = country.Name
        AddBusinessRules()
        MarkAsChild()
    End Sub
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        'Add Data Portal criteria here  
        Private ReadOnly _code As String

        Public Sub New(ByVal code As String)
            _code = code
        End Sub

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@CODE", _code)
        End Sub
    End Class
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        'nothing
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _code = dataReader.GetString("COUNTRY").ToUpper()

        If _code.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase) Then
            ID = New Guid("E097868B-A4AA-47FC-96F2-45DC8376542E") 'fixed guid for global
            _name = "Global"
        Else
            Try
                With TMME.Common.DataObjects.Country.GetCountry(_code)
                    ID = .ID
                    _name = .Name
                End With
            Catch
                _name = _code & " {Unknown Country Code}"
            End Try
        End If

        With dataReader
            _currency = TMME.Common.DataObjects.Currency.GetCurrency(.GetString("CURRENCY"))
            _steering = SteeringInfo.GetSteeringInfo(dataReader)
            _createLocalEquipment = .GetBoolean("CREATELOCALEQUIPMENT")
            _addColourAndTrimToPacks = .GetBoolean("ADDCOLOURANDTRIMTOPACKS")
            _manageAssets = .GetBoolean("MANAGEASSETS")
            _overruleAtCarLevel = .GetBoolean("OVERRULEATCARLEVEL")
            _usesNonVatPrice = .GetBoolean("USESNONVATPRICE")
            _usesXmlImport = .GetBoolean("USESXMLIMPORT")
            _usesBrochures = .GetBoolean("USESBROCHURES")
            _usesEnergyEfficiencySpec = .GetBoolean("USESENERGYEFFICIENCYSPEC")
            _isLocalizedInA2A = .GetBoolean("ISLOCALIZEDINA2A")
            _isRegionCountry = .GetBoolean("ISREGIONCOUNTRY")
            _mainRegionCountryCode = .GetString("REGIONMAINCOUNTRY")
            _mainRegionLanguageCode = .GetString("REGIONMAINLANGUAGE")
            _nmscCode = .GetString("NMSCCODE").Trim()
            _canSynchronizeVOMPrices = .GetBoolean("CANSYNCHRONIZEVOMPRICES")
            _canSynchronizePPOPrices = .GetBoolean("CANSYNCHRONIZEPPOPRICES")
            _canSynchronizeAccessoryPrices = .GetBoolean("CANSYNCHRONIZEACCESSORYPRICES")
            _flatRate = .GetDecimal("ACCESSORYVATFLATRATE")
        End With
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@CODE", Code)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@CODE", Code)
        command.Parameters.AddWithValue("@CURRENCY", Currency.Code)
        command.Parameters.AddWithValue("@STEERINGID", Steering.ID)
        command.Parameters.AddWithValue("@CREATELOCALEQUIPMENT", CreateLocalEquipment)
        command.Parameters.AddWithValue("@ADDCOLOURANDTRIMTOPACKS", AddColourAndTrimToPacks)
        command.Parameters.AddWithValue("@MANAGEASSETS", ManageAssets)
        command.Parameters.AddWithValue("@OVERRULEATCARLEVEL", OverruleAtCarLevel)
        command.Parameters.AddWithValue("@USESNONVATPRICE", UsesNonVATPrice)
        command.Parameters.AddWithValue("@USESXMLIMPORT", UsesXmlImport)
        command.Parameters.AddWithValue("@USESBROCHURES", UsesBrochures)
        command.Parameters.AddWithValue("@USESENERGYEFFICIENCYSPEC", UsesEnergyEfficiencySpecifications)
        command.Parameters.AddWithValue("@ISLOCALIZEDINA2A", Me.LocalizedInA2A)
        command.Parameters.AddWithValue("@NMSCCODE", NmscCode)
        command.Parameters.AddWithValue("@REQUESTEDBY", MyContext.GetContext().Account)
        command.Parameters.AddWithValue("@CANSYNCHRONIZEVOMPRICES", CanSynchronizeVOMPrices)
        command.Parameters.AddWithValue("@CANSYNCHRONIZEPPOPRICES", CanSynchronizePPOPrices)
        command.Parameters.AddWithValue("@CANSYNCHRONIZEACCESSORYPRICES", CanSynchronizeAccessoryPrices)
        command.Parameters.AddWithValue("@ACCESSORYVATFLATRATE", FlatRate)
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'nothing
    End Sub
    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'nothing
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        'nothing
    End Sub


    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not (_languages Is Nothing) Then Languages.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region

#Region " SqlDatabaseContext "
    Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
        Get
            Return New SqlDatabaseContext(False)
        End Get
    End Property

#End Region

End Class
