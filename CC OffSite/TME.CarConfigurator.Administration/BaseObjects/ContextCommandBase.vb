Namespace BaseObjects
    <Serializable()> Friend Class ContextCommand
        Inherits Templates.SqlServer.CommandBase
        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

        Public Shared Function ExecuteScalar(ByVal commandInfo As CommandInfo) As Object
            Return DataPortal.Execute(Of ContextCommand)(New ContextCommand(commandInfo, False)).Result
        End Function

        Public Shared Sub Execute(ByVal commandInfo As CommandInfo)
            DataPortal.Execute(Of ContextCommand)(New ContextCommand(commandInfo, True))
        End Sub


        Private Sub New()
            'prevent direct creation
            'allow dataporttal to create us
        End Sub

        Friend Sub New(ByVal commandInfo As CommandInfo, ByVal noneQueryCommand As Boolean)
            MyBase.Command = commandInfo
            Me.NoneQueryCommand = noneQueryCommand
        End Sub


    End Class
End Namespace
