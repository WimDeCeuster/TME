Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.ActiveFilterTool.Enumerations
Imports TME.CarConfigurator.Administration.Enums
Imports TME.BusinessObjects
Imports Rules = TME.BusinessObjects.ValidationRules
Imports TME.CarConfigurator.Administration.Interfaces

Namespace ActiveFilterTool
    <Serializable()> Public NotInheritable Class ModelSubSets
        Inherits BaseObjects.StronglySortedListBase(Of ModelSubSets, ModelSubSet)

#Region " Business Properties & Methods "
        Private _type As VehicleType
        Private Property Type() As VehicleType
            Get
                Return _type
            End Get
            Set(ByVal value As VehicleType)
                _type = value
            End Set
        End Property


        Public Shadows Function Add(ByVal model As Model) As ModelSubSet
            Dim _subset As ModelSubSet = ModelSubSet.NewModelSubSetChild(model, Me.Type)
            MyBase.Add(_subset)
            Return _subset
        End Function

#End Region

#Region " Shared Factory Methods "

        Public Shared Function GetModelSubSets(ByVal type As VehicleType) As ModelSubSets
            Dim _subSets As ModelSubSets = DataPortal.Fetch(Of ModelSubSets)(New CustomCriteria(type))
            _subSets.Type = type
            Return _subSets
        End Function

#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
            'Allow dataportal to create us
        End Sub
#End Region

#Region " Criteria "
        <Serializable()> Private Class CustomCriteria
            Inherits CommandCriteria

            'Add Data Portal criteria here
            Private ReadOnly _vehicleType As VehicleType
            Public Sub New(ByVal type As VehicleType)
                _vehicleType = type
            End Sub
            Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
                command.Parameters.AddWithValue("@VEHICLETYPE", CType(_vehicleType, Int16))
            End Sub
        End Class
#End Region


    End Class
    <Serializable()> Public NotInheritable Class ModelSubSet
        Inherits BaseObjects.TranslateableBusinessBase
        Implements BaseObjects.ISortedIndex
        Implements BaseObjects.ISortedIndexSetter
        Implements ILinkedAssets
        Implements ILinks

#Region " Business Properties & Methods "

        Private _code As String = String.Empty
        Private _name As String = String.Empty
        Private _status As Integer
        Private _special As Boolean
        Private _index As Integer
        Private _owner As String

        Private _type As VehicleType
        Private _modelInfo As ModelInfo

        Private _compatibilites As ModelSubSetCompatibilites
        Private _incompatibilites As ModelSubSetInCompatibilites
        Private _classifications As ModelSubSetClassifications
        Private _links As Links

        Private _assets As LinkedAssets

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
        Public Property Activated() As Boolean
            Get
                Return ((_status And Status.AvailableToNmscs) = Status.AvailableToNmscs)
            End Get
            Set(ByVal value As Boolean)
                If Not value.Equals(Me.Activated) Then
                    If Me.Activated Then
                        _status -= Status.AvailableToNmscs
                    Else
                        _status += Status.AvailableToNmscs
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
                    If value Then
                        _status += Status.ApprovedForLive
                    Else
                        _status -= Status.ApprovedForLive
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
                    If Me.Preview Then
                        _status -= Status.ApprovedForPreview
                    Else
                        _status += Status.ApprovedForPreview
                    End If
                    PropertyHasChanged("Preview")
                End If
            End Set
        End Property
        Public Property Special() As Boolean
            Get
                Return _special
            End Get
            Set(ByVal value As Boolean)
                If Not value.Equals(_special) Then
                    _special = value
                    PropertyHasChanged("Special")
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

        Public ReadOnly Property Model() As ModelInfo
            Get
                Return _modelInfo
            End Get
        End Property
        Public ReadOnly Property Type() As VehicleType
            Get
                Return _type
            End Get
        End Property

        Public ReadOnly Property Compatibilites() As ModelSubSetCompatibilites
            Get
                If _compatibilites Is Nothing Then
                    If Me.IsNew Then
                        _compatibilites = ModelSubSetCompatibilites.NewModelSubSetCompatibilites(Me)
                    Else
                        LoadFittings()
                    End If
                End If
                Return _compatibilites
            End Get
        End Property
        Public ReadOnly Property InCompatibilites() As ModelSubSetInCompatibilites
            Get
                If _incompatibilites Is Nothing Then
                    If Me.IsNew Then
                        _incompatibilites = ModelSubSetInCompatibilites.NewModelSubSetInCompatibilites(Me)
                    Else
                        LoadFittings()
                    End If
                End If
                Return _incompatibilites
            End Get
        End Property
        Private Sub LoadFittings()
            With ModelSubSetFittings.GetModelSubSetFittings(Me)
                _compatibilites = .Compatibilites
                _incompatibilites = .Incompatibilites
            End With
        End Sub
        Public ReadOnly Property Classifications() As ModelSubSetClassifications
            Get
                If _classifications Is Nothing Then
                    If Me.IsNew Then
                        _classifications = ModelSubSetClassifications.NewModelSubSetClassifications(Me)
                    Else
                        _classifications = ModelSubSetClassifications.GetModelSubSetClassifications(Me)
                    End If
                End If
                Return _classifications
            End Get
        End Property

        Public ReadOnly Property Assets() As LinkedAssets Implements ILinkedAssets.Assets
            Get
                If _assets Is Nothing Then
                    If Me.IsNew Then
                        _assets = LinkedAssets.NewLinkedAssets(Me.ID)
                    Else
                        _assets = LinkedAssets.GetLinkedAssets(Me.ID)
                    End If
                End If
                Return _assets
            End Get
        End Property
        Public ReadOnly Property Links() As Links Implements ILinks.Links
            Get
                If _links Is Nothing Then _links = Links.GetLinks(Me)
                Return _links
            End Get
        End Property

        Public Function HasOwnership() As Boolean
            Return (String.Compare(_owner, MyContext.GetContext().CountryCode, True) = 0)
        End Function
#End Region

#Region " Business & Validation Rules "
        Protected Overrides Sub AddBusinessRules()
            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.Required, Validation.RuleHandler), "Code")
            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Code", 100))

            ValidationRules.AddRule(DirectCast(AddressOf Rules.String.MaxLength, Validation.RuleHandler), New Rules.String.MaxLengthRuleArgs("Name", 255))
        End Sub
#End Region

#Region " System.Object Overrides "
        Public Overloads Overrides Function ToString() As String
            If Me.Name.Length = 0 Then
                Return Me.Model.Name
            Else
                Return Me.Name
            End If
        End Function
        Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
            Return String.Compare(obj, Me.Code, True) = 0 OrElse String.Compare(obj, Me.Name, True) = 0
        End Function
#End Region

#Region " Framework Overrides "
        Public Overloads Overrides ReadOnly Property IsValid() As Boolean
            Get
                If Not MyBase.IsValid Then Return False
                If Not (_compatibilites Is Nothing) AndAlso Not _compatibilites.IsValid Then Return False
                If Not (_incompatibilites Is Nothing) AndAlso Not _incompatibilites.IsValid Then Return False
                If Not (_classifications Is Nothing) AndAlso Not _classifications.IsValid Then Return False
                If Not (_assets Is Nothing) AndAlso Not _assets.IsValid Then Return False
                If Not (_links Is Nothing) AndAlso Not _links.IsValid Then Return False
                Return True
            End Get
        End Property
        Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
            Get
                If MyBase.IsDirty Then Return True
                If Not (_compatibilites Is Nothing) AndAlso _compatibilites.IsDirty Then Return True
                If Not (_incompatibilites Is Nothing) AndAlso _incompatibilites.IsDirty Then Return True
                If Not (_classifications Is Nothing) AndAlso _classifications.IsDirty Then Return True
                If Not (_assets Is Nothing) AndAlso _assets.IsDirty Then Return True
                If Not (_links Is Nothing) AndAlso _links.IsDirty Then Return True
                Return False
            End Get
        End Property
#End Region

#Region " Shared Factory Methods "

        Public Shared Function NewModelSubSet(ByVal model As Model, ByVal type As VehicleType) As ModelSubSet
            Dim _subset As ModelSubSet = New ModelSubSet
            _subset.Create(model, type)
            Return _subset
        End Function
        Friend Shared Function NewModelSubSetChild(ByVal model As Model, ByVal type As VehicleType) As ModelSubSet
            Dim _subset As ModelSubSet = New ModelSubSet
            _subset.Create(model, type)
            _subset.MarkAsChild()
            Return _subset
        End Function

        Public Shared Function GetModelSubSet(ByVal id As Guid) As ModelSubSet
            Return DataPortal.Fetch(Of ModelSubSet)(New Criteria(id))
        End Function

        Public Shared Sub DeleteModelSubSet(ByVal id As Guid)
            DataPortal.Delete(Of ModelSubSet)(New Criteria(id))
        End Sub
#End Region

#Region " Constructors "
        Private Sub New()
            'Prevent direct creation
        End Sub
#End Region

#Region " Data Access "

        Private Overloads Sub Create(ByVal newModel As Model, ByVal newType As VehicleType)
            _modelInfo = ModelInfo.GetModelInfo(newModel)
            _type = newType
            Me.Activated = Not (MyContext.GetContext().CountryCode = Environment.GlobalCountryCode)
            Me.Compatibilites.Add() 'Add default [ALL] fitting..
        End Sub
        Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
            With dataReader
                _modelInfo = ModelInfo.GetModelInfo(dataReader)
                _type = CType(.GetValue("VEHICLETYPE"), VehicleType)
                _code = .GetString("INTERNALCODE")
                _name = .GetString("SHORTNAME")
                _special = .GetBoolean("SPECIAL")
                _status = .GetInt16("STATUSID")
                _index = .GetInt16("SORTORDER")
                _owner = dataReader.GetString("OWNER")
            End With
            MyBase.FetchFields(dataReader)
        End Sub

        Protected Overrides Sub AddInsertCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            Me.AddCommandFields(command)
        End Sub
        Protected Overrides Sub AddUpdateCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            Me.AddCommandFields(command)
        End Sub
        Private Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@VEHICLETYPE", Me.Type)
            command.Parameters.AddWithValue("@MODELID", Me.Model.ID)
            command.Parameters.AddWithValue("@INTERNALCODE", Me.Code)
            command.Parameters.AddWithValue("@SHORTNAME", Me.Name)
            command.Parameters.AddWithValue("@STATUSID", _status)
            command.Parameters.AddWithValue("@SPECIAL", Me.Special)
            command.Parameters.AddWithValue("@SORTORDER", Me.Index)
        End Sub

        Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
            MyBase.UpdateChildren(transaction)
            If Not _compatibilites Is Nothing Then _compatibilites.Update(transaction)
            If Not _incompatibilites Is Nothing Then _incompatibilites.Update(transaction)
            If Not _classifications Is Nothing Then _classifications.Update(transaction)
            If Not _assets Is Nothing Then _assets.Update(transaction)
            If Not _links Is Nothing Then _links.Update(transaction)
        End Sub

#End Region

#Region " Base Object Overrides "

        Protected Friend Overrides Function GetBaseName() As String
            Return Me.Name
        End Function
        Public Overrides ReadOnly Property Entity As Entity
            Get
                Return Entity.MODELSUBSET
            End Get
        End Property
#End Region

    End Class
End Namespace