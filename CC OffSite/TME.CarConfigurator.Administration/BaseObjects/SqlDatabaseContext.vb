Namespace BaseObjects
    Friend Class SqlDatabaseContext
        Implements TME.BusinessObjects.Templates.SqlServer.ISqlDatabaseContext

        ReadOnly _languageCode As String = String.Empty
        ReadOnly _addTrailingParameters As Boolean

        Public ReadOnly Property ConnectionEntity() As String Implements BusinessObjects.Templates.IDatabaseContext.ConnectionEntity
            Get
#If DEBUG Then
                Return Environment.ConnectionEntity & "_DEBUG"
#Else
      Return Environment.ConnectionEntity
#End If
            End Get
        End Property
        Public ReadOnly Property ConnectionScope() As String Implements BusinessObjects.Templates.IDatabaseContext.ConnectionScope
            Get
                Return Environment.ConnectionScope
            End Get
        End Property
        Public ReadOnly Property PrincipalName() As String Implements BusinessObjects.Templates.IDatabaseContext.PrincipalName
            Get
                Return MyContext.GetContext().Account
            End Get
        End Property

        Public ReadOnly Property ConnectionTimeout() As Nullable(Of Integer) Implements BusinessObjects.Templates.IDatabaseContext.ConnectionTimeout
            Get
                Return Environment.ConnectionTimeout
            End Get
        End Property


        Public Sub AddLeadingParameters(ByVal command As System.Data.SqlClient.SqlCommand) Implements BusinessObjects.Templates.SqlServer.ISqlDatabaseContext.AddLeadingParameters
            'none
        End Sub
        Public Sub AddTrailingParameters(ByVal command As System.Data.SqlClient.SqlCommand) Implements BusinessObjects.Templates.SqlServer.ISqlDatabaseContext.AddTrailingParameters
            If Not _addTrailingParameters Then Exit Sub

            If _languageCode.Equals(String.Empty) Then
                MyContext.GetContext().AppendParameters(command)
            Else
                MyContext.GetContext().AppendParameters(command, _languageCode)
            End If
        End Sub

        Public Sub New()
            _addTrailingParameters = True
        End Sub
        Public Sub New(ByVal languageCode As String)
            _languageCode = languageCode
            _addTrailingParameters = True
        End Sub
        Public Sub New(ByVal addTrailingParameters As Boolean)
            _addTrailingParameters = addTrailingParameters
        End Sub
    End Class
End Namespace