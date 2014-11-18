Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

Namespace Translations
    <Serializable()> Public NotInheritable Class Labels
        Inherits BaseObjects.ContextUniqueGuidListBase(Of Labels, Label)

#Region " Business Properties & Methods "
        Friend Property Translation() As Translation
            Get
                Return DirectCast(Me.Parent, Translation)
            End Get
            Private Set(ByVal value As Translation)
                Me.SetParent(value)
            End Set
        End Property
#End Region

#Region " Shared Factory Methods "
        Private Shared Function NewLabels(ByVal obj As Translation) As Labels

            Dim _definitions As IEnumerable(Of LabelEntityDefinition) = (
                    From x In LabelDefinitions.GetLabelDefinitions()
                    Where x.Entities.Contains(obj.Entity)
                    Select x.Entities(obj.Entity)
            ).OrderBy(Function(y) y.Index)

            Dim _labels As Labels = New Labels()
            _labels.Translation = obj
            For Each _definition As LabelEntityDefinition In _definitions
                _labels.Add(Label.NewLabel(_definition))
            Next
            Return _labels
        End Function
        Friend Shared Function GetLabels(ByVal obj As Translation) As Labels

            Dim _labels As Labels = Labels.NewLabels(obj)

            'In case the parent is a new object or no labels were defned, then there is no need to go to the database
            If obj.IsNew OrElse _labels.Count = 0 Then Return _labels


            'Go to the database to retrieve the labels
            Dim _dbLabels As Labels = DataPortal.Fetch(Of Labels)(New CustomCriteria(obj))
            For Each _dbLabel As Label In _dbLabels
                _labels(_dbLabel.ID).SetDbLabel(_dbLabel)
            Next

            Return _labels
        End Function
        Friend Shared Function GetLabels(ByVal obj As Translation, ByVal dataReader As SafeDataReader) As Labels
            Dim _labels As Labels = Labels.NewLabels(obj)
            Dim _dbLabels As Labels = New Labels()
            _dbLabels.Fetch(dataReader)

            For Each _dbLabel As Label In _dbLabels
                _labels(_dbLabel.ID).SetDbLabel(_dbLabel)
            Next
            Return _labels
        End Function
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            Private ReadOnly _objectID As Guid = Guid.Empty
            Private ReadOnly _modelID As Guid = Guid.Empty
            Private ReadOnly _generationID As Guid = Guid.Empty
            Private ReadOnly _entity As Entity
            Private ReadOnly _country As String
            Private ReadOnly _language As String

            Public Sub New(ByVal obj As Translation)
                _objectID = obj.ID
                _modelID = obj.ModelID
                _generationID = obj.GenerationID
                _entity = obj.Entity
                _country = obj.Country
                _language = obj.Language

                If _modelID.Equals(Guid.Empty) Then
                    Me.CommandText = "GetLabels"
                Else
                    Me.CommandText = "GetModelGenerationLabels"
                End If
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@OBJECTID", _objectID)
                command.Parameters.AddWithValue("@OBJECTENTITY", _entity.ToString())
                If Not _modelID.Equals(Guid.Empty) Then
                    command.Parameters.AddWithValue("@MODELID", _modelID)
                    command.Parameters.AddWithValue("@GENERATIONID", _generationID)
                End If
                command.Parameters.AddWithValue("@COUNTRY", _country)
                command.Parameters.AddWithValue("@LANGUAGE", _language)
            End Sub
        End Class
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "
        Protected Overrides Function GetObject(ByVal dataReader As Common.Database.SafeDataReader) As Label
            Return MyBase.GetObject(dataReader, False)
        End Function
#End Region
    End Class
    <Serializable()> Public NotInheritable Class Label
        Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of Label)

#Region " Business Properties & Methods "
        Private _definition As LabelDefinition
        Private _entity As Entity
        Private _value As String

        Public ReadOnly Property Definition() As LabelDefinition
            Get
                If _definition Is Nothing Then _definition = LabelDefinition.GetLabelDefinition(Me.ID)
                Return _definition
            End Get
        End Property
        Public ReadOnly Property Mandatory() As Boolean
            Get
                Return Me.Definition.Entities(Me.Entity).Mandatory
            End Get
        End Property
        Private ReadOnly Property Entity() As Entity
            Get
                Return _entity
            End Get
        End Property

        Public Property Value() As String
            Get
                Return _value
            End Get
            Set(ByVal updatedValue As String)
                If _value <> updatedValue Then
                    _value = updatedValue
                    PropertyHasChanged("Value")
                End If
            End Set
        End Property

        Private ReadOnly Property Translation() As Translation
            Get
                Return DirectCast(Me.Parent, Labels).Translation
            End Get
        End Property
        Friend Sub SetDbLabel(ByVal dbLabel As Label)
            Me.Value = dbLabel.Value
            Me.MarkOld()
        End Sub
#End Region

#Region " Business & Validation Rules "
        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf Label.ValueValid, Validation.RuleHandler), "Value")
        End Sub
        Private Shared Function ValueValid(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
            Dim _label As Label = DirectCast(target, Label)

            If _label.Value.Length = 0 AndAlso _label.Definition.Entities(_label.Entity).Mandatory Then
                e.Description = String.Format("You need to supply a value for {0}.", _label.Definition.Name)
                Return False
            End If
            If _label.Value.Length > _label.Definition.MaxSize Then
                e.Description = String.Format("The value for {0} is to long. The maximum allowed characters is {1}.", _label.Definition.Name, _label.Definition.MaxSize)
                Return False
            End If

            Return True
        End Function
#End Region

#Region " Shared Factory Methods "
        Friend Shared Function NewLabel(ByVal entityDefinition As LabelEntityDefinition) As Label
            Dim _label As Label = New Label()
            _label.Create(entityDefinition)
            Return _label
        End Function
#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            Return Me.Definition.Name
        End Function
        Public Overloads Overrides Function Equals(ByVal name As String) As Boolean
            Return String.Compare(Me.ToString(), name, False) = 0
        End Function
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            Me.AllowNew = False
            Me.AllowRemove = False
        End Sub
#End Region

#Region " Data Access "
        Private Shadows Sub Create(ByVal entityDefinition As LabelEntityDefinition)
            ID = entityDefinition.LabelDefinition.ID
            _definition = entityDefinition.LabelDefinition
            _entity = entityDefinition.Entity
            If (entityDefinition.DefaultValue.Length > 0) Then
                AllowNew = True
                _value = entityDefinition.DefaultValue
                MarkNew()
            Else
                _value = String.Empty
                MarkOld()
            End If
            ValidationRules.CheckRules()
        End Sub
        Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
            ID = dataReader.GetGuid("LABELID")
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            _value = dataReader.GetString("VALUE")
            _entity = dataReader.GetEntity("ENTITY")
            MyBase.FetchFields(dataReader)
        End Sub

        Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@LABELID", Me.ID)
        End Sub
        Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@LABELID", Me.ID)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            If Not Me.Translation.ModelID.Equals(Guid.Empty) Then
                command.CommandText = "insertModelGenerationLabel"
            End If
            Me.AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            If Not Me.Translation.ModelID.Equals(Guid.Empty) Then
                command.CommandText = "updateModelGenerationLabel"
            End If
            Me.AddCommandFields(command)
        End Sub
        Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            If Not Me.Translation.ModelID.Equals(Guid.Empty) Then
                command.CommandText = "updateModelGenerationLabel"
                command.Parameters.AddWithValue("@MODELID", Me.Translation.ModelID)
                command.Parameters.AddWithValue("@GENERATIONID", Me.Translation.GenerationID)
            End If
            command.Parameters.AddWithValue("@OBJECTID", Me.Translation.GetObjectID())
            command.Parameters.AddWithValue("@OBJECTENTITY", Me.Translation.Entity)
            command.Parameters.AddWithValue("@VALUE", Me.Value)
            command.Parameters.AddWithValue("@COUNTRY", Me.Translation.Country)
            command.Parameters.AddWithValue("@LANGUAGE", Me.Translation.Language)
        End Sub
#End Region

    End Class
End Namespace
