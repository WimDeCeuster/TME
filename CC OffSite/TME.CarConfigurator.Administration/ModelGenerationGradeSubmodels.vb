Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationGradeSubModels
    Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelGenerationGradeSubModels, ModelGenerationGradeSubModel)

#Region " Business Properties & Methods "

    Friend Property Grade() As ModelGenerationGrade
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationGrade)
        End Get
        Private Set(ByVal value As ModelGenerationGrade)
            Me.SetParent(value)
        End Set
    End Property
    Private Shadows Sub Add(ByVal subModel As SubModelInfo)
        Dim _subModel As ModelGenerationGradeSubModel = ModelGenerationGradeSubModel.CreateModelGenerationGradeSubModel(Me.Grade, subModel)
        If Not Me.Grade.IsNew AndAlso Entity.MODELGENERATIONGRADESUBMODEL.HasDefaultTranslation() Then
            _subModel.Translation.InitializeLabels()
        End If
        MyBase.Add(_subModel)
    End Sub
    Friend Shadows Sub Add(ByVal dataReader As SafeDataReader)
        Me.AllowNew = True
        MyBase.Add(GetObject(dataReader))
        Me.AllowNew = False
    End Sub

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetModelGenerationGradeSubModels(ByVal grade As ModelGenerationGrade) As ModelGenerationGradeSubModels
        Dim _subModels As ModelGenerationGradeSubModels = DataPortal.Fetch(Of ModelGenerationGradeSubModels)(New ParentCriteria(grade.ID, "@GRADEID"))
        _subModels.Grade = grade
        _subModels.Synchronize()
        Return _subModels
    End Function
    Friend Shared Function NewModelGenerationGradeSubModels(ByVal grade As ModelGenerationGrade) As ModelGenerationGradeSubModels
        Dim _subModels As ModelGenerationGradeSubModels = New ModelGenerationGradeSubModels()
        _subModels.Grade = grade
        Return _subModels
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AllowNew = False
        Me.AllowRemove = False
        Me.AllowEdit = Not MyContext.GetContext().IsSlaveRegionCountry
    End Sub
#End Region

#Region " Data Access "
    Friend Sub Synchronize(Optional ByVal gradeCars As IList(Of Car) = Nothing)
        If Not AllowEdit Then Exit Sub

        If gradeCars Is Nothing Then gradeCars = Grade.Cars.ToList()
        AddMissingObjects(gradeCars)
        RemoveObjectsThatNoLongerExist(gradeCars)
    End Sub
    Private Sub AddMissingObjects(ByVal gradeCars As IEnumerable(Of Car))
        Dim toBeAdded = gradeCars.Where(Function(car) Not car.SubModelID.Equals(Guid.Empty) AndAlso Not Any(Function(x) x.SubModel.ID.Equals(car.SubModelID))).Select(Function(car) car.SubModelID).Distinct().ToList()
        If Not toBeAdded.Any() Then Return

        Dim initialAllowNewValue = AllowNew
        Dim list = Grade.Generation.SubModels

        AllowNew = True
        For Each id In toBeAdded
            Add(list(id).GetInfo())
        Next
        AllowNew = initialAllowNewValue
    End Sub
    Private Sub RemoveObjectsThatNoLongerExist(ByVal gradeCars As IEnumerable(Of Car))
        Dim toBeRemoved = Where(Function(gradeSubModel) Not gradeCars.Any(Function(car) car.SubModelID.Equals(gradeSubModel.SubModel.ID))).ToList()
        Dim initialAllowRemoveValue = AllowRemove

        AllowRemove = True
        For Each obj In toBeRemoved
            obj.Remove()
        Next
        AllowRemove = initialAllowRemoveValue
    End Sub
#End Region

End Class

<Serializable()> Public NotInheritable Class ModelGenerationGradeSubModel
    Inherits BaseObjects.TranslateableBusinessBase
    Implements ISortedIndex
    Implements IUpdatableAssetSet
    Implements IOwnedBy

#Region " Business Properties & Methods "
    Private _subModelId As Guid
    Private _submodel As ModelGenerationSubModel
    Private _assetSet As AssetSet
    Private _owner As String

    Private ReadOnly Property SubmodelID() As Guid
        Get
            Return _subModelId
        End Get
    End Property
    Public ReadOnly Property Grade() As ModelGenerationGrade
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationGradeSubModels).Grade
        End Get
    End Property

    Public ReadOnly Property SubModel() As ModelGenerationSubModel
        Get
            If _submodel Is Nothing Then
                _submodel = Me.Grade.Generation.SubModels(Me.SubmodelID)
            End If
            Return _submodel
        End Get
    End Property

    Public ReadOnly Property Code() As String
        Get
            Return SubModel.Code
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return SubModel.Name
        End Get
    End Property
    Public ReadOnly Property Index() As Integer Implements BaseObjects.ISortedIndex.Index
        Get
            Return SubModel.Index
        End Get
    End Property

    Public Function GetInfo() As SubModelInfo
        Return Me.SubModel.GetInfo()
    End Function

    Public ReadOnly Property Visible() As Boolean
        Get
            Return Me.Grade.Cars.Any(Function(x) x.Approved OrElse x.Preview)
        End Get
    End Property
    Public ReadOnly Property AssetSet() As AssetSet Implements IHAsAssetSet.AssetSet
        Get
            If _assetSet Is Nothing Then
                _assetSet = AssetSet.NewAssetSet(Me)
            End If
            Return _assetSet
        End Get
    End Property

    Public Sub ChangeReference(ByVal updatedAssetSet As AssetSet) Implements IUpdatableAssetSet.ChangeReference
        _assetSet = updatedAssetSet
    End Sub

    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return _owner
        End Get
    End Property

    Friend Sub Remove()
        Me.AllowRemove = True
        DirectCast(Me.Parent, ModelGenerationGradeSubModels).Remove(Me)
    End Sub
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeSubModel) As Boolean
        Return Not (obj Is Nothing) AndAlso (Me.Equals(obj.ID) OrElse (Me.Grade.Equals(obj.Grade) AndAlso Me.SubModel.Equals(obj.SubModel)))
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationSubModel) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.SubModel.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As SubModelInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.SubModel.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationGradeSubModel Then Return Me.Equals(DirectCast(obj, ModelGenerationGradeSubModel))
        If TypeOf obj Is ModelGenerationSubModel Then Return Me.Equals(DirectCast(obj, ModelGenerationSubModel))
        If TypeOf obj Is SubModelInfo Then Return Me.Equals(DirectCast(obj, SubModelInfo))
        If TypeOf obj Is Guid Then Return Me.Equals(DirectCast(obj, Guid))
        Return False
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assetSet Is Nothing) AndAlso Not _assetSet.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assetSet Is Nothing) AndAlso _assetSet.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function CreateModelGenerationGradeSubModel(ByVal grade As ModelGenerationGrade, ByVal submodel As SubModelInfo) As ModelGenerationGradeSubModel
        If grade.IsNew Then
            Dim _subModel As ModelGenerationGradeSubModel = New ModelGenerationGradeSubModel()
            _subModel.Create(grade.ID)
            _subModel._subModelId = submodel.ID
            _subModel._owner = grade.Owner
            _subModel.MarkOld()
            Return _subModel
        Else
            Dim _subModel As ModelGenerationGradeSubModel = DataPortal.Create(Of ModelGenerationGradeSubModel)(New CustomCriteria(grade, submodel))
            _subModel.MarkOld()
            Return _subModel
        End If
    End Function
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _gradeID As Guid
        Private ReadOnly _submodelID As Guid

        Public Sub New(ByVal grade As ModelGenerationGrade, ByVal submodel As SubModelInfo)
            _gradeID = grade.ID
            _submodelID = submodel.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GRADEID", _gradeID)
            command.Parameters.AddWithValue("@SUBMODELID", _submodelID)
        End Sub

    End Class
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AllowNew = False
        Me.AllowEdit = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "

    Protected Overrides Sub CreateFields(ByVal dataReader As Common.Database.SafeDataReader)
        _subModelId = dataReader.GetGuid("SUBMODELID")
        _owner = dataReader.GetString("OWNER")
        _assetSet = AssetSet.GetAssetSet(Me, dataReader)
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _subModelId = dataReader.GetGuid("SUBMODELID")
        _owner = dataReader.GetString("OWNER")
        _assetSet = AssetSet.GetAssetSet(Me, dataReader)
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If _assetSet IsNot Nothing Then _assetSet.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

#Region " Base Object Overrides "
    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function

    Public Overrides ReadOnly Property ModelID() As Guid
        Get
            Return Grade.Generation.Model.ID
        End Get
    End Property

    Public Overrides ReadOnly Property GenerationID() As Guid
        Get
            Return Grade.Generation.ID
        End Get
    End Property

    Public Overrides ReadOnly Property Entity() As Entity
        Get
            Return Entity.MODELGENERATIONGRADESUBMODEL
        End Get
    End Property
#End Region

End Class
