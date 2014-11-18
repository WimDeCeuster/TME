Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationGradePacks
    Inherits BaseObjects.ContextUniqueGuidListBase(Of ModelGenerationGradePacks, ModelGenerationGradePack)

#Region " Delegates & Events "
    Friend Delegate Sub PacksChangedHandler(ByVal item As ModelGenerationGradePack)
    Friend Event PackAdded As PacksChangedHandler
    Friend Event PackRemoved As PacksChangedHandler
#End Region

#Region " Business Properties & Methods "

    Friend Property Grade() As ModelGenerationGrade
        Get
            Return DirectCast(Me.Parent, ModelGenerationGrade)
        End Get
        Private Set(ByVal value As ModelGenerationGrade)
            Me.SetParent(value)
            AddHandler value.Generation.Packs.PackAdded, AddressOf OnPackAdded
            AddHandler value.Generation.Packs.PackRemoved, AddressOf OnPackRemoved
        End Set
    End Property
    Private Sub OnPackAdded(ByVal generationPack As ModelGenerationPack)
        If Me.Contains(generationPack.ID) Then Throw New ApplicationException("The item already exists in this collection")

        Dim gradePack As ModelGenerationGradePack = ModelGenerationGradePack.NewModelGenerationGradePack(generationPack)

        Me.AllowNew = True
        Me.Add(gradePack)
        Me.AllowNew = False

        RaiseEvent PackAdded(gradePack)
    End Sub
    Private Sub OnPackRemoved(ByVal generationPack As ModelGenerationPack)
        Dim gradePack As ModelGenerationGradePack = Me(generationPack.ID)

        Me.AllowRemove = True
        gradePack.Remove()
        Me.AllowRemove = False

        RaiseEvent PackRemoved(gradePack)
    End Sub

#End Region

#Region " Copy Method "
    Friend Function Copy(ByVal newGrade As ModelGenerationGrade) As ModelGenerationGradePacks
        Dim clone As ModelGenerationGradePacks = Me.Clone()
        clone.Grade = newGrade
        Return clone
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function GetPacks(ByVal grade As ModelGenerationGrade) As ModelGenerationGradePacks
        Dim packs As ModelGenerationGradePacks = New ModelGenerationGradePacks
        If grade.IsNew Then
            packs.Combine(grade.Generation.Packs, Nothing)
        Else
            packs.Combine(grade.Generation.Packs, DataPortal.Fetch(Of ModelGenerationGradePacks)(New CustomCriteria(grade)))
        End If
        packs.Grade = grade
        Return packs
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.AllowNew = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private ReadOnly _generationID As Guid
        Private ReadOnly _gradeID As Guid

        Public Sub New(ByVal grade As ModelGenerationGrade)
            _generationID = grade.Generation.ID
            _gradeID = grade.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
            command.Parameters.AddWithValue("@GRADEID", _gradeID)
        End Sub

    End Class
#End Region

#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property

    Private Sub OnBeforeUpdateCommand(ByVal obj As System.Data.SqlClient.SqlTransaction) Handles Me.BeforeUpdateCommand
        'Clear the list of deleted objects. 
        'These database will take care of this for us via deleteGrenerationPack
        Me.DeletedList.Clear()
    End Sub


    Private Sub Combine(ByVal generationPacks As IEnumerable(Of ModelGenerationPack), ByVal gradePacks As ModelGenerationGradePacks)
        Me.AllowNew = True
        For Each pack As ModelGenerationPack In generationPacks
            If Not gradePacks Is Nothing AndAlso gradePacks.Contains(pack.ID) Then
                Me.Add(gradePacks(pack.ID))
            Else
                Me.Add(ModelGenerationGradePack.NewModelGenerationGradePack(pack))
            End If
        Next
        Me.AllowNew = False
    End Sub
#End Region


End Class
<Serializable()> Public NotInheritable Class ModelGenerationGradePack
    Inherits BaseObjects.ContextUniqueGuidBusinessBase(Of ModelGenerationGradePack)


#Region " Business Properties & Methods "
    Private _gradeFeature As Boolean = False
    Private _optionalGradeFeature As Boolean = False
    Private _availability As Availability

    <XmlInfo(XmlNodeType.Attribute)> Public Property GradeFeature() As Boolean
        Get
            Return _gradeFeature
        End Get
        Set(ByVal value As Boolean)
            If Not Me.GradeFeature = value Then
                _gradeFeature = value
                PropertyHasChanged("GradeFeature")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property OptionalGradeFeature() As Boolean
        Get
            Return _optionalGradeFeature
        End Get
        Set(ByVal value As Boolean)
            If Not Me.OptionalGradeFeature = value Then
                _optionalGradeFeature = value
                PropertyHasChanged("OptionalGradeFeature")
            End If
        End Set
    End Property
    <XmlInfo(XmlNodeType.Attribute)> Public Property Availability() As Availability
        Get
            Return _availability
        End Get
        Set(ByVal value As Availability)
            If Not value.Equals(Me.Availability) Then
                _availability = value
                PropertyHasChanged("Availability")
            End If
        End Set
    End Property

    Friend Sub Remove()
        Me.AllowRemove = True
        DirectCast(Me.Parent, ModelGenerationGradePacks).Remove(Me)
        Me.AllowRemove = False
    End Sub

#End Region

#Region " Reference Properties & Methods "
    Private _refObject As ModelGenerationPack

    Private ReadOnly Property Grade() As ModelGenerationGrade
        Get
            If Me.Parent Is Nothing Then Return Nothing
            Return DirectCast(Me.Parent, ModelGenerationGradePacks).Grade
        End Get
    End Property
    Friend ReadOnly Property GenerationPack() As ModelGenerationPack
        Get
            If _refObject Is Nothing Then
                If Grade Is Nothing Then Return Nothing
                _refObject = Grade.Generation.Packs(ID)
            End If
            Return _refObject
        End Get
    End Property

    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return GenerationPack.ShortID
        End Get
    End Property
    Public ReadOnly Property Code() As String
        Get
            Return Me.GenerationPack.Code
        End Get
    End Property
    Public ReadOnly Property LocalCode() As String
        Get
            Return Me.GenerationPack.LocalCode
        End Get
    End Property
    Public ReadOnly Property Name() As String
        Get
            Return Me.GenerationPack.Name
        End Get
    End Property
    Public ReadOnly Property Index() As Integer
        Get
            Return Me.GenerationPack.Index
        End Get
    End Property
    Public ReadOnly Property Price() As Decimal
        Get
            Return Me.GenerationPack.Price
        End Get
    End Property
    Public ReadOnly Property VatPrice() As Decimal
        Get
            Return Me.GenerationPack.VatPrice
        End Get
    End Property

    Public ReadOnly Property Translation() As Translations.Translation
        Get
            Return Me.GenerationPack.Translation
        End Get
    End Property
    Public ReadOnly Property AlternateName() As String
        Get
            Return Me.GenerationPack.AlternateName
        End Get
    End Property

    Public ReadOnly Property MasterIDs() As List(Of Guid)
        Get
            Return GenerationPack.MasterIDs
        End Get
    End Property

    Public ReadOnly Property MasterPacks() As ModelGenerationMasterPacks
        Get
            Return GenerationPack.MasterPacks
        End Get
    End Property

    Public ReadOnly Property MasterType() As MasterPackType
        Get
            Return GenerationPack.MasterType
        End Get
    End Property


#End Region


#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationGradePack(ByVal generationPack As ModelGenerationPack) As ModelGenerationGradePack
        Dim pack As ModelGenerationGradePack = New ModelGenerationGradePack()
        pack.Create(generationPack)
        Return pack
    End Function
#End Region

#Region " System.Object Overrides "
    Public Overrides Function Equals(ByVal obj As String) As Boolean
        Return Not (obj Is Nothing) AndAlso Me.GenerationPack.Equals(obj)
    End Function
#End Region
    
#Region " Constructors "
    Private Sub New()
        Me.MarkAsChild()
        Me.AllowNew = False
        Me.AllowRemove = False
    End Sub
#End Region

#Region " Data Access "
    Private Overloads Sub Create(ByVal pack As ModelGenerationPack)
        MyBase.Create(pack.ID)
        _availability = Availability.NotAvailable
        _refObject = pack
        MyBase.MarkOld()
    End Sub

    Protected Overrides Sub FetchSpecializedFields(ByVal dataReader As Common.Database.SafeDataReader)
        ID = dataReader.GetGuid("PACKID")
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _availability = CType(.GetValue("AVAILABILITY"), Availability)
            _gradeFeature = .GetBoolean("GRADEFEATURE")
            _optionalGradeFeature = .GetBoolean("OPTIONALGRADEFEATURE", False)
        End With
        MyBase.FetchFields(dataReader)
    End Sub

    Protected Overrides Sub AddUpdateCommandSpecializedFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateModelGenerationGradePack"
        command.Parameters.AddWithValue("@GENERATIONID", Me.Grade.Generation.ID)
        command.Parameters.AddWithValue("@GRADEID", Me.Grade.ID)
        command.Parameters.AddWithValue("@PACKID", Me.ID)
    End Sub

    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@AVAILABILITY", Me.Availability)
        command.Parameters.AddWithValue("@GRADEFEATURE", Me.GradeFeature)
        command.Parameters.AddWithValue("@OPTIONALGRADEFEATURE", Me.OptionalGradeFeature)
    End Sub
#End Region


End Class
