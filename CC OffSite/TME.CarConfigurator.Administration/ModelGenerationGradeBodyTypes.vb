Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationGradeBodyTypes
    Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelGenerationGradeBodyTypes, ModelGenerationGradeBodyType)

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

    Friend Shadows Function Add(ByVal bodyType As BodyTypeInfo, ByVal index As Integer) As ModelGenerationGradeBodyType
        Dim newBodyType As ModelGenerationGradeBodyType = ModelGenerationGradeBodyType.GetModelGenerationGradeBodyType(bodyType, index)
        MyBase.Add(newBodyType)
        Return newBodyType
    End Function
    Friend Function AddCopy(ByVal bodyType As ModelGenerationGradeBodyType) As ModelGenerationGradeBodyType
        Dim newBodyType As ModelGenerationGradeBodyType = ModelGenerationGradeBodyType.GetCopy(Me.Grade, bodyType)
        MyBase.Add(newBodyType)
        Return newBodyType
    End Function

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewModelGenerationGradeBodyTypes(ByVal grade As ModelGenerationGrade) As ModelGenerationGradeBodyTypes
        Dim _bodyTypes As ModelGenerationGradeBodyTypes = New ModelGenerationGradeBodyTypes
        _bodyTypes.Grade = grade

        Return _bodyTypes
    End Function
    Friend Shared Function GetCopy(ByVal target As ModelGenerationGrade, ByVal source As ModelGenerationGrade) As ModelGenerationGradeBodyTypes
        Dim _bodyTypes As ModelGenerationGradeBodyTypes = New ModelGenerationGradeBodyTypes
        _bodyTypes.Grade = target
        For Each _bodyType As ModelGenerationGradeBodyType In source.BodyTypes
            _bodyTypes.AddCopy(_bodyType)
        Next
        Return _bodyTypes
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
    End Sub
#End Region

    Sub Synchronize(gradeCars As IList(Of Car))
        AddMissingObjects(gradeCars)

        If Not AllowEdit Then Return
        UpdateObjectStatuses(gradeCars)
    End Sub
    Private Sub AddMissingObjects(gradeCars As IEnumerable(Of Car))
        Dim toBeAdded = gradeCars.Where(Function(car) Not Contains(car.BodyTypeID)).Select(Function(car) car.BodyTypeID).Distinct().ToList()
        If Not toBeAdded.Any() Then Return

        Dim initialAllowNewValue = AllowNew
        Dim list = Grade.Generation.BodyTypes

        AllowNew = True
        For Each id In toBeAdded
            Add(list(id).GetInfo(), list(id).Index)
        Next
        AllowNew = initialAllowNewValue
    End Sub
    Private Sub UpdateObjectStatuses(gradeCars As IList(Of Car))
        For Each bodyType In Me
            Dim availableCars = gradeCars.Where(Function(car) car.BodyTypeID.Equals(bodyType.ID)).ToList()
            bodyType.Approved = availableCars.Any(Function(car) car.Approved)
            bodyType.Preview = availableCars.Any(Function(car) car.Preview)
        Next
    End Sub
End Class
<Serializable()> Public NotInheritable Class ModelGenerationGradeBodyType
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of ModelGenerationGradeBodyType)

#Region " Business Properties & Methods "
    Private _name As String = String.Empty
    Private _index As Integer
    Private _colourCombinations As LinkedColourCombinations
    Private _status As Integer

    Friend ReadOnly Property Generation() As ModelGeneration
        Get
            If Me.Grade Is Nothing Then Return Nothing
            Return Me.Grade.Generation
        End Get
    End Property
    Public ReadOnly Property Grade() As ModelGenerationGrade
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationGradeBodyTypes).Grade
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property
    Public ReadOnly Property Index() As Integer
        Get
            Return _index
        End Get
    End Property
    Public ReadOnly Property ColourCombinations() As LinkedColourCombinations
        Get
            If _colourCombinations Is Nothing Then
                _colourCombinations = LinkedColourCombinations.GetLinkedColourCombinations(Me.Generation, Me.GetPartialCarSpecification())
                AddDelegates()
            End If
            Return _colourCombinations
        End Get
    End Property

    Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Me.Approved) Then
                If Me.Approved Then
                    _status -= Status.ApprovedForLive
                    If Not Me.Declined Then _status += Status.Declined
                Else
                    _status += Status.ApprovedForLive
                    If Me.Declined Then _status -= Status.Declined
                End If
            End If
        End Set
    End Property
    Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Me.Declined) Then
                If Me.Declined Then
                    _status -= Status.Declined
                    If Not Me.Approved Then _status += Status.ApprovedForLive
                Else
                    _status += Status.Declined
                    If Me.Approved Then _status -= Status.ApprovedForLive
                End If
            End If
        End Set
    End Property
    Public Property Preview() As Boolean
        Get
            Return ((_status And Status.ApprovedForPreview) = Status.ApprovedForPreview)
        End Get
        Friend Set(ByVal value As Boolean)
            If Not value.Equals(Me.Preview) Then
                If Me.Preview Then
                    _status -= Status.ApprovedForPreview
                Else
                    _status += Status.ApprovedForPreview
                End If
            End If
        End Set
    End Property
    Public ReadOnly Property Visible() As Boolean
        Get
            Return Me.Approved OrElse Me.Preview
        End Get
    End Property

    Friend Function GetPartialCarSpecification() As PartialCarSpecification
        Return GetPartialCarSpecification(Me.Grade)
    End Function
    Private Function GetPartialCarSpecification(ByVal forGrade As ModelGenerationGrade) As PartialCarSpecification
        Dim _partialCarSpecification As PartialCarSpecification = PartialCarSpecification.NewPartialCarSpecification()
        _partialCarSpecification.ModelID = forGrade.Generation.Model.ID
        _partialCarSpecification.GenerationID = forGrade.Generation.ID
        _partialCarSpecification.GradeID = forGrade.ID
        _partialCarSpecification.BodyTypeID = Me.ID
        Return _partialCarSpecification
    End Function

#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function

    Public Overloads Function Equals(ByVal obj As ModelGenerationGradeBodyType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As ModelGenerationBodyType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As BodyTypeInfo) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Function Equals(ByVal obj As BodyType) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.Equals(obj.ID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        If TypeOf obj Is ModelGenerationGradeBodyType Then
            Return Me.Equals(DirectCast(obj, ModelGenerationGradeBodyType))
        ElseIf TypeOf obj Is ModelGenerationBodyType Then
            Return Me.Equals(DirectCast(obj, ModelGenerationBodyType))
        ElseIf TypeOf obj Is BodyTypeInfo Then
            Return Me.Equals(DirectCast(obj, BodyTypeInfo))
        ElseIf TypeOf obj Is BodyType Then
            Return Me.Equals(DirectCast(obj, BodyType))
        ElseIf TypeOf obj Is Guid Then
            Return Me.Equals(DirectCast(obj, Guid))
        Else
            Return False
        End If
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_colourCombinations Is Nothing) AndAlso Not _colourCombinations.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_colourCombinations Is Nothing) AndAlso _colourCombinations.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetModelGenerationGradeBodyType(ByVal bodyType As BodyTypeInfo, ByVal index As Integer) As ModelGenerationGradeBodyType
        Dim _bodyType As ModelGenerationGradeBodyType = New ModelGenerationGradeBodyType()
        _bodyType.Create(bodyType.ID)
        _bodyType._name = bodyType.Name
        _bodyType._index = index
        _bodyType.MarkOld()
        Return _bodyType
    End Function
    Friend Shared Function GetCopy(ByVal targetGrade As ModelGenerationGrade, ByVal source As ModelGenerationGradeBodyType) As ModelGenerationGradeBodyType
        Dim _target As ModelGenerationGradeBodyType = New ModelGenerationGradeBodyType()
        _target.Create(source.ID)
        _target._name = source.Name
        _target._index = source.Index

        source.RemoveDelegates()
        _target._colourCombinations = source.ColourCombinations.Copy(_target.GetPartialCarSpecification(targetGrade))
        source.AddDelegates()

        _target.AddDelegates()
        _target.MarkOld()
        Return _target
    End Function

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

    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not (_colourCombinations Is Nothing) Then _colourCombinations.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub

#End Region

End Class