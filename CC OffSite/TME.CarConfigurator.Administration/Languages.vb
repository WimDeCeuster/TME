<Serializable()> Public Class Languages
    Inherits ContextListBase(Of Languages, Language)

#Region " Business Properties & Methods "

    Public Property Country() As Country
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, Country)
        End Get
        Private Set(ByVal value As Country)
            SetParent(value)
        End Set
    End Property

    Public Shadows Function Add(ByVal languageCode As String) As Language
        Dim newLanguage As Language = Language.NewLanguage(languageCode)
        MyBase.Add(newLanguage)
        Return newLanguage
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetLanguages(ByVal country As Country) As Languages
        Dim languages = DataPortal.Fetch(Of Languages)(New CustomCriteria(country.Code))
        languages.Country = country
        Return languages
    End Function

#End Region
    
#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _country As String

        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@COUNTRY", _country)
        End Sub
        Public Sub New(ByVal country As String)
            _country = country
        End Sub
    End Class

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
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
<Serializable()> Public Class Language
    Inherits ContextBusinessBase(Of Language)


#Region " Business Properties & Methods "
    Private _code As String
    Private _name As String
    Private _localName As String
    
    Public ReadOnly Property Country() As Country
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, Languages).Country
        End Get
    End Property
    Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            If String.IsNullOrEmpty(_name) Then SetNames()
            Return _name
        End Get
    End Property
    Public ReadOnly Property LocalName() As String
        Get
            If String.IsNullOrEmpty(_localName) Then SetNames()
            Return _localName
        End Get
    End Property

    Private Sub SetNames()
        Try
            Dim obj = TMME.Common.DataObjects.Language.GetLanguage(Code)
            _name = obj.Name
            _localName = obj.LocalName
        Catch
            _name = String.Format("{0} {{Unkown Language Code}}", Code)
            _localName = _name
        End Try
    End Sub

    Protected Overrides Function GetIdValue() As Object
        Return Code
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
    Public Overloads Function Equals(ByVal value As TMME.Common.DataObjects.Language) As Boolean
        Return Not (value Is Nothing) AndAlso Equals(value.Code)
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewLanguage(ByVal code As String) As Language
        Dim obj As Language = New Language()
        obj._code = code
        Return obj
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        AllowEdit = False
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _code = dataReader.GetString("LANGUAGE").ToUpper()
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@COUNTRY", Country.Code)
        command.Parameters.AddWithValue("@LANGUAGE", Code)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@COUNTRY", Country.Code)
        command.Parameters.AddWithValue("@LANGUAGE", Code)
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