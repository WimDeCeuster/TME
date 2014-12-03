Imports System.Collections.Generic
Imports TME.CarConfigurator.Administration.Assets
Imports TME.CarConfigurator.Administration.Enums
<Serializable()> Public NotInheritable Class ModelGenerationPacks
    Inherits StronglySortedListBase(Of ModelGenerationPacks, ModelGenerationPack)
#Region " Delegates & Events "
    Friend Delegate Sub PacksChangedHandler(ByVal pack As ModelGenerationPack)
    Friend Event PackAdded As PacksChangedHandler
    Friend Event PackRemoved As PacksChangedHandler
#End Region
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
    Private Sub MyListChanged(ByVal sender As Object, ByVal e As ComponentModel.ListChangedEventArgs) Handles Me.ListChanged
        If e.ListChangedType = ComponentModel.ListChangedType.ItemAdded Then
            RaiseEvent PackAdded(Me(e.NewIndex))
        End If
    End Sub
    Private Sub MyRemovingItem(ByVal sender As Object, ByVal e As Core.RemovingItemEventArgs) Handles Me.RemovingItem
        RaiseEvent PackRemoved(DirectCast(e.RemovingItem, ModelGenerationPack))
    End Sub
#End Region
#Region " Shared Factory Methods "
    Friend Shared Function NewModelGenerationPacks(ByVal generation As ModelGeneration) As ModelGenerationPacks
        Dim packs As ModelGenerationPacks = New ModelGenerationPacks()
        packs.Generation = generation
        Return packs
    End Function
    Friend Shared Function GetModelGenerationPacks(ByVal generation As ModelGeneration) As ModelGenerationPacks
        Dim packs As ModelGenerationPacks = DataPortal.Fetch(Of ModelGenerationPacks)(New CustomCriteria(generation))
        packs.Generation = generation
        Return packs
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
        Private ReadOnly _modelID As Guid
        Private ReadOnly _generationID As Guid
        Public Sub New(ByVal generation As ModelGeneration)
            _modelID = generation.Model.ID
            _generationID = generation.ID
        End Sub
        Public Overloads Overrides Sub AddCommandFields(ByVal command As System.Data.SqlClient.SqlCommand)
            command.Parameters.AddWithValue("@MODELID", _modelID)
            command.Parameters.AddWithValue("@GENERATIONID", _generationID)
        End Sub
    End Class
#End Region
#Region " Data Access "
    Protected Overrides ReadOnly Property RaiseListChangedEventsDuringFetch() As Boolean
        Get
            Return False
        End Get
    End Property
    Protected Overrides Sub FetchNextResult(ByVal dataReader As Common.Database.SafeDataReader)
        FetchExteriorColourApplicabilities(dataReader)
        If dataReader.NextResult Then FetchUpholsteryApplicabilities(dataReader)
        If dataReader.NextResult Then FetchRules(dataReader) 'Item rules
        If dataReader.NextResult Then FetchRules(dataReader) 'Pack rules
    End Sub
    Private Sub FetchExteriorColourApplicabilities(ByVal dataReader As Common.Database.SafeDataReader)
        While dataReader.Read
            Dim pack = Item(dataReader.GetGuid("PACKID"))
            If pack Is Nothing Then Continue While
            pack.ExteriorColourApplicabilities.Add(dataReader)
        End While
    End Sub
    Private Sub FetchUpholsteryApplicabilities(ByVal dataReader As Common.Database.SafeDataReader)
        While dataReader.Read
            Dim pack = Item(dataReader.GetGuid("PACKID"))
            If pack Is Nothing Then Continue While
            pack.UpholsteryApplicabilities.Add(dataReader)
        End While
    End Sub
    Private Sub FetchRules(ByVal dataReader As Common.Database.SafeDataReader)
        While dataReader.Read
            Dim pack = Item(dataReader.GetGuid("PACKID"))
            If pack Is Nothing Then Continue While
            pack.Rules.Add(dataReader)
        End While
    End Sub
#End Region
End Class
<Serializable()> Public NotInheritable Class ModelGenerationPack
    Inherits LocalizeableBusinessBase
    Implements ISortedIndex
    Implements ISortedIndexSetter
    Implements IUpdatableAssetSet
    Implements IOwnedBy
    Implements IPrice
    Implements IMasterListObject
#Region " Business Properties & Methods "
    Private _shortID As Nullable(Of Integer) = Nothing
    Private _code As String = String.Empty
    Private _name As String = String.Empty
    Private _description As String = String.Empty
    Private _status As Integer
    Private _index As Integer
    Private _owner As String = String.Empty
    Private _price As Decimal = 0D
    Private _vatPrice As Decimal = 0D
    Private WithEvents _equipment As ModelGenerationPackItems
    Private WithEvents _rules As ModelGenerationPackRules
    Private WithEvents _extColourApplicabilities As ExteriorColourApplicabilities
    Private _upholsteryApplicabilities As UpholsteryApplicabilities
    Private _assetSet As AssetSet
    Private _masterPacks As ModelGenerationMasterPacks
    Private _accentColourCombinations As AccentColourCombinations
    Public ReadOnly Property Generation() As ModelGeneration
        Get
            If Parent Is Nothing Then Return Nothing
            Return DirectCast(Parent, ModelGenerationPacks).Generation
        End Get
    End Property
    Public ReadOnly Property ShortID() As Nullable(Of Integer)
        Get
            Return _shortID
        End Get
    End Property
    Public Property Code() As String Implements IMasterListObject.MasterCode
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
    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            If _description <> value Then
                _description = value
                PropertyHasChanged("Description")
            End If
        End Set
    End Property
    Public ReadOnly Property Index() As Integer Implements ISortedIndex.Index
        Get
            Return _index
        End Get
    End Property
    Friend WriteOnly Property SetIndex() As Integer Implements ISortedIndexSetter.SetIndex
        Set(ByVal value As Integer)
            If Not _index.Equals(value) Then
                _index = value
                PropertyHasChanged("Index")
            End If
        End Set
    End Property
    Public ReadOnly Property Owner() As String Implements IOwnedBy.Owner
        Get
            Return _owner
        End Get
    End Property
    Private ReadOnly Property MasterCode() As String Implements IMasterListObjectReference.MasterCode
        Get
            Return Code
        End Get
    End Property
    Public ReadOnly Property MasterIDs() As List(Of Guid) Implements IMasterListObjectReference.MasterIDs
        Get
            Return MasterPacks.Select(Function(x) x.ID).ToList()
        End Get
    End Property
    Public ReadOnly Property MasterType() As MasterPackType
        Get
            If MasterPacks.Any() Then Return MasterPacks(0).Type
            Return MasterPackType.None
        End Get
    End Property
    Public Property Activated() As Boolean
        Get
            Return ((_status And Status.AvailableToNmscs) = Status.AvailableToNmscs)
        End Get
        Set(ByVal value As Boolean)
            If Not value.Equals(Activated) Then
                If Activated Then
                    _status -= Status.AvailableToNmscs
                Else
                    _status += Status.AvailableToNmscs
                End If
                PropertyHasChanged("Activated")
            End If
        End Set
    End Property
    Public Property Price() As Decimal Implements IPrice.PriceExcludingVat
        Get
            Return _price
        End Get
        Set(ByVal value As Decimal)
            If Not _price.Equals(value) Then
                _price = value
                PropertyHasChanged("Price")
            End If
        End Set
    End Property
    Public Property VatPrice() As Decimal Implements IPrice.PriceIncludingVat
        Get
            Return _vatPrice
        End Get
        Set(ByVal value As Decimal)
            If Not _vatPrice.Equals(value) Then
                _vatPrice = value
                PropertyHasChanged("VatPrice")
            End If
        End Set
    End Property
    Public ReadOnly Property Equipment() As ModelGenerationPackItems
        Get
            If _equipment Is Nothing Then
                If IsNew Then
                    _equipment = ModelGenerationPackItems.NewModelGenerationPackItems(Me)
                Else
                    _equipment = ModelGenerationPackItems.GetModelGenerationPackItems(Me)
                End If
            End If
            Return _equipment
        End Get
    End Property
    Private Sub EquipmentChanged(ByVal item As ModelGenerationPackItem) Handles _equipment.PackItemAdded, _equipment.PackItemRemoved
        ValidationRules.CheckRules("EquipmentAndRules")
        AccentColourCombinations.Synchronize()
    End Sub
    Private Sub EquipmentColouringModeChanged(item As ModelGenerationPackItem) Handles _equipment.PackItemColouringModeChanged
        AccentColourCombinations.Synchronize()
    End Sub
    Public ReadOnly Property Rules() As ModelGenerationPackRules
        Get
            If _rules Is Nothing Then _rules = ModelGenerationPackRules.NewModelGenerationPackRules(Me)
            Return _rules
        End Get
    End Property
    Private Sub RulesChanged(ByVal rule As ModelGenerationRule) Handles _rules.RuleAdded, _rules.RuleRemoved
        ValidationRules.CheckRules("EquipmentAndRules")
    End Sub
    Public ReadOnly Property ExteriorColourApplicabilities() As ExteriorColourApplicabilities
        Get
            If _extColourApplicabilities Is Nothing Then _extColourApplicabilities = ExteriorColourApplicabilities.NewExteriorColourApplicabilities(Me)
            Return _extColourApplicabilities
        End Get
    End Property
    Private Sub ExteriorColourApplicabilitiesChanged(exteriorColourApplicability As ExteriorColourApplicability) Handles _extColourApplicabilities.ApplicabilityAdded, _extColourApplicabilities.ApplicabilityRemoved
        AccentColourCombinations.Synchronize()
    End Sub
    Public ReadOnly Property UpholsteryApplicabilities() As UpholsteryApplicabilities
        Get
            If _upholsteryApplicabilities Is Nothing Then _upholsteryApplicabilities = UpholsteryApplicabilities.NewUpholsteryApplicabilities(Me)
            Return _upholsteryApplicabilities
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
    Public Overloads Overrides Function CanWriteProperty(ByVal propertyName As String) As Boolean
        If MyContext.GetContext().IsGlobal Then Return MyBase.CanWriteProperty(propertyName)
        If Owner.Equals(MyContext.GetContext().CountryCode(), StringComparison.InvariantCultureIgnoreCase) Then Return MyBase.CanWriteProperty(propertyName)
        Select Case propertyName
            Case "Index", "Price", "VatPrice", "LocalCode"
                Return MyBase.CanWriteProperty(propertyName)
            Case Else
                Return False
        End Select
    End Function
    Public ReadOnly Property MasterPacks() As ModelGenerationMasterPacks
        Get
            If _masterPacks Is Nothing Then
                _masterPacks = ModelGenerationMasterPacks.GetModelGenerationPackMasterPacks(Me)
            End If
            Return _masterPacks
        End Get
    End Property
    Public ReadOnly Property AccentColourCombinations() As AccentColourCombinations
        Get
            If _accentColourCombinations Is Nothing Then
                _accentColourCombinations = AccentColourCombinations.GetAccentColourCombinations(Me)
                _accentColourCombinations.Synchronize()
            End If
            Return _accentColourCombinations
        End Get
    End Property
    Public Function BodyColours() As IEnumerable(Of ExteriorColourInfo)
        If ExteriorColourApplicabilities.Any() Then Return ExteriorColourApplicabilities.Select(Function(x) x.GetInfo())
        Return Generation.ColourCombinations.ExteriorColours().Select(Function(x) x.GetInfo())
    End Function
    Public Function PrimaryAccentColours() As IEnumerable(Of ExteriorColourInfo)
        Return Equipment.
        Where(Function(x) Not x.Colour.IsEmpty() AndAlso (x.ColouringModes And ColouringModes.PrimaryAccentColour) = ColouringModes.PrimaryAccentColour).
        Select(Function(x) x.Colour).
        Distinct()
    End Function
    Public Function SecondaryAccentColours() As IEnumerable(Of ExteriorColourInfo)
        Return Equipment.
        Where(Function(x) Not x.Colour.IsEmpty() AndAlso (x.ColouringModes And ColouringModes.SecondaryAccentColour) = ColouringModes.SecondaryAccentColour).
        Select(Function(x) x.Colour).
        Distinct()
    End Function
    Public ReadOnly Property ContainsSuffixOptions() As Boolean
        Get
            Return Equipment.OfType(Of ModelGenerationPackOption).Any(Function(o) DirectCast(Generation.Equipment(o.ID), ModelGenerationOption).SuffixOption)
        End Get
    End Property
#End Region
#Region " Business & Validation Rules "
    Protected Overrides Sub AddBusinessRules()
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.Required, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Code", 50))
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.String.MaxLength, Validation.RuleHandler), New BusinessObjects.ValidationRules.String.MaxLengthRuleArgs("Name", 255))
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Value.Unique, Validation.RuleHandler), "Code")
        ValidationRules.AddRule(DirectCast(AddressOf BusinessObjects.ValidationRules.Value.Unique, Validation.RuleHandler), "Name")
        ValidationRules.AddRule(DirectCast(AddressOf EquipmentAndRulesValidationRule, Validation.RuleHandler), "EquipmentAndRules")
    End Sub
    Private Shared Function EquipmentAndRulesValidationRule(ByVal target As Object, ByVal e As Validation.RuleArgs) As Boolean
        Dim pack As ModelGenerationPack = DirectCast(target, ModelGenerationPack)
        If pack.Rules.Any(Function(x) pack.Equipment.Contains(x.ID)) Then
            e.Description = String.Format("The pack has an include/exclude on an equipment item which is also a part of the pack's content.{0}This is not allowed!", System.Environment.NewLine)
            Return False
        End If
        Return True
    End Function
#End Region
#Region " System.Object Overrides "
    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function
    Public Overloads Overrides Function Equals(ByVal obj As String) As Boolean
        If String.Compare(LocalCode, obj) = 0 Then Return True
        Return String.Compare(Code, obj, True) = 0
    End Function
#End Region
#Region " Framework Overrides "
    Public Overloads Overrides ReadOnly Property IsValid() As Boolean
        Get
            If Not MyBase.IsValid Then Return False
            If Not (_assetSet Is Nothing) AndAlso Not _assetSet.IsValid Then Return False
            If Not (_equipment Is Nothing) AndAlso Not _equipment.IsValid Then Return False
            If Not (_rules Is Nothing) AndAlso Not _rules.IsValid Then Return False
            If Not (_extColourApplicabilities Is Nothing) AndAlso Not _extColourApplicabilities.IsValid Then Return False
            If Not (_upholsteryApplicabilities Is Nothing) AndAlso Not _upholsteryApplicabilities.IsValid Then Return False
            If Not (_masterPacks Is Nothing) AndAlso Not _masterPacks.IsValid Then Return False
            If Not (_accentColourCombinations Is Nothing) AndAlso Not _accentColourCombinations.IsValid Then Return False
            Return True
        End Get
    End Property
    Public Overloads Overrides ReadOnly Property IsDirty() As Boolean
        Get
            If MyBase.IsDirty Then Return True
            If Not (_assetSet Is Nothing) AndAlso _assetSet.IsDirty Then Return True
            If Not (_equipment Is Nothing) AndAlso _equipment.IsDirty Then Return True
            If Not (_rules Is Nothing) AndAlso _rules.IsDirty Then Return True
            If Not (_extColourApplicabilities Is Nothing) AndAlso _extColourApplicabilities.IsDirty Then Return True
            If Not (_upholsteryApplicabilities Is Nothing) AndAlso _upholsteryApplicabilities.IsDirty Then Return True
            If Not (_masterPacks Is Nothing) AndAlso _masterPacks.IsDirty Then Return True
            If Not (_accentColourCombinations Is Nothing) AndAlso _accentColourCombinations.IsDirty Then Return True
            Return False
        End Get
    End Property
#End Region
#Region " Constructors "
    Private Sub New()
        'Prevent direct creation
    End Sub
#End Region
#Region " Data Access "
    Protected Overrides Sub InitializeFields()
        MyBase.InitializeFields()
        _status = Status.ApprovedForLive + Status.ApprovedForPreview
        _owner = MyContext.GetContext.CountryCode()
    End Sub
    Protected Overrides Sub FetchFields(ByVal dataReader As Common.Database.SafeDataReader)
        With dataReader
            _shortID = .GetInt32("SHORTID")
            _code = .GetString("INTERNALCODE")
            _name = .GetString("SHORTNAME")
            _description = .GetString("DESCRIPTION")
            _index = .GetInt16("SORTORDER")
            _status = .GetInt16("STATUSID")
            _owner = .GetString("OWNER")
            _price = Environment.ConvertPrice(CType(.GetValue("PRICE"), Decimal), .GetString("CURRENCY"))
            _vatPrice = Environment.ConvertPrice(CType(.GetValue("PRICEVAT"), Decimal), .GetString("CURRENCY"))
            _assetSet = AssetSet.GetAssetSet(Me, dataReader)
        End With
        MyBase.FetchFields(dataReader)
        AllowNew = True
        AllowEdit = True
        AllowRemove = (String.Compare(MyContext.GetContext().CountryCode, _owner, True) = 0) ' for the moment only only local packs to be deleted locally
    End Sub
    Protected Overrides Sub AddInsertCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Protected Overrides Sub AddUpdateCommandFields(ByVal command As SqlCommand)
        AddCommandFields(command)
    End Sub
    Private Sub AddCommandFields(ByVal command As SqlCommand)
        command.Parameters.AddWithValue("@OWNER", Owner)
        command.Parameters.AddWithValue("@INTERNALCODE", Code)
        command.Parameters.AddWithValue("@LOCALCODE", LocalCode)
        command.Parameters.AddWithValue("@SHORTNAME", Name)
        command.Parameters.AddWithValue("@DESCRIPTION", Description)
        command.Parameters.AddWithValue("@SORTORDER", Index)
        command.Parameters.AddWithValue("@STATUSID", _status)
        command.Parameters.AddWithValue("@PRICE", Price)
        command.Parameters.AddWithValue("@PRICEVAT", VatPrice)
        command.Parameters.AddWithValue("@CURRENCY", MyContext.GetContext().Currency.Code)
        command.Parameters.AddWithValue("@MODELID", Generation.Model.ID)
        command.Parameters.AddWithValue("@GENERATIONID", Generation.ID)
    End Sub
    Protected Overrides Sub UpdateChildren(ByVal transaction As System.Data.SqlClient.SqlTransaction)
        If Not _assetSet Is Nothing Then _assetSet.Update(transaction)
        If Not _equipment Is Nothing Then _equipment.Update(transaction)
        If Not _rules Is Nothing Then _rules.Update(transaction)
        If Not _extColourApplicabilities Is Nothing Then _extColourApplicabilities.Update(transaction)
        If Not _upholsteryApplicabilities Is Nothing Then _upholsteryApplicabilities.Update(transaction)
        If Not _masterPacks Is Nothing Then _masterPacks.Update(transaction)
        If Not _accentColourCombinations Is Nothing Then _accentColourCombinations.Update(transaction)
        MyBase.UpdateChildren(transaction)
    End Sub
#End Region
#Region " Base Object Overrides"
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
            Return Entity.PACK
        End Get
    End Property
#End Region
End Class