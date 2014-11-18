Imports TME.BusinessObjects.Templates.SqlServer
Imports TME.CarConfigurator.Administration.Enums

Namespace BaseObjects
    <Serializable()> Public MustInherit Class ContextUniqueGuidListBase(Of T As ContextUniqueGuidListBase(Of T, TC), TC As Templates.SqlServer.Specialized.Core.UniqueGuidBusinessBase)
        Inherits Templates.SqlServer.Specialized.UniqueGuidBusinessListBase(Of T, TC)

        Public Overrides Function Add() As TC
            Dim c As TC = MyBase.Add()
            If TypeOf c Is TranslateableBusinessBase Then
                Dim entity As Entity = DirectCast(DirectCast(c, Object), TranslateableBusinessBase).Entity
                If entity.HasDefaultTranslation() Then
                    DirectCast(DirectCast(c, Object), TranslateableBusinessBase).Translation.InitializeLabels()
                End If
            End If
            Return c
        End Function
        Public Overrides Function Add(ByVal id As Guid) As TC
            Return MyBase.Add(id)
        End Function

        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

        Friend Overloads Sub Update(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyUpdate(transaction, ChangesToUpdate.All)
        End Sub
        Friend Overloads Sub Update(ByVal transaction As System.Data.SqlClient.SqlTransaction, ByVal changesToUpdate As ChangesToUpdate)
            MyUpdate(transaction, changesToUpdate)
        End Sub

        Protected Overridable Sub MyUpdate(ByVal transaction As System.Data.SqlClient.SqlTransaction, ByVal changesToUpdate As ChangesToUpdate)
            MyBase.Update(transaction, changesToUpdate)
        End Sub

    End Class
    <Serializable()> Public MustInherit Class ContextUniqueGuidBusinessBase(Of T As ContextUniqueGuidBusinessBase(Of T))
        Inherits Templates.SqlServer.Specialized.UniqueGuidBusinessBase(Of T)
        Implements IUniqueGuid


        Protected Overrides ReadOnly Property SqlDatabaseContext() As Templates.SqlServer.ISqlDatabaseContext
            Get
                Return New SqlDatabaseContext()
            End Get
        End Property

        Protected Overrides Sub AddAuditFields(ByVal command As System.Data.SqlClient.SqlCommand)
            'None
        End Sub
        Protected Overrides Sub FetchAuditFields(ByVal dataReader As Common.Database.SafeDataReader)
            CreatedBy = dataReader.GetString(GetFieldName("CREATEDBY"), "{data not retrieved}")
            CreatedOn = dataReader.GetDateTime(GetFieldName("CREATEDON"), DateTime.MinValue)
            ModifiedBy = dataReader.GetString(GetFieldName("MODIFIEDBY"), CreatedBy)
            ModifiedOn = dataReader.GetDateTime(GetFieldName("MODIFIEDON"), ModifiedOn)
        End Sub

        Friend Overloads Sub Update(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.Update(transaction)
        End Sub
        Friend Overloads Sub Delete(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.Delete(transaction)
        End Sub

        Protected Friend Overridable Function GetObjectID() As Guid Implements IUniqueGuid.GetObjectID
            Return ID
        End Function



#If DEBUG Then
        'turn on autocomplete of propertyname when using this method through resharper
        <JetBrains.Annotations.NotifyPropertyChangedInvocator()>
        Protected Overrides Sub PropertyHasChanged(ByVal propertyName As String)
            MyBase.PropertyHasChanged(propertyName)
        End Sub
#End If

    End Class

End Namespace
