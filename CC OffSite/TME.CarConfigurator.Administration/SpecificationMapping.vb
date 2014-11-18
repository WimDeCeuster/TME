Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class SpecificationMapping
    Inherits BaseObjects.StronglySortedListBase(Of SpecificationMapping, SpecificationMappingLine)

#Region " Business Properties & Methods "
    Friend Property Specification() As Specification
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, Specification)
        End Get
        Private Set(ByVal value As Specification)
            Me.SetParent(value)
            Me.AllowEdit = value.AllowRemove
            Me.AllowNew = value.AllowRemove
            Me.AllowRemove = value.AllowRemove

            For Each _line As SpecificationMappingLine In Me
                _line.SetSecurityRights()
            Next

        End Set
    End Property

    Public Overloads Overrides Function MoveDown(ByVal mappingLine As SpecificationMappingLine) As Boolean
        If Not mappingLine.AllowEdit Then Throw New System.Security.SecurityException("Move action not allowed")
        If CanMoveDown(mappingLine) Then
            Me.Swap(mappingLine, (Me.ElementAt(Me.IndexOf(mappingLine) + 1)))
            Return True
        Else
            Return False
        End If
    End Function
    Public Overloads Overrides Function MoveUp(ByVal mappingLine As SpecificationMappingLine) As Boolean
        If Not mappingLine.AllowEdit Then Throw New System.Security.SecurityException("Move action not allowed")
        If CanMoveUp(mappingLine) Then
            Me.Swap(mappingLine, (Me.ElementAt(Me.IndexOf(mappingLine) - 1)))
            Return True
        Else
            Return False
        End If
    End Function
    Private Overloads Sub Swap(ByVal lineA As SpecificationMappingLine, ByVal lineB As SpecificationMappingLine)
        Dim _a As SpecificationMappingLine = lineA.Clone()
        Dim _b As SpecificationMappingLine = lineB.Clone()

        lineA.MasterID = Nothing
        lineB.MasterID = Nothing

        lineA.MasterID = _b.MasterID
        lineA.ImportFormula = _b.ImportFormula

        lineB.MasterID = _a.MasterID
        lineB.ImportFormula = _a.ImportFormula
    End Sub

    Friend Overloads Sub Add(ByVal dataReader As SafeDataReader)
        RaiseListChangedEvents = False
        Me.AllowNew = True
        MyBase.Add(GetObject(dataReader, True))
        Me.AllowNew = False
        RaiseListChangedEvents = True
    End Sub

#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewSpecificationMapping(ByVal specification As Specification) As SpecificationMapping
        Dim _mapping As SpecificationMapping = New SpecificationMapping()
        _mapping.Specification = specification
        Return _mapping
    End Function
    Friend Shared Function GetSpecificationMapping(ByVal specification As Specification) As SpecificationMapping
        Dim _mapping As SpecificationMapping = DataPortal.Fetch(Of SpecificationMapping)(New CustomCriteria(specification))
        _mapping.Specification = specification
        Return _mapping
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _specificationID As Guid

        Public Sub New(ByVal specification As Specification)
            _specificationID = specification.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@TECHSPECID", _specificationID)
        End Sub
    End Class
#End Region


End Class
<Serializable()> Public Class SpecificationMappingLine
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of SpecificationMappingLine)
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter

#Region " Business Properties & Methods "
    Private _specMasterID As Integer
    Private _index As Integer = Integer.MaxValue
    Private _importFormula As String = String.Empty

    Public Property MasterID() As Integer
        Get
            Return _specMasterID
        End Get
        Set(ByVal value As Integer)
            If _specMasterID.Equals(value) Then Return

            _specMasterID = value
            PropertyHasChanged("MasterID")
        End Set
    End Property
    Public ReadOnly Property Index() As Integer Implements BaseObjects.ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements BaseObjects.ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property
    Public Property ImportFormula() As String
        Get
            Return _importFormula
        End Get
        Set(ByVal value As String)
            If _importFormula <> value Then
                _importFormula = value
                PropertyHasChanged("ImportFormula")
            End If
        End Set
    End Property

    Private ReadOnly Property Specification() As Specification
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, SpecificationMapping).Specification
        End Get
    End Property

    Friend Sub SetSecurityRights()
        With DirectCast(Me.Parent, SpecificationMapping)
            Me.AllowNew = .AllowNew
            Me.AllowEdit = .AllowEdit
            Me.AllowRemove = .AllowRemove
        End With
    End Sub

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Object.Required, Validation.RuleHandler), "MasterID")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "MasterID")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("ImportFormula", 80))
    End Sub
#End Region

#Region " System.Object Overrides "


    Public Overloads Function Equals(ByVal obj As SpecificationMappingLine) As Boolean
        If obj Is Nothing Then Return False
        Return Me.Equals(obj.MasterID)
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean
        Dim _specificationMappingLine = TryCast(obj, SpecificationMappingLine)
        If (_specificationMappingLine IsNot Nothing) Then
            Return Me.Equals(_specificationMappingLine)
        End If
        Return False
    End Function
#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AutoDiscover = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _index = .GetInt16(GetFieldName("INDEX"))
            _specMasterID = .GetInt32(GetFieldName("TECHSPECCODE"))
            _importFormula = .GetString(GetFieldName("IMPORTFORMULA"))
        End With
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@TECHSPECID", Me.Specification.ID)
        command.Parameters.AddWithValue("@INDEX", Me.Index)
        command.Parameters.AddWithValue("@TECHSPECCODE", Me.MasterID)
        command.Parameters.AddWithValue("@IMPORTFORMULA", Me.ImportFormula)
    End Sub

#End Region

End Class