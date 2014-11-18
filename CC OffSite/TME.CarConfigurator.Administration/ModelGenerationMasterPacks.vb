Imports TME.CarConfigurator.Administration.Enums

<Serializable()> Public NotInheritable Class ModelGenerationMasterPacks
    Inherits ContextUniqueGuidListBase(Of ModelGenerationMasterPacks, ModelGenerationMasterPack)
#Region " Business Properties & Methods "
    Friend Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPack)
        End Get
        Private Set(ByVal value As ModelGenerationPack)
            SetParent(value)
        End Set
    End Property

    Friend Overloads Function Add(ByVal safeDataReader As SafeDataReader) As ModelGenerationMasterPack
        Dim modelGenerationPackMaster As ModelGenerationMasterPack = GetObject(safeDataReader)
        Add(modelGenerationPackMaster)
        Return modelGenerationPackMaster
    End Function
#End Region
#Region " Shared Factory Methods "

    Friend Shared Function GetModelGenerationPackMasterPacks(ByVal modelGenerationPack As ModelGenerationPack) As ModelGenerationMasterPacks
        Dim masterPacks As ModelGenerationMasterPacks
        If modelGenerationPack.IsNew Then
            masterPacks = New ModelGenerationMasterPacks()
        Else
            masterPacks = DataPortal.Fetch(Of ModelGenerationMasterPacks)(New ParentCriteria(modelGenerationPack.ID, "@PACKID"))
        End If
        masterPacks.Pack = modelGenerationPack
        Return masterPacks
    End Function
#End Region
#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        MarkAsChild()
    End Sub
#End Region
End Class
<Serializable()> Public NotInheritable Class ModelGenerationMasterPack
    Inherits ContextUniqueGuidBusinessBase(Of ModelGenerationMasterPack)

#Region " Business Properties & Methods "
    Private _description As String
    Private _type As MasterPackType

    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            If _description.Equals(value) Then Return
            _description = value
            PropertyHasChanged("Description")
        End Set
    End Property

    Public Property Type() As MasterPackType
        Get
            Return _type
        End Get
        Set(value As MasterPackType)
            If value.Equals(_type) Then Return
            _type = value
            PropertyHasChanged("Type")
        End Set
    End Property

    Public ReadOnly Property Pack() As ModelGenerationPack
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationMasterPacks).Pack
        End Get
    End Property

    Public Sub Remove()
        Pack.MasterPacks.Remove(Me)
    End Sub

#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _description = String.Empty
    End Sub

    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _description = .GetString("DESCRIPTION")
            _type = DirectCast(.GetInt16(GetFieldName("TYPE")), MasterPackType)
        End With
        MyBase.FetchFields(dataReader)
        AllowNew = True
        AllowEdit = True
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
        command.Parameters.AddWithValue("@PACKID", Pack.ID)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@PACKID", Pack.ID)
    End Sub

    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@DESCRIPTION", Description)
        command.Parameters.AddWithValue("@TYPE", Type)
    End Sub
#End Region


End Class