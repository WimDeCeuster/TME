Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class BaseLinks
    Inherits BaseObjects.ContextListBase(Of BaseLinks, BaseLink)

#Region " Business Properties & Methods "

    Private _linkType As LinkType
    Private _preview As Boolean

    Public ReadOnly Property Type() As LinkType
        Get
            Return _linkType
        End Get
    End Property
    Public ReadOnly Property Preview() As Boolean
        Get
            Return _preview
        End Get
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal country As String, ByVal language As String) As BaseLink
        Get
            For Each _link As BaseLink In Me
                If String.Compare(_link.CountryCode, country, True) = 0 AndAlso String.Compare(_link.LanguageCode, language, True) = 0 Then
                    Return _link
                End If
            Next
            Return Nothing
        End Get
    End Property
    Public Shadows Function Add(ByVal country As String, ByVal language As String) As BaseLink
        If Me.Contains(country, language) Then Throw New Exceptions.BaseLinkAlreadyExists

        Dim _link As BaseLink = BaseLink.NewBaseLink(country, language)
        MyBase.Add(_link)
        Return _link
    End Function
    Public Shadows Function Contains(ByVal country As String, ByVal language As String) As Boolean
        Return Not (Me(country, language) Is Nothing)
    End Function

#End Region

#Region " Shared Factory Methods "

    Public Shared Function GetBaseLinks(ByVal linkType As LinkType, ByVal preview As Boolean) As BaseLinks
        Dim _links As BaseLinks = DataPortal.Fetch(Of BaseLinks)(New CustomCriteria(linkType, preview))
        _links._linkType = linkType
        _links._preview = preview
        Return _links
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _linkType As Int16
        Private ReadOnly _preview As Boolean

        'Add Data Portal criteria here
        Public Sub New(ByVal linkType As LinkType, ByVal preview As Boolean)
            _linkType = linkType.ID
            _preview = preview
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@LINKTYPE", _linkType)
            command.Parameters.AddWithValue("@PREVIEW", _preview)
        End Sub

    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class BaseLink
    Inherits BaseObjects.ContextBusinessBase(Of BaseLink)

#Region " Business Properties & Methods "

    ' Declare variables to contain object state
    ' Declare variables for any child collections

    Private _country As String = String.Empty
    Private _language As String = String.Empty
    Private _url As String = String.Empty


    Public ReadOnly Property Type() As LinkType
        Get
            If Not Me.Parent Is Nothing Then
                Return DirectCast(Me.Parent, BaseLinks).Type
            End If
            Return Nothing
        End Get
    End Property
    Public ReadOnly Property Preview() As Boolean
        Get
            If Not Me.Parent Is Nothing Then
                Return DirectCast(Me.Parent, BaseLinks).Preview
            End If
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property CountryCode() As String
        Get
            Return _country
        End Get
    End Property
    Public ReadOnly Property LanguageCode() As String
        Get
            Return _language
        End Get
    End Property
    Public Property Url() As String
        Get
            Return _url
        End Get
        Set(ByVal value As String)
            If _url <> value Then
                _url = value
                PropertyHasChanged("Url")
            End If
        End Set
    End Property


#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "Url")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Url", 255))
        ValidationRules.AddRule(DirectCast(AddressOf BaseLink.UrlValid, Validation.RuleHandler), "Url")
    End Sub
    Private Shared Function UrlValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _target As BaseLink = DirectCast(target, BaseLink)
        If _target.Url.Length = 0 Then Return True

        Dim _regEx As Text.RegularExpressions.Regex = New Text.RegularExpressions.Regex("(((http|https)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?", Text.RegularExpressions.RegexOptions.IgnoreCase)
        If _regEx.IsMatch(_target.Url) Then
            e.Description = String.Empty
            Return True
        Else
            e.Description = "The url does not match the regular expression."
            Return False
        End If

    End Function
#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Me.Type.ToString() & Me.Preview.ToString() & Me.CountryCode & Me.LanguageCode
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Url
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewBaseLink(ByVal country As String, ByVal language As String) As BaseLink
        Dim _link As BaseLink = New BaseLink
        _link._country = country
        _link._language = language
        Return _link
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AutoDiscover = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _country = .GetString("COUNTRY")
            _language = .GetString("LANGUAGE")
            _url = .GetString("URL")
        End With
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@LINKTYPE", Me.Type.ID)
        command.Parameters.AddWithValue("@PREVIEW", Me.Preview)
        command.Parameters.AddWithValue("@URL", Me.Url)
        MyContext.GetContext().AppendParameters(command, Me.CountryCode, Me.LanguageCode)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@LINKTYPE", Me.Type.ID)
        command.Parameters.AddWithValue("@PREVIEW", Me.Preview)
        MyContext.GetContext().AppendParameters(command, Me.CountryCode, Me.LanguageCode)
    End Sub

    Protected Overrides ReadOnly Property SqlDatabaseContext() As BusinessObjects.Templates.SqlServer.ISqlDatabaseContext
        Get
            Return New BaseObjects.SqlDatabaseContext(False)
        End Get
    End Property

#End Region

End Class
