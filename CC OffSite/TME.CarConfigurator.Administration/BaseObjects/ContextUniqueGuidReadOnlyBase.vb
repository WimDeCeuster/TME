Namespace BaseObjects
    <Serializable()> Public MustInherit Class ContextUniqueGuidReadOnlyListBase(Of T As ContextUniqueGuidReadOnlyListBase(Of T, C), C As Templates.SqlServer.Specialized.Core.UniqueGuidReadOnlyBase)
        Inherits Templates.SqlServer.Specialized.UniqueGuidReadOnlyListBase(Of T, C)

        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

    End Class
    <Serializable()> Public MustInherit Class ContextUniqueGuidReadOnlyBase(Of T As ContextUniqueGuidReadOnlyBase(Of T))
        Inherits Templates.SqlServer.Specialized.UniqueGuidReadOnlyBase(Of T)

        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

    End Class
End Namespace