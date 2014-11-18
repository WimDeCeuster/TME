Imports TME.CarConfigurator.Administration.Enums

Namespace BaseObjects
    <Serializable()> Public MustInherit Class TranslateableBusinessBase
        Inherits ContextUniqueGuidBusinessBase(Of TranslateableBusinessBase)
        Implements IEntityObject
        Implements IGenerationObject

#Region " Business Properties & Methods"

        Private _translation As Translations.Translation
        Private _translations As Translations.Translations

        Protected Friend MustOverride Function GetBaseName() As String

        Public Overridable ReadOnly Property ModelID() As Guid
            Get
                Return Guid.Empty
            End Get
        End Property
        Public Overridable ReadOnly Property GenerationID() As Guid Implements IGenerationObject.GenerationID
            Get
                Return Guid.Empty
            End Get
        End Property
        Public MustOverride ReadOnly Property Entity As Entity Implements IEntityObject.Entity

        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property AlternateName() As String
            'this property is required because some controls bind directly or use it for sorting, thus no chance for evaluation
            Get
                If Translation.Name.Length = 0 Then Return BaseName
                If MyContext.GetContext().IsGlobal Then Return BaseName
                Return Translation.Name
            End Get
        End Property
        <XmlInfo(XmlNodeType.None)> Public ReadOnly Property BaseName() As String
            Get
                Return GetBaseName()
            End Get
        End Property
        Public ReadOnly Property Translation() As Translations.Translation
            Get
                If _translation Is Nothing Then
                    If IsNew Then
                        _translation = Administration.Translations.Translation.NewTranslation(Me)
                    Else
                        _translation = Administration.Translations.Translation.GetTranslation(Me)
                    End If
                End If
                Return _translation
            End Get
        End Property
        Public ReadOnly Property Translations() As Translations.Translations
            Get
                If _translations Is Nothing Then _translations = Administration.Translations.Translations.GetTranslations(Me)
                Return _translations
            End Get
        End Property
        Friend Sub Copy(ByVal source As TranslateableBusinessBase)
            If source._translation Is Nothing Then Return
            _translation = Administration.Translations.Translation.Copy(Me, source._translation)
        End Sub
        Friend Sub Copy(ByVal generation As ModelGeneration, ByVal source As TranslateableBusinessBase)
            If source._translation Is Nothing Then Return
            _translation = Administration.Translations.Translation.Copy(generation, Me, source._translation)
        End Sub
#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return AlternateName
        End Function
#End Region

#Region " Framework Overrides "

        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not IsBaseValid Then Return False
                If Not (_translation Is Nothing) AndAlso Not _translation.IsValid Then Return False
                If Not (_translations Is Nothing) AndAlso Not _translations.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If IsBaseDirty Then Return True
                If Not (_translation Is Nothing) AndAlso _translation.IsDirty Then Return True
                If Not (_translations Is Nothing) AndAlso _translations.IsDirty Then Return True
                Return False
            End Get
        End Property

#End Region

#Region " Data Access "
        Friend Function GetFieldPrefix() As String
            Return FieldPrefix
        End Function
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            If dataReader.FieldExists(GetFieldName("LOCALNAME")) Then
                _translation = Administration.Translations.Translation.GetTranslation(Me, dataReader)
            End If
        End Sub
        Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            If Not _translation Is Nothing Then _translation.Update(transaction)
            If Not _translations Is Nothing Then _translations.Update(transaction)
        End Sub
#End Region



    End Class
End Namespace
