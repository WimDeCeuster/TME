<Serializable> Public Class ModelGenerationGradeOptionPackRelations
    Inherits ContextUniqueGuidListBase(Of ModelGenerationGradeOptionPackRelations, ModelGenerationGradeOptionPackRelation)
#Region "Business Properties & Methods"

    Friend ReadOnly Property GradeOption As ModelGenerationGradeOption
        Get
            Return DirectCast(Parent, ModelGenerationGradeOption)
        End Get
    End Property

    Public Overloads Function Add(ByVal modelGenerationPack As ModelGenerationPack) As ModelGenerationGradeOptionPackRelation
        Dim relation = ModelGenerationGradeOptionPackRelation.GetRelation(modelGenerationPack)
        Add(relation)
        Return relation
    End Function

    Public Overloads Sub Remove(ByVal modelGenerationPack As ModelGenerationPack)
        Remove(modelGenerationPack.ID)
    End Sub
#End Region

#Region "Shared Factory Methods"
    Public Shared Function NewRelations(ByVal gradeOption As ModelGenerationGradeOption) As ModelGenerationGradeOptionPackRelations
        Dim relations = New ModelGenerationGradeOptionPackRelations
        relations.SetParent(gradeOption)
        Return relations
    End Function

    Public Shared Function GetRelations(ByVal modelGenerationGradeOption As ModelGenerationGradeOption) As ModelGenerationGradeOptionPackRelations
        Dim criteria = new OptionRelationsCriteria(modelGenerationGradeOption)
        Dim relations = DataPortal.Fetch(of ModelGenerationGradeOptionPackRelations)(criteria)
        relations.SetParent(modelGenerationGradeOption)
        return relations
    End Function
#End Region
    
#Region " Criteria "
    <Serializable()> Private Class OptionRelationsCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid
        Private ReadOnly _gradeID As Guid
        Private ReadOnly _optionID As Guid

        Public Sub New(ByVal gradeOption As ModelGenerationGradeOption)
            _generationID = gradeOption.grade.Generation.ID
            _gradeID = gradeOption.grade.ID
            _optionID = gradeOption.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
            command.Parameters.AddWithValue("@GRADEID", _gradeID)
            command.Parameters.AddWithValue("@OPTIONID", _optionID)
        End Sub

    End Class
#End Region

#Region "Constructors"

    Private Sub New()
        'prevent direct creation
    End Sub

#End Region
End Class

<Serializable> Public Class ModelGenerationGradeOptionPackRelation
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationGradeOptionPackRelation)
#Region "Business Properties & Methods"

    Private _name As String

    Public ReadOnly Property Name() As String
        Get
            _name = if(_name, GradeOption.Grade.Generation.Packs(ID).Name)
            Return _name
        End Get
    End Property

    Private ReadOnly Property GradeOption As ModelGenerationGradeOption
        Get
            Return DirectCast(Parent, ModelGenerationGradeOptionPackRelations).GradeOption
        End Get
    End Property
#End Region

#Region "Shared Factory Methods"
    Public Shared Function GetRelation(ByVal modelGenerationPack As ModelGenerationPack) As ModelGenerationGradeOptionPackRelation
        Dim relation = New ModelGenerationGradeOptionPackRelation
        relation.ID = modelGenerationPack.ID
        relation._name = modelGenerationPack.Name
        Return relation
    End Function
#End Region

#Region "Constructors"
    Private Sub New()
        'prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region "Data Access"

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        MyBase.AddInsertCommandFields(command)
        AddCommandFields(command)
    End Sub

    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        MyBase.AddDeleteCommandFields(command)
        AddCommandFields(command)
    End Sub

    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("GENERATIONID", GradeOption.Grade.Generation.ID)
        command.Parameters.AddWithValue("GRADEID", GradeOption.Grade.ID)
        command.Parameters.AddWithValue("OPTIONID", GradeOption.ID)
    End Sub

#End Region

End Class