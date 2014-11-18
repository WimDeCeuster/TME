Imports TME.BusinessObjects.Templates.SqlServer

Namespace BaseObjects
    <Serializable()> Public MustInherit Class ContextListBase(Of T As ContextListBase(Of T, TC), TC As Templates.SqlServer.Core.BusinessBase)
        Inherits Templates.SqlServer.BusinessListBase(Of T, TC)

        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

        Friend Overloads Sub Update(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.Update(transaction)
        End Sub
        Friend Overloads Sub Update(ByVal transaction As System.Data.SqlClient.SqlTransaction, ByVal changesToUpdate As ChangesToUpdate)
            MyBase.Update(transaction, changesToUpdate)
        End Sub

    End Class
    <Serializable()> Public MustInherit Class ContextBusinessBase(Of T As ContextBusinessBase(Of T))
        Inherits Templates.SqlServer.BusinessBase(Of T)

        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

        Protected Overrides Sub AddAuditFields(ByVal command As System.Data.SqlClient.SqlCommand)
            'None
        End Sub
        Protected Overrides Sub FetchAuditFields(ByVal dataReader As Common.Database.SafeDataReader)
            If Not dataReader.FieldExists(GetFieldName("CREATEDBY")) Then
                Debug.WriteLine(String.Format("Missing AUDIT fields for {0}", Me.GetType().Name))
            End If

            CreatedBy = dataReader.GetString(GetFieldName("CREATEDBY"), "{unkown}")
            CreatedOn = dataReader.GetDateTime(GetFieldName("CREATEDON"), DateTime.MinValue)
            ModifiedBy = dataReader.GetString(GetFieldName("MODIFIEDBY"), CreatedBy)
            ModifiedOn = dataReader.GetDateTime(GetFieldName("MODIFIEDON"), ModifiedOn)
        End Sub

        Friend Overloads Sub Update(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.Update(transaction)
        End Sub

#If DEBUG Then
        'turn on autocomplete of propertyname when using this method through resharper
        <JetBrains.Annotations.NotifyPropertyChangedInvocator()>
        Protected Overrides Sub PropertyHasChanged(ByVal propertyName As String)
            MyBase.PropertyHasChanged(propertyName)
        End Sub
#End If
    End Class
End Namespace
