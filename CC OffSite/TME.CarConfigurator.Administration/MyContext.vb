Imports TME.CarConfigurator.Administration.Assets

<Serializable()> Public NotInheritable Class MyContext
    Inherits BusinessBase(Of MyContext)

    Protected Overrides Function GetIdValue() As Object
        Return Account
    End Function

#Region " Business Properties & Methods "
    Private _systemContext As Boolean = False
    Private _account As String = String.Empty
    Private _brand As String = String.Empty
    Private _countryCode As String = String.Empty
    Private _languageCode As String = String.Empty


    Private _modelID As Guid = Guid.Empty
    Private _modelGenerationID As Guid = Guid.Empty
    Private _country As Country


    '<NotUndoable()> Private _permissions As IPermissions

    Public ReadOnly Property SystemContext() As Boolean
        Get
            Return _systemContext
        End Get
    End Property


    Public Property Brand() As String
        Get
            If _brand.Length = 0 Then
                If System.Configuration.ConfigurationManager.AppSettings().AllKeys.Contains("Brand") Then
                    _brand = System.Configuration.ConfigurationManager.AppSettings()("Brand")
                End If
            End If
            Return _brand
        End Get
        Set(ByVal value As String)
            If Not _brand.Equals(value) Then
                _brand = value
                PropertyHasChanged("Brand")
            End If
        End Set
    End Property
    Public ReadOnly Property Account() As String
        Get
            Return _account
        End Get
    End Property
    Public Property CountryCode() As String
        Get
            Return _countryCode
        End Get
        Set(ByVal value As String)
            value = value.ToUpperInvariant()
            If Not _countryCode.Equals(value) Then
                _countryCode = value
                _country = Nothing
                _languageCode = String.Empty
                _steerings = Nothing
                PropertyHasChanged("CountryCode")

            End If
        End Set
    End Property
    Public ReadOnly Property Country() As Country
        Get
            If _country Is Nothing AndAlso Not _countryCode.Equals(String.Empty) Then
                _country = Countries(_countryCode)
            End If

            Return _country
        End Get
    End Property
    Public Property LanguageCode() As String
        Get
            Return _languageCode
        End Get
        Set(ByVal value As String)
            value = value.ToUpperInvariant()
            If Not value.Equals(_languageCode) Then
                _languageCode = value
                PropertyHasChanged("LanguageCode")
            End If
        End Set
    End Property
    Public Property ModelID() As Guid
        Get
            Return _modelID
        End Get
        Set(ByVal value As Guid)
            If Not _modelID.Equals(value) Then
                _modelID = value
                _modelGenerationID = Guid.Empty
                PropertyHasChanged("ModelID")
            End If
        End Set
    End Property
    Public Property ModelGenerationID() As Guid
        Get
            Return _modelGenerationID
        End Get
        Set(ByVal value As Guid)
            If Not _modelGenerationID.Equals(value) Then
                _modelGenerationID = value
                PropertyHasChanged("ModelGenerationID")
            End If
        End Set
    End Property

    Private _modelGeneration As ModelGeneration
    Public Property ModelGeneration As ModelGeneration
        Get
            If ModelGenerationID = Guid.Empty Then Return Nothing
            If _modelGeneration IsNot Nothing AndAlso _modelGeneration.ID = ModelGenerationID Then Return _modelGeneration
            _modelGeneration = ModelGeneration.GetModelGeneration(ModelGenerationID)
            Return _modelGeneration
        End Get
        Set(value As ModelGeneration)
            If _modelGeneration IsNot Nothing AndAlso value IsNot Nothing AndAlso _modelGeneration.Equals(value) Then Return 'stays the same
            If _modelGeneration Is Nothing AndAlso value Is Nothing Then Return 'stays the same

            _modelGeneration = value
            If _modelGeneration IsNot Nothing Then ModelGenerationID = _modelGeneration.ID 'don't set the id to Guid.Empty => modelgeneration nothing means that there's gonna be a 'refetch' of the data, so we need the ID to still be set correctly
            PropertyHasChanged("ModelGeneration")
        End Set
    End Property
    Public ReadOnly Property Currency() As TMME.Common.DataObjects.Currency
        Get
            If Me.Country Is Nothing Then Return Nothing
            Return Me.Country.Currency
        End Get
    End Property
    Public ReadOnly Property CanViewAssets() As Boolean
        Get
            If Me.Country Is Nothing Then Return False
            Return Me.Country.ManageAssets AndAlso (Not Me.IsRegionCountry OrElse Me.IsMainRegionCountry)
        End Get
    End Property
    Public ReadOnly Property CanManageAssets() As Boolean
        Get
            If Me.Country Is Nothing Then Return False
            Return Me.Country.ManageAssets AndAlso (Not Me.IsRegionCountry OrElse Me.IsMainRegionCountry)
        End Get
    End Property
    Public ReadOnly Property CanCreateLocalEquipment() As Boolean
        Get
            If Me.Country Is Nothing Then Return False
            Return Me.Country.CreateLocalEquipment
        End Get
    End Property
    Public ReadOnly Property CanOverruleAtCarLevel() As Boolean
        Get
            If Me.Country Is Nothing Then Return False
            Return Me.Country.OverruleAtCarLevel
        End Get
    End Property
    Public ReadOnly Property UsesNonVATPrice() As Boolean
        Get
            If Me.Country Is Nothing Then Return False
            Return Me.Country.UsesNonVATPrice
        End Get
    End Property
    Public ReadOnly Property IsRegionCountry() As Boolean
        Get
            If Me.Country Is Nothing Then Return False
            Return Me.Country.IsRegionCountry
        End Get
    End Property
    Public ReadOnly Property IsMainRegionCountry() As Boolean
        Get
            If Me.Country Is Nothing Then Return False
            Return Me.Country.IsMainRegionCountry
        End Get
    End Property
    Public ReadOnly Property IsSlaveRegionCountry() As Boolean
        Get
            If Me.Country Is Nothing Then Return False
            Return Me.Country.IsSlaveRegionCountry
        End Get
    End Property
    Public Function IsGlobal() As Boolean
        Return Me.CountryCode.Equals(Environment.GlobalCountryCode, StringComparison.InvariantCultureIgnoreCase)
    End Function
    Public Function IsEurope() As Boolean
        Return Me.CountryCode.Equals(Environment.EuropeCountryCode, StringComparison.InvariantCultureIgnoreCase)
    End Function
    Public Function OnlyCsgAdministrator() As Boolean
        Return (Thread.CurrentPrincipal.IsInRole("CSG Administrator") OrElse Thread.CurrentPrincipal.IsInRole("NMSC ACCESSORY Administrator")) AndAlso Not (Thread.CurrentPrincipal.IsInRole("MKT Administrator") OrElse Thread.CurrentPrincipal.IsInRole("NMSC Administrator") OrElse Thread.CurrentPrincipal.IsInRole("ISG Administrator"))
    End Function

    Friend Sub AppendParameters(ByVal command As SqlCommand)
        With command.Parameters
            If Not .Contains("@COUNTRY") Then .AddWithValue("@COUNTRY", Me.CountryCode.ToUpperInvariant())
            If Not .Contains("@LANGUAGE") Then .AddWithValue("@LANGUAGE", Me.LanguageCode.ToUpperInvariant())
            If Not .Contains("@REQUESTEDBY") Then .AddWithValue("@REQUESTEDBY", Me.Account)
        End With
    End Sub
    Friend Sub AppendParameters(ByVal command As SqlCommand, ByVal alternateLanguageCode As String)
        With command.Parameters
            If Not .Contains("@COUNTRY") Then .AddWithValue("@COUNTRY", Me.CountryCode.ToUpperInvariant())
            If Not .Contains("@LANGUAGE") Then .AddWithValue("@LANGUAGE", alternateLanguageCode.ToUpperInvariant())
            If Not .Contains("@REQUESTEDBY") Then .AddWithValue("@REQUESTEDBY", Me.Account)
        End With
    End Sub
    Friend Sub AppendParameters(ByVal command As SqlCommand, ByVal alternateCountryCode As String, ByVal alternateLanguageCode As String)
        With command.Parameters
            If Not .Contains("@COUNTRY") Then .AddWithValue("@COUNTRY", alternateCountryCode.ToUpperInvariant())
            If Not .Contains("@LANGUAGE") Then .AddWithValue("@LANGUAGE", alternateLanguageCode.ToUpperInvariant())
            If Not .Contains("@REQUESTEDBY") Then .AddWithValue("@REQUESTEDBY", Me.Account)
        End With
    End Sub

#End Region

#Region " Dependencies "

    Private _lazyLoad As Boolean = True
    Private _equipmentGroups As EquipmentGroups
    Private _countries As Countries
    Private _asstTypeGroups As AssetTypeGroups
    Private _steerings As Steerings

    Public Property LazyLoad As Boolean
        Get
            Return _lazyLoad
        End Get
        Set(ByVal value As Boolean)
            _lazyLoad = value
        End Set
    End Property

    Public Property EquipmentGroups() As EquipmentGroups
        Get
            If (_equipmentGroups Is Nothing) Then
                _equipmentGroups = EquipmentGroups.GetEquipmentGroups(Me.LazyLoad)
            End If
            Return _equipmentGroups
        End Get
        Set(ByVal value As EquipmentGroups) 'note that value can be null
            _equipmentGroups = value
        End Set
    End Property

    Friend Function EquipmentGroupsInitialized() As Boolean
        Return Not (_equipmentGroups Is Nothing)
    End Function

    Public Property Countries As Countries
        Get
            _countries = If(_countries, Countries.GetCountries())
            Return _countries
        End Get
        Set(value As Countries)
            _countries = value
        End Set
    End Property
    Public ReadOnly Property AssetTypeGroups() As AssetTypeGroups
        Get
            If _asstTypeGroups Is Nothing Then _asstTypeGroups = AssetTypeGroups.GetAssetTypeGroups()
            Return _asstTypeGroups
        End Get
    End Property
    Public ReadOnly Property Steerings() As Steerings
        Get
            If _steerings Is Nothing Then _steerings = Steerings.FetchSteerings()
            Return _steerings
        End Get
    End Property
#End Region

#Region " Shared Factory Methods "

    Public Shared Function GetContext() As MyContext
        SyncLock ApplicationContext.ClientContext
            If ApplicationContext.ClientContext.Contains("mycontext") Then Return DirectCast(ApplicationContext.ClientContext("mycontext"), MyContext)
            If Not _systemContextInstance Is Nothing Then Return _systemContextInstance

            Dim contextInstance As MyContext = DataPortal.Fetch(Of MyContext)()
            ApplicationContext.ClientContext.Add("mycontext", contextInstance)
            Return contextInstance
        End SyncLock
    End Function
    Public Shared Function ClearContext() As Boolean
        _systemContextInstance = Nothing
        If Not ApplicationContext.ClientContext.Contains("mycontext") Then Return False
        ApplicationContext.ClientContext.Remove("mycontext")
        Return True
    End Function

    <ThreadStatic()> Private Shared _systemContextInstance As MyContext

    Public Shared Function SetSystemContext(ByVal brand As String, ByVal countryCode As String, ByVal languageCode As String) As MyContext
        Thread.CurrentPrincipal = New GenericPrincipal(New GenericIdentity("system"), New String() {"BASE Administrator", "MKT Administrator", "NMSC LTH Administrator", "ISG Administrator", "CSG LTH Administrator", "NMSC Administrator", "NMSC ACCESSORY Administrator", "CSG Administrator", "CORE Administrator"})


        If _systemContextInstance Is Nothing OrElse Not _systemContextInstance.Brand.Equals(brand) OrElse Not _systemContextInstance.CountryCode.Equals(countryCode) OrElse Not _systemContextInstance.LanguageCode.Equals(languageCode) Then
            _systemContextInstance = New MyContext
            _systemContextInstance._systemContext = True
            With _systemContextInstance
                ._brand = brand
                ._account = System.Reflection.Assembly.GetCallingAssembly().FullName()
                ._countryCode = countryCode.ToUpperInvariant()
                ._languageCode = languageCode.ToUpperInvariant()
                .MarkOld()

                If ._countryCode.Equals(String.Empty) Then Throw New Exceptions.InvalidCountryIdentifier
                If ._languageCode.Equals(String.Empty) Then Throw New Exceptions.InvalidLanguageIdentifier
            End With
        End If
        If Not _systemContextInstance.LanguageCode.Equals(languageCode) Then _systemContextInstance._languageCode = languageCode

        If TME.BusinessObjects.ApplicationContext.ClientContext.Contains("mycontext") Then
            TME.BusinessObjects.ApplicationContext.ClientContext("mycontext") = _systemContextInstance
        Else
            TME.BusinessObjects.ApplicationContext.ClientContext.Add("mycontext", _systemContextInstance)
        End If
        Return _systemContextInstance
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "

    Protected Shadows Sub DataPortal_Fetch()

        Dim _profile As TMME.Security.Library.ApplicationProfile = TMME.Security.Library.ApplicationProfile.GetProfile()
        _account = _profile.ToString()
        _countryCode = _profile.Properties("CountryCode").Value.ToUpperInvariant()
        _languageCode = _profile.Properties("LanguageCode").Value.ToUpperInvariant()
        ' bShowDetails = CType(oProfile.Properties("ShowDetails").Value, Boolean)

        Dim _buffer As String = _profile.Properties("ModelID").Value
        If _buffer.Length = 0 Then
            _modelID = Guid.Empty
        Else
            _modelID = New Guid(_buffer)
        End If

        _buffer = _profile.Properties("ModelGenerationID").Value
        If _buffer.Length = 0 Then
            _modelGenerationID = Guid.Empty
        Else
            _modelGenerationID = New Guid(_buffer)
        End If
        MarkOld()
    End Sub
    Protected Overrides Sub DataPortal_Update()
        'Insert or Update object data in the data store
        Dim oProfile As TMME.Security.Library.ApplicationProfile = TMME.Security.Library.ApplicationProfile.GetProfile()
        oProfile.Properties("CountryCode").Value = _countryCode
        oProfile.Properties("LanguageCode").Value = _languageCode
        oProfile.Properties("ModelID").Value = _modelID.ToString()
        oProfile.Properties("ModelGenerationID").Value = _modelGenerationID.ToString()
        'oProfile.Properties("ShowDetails").Value = bShowDetails.ToString()
        oProfile.Save()
    End Sub

#End Region

End Class
