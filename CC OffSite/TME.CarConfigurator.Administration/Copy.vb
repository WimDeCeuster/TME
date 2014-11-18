Public Class Copy
    Public Shared Sub Start(ByVal generation As ModelGeneration, ByVal sourceCountry As String, ByVal asynchronous As Boolean)
        If asynchronous Then
            Dim _brand As String = MyContext.GetContext().Brand
            Dim _country As String = MyContext.GetContext().CountryCode
            Dim _language As String = MyContext.GetContext().LanguageCode

            ThreadPool.QueueUserWorkItem(Function(s) SetContextAndStart(generation, sourceCountry, _brand, _country, _language))
        Else
            ContextCommand.Execute(New SignalStartCopyCommand(generation))
            ContextCommand.Execute(New StartCopyCommand(generation, sourceCountry))
        End If
    End Sub
    Private Shared Function SetContextAndStart(ByVal generation As ModelGeneration, ByVal sourceCountry As String, ByVal brand As String, ByVal country As String, ByVal language As String) As Boolean
        MyContext.SetSystemContext(brand, country, language)
        Start(generation, sourceCountry, False)
        MyContext.ClearContext()
    End Function

    <Serializable()> Private Class SignalStartCopyCommand
        Inherits BaseObjects.ContextCommand.CommandInfo

        Private ReadOnly _id As Guid
        Public Sub New(ByVal generation As ModelGeneration)
            _id = generation.ID
        End Sub
        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "copyGenerationSignal"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _id)
        End Sub
    End Class
    <Serializable()> Private Class StartCopyCommand
        Inherits BaseObjects.ContextCommand.CommandInfo

        Private ReadOnly _id As Guid
        Private ReadOnly _sourceCountry As String

        Public Sub New(ByVal generation As ModelGeneration, ByVal sourceCountry As String)
            _id = generation.ID
            _sourceCountry = sourceCountry
        End Sub
        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "copyGeneration"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _id)
            command.Parameters.AddWithValue("@SOURCECOUNTRY", _sourceCountry)
        End Sub
    End Class
End Class
