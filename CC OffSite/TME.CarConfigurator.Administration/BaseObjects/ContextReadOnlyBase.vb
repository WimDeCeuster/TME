Namespace BaseObjects
    <Serializable()> Public MustInherit Class ContextReadOnlyListBase(Of T As ContextReadOnlyListBase(Of T, C), C As Templates.SqlServer.Core.ReadOnlyBase)
        Inherits Templates.SqlServer.ReadOnlyListBase(Of T, C)

        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

    End Class
    <Serializable()> Public MustInherit Class ContextReadOnlyBase(Of T As ContextReadOnlyBase(Of T))
        Inherits Templates.SqlServer.ReadOnlyBase(Of T)

        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

    End Class
End Namespace