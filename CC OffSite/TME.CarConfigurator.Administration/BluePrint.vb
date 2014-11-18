Public Class BluePrint
    Public Shared Sub Start(ByVal generation As ModelGeneration, ByVal asynchronous As Boolean)
        If asynchronous Then
            Dim _brand As String = MyContext.GetContext().Brand
            Dim _country As String = MyContext.GetContext().CountryCode
            Dim _language As String = MyContext.GetContext().LanguageCode

            ThreadPool.QueueUserWorkItem(Function(s) BluePrint.SetContextAndStart(generation, _brand, _country, _language))
        Else
            BaseObjects.ContextCommand.Execute(New SignalStartBluePrintCommand(generation))
            BaseObjects.ContextCommand.Execute(New StartBluePrintCommand(generation))
        End If
    End Sub
    Private Shared Function SetContextAndStart(ByVal generation As ModelGeneration, ByVal brand As String, ByVal country As String, ByVal language As String) As Boolean
        MyContext.SetSystemContext(brand, country, language)
        BluePrint.Start(generation, False)
        MyContext.ClearContext()
    End Function

    <Serializable()> Private Class SignalStartBluePrintCommand
        Inherits BaseObjects.ContextCommand.CommandInfo

        Private ReadOnly _id As Guid
        Public Sub New(ByVal generation As ModelGeneration)
            _id = generation.ID
        End Sub
        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "bluePrintGenerationSignal"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _id)
        End Sub
    End Class
    <Serializable()> Private Class StartBluePrintCommand
        Inherits BaseObjects.ContextCommand.CommandInfo

        Private ReadOnly _id As Guid
        Public Sub New(ByVal generation As ModelGeneration)
            _id = generation.ID
        End Sub
        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "bluePrintGeneration"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _id)
        End Sub
    End Class

End Class
