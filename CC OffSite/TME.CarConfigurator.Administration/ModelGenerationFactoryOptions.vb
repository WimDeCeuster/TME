<Serializable()>
Public Class ModelGenerationFactoryOptions
    Inherits ContextUniqueGuidListBase(Of ModelGenerationFactoryOptions, ModelGenerationFactoryOption)

#Region "Shared Factory Methods"
    Friend Shared Function GetModelGenerationFactoryOptions(ByVal factoryGeneration As ModelGenerationFactoryGeneration) As ModelGenerationFactoryOptions
        Return DataPortal.Fetch(Of ModelGenerationFactoryOptions)(New ModelGenerationFactoryOptionsCommandCriteria(factoryGeneration))
    End Function

#Region "Criteria"
    <Serializable()> Protected Class ModelGenerationFactoryOptionsCommandCriteria
        Inherits CommandCriteria
        Private ReadOnly _factoryGenerationId As Guid
        Private ReadOnly _modelGenerationId As Guid

        Public Sub New(ByVal factoryGeneration As ModelGenerationFactoryGeneration)
            _modelGenerationId = factoryGeneration.Generation.ID
            _factoryGenerationId = factoryGeneration.ID
        End Sub

        Public Overrides Sub AddCommandFields(ByVal command As SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _modelGenerationId)
            command.Parameters.AddWithValue("@FACTORYGENERATIONID", _factoryGenerationId)
        End Sub
    End Class
#End Region
#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region

#Region "Data Access"

    Protected Overrides Sub FetchNextResult(ByVal dataReader As SafeDataReader)
        FetchModelGenerationFactoryOptionValues(dataReader)
    End Sub

    Private Sub FetchModelGenerationFactoryOptionValues(ByVal dataReader As SafeDataReader)
        While dataReader.Read()
            Dim optionId As Guid = dataReader.GetGuid("FACTORYGENERATIONOPTIONID")
            'there should always be an option present for the values returned, so the statement below is safe
            Me(optionId).Values.Add(dataReader)
        End While
    End Sub
#End Region

    Friend Property ModelGenerationFactoryGeneration() As ModelGenerationFactoryGeneration
        Get
            Return DirectCast(Parent, ModelGenerationFactoryGeneration)
        End Get
        Set(ByVal value As ModelGenerationFactoryGeneration)
            SetParent(value)
        End Set
    End Property

End Class

<Serializable()>
Public Class ModelGenerationFactoryOption
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationFactoryOption)

#Region "Business Properties & Methods"

    Private _factoryMasterSpec As FactoryMasterSpecInfo
    Private _description As String
    Private _smsCode As String
    Private _specPos As Integer
    Private _values As ModelGenerationFactoryOptionValues

    Public ReadOnly Property FactoryMasterSpec As FactoryMasterSpecInfo
        Get
            Return _factoryMasterSpec
        End Get
    End Property

    Public ReadOnly Property Description As String
        Get
            If _description.Length = 0 Then Return FactoryMasterSpec.Description
            Return _description
        End Get
    End Property
    Public ReadOnly Property SmsCode As String
        Get
            Return _smsCode
        End Get
    End Property

    Public ReadOnly Property SpecPos As Integer
        Get
            Return _specPos
        End Get
    End Property

    Public ReadOnly Property Values As ModelGenerationFactoryOptionValues
        Get
            If _values Is Nothing Then
                _values = ModelGenerationFactoryOptionValues.NewModelGenerationFactoryOptionValues()
                _values.FactoryOption = Me
            End If
            Return _values
        End Get
    End Property

    Public ReadOnly Property FactoryGeneration() As ModelGenerationFactoryGeneration
        Get
            Return DirectCast(Parent, ModelGenerationFactoryOptions).ModelGenerationFactoryGeneration
        End Get
    End Property

#End Region

#Region "Framework Overrides"
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If _values IsNot Nothing AndAlso Not _values.IsValid Then Return False
            Return MyBase.IsValid
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If _values IsNot Nothing AndAlso _values.IsDirty Then Return True
            Return MyBase.IsDirty
        End Get
    End Property
#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
    End Sub
#End Region

#Region "Data Access"

    Protected Overrides Sub FetchFields(ByVal dataReader As SafeDataReader)
        MyBase.FetchFields(dataReader)

        _factoryMasterSpec = FactoryMasterSpecInfo.GetFactoryMasterSpecInfo(dataReader)
        _description = dataReader.GetString("DESCRIPTION").Trim()
        _smsCode = dataReader.GetString("SMSCODE").Trim()
        _specPos = dataReader.GetInt32("SPECPOS")
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As SqlTransaction)
        If Not _values Is Nothing Then _values.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region
End Class