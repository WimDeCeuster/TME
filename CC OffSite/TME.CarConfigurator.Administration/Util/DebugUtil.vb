Namespace Util
    Public Class DebugUtil
        Public Shared Function CurrentConnectionString() As String
            Dim context = New SqlDatabaseContext()
            Dim connectionString = Connection.GetString(context.ConnectionScope, context.ConnectionEntity)
            Return connectionString
        End Function
    End Class
End Namespace