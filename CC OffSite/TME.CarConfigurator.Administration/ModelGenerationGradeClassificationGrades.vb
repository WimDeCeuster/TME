Imports TME.CarConfigurator.Administration.Exceptions

<Serializable()> Public NotInheritable Class ModelGenerationGradeClassificationGrades
    Inherits ContextUniqueGuidListBase(Of ModelGenerationGradeClassificationGrades, ModelGenerationGradeClassificationGrade)

#Region " Business Properties & Methods "
    Friend Property GradeClassification() As ModelGenerationGradeClassification
        Get
            If Parent Is Nothing Then Return Nothing
            Dim group = TryCast(Parent, ModelGenerationGradeClassification)
            If Not group Is Nothing Then Return group

            Return DirectCast(Parent, ModelGenerationGradeClassification)
        End Get
        Private Set(ByVal value As ModelGenerationGradeClassification)
            SetParent(value)
        End Set
    End Property

    Public Overloads Function Add(ByVal grade As ModelGenerationGrade) As ModelGenerationGradeClassificationGrade
        If Contains(grade.ID) Then Throw New ObjectAlreadyExists("The item already exists in this collection")

        Dim modelGenerationGrade As ModelGenerationGradeClassificationGrade = ModelGenerationGradeClassificationGrade.NewModelGenerationGradeClassificationGrade(grade)
        Add(modelGenerationGrade)
        Return modelGenerationGrade
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetModelGenerationGradeClassificationGrades(ByVal gradeClassification As ModelGenerationGradeClassification) As ModelGenerationGradeClassificationGrades
        Dim grades As ModelGenerationGradeClassificationGrades = DataPortal.Fetch(Of ModelGenerationGradeClassificationGrades)(New ParentCriteria(gradeClassification.ID, "@GRADECLASSIFICATIONID"))
        grades.GradeClassification = gradeClassification
        Return grades
    End Function
#End Region

End Class

<Serializable()> Public NotInheritable Class ModelGenerationGradeClassificationGrade
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationGradeClassificationGrade)

#Region "Business Properties & Methods"
    Private ReadOnly Property Generation() As ModelGeneration
        Get
            Return Classification.Generation
        End Get
    End Property
    Private ReadOnly Property Classification() As ModelGenerationGradeClassification
        Get
            Return DirectCast(Parent, ModelGenerationGradeClassificationGrades).GradeClassification
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return Generation.Grades(ID).GetInfo().Name
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationGradeClassificationGrade(ByVal grade As ModelGenerationGrade) As ModelGenerationGradeClassificationGrade
        Dim classifiedGrade = New ModelGenerationGradeClassificationGrade()
        classifiedGrade.Create(grade.ID)
        Return classifiedGrade
    End Function
#End Region

#Region " Constructors "

    Public Sub New()
        'Prevent direct creation
        MarkAsChild()
        AllowEdit = False
    End Sub
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Dim hashCode = ID.GetHashCode()
        hashCode = CInt((hashCode * 397) ^ Classification.GetHashCode())
        Return hashCode
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeClassificationGrade) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID) AndAlso Classification.Equals(obj.Classification)
    End Function

    Protected Overrides Function GetIdValue() As Object
        Return ID
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationGradeClassificationGrade Then
            Return Equals(DirectCast(obj, ModelGenerationGradeClassificationGrade))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is ModelGenerationGradeClassificationGrade Then
            Return DirectCast(objA, ModelGenerationGradeClassificationGrade).Equals(objB)
        ElseIf TypeOf objB Is ModelGenerationGradeClassificationGrade Then
            Return DirectCast(objB, ModelGenerationGradeClassificationGrade).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Data Access "

    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As SafeDataReader)
        ID = dataReader.GetGuid("GRADEID")
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddInsertCommandSpecializedFields(ByVal command As SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandSpecializedFields(ByVal command As SqlCommand)
        AddCommandSpecializedFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        AddCommandFields(command)
    End Sub


    Private Sub AddCommandSpecializedFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@GRADEID", ID)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@GRADECLASSIFICATIONID", Classification.ID)
    End Sub
#End Region
End Class
