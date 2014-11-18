Imports System.Collections.Generic

Public Class ModelGenerationQueryResults
    Inherits ContextUniqueGuidReadOnlyListBase(Of ModelGenerationQueryResults, ModelGenerationQueryResult)

#Region "Shared Factory Methods"

    Public Shared Function FindByFactoryGenerationID(ByVal factoryGenerationID As Guid) As IEnumerable(Of ModelGenerationQueryResult)
        Return DataPortal.Fetch(Of ModelGenerationQueryResults)(New FactoryGenerationCriteria(factoryGenerationID))
    End Function

#Region "Criteria"
    Private Class FactoryGenerationCriteria
        Inherits CommandCriteria
        Private ReadOnly _factoryGenerationID As Guid

        Public Sub New(ByVal factoryGenerationID As Guid)
            _factoryGenerationID = factoryGenerationID
        End Sub

        Public Overrides Sub AddCommandFields(ByVal command As SqlCommand)
            command.CommandText = "getModelGenerationsByFactoryGenerationID"
            command.Parameters.AddWithValue("@FACTORYGENERATIONID", _factoryGenerationID)
        End Sub
    End Class
#End Region

#End Region

#Region "Constructors"

    Private Sub New()
        'prevent direct creation
    End Sub

#End Region
End Class

Public Class ModelGenerationQueryResult
    Inherits ContextUniqueGuidReadOnlyBase(Of ModelGenerationQueryResult)

#Region "Business Properties & Methods"
    Private _name As String

    Public ReadOnly Property Name As String
        Get
            Return _name
        End Get
    End Property
#End Region

#Region "Constructors"

    Private Sub New()
        'prevent direct creation
    End Sub

#End Region

#Region "Data Access"
    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)
        _name = dataReader.GetString("SHORTNAME")
    End Sub
#End Region
End Class