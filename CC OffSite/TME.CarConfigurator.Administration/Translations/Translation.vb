Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects.Validation
Imports Rules = TME.BusinessObjects.ValidationRules

Namespace Translations
    <Serializable()> Public Class Translations
        Inherits ContextUniqueGuidListBase(Of Translations, Translation)

#Region " Business Properties & Methods "
        Default Public Shadows ReadOnly Property Item(ByVal country As String, ByVal language As String) As Translation
            Get
                Return SingleOrDefault(Function(x) x.Equals(country, language))
            End Get
        End Property
        Public Shadows Function Contains(ByVal country As String, ByVal language As String) As Boolean
            Return Any(Function(x) x.Equals(country, language))
        End Function
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function GetTranslations(ByVal [object] As TranslateableBusinessBase) As Translations
            Return DataPortal.Fetch(Of Translations)(New CustomCriteria([object]))
        End Function
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            Private ReadOnly _id As Guid = Guid.Empty
            Private ReadOnly _modelID As Guid = Guid.Empty
            Private ReadOnly _generationID As Guid = Guid.Empty
            Private ReadOnly _entity As Entity

            Public Sub New(ByVal [object] As TranslateableBusinessBase)
                _id = [object].ID
                _modelID = [object].ModelID()
                _generationID = [object].GenerationID()
                _entity = [object].Entity

                If _modelID.Equals(Guid.Empty) Then
                    CommandText = "GetTranslations"
                Else
                    CommandText = "GetModelGenerationTranslations"
                End If
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As SqlCommand)
                command.Parameters.AddWithValue("@ID", _id)
                command.Parameters.AddWithValue("@ENTITY", _entity.ToString())
                If Not _modelID.Equals(Guid.Empty) Then
                    command.Parameters.AddWithValue("@MODELID", _modelID)
                    command.Parameters.AddWithValue("@GENERATIONID", _generationID)
                End If
            End Sub
        End Class
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            AllowNew = False
            AllowRemove = False
        End Sub
#End Region

    End Class
    <Serializable()> Public Class Translation
        Inherits ContextUniqueGuidBusinessBase(Of Translation)

#Region " Business Properties & Methods "
        Private _modelID As Guid = Guid.Empty
        Private _generationID As Guid = Guid.Empty
        Private _entity As Entity
        Private _name As String = String.Empty
        Private _toolTip As String = String.Empty
        Private _description As String = String.Empty
        Private _footNote As String = String.Empty
        Private _labels As Labels
        Private _country As String = String.Empty
        Private _language As String = String.Empty


        Public Property Country As String
            Get
                If _country.Length > 0 Then Return _country
                Return MyContext.GetContext().CountryCode
            End Get
            Private Set(value As String)
                _country = value
            End Set
        End Property
        Public Property Language As String
            Get
                If _language.Length > 0 Then Return _language
                Return MyContext.GetContext().LanguageCode
            End Get
            Private Set(value As String)
                _language = value
            End Set
        End Property

        Public ReadOnly Property ModelID() As Guid
            Get
                Return _modelID
            End Get
        End Property
        Public ReadOnly Property GenerationID() As Guid
            Get
                Return _generationID
            End Get
        End Property
        Public ReadOnly Property Entity() As Entity
            Get
                Return _entity
            End Get
        End Property

        Public Property Name() As String
            Get
                Return _name
            End Get
            Set(ByVal value As String)
                If _name <> value Then
                    _name = value
                    PropertyHasChanged("Name")
                End If
            End Set
        End Property
        Public Property ToolTip() As String
            Get
                Return _toolTip
            End Get
            Set(ByVal value As String)
                If _toolTip <> value Then
                    _toolTip = value
                    PropertyHasChanged("ToolTip")
                End If
            End Set
        End Property
        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                If _description <> value Then
                    _description = value
                    PropertyHasChanged("Description")
                End If
            End Set
        End Property
        Public Property FootNote() As String
            Get
                Return _footNote
            End Get
            Set(ByVal value As String)
                If _footNote <> value Then
                    _footNote = value
                    PropertyHasChanged("FootNote")
                End If
            End Set
        End Property
        Public ReadOnly Property Labels() As Labels
            Get
                InitializeLabels()
                Return _labels
            End Get
        End Property

        Friend Sub InitializeLabels()
            If (_labels Is Nothing) Then _labels = Labels.GetLabels(Me)
        End Sub


#End Region

#Region " Business & Validation Rules "

        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, RuleHandler), "Name")

            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, RuleHandler), New Rules.String.MaxLengthRuleArgs("ToolTip", 512))
            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, RuleHandler), New Rules.String.MaxLengthRuleArgs("FootNote", 1024))
        End Sub

#End Region

#Region " Framework Overrides "
        Public Overrides ReadOnly Property IsDirty As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If Not _labels Is Nothing AndAlso _labels.IsDirty Then Return True
                Return False
            End Get
        End Property
        Public Overrides ReadOnly Property IsValid As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If Not _labels Is Nothing AndAlso Not _labels.IsValid Then Return False
                Return True
            End Get
        End Property
#End Region

#Region " System.Object Overrides "

        Public Overloads Overrides Function ToString() As String
            Return Name
        End Function
        Public Overloads Function Equals(ByVal countryCode As String, ByVal languageCode As String) As Boolean
            Return Country.Equals(countryCode, StringComparison.InvariantCultureIgnoreCase) AndAlso
                   Language.Equals(languageCode, StringComparison.InvariantCultureIgnoreCase)
        End Function

#End Region

#Region " Shared Factory Methods "

        Friend Shared Function GetTranslation(ByVal [object] As TranslateableBusinessBase) As Translation
            Return GetTranslation(New CustomCriteria([object]))
        End Function

        Public Shared Function GetBaseTranslation(ByVal [object] As TranslateableBusinessBase) As Translation
            Return GetTranslation([object], Environment.GlobalCountryCode, "EN")
        End Function
        Private Shared Function GetTranslation(ByVal [object] As TranslateableBusinessBase, ByVal country As String, ByVal language As String) As Translation
            Dim criteria As CustomCriteria = New CustomCriteria([object])
            criteria.Country = country
            criteria.Language = language

            Dim result As Translation = GetTranslation(criteria)
            result.Country = country
            result.Language = language
            Return result

        End Function
        Private Shared Function GetTranslation(ByVal criteria As CustomCriteria) As Translation
            Dim result As Translation = DataPortal.Fetch(Of Translation)(criteria)
            result._entity = criteria.Entity
            Return result
        End Function

        Friend Shared Function GetTranslation(ByVal [object] As TranslateableBusinessBase, ByVal dataReader As SafeDataReader) As Translation

            Dim result As Translation = New Translation
            result.FieldPrefix = [object].GetFieldPrefix()
            result.Fetch(dataReader)
            result.ID = [object].GetObjectID()

            If result._modelID.Equals(Guid.Empty) Then
                result._modelID = [object].ModelID()
                result._generationID = [object].GenerationID()
            End If
            result._entity = [object].Entity
            result.MarkAsChild()
            result.MarkOld()
            Return result
        End Function


        Friend Shared Function Copy(ByVal base As TranslateableBusinessBase, ByVal source As Translation) As Translation
            Dim target As Translation = New Translation
            target.ID = base.GetObjectID()
            target._name = source.Name
            target._toolTip = source.ToolTip
            target._description = source.Description
            target._footNote = source.FootNote
            target._modelID = base.ModelID()
            target._generationID = base.GenerationID()
            target._entity = base.Entity
            target.MarkAsChild()

            If source.ID.Equals(target.ID) Then
                target.MarkOld()
            Else
                target.MarkNew()
            End If

            Return target
        End Function
        Friend Shared Function Copy(ByVal generation As ModelGeneration, ByVal base As TranslateableBusinessBase, ByVal source As Translation) As Translation
            Dim target As Translation = New Translation
            target.ID = base.GetObjectID()
            target._name = source.Name
            target._toolTip = source.ToolTip
            target._description = source.Description
            target._footNote = source.FootNote
            target._modelID = generation.Model.ID
            target._generationID = generation.ID
            target._entity = base.Entity
            target.MarkAsChild()

            If source.ID.Equals(target.ID) Then
                target.MarkOld()
            Else
                target.MarkNew()
            End If

            Return target
        End Function

        Friend Shared Function NewTranslation(ByVal base As TranslateableBusinessBase) As Translation
            Dim result As Translation = New Translation
            result.Create()
            result.ID = base.GetObjectID()
            result._modelID = base.ModelID()
            result._generationID = base.GenerationID()
            result._entity = base.Entity
            result._labels = Labels.GetLabels(result)
            result.MarkOld()
            Return result
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow data portal to create us
        End Sub
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            Private ReadOnly _id As Guid = Guid.Empty
            Private ReadOnly _modelID As Guid = Guid.Empty
            Private ReadOnly _generationID As Guid = Guid.Empty
            Public Country As String = String.Empty
            Public Language As String = String.Empty
            Public ReadOnly Entity As Entity

            Public Sub New(ByVal [object] As TranslateableBusinessBase)
                _id = [object].ID
                _modelID = [object].ModelID()
                _generationID = [object].GenerationID()
                Entity = [object].Entity

                If _modelID.Equals(Guid.Empty) Then
                    CommandText = "GetTranslation"
                Else
                    CommandText = "GetModelGenerationTranslation"
                End If
            End Sub

            Public Overloads Overrides Sub AddCommandFields(ByVal command As SqlCommand)
                command.Parameters.AddWithValue("@ID", _id)
                command.Parameters.AddWithValue("@ENTITY", Entity.ToString())
                If Not _modelID.Equals(Guid.Empty) Then
                    command.Parameters.AddWithValue("@MODELID", _modelID)
                    command.Parameters.AddWithValue("@GENERATIONID", _generationID)
                End If
                If Not Country.Equals(String.Empty) Then
                    command.Parameters.AddWithValue("@COUNTRY", Country)
                    command.Parameters.AddWithValue("@LANGUAGE", Language)
                End If
            End Sub
        End Class
#End Region

#Region " Data Access "

        Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As SafeDataReader)
            ID = dataReader.GetGuid(GetFieldName("ID"), Guid.Empty)
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
            With dataReader

                'Only supplied if getting a translation directly
                If .FieldExists(GetFieldName("ENTITY")) Then
                    _entity = dataReader.GetEntity(GetFieldName("ENTITY"))
                End If

                _country = .GetString("COUNTRY", MyContext.GetContext().CountryCode)
                _language = .GetString("LANGUAGE", MyContext.GetContext().LanguageCode)
                _name = .GetString(GetFieldName("LOCALNAME"))
                _toolTip = .GetString(GetFieldName("LOCALTOOLTIP"))
                _description = .GetString(GetFieldName("LOCALDESCRIPTION"))
                _footNote = .GetString(GetFieldName("LOCALFOOTNOTE"))


                If .FieldExists("MODELID") AndAlso .FieldExists("GENERATIONID") Then
                    _modelID = .GetGuid("MODELID")
                    _generationID = .GetGuid("GENERATIONID")
                End If
            End With
        End Sub
        Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
            _labels = Labels.GetLabels(Me, dataReader)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
            command.CommandText = "insert" & GetCommandSuffix()
            AppendCoreParameters(command)
            AppendParameters(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
            command.CommandText = "update" & GetCommandSuffix()
            AppendCoreParameters(command)
            AppendParameters(command)
        End Sub
        Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
            command.CommandText = "delete" & GetCommandSuffix()
            AppendCoreParameters(command)
        End Sub

        Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            If Not _labels Is Nothing Then _labels.Update(transaction)
            MyBase.UpdateChildren(transaction)
        End Sub

        Private Function GetCommandSuffix() As String
            If _modelID.Equals(Guid.Empty) OrElse _generationID.Equals(Guid.Empty) Then
                Return "Translation"
            Else
                Return "ModelGenerationTranslation"
            End If
        End Function
        Private Sub AppendCoreParameters(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@COUNTRY", Country)
            command.Parameters.AddWithValue("@LANGUAGE", Language)
            If Not _modelID.Equals(Guid.Empty) Then
                command.Parameters.AddWithValue("@MODELID", _modelID)
                command.Parameters.AddWithValue("@GENERATIONID", _generationID)
            End If
        End Sub
        Private Sub AppendParameters(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@NAME", Name)
            command.Parameters.AddWithValue("@TOOLTIP", ToolTip)
            command.Parameters.AddWithValue("@DESCRIPTION", Description)
            command.Parameters.AddWithValue("@FOOTNOTE", FootNote)
        End Sub

#End Region

    End Class
End Namespace
