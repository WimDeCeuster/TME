Imports TME.BusinessObjects.Templates.SqlServer

<Serializable()> Public NotInheritable Class FileTypes
    Inherits ContextReadOnlyListBase(Of FileTypes, FileType)

#Region " Shared Factory Methods "

    Public Shared Function GetFileTypes() As FileTypes
        Return DataPortal.Fetch(Of FileTypes)(New Criteria)
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " SqlDatabaseContext "
    Protected Overrides ReadOnly Property SqlDatabaseContext() As ISqlDatabaseContext
        Get
            Return New SqlDatabaseContext(False)
        End Get
    End Property
#End Region

End Class
<Serializable()> Public NotInheritable Class FileType
    Inherits ContextReadOnlyBase(Of FileType)

#Region " Business Properties & Methods "
    ' Declare variables to contain object state
    ' Declare variables for any child collections
    Private _code As String
    Private _type As String
    Private _description As String

    ' Implement read-only properties and methods for interaction of the UI,
    ' or any other client code, with the object
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Code() As String
        Get
            Return _code
        End Get
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Type() As String
        Get
            Return _type
        End Get
    End Property
    <XmlInfo(XmlNodeType.None)> Public ReadOnly Property Description() As String
        Get
            Return _description
        End Get
    End Property

    Public Function IsImage() As Boolean
        Return Type.Equals("IMAGE", StringComparison.InvariantCultureIgnoreCase)
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Code.ToString()
    End Function

#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Code
    End Function
#End Region


#Region " Shared Factory Methods "

    Friend Shared Function GetFileType(ByVal dataReader As SafeDataReader, ByVal fieldPrefix As String) As FileType
        Dim type As FileType = New FileType
        type.FieldPrefix = fieldPrefix
        type.Fetch(dataReader)
        Return type
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        _code = dataReader.GetString(GetFieldName("CODE")).Trim()
        _description = dataReader.GetString(GetFieldName("DESCRIPTION"))
        _type = dataReader.GetString(GetFieldName("TYPE"))
    End Sub
#End Region

End Class