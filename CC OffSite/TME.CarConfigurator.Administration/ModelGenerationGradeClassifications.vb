Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class ModelGenerationGradeClassifications
    Inherits StronglySortedListBase(Of ModelGenerationGradeClassifications, ModelGenerationGradeClassification)


#Region " Business Properties & Methods "
    Friend Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGeneration)
        End Get
        Private Set(ByVal value As ModelGeneration)
            SetParent(value)
        End Set
    End Property
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetModelGenerationGradeClassifications(ByVal generation As ModelGeneration) As ModelGenerationGradeClassifications
        Dim groups As ModelGenerationGradeClassifications
        If generation.IsNew Then
            groups = New ModelGenerationGradeClassifications()
        Else
            groups = DataPortal.Fetch(Of ModelGenerationGradeClassifications)(New ParentCriteria(generation.ID, "@GENERATIONID"))
        End If
        groups.Generation = generation
        Return groups
    End Function

    Friend Shared Function GetModelGenerationGradeClassifications(ByVal generation As ModelGeneration, ByVal dataReader As SafeDataReader) As ModelGenerationGradeClassifications
        Dim gradeGroups As ModelGenerationGradeClassifications = New ModelGenerationGradeClassifications()
        gradeGroups.Fetch(dataReader)
        gradeGroups.Generation = generation
        Return gradeGroups
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

End Class

<Serializable()> Public NotInheritable Class ModelGenerationGradeClassification
    Inherits TranslateableBusinessBase
    Implements ISortedIndex
    Implements ISortedIndexSetter

#Region " Business Properties & Methods "
    Private _name As String
    Private _index As Integer
    Private _grades As ModelGenerationGradeClassificationGrades

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(value As String)
            If _name <> value Then
                _name = value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property

    Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not CanWriteProperty("Index") Then Exit Property
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property

    Public ReadOnly Property Grades() As ModelGenerationGradeClassificationGrades
        Get
            If _grades Is Nothing Then _grades = ModelGenerationGradeClassificationGrades.GetModelGenerationGradeClassificationGrades(Me)
            Return _grades
        End Get
    End Property

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationGradeClassifications).Generation
        End Get
    End Property


#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")
    End Sub

#End Region

#Region " Framework Overrides "

    Public Overrides ReadOnly Property ModelID() As Guid
        Get
            Return Generation.Model.ID
        End Get
    End Property

    Public Overrides ReadOnly Property GenerationID() As Guid
        Get
            Return Generation.ID
        End Get
    End Property
    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function

    Public Overrides ReadOnly Property Entity() As Entity
        Get
            Return Entity.MODELGENERATIONGRADECLASSIFICATION
        End Get
    End Property


    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_grades Is Nothing) AndAlso Not _grades.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_grades Is Nothing) AndAlso _grades.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeClassification) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function

    Protected Overrides Function GetIdValue() As Object
        Return ID
    End Function

    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationGradeClassification Then
            Return Equals(DirectCast(obj, ModelGenerationGradeClassification))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is ModelGenerationGradeClassification Then
            Return DirectCast(objA, ModelGenerationGradeClassification).Equals(objB)
        ElseIf TypeOf objB Is ModelGenerationGradeClassification Then
            Return DirectCast(objB, ModelGenerationGradeClassification).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _name = String.Empty
        _index = 9999
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _name = .GetString("NAME")
            _index = .GetInt16("SORTORDER")
        End With
        MyBase.FetchFields(dataReader)
        AllowNew = True
        AllowEdit = True
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub

    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@NAME", Name)
        command.Parameters.AddWithValue("@SORTORDER", Index)
        command.Parameters.AddWithValue("@GENERATIONID", Generation.ID)
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _grades Is Nothing Then _grades.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region
End Class

