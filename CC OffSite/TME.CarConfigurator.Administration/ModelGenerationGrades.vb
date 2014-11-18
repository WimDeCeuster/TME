Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class ModelGenerationGrades
    Inherits StronglySortedListBase(Of ModelGenerationGrades, ModelGenerationGrade)

#Region " Business Properties & Methods "
    Friend Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Dim gen = TryCast(Parent, ModelGeneration)
            If Not gen Is Nothing Then Return gen

            Return DirectCast(Parent, ModelGenerationGradeClassification).Generation
        End Get
        Private Set(ByVal value As ModelGeneration)
            SetParent(value)
        End Set
    End Property
    Public Function AddCopy(ByVal source As ModelGenerationGrade) As ModelGenerationGrade
        Dim grade As ModelGenerationGrade = ModelGenerationGrade.GetCopy(Generation, source)
        Add(grade)
        Return grade
    End Function
    Protected Overrides Function CanMove(ByVal grade As ModelGenerationGrade) As Boolean
        If Not grade.CanWriteProperty("Index") Then Return False
        Return True
    End Function
#End Region

#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationGrades(ByVal generation As ModelGeneration) As ModelGenerationGrades
        Dim grades As ModelGenerationGrades = New ModelGenerationGrades()
        grades.Generation = generation
        Return grades
    End Function
    Friend Shared Function GetModelGenerationGrades(ByVal generation As ModelGeneration) As ModelGenerationGrades
        Dim grades As ModelGenerationGrades = DataPortal.Fetch(Of ModelGenerationGrades)(New CustomCriteria(generation))
        grades.Generation = generation
        Return grades
    End Function
    Friend Shared Function GetModelGenerationGrades(ByVal generation As ModelGeneration, ByVal dataReader As SafeDataReader) As ModelGenerationGrades
        Dim grades As ModelGenerationGrades = New ModelGenerationGrades()
        grades.Generation = generation
        grades.Fetch(dataReader)
        Return grades
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid

        Public Sub New(ByVal generation As ModelGeneration)
            _generationID = generation.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Friend Sub Synchronize()
        For Each grade In Me
            Dim availableCars = Generation.Cars.Where(Function(car) car.GradeID.Equals(grade.ID)).ToList()

            If AllowEdit Then
                grade.Activated = availableCars.Any()
                grade.Approved = availableCars.Any(Function(car) car.Approved)
                grade.Preview = availableCars.Any(Function(car) car.Preview)
            End If

            grade.SubModels.Synchronize(availableCars)
            grade.BodyTypes.Synchronize(availableCars)
        Next
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class ModelGenerationGrade
    Inherits LocalizeableBusinessBase
    Implements ISortedIndex
    Implements ISortedIndexSetter
    Implements IUpdatableAssetSet
    Implements IOwnedBy
    Implements IMasterObject

#Region " Business Properties & Methods "
    <NotUndoable(), NoAutoDiscover()> Private _generation As ModelGeneration = Nothing
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _owner As String = String.Empty
    Private _status As Integer
    Private _index As Integer
    Private _basedUponGradeID As Guid = Guid.Empty

    Private _bodyTypes As ModelGenerationGradeBodyTypes
    Private _subModels As ModelGenerationGradeSubModels
    Private WithEvents _equipment As ModelGenerationGradeEquipment
    Private _packs As ModelGenerationGradePacks
    Private _assetSet As AssetSet

    Private _masterID As Guid
    Private _masterDescription As String

    Public Property Generation() As ModelGeneration
        Get
            If _generation Is Nothing Then
                If Parent Is Nothing Then Return Nothing
                _generation = DirectCast(Parent, ModelGenerationGrades).Generation
            End If
            Return _generation
        End Get
        Private Set(ByVal value As ModelGeneration)
            _generation = value
        End Set
    End Property

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code.Equals(value) Then Return

            _code = value
            If AllowEdit Then _localCode = value
            PropertyHasChanged("Code")
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name.Equals(value) Then Return

            _name = value
            PropertyHasChanged("Name")
        End Set
    End Property
    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return _owner
        End Get
    End Property
    Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If _index.Equals(value) Then Return

            _index = value
            PropertyHasChanged("Index")
        End Set
    End Property
    Public Property Activated() As Boolean
        Get
            Return ((_status And Status.AvailableToNmscs) = Status.AvailableToNmscs)
        End Get
        Friend Set(ByVal value As Boolean)
            If value.Equals(Activated) Then Return

            If Activated Then
                _status -= Status.AvailableToNmscs
            Else
                _status += Status.AvailableToNmscs
            End If
            PropertyHasChanged("Activated")
        End Set
    End Property
    Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Approved) Then
                If Approved Then
                    _status -= Status.ApprovedForLive
                    If Not Declined Then _status += Status.Declined
                Else
                    _status += Status.ApprovedForLive
                    If Declined Then _status -= Status.Declined
                End If
                PropertyHasChanged("Approved")
            End If
        End Set
    End Property
    Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Friend Set(ByVal value As Boolean)
            If value.Equals(Declined) Then Return

            If Declined Then
                _status -= Status.Declined
                If Not Approved Then _status += Status.ApprovedForLive
            Else
                _status += Status.Declined
                If Approved Then _status -= Status.ApprovedForLive
            End If
            PropertyHasChanged("Declined")
        End Set
    End Property
    Public Property Preview() As Boolean
        Get
            Return ((_status And Status.ApprovedForPreview) = Status.ApprovedForPreview)
        End Get
        Friend Set(ByVal value As Boolean)
            If value.Equals(Preview) Then Return
            If Preview Then
                _status -= Status.ApprovedForPreview
            Else
                _status += Status.ApprovedForPreview
            End If
            PropertyHasChanged("Preview")
        End Set
    End Property
    Public Property Special() As Boolean
        Get
            Return ((_status And Status.Special) = Status.Special)
        End Get
        Set(ByVal value As Boolean)
            If Special.Equals(value) Then Return

            If Special Then
                _status -= Status.Special
            Else
                _status += Status.Special
            End If
            PropertyHasChanged("Special")
        End Set
    End Property
    Public ReadOnly Property Visible() As Boolean
        Get
            Return Activated AndAlso (Approved OrElse Preview)
        End Get
    End Property

    Public Property BasedUpon() As GradeInfo
        Get
            If _basedUponGradeID.Equals(Guid.Empty) Then Return GradeInfo.Empty
            If Generation.Grades.Contains(_basedUponGradeID) Then Return Generation.Grades(_basedUponGradeID).GetInfo()

            BasedUpon = GradeInfo.Empty
            Return BasedUpon
        End Get
        Set(ByVal value As GradeInfo)
            If _basedUponGradeID.Equals(value.ID) Then Return
            _basedUponGradeID = value.ID
            PropertyHasChanged("BasedUpon")
        End Set
    End Property
    Public Property MasterID() As Guid Implements IMasterObject.MasterID
        Get
            Return _masterID
        End Get
        Set(ByVal value As Guid)
            If _masterID.Equals(value) Then Return

            _masterID = value
            If _masterID.Equals(Guid.Empty) Then _masterDescription = String.Empty
            PropertyHasChanged("MasterID")
            ValidationRules.CheckRules("Master")
        End Set
    End Property
    Public Property MasterDescription() As String Implements IMasterObject.MasterDescription
        Get
            Return _masterDescription
        End Get
        Set(ByVal value As String)
            If _masterDescription.Equals(value) Then Return

            _masterDescription = value
            PropertyHasChanged("MasterDescription")
            ValidationRules.CheckRules("Master")
        End Set
    End Property

    Public ReadOnly Property RefMasterID() As Guid Implements IMasterObjectReference.MasterID
        Get
            Return MasterID
        End Get
    End Property

    Public ReadOnly Property RefMasterDescription() As String Implements IMasterObjectReference.MasterDescription
        Get
            Return MasterDescription
        End Get
    End Property

    Public ReadOnly Property BodyTypes() As ModelGenerationGradeBodyTypes
        Get
            If _bodyTypes Is Nothing Then
                _bodyTypes = ModelGenerationGradeBodyTypes.NewModelGenerationGradeBodyTypes(Me)
            End If
            Return _bodyTypes
        End Get
    End Property
    Public ReadOnly Property SubModels() As ModelGenerationGradeSubModels
        Get
            If _subModels Is Nothing Then
                If IsNew Then
                    _subModels = ModelGenerationGradeSubModels.NewModelGenerationGradeSubModels(Me)
                Else
                    _subModels = ModelGenerationGradeSubModels.GetModelGenerationGradeSubModels(Me)
                End If
            End If
            Return _subModels
        End Get
    End Property
    Friend Sub PrepareSubModelsForEagerLoad()
        _subModels = ModelGenerationGradeSubModels.NewModelGenerationGradeSubModels(Me)
    End Sub
    Friend Sub AddSubModel(ByVal dataReader As SafeDataReader)
        _subModels.Add(dataReader)
    End Sub
    Public ReadOnly Property Equipment() As ModelGenerationGradeEquipment
        Get
            If _equipment Is Nothing Then _equipment = ModelGenerationGradeEquipment.GetEquipment(Me)
            Return _equipment
        End Get
    End Property
    Public ReadOnly Property Packs() As ModelGenerationGradePacks
        Get
            If _packs Is Nothing Then _packs = ModelGenerationGradePacks.GetPacks(Me)
            Return _packs
        End Get
    End Property
    Public ReadOnly Property AssetSet() As AssetSet Implements IHasAssetSet.AssetSet
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

Private Sub MyPropertyChanged(ByVal sender As Object, ByVal e As ComponentModel.PropertyChangedEventArgs) Handles Me.PropertyChanged
        If Owner.Equals(MyContext.GetContext().CountryCode, StringComparison.InvariantCultureIgnoreCase) Then
            If e.PropertyName.Equals("LocalCode", StringComparison.InvariantCultureIgnoreCase) Then _code = LocalCode
        End If
    End Sub
    Public Overloads Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If propertyName.Equals("Activated", StringComparison.InvariantCultureIgnoreCase) Then Return True
        If propertyName.Equals("Approved", StringComparison.InvariantCultureIgnoreCase) Then Return True
        If propertyName.Equals("Declined", StringComparison.InvariantCultureIgnoreCase) Then Return True
        If propertyName.Equals("Preview", StringComparison.InvariantCultureIgnoreCase) Then Return True
        If propertyName.Equals("Special", StringComparison.InvariantCultureIgnoreCase) Then Return True
        If propertyName.Equals("Index", StringComparison.InvariantCultureIgnoreCase) Then Return True
        If propertyName.Equals("BasedUpon", StringComparison.InvariantCultureIgnoreCase) Then Return True
        Return MyBase.CanWriteProperty(propertyName)
    End Function

    Friend Function GetPartialCarSpecification() As PartialCarSpecification
        Dim partialCarSpecification As PartialCarSpecification = partialCarSpecification.NewPartialCarSpecification()
        partialCarSpecification.ModelID = Generation.Model.ID
        partialCarSpecification.GenerationID = Generation.ID
        partialCarSpecification.GradeID = ID
        Return partialCarSpecification
    End Function


    Public Function Cars() As IEnumerable(Of Car)
        Return (From x In Generation.Cars Where x.Grade.Equals(Me))
    End Function

    Public Function GetInfo() As GradeInfo
        Return GradeInfo.GetGradeInfo(Me)
    End Function

   
#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf NotBasedUponMySelf, Validation.RuleHandler), "BasedUpon")
        ValidationRules.AddRule(DirectCast(AddressOf CheckCircularReference, Validation.RuleHandler), "BasedUpon")

        ValidationRules.AddRule(DirectCast(AddressOf Administration.ValidationRules.MasterReference.Valid, Validation.RuleHandler), "Master")
    End Sub

    Private Shared Function NotBasedUponMySelf(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim grade As ModelGenerationGrade = DirectCast(target, ModelGenerationGrade)
        If grade._basedUponGradeID.Equals(grade.ID) Then
            e.Description = "The grade can not be based upon itself."
            Return False
        End If
        Return True
    End Function
    Private Shared Function CheckCircularReference(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim grade As ModelGenerationGrade = DirectCast(target, ModelGenerationGrade)
        Dim gradeId As Guid = grade.ID

        While Not grade._basedUponGradeID.Equals(Guid.Empty)
            If grade.ID.Equals(grade._basedUponGradeID) Then Return True 'Based upon itself, but this is already detected by NotBasedUponMySelf rule..
            If grade._basedUponGradeID.Equals(gradeId) Then
                e.Description = "A circular reference was detected!"
                Return False
            End If
            grade = DirectCast(grade.Parent, ModelGenerationGrades)(grade.BasedUpon.ID)
        End While
        Return True
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationGrade) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As GradeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso ID.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationGrade Then
            Return Equals(DirectCast(obj, ModelGenerationGrade))
        ElseIf TypeOf obj Is GradeInfo Then
            Return Equals(DirectCast(obj, GradeInfo))
        ElseIf TypeOf obj Is String Then
            Return Equals(DirectCast(obj, String))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assetSet Is Nothing) AndAlso Not _assetSet.IsValid Then Return False
            If Not (_bodyTypes Is Nothing) AndAlso Not _bodyTypes.IsValid Then Return False
            If Not (_subModels Is Nothing) AndAlso Not _subModels.IsValid Then Return False
            If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
            If Not (_packs Is Nothing) AndAlso Not _packs.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assetSet Is Nothing) AndAlso _assetSet.IsDirty Then Return True
            If Not (_bodyTypes Is Nothing) AndAlso _bodyTypes.IsDirty Then Return True
            If Not (_subModels Is Nothing) AndAlso _subModels.IsDirty Then Return True
            If Not (_equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
            If Not (_packs Is Nothing) AndAlso _packs.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetCopy(ByVal target As ModelGeneration, ByVal source As ModelGenerationGrade) As ModelGenerationGrade
        Dim targetGrade As ModelGenerationGrade = New ModelGenerationGrade()
        targetGrade.Create()
        targetGrade.Generation = target
        targetGrade._bodyTypes = ModelGenerationGradeBodyTypes.GetCopy(targetGrade, source)
        targetGrade._equipment = source.Equipment.Copy(targetGrade)
        targetGrade._packs = source.Packs.Copy(targetGrade)
        targetGrade._basedUponGradeID = source.ID
        Return targetGrade
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _status = (Status.AvailableToNmscs + Status.Declined)
        _masterID = Guid.Empty
        _masterDescription = String.Empty
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _owner = .GetString("OWNER")
            _status = .GetInt16("STATUSID")
            _index = .GetInt16("SORTORDER")
            _basedUponGradeID = .GetGuid("BASEDUPONGRADEID")
            _masterID = .GetGuid("MASTERID")
            _masterDescription = .GetString("MASTERDESCRIPTION")
            _assetSet = AssetSet.GetAssetSet(Me, dataReader)
        End With
        MyBase.FetchFields(dataReader)
        AllowEdit = (String.Compare(MyContext.GetContext().CountryCode, _owner, True) = 0)
        AllowRemove = AllowEdit

        If AllowEdit And SupportsLocalCode Then
            _localCode = _code
        End If
    End Sub

    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        'If Not GradeGroup Is Nothing Then
        '    command.CommandText = "insertModelGenerationGradeGroupGrade"
        '    command.Parameters.AddWithValue("@GRADEGROUPID", GradeGroup.ID)
        'End If
        AddUpdateCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        'If Not GradeGroup Is Nothing Then
        '    command.CommandText = "updateModelGenerationGradeGroupGrade"
        '    command.Parameters.AddWithValue("@GRADEGROUPID", GradeGroup.ID)
        'End If
        command.Parameters.AddWithValue("@INTERNALCODE", Code)
        command.Parameters.AddWithValue("@LOCALCODE", LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Name)
        command.Parameters.AddWithValue("@GENERATIONID", Generation.ID)
        command.Parameters.AddWithValue("@STATUSID", _status)
        command.Parameters.AddWithValue("@SORTORDER", Index)
        command.Parameters.AddWithValue("@BASEDUPONGRADEID", _basedUponGradeID.GetDbValue())
        If MasterID.Equals(Guid.Empty) Then
            command.Parameters.AddWithValue("@MASTERID", DBNull.Value)
            command.Parameters.AddWithValue("@MASTERDESCRIPTION", DBNull.Value)
        Else
            command.Parameters.AddWithValue("@MASTERID", MasterID)
            command.Parameters.AddWithValue("@MASTERDESCRIPTION", MasterDescription)
        End If
    End Sub

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not (_subModels Is Nothing) Then _subModels.Update(transaction)
        If Not (_bodyTypes Is Nothing) Then _bodyTypes.Update(transaction)
        If Not (_equipment Is Nothing) Then _equipment.Update(transaction)
        If Not (_packs Is Nothing) Then _packs.Update(transaction)
        If Not _assetSet Is Nothing Then _assetSet.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

#Region " Base Object Overrides "

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

    Protected Friend Overrides Function GetBaseCode() As String
        Return Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Name
    End Function
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.MODELGENERATIONGRADE
        End Get
    End Property
#End Region


    Friend Sub CheckRules()
        ValidationRules.CheckRules()
    End Sub
End Class
<Serializable(), XmlInfo("grade")> Public NotInheritable Class GradeInfo

#Region " Business Properties & Methods "
    Private _id As Guid = Guid.Empty
    Private _name As String = String.Empty

    <XmlInfo(XmlNodeType.Attribute)> Public ReadOnly Property ID() As Guid
        Get
            Return _id
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    Public Function IsEmpty() As Boolean
        Return ID.Equals(Guid.Empty)
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function GetHashCode() As Integer
        Return ID.GetHashCode()
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationGrade) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As GradeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As Guid) As Boolean
        Return ID.Equals(obj)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is GradeInfo Then
            Return Equals(DirectCast(obj, GradeInfo))
        ElseIf TypeOf obj Is ModelGenerationGrade Then
            Return Equals(DirectCast(obj, ModelGenerationGrade))
        ElseIf TypeOf obj Is Guid Then
            Return Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function
    Public Overloads Shared Function Equals(ByVal objA As Object, ByVal objB As Object) As Boolean
        If TypeOf objA Is GradeInfo Then
            Return DirectCast(objA, GradeInfo).Equals(objB)
        ElseIf TypeOf objB Is GradeInfo Then
            Return DirectCast(objB, GradeInfo).Equals(objA)
        Else
            Return False
        End If
    End Function

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetGradeInfo(ByVal dataReader As SafeDataReader) As GradeInfo
        Dim gradeInfo As GradeInfo = New GradeInfo
        gradeInfo.Fetch(dataReader)
        Return gradeInfo
    End Function
    Friend Shared Function GetGradeInfo(ByVal grade As ModelGenerationGrade) As GradeInfo
        Dim gradeInfo As GradeInfo = New GradeInfo
        gradeInfo.Fetch(grade)
        Return gradeInfo
    End Function

    Public Shared ReadOnly Property Empty() As GradeInfo
        Get
            Return New GradeInfo
        End Get
    End Property
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "

    Private Sub Fetch(ByVal dataReader As SafeDataReader)
        With dataReader
            _id = .GetGuid("GRADEID")
            _name = .GetString("GRADENAME")
        End With
    End Sub
    Private Sub Fetch(ByVal grade As ModelGenerationGrade)
        With grade
            _id = .ID
            _name = .Name
        End With
    End Sub
#End Region

End Class