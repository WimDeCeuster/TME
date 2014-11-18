Imports TME.CarConfigurator.Administration.Enums
Imports Rules = TME.BusinessObjects.ValidationRules

<Serializable()> Public NotInheritable Class Models
    Inherits BaseObjects.StronglySortedListBase(Of Models, Model)

#Region " Contains Methods "

    Public Overloads Function ContainsDeleted(ByVal obj As Model) As Boolean
        Return DeletedList.Any(Function(model) model.Equals(obj))
    End Function

#End Region

#Region " Shared Factory Methods "

    Public Shared Function GetModels() As Models
        Return DataPortal.Fetch(Of Models)(New Criteria())
    End Function

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

End Class
<Serializable()> Public NotInheritable Class Model
    Inherits BaseObjects.LocalizeableBusinessBase
    Implements BaseObjects.ISortedIndex
    Implements BaseObjects.ISortedIndexSetter
    Implements ILinks

#Region " Business Properties & Methods "
    Private _approvalChanged As Boolean = False
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _brand As Brand
    Private _status As Integer
    Private _index As Integer
    Private _suffixModeAvailable As Boolean = False

    Private WithEvents _generations As ModelGenerations
    Private _links As Links
    Private _factoryModels As FactoryModels

    Public Property Code() As String
        Get
            Return _code
        End Get
        Set(ByVal value As String)
            If _code <> value Then
                _code = value
                PropertyHasChanged("Code")
            End If
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            If _name <> value Then
                _name = value
                PropertyHasChanged("Name")
            End If
        End Set
    End Property
    Public ReadOnly Property SuffixModeAvailable() As Boolean
        Get
            If _generations Is Nothing Then Return _suffixModeAvailable
            Return _generations.Any(Function(x) x.SuffixModeAvailable)
        End Get
    End Property
    Public ReadOnly Property Brand() As Brand
        Get
            Return _brand
        End Get
    End Property

    Public ReadOnly Property Promoted() As Boolean
        Get
            Return ((_status And Status.Promoted) = Status.Promoted)
        End Get
    End Property

    Public Property Activated() As Boolean
        Get
            Return ((_status And Status.AvailableToNMSCs) = Status.AvailableToNMSCs)
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Activated) Then
                If Me.Activated Then
                    _status -= Status.AvailableToNMSCs
                Else
                    _status += Status.AvailableToNMSCs
                End If
                PropertyHasChanged("Activated")
            End If
        End Set
    End Property
    Public Property Approved() As Boolean
        Get
            Return ((_status And Status.ApprovedForLive) = Status.ApprovedForLive)
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Approved) Then
                _approvalChanged = True
                If value Then
                    _status += Status.ApprovedForLive
                Else
                    _status -= Status.ApprovedForLive
                    If Me.Promoted Then
                        _status -= Status.Promoted
                    End If
                End If
                Me.Declined = Not (value)
                PropertyHasChanged("Approved")
            End If
        End Set
    End Property

    Public Property Declined() As Boolean
        Get
            Return ((_status And Status.Declined) = Status.Declined)
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Declined) Then
                If value Then
                    _status += Status.Declined
                Else
                    _status -= Status.Declined
                End If
                Me.Approved = Not (value)
                PropertyHasChanged("Declined")
            End If
        End Set
    End Property
    Public Property Preview() As Boolean
        Get
            Return ((_status And Status.ApprovedForPreview) = Status.ApprovedForPreview)
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Me.Preview) Then
                _approvalChanged = True
                If Me.Preview Then
                    _status -= Status.ApprovedForPreview
                Else
                    _status += Status.ApprovedForPreview
                End If
                PropertyHasChanged("Preview")
            End If
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

    Public ReadOnly Property Generations() As ModelGenerations
        Get
            If _generations Is Nothing Then
                If Me.IsNew Then
                    _generations = ModelGenerations.NewModelGenerations(Me)
                Else
                    _generations = ModelGenerations.GetModelGenerations(Me)
                End If
            End If
            Return _generations
        End Get
    End Property
    Private Sub GenerationsGenerationAvailabilityChanged(ByVal items As ModelGenerations) Handles _generations.GenerationAvailabilityChanged
        Me._approvalChanged = True
        Me.ValidationRules.CheckRules()
        Me.MarkDirty()
    End Sub

    Public ReadOnly Property Links() As Links Implements ILinks.Links
        Get
            If _links Is Nothing Then _links = Links.GetLinks(Me)
            Return _links
        End Get
    End Property
    Public ReadOnly Property FactoryModels() As FactoryModels
        Get
            If _factoryModels Is Nothing Then
                _factoryModels = FactoryModels.GetFactoryModels(Me)
            End If
            Return _factoryModels
        End Get
    End Property

    Public Function GetInfo() As ModelInfo
        Return ModelInfo.GetModelInfo(Me)
    End Function

    Public Shadows Sub MarkDirty()
        MyBase.MarkDirty()
    End Sub

#End Region

#Region " Business & Validation Rules "

    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))

        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf Rules.Value.Unique, Validation.RuleHandler), "Name")

        ValidationRules.AddRule(DirectCast(AddressOf Model.ApprovedRules, Validation.RuleHandler), "Approved")
        ValidationRules.AddRule(DirectCast(AddressOf Model.PreviewRules, Validation.RuleHandler), "Preview")

    End Sub

    Private Shared Function ApprovedRules(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _model As Model = DirectCast(target, Model)
        If _model._approvalChanged AndAlso _model.Approved Then
            If _model.Generations.Any(Function(generation) generation.Approved) Then
                Return True
            End If
            e.Description = "You can <b>not</b> approve " & _model.Name & " for ""LIVE"" at this moment.<br/> You must approve at least one generation before you can continue."
            Return False
        End If
        Return True
    End Function
    Private Shared Function PreviewRules(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim _model As Model = DirectCast(target, Model)
        If _model._approvalChanged AndAlso _model.Preview Then

            If _model.Generations.Any(Function(x) x.Preview) Then
                Return True
            End If

            e.Description = "You can <b>not</b> approve " & _model.Name & " for ""PREVIEW"" at this moment.<br/> You must approve at least one generation before you can continue."
            Return False
        End If
        Return True
    End Function
#End Region

#Region " System.Object Overrides "

    Public Overloads Overrides Function ToString() As String
        Return Me.Name
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        Return (String.Compare(Me.Code, obj, True) = 0 OrElse String.Compare(Me.Name, obj, True) = 0)
    End Function

#End Region

#Region " Framework Overrides "

    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_generations Is Nothing) AndAlso Not _generations.IsValid Then Return False
            If Not (_links Is Nothing) AndAlso Not _links.IsValid Then Return False
            If Not (_factoryModels Is Nothing) AndAlso Not _factoryModels.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_generations Is Nothing) AndAlso _generations.IsDirty Then Return True
            If Not (_links Is Nothing) AndAlso _links.IsDirty Then Return True
            If Not (_factoryModels Is Nothing) AndAlso _factoryModels.IsDirty Then Return True
            Return False
        End Get
    End Property

#End Region

#Region " Shared Factory Methods "

    Public Shared Function NewModel() As Model
        Return New Model()
    End Function
    Public Shared Function GetModel(ByVal id As Guid) As Model
        Try
            Return DataPortal.Fetch(Of Model)(New Criteria(id))
        Catch ex As DataPortalException
            If Not ex.InnerException Is Nothing AndAlso TypeOf ex.InnerException Is Server.CallMethodException Then
                If Not ex.InnerException.InnerException Is Nothing AndAlso TypeOf ex.InnerException.InnerException Is Templates.Exceptions.NoDataReturnedException Then
                    Throw New Exceptions.InvalidModelIdentifier(String.Format("No model with id '{0}' could be retrieved.", id), ex)
                End If
            End If
            Throw
        End Try
    End Function
    Public Shared Sub DeleteModel(ByVal id As Guid)
        DataPortal.Delete(Of Model)(New Criteria(id))
    End Sub

    Public Shared Function GetPromotedModel() As Model
        Return GetModel(DirectCast(BaseObjects.ContextCommand.ExecuteScalar(New GetPromotedModelId()), Guid))
    End Function
    Public Shared Sub PromoteModel(ByVal model As Model)
        If model Is Nothing Then Throw New Exceptions.InvalidModelIdentifier
        If Not model.Approved Then Throw New Exceptions.ModelNeedsToBeApproved
        BaseObjects.ContextCommand.Execute(New PromoteModelCommand(model.ID))
    End Sub

#End Region

#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region

#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _brand = Brands.GetBrands()(MyContext.GetContext().Brand)
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _status = .GetInt16("STATUSID")
            _index = .GetInt16("SORTORDER")
            _suffixModeAvailable = .GetBoolean("SUFFIXMODEAVAILABLE")
        End With
        _brand = Brand.GetBrand(dataReader, "BRAND")
        MyBase.FetchFields(dataReader)
    End Sub
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        _generations = ModelGenerations.GetModelGenerations(Me, dataReader)
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        Me.AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@INTERNALCODE", Me.Code)
        command.Parameters.AddWithValue("@LOCALCODE", Me.LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
        command.Parameters.AddWithValue("@BRANDID", Me.Brand.ID)
        command.Parameters.AddWithValue("@STATUSID", _status)
        command.Parameters.AddWithValue("@SORTORDER", Me.Index)
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        MyBase.UpdateChildren(transaction)
        If Not _generations Is Nothing Then _generations.Update(transaction)
        If Not _links Is Nothing Then _links.Update(transaction)
        If Not _factoryModels Is Nothing Then _factoryModels.Update(transaction)
    End Sub

#End Region

#Region " PromotedModel Helper Class "
    <Serializable()> Private Class GetPromotedModelId
        Inherits BaseObjects.ContextCommand.CommandInfo
        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "getPromotedModelID"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            'none
        End Sub
    End Class
    <Serializable()> Private Class PromoteModelCommand
        Inherits BaseObjects.ContextCommand.CommandInfo

        Private ReadOnly _modelId As Guid

        Public Overloads Overrides ReadOnly Property CommandText() As String
            Get
                Return "promoteModel"
            End Get
        End Property
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@MODELID", _modelId)
        End Sub

        Public Sub New(ByVal modelId As Guid)
            _modelId = modelId
        End Sub

    End Class
#End Region

#Region " Base Object Overrides "
    Protected Friend Overrides Function GetBaseCode() As String
        Return Me.Code
    End Function
    Protected Friend Overrides Function GetBaseName() As String
        Return Me.Name
    End Function

   
    Public Overrides ReadOnly Property Entity As Entity
        Get
            Return Entity.MODEL
        End Get
    End Property
#End Region


End Class
<Serializable()> Public Structure ModelInfo

#Region " Business Properties & Methods "
    Private _id As Guid
    Private _name As String

    Public Property ID() As Guid
        Get
            Return _id
        End Get
        Private Set(value As Guid)
            _id = value
        End Set
    End Property
    Public Property Name() As String
        Get
            Return _name
        End Get
        Private Set(value As String)
            _name = value
        End Set
    End Property

#End Region

#Region " Shared Factory Methods "
    Friend Shared Function GetModelInfo(ByVal dataReader As SafeDataReader) As ModelInfo
        Dim info As ModelInfo = New ModelInfo
        info.ID = dataReader.GetGuid("MODELID")
        info.Name = dataReader.GetString("MODELNAME")
        Return info
    End Function
    Friend Shared Function GetModelInfo(ByVal model As Model) As ModelInfo
        Dim info As ModelInfo = New ModelInfo
        info.ID = model.ID
        info.Name = model.Name
        Return info
    End Function
    Friend Shared Function GetModelInfo(ByVal generation As ModelGeneration) As ModelInfo
        Dim info As ModelInfo = New ModelInfo
        info.ID = generation.Model.ID
        info.Name = generation.Model.Name
        Return info

    End Function
    Friend Shared Function Empty() As ModelInfo
        Dim info As ModelInfo = New ModelInfo
        info.ID = Guid.Empty
        info.Name = String.Empty
        Return info
    End Function

#End Region

End Structure