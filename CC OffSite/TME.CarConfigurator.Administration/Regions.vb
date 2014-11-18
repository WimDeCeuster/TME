
Imports System.Collections.Generic

<Serializable()> Public NotInheritable Class Region
    Inherits ContextReadOnlyBase(Of Region)

#Region " Business Properties & Methods "

    Private _code As String
    Private _name As String
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
            Return _countries
        End Get
    End Property

    Public Function Languages() As IEnumerable(Of Language)
        Return (
            From country In Countries
            From language In country.Languages
            Select language
            )
    End Function
#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Code
    End Function
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
#End Region

#Region " Shared Factory Methods "

    Public Shared Function GetRegion(ByVal countryCode As String) As Region
        Return DataPortal.Fetch(Of Region)(New CustomCriteria(countryCode))
    End Function

#End Region

#Region " Constructors "
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
            command.Parameters.AddWithValue("@COUNTRY", _code)
        End Sub
    End Class
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString(GetFieldName("CODE"))
            _name = .GetString(GetFieldName("NAME"))
        End With
    End Sub
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        _countries = Countries.GetCountries(dataReader)
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