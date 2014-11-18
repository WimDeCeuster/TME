Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Links
    Inherits BaseObjects.ContextListBase(Of Links, Link)

#Region " Business Properties & Methods "

    Private _parentId As Guid = Guid.Empty

    Friend Property ParentID() As Guid
        Get
            Return _parentId
        End Get
        Private Set(ByVal value As Guid)
            _parentId = value
        End Set
    End Property

    Default Public Overloads ReadOnly Property Item(ByVal type As LinkType) As Link
        Get
            For Each _link As Link In Me
                If _link.Type.Equals(type) Then
                    Return _link
                End If
            Next
            Return Nothing
        End Get
    End Property

    Public Shadows Function Add(ByVal type As LinkType) As Link
        If Me.Contains(type) Then Throw New Exceptions.ObjectAlreadyExists("link", type)

        Dim _link As Link = Link.NewLink(type)
        MyBase.Add(_link)
        Return _link
    End Function
    Public Shadows Function Contains(ByVal type As LinkType) As Boolean
        Return Not (Me(type) Is Nothing)
    End Function

#End Region

#Region " Shared Factory Methods "
    Private Shared Function NewLinks(ByVal parent As Object, ByVal id As Guid) As Links
        Dim _links As Links = New Links
        _links.SetParent(parent)
        _links.ParentID = id
        Return _links
    End Function


    Friend Shared Function GetLinks(ByVal obj As Model) As Links
        If obj.IsNew Then Return NewLinks(obj, obj.ID)

        Dim _links As Links = DataPortal.Fetch(Of Links)(New CustomCriteria(obj.ID, Entity.MODEL))
        _links.SetParent(obj)
        _links.ParentID = obj.ID
        Return _links
    End Function
    Friend Shared Function GetLinks(ByVal obj As ActiveFilterTool.ModelSubSet) As Links
        If obj.IsNew Then Return NewLinks(obj, obj.ID)

        Dim _links As Links = DataPortal.Fetch(Of Links)(New CustomCriteria(obj.ID, obj.Model.ID, Entity.MODELSUBSET))
        _links.SetParent(obj)
        _links.ParentID = obj.ID
        Return _links
    End Function
    Friend Shared Function GetLinks(ByVal obj As ModelGenerationSubModel) As Links
        If obj.IsNew Then Return NewLinks(obj, obj.ID)

        Dim _links As Links = DataPortal.Fetch(Of Links)(New CustomCriteria(obj.ID, obj.Generation.Model.ID, Entity.SUBMODEL))
        _links.SetParent(obj)
        _links.ParentID = obj.ID
        Return _links
    End Function
    Friend Shared Function GetLinks(ByVal obj As EquipmentItem) As Links
        If obj.IsNew Then Return NewLinks(obj, obj.ID)

        Dim _links As Links = DataPortal.Fetch(Of Links)(New CustomCriteria(obj.ID, Entity.EQUIPMENT))
        _links.SetParent(obj)
        _links.ParentID = obj.ID
        Return _links
    End Function
    Friend Shared Function GetLinks(ByVal obj As Promotions.Promotion) As Links
        If obj.IsNew Then Return NewLinks(obj, obj.ID)

        Dim _links As Links = DataPortal.Fetch(Of Links)(New CustomCriteria(obj.ID, Entity.PROMOTION))
        _links.SetParent(obj)
        _links.ParentID = obj.ID
        Return _links
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        'Allow data portal to create us
        MarkAsChild()
    End Sub
#End Region

#Region " Criteria "
    <Serializable()> Private Class CustomCriteria
        Inherits CommandCriteria

        Private _parentObjectID As Guid = Guid.Empty
        Private ReadOnly _objectID As Guid = Guid.Empty
        Private ReadOnly _entity As Entity = Entity.NOTHING

        'Add Data Portal criteria here
        Public Sub New(ByVal objectID As Guid, ByVal entity As Entity)
            CommandText = "getObjectLinks"
            _objectID = objectID
            _entity = entity
        End Sub
        Public Sub New(ByVal objectID As Guid, ByVal parentObjectID As Guid, ByVal entity As Entity)
            CommandText = "getObjectLinks"
            _parentObjectID = parentObjectID
            _objectID = objectID
            _entity = entity
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@OBJECTID", _objectID)
            command.Parameters.AddWithValue("@PARENTOBJECTID", _parentObjectID.GetDbValue())
            command.Parameters.AddWithValue("@ENTITY", _entity.ToString())
        End Sub

    End Class
#End Region

End Class
<Serializable()> Public NotInheritable Class Link
    Inherits BaseObjects.ContextBusinessBase(Of Link)

#Region " Business Properties & Methods "

    Private _type As LinkType
    Private _urlPart As String = String.Empty
    Private _label As String = String.Empty

    Public ReadOnly Property Type() As LinkType
        Get
            Return _type
        End Get
    End Property
    Public Property Label() As String
        Get
            Return _label
        End Get
        Set(ByVal value As String)
            If _label <> value Then
                _label = value
                PropertyHasChanged("Label")
            End If
        End Set
    End Property
    Public Property UrlPart() As String
        Get
            Return _urlPart
        End Get
        Set(ByVal value As String)
            If _urlPart <> value Then
                _urlPart = value
                PropertyHasChanged("UrlPart")
            End If
        End Set
    End Property


    Private ReadOnly Property ParentID() As Guid
        Get
            Return DirectCast(Me.Parent, Links).ParentID
        End Get
    End Property
#End Region

#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("UrlPart", 255))
    End Sub
#End Region

#Region " Framework Overrides "
    Protected Overrides Function GetIdValue() As Object
        Return Me.Type
    End Function
#End Region

#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Me.UrlPart
    End Function
#End Region

#Region " Shared Factory Methods "

    Friend Shared Function NewLink(ByVal type As LinkType) As Link
        Dim _link As Link = New Link
        _link._type = type
        Return _link
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
        Me.MarkAsChild()
        Me.AutoDiscover = False
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        _type = LinkType.GetLinkType(dataReader)
        _label = dataReader.GetString("LABEL")
        _urlPart = dataReader.GetString("URLPART")
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "insertObjectLink"
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "updateObjectLink"
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.Parameters.AddWithValue("@OBJECTID", Me.ParentID)
        command.Parameters.AddWithValue("@LINKTYPE", Me.Type.ID)
        command.Parameters.AddWithValue("@LABEL", Me.Label)
        command.Parameters.AddWithValue("@URLPART", Me.UrlPart)
    End Sub
    Protected Overrides Sub AddDeleteCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
        command.CommandText = "deleteObjectLink"
        command.Parameters.AddWithValue("@OBJECTID", Me.ParentID)
        command.Parameters.AddWithValue("@LINKTYPE", Me.Type.ID)
    End Sub
#End Region

End Class